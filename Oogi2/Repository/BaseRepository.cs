using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Oogi2.BulkSupport;
using Oogi2.Entities;
using Oogi2.Queries;

namespace Oogi2
{
    class BaseRepository<T> where T : BaseEntity, new()
    {
        readonly IConnection _connection;

        internal BaseRepository(IConnection connection)
        {
            _connection = connection;
        }

        internal async Task<T> GetFirstOrDefaultHelperAsync()
        {
            var qq = new SqlQuerySpecQuery<T>();
            var sqlq = qq.ToGetFirstOrDefault(new T());            
            var sq = sqlq.ToSqlQuery();

            return await _connection.QueryOneItemAsync<T>(sq);
        }

        internal async Task<T> GetFirstOrDefaultHelperAsync(IQuery query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            QueryDefinition sqlq = query.ToGetFirstOrDefault();            
            var sq = sqlq.ToSqlQuery();

            return await _connection.QueryOneItemAsync<T>(sq);
        }

        internal async Task<T> GetFirstOrDefaultHelperAsync(IQuery<T> query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            QueryDefinition sqlq = query.ToGetFirstOrDefault(new T());
            var sq = sqlq.ToSqlQuery();

            return await _connection.QueryOneItemAsync<T>(sq);
        }

        internal Task<T> CreateDocumentAsync(T entity) => _connection.CreateItemAsync(entity);

        internal Task<T> ReplaceDocumentAsync(T entity) => _connection.ReplaceItemAsync(entity);

        internal Task<T> UpsertDocumentAsync(T entity) => _connection.UpsertItemAsync(entity);

        internal Task<bool> DeleteDocumentAsync(T entity) => _connection.DeleteItemAsync(entity);

        internal Task<bool> DeleteDocumentAsync(string id, string partitionKey = null) => _connection.DeleteItemAsync<dynamic>(id, partitionKey);

        public Task<List<T>> GetListHelperAsync(IQuery query, QueryRequestOptions requestOptions)
        {
            var sq = query.ToQueryDefinition().ToSqlQuery();
            return _connection.QueryMoreItemsAsync<T>(sq, requestOptions);
        }

        public Task<List<T>> GetListHelperAsync(IQuery<T> query, QueryRequestOptions requestOptions)
        {
            var sq = query.ToQueryDefinition(new T()).ToSqlQuery();
            return _connection.QueryMoreItemsAsync<T>(sq, requestOptions);
        }

        internal Task<BulkOperationResponse<T>> ProcessBulkOperationsAsync(List<BulkOperation<T>> bulkOperations)
        {
            return _connection.ProcessBulkOperationsAsync<T>(bulkOperations);
        }
    }
}