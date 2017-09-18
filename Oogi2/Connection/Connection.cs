using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Sushi2;
using Microsoft.Azure.Documents.Linq;

namespace Oogi2
{
    public class Connection : IConnection
    {
        public DocumentClient Client { get; private set; }

        public string DatabaseId { get; private set; }
        public string CollectionId { get; private set; }

        static Connection()
        {
            Tools.SetJsonDefaultSettings();    
        }

        /// <summary>
        /// Ctor.
        /// </summary>
        public Connection(string endpoint, string authorizationKey, string database, string collection, ConnectionPolicy connectionPolicy = null)
        {
            var defaultConnectionPolicy = new ConnectionPolicy
                                   {                                       
                                       ConnectionMode = ConnectionMode.Direct,
                                       ConnectionProtocol = Protocol.Tcp
                                   };

            Client = new DocumentClient(new Uri(endpoint), authorizationKey, connectionPolicy ?? defaultConnectionPolicy);            
            DatabaseId = database;
            CollectionId = collection;
        }

        /// <summary>
        /// Upsert document(s) as pure json.
        /// </summary>
        public List<object> UpsertJson(string jsonString)
        {
            return AsyncTools.RunSync(() => UpsertJsonAsync(jsonString));
        }

        /// <summary>
        /// Upsert document(s) as pure json.
        /// </summary>
        public async Task<List<object>> UpsertJsonAsync(string jsonString)
        {
            if (jsonString == null)
                throw new ArgumentNullException(nameof(jsonString));

            var result = new List<object>();
            var docs = JsonConvert.DeserializeObject<List<object>>(jsonString);

            foreach (var doc in docs)
            {
                result.Add(await Core.ExecuteWithRetriesAsync(() => Client.UpsertDocumentAsync(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId), doc)));
            }

            return result;
        }

        /// <summary>
        /// Execute query.
        /// </summary>
        public async Task<object> ExecuteQueryAsync(string query)
        {            
            var q = Client.CreateDocumentQuery(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId), query).AsDocumentQuery();            
            var result = await Core.ExecuteWithRetriesAsync(() => QueryMoreDocumentsAsync(q));

            return result;
        }

        /// <summary>
        /// Execute query.
        /// </summary>
        public object ExecuteQuery(string query)
        {                        
            var result = AsyncTools.RunSync(() => ExecuteQueryAsync(query));

            return result;
        }

        /// <summary>
        /// Create store procedure.
        /// </summary>
        public async Task<StoredProcedure> CreateStoredProcedureAsync(StoredProcedure sp)
        {         
            StoredProcedure createdStoredProcedure = await Client.CreateStoredProcedureAsync(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId), sp);
            return createdStoredProcedure;
        }

        /// <summary>
        /// Read store procedure.
        /// </summary>
        public async Task<StoredProcedure> ReadStoredProcedureAsync(string storeProcedureId)
        {            
            StoredProcedure storedProcedure = await Client.ReadStoredProcedureAsync(UriFactory.CreateStoredProcedureUri(DatabaseId, CollectionId, storeProcedureId));
            return storedProcedure;
        }

        /// <summary>
        /// Read store procedure.
        /// </summary>
        public StoredProcedure ReadStoredProcedure(string storeProcedureId)
        {
            StoredProcedure storedProcedure = AsyncTools.RunSync(() => Client.ReadStoredProcedureAsync(UriFactory.CreateStoredProcedureUri(DatabaseId, CollectionId, storeProcedureId)));
            return storedProcedure;
        }

        /// <summary>
        /// Delete store procedure.
        /// </summary>
        public async Task<StoredProcedure> DeleteStoredProcedureAsync(string storeProcedureId)
        {
            StoredProcedure storedProcedure = await Client.DeleteStoredProcedureAsync(UriFactory.CreateStoredProcedureUri(DatabaseId, CollectionId, storeProcedureId));
            return storedProcedure;
        }

        /// <summary>
        /// Delete store procedure.
        /// </summary>
        public StoredProcedure DeleteStoredProcedure(string storeProcedureId)
        {
            StoredProcedure storedProcedure = AsyncTools.RunSync(() => Client.DeleteStoredProcedureAsync(UriFactory.CreateStoredProcedureUri(DatabaseId, CollectionId, storeProcedureId)));
            return storedProcedure;
        }

        /// <summary>
        /// Create store procedure.
        /// </summary>
        public StoredProcedure CreateStoredProcedure(StoredProcedure sp)
        {
            return AsyncTools.RunSync(() => CreateStoredProcedureAsync(sp));
        }              

        static async Task<List<dynamic>> QueryMoreDocumentsAsync(IDocumentQuery<dynamic> query)
        {
            var entitiesRetrieved = new List<dynamic>();

            while (query.HasMoreResults)
            {
                var queryResponse = await Core.ExecuteWithRetriesAsync(() => QuerySingleDocumentAsync(query));

                var entities = queryResponse.AsEnumerable();

                if (entities != null)
                    entitiesRetrieved.AddRange(entities);
            }

            return entitiesRetrieved;
        }

        static async Task<FeedResponse<dynamic>> QuerySingleDocumentAsync(IDocumentQuery<dynamic> query)
        {
            return await query.ExecuteNextAsync<dynamic>();
        }

        /// <summary>
        /// Delete document by id.
        /// </summary>
        public async Task<object> DeleteAsync(string id)
        {
            var response = await Core.ExecuteWithRetriesAsync(() => Client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id)));
            return response;
        }        
    }
}
