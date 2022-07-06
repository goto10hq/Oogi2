using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using Microsoft.Azure.Cosmos;
using Oogi2.Helpers;
using System.IO;
using System.Text;
using Oogi2.Exceptions;
using System.Reflection;
using Sushi2;
using Oogi2.Entities;
using System.Dynamic;
using Oogi2.BulkSupport;

namespace Oogi2
{
    /// <summary>
    /// Connection.
    /// </summary>
    public class Connection : IConnection
    {
        /// <summary>
        /// Gets the cosmos client.
        /// </summary>
        /// <value>The cosmos client.</value>
        public CosmosClient Client { get; }

        /// <summary>
        /// Gets the database identifier.
        /// </summary>
        /// <value>The database identifier.</value>
        public string DatabaseId { get; }

        public PartitionKey PartitionKey { get; set; }

        /// <summary>
        /// Gets the collection identifier.
        /// </summary>
        /// <value>The collection identifier.</value>
        public string ContainerId { get; }

        public Container Container { get; }

        public Database Database { get; }

        static readonly CustomSerializer _serializer = new CustomSerializer();

        static Connection()
        {
            Tools.SetJsonDefaultSettings();
        }
        
        public Connection(string endpoint, string authorizationKey, string database, string container, string partitionKey, bool allowBulkExecution) : this(endpoint, authorizationKey, database, container, partitionKey, new CosmosClientOptions 
        {
            Serializer = new CustomSerializer(),
            AllowBulkExecution = allowBulkExecution
        })
        {            
        }
        
        public Connection(string endpoint, string authorizationKey, string database, string container, string partitionKey = null, CosmosClientOptions options = null)
        {
            CosmosClientOptions clientOptions = options ?? new CosmosClientOptions()
            {
                Serializer = new CustomSerializer()
            };

            Client = new CosmosClient(endpoint, authorizationKey, clientOptions);
            DatabaseId = database;
            ContainerId = container;
            PartitionKey = string.IsNullOrEmpty(partitionKey) ? PartitionKey.None : new PartitionKey(partitionKey);

            Database = Client.GetDatabase(database);

            if (PartitionKey != PartitionKey.None)
            {
                AsyncTools.RunSync(() => Database.CreateContainerIfNotExistsAsync(container, partitionKey));
            }

            Container = Client.GetContainer(database, container);
        }

        /// <summary>
        /// Upserts the json document(s) directly.
        /// </summary>
        /// <returns>A list of documents.</returns>
        /// <param name="jsonString">Json string.</param>
        public async Task<List<T>> UpsertJsonAsync<T>(string jsonString)
        {
            if (jsonString == null)
                throw new ArgumentNullException(nameof(jsonString));

            var byteArray = Encoding.UTF8.GetBytes(jsonString);

            using (var stream = new MemoryStream(byteArray))
            {
                using (var responseMessage = await Container.CreateItemStreamAsync(stream, PartitionKey))
                {
                    if (responseMessage.StatusCode != HttpStatusCode.Created && responseMessage.StatusCode != HttpStatusCode.OK)
                        throw new OogiException($"UpsertJsonAsync failed with status code {responseMessage.StatusCode}.");

                    return _serializer.FromStream<IList<T>>(responseMessage.Content).ToList();
                }
            }
        }

        public async Task<T> QueryOneItemAsync<T>(string query)
        {
            using (FeedIterator<T> setIterator = Container.GetItemQueryIterator<T>(query, requestOptions: new QueryRequestOptions { MaxItemCount = 1 }))
            {
                while (setIterator.HasMoreResults)
                {
                    foreach (T item in await setIterator.ReadNextAsync())
                    {
                        return item;
                    }
                }
            }

            return default;
        }

        //public async Task<bool> DeleteContainerAsync()
        //{
        //    var responseMessage = await Container.DeleteContainerAsync();

        //    if (responseMessage.StatusCode != HttpStatusCode.NoContent)
        //        throw new OogiException($"DeleteContainerAsync failed with status code {responseMessage.StatusCode}.");

        //    return true;
        //}

        public async Task<bool> DeleteItemAsync<T>(string id, string partitionKey = null)
        {
            var responseMessage = await Container.DeleteItemAsync<T>(id, partitionKey == null ? PartitionKey.None : new PartitionKey(partitionKey));

            if (responseMessage.StatusCode != HttpStatusCode.NoContent)
                throw new OogiException($"DeleteItemAsync failed with status code {responseMessage.StatusCode}.");

            return true;
        }

        public async Task<bool> DeleteItemAsync<T>(T item)
        {
            var id = GetId(item);
            string partitionKey = null;

            if (PartitionKey != PartitionKey.None)
            {
                if (item is BaseEntity be)
                    partitionKey = be.PartitionKey;
                else
                    partitionKey = GetPartitionKey(item);
            }
            
            var responseMessage = await Container.DeleteItemAsync<T>(id, PartitionKey == PartitionKey.None ? PartitionKey.None : new PartitionKey(partitionKey));

            if (responseMessage.StatusCode != HttpStatusCode.NoContent)
                throw new OogiException($"DeleteItemAsync failed with status code {responseMessage.StatusCode}.");

            return true;            
        }

