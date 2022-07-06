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

        public Task<T> UpsertAsync(T entity) => _repository.UpsertItemAsync(entity);

        public Task<T> CreateAsync(T entity) => _repository.CreateItemAsync(entity);

        public Task<T> ReplaceAsync(T entity) => _repository.ReplaceItemAsync(entity);

        public Task<bool> DeleteAsync(string id, string partitionKey = null) => _repository.DeleteItemAsync(id, partitionKey);

        public Task<bool> DeleteAsync(T entity) => _repository.DeleteItemAsync(entity);

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

        public Task<T> PatchAsync(string id, string partitionKey, List<PatchOperation> patches)
        {
            return _repository.PatchAsync(id, partitionKey, patches);
        }

        public Task<T> PatchAsync(string id, List<PatchOperation> patches)
        {
            return _repository.PatchAsync(id, null, patches);
        }

        public Task<T> PatchAsync(T entity, List<PatchOperation> patches)
        {
            return _repository.PatchAsync(entity, patches);
        }
    }
}