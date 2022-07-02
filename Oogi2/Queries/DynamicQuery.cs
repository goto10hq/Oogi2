using Microsoft.Azure.Cosmos;
using Oogi2.Entities;

namespace Oogi2.Queries
{
    public class DynamicQuery<T> : IQuery, IQuery<T> where T : BaseEntity
    {
        readonly object _parameters;
        readonly string _sql;
        const string EntityName = "entity";

        QueryDefinition _sqlQuerySpec;
        QueryDefinition SqlQuerySpec => _sqlQuerySpec ?? (_sqlQuerySpec = ConvertToSqlQuerySpec(_sql, _parameters));

        public DynamicQuery()
        {
        }

        public DynamicQuery(string sql, object parameters = null)
        {
            _sql = sql;
            _parameters = parameters;
        }

        static QueryDefinition ConvertToSqlQuerySpec(string sql, object parameters)
        {
            if (string.IsNullOrEmpty(sql))
                return null;

            var sqlqs = new QueryDefinition(sql);

            if (parameters == null)
                return sqlqs;

            var sqlParameters = Tools.AnonymousObjectToSqlParameters(parameters);

            if (sqlParameters == null)
                return sqlqs;

            var result = new QueryDefinition(sqlqs.QueryText);

            foreach (var (Name, Value) in sqlParameters)
            {
                result = result.WithParameter(Name, Value);
            }

            return result;
        }

        public QueryDefinition ToQueryDefinition(T item)
        {
            return SqlQuerySpec;
        }

        public QueryDefinition ToGetFirstOrDefault(T item)
        {
            if (SqlQuerySpec == null)
            {
                return new QueryDefinition($"select top 1 * from c where c[\"{EntityName}\"] = @entity")
                    .WithParameter("@entity", item.Entity);
            }

            return SqlQuerySpec;
        }

        public QueryDefinition ToGetAll(T item)
        {
            return new QueryDefinition($"select * from c where c[\"{EntityName}\"] = @entity")
                .WithParameter("@entity", item.Entity);
        }

        public QueryDefinition ToQueryDefinition()
        {
            return SqlQuerySpec;
        }
    }

    public class DynamicQuery : IQuery
    {
        readonly object _parameters;
        readonly string _sql;

        QueryDefinition _sqlQuerySpec;
        QueryDefinition SqlQuerySpec => _sqlQuerySpec ?? (_sqlQuerySpec = ConvertToSqlQuerySpec(_sql, _parameters));

        public DynamicQuery()
        {
        }

        public DynamicQuery(string sql, object parameters = null)
        {
            _sql = sql;
            _parameters = parameters;
        }

        static QueryDefinition ConvertToSqlQuerySpec(string sql, object parameters)
        {
            if (string.IsNullOrEmpty(sql))
                return null;

            var sqlqs = new QueryDefinition(sql);

            if (parameters == null)
                return sqlqs;

            var sqlParameters = Tools.AnonymousObjectToSqlParameters(parameters);

            if (sqlParameters == null)
                return sqlqs;

            var result = new QueryDefinition(sql);

            foreach (var (Name, Value) in sqlParameters)
            {
                result = result.WithParameter(Name, Value);
            }

            return result;
        }

        public QueryDefinition ToQueryDefinition()
        {
            return SqlQuerySpec;
        }

        public QueryDefinition ToGetFirstOrDefault()
        {
            return SqlQuerySpec ?? new QueryDefinition("select top 1 * from c");
        }

        public QueryDefinition ToGetAll()
        {
            return new QueryDefinition("select * from c");
        }
    }
}