        public async Task<T> UpsertItemAsync<T>(T item)
        {
            SetId(item);

            var responseMessage = await Container.UpsertItemAsync<T>(item);

            if (responseMessage.StatusCode != HttpStatusCode.Created)
                throw new OogiException($"UpsertItemAsync failed with status code {responseMessage.StatusCode}.");

            return responseMessage.Resource;
        }

        public async Task<T> CreateItemAsync<T>(T item)
        {
            if (!SetId(item))
            {
                var d = SetIdToDynamic(item);
                
                var responseMessageD = await Container.CreateItemAsync<T>(d);

                if (responseMessageD.StatusCode != HttpStatusCode.Created)
                    throw new OogiException($"CreateItemAsync failed with status code {responseMessageD.StatusCode}.");

                return responseMessageD.Resource;
            }                                  

            var responseMessage = await Container.CreateItemAsync(item);

            if (responseMessage.StatusCode != HttpStatusCode.Created)
                throw new OogiException($"CreateItemAsync failed with status code {responseMessage.StatusCode}.");

            return responseMessage.Resource;
        }

        public async Task<T> ReplaceItemAsync<T>(T item)
        {
            var id = GetId(item);
            
            var responseMessage = await Container.ReplaceItemAsync<T>(item, id);

            if (responseMessage.StatusCode != HttpStatusCode.OK)
                throw new OogiException($"ReplaceItemAsync failed with status code {responseMessage.StatusCode}.");

            return responseMessage.Resource;
        }

        public async Task<List<T>> QueryMoreItemsAsync<T>(string query, QueryRequestOptions requestOptions)
        {
            List<T> results = new List<T>();

            using (FeedIterator<T> setIterator = Container.GetItemQueryIterator<T>(
                query,
                requestOptions: requestOptions))
            {
                while (setIterator.HasMoreResults)
                {
                    FeedResponse<T> response = await setIterator.ReadNextAsync();
                    results.AddRange(response);
                }
            }

            return results;
        }        

        internal static bool SetId<T>(T entity)
        {
            PropertyInfo propInfoId = typeof(T).GetProperty("Id");

            if (propInfoId != null)
            {
                if (propInfoId.GetValue(entity) == null)
                    propInfoId.SetValue(entity, Guid.NewGuid().ToString());

                return true;
            }

            return false;
        }

        internal static string GetId<T>(T entity)
        {
            PropertyInfo propInfoId = typeof(T).GetProperty("Id");

            if (propInfoId != null)
            {
                return propInfoId.GetValue(entity).ToString();
            }
            else
            {                
                foreach (System.ComponentModel.PropertyDescriptor property in System.ComponentModel.TypeDescriptor.GetProperties(entity.GetType()))
                {
                    if (property.Name == "Id")
                    {
                        return property.GetValue(entity)?.ToString();                        
                    }                    
                }

                return null;
            }
        }

        internal static string GetPartitionKey<T>(T entity)
        {
            PropertyInfo propInfoPartitionKey = typeof(T).GetProperty("PartitionKey");

            if (propInfoPartitionKey != null)
            {
                return propInfoPartitionKey.GetValue(entity)?.ToString();
            }
            else
            {
                foreach (System.ComponentModel.PropertyDescriptor property in System.ComponentModel.TypeDescriptor.GetProperties(entity.GetType()))
                {
                    if (property.Name == "PartitionKey")
                    {
                        return property.GetValue(entity)?.ToString();
                    }
                }

                return null;
            }            
        }

        internal static dynamic SetIdToDynamic(object value)
        {
            IDictionary<string, object> expando = new ExpandoObject();

            foreach (System.ComponentModel.PropertyDescriptor property in System.ComponentModel.TypeDescriptor.GetProperties(value.GetType()))
            {
                if (property.Name == "Id")
                {
                    if (property.GetValue(value) == null)
                    {
                        expando.Add("Id", Guid.NewGuid());
                        continue;
                    }                    
                }

                expando.Add(property.Name, property.GetValue(value));
            }

            if (expando.Keys.All(x => x != "Id"))
                expando.Add("Id", Guid.NewGuid());

            return expando as ExpandoObject;
        }

        public Task<BulkOperationResponse<T>> ProcessBulkOperationsAsync<T>(List<BulkOperation<T>> bulkOperations)
        {
            BulkOperations<T> bo = new BulkOperations<T>(bulkOperations.Count());

            foreach(var op in bulkOperations)
            {
                bo.Tasks.Add(BulkSupport.Helpers.CaptureOperationResponse(op.Operation, op.Item));
            }

            return bo.ExecuteAsync();
        }
    }
}