//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.Azure.Documents;
//using Microsoft.Azure.Documents.Client;
//using Microsoft.Azure.Documents.Linq;
//using Oogi2.Queries;
//using Sushi2;
//using System.Dynamic;
//using System.Net;

//namespace Oogi2
//{
//    /// <summary>
//    /// Repository.
//    /// </summary>
//    public class CommonRepository
//    {
//        /// <summary>
//        /// Initializes a new instance of the <see cref="T:Oogi2.Repository`"/> class.
//        /// </summary>
//        /// <param name="connection">Connection.</param>
//        public Repository(IConnection connection)
//        {
//            _repository = new Repository<dynamic>(connection);
//        }

//        /// <summary>
//        /// Gets the first or default document.
//        /// </summary>
//        /// <returns>The first document.</returns>
//        /// <param name="query">Query.</param>
//        public async Task<T> GetFirstOrDefaultAsync(SqlQuerySpec query = null)
//        {
//            return await GetFirstOrDefaultHelperAsync(new SqlQuerySpecQuery<T>(query));
//        }

//        /// <summary>
//        /// Gets the first or default document.
//        /// </summary>
//        /// <returns>The first document.</returns>
//        /// <param name="query">Query.</param>
//        public async Task<T> GetFirstOrDefaultAsync(DynamicQuery query)
//        {
//            return await GetFirstOrDefaultHelperAsync(query);
//        }

//        /// <summary>
//        /// Gets the first or default document.
//        /// </summary>
//        /// <returns>The first document.</returns>
//        /// <param name="query">Query.</param>
//        public async Task<T> GetFirstOrDefaultAsync(string query, object parameters)
//        {
//            return await GetFirstOrDefaultHelperAsync(new DynamicQuery<T>(query, parameters));
//        }

//        /// <summary>
//        /// Gets the first or default document.
//        /// </summary>
//        /// <returns>The first document.</returns>
//        /// <param name="query">Query.</param>
//        public T GetFirstOrDefault(SqlQuerySpec query = null)
//        {
//            return AsyncTools.RunSync(() => GetFirstOrDefaultAsync(query));
//        }

//        /// <summary>
//        /// Gets the first or default document.
//        /// </summary>
//        /// <returns>The first document.</returns>
//        /// <param name="query">Query.</param>
//        public T GetFirstOrDefault(DynamicQuery query)
//        {
//            return AsyncTools.RunSync(() => GetFirstOrDefaultAsync(query));
//        }

//        /// <summary>
//        /// Gets the first or default document.
//        /// </summary>
//        /// <returns>The first document.</returns>
//        /// <param name="sql">Sql query.</param>
//        /// <param name="parameters">Parameters.</param>
//        public T GetFirstOrDefault(string sql, object parameters)
//        {
//            return AsyncTools.RunSync(() => GetFirstOrDefaultAsync(new DynamicQuery<T>(sql, parameters).ToSqlQuerySpec()));
//        }

//        /// <summary>
//        /// Gets the first or default document.
//        /// </summary>
//        /// <returns>The first document.</returns>
//        /// <param name="id">The id of the document.</param>
//        public async Task<T> GetFirstOrDefaultAsync(string id)
//        {            
//            return await GetFirstOrDefaultHelperAsync(new IdQuery<T>(id));
//        }

//        /// <summary>
//        /// Gets the first or default document.
//        /// </summary>
//        /// <returns>The first document.</returns>
//        /// <param name="id">The id of the document.</param>
//        public T GetFirstOrDefault(string id)
//        {
//            return AsyncTools.RunSync(() => GetFirstOrDefaultAsync(id));
//        }

//        /// <summary>
//        /// Upserts the document.
//        /// </summary>
//        /// <returns>The document.</returns>
//        /// <param name="entity">Entity.</param>
//        public async Task<T> UpsertAsync(T entity)
//        {            
//            var response = await UpsertDocumentAsync(entity);
//            return response;
//        }

//        /// <summary>
//        /// Upserts the document.
//        /// </summary>
//        /// <returns>The document.</returns>
//        /// <param name="entity">Entity.</param>
//        public T Upsert(T entity)
//        {
//            var response = AsyncTools.RunSync(() => UpsertAsync(entity));
//            return response;
//        }

//        /// <summary>
//        /// Creates the document.
//        /// </summary>
//        /// <returns>The document.</returns>
//        /// <param name="entity">Entity.</param>
//        public async Task<T> CreateAsync(T entity)
//        {            
//            var response = await CreateDocumentAsync(entity);
//            return response;            
//        }

//        /// <summary>
//        /// Creates the document.
//        /// </summary>
//        /// <returns>The document.</returns>
//        /// <param name="entity">Entity.</param>
//        public T Create(T entity)
//        {
//            var response = AsyncTools.RunSync(() => CreateAsync(entity));
//            return response;
//        }

//        /// <summary>
//        /// Replaces the document.
//        /// </summary>
//        /// <returns>The document.</returns>
//        /// <param name="entity">Entity.</param>
//        public async Task<T> ReplaceAsync(T entity)
//        {            
//            var response = await ReplaceDocumentAsync(entity);
//            return response;
//        }

//        /// <summary>
//        /// Replaces the document.
//        /// </summary>
//        /// <returns>The document.</returns>
//        /// <param name="entity">Entity.</param>
//        public T Replace(T entity)
//        {
//            var response = AsyncTools.RunSync(() => ReplaceAsync(entity));
//            return response;
//        }

//        /// <summary>
//        /// Delete the specified id.
//        /// </summary>
//        /// <returns><c>true</c> if document has been deleted; otherwise, <c>false</c>.</returns>
//        /// <param name="id">The id of the document.</param>
//        public async Task<bool> DeleteAsync(string id)
//        {            
//            var response = await DeleteDocumentAsync(id);
//            return response;            
//        }

//        /// <summary>
//        /// Delete the specified id.
//        /// </summary>
//        /// <returns><c>true</c> if document has been deleted; otherwise, <c>false</c>.</returns>
//        /// <param name="id">The id of the document.</param>
//        public bool Delete(string id)
//        {
//            var response = AsyncTools.RunSync(() => DeleteAsync(id));
//            return response;
//        }

//        /// <summary>
//        /// Delete the specified id.
//        /// </summary>
//        /// <returns><c>true</c> if document has been deleted; otherwise, <c>false</c>.</returns>
//        /// <param name="entity">Entity.</param>
//        public async Task<bool> DeleteAsync(T entity)
//        {            
//            var response = await DeleteDocumentAsync(entity);
//            return response;
//        }

//        /// <summary>
//        /// Delete the specified id.
//        /// </summary>
//        /// <returns><c>true</c> if document has been deleted; otherwise, <c>false</c>.</returns>
//        /// <param name="entity">Entity.</param>
//        public bool Delete(T entity)
//        {
//            var response = AsyncTools.RunSync(() => DeleteAsync(GetId(entity)));
//            return response;
//        }

//        /// <summary>
//        /// Gets all documents.
//        /// </summary>
//        /// <returns>The documents.</returns>
//        public async Task<IList<T>> GetAllAsync()
//        {            
//            var query = new SqlQuerySpecQuery<T>();
//            var sq = query.ToGetAll().ToSqlQuery(); 
//            var q = _connection.Client.CreateDocumentQuery<T>(UriFactory.CreateDocumentCollectionUri(_connection.DatabaseId, _connection.CollectionId), sq).AsDocumentQuery();
            
//            var response = await QueryMoreDocumentsAsync(q);
//            return response;
//        }

//        /// <summary>
//        /// Gets all documents.
//        /// </summary>
//        /// <returns>The documents.</returns>
//        public IList<T> GetAll()
//        {
//            var response = AsyncTools.RunSync(GetAllAsync);
//            return response;
//        }

//        /// <summary>
//        /// Gets the list of documents.
//        /// </summary>
//        /// <returns>The documents.</returns>
//        /// <param name="query">Query.</param>
//        public async Task<IList<T>> GetListAsync(SqlQuerySpec query)
//        {
//            return await GetListHelperAsync(new SqlQuerySpecQuery<T>(query));
//        }

//        /// <summary>
//        /// Gets the list of documents.
//        /// </summary>
//        /// <returns>The documents.</returns>
//        /// <param name="query">Query.</param>
//        public async Task<IList<T>> GetListAsync(DynamicQuery query)
//        {
//            return await GetListHelperAsync(query);
//        }

//        /// <summary>
//        /// Gets the list of documents.
//        /// </summary>
//        /// <returns>The documents.</returns>
//        /// <param name="query">Query.</param>
//        public async Task<IList<T>> GetListAsync(string query, object parameters)
//        {
//            return await GetListHelperAsync(new DynamicQuery<T>(query, parameters));
//        }

//        /// <summary>
//        /// Gets the list of documents.
//        /// </summary>
//        /// <returns>The documents.</returns>
//        /// <param name="query">Query.</param>
//        public IList<T> GetList(SqlQuerySpec query)
//        {
//            return AsyncTools.RunSync(() => GetListAsync(query));
//        }

//        /// <summary>
//        /// Gets the list of documents.
//        /// </summary>
//        /// <returns>The documents.</returns>
//        /// <param name="query">Query.</param>
//        public IList<T> GetList(DynamicQuery query)
//        {
//            return AsyncTools.RunSync(() => GetListAsync(query));
//        }

//        /// <summary>
//        /// Gets the list of documents.
//        /// </summary>
//        /// <returns>The documents.</returns>
//        /// <param name="query">Query.</param>
//        public IList<T> GetList(string query, object parameters)
//        {
//            return AsyncTools.RunSync(() => GetListAsync(new DynamicQuery<T>(query, parameters).ToSqlQuerySpec()));
//        }

//        async Task<T> GetFirstOrDefaultHelperAsync(IQuery query = null)
//        {
//            SqlQuerySpec sqlq;

//            if (query == null)
//            {
//                var qq = new SqlQuerySpecQuery<T>();
//                sqlq = qq.ToGetFirstOrDefault();
//            }
//            else
//            {
//                sqlq = query.ToGetFirstOrDefault();
//            }

//            var sq = sqlq.ToSqlQuery();
//            var q = _connection.Client.CreateDocumentQuery<T>(UriFactory.CreateDocumentCollectionUri(_connection.DatabaseId, _connection.CollectionId), sq).AsDocumentQuery();
//            var response = await QuerySingleDocumentAsync(q);
//            return response.AsEnumerable().FirstOrDefault();
//        }

//        static async Task<FeedResponse<T>> QuerySingleDocumentAsync(IDocumentQuery<T> query)
//        {
//            return await query.ExecuteNextAsync<T>();
//        }
        
//        async Task<T> CreateDocumentAsync(T entity)
//        {
//            var expando = Core.CreateExpandoFromObject<T>(entity);
            
//            var response = await _connection.Client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(_connection.DatabaseId, _connection.CollectionId), expando);
//            var ret = (T)(dynamic)response.Resource;
//            return ret;
//        }

//        async Task<T> ReplaceDocumentAsync(T entity)
//        {
//            var expando = Core.CreateExpandoFromObject<T>(entity);
            
//            var response = await _connection.Client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(_connection.DatabaseId, _connection.CollectionId, GetId(entity)), expando);
//            var ret = (T)(dynamic)response.Resource;
//            return ret;
//        }
		
//		async Task<T> UpsertDocumentAsync(T entity)
//        {
//            var expando = Core.CreateExpandoFromObject<T>(entity);
            
//            var response = await _connection.Client.UpsertDocumentAsync(UriFactory.CreateDocumentCollectionUri(_connection.DatabaseId, _connection.CollectionId), expando);
//            var ret = (T)(dynamic)response.Resource;
//            return ret;
//        }

//        async Task<bool> DeleteDocumentAsync(T entity)
//        {            
//            var response = await _connection.Client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(_connection.DatabaseId, _connection.CollectionId, GetId(entity)));
//            var isSuccess = response.StatusCode == HttpStatusCode.NoContent;
//            return isSuccess;
//        }

//        async Task<bool> DeleteDocumentAsync(string id)
//        {            
//            var response = await _connection.Client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(_connection.DatabaseId, _connection.CollectionId, id));
//            var isSuccess = response.StatusCode == HttpStatusCode.NoContent;
//            return isSuccess;
//        }

//        static async Task<List<T>> QueryMoreDocumentsAsync(IDocumentQuery<T> query)
//        {
//            var entitiesRetrieved = new List<T>();
            
//            while (query.HasMoreResults)
//            {                
//                var queryResponse = await QuerySingleDocumentAsync(query);
                
//                var entities = queryResponse.AsEnumerable();

//                if (entities != null)
//                    entitiesRetrieved.AddRange(entities);
//            }

//            return entitiesRetrieved;
//        }        

//        static string GetId(T entity)
//        {
//            var ids = new List<string> { "Id", "id", "ID" };

//            if (entity is IDynamicMetaObjectProvider)
//            {
//                IDictionary<string, object> propertyValues = (IDictionary<string, object>)entity;

//                foreach(var pv in propertyValues)
//                {
//                    foreach (var id in ids)
//                    {
//                        if (pv.Key.Equals(id))
//                            return pv.Value.ToString();
//                    }
//                }
//            }

//            foreach (var id in ids)
//            {
//                var v = entity.GetPropertyValue<string>(id);

//                if (v != null)
//                    return v;
//            }

//            throw new System.Exception($"Entity {typeof(T)} has got no property named Id/id/ID.");
//        }

//        async Task<IList<T>> GetListHelperAsync(IQuery query)
//        {
//            var sq = query.ToSqlQuerySpec().ToSqlQuery();
//            var q = _connection.Client.CreateDocumentQuery<T>(UriFactory.CreateDocumentCollectionUri(_connection.DatabaseId, _connection.CollectionId), sq).AsDocumentQuery();

//            var response = await QueryMoreDocumentsAsync(q);
//            return response;
//        }
//    }
//}
