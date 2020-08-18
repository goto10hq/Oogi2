using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Oogi2.Queries;
using Sushi2;

namespace Oogi2
{
    public class Repository
    {
        readonly BaseRepository<dynamic> _repository;

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="connection">Connection.</param>
        public Repository(IConnection connection)
        {
            _repository = new BaseRepository<dynamic>(connection);
        }

        /// <summary>
        /// Gets the first or default document.
        /// </summary>
        /// <param name="query">Query.</param>
        /// <param name="feedOptions">Feed options.</param>
        /// <returns>The first document or null.</returns>
        public Task<dynamic> GetFirstOrDefaultAsync(SqlQuerySpec query = null, FeedOptions feedOptions = null)
        {
            return _repository.GetFirstOrDefaultHelperAsync(new SqlQuerySpecQuery<dynamic>(query), feedOptions);
        }

        /// <summary>
        /// Gets the first or default document.
        /// </summary>
        /// <param name="query">Query.</param>
        /// <param name="feedOptions">Feed options.</param>
        /// <returns>The first document or null.</returns>
        public Task<dynamic> GetFirstOrDefaultAsync(DynamicQuery query, FeedOptions feedOptions = null)
        {
            return _repository.GetFirstOrDefaultHelperAsync(query, feedOptions);
        }

        /// <summary>
        /// Gets the first or default document.
        /// </summary>
        /// <param name="query">Query.</param>
        /// <param name="parameters">Sql parameters.</param>
        /// <param name="feedOptions">Feed options.</param>
        /// <returns>The first document or null.</returns>
        public Task<dynamic> GetFirstOrDefaultAsync(string query, object parameters, FeedOptions feedOptions = null)
        {
            return _repository.GetFirstOrDefaultHelperAsync(new DynamicQuery<dynamic>(query, parameters), feedOptions);
        }

        /// <summary>
        /// Gets the first or default document.
        /// </summary>
        /// <param name="query">Query.</param>
        /// <param name="feedOptions">Feed options.</param>
        /// <returns>The first document or null.</returns>
        public dynamic GetFirstOrDefault(SqlQuerySpec query = null, FeedOptions feedOptions = null)
        {
            return AsyncTools.RunSync(() => GetFirstOrDefaultAsync(query, feedOptions));
        }

        /// <summary>
        /// Gets the first or default document.
        /// </summary>
        /// <param name="query">Query.</param>
        /// <param name="feedOptions">Feed options.</param>
        /// <returns>The first document or null.</returns>
        public dynamic GetFirstOrDefault(DynamicQuery query, FeedOptions feedOptions = null)
        {
            return AsyncTools.RunSync(() => GetFirstOrDefaultAsync(query, feedOptions));
        }

        /// <summary>
        /// Gets the first or default document.
        /// </summary>
        /// <param name="sql">Query.</param>
        /// <param name="parameters">Sql parameters.</param>
        /// <param name="feedOptions">Feed options.</param>
        /// <returns>The first document or null.</returns>
        public dynamic GetFirstOrDefault(string sql, object parameters, FeedOptions feedOptions = null)
        {
            return AsyncTools.RunSync(() => GetFirstOrDefaultAsync(new DynamicQuery<dynamic>(sql, parameters).ToSqlQuerySpec(), feedOptions));
        }

        /// <summary>
        /// Gets the first or default document.
        /// </summary>
        /// <param name="id">The id of the document.</param>
        /// <param name="feedOptions">Feed options.</param>
        /// <returns>The first document or null.</returns>
        public Task<dynamic> GetFirstOrDefaultAsync(string id, FeedOptions feedOptions = null)
        {
            return _repository.GetFirstOrDefaultHelperAsync(new IdQuery<dynamic>(id), feedOptions);
        }

        /// <summary>
        /// Gets the first or default document.
        /// </summary>
        /// <param name="id">The id of the document.</param>
        /// <param name="feedOptions">Feed options.</param>
        /// <returns>The first document or null.</returns>
        public dynamic GetFirstOrDefault(string id, FeedOptions feedOptions = null)
        {
            return AsyncTools.RunSync(() => GetFirstOrDefaultAsync(id, feedOptions));
        }

        /// <summary>
        /// Upserts the document.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <param name="requestOptions">Request options.</param>
        /// <returns>The document.</returns>
        public async Task<dynamic> UpsertAsync(dynamic entity, RequestOptions requestOptions = null)
        {
            return (dynamic)await _repository.UpsertDocumentAsync(entity, requestOptions).ConfigureAwait(false);
        }

        /// <summary>
        /// Upserts the document.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <param name="requestOptions">Request options.</param>
        /// <returns>The document.</returns>
        public dynamic Upsert(dynamic entity, RequestOptions requestOptions = null)
        {
            return (dynamic)AsyncTools.RunSync<dynamic>(() => UpsertAsync(entity, requestOptions));
        }

        /// <summary>
        /// Creates the document.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <param name="requestOptions">Request options.</param>
        /// <returns>The document.</returns>
        public async Task<dynamic> CreateAsync(dynamic entity, RequestOptions requestOptions = null)
        {
            return (dynamic)await _repository.CreateDocumentAsync(entity, requestOptions).ConfigureAwait(false);
        }

        /// <summary>
        /// Creates the document.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <param name="requestOptions">Request options.</param>
        /// <returns>The document.</returns>
        public dynamic Create(dynamic entity, RequestOptions requestOptions = null)
        {
            return (dynamic)AsyncTools.RunSync<dynamic>(() => CreateAsync(entity, requestOptions));
        }

        /// <summary>
        /// Replaces the document.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <param name="requestOptions">Request options.</param>
        /// <returns>The document.</returns>
        public async Task<dynamic> ReplaceAsync(dynamic entity, RequestOptions requestOptions = null)
        {
            return (dynamic)await _repository.ReplaceDocumentAsync(entity, requestOptions).ConfigureAwait(false);
        }

        /// <summary>
        /// Replaces the document.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <param name="requestOptions">Request options.</param>
        /// <returns>The document.</returns>
        public dynamic Replace(dynamic entity, RequestOptions requestOptions = null)
        {
            return (dynamic)AsyncTools.RunSync<dynamic>(() => ReplaceAsync(entity, requestOptions));
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
        public async Task<bool> DeleteAsync(dynamic entity, RequestOptions requestOptions = null)
        {
            return (dynamic)await _repository.DeleteDocumentAsync(entity, requestOptions);
        }

        /// <summary>
        /// Delete the specified id.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <param name="requestOptions">Request options.</param>
        /// <returns><c>true</c> if document has been deleted; otherwise, <c>false</c>.</returns>
        public bool Delete(dynamic entity, RequestOptions requestOptions = null)
        {
            return AsyncTools.RunSync<bool>(() => DeleteAsync(BaseRepository<dynamic>.GetId(entity), requestOptions));
        }

        /// <summary>
        /// Gets all documents.
        /// </summary>
        /// <param name="feedOptions"></param>
        /// <returns>The documents.</returns>
        Task<IList<dynamic>> GetAllAsync(FeedOptions feedOptions = null)
        {
            var query = new SqlQuerySpecQuery<dynamic>();
            var sq = query.ToGetAll();

            return _repository.GetListHelperAsync(new SqlQuerySpecQuery<dynamic>(sq), feedOptions);
        }

        /// <summary>
        /// Gets all documents.
        /// </summary>
        /// <param name="feedOptions">Feed options.</param>
        /// <returns>The documents.</returns>
        public IList<dynamic> GetAll(FeedOptions feedOptions = null)
        {
            return AsyncTools.RunSync(() => GetAllAsync(feedOptions));
        }

        /// <summary>
        /// Gets the list of documents.
        /// </summary>
        /// <param name="query">Query.</param>
        /// <param name="feedOptions">Feed options.</param>
        /// <returns>The documents.</returns>
        public Task<IList<dynamic>> GetListAsync(SqlQuerySpec query, FeedOptions feedOptions = null)
        {
            return _repository.GetListHelperAsync(new SqlQuerySpecQuery<dynamic>(query), feedOptions);
        }

        /// <summary>
        /// Gets the list of documents.
        /// </summary>
        /// <param name="query">Query.</param>
        /// <param name="feedOptions">Feed options.</param>
        /// <returns>The documents.</returns>
        public Task<IList<dynamic>> GetListAsync(DynamicQuery query, FeedOptions feedOptions = null)
        {
            return _repository.GetListHelperAsync(query, feedOptions);
        }

        /// <summary>
        /// Gets the list of documents.
        /// </summary>
        /// <param name="query">Query.</param>
        /// <param name="parameters">Sql parameters.</param>
        /// <param name="feedOptions">Feed options.</param>
        /// <returns>The documents.</returns>
        public Task<IList<dynamic>> GetListAsync(string query, object parameters, FeedOptions feedOptions = null)
        {
            return _repository.GetListHelperAsync(new DynamicQuery<dynamic>(query, parameters), feedOptions);
        }

        /// <summary>
        /// Gets the list of documents.
        /// </summary>
        /// <param name="query">Query.</param>
        /// <param name="feedOptions">Feed options.</param>
        /// <returns>The documents.</returns>
        public IList<dynamic> GetList(SqlQuerySpec query, FeedOptions feedOptions = null)
        {
            return AsyncTools.RunSync(() => GetListAsync(query, feedOptions));
        }

        /// <summary>
        /// Gets the list of documents.
        /// </summary>
        /// <param name="query">Query.</param>
        /// <param name="feedOptions">Feed options.</param>
        /// <returns>The documents.</returns>
        public IList<dynamic> GetList(DynamicQuery query, FeedOptions feedOptions = null)
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
        public IList<dynamic> GetList(string query, object parameters = null, FeedOptions feedOptions = null)
        {
            return AsyncTools.RunSync(() => GetListAsync(new DynamicQuery<dynamic>(query, parameters).ToSqlQuerySpec(), feedOptions));
        }
    }
}