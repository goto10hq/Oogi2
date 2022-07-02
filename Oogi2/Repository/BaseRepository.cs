using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
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

        internal async Task<T> GetFirstOrDefaultHelperAsync(IQuery<T> query = null)
        {
            QueryDefinition sqlq;

            if (query == null)
            {
                var qq = new SqlQuerySpecQuery<T>();
                sqlq = qq.ToGetFirstOrDefault(new T());
            }
            else
            {
                sqlq = query.ToGetFirstOrDefault(new T());
            }

            var sq = sqlq.ToSqlQuery();

            return await _connection.QueryOneItemAsync<T>(sq);
        }

        internal async Task<T> CreateDocumentAsync(T entity)
        {
            return await _connection.CreateItemAsync(entity).ConfigureAwait(false);
        }

        internal async Task<T> ReplaceDocumentAsync(T entity)
        {
            return await _connection.ReplaceItemAsync(entity).ConfigureAwait(false);
        }

        internal async Task<T> UpsertDocumentAsync(T entity)
        {
            return await _connection.UpsertItemAsync(entity).ConfigureAwait(false);
        }

        internal async Task<bool> DeleteDocumentAsync(T entity)
        {
            return await _connection.DeleteItemAsync(entity).ConfigureAwait(false);
        }

        internal async Task<bool> DeleteDocumentAsync(string id)
        {
            return await _connection.DeleteItemAsync<T>(id).ConfigureAwait(false);
        }

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
    }
}