using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Oogi2.BulkSupport;
using Oogi2.Entities;
using Oogi2.Queries;

namespace Oogi2
{
    public class Repository<T> where T : BaseEntity, new()
    {
        readonly BaseRepository<T> _repository;

        public Repository(IConnection connection)
        {
            _repository = new BaseRepository<T>(connection);
        }

        public Task<T> GetFirstOrDefaultAsync(QueryDefinition query) => _repository.GetFirstOrDefaultHelperAsync(new SqlQuerySpecQuery(query));

        public Task<T> GetFirstOrDefaultAsync() => _repository.GetFirstOrDefaultHelperAsync(new SqlQuerySpecQuery<T>(null));

        public Task<T> GetFirstOrDefaultAsync(DynamicQuery query) => _repository.GetFirstOrDefaultHelperAsync(query);

        public Task<T> GetFirstOrDefaultAsync(string query, object parameters)
        {
            IQuery<T> q = new DynamicQuery<T>(query, parameters);
            return _repository.GetFirstOrDefaultHelperAsync(q);
        }

        public Task<T> GetFirstOrDefaultAsync(string id, string partitionKey = null) => _repository.GetFirstOrDefaultHelperAsync(new IdQuery<T>(id, partitionKey));

        public Task<T> UpsertAsync(T entity, ItemRequestOptions requestOptions = null) => _repository.UpsertItemAsync(entity, requestOptions);

        public Task<T> CreateAsync(T entity, ItemRequestOptions requestOptions = null) => _repository.CreateItemAsync(entity, requestOptions);

        public Task<T> ReplaceAsync(T entity, ItemRequestOptions requestOptions = null) => _repository.ReplaceItemAsync(entity, requestOptions);

        public Task<bool> DeleteAsync(string id, string partitionKey = null, ItemRequestOptions requestOptions = null) => _repository.DeleteItemAsync(id, partitionKey, requestOptions);

        public Task<bool> DeleteAsync(T entity, ItemRequestOptions requestOptions = null) => _repository.DeleteItemAsync(entity, requestOptions);

        public Task<List<T>> GetAllAsync(QueryRequestOptions requestOptions = null)
        {
            var query = new SqlQuerySpecQuery<T>();
            var sq = query.ToGetAll(new T());

            return _repository.GetListHelperAsync(new SqlQuerySpecQuery<T>(sq), requestOptions);
        }

        public Task<List<T>> GetListAsync(QueryDefinition query, QueryRequestOptions requestOptions = null) => _repository.GetListHelperAsync(new SqlQuerySpecQuery<T>(query), requestOptions);

        public Task<List<T>> GetListAsync(DynamicQuery query, QueryRequestOptions requestOptions = null) => _repository.GetListHelperAsync(query, requestOptions);

        public Task<List<T>> GetListAsync(string query, object parameters = null, QueryRequestOptions requestOptions = null) => _repository.GetListHelperAsync(new DynamicQuery(query, parameters), requestOptions);

        public Task<BulkOperationResponse<T>> ProcessBulkOperationsAsync(List<BulkOperation<T>> bulkOperations) => _repository.ProcessBulkOperationsAsync(bulkOperations);

        public Task<T> PatchAsync(string id, string partitionKey, List<PatchOperation> patches, string filterPredicate = null) => _repository.PatchAsync(id, partitionKey, patches, filterPredicate);

        public Task<T> PatchAsync(string id, List<PatchOperation> patches, string filterPredicate = null) => _repository.PatchAsync(id, null, patches, filterPredicate);

        public Task<T> PatchAsync(T entity, List<PatchOperation> patches, string filterPredicate = null) => _repository.PatchAsync(entity, patches, filterPredicate);

        public Task<T> PatchAsync(string id, string partitionKey, List<PatchOperation> patches, PatchItemRequestOptions requestOptions) => _repository.PatchAsync(id, partitionKey, patches, requestOptions);

        public Task<T> PatchAsync(string id, List<PatchOperation> patches, PatchItemRequestOptions requestOptions) => _repository.PatchAsync(id, null, patches, requestOptions);

        public Task<T> PatchAsync(T entity, List<PatchOperation> patches, PatchItemRequestOptions requestOptions) => _repository.PatchAsync(entity, patches, requestOptions);
    }
}