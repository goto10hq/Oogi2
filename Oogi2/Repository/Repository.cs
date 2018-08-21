using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Oogi2.Queries;
using Sushi2;

namespace Oogi2
{
    /// <summary>
    /// Repository.
    /// </summary>
    /// <typeparam name="T">Entity type.</typeparam>
    public class Repository<T> where T : class
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
        /// <param name="feedOptions">Feed options.</param>
        /// <returns>The first document.</returns>
        public Task<T> GetFirstOrDefaultAsync(SqlQuerySpec query = null, FeedOptions feedOptions = null)
        {
            return _repository.GetFirstOrDefaultHelperAsync(new SqlQuerySpecQuery<T>(query), feedOptions);
        }

        /// <summary>
        /// Gets the first or default document.
        /// </summary>
        /// <param name="query">Query.</param>
        /// <param name="feedOptions">Feed options.</param>
        /// <returns>The first document.</returns>
        public Task<T> GetFirstOrDefaultAsync(DynamicQuery query, FeedOptions feedOptions = null)
        {
            return _repository.GetFirstOrDefaultHelperAsync(query, feedOptions);
        }

        /// <summary>
        /// Gets the first or default document.
        /// </summary>
        /// <param name="query">Query.</param>
        /// <param name="parameters">Sql parameters.</param>
        /// <param name="feedOptions">Feed options.</param>
        /// <returns>The first document.</returns>
        public Task<T> GetFirstOrDefaultAsync(string query, object parameters, FeedOptions feedOptions = null)
        {
            return _repository.GetFirstOrDefaultHelperAsync(new DynamicQuery<T>(query, parameters), feedOptions);
        }

        /// <summary>
        /// Gets the first or default document.
        /// </summary>
        /// <param name="query">Query.</param>
        /// <param name="feedOptions">Feed options.</param>
        /// <returns>The first document.</returns>
        public T GetFirstOrDefault(SqlQuerySpec query = null, FeedOptions feedOptions = null)
        {
            return AsyncTools.RunSync(() => GetFirstOrDefaultAsync(query, feedOptions));
        }

        /// <summary>
        /// Gets the first or default document.
        /// </summary>
        /// <param name="query">Query.</param>
        /// <param name="feedOptions">Feed options.</param>
        /// <returns>The first document.</returns>
        public T GetFirstOrDefault(DynamicQuery query, FeedOptions feedOptions = null)
        {
            return AsyncTools.RunSync(() => GetFirstOrDefaultAsync(query, feedOptions));
        }

        /// <summary>
        /// Gets the first or default document.
        /// </summary>
        /// <param name="sql">Sql query.</param>
        /// <param name="parameters">Parameters.</param>
        /// <param name="feedOptions">Feed options.</param>
        /// <returns>The first document.</returns>
        public T GetFirstOrDefault(string sql, object parameters, FeedOptions feedOptions = null)
        {
            return AsyncTools.RunSync(() => GetFirstOrDefaultAsync(new DynamicQuery<T>(sql, parameters).ToSqlQuerySpec(), feedOptions));
        }

        /// <summary>
        /// Gets the first or default document.
        /// </summary>
        /// <param name="id">The id of the document.</param>
        /// <param name="feedOptions">Feed options.</param>
        /// <returns>The first document.</returns>
        public Task<T> GetFirstOrDefaultAsync(string id, FeedOptions feedOptions = null)
        {
            return _repository.GetFirstOrDefaultHelperAsync(new IdQuery<T>(id), feedOptions);
        }

        /// <summary>
        /// Gets the first or default document.
        /// </summary>
        /// <param name="id">The id of the document.</param>
        /// <param name="requestOptions">Request options.</param>
        /// <returns>The first document.</returns>
        public T GetFirstOrDefault(string id, RequestOptions requestOptions = null)
        {
            return AsyncTools.RunSync(() => GetFirstOrDefaultAsync(id, requestOptions));
        }

        /// <summary>
        /// Upserts the document.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <param name="requestOptions">Request options.</param>
        /// <returns>The document.</returns>
        public Task<T> UpsertAsync(T entity, RequestOptions requestOptions = null)
        {
            return _repository.UpsertDocumentAsync(entity, requestOptions);
        }

        /// <summary>
        /// Upserts the document.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <returns>The document.</returns>
        public T Upsert(T entity, RequestOptions requestOptions = null)
        {
            return AsyncTools.RunSync(() => UpsertAsync(entity, requestOptions));
        }

        /// <summary>
        /// Creates the document.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <param name="requestOptions">Request options.</param>
        /// <returns>The document.</returns>
        public Task<T> CreateAsync(T entity, RequestOptions requestOptions = null)
        {
            return _repository.CreateDocumentAsync(entity, requestOptions);
        }

        /// <summary>
        /// Creates the document.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <param name="requestOptions">Request options.</param>
        /// <returns>The document.</returns>
        public T Create(T entity, RequestOptions requestOptions = null)
        {
            return AsyncTools.RunSync(() => CreateAsync(entity, requestOptions));
        }

        /// <summary>
        /// Replaces the document.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <param name="requestOptions">Request options.</param>
        /// <returns>The document.</returns>
        public Task<T> ReplaceAsync(T entity, RequestOptions requestOptions = null)
        {
            return _repository.ReplaceDocumentAsync(entity, requestOptions);
        }

        /// <summary>
        /// Replaces the document.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <param name="requestOptions">Request options.</param>
        /// <returns>The document.</returns>
        public T Replace(T entity, RequestOptions requestOptions = null)
        {
            return AsyncTools.RunSync(() => ReplaceAsync(entity, requestOptions));
        }

        /// <summary>
        /// Delete the specified id.
        /// </summary>
        /// <param name="id">The id of the document.</param>
        /// <param name="requestOptions">Request options.</param>
        /// <returns><c>true</c> if document has been deleted; otherwise, <c>false</c>.</returns>
        public Task<bool> DeleteAsync(string id, RequestOptions requestOptions = null)
        {
            return _repository.DeleteDocumentAsync(id, requestOptions);
        }

        /// <summary>
        /// Delete the specified id.
        /// </summary>
        /// <param name="id">The id of the document.</param>
        /// <param name="requestOptions">Request options.</param>
        /// <returns><c>true</c> if document has been deleted; otherwise, <c>false</c>.</returns>
        public bool Delete(string id, RequestOptions requestOptions = null)
        {
            return AsyncTools.RunSync(() => DeleteAsync(id, requestOptions));
        }

        /// <summary>
        /// Delete the specified id.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <param name="requestOptions">Request options.</param>
        /// <returns><c>true</c> if document has been deleted; otherwise, <c>false</c>.</returns>
        public Task<bool> DeleteAsync(T entity, RequestOptions requestOptions = null)
        {
            return _repository.DeleteDocumentAsync(entity, requestOptions);
        }

        /// <summary>
        /// Delete the specified id.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <param name="requestOptions">Request options.</param>
        /// <returns><c>true</c> if document has been deleted; otherwise, <c>false</c>.</returns>
        public bool Delete(T entity, RequestOptions requestOptions = null)
        {
            return AsyncTools.RunSync(() => DeleteAsync(BaseRepository<T>.GetId(entity), requestOptions));
        }

        /// <summary>
        /// Gets all documents.
        /// </summary>
        /// <param name="feedOptions">Feed options.</param>
        /// <returns>The documents.</returns>
        public Task<IList<T>> GetAllAsync(FeedOptions feedOptions = null)
        {
            var query = new SqlQuerySpecQuery<T>();
            var sq = query.ToGetAll();

            return _repository.GetListHelperAsync(new SqlQuerySpecQuery<T>(sq), feedOptions);
        }

        /// <summary>
        /// Gets all documents.
        /// </summary>
        /// <param name="feedOptions">Feed options.</param>
        /// <returns>The documents.</returns>
        public IList<T> GetAll(FeedOptions feedOptions = null)
        {
            return AsyncTools.RunSync(() => GetAllAsync(feedOptions));
        }

        /// <summary>
        /// Gets the list of documents.
        /// </summary>
        /// <param name="query">Query.</param>
        /// <param name="feedOptions">Feed options.</param>
        /// <returns>The documents.</returns>
        public Task<IList<T>> GetListAsync(SqlQuerySpec query, FeedOptions feedOptions = null)
        {
            return _repository.GetListHelperAsync(new SqlQuerySpecQuery<T>(query), feedOptions);
        }

        /// <summary>
        /// Gets the list of documents.
        /// </summary>
        /// <param name="query">Query.</param>
        /// <param name="feedOptions">Feed options.</param>
        /// <returns>The documents.</returns>
        public Task<IList<T>> GetListAsync(DynamicQuery query, FeedOptions feedOptions = null)
        {
            return _repository.GetListHelperAsync(query, feedOptions);
        }

        /// <summary>
        /// Gets the list of documents.
        /// </summary>
        /// <param name="query">Query.</param>
        /// <param name="parameters">Sql parameters</param>
        /// <param name="feedOptions">Feed options.</param>
        /// <returns>The documents.</returns>
        public Task<IList<T>> GetListAsync(string query, object parameters = null, FeedOptions feedOptions = null)
        {
            return _repository.GetListHelperAsync(new DynamicQuery<T>(query, parameters), feedOptions);
        }

        /// <summary>
        /// Gets the list of documents.
        /// </summary>
        /// <param name="query">Query.</param>
        /// <param name="feedOptions">Feed options.</param>
        /// <returns>The documents.</returns>
        public IList<T> GetList(SqlQuerySpec query, FeedOptions feedOptions = null)
        {
            return AsyncTools.RunSync(() => GetListAsync(query, feedOptions));
        }

        /// <summary>
        /// Gets the list of documents.
        /// </summary>
        /// <param name="query">Query.</param>
        /// <param name="feedOptions">Feed options.</param>
        /// <returns>The documents.</returns>
        public IList<T> GetList(DynamicQuery query, FeedOptions feedOptions)
        {
            return AsyncTools.RunSync(() => GetListAsync(query, feedOptions));
        }

        /// <summary>
        /// Gets the list of documents.
        /// </summary>
        /// <param name="query">Query.</param>
        /// <param name="parameters">Sql parameters.</param>
        /// <param name="feedOptions">Feed options.</param>
        /// <returns>The documents.</returns>
        public IList<T> GetList(string query, object parameters = null, FeedOptions feedOptions = null)
        {
            return AsyncTools.RunSync(() => GetListAsync(new DynamicQuery<T>(query, parameters).ToSqlQuerySpec(), feedOptions));
        }
    }
}