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

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="connection">Connection.</param>
        public CommonRepository(IConnection connection)
        {
            _connection = connection;
        }

        /// <summary>
        /// Gets the first or default document.
        /// </summary>
        /// <param name="query">Query.</param>        
        /// <returns>The first document or null.</returns>
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

        /// <summary>
        /// Gets the first or default document.
        /// </summary>
        /// <param name="query">Query.</param>
        /// <param name="parameters">Sql parameters.</param>        
        /// <returns>The first document or null.</returns>
        public Task<T> GetFirstOrDefaultAsync(string query, object parameters)
        {
            return GetFirstOrDefaultAsync(new DynamicQuery(query, parameters));
        }

        /// <summary>
        /// Gets the first or default document.
        /// </summary>
        /// <param name="id">The id of the document.</param>
        /// <returns>The first document or null.</returns>
        public Task<T> GetFirstOrDefaultAsync(string id)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            var dq = new DynamicQuery("select top 1 * from c where c.id = @id", new { id });

            return _connection.QueryOneItemAsync<T>(dq.ToSqlQuery());
        }

        /// <summary>
        /// Upserts the document.
        /// </summary>
        /// <param name="entity">Entity.</param>        
        /// <returns>The document.</returns>
        public async Task<T> UpsertAsync(T entity)
        {
            return await _connection.UpsertItemAsync(entity).ConfigureAwait(false);
        }

        /// <summary>
        /// Creates the document.
        /// </summary>
        /// <param name="entity">Entity.</param>        
        /// <returns>The document.</returns>
        public async Task<T> CreateAsync(T entity)
        {
            return await _connection.CreateItemAsync(entity).ConfigureAwait(false);
        }

        /// <summary>
        /// Replaces the document.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <returns>The document.</returns>
        public async Task<T> ReplaceAsync(T entity)
        {
            return await _connection.ReplaceItemAsync(entity).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete the specified id.
        /// </summary>
        /// <param name="id">The id of the document.</param>        
        /// <returns><c>true</c> if document has been deleted; otherwise, <c>false</c>.</returns>
        public async Task<bool> DeleteAsync(string id, string partitionKey = null)
        {
            return await _connection.DeleteItemAsync<dynamic>(id, partitionKey);
        }

        /// <summary>
        /// Delete the specified id.
        /// </summary>
        /// <param name="item">Entity.</param>        
        /// <returns><c>true</c> if document has been deleted; otherwise, <c>false</c>.</returns>
        public async Task<bool> DeleteAsync(T item)
        {
            return await _connection.DeleteItemAsync(item);
        }

        /// <summary>
        /// Gets the list of documents.
        /// </summary>
        /// <param name="query">Query.</param>
        /// <param name="requestOptions">Feed options.</param>        
        /// <returns>The documents.</returns>
        public async Task<List<T>> GetListAsync(DynamicQuery query, QueryRequestOptions requestOptions = null)
        {
            var sq = query.ToQueryDefinition().ToSqlQuery();
            return await _connection.QueryMoreItemsAsync<T>(sq, requestOptions);
        }

        /// <summary>
        /// Gets the list of documents.
        /// </summary>
        /// <param name="query">Query.</param>
        /// <param name="parameters">Sql parameters.</param>
        /// <param name="requestOptions">Feed options.</param>
        /// <returns>The documents.</returns>
        public async Task<List<T>> GetListAsync(string query, object parameters, QueryRequestOptions requestOptions = null)
        {
            var sq = new DynamicQuery(query, parameters).ToQueryDefinition().ToSqlQuery();
            return await _connection.QueryMoreItemsAsync<T>(sq, requestOptions);
        }
    }
}