using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Oogi2.Queries;

namespace Oogi2
{
    public class CommonRepository<T> where T : class
    {
        readonly IConnection _connection;

        public CommonRepository(IConnection connection)
        {
            _connection = connection;
        }

        public Task<T> GetFirstOrDefaultAsync(DynamicQuery query)
        {
            QueryDefinition sqlq;

            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            else
            {
                sqlq = query.ToGetFirstOrDefault();
            }

            var sq = sqlq.ToSqlQuery();

            return _connection.QueryOneItemAsync<T>(sq);
        }

        public Task<T> GetFirstOrDefaultAsync(string query, object parameters)
        {
            return GetFirstOrDefaultAsync(new DynamicQuery(query, parameters));
        }

        public Task<T> GetFirstOrDefaultAsync(string id)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            var dq = new DynamicQuery("select top 1 * from c where c.id = @id", new { id });

            return _connection.QueryOneItemAsync<T>(dq.ToSqlQuery());
        }

        public Task<T> UpsertAsync(T entity, ItemRequestOptions requestOptions = null) => _connection.UpsertItemAsync(entity, requestOptions);

        public Task<T> CreateAsync(T entity, ItemRequestOptions requestOptions = null) => _connection.CreateItemAsync(entity, requestOptions);

        public Task<T> ReplaceAsync(T entity, ItemRequestOptions requestOptions = null) => _connection.ReplaceItemAsync(entity, requestOptions);

        public Task<bool> DeleteAsync(string id, string partitionKey = null, ItemRequestOptions requestOptions = null) => _connection.DeleteItemAsync<dynamic>(id, partitionKey, requestOptions);

        public Task<bool> DeleteAsync(T item, ItemRequestOptions requestOptions = null) => _connection.DeleteItemAsync(item, requestOptions);

        public async Task<List<T>> GetListAsync(DynamicQuery query, QueryRequestOptions requestOptions = null)
        {
            var sq = query.ToQueryDefinition().ToSqlQuery();
            return await _connection.QueryMoreItemsAsync<T>(sq, requestOptions);
        }

        public async Task<List<T>> GetListAsync(string query, object parameters, QueryRequestOptions requestOptions = null)
        {
            var sq = new DynamicQuery(query, parameters).ToQueryDefinition().ToSqlQuery();
            return await _connection.QueryMoreItemsAsync<T>(sq, requestOptions);
        }

        public Task<T> PatchAsync(string id, string partitionKey, List<PatchOperation> patches, string filterPredicate = null) => _connection.PatchItemAsync<T>(id, partitionKey, patches, filterPredicate);

        public Task<T> PatchAsync(string id, List<PatchOperation> patches, string filterPredicate = null) => _connection.PatchItemAsync<T>(id, null, patches, filterPredicate);

        public Task<T> PatchAsync(T entity, List<PatchOperation> patches, string filterPredicate = null) => _connection.PatchItemAsync(entity, patches, filterPredicate);

        public Task<T> PatchAsync(string id, string partitionKey, List<PatchOperation> patches, PatchItemRequestOptions requestOptions) => _connection.PatchItemAsync<T>(id, partitionKey, patches, requestOptions);

        public Task<T> PatchAsync(string id, List<PatchOperation> patches, PatchItemRequestOptions requestOptions) => _connection.PatchItemAsync<T>(id, null, patches, requestOptions);

        public Task<T> PatchAsync(T entity, List<PatchOperation> patches, PatchItemRequestOptions requestOptions) => _connection.PatchItemAsync(entity, patches, requestOptions);
    }
}