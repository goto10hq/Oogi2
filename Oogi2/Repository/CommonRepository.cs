using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Oogi2.Queries;
using Sushi2;

namespace Oogi2
{
    public class CommonRepository
    {
        readonly BaseRepository<dynamic> _repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Oogi2.Repository`1"/> class.
        /// </summary>
        /// <param name="connection">Connection.</param>
        public CommonRepository(IConnection connection)
        {
            _repository = new BaseRepository<dynamic>(connection);
        }

        /// <summary>
        /// Gets the first or default document.
        /// </summary>
        /// <returns>The first document.</returns>
        /// <param name="query">Query.</param>
        public async Task<dynamic> GetFirstOrDefaultAsync(SqlQuerySpec query = null)
        {
            return await _repository.GetFirstOrDefaultHelperAsync(new SqlQuerySpecQuery<dynamic>(query));
        }

        /// <summary>
        /// Gets the first or default document.
        /// </summary>
        /// <returns>The first document.</returns>
        /// <param name="query">Query.</param>
        public async Task<dynamic> GetFirstOrDefaultAsync(DynamicQuery query)
        {
            return await _repository.GetFirstOrDefaultHelperAsync(query);
        }

        /// <summary>
        /// Gets the first or default document.
        /// </summary>
        /// <returns>The first document.</returns>
        /// <param name="query">Query.</param>
        public async Task<dynamic> GetFirstOrDefaultAsync(string query, object parameters)
        {
            return await _repository.GetFirstOrDefaultHelperAsync(new DynamicQuery<dynamic>(query, parameters));
        }

        /// <summary>
        /// Gets the first or default document.
        /// </summary>
        /// <returns>The first document.</returns>
        /// <param name="query">Query.</param>
        public dynamic GetFirstOrDefault(SqlQuerySpec query = null)
        {
            return AsyncTools.RunSync(() => GetFirstOrDefaultAsync(query));
        }

        /// <summary>
        /// Gets the first or default document.
        /// </summary>
        /// <returns>The first document.</returns>
        /// <param name="query">Query.</param>
        public dynamic GetFirstOrDefault(DynamicQuery query)
        {
            return AsyncTools.RunSync(() => GetFirstOrDefaultAsync(query));
        }

        /// <summary>
        /// Gets the first or default document.
        /// </summary>
        /// <returns>The first document.</returns>
        /// <param name="sql">Sql query.</param>
        /// <param name="parameters">Parameters.</param>
        public dynamic GetFirstOrDefault(string sql, object parameters)
        {
            return AsyncTools.RunSync(() => GetFirstOrDefaultAsync(new DynamicQuery<dynamic>(sql, parameters).ToSqlQuerySpec()));
        }

        /// <summary>
        /// Gets the first or default document.
        /// </summary>
        /// <returns>The first document.</returns>
        /// <param name="id">The id of the document.</param>
        public async Task<dynamic> GetFirstOrDefaultAsync(string id)
        {
            return await _repository.GetFirstOrDefaultHelperAsync(new IdQuery<dynamic>(id));
        }

        /// <summary>
        /// Gets the first or default document.
        /// </summary>
        /// <returns>The first document.</returns>
        /// <param name="id">The id of the document.</param>
        public dynamic GetFirstOrDefault(string id)
        {
            return AsyncTools.RunSync(() => GetFirstOrDefaultAsync(id));
        }

        /// <summary>
        /// Upserts the document.
        /// </summary>
        /// <returns>The document.</returns>
        /// <param name="entity">Entity.</param>
        public async Task<dynamic> UpsertAsync(dynamic entity)
        {
            var response = await _repository.UpsertDocumentAsync(entity);
            return response;
        }

        /// <summary>
        /// Upserts the document.
        /// </summary>
        /// <returns>The document.</returns>
        /// <param name="entity">Entity.</param>
        public dynamic Upsert(dynamic entity)
        {
            var response = AsyncTools.RunSync<dynamic>(() => UpsertAsync(entity));
            return response;
        }

        /// <summary>
        /// Creates the document.
        /// </summary>
        /// <returns>The document.</returns>
        /// <param name="entity">Entity.</param>
        public async Task<dynamic> CreateAsync(dynamic entity)
        {
            var response = await _repository.CreateDocumentAsync(entity);
            return response;
        }

        /// <summary>
        /// Creates the document.
        /// </summary>
        /// <returns>The document.</returns>
        /// <param name="entity">Entity.</param>
        public dynamic Create(dynamic entity)
        {
            var response = AsyncTools.RunSync<dynamic>(() => CreateAsync(entity));
            return response;
        }

        /// <summary>
        /// Replaces the document.
        /// </summary>
        /// <returns>The document.</returns>
        /// <param name="entity">Entity.</param>
        public async Task<dynamic> ReplaceAsync(dynamic entity)
        {
            var response = await _repository.ReplaceDocumentAsync(entity);
            return response;
        }

        /// <summary>
        /// Replaces the document.
        /// </summary>
        /// <returns>The document.</returns>
        /// <param name="entity">Entity.</param>
        public dynamic Replace(dynamic entity)
        {
            var response = AsyncTools.RunSync<dynamic>(() => ReplaceAsync(entity));
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
        public async Task<bool> DeleteAsync(dynamic entity)
        {
            var response = await _repository.DeleteDocumentAsync(entity);
            return response;
        }

        /// <summary>
        /// Delete the specified id.
        /// </summary>
        /// <returns><c>true</c> if document has been deleted; otherwise, <c>false</c>.</returns>
        /// <param name="entity">Entity.</param>
        public bool Delete(dynamic entity)
        {
            var response = AsyncTools.RunSync<bool>(() => DeleteAsync(BaseRepository<dynamic>.GetId(entity)));
            return response;
        }

        /// <summary>
        /// Gets all documents.
        /// </summary>
        /// <returns>The documents.</returns>
        public async Task<IList<dynamic>> GetAllAsync()
        {
            var query = new SqlQuerySpecQuery<dynamic>();
            var sq = query.ToGetAll();

            return await _repository.GetListHelperAsync(new SqlQuerySpecQuery<dynamic>(sq));
        }

        /// <summary>
        /// Gets all documents.
        /// </summary>
        /// <returns>The documents.</returns>
        public IList<dynamic> GetAll()
        {
            var response = AsyncTools.RunSync(GetAllAsync);
            return response;
        }

        /// <summary>
        /// Gets the list of documents.
        /// </summary>
        /// <returns>The documents.</returns>
        /// <param name="query">Query.</param>
        public async Task<IList<dynamic>> GetListAsync(SqlQuerySpec query)
        {
            return await _repository.GetListHelperAsync(new SqlQuerySpecQuery<dynamic>(query));
        }

        /// <summary>
        /// Gets the list of documents.
        /// </summary>
        /// <returns>The documents.</returns>
        /// <param name="query">Query.</param>
        public async Task<IList<dynamic>> GetListAsync(DynamicQuery query)
        {
            return await _repository.GetListHelperAsync(query);
        }

        /// <summary>
        /// Gets the list of documents.
        /// </summary>
        /// <returns>The documents.</returns>
        /// <param name="query">Query.</param>
        public async Task<IList<dynamic>> GetListAsync(string query, object parameters)
        {
            return await _repository.GetListHelperAsync(new DynamicQuery<dynamic>(query, parameters));
        }

        /// <summary>
        /// Gets the list of documents.
        /// </summary>
        /// <returns>The documents.</returns>
        /// <param name="query">Query.</param>
        public IList<dynamic> GetList(SqlQuerySpec query)
        {
            return AsyncTools.RunSync(() => GetListAsync(query));
        }

        /// <summary>
        /// Gets the list of documents.
        /// </summary>
        /// <returns>The documents.</returns>
        /// <param name="query">Query.</param>
        public IList<dynamic> GetList(DynamicQuery query)
        {
            return AsyncTools.RunSync(() => GetListAsync(query));
        }

        /// <summary>
        /// Gets the list of documents.
        /// </summary>
        /// <returns>The documents.</returns>
        /// <param name="query">Query.</param>
        public IList<dynamic> GetList(string query, object parameters)
        {
            return AsyncTools.RunSync(() => GetListAsync(new DynamicQuery<dynamic>(query, parameters).ToSqlQuerySpec()));
        }
    }
}
