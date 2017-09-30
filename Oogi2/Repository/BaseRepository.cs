using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Oogi2.Queries;
using Sushi2;

namespace Oogi2
{
    class BaseRepository<T> where T : class
    {
        readonly IConnection _connection;

        internal BaseRepository(IConnection connection)
        {
            _connection = connection;
        }

        internal async Task<T> GetFirstOrDefaultHelperAsync(IQuery query = null)
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
            var response = await QuerySingleDocumentAsync(q);
            return response.AsEnumerable().FirstOrDefault();
        }

        internal static async Task<FeedResponse<T>> QuerySingleDocumentAsync(IDocumentQuery<T> query)
        {
            return await query.ExecuteNextAsync<T>();
        }

        internal async Task<T> CreateDocumentAsync(T entity)
        {
            var expando = Core.CreateExpandoFromObject<T>(entity);

            var response = await _connection.Client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(_connection.DatabaseId, _connection.CollectionId), expando);
            var ret = (T)(dynamic)response.Resource;
            return ret;
        }

        internal async Task<T> ReplaceDocumentAsync(T entity)
        {
            var expando = Core.CreateExpandoFromObject<T>(entity);

            var response = await _connection.Client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(_connection.DatabaseId, _connection.CollectionId, GetId(entity)), expando);
            var ret = (T)(dynamic)response.Resource;
            return ret;
        }

        internal async Task<T> UpsertDocumentAsync(T entity)
        {
            var expando = Core.CreateExpandoFromObject<T>(entity);

            var response = await _connection.Client.UpsertDocumentAsync(UriFactory.CreateDocumentCollectionUri(_connection.DatabaseId, _connection.CollectionId), expando);
            var ret = (T)(dynamic)response.Resource;
            return ret;
        }

        internal async Task<bool> DeleteDocumentAsync(T entity)
        {
            var response = await _connection.Client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(_connection.DatabaseId, _connection.CollectionId, GetId(entity)));
            var isSuccess = response.StatusCode == HttpStatusCode.NoContent;
            return isSuccess;
        }

        internal async Task<bool> DeleteDocumentAsync(string id)
        {
            var response = await _connection.Client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(_connection.DatabaseId, _connection.CollectionId, id));
            var isSuccess = response.StatusCode == HttpStatusCode.NoContent;
            return isSuccess;
        }

        internal static async Task<IList<T>> QueryMoreDocumentsAsync(IDocumentQuery<T> query)
        {
            var entitiesRetrieved = new List<T>();

            while (query.HasMoreResults)
            {
                var queryResponse = await QuerySingleDocumentAsync(query);

                var entities = queryResponse.AsEnumerable();

                if (entities != null)
                    entitiesRetrieved.AddRange(entities);
            }

            return entitiesRetrieved;
        }

        internal static string GetId(T entity)
        {
            var ids = new List<string> { "Id", "id", "ID" };

            if (entity is IDynamicMetaObjectProvider)
            {
                IDictionary<string, object> propertyValues = (IDictionary<string, object>)entity;

                foreach (var pv in propertyValues)
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

            throw new Exception($"Entity {typeof(T)} has got no property named Id/id/ID.");
        }

        internal async Task<IList<T>> GetListHelperAsync(IQuery query)
        {
            var sq = query.ToSqlQuerySpec().ToSqlQuery();
            var q = _connection.Client.CreateDocumentQuery<T>(UriFactory.CreateDocumentCollectionUri(_connection.DatabaseId, _connection.CollectionId), sq).AsDocumentQuery();

            var response = await QueryMoreDocumentsAsync(q);
            return response;
        }
    }
}
