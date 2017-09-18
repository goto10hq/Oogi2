using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Oogi2.Queries;
using Sushi2;
using System.Dynamic;

namespace Oogi2
{
    public class Repository<T> where T : class
    {
        readonly IConnection _connection;

        public Repository(IConnection connection)
        {
            _connection = connection;            
        }
        
        async Task<T> GetFirstOrDefaultHelperAsync(IQuery query = null)
        {
            SqlQuerySpec sqlq;

            if (query == null)
            {
                var qq = new SqlQuerySpecQuery<T>();
                sqlq = qq.ToGetFirstOrDefault();
            }
            else
            {
                sqlq = query.ToGetFirstOrDefault();
            }

            var sq = sqlq.ToSqlQuery();
            var q = _connection.Client.CreateDocumentQuery<T>(UriFactory.CreateDocumentCollectionUri(_connection.DatabaseId, _connection.CollectionId), sq).AsDocumentQuery();
            var response = await Core.ExecuteWithRetriesAsync(() => QuerySingleDocumentAsync(q));
            return response.AsEnumerable().FirstOrDefault();
        }

        /// <summary>
        /// Get first or default.
        /// </summary>
        public async Task<T> GetFirstOrDefaultAsync(SqlQuerySpec query = null)
        {
            return await GetFirstOrDefaultHelperAsync(new SqlQuerySpecQuery<T>(query));
        }

        /// <summary>
        /// Get first or default.
        /// </summary>
        //public async Task<T> GetFirstOrDefaultAsync(DynamicQuery<T> query)
        //{
        //    return await GetFirstOrDefaultHelperAsync(query);
        //}

        /// <summary>
        /// Get first or default.
        /// </summary>
        public async Task<T> GetFirstOrDefaultAsync(DynamicQuery query)
        {
            return await GetFirstOrDefaultHelperAsync(query);
        }

        /// <summary>
        /// Get first or default.
        /// </summary>
        public async Task<T> GetFirstOrDefaultAsync(string query, object parameters)
        {
            return await GetFirstOrDefaultHelperAsync(new DynamicQuery<T>(query, parameters));
        }

        /// <summary>
        /// Get first or default.
        /// </summary>
        public T GetFirstOrDefault(SqlQuerySpec query = null)
        {
            return AsyncTools.RunSync(() => GetFirstOrDefaultAsync(query));
        }

        /// <summary>
        /// Get first or default.
        /// </summary>
        //public T GetFirstOrDefault(DynamicQuery<T> query)
        //{
        //    return AsyncTools.RunSync(() => GetFirstOrDefaultAsync(query));
        //}

        /// <summary>
        /// Get first or default.
        /// </summary>
        public T GetFirstOrDefault(DynamicQuery query)
        {
            return AsyncTools.RunSync(() => GetFirstOrDefaultAsync(query));
        }

        /// <summary>
        /// Get first or default.
        /// </summary>
        public T GetFirstOrDefault(string sql, object parameters)
        {
            return AsyncTools.RunSync(() => GetFirstOrDefaultAsync(new DynamicQuery<T>(sql, parameters).ToSqlQuerySpec()));
        }

        /// <summary>
        /// Get first or default.
        /// </summary>
        public async Task<T> GetFirstOrDefaultAsync(string id)
        {            
            return await GetFirstOrDefaultHelperAsync(new IdQuery<T>(id));
        }

        /// <summary>
        /// Get first or default.
        /// </summary>
        public T GetFirstOrDefault(string id)
        {
            return AsyncTools.RunSync(() => GetFirstOrDefaultAsync(id));
        }

        /// <summary>
        /// Upsert entity.
        /// </summary>
        public async Task<T> UpsertAsync(T entity)
        {
            var response = await Core.ExecuteWithRetriesAsync(() => UpsertDocumentAsync(entity));
            return response;
        }

        /// <summary>
        /// Upsert entity.
        /// </summary>
        public T Upsert(T entity)
        {
            var ret = AsyncTools.RunSync(() => UpsertAsync(entity));
            return ret;
        }
		
        /// <summary>
        /// Create entity.
        /// </summary>
        public async Task<T> CreateAsync(T entity)
        {
            var ro = new RequestOptions();
            
            var response = await Core.ExecuteWithRetriesAsync(() => CreateDocumentAsync(entity, ro));
            return response;            
        }

        /// <summary>
        /// Create entity.
        /// </summary>
        public T Create(T entity)
        {
            var ret = AsyncTools.RunSync(() => CreateAsync(entity));
            return ret;
        }

        /// <summary>
        /// Replace entity.
        /// </summary>
        public async Task<T> ReplaceAsync(T entity)
        {
            var response = await Core.ExecuteWithRetriesAsync(() => ReplaceDocumentAsync(entity));
            return response;
        }

        /// <summary>
        /// Replace entity.
        /// </summary>
        public T Replace(T entity)
        {
            var ret = AsyncTools.RunSync(() => ReplaceAsync(entity));
            return ret;
        }

        /// <summary>
        /// Delete entity.
        /// </summary>
        public async Task<T> DeleteAsync(string id)
        {
            var response = await Core.ExecuteWithRetriesAsync(() => DeleteDocumentAsync(id));            
            return response;            
        }

        /// <summary>
        /// Delete entity.
        /// </summary>
        public T Delete(string id)
        {
            var en = AsyncTools.RunSync(() => DeleteAsync(id));
            return en;
        }

        /// <summary>
        /// Delete entity.
        /// </summary>
        public async Task<T> DeleteAsync(T entity)
        {
            var response = await Core.ExecuteWithRetriesAsync(() => DeleteDocumentAsync(entity));
            return response;
        }

        /// <summary>
        /// Delete entity.
        /// </summary>
        public T Delete(T entity)
        {
            var en = AsyncTools.RunSync(() => DeleteAsync(GetId(entity)));
            return en;
        }

		/// <summary>
        /// Get list of all entities from query.
        /// </summary>        
        public async Task<List<T>> GetAllAsync()
        {            
            var query = new SqlQuerySpecQuery<T>();
		    var sq = query.ToGetAll().ToSqlQuery(); 
            var q = _connection.Client.CreateDocumentQuery<T>(UriFactory.CreateDocumentCollectionUri(_connection.DatabaseId, _connection.CollectionId), sq).AsDocumentQuery();

            var response = await Core.ExecuteWithRetriesAsync(() => QueryMoreDocumentsAsync(q));
            return response;
        }

        /// <summary>
        /// Get list of all entities from query.
        /// </summary>        
        public List<T> GetAll()
        {
            var all = AsyncTools.RunSync(GetAllAsync);
            return all;
        }

        async Task<List<T>> GetListHelperAsync(IQuery query)
        {
            var sq = query.ToSqlQuerySpec().ToSqlQuery();
            var q = _connection.Client.CreateDocumentQuery<T>(UriFactory.CreateDocumentCollectionUri(_connection.DatabaseId, _connection.CollectionId), sq).AsDocumentQuery();

            var response = await Core.ExecuteWithRetriesAsync(() => QueryMoreDocumentsAsync(q));
            return response;
        }

        /// <summary>
        /// Get list from query.
        /// </summary>        
        public async Task<List<T>> GetListAsync(SqlQuerySpec query)
        {
            return await GetListHelperAsync(new SqlQuerySpecQuery<T>(query));
        }

        /// <summary>
        /// Get list from query.
        /// </summary>        
        //public async Task<List<T>> GetListAsync(DynamicQuery<T> query)
        //{
        //    return await GetListHelperAsync(query);
        //}

        /// <summary>
        /// Get list from query.
        /// </summary>        
        public async Task<List<T>> GetListAsync(DynamicQuery query)
        {
            return await GetListHelperAsync(query);
        }

        /// <summary>
        /// Get list from query.
        /// </summary>        
        public async Task<List<T>> GetListAsync(string query, object parameters)
        {
            return await GetListHelperAsync(new DynamicQuery<T>(query, parameters));
        }

        /// <summary>
        /// Get list from query.
        /// </summary>        
        public List<T> GetList(SqlQuerySpec query)
        {
            return AsyncTools.RunSync(() => GetListAsync(query));
        }

        /// <summary>
        /// Get list from query.
        /// </summary>        
        //public List<T> GetList(DynamicQuery<T> query)
        //{
        //    return AsyncTools.RunSync(() => GetListAsync(query));
        //}

        /// <summary>
        /// Get list from query.
        /// </summary>        
        public List<T> GetList(DynamicQuery query)
        {
            return AsyncTools.RunSync(() => GetListAsync(query));
        }

        /// <summary>
        /// Get list from query.
        /// </summary>        
        public List<T> GetList(string query, object parameters)
        {
            return AsyncTools.RunSync(() => GetListAsync(new DynamicQuery<T>(query, parameters).ToSqlQuerySpec()));
        }       
                
        static async Task<FeedResponse<T>> QuerySingleDocumentAsync(IDocumentQuery<T> query)
        {
            return await query.ExecuteNextAsync<T>();
        }
        
        async Task<T> CreateDocumentAsync(T entity, RequestOptions requestOptions)
        {
            var expando = Core.CreateExpandoFromObject<T>(entity);

            var response = await Core.ExecuteWithRetriesAsync(() => _connection.Client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(_connection.DatabaseId, _connection.CollectionId), expando, requestOptions));
            var ret = (T)(dynamic)response.Resource;
            return ret;
        }

        async Task<T> ReplaceDocumentAsync(T entity)
        {
            var expando = Core.CreateExpandoFromObject<T>(entity);

            var response = await Core.ExecuteWithRetriesAsync(() => _connection.Client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(_connection.DatabaseId, _connection.CollectionId, GetId(entity)), expando));
            var ret = (T)(dynamic)response.Resource;
            return ret;
        }
		
		async Task<T> UpsertDocumentAsync(T entity)
        {
            var expando = Core.CreateExpandoFromObject<T>(entity);

            var response = await Core.ExecuteWithRetriesAsync(() => _connection.Client.UpsertDocumentAsync(UriFactory.CreateDocumentCollectionUri(_connection.DatabaseId, _connection.CollectionId), expando));
            var ret = (T)(dynamic)response.Resource;
            return ret;
        }

        async Task<T> DeleteDocumentAsync(T entity)
        {
            var response = await Core.ExecuteWithRetriesAsync(() => _connection.Client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(_connection.DatabaseId, _connection.CollectionId, GetId(entity))));
            var ret = (T)(dynamic)response.Resource;
            return ret;
        }

        async Task<T> DeleteDocumentAsync(string id)
        {
            var response = await Core.ExecuteWithRetriesAsync(() => _connection.Client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(_connection.DatabaseId, _connection.CollectionId, id)));
            var ret = (T)(dynamic)response.Resource;
            return ret;
        }

        static async Task<List<T>> QueryMoreDocumentsAsync(IDocumentQuery<T> query)
        {
            var entitiesRetrieved = new List<T>();
            
            while (query.HasMoreResults)
            {
                var queryResponse = await Core.ExecuteWithRetriesAsync(() => QuerySingleDocumentAsync(query));                
                
                var entities = queryResponse.AsEnumerable();

                if (entities != null)
                    entitiesRetrieved.AddRange(entities);
            }

            return entitiesRetrieved;
        }        

        static string GetId(T entity)
        {
            var ids = new List<string> { "Id", "id", "ID" };

            if (entity is IDynamicMetaObjectProvider)
            {
                IDictionary<string, object> propertyValues = (IDictionary<string, object>)entity;

                foreach(var pv in propertyValues)
                {
                    foreach (var id in ids)
                    {
                        if (pv.Key.Equals(id))
                            return pv.Value.ToString();
                    }
                }
            }

            foreach (var id in ids)
            {
                var v = entity.GetPropertyValue<string>(id);

                if (v != null)
                    return v;
            }

            throw new System.Exception($"Entity {typeof(T)} has got no property named Id/id/ID.");
        }
    }
}
