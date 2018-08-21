using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Newtonsoft.Json.Linq;
using Oogi2.Queries;
using Oogi2.Tokens;
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

        internal async Task<T> GetFirstOrDefaultHelperAsync(IQuery query = null, FeedOptions feedOptions = null)
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
            var q = _connection.Client.CreateDocumentQuery<T>(UriFactory.CreateDocumentCollectionUri(_connection.DatabaseId, _connection.CollectionId), sq, feedOptions).AsDocumentQuery();
            var response = await QuerySingleDocumentAsync(q).ConfigureAwait(false);
            return response.AsEnumerable().FirstOrDefault();
        }

        internal async Task<AggregateResult> GetAggregateHelperAsync(IQuery query = null, FeedOptions feedOptions = null)
        {
            var sq = query.ToSqlQuerySpec().ToSqlQuery();
            var q = _connection.Client.CreateDocumentQuery<T>(UriFactory.CreateDocumentCollectionUri(_connection.DatabaseId, _connection.CollectionId), sq, feedOptions).AsDocumentQuery();

            var response = await QueryMoreDocumentsAsync(q).ConfigureAwait(false);
            var result = new AggregateResult();

            if (response != null)
            {
                foreach (var r in response)
                {
                    var ar = r as AggregateResult;

                    if (ar != null)
                    {
                        if (!result.Number.HasValue)
                            result.Number = 0;

                        result.Number += ar.Number;
                    }
                }
            }

            return result;
        }

        internal static Task<FeedResponse<T>> QuerySingleDocumentAsync(IDocumentQuery<T> query)
        {
            return query.ExecuteNextAsync<T>();
        }

        internal async Task<T> CreateDocumentAsync(T entity, RequestOptions requestOptions)
        {
            var expando = Core.CreateExpandoFromObject<T>(entity);

            var response = await _connection.Client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(_connection.DatabaseId, _connection.CollectionId), expando, requestOptions).ConfigureAwait(false);
            return (T)(dynamic)response.Resource;
        }

        internal async Task<T> ReplaceDocumentAsync(T entity, RequestOptions requestOptions)
        {
            var expando = Core.CreateExpandoFromObject<T>(entity);

            var response = await _connection.Client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(_connection.DatabaseId, _connection.CollectionId, GetId(entity)), expando, requestOptions).ConfigureAwait(false);
            return (T)(dynamic)response.Resource;
        }

        internal async Task<T> UpsertDocumentAsync(T entity, RequestOptions requestOptions)
        {
            var expando = Core.CreateExpandoFromObject<T>(entity);

            var response = await _connection.Client.UpsertDocumentAsync(UriFactory.CreateDocumentCollectionUri(_connection.DatabaseId, _connection.CollectionId), expando, requestOptions).ConfigureAwait(false);
            return (T)(dynamic)response.Resource;
        }

        internal async Task<bool> DeleteDocumentAsync(T entity, RequestOptions requestOptions)
        {
            var response = await _connection.Client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(_connection.DatabaseId, _connection.CollectionId, GetId(entity)), requestOptions).ConfigureAwait(false);
            return response.StatusCode == HttpStatusCode.NoContent;
        }

        internal async Task<bool> DeleteDocumentAsync(string id, RequestOptions requestOptions)
        {
            var response = await _connection.Client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(_connection.DatabaseId, _connection.CollectionId, id), requestOptions).ConfigureAwait(false);
            return response.StatusCode == HttpStatusCode.NoContent;
        }

        internal static async Task<IList<T>> QueryMoreDocumentsAsync(IDocumentQuery<T> query)
        {
            var entitiesRetrieved = new List<T>();

            while (query.HasMoreResults)
            {
                var queryResponse = await QuerySingleDocumentAsync(query).ConfigureAwait(false);

                var entities = queryResponse.AsEnumerable();

                if (entities != null)
                    entitiesRetrieved.AddRange(entities);
            }

            return entitiesRetrieved;
        }

        internal static string GetId(T entity)
        {
            var ids = new List<string> { "Id", "id", "ID", "iD" };

            if (entity is IDynamicMetaObjectProvider dyn)
            {
                var j = JObject.FromObject(entity);

                if (j != null)
                {
                    foreach (var id in ids)
                    {
                        if (j[id] != null)
                        {
                            var jv = j[id].Value<string>();

                            if (jv != null)
                                return jv;
                        }
                    }
                }

                throw new Exception($"Entity {typeof(T)} has got no property named Id/id/ID/iD.");
            }

            foreach (var id in ids)
            {
                var v = entity.GetPropertyValue<string>(id);

                if (v != null)
                    return v;
            }

            throw new Exception($"Entity {typeof(T)} has got no property named Id/id/ID/iD.");
        }

        internal Task<IList<T>> GetListHelperAsync(IQuery query, FeedOptions feedOptions)
        {
            var sq = query.ToSqlQuerySpec().ToSqlQuery();
            var q = _connection.Client.CreateDocumentQuery<T>(UriFactory.CreateDocumentCollectionUri(_connection.DatabaseId, _connection.CollectionId), sq, feedOptions).AsDocumentQuery();

            return QueryMoreDocumentsAsync(q);
        }
    }
}