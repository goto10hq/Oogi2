using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
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
        /// <returns>The first document.</returns>
        /// <param name="query">Query.</param>
        public Task<T> GetFirstOrDefaultAsync(SqlQuerySpec query = null)
        {
            return _repository.GetFirstOrDefaultHelperAsync(new SqlQuerySpecQuery<T>(query));
        }

        /// <summary>
        /// Gets the first or default document.
        /// </summary>
        /// <returns>The first document.</returns>
        /// <param name="query">Query.</param>
        public Task<T> GetFirstOrDefaultAsync(DynamicQuery query)
        {
            return _repository.GetFirstOrDefaultHelperAsync(query);
        }

        /// <summary>
        /// Gets the first or default document.
        /// </summary>
        /// <returns>The first document.</returns>
        /// <param name="query">Query.</param>
        /// <param name="parameters">Sql parameters.</param>
        public Task<T> GetFirstOrDefaultAsync(string query, object parameters)
        {
            return _repository.GetFirstOrDefaultHelperAsync(new DynamicQuery<T>(query, parameters));
        }

        /// <summary>
        /// Gets the first or default document.
        /// </summary>
        /// <returns>The first document.</returns>
        /// <param name="query">Query.</param>
        public T GetFirstOrDefault(SqlQuerySpec query = null)
        {
            return AsyncTools.RunSync(() => GetFirstOrDefaultAsync(query));
        }

        /// <summary>
        /// Gets the first or default document.
        /// </summary>
        /// <returns>The first document.</returns>
        /// <param name="query">Query.</param>
        public T GetFirstOrDefault(DynamicQuery query)
        {
            return AsyncTools.RunSync(() => GetFirstOrDefaultAsync(query));
        }

        /// <summary>
        /// Gets the first or default document.
        /// </summary>
        /// <returns>The first document.</returns>
        /// <param name="sql">Sql query.</param>
        /// <param name="parameters">Parameters.</param>
        public T GetFirstOrDefault(string sql, object parameters)
        {
            return AsyncTools.RunSync(() => GetFirstOrDefaultAsync(new DynamicQuery<T>(sql, parameters).ToSqlQuerySpec()));
        }

        /// <summary>
        /// Gets the first or default document.
        /// </summary>
        /// <returns>The first document.</returns>
        /// <param name="id">The id of the document.</param>
        public Task<T> GetFirstOrDefaultAsync(string id)
        {
            return _repository.GetFirstOrDefaultHelperAsync(new IdQuery<T>(id));
        }

        /// <summary>
        /// Gets the first or default document.
        /// </summary>
        /// <returns>The first document.</returns>
        /// <param name="id">The id of the document.</param>
        public T GetFirstOrDefault(string id)
        {
            return AsyncTools.RunSync(() => GetFirstOrDefaultAsync(id));
        }

        /// <summary>
        /// Upserts the document.
        /// </summary>
        /// <returns>The document.</returns>
        /// <param name="entity">Entity.</param>
        public Task<T> UpsertAsync(T entity)
        {
            return _repository.UpsertDocumentAsync(entity);
        }

        /// <summary>
        /// Upserts the document.
        /// </summary>
        /// <returns>The document.</returns>
        /// <param name="entity">Entity.</param>
        public T Upsert(T entity)
        {
            return AsyncTools.RunSync(() => UpsertAsync(entity));
        }

        /// <summary>
        /// Creates the document.
        /// </summary>
        /// <returns>The document.</returns>
        /// <param name="entity">Entity.</param>
        public Task<T> CreateAsync(T entity)
        {
            return _repository.CreateDocumentAsync(entity);
        }

        /// <summary>
        /// Creates the document.
        /// </summary>
        /// <returns>The document.</returns>
        /// <param name="entity">Entity.</param>
        public T Create(T entity)
        {
            return AsyncTools.RunSync(() => CreateAsync(entity));
        }

        /// <summary>
        /// Replaces the document.
        /// </summary>
        /// <returns>The document.</returns>
        /// <param name="entity">Entity.</param>
        public Task<T> ReplaceAsync(T entity)
        {
            return _repository.ReplaceDocumentAsync(entity);
        }

        /// <summary>
        /// Replaces the document.
        /// </summary>
        /// <returns>The document.</returns>
        /// <param name="entity">Entity.</param>
        public T Replace(T entity)
        {
            return AsyncTools.RunSync(() => ReplaceAsync(entity));
        }

        /// <summary>
        /// Delete the specified id.
        /// </summary>
        /// <returns><c>true</c> if document has been deleted; otherwise, <c>false</c>.</returns>
        /// <param name="id">The id of the document.</param>
        public Task<bool> DeleteAsync(string id)
        {
            return _repository.DeleteDocumentAsync(id);
        }

        /// <summary>
        /// Delete the specified id.
        /// </summary>
        /// <returns><c>true</c> if document has been deleted; otherwise, <c>false</c>.</returns>
        /// <param name="id">The id of the document.</param>
        public bool Delete(string id)
        {
            return AsyncTools.RunSync(() => DeleteAsync(id));
        }

        /// <summary>
        /// Delete the specified id.
        /// </summary>
        /// <returns><c>true</c> if document has been deleted; otherwise, <c>false</c>.</returns>
        /// <param name="entity">Entity.</param>
        public Task<bool> DeleteAsync(T entity)
        {
            return _repository.DeleteDocumentAsync(entity);
        }

        /// <summary>
        /// Delete the specified id.
        /// </summary>
        /// <returns><c>true</c> if document has been deleted; otherwise, <c>false</c>.</returns>
        /// <param name="entity">Entity.</param>
        public bool Delete(T entity)
        {
            return AsyncTools.RunSync(() => DeleteAsync(BaseRepository<T>.GetId(entity)));
        }

        /// <summary>
        /// Gets all documents.
        /// </summary>
        /// <returns>The documents.</returns>
        public Task<IList<T>> GetAllAsync()
        {
            var query = new SqlQuerySpecQuery<T>();
            var sq = query.ToGetAll();

            return _repository.GetListHelperAsync(new SqlQuerySpecQuery<T>(sq));
        }

        /// <summary>
        /// Gets all documents.
        /// </summary>
        /// <returns>The documents.</returns>
        public IList<T> GetAll()
        {
            return AsyncTools.RunSync(GetAllAsync);
        }

        /// <summary>
        /// Gets the list of documents.
        /// </summary>
        /// <returns>The documents.</returns>
        /// <param name="query">Query.</param>
        public Task<IList<T>> GetListAsync(SqlQuerySpec query)
        {
            return _repository.GetListHelperAsync(new SqlQuerySpecQuery<T>(query));
        }

        /// <summary>
        /// Gets the list of documents.
        /// </summary>
        /// <returns>The documents.</returns>
        /// <param name="query">Query.</param>
        public Task<IList<T>> GetListAsync(DynamicQuery query)
        {
            return _repository.GetListHelperAsync(query);
        }

        /// <summary>
        /// Gets the list of documents.
        /// </summary>
        /// <returns>The documents.</returns>
        /// <param name="query">Query.</param>
        /// <param name="parameters">Sql parameters</param>
        public Task<IList<T>> GetListAsync(string query, object parameters = null)
        {
            return _repository.GetListHelperAsync(new DynamicQuery<T>(query, parameters));
        }

        /// <summary>
        /// Gets the list of documents.
        /// </summary>
        /// <returns>The documents.</returns>
        /// <param name="query">Query.</param>
        public IList<T> GetList(SqlQuerySpec query)
        {
            return AsyncTools.RunSync(() => GetListAsync(query));
        }

        /// <summary>
        /// Gets the list of documents.
        /// </summary>
        /// <returns>The documents.</returns>
        /// <param name="query">Query.</param>
        public IList<T> GetList(DynamicQuery query)
        {
            return AsyncTools.RunSync(() => GetListAsync(query));
        }

        /// <summary>
        /// Gets the list of documents.
        /// </summary>
        /// <returns>The documents.</returns>
        /// <param name="query">Query.</param>
        /// <param name="parameters">Sql parameters.</param>
        public IList<T> GetList(string query, object parameters = null)
        {
            return AsyncTools.RunSync(() => GetListAsync(new DynamicQuery<T>(query, parameters).ToSqlQuerySpec()));
        }
    }
}