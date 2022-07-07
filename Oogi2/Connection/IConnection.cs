using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Oogi2.BulkSupport;

namespace Oogi2
{
    /// <summary>
    /// Connection interface.
    /// </summary>
    public interface IConnection
    {
        /// <summary>
        /// Gets the cosmos client.
        /// </summary>
        /// <value>The cosmos client.</value>
        CosmosClient Client { get; }

        /// <summary>
        /// Gets the database identifier.
        /// </summary>
        /// <value>The database identifier.</value>
        string DatabaseId { get; }

        /// <summary>
        /// Gets the collection identifier.
        /// </summary>
        /// <value>The collection identifier.</value>
        string ContainerId { get; }

        Task<T> QueryOneItemAsync<T>(string query);
        Task<List<T>> QueryMoreItemsAsync<T>(string query, QueryRequestOptions requestOptions);
        Task<T> UpsertItemAsync<T>(T item);
        Task<T> CreateItemAsync<T>(T item);
        Task<T> ReplaceItemAsync<T>(T item);
        Task<bool> DeleteItemAsync<T>(T item);
        Task<bool> DeleteItemAsync<T>(string id, string partitionKey = null);
        Task<T> PatchItemAsync<T>(string id, string partitionKey, List<PatchOperation> patches, string filterPredicate = null);
        Task<T> PatchItemAsync<T>(T item, List<PatchOperation> patches, string filterPredicate = null);
        Task<BulkOperationResponse<T>> ProcessBulkOperationsAsync<T>(List<BulkOperation<T>> bulkOperations);
        TransactionalBatch CreateTransactionalBatch(string partitionKey = null);
    }
}
