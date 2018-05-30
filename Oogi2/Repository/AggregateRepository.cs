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

        /// <summary>
        /// Gets the first or default document.
        /// </summary>
        /// <returns>The first document.</returns>
        /// <param name="query">Query.</param>
        public async Task<AggregateResult> GetAsync(SqlQuerySpec query)
        {
            if (query == null)
                throw new System.ArgumentNullException(nameof(query));

            return await _repository.GetAggregateHelperAsync(new SqlQuerySpecQuery<AggregateResult>(query)).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the first or default document.
        /// </summary>
        /// <returns>The first document.</returns>
        /// <param name="query">Query.</param>
        public async Task<AggregateResult> GetAsync(DynamicQuery query)
        {
            if (query == null)
                throw new System.ArgumentNullException(nameof(query));

            return await _repository.GetAggregateHelperAsync(query).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the first or default document.
        /// </summary>
        /// <returns>The first document.</returns>
        /// <param name="query">Query.</param>
        public async Task<AggregateResult> GetAsync(string query, object parameters)
        {
            if (query == null)
                throw new System.ArgumentNullException(nameof(query));

            return await _repository.GetAggregateHelperAsync(new DynamicQuery<AggregateResult>(query, parameters)).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the first or default document.
        /// </summary>
        /// <returns>The first document.</returns>
        /// <param name="query">Query.</param>
        public AggregateResult Get(SqlQuerySpec query)
        {
            if (query == null)
                throw new System.ArgumentNullException(nameof(query));

            return AsyncTools.RunSync(() => GetAsync(query));
        }

        /// <summary>
        /// Gets the first or default document.
        /// </summary>
        /// <returns>The first document.</returns>
        /// <param name="query">Query.</param>
        public AggregateResult Get(DynamicQuery query)
        {
            if (query == null)
                throw new System.ArgumentNullException(nameof(query));

            return AsyncTools.RunSync(() => GetAsync(query));
        }

        /// <summary>
        /// Gets the first or default document.
        /// </summary>
        /// <returns>The first document.</returns>
        /// <param name="sql">Sql query.</param>
        /// <param name="parameters">Parameters.</param>
        public AggregateResult Get(string sql, object parameters)
        {
            if (sql == null)
                throw new System.ArgumentNullException(nameof(sql));

            return AsyncTools.RunSync(() => GetAsync(new DynamicQuery<AggregateResult>(sql, parameters).ToSqlQuerySpec()));
        }
    }
}