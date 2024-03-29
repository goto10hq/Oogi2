﻿using System;
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

        internal Task<T> CreateItemAsync(T entity, ItemRequestOptions requestOptions = null) => _connection.CreateItemAsync(entity, requestOptions);

        internal Task<T> ReplaceItemAsync(T entity, ItemRequestOptions requestOptions = null) => _connection.ReplaceItemAsync(entity, requestOptions);

        internal Task<T> UpsertItemAsync(T entity, ItemRequestOptions requestOptions = null) => _connection.UpsertItemAsync(entity, requestOptions);

        internal Task<bool> DeleteItemAsync(T entity, ItemRequestOptions requestOptions = null) => _connection.DeleteItemAsync(entity, requestOptions);

        internal Task<bool> DeleteItemAsync(string id, string partitionKey = null, ItemRequestOptions requestOptions = null) => _connection.DeleteItemAsync<dynamic>(id, partitionKey, requestOptions);

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

        internal Task<BulkOperationResponse<T>> ProcessBulkOperationsAsync(List<BulkOperation<T>> bulkOperations, int? dop = null) => _connection.ProcessBulkOperationsAsync<T>(bulkOperations, dop);

        public Task<T> PatchAsync(string id, string partitionKey, List<PatchOperation> patches, string filterPredicate = null) => _connection.PatchItemAsync<T>(id, partitionKey, patches, filterPredicate);

        public Task<T> PatchAsync(string id, List<PatchOperation> patches, string filterPredicate = null) => _connection.PatchItemAsync<T>(id, null, patches, filterPredicate);

        public Task<T> PatchAsync(T entity, List<PatchOperation> patches, string filterPredicate = null) => _connection.PatchItemAsync(entity, patches, filterPredicate);

        public Task<T> PatchAsync(string id, string partitionKey, List<PatchOperation> patches, PatchItemRequestOptions requestOptions = null) => _connection.PatchItemAsync<T>(id, partitionKey, patches, requestOptions);

        public Task<T> PatchAsync(string id, List<PatchOperation> patches, PatchItemRequestOptions requestOptions = null) => _connection.PatchItemAsync<T>(id, null, patches, requestOptions);

        public Task<T> PatchAsync(T entity, List<PatchOperation> patches, PatchItemRequestOptions requestOptions = null) => _connection.PatchItemAsync(entity, patches, requestOptions);

        public Task<T> PatchAsync(string id, string partitionKey, PatchOperation patch, string filterPredicate = null) => _connection.PatchItemAsync<T>(id, partitionKey, new List<PatchOperation> { patch }, filterPredicate);

        public Task<T> PatchAsync(string id, PatchOperation patch, string filterPredicate = null) => _connection.PatchItemAsync<T>(id, null, new List<PatchOperation> { patch }, filterPredicate);

        public Task<T> PatchAsync(T entity, PatchOperation patch, string filterPredicate = null) => _connection.PatchItemAsync(entity, new List<PatchOperation> { patch }, filterPredicate);

        public Task<T> PatchAsync(string id, string partitionKey, PatchOperation patch, PatchItemRequestOptions requestOptions = null) => _connection.PatchItemAsync<T>(id, partitionKey, new List<PatchOperation> { patch }, requestOptions);

        public Task<T> PatchAsync(string id, PatchOperation patch, PatchItemRequestOptions requestOptions = null) => _connection.PatchItemAsync<T>(id, null, new List<PatchOperation> { patch }, requestOptions);

        public Task<T> PatchAsync(T entity, PatchOperation patch, PatchItemRequestOptions requestOptions = null) => _connection.PatchItemAsync(entity, new List<PatchOperation> { patch }, requestOptions);
    }
}