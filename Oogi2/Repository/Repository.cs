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
    public class Repository<T> where T : class
    {
        readonly BaseRepository<T> _repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Oogi2.Repository`1"/> class.
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
        public async Task<T> GetFirstOrDefaultAsync(SqlQuerySpec query = null)
        {
            return await _repository.GetFirstOrDefaultHelperAsync(new SqlQuerySpecQuery<T>(query));
        }

        /// <summary>
        /// Gets the first or default document.
        /// </summary>
        /// <returns>The first document.</returns>
        /// <param name="query">Query.</param>
        public async Task<T> GetFirstOrDefaultAsync(DynamicQuery query)
        {
            return await _repository.GetFirstOrDefaultHelperAsync(query);
        }

        /// <summary>
        /// Gets the first or default document.
        /// </summary>
        /// <returns>The first document.</returns>
        /// <param name="query">Query.</param>
        public async Task<T> GetFirstOrDefaultAsync(string query, object parameters)
        {
            return await _repository.GetFirstOrDefaultHelperAsync(new DynamicQuery<T>(query, parameters));
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
        public async Task<T> GetFirstOrDefaultAsync(string id)
        {            
            return await _repository.GetFirstOrDefaultHelperAsync(new IdQuery<T>(id));
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
        public async Task<T> UpsertAsync(T entity)
        {            
            var response = await _repository.UpsertDocumentAsync(entity);
            return response;
        }

        /// <summary>
        /// Upserts the document.
        /// </summary>
        /// <returns>The document.</returns>
        /// <param name="entity">Entity.</param>
        public T Upsert(T entity)
        {
            var response = AsyncTools.RunSync(() => UpsertAsync(entity));
            return response;
        }

        /// <summary>
        /// Creates the document.
        /// </summary>
        /// <returns>The document.</returns>
        /// <param name="entity">Entity.</param>
        public async Task<T> CreateAsync(T entity)
        {            
            var response = await _repository.CreateDocumentAsync(entity);
            return response;            
        }

        /// <summary>
        /// Creates the document.
        /// </summary>
        /// <returns>The document.</returns>
        /// <param name="entity">Entity.</param>
        public T Create(T entity)
        {
            var response = AsyncTools.RunSync(() => CreateAsync(entity));
            return response;
        }

        /// <summary>
        /// Replaces the document.
        /// </summary>
        /// <returns>The document.</returns>
        /// <param name="entity">Entity.</param>
        public async Task<T> ReplaceAsync(T entity)
        {            
            var response = await _repository.ReplaceDocumentAsync(entity);
            return response;
        }

        /// <summary>
        /// Replaces the document.
        /// </summary>
        /// <returns>The document.</returns>
        /// <param name="entity">Entity.</param>
        public T Replace(T entity)
        {
            var response = AsyncTools.RunSync(() => ReplaceAsync(entity));
            return response;
        }

        /// <summary>
        /// Delete the specified id.
        /// </summary>
        /// <returns><c>true</c> if document has been deleted; otherwise, <c>false</c>.</returns>
        /// <param name="id">The id of the document.</param>
        public async Task<bool> DeleteAsync(string id)
        {            
            var response = await _repository.DeleteDocumentAsync(id);
            return response;            
        }

        /// <summary>
        /// Delete the specified id.
        /// </summary>
        /// <returns><c>true</c> if document has been deleted; otherwise, <c>false</c>.</returns>
        /// <param name="id">The id of the document.</param>
        public bool Delete(string id)
        {
            var response = AsyncTools.RunSync(() => DeleteAsync(id));
            return response;
        }

        /// <summary>
        /// Delete the specified id.
        /// </summary>
        /// <returns><c>true</c> if document has been deleted; otherwise, <c>false</c>.</returns>
        /// <param name="entity">Entity.</param>
        public async Task<bool> DeleteAsync(T entity)
        {            
            var response = await _repository.DeleteDocumentAsync(entity);
            return response;
        }

        /// <summary>
        /// Delete the specified id.
        /// </summary>
        /// <returns><c>true</c> if document has been deleted; otherwise, <c>false</c>.</returns>
        /// <param name="entity">Entity.</param>
        public bool Delete(T entity)
        {
            var response = AsyncTools.RunSync(() => DeleteAsync(BaseRepository<T>.GetId(entity)));
            return response;
        }

        /// <summary>
        /// Gets all documents.
        /// </summary>
        /// <returns>The documents.</returns>
        public async Task<IList<T>> GetAllAsync()
        {            
            var query = new SqlQuerySpecQuery<T>();
            var sq = query.ToGetAll(); 

            return await _repository.GetListHelperAsync(new SqlQuerySpecQuery<T>(sq));
        }

        /// <summary>
        /// Gets all documents.
        /// </summary>
        /// <returns>The documents.</returns>
        public IList<T> GetAll()
        {
            var response = AsyncTools.RunSync(GetAllAsync);
            return response;
        }

        /// <summary>
        /// Gets the list of documents.
        /// </summary>
        /// <returns>The documents.</returns>
        /// <param name="query">Query.</param>
        public async Task<IList<T>> GetListAsync(SqlQuerySpec query)
        {
            return await _repository.GetListHelperAsync(new SqlQuerySpecQuery<T>(query));
        }

        /// <summary>
        /// Gets the list of documents.
        /// </summary>
        /// <returns>The documents.</returns>
        /// <param name="query">Query.</param>
        public async Task<IList<T>> GetListAsync(DynamicQuery query)
        {
            return await _repository.GetListHelperAsync(query);
        }

        /// <summary>
        /// Gets the list of documents.
        /// </summary>
        /// <returns>The documents.</returns>
        /// <param name="query">Query.</param>
        public async Task<IList<T>> GetListAsync(string query, object parameters)
        {
            return await _repository.GetListHelperAsync(new DynamicQuery<T>(query, parameters));
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
        public IList<T> GetList(string query, object parameters = null)
        {
            return AsyncTools.RunSync(() => GetListAsync(new DynamicQuery<T>(query, parameters).ToSqlQuerySpec()));
        }
    }
}
