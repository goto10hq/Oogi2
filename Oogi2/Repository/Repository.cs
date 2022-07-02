using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Oogi2.Entities;
using Oogi2.Queries;

namespace Oogi2
{
    public class Repository<T> where T : BaseEntity, new()
    {
        readonly BaseRepository<T> _repository;

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="connection">Connection.</param>
        public Repository(IConnection connection)
        {
            _repository = new BaseRepository<T>(connection);
        }

        /// <summary>
        /// Gets the first or default document.
        /// </summary>
        /// <param name="query">Query.</param>        
        /// <returns>The first document.</returns>
        public Task<T> GetFirstOrDefaultAsync(QueryDefinition query = null)
        {
            return _repository.GetFirstOrDefaultHelperAsync(new SqlQuerySpecQuery<T>(query));
        }

        /// <summary>
        /// Gets the first or default document.
        /// </summary>
        /// <param name="query">Query.</param>        
        /// <returns>The first document.</returns>
        public Task<T> GetFirstOrDefaultAsync(DynamicQuery<T> query)
        {
            return _repository.GetFirstOrDefaultHelperAsync(query);
        }

        /// <summary>
        /// Gets the first or default document.
        /// </summary>
        /// <param name="query">Query.</param>
        /// <param name="parameters">Sql parameters.</param>        
        /// <returns>The first document.</returns>
        public Task<T> GetFirstOrDefaultAsync(string query, object parameters)
        {
            return _repository.GetFirstOrDefaultHelperAsync(new DynamicQuery<T>(query, parameters));
        }

        /// <summary>
        /// Gets the first or default document.
        /// </summary>
        /// <param name="id">The id of the document.</param>        
        /// <returns>The first document.</returns>
        public Task<T> GetFirstOrDefaultAsync(string id)
        {
            return _repository.GetFirstOrDefaultHelperAsync(new IdQuery<T>(id));
        }

        /// <summary>
        /// Upserts the document.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <param name="requestOptions">Request options.</param>
        /// <returns>The document.</returns>
        public Task<T> UpsertAsync(T entity)
        {
            return _repository.UpsertDocumentAsync(entity);
        }

        /// <summary>
        /// Creates the document.
        /// </summary>
        /// <param name="entity">Entity.</param>        
        /// <returns>The document.</returns>
        public Task<T> CreateAsync(T entity)
        {
            return _repository.CreateDocumentAsync(entity);

        }

        /// <summary>
        /// Replaces the document.
        /// </summary>
        /// <param name="entity">Entity.</param>        
        /// <returns>The document.</returns>
        public Task<T> ReplaceAsync(T entity)
        {
            return _repository.ReplaceDocumentAsync(entity);
        }

        /// <summary>
        /// Delete the specified id.
        /// </summary>
        /// <param name="id">The id of the document.</param>        
        /// <returns><c>true</c> if document has been deleted; otherwise, <c>false</c>.</returns>
        public Task<bool> DeleteAsync(string id)
        {
            return _repository.DeleteDocumentAsync(id);
        }

        /// <summary>
        /// Delete the specified id.
        /// </summary>
        /// <param name="entity">Entity.</param>        
        /// <returns><c>true</c> if document has been deleted; otherwise, <c>false</c>.</returns>
        public Task<bool> DeleteAsync(T entity)
        {
            return _repository.DeleteDocumentAsync(entity);
        }

        /// <summary>
        /// Gets all documents.
        /// </summary>
        /// <param name="requestOptions">Request options.</param>
        /// <returns>The documents.</returns>
        public Task<List<T>> GetAllAsync(QueryRequestOptions requestOptions = null)
        {
            var query = new SqlQuerySpecQuery<T>();
            var sq = query.ToGetAll(new T());

            return _repository.GetListHelperAsync(new SqlQuerySpecQuery<T>(sq), requestOptions);
        }

        /// <summary>
        /// Gets the list of documents.
        /// </summary>
        /// <param name="query">Query.</param>
        /// <param name="requestOptions">Request options.</param>
        /// <returns>The documents.</returns>
        public Task<List<T>> GetListAsync(QueryDefinition query, QueryRequestOptions requestOptions = null)
        {
            return _repository.GetListHelperAsync(new SqlQuerySpecQuery<T>(query), requestOptions);
        }

        /// <summary>
        /// Gets the list of documents.
        /// </summary>
        /// <param name="query">Query.</param>
        /// <param name="requestOptions">Feed options.</param>
        /// <returns>The documents.</returns>
        public Task<List<T>> GetListAsync(DynamicQuery query, QueryRequestOptions requestOptions = null)
        {
            return _repository.GetListHelperAsync(query, requestOptions);
        }

        /// <summary>
        /// Gets the list of documents.
        /// </summary>
        /// <param name="query">Query.</param>
        /// <param name="parameters">Sql parameters</param>
        /// <param name="requestOptions">Feed options.</param>
        /// <returns>The documents.</returns>
        public Task<List<T>> GetListAsync(string query, object parameters = null, QueryRequestOptions requestOptions = null)
        {
            return _repository.GetListHelperAsync(new DynamicQuery(query, parameters), requestOptions);
        }
    }
}