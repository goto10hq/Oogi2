﻿using System;
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
    /// <summary>
    /// Connection.
    /// </summary>
    public class Connection : IConnection
    {
        /// <summary>
        /// Gets the client.
        /// </summary>
        /// <value>The client.</value>
        public DocumentClient Client { get; private set; }

        /// <summary>
        /// Gets the database identifier.
        /// </summary>
        /// <value>The database identifier.</value>
        public string DatabaseId { get; private set; }

        /// <summary>
        /// Gets the collection identifier.
        /// </summary>
        /// <value>The collection identifier.</value>
        public string CollectionId { get; private set; }

        /// <summary>
        /// Initializes the <see cref="T:Oogi2.Connection"/> class.
        /// Set default Json ser/deser settings.
        /// TODO: provide trought settings in ctor.
        /// </summary>
        static Connection()
        {
            Tools.SetJsonDefaultSettings();    
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Oogi2.Connection"/> class.
        /// </summary>
        /// <param name="endpoint">Endpoint.</param>
        /// <param name="authorizationKey">Authorization key.</param>
        /// <param name="database">Database.</param>
        /// <param name="collection">Collection.</param>
        /// <param name="connectionPolicy">Connection policy.</param>
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
        /// Upserts the json document(s) directly.
        /// </summary>
        /// <returns>Objects.</returns>
        /// <param name="jsonString">Json string.</param>
        public List<object> UpsertJson(string jsonString)
        {
            return AsyncTools.RunSync(() => UpsertJsonAsync(jsonString));
        }

		/// <summary>
		/// Upserts the json document(s) directly.
		/// </summary>
		/// <returns>Objects.</returns>
		/// <param name="jsonString">Json string.</param>
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
        /// Executes the query async.
        /// </summary>
        /// <returns>The query async.</returns>
        /// <param name="query">Query.</param>
        public async Task<object> ExecuteQueryAsync(string query)
        {            
            var q = Client.CreateDocumentQuery(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId), query).AsDocumentQuery();            
            var result = await Core.ExecuteWithRetriesAsync(() => QueryMoreDocumentsAsync(q));

            return result;
        }

         /// <summary>
         /// Executes the query.
         /// </summary>
         /// <returns>The query.</returns>
         /// <param name="query">Query.</param>
        public object ExecuteQuery(string query)
        {                        
            var result = AsyncTools.RunSync(() => ExecuteQueryAsync(query));

            return result;
        }

        /// <summary>
        /// Creates the stored procedure async.
        /// </summary>
        /// <returns>The stored procedure.</returns>
        /// <param name="storedProcedure">Stored procedure.</param>
        public async Task<StoredProcedure> CreateStoredProcedureAsync(StoredProcedure storedProcedure)
        {         
            StoredProcedure createdStoredProcedure = await Client.CreateStoredProcedureAsync(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId), storedProcedure);

            return createdStoredProcedure;
        }

        /// <summary>
        /// Reads the stored procedure async.
        /// </summary>
        /// <returns>The stored procedure.</returns>
        /// <param name="storedProcedureId">Stored procedure identifier.</param>
        public async Task<StoredProcedure> ReadStoredProcedureAsync(string storedProcedureId)
        {            
            StoredProcedure storedProcedure = await Client.ReadStoredProcedureAsync(UriFactory.CreateStoredProcedureUri(DatabaseId, CollectionId, storedProcedureId));

            return storedProcedure;
        }

        /// <summary>
        /// Reads the stored procedure.
        /// </summary>
        /// <returns>The stored procedure.</returns>
        /// <param name="storedProcedureId">Stored procedure identifier.</param>
        public StoredProcedure ReadStoredProcedure(string storedProcedureId)
        {
            StoredProcedure storedProcedure = AsyncTools.RunSync(() => Client.ReadStoredProcedureAsync(UriFactory.CreateStoredProcedureUri(DatabaseId, CollectionId, storedProcedureId)));

            return storedProcedure;
        }

        /// <summary>
        /// Deletes the stored procedure async.
        /// </summary>
        /// <returns>The stored procedure async.</returns>
        /// <param name="storedProcedureId">Stored procedure identifier.</param>
        public async Task<StoredProcedure> DeleteStoredProcedureAsync(string storedProcedureId)
        {
            StoredProcedure storedProcedure = await Client.DeleteStoredProcedureAsync(UriFactory.CreateStoredProcedureUri(DatabaseId, CollectionId, storedProcedureId));

            return storedProcedure;
        }

        /// <summary>
        /// Deletes the stored procedure.
        /// </summary>
        /// <returns>The stored procedure.</returns>
        /// <param name="storedProcedureId">Stored procedure identifier.</param>
        public StoredProcedure DeleteStoredProcedure(string storedProcedureId)
        {
            StoredProcedure storedProcedure = AsyncTools.RunSync(() => Client.DeleteStoredProcedureAsync(UriFactory.CreateStoredProcedureUri(DatabaseId, CollectionId, storedProcedureId)));

            return storedProcedure;
        }

        /// <summary>
        /// Creates the stored procedure.
        /// </summary>
        /// <returns>The stored procedure.</returns>
        /// <param name="storedProcedure">Stored procedure.</param>
        public StoredProcedure CreateStoredProcedure(StoredProcedure storedProcedure)
        {
            return AsyncTools.RunSync(() => CreateStoredProcedureAsync(storedProcedure));
        }

        /// <summary>
        /// Deletes the document.
        /// </summary>
        /// <returns>Document?</returns>
        /// <param name="id">Identifier.</param>
        /// <remarks>TODO: check up return type</remarks>
        public async Task<object> DeleteAsync(string id)
        {
            var response = await Core.ExecuteWithRetriesAsync(() => Client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id)));

            return response;
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
    }
}
