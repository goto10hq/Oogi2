using Microsoft.Azure.Documents;
using Oogi2.Queries;
using Oogi2.Tokens;
using Sushi2;
using System.Threading.Tasks;

namespace Oogi2
{
    public class AggregateRepository : IRepository
    {
        readonly BaseRepository<AggregateResult> _repository;

        public AggregateRepository(IConnection connection)
        {
            _repository = new BaseRepository<AggregateResult>(connection);
        }

        public async Task<long?> GetAsync(SqlQuerySpec query)
        {
            if (query == null)
                throw new System.ArgumentNullException(nameof(query));

            var r = await _repository.GetAggregateHelperAsync(new SqlQuerySpecQuery<AggregateResult>(query)).ConfigureAwait(false);

            return r.Number;
        }

        public async Task<long?> GetAsync(DynamicQuery query)
        {
            if (query == null)
                throw new System.ArgumentNullException(nameof(query));

            var r = await _repository.GetAggregateHelperAsync(query).ConfigureAwait(false);

            return r.Number;
        }

        public async Task<long?> GetAsync(string query, object parameters)
        {
            if (query == null)
                throw new System.ArgumentNullException(nameof(query));

            var r = await _repository.GetAggregateHelperAsync(new DynamicQuery<AggregateResult>(query, parameters)).ConfigureAwait(false);

            return r.Number;
        }

        public long? Get(SqlQuerySpec query)
        {
            if (query == null)
                throw new System.ArgumentNullException(nameof(query));

            return AsyncTools.RunSync(() => GetAsync(query));
        }

        public long? Get(DynamicQuery query)
        {
            if (query == null)
                throw new System.ArgumentNullException(nameof(query));

            return AsyncTools.RunSync(() => GetAsync(query));
        }

        public long? Get(string sql, object parameters)
        {
            if (sql == null)
                throw new System.ArgumentNullException(nameof(sql));

            return AsyncTools.RunSync(() => GetAsync(new DynamicQuery<AggregateResult>(sql, parameters).ToSqlQuerySpec()));
        }
    }
}