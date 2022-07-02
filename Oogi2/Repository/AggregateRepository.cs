using Microsoft.Azure.Cosmos;
using Oogi2.Queries;
using Oogi2.Tokens;
using System.Threading.Tasks;

namespace Oogi2
{
    public class AggregateRepository
    {
        readonly IConnection _connection;

        public AggregateRepository(IConnection connection)
        {
            _connection = connection;
        }

        public async Task<long?> GetAsync(QueryDefinition query, QueryRequestOptions requestOptions = null)
        {
            return await GetAsync(query.ToDynamicQuery(), requestOptions);
        }

        public async Task<long?> GetAsync(DynamicQuery query, QueryRequestOptions requestOptions = null)
        {
            if (query == null)
                throw new System.ArgumentNullException(nameof(query));

            var r = await GetAggregateHelperAsync(query, requestOptions).ConfigureAwait(false);

            return r.Number;
        }

        public async Task<long?> GetAsync(string query, object parameters, QueryRequestOptions requestOptions = null)
        {
            if (query == null)
                throw new System.ArgumentNullException(nameof(query));

            var r = await GetAggregateHelperAsync(new DynamicQuery(query, parameters), requestOptions).ConfigureAwait(false);

            return r.Number;
        }

        async Task<AggregateResult> GetAggregateHelperAsync(DynamicQuery query, QueryRequestOptions requestOptions = null)
        {
            var sq = query.ToQueryDefinition().ToSqlQuery();
            var items = await _connection.QueryMoreItemsAsync<AggregateResult>(sq, requestOptions);

            var result = new AggregateResult();

            if (items != null)
            {
                foreach (var ar in items)
                {
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
    }
}