using Microsoft.Azure.Documents;
using Oogi2.Attributes;
using Sushi2;

namespace Oogi2.Queries
{
    public class DynamicQuery<T> : IQuery where T : class
    {
        readonly object _parameters;
        readonly string _sql;
        readonly string _entityName = typeof(T).GetAttributeValue((EntityType a) => a.Name);
        readonly string _entityValue = typeof(T).GetAttributeValue((EntityType a) => a.Value);

        SqlQuerySpec _sqlQuerySpec;
        SqlQuerySpec SqlQuerySpec => _sqlQuerySpec ?? (_sqlQuerySpec = ConvertToSqlQuerySpec(_sql, _parameters));

        public DynamicQuery()
        {
        }

        public DynamicQuery(string sql, object parameters = null)
        {
            _sql = sql;
            _parameters = parameters;
        }

        static SqlQuerySpec ConvertToSqlQuerySpec(string sql, object parameters)
        {
            if (string.IsNullOrEmpty(sql))
                return null;

            var sqlqs = new SqlQuerySpec(sql);

            if (parameters == null)
                return sqlqs;

            var sqlParameters = Tools.AnonymousObjectToSqlParameters(parameters);

            if (sqlParameters == null)
                return sqlqs;

            return new SqlQuerySpec(sql, sqlParameters);
        }

        public SqlQuerySpec ToSqlQuerySpec()
        {
            return SqlQuerySpec;
        }

        public SqlQuerySpec ToGetFirstOrDefault()
        {
            if (SqlQuerySpec == null)
            {
                if (_entityName == null)
                    return new SqlQuerySpec("select top 1 * from c");

                return new SqlQuerySpec($"select top 1 * from c where c[\"{_entityName}\"] = @entity",
                    new SqlParameterCollection
                    {
                        new SqlParameter("@entity", _entityValue)
                    });
            }

            return SqlQuerySpec;
        }

        public SqlQuerySpec ToGetAll()
        {
            if (_entityName == null)
                return new SqlQuerySpec("select * from c");

            return new SqlQuerySpec($"select * from c where c[\"{_entityName}\"] = @entity",
                new SqlParameterCollection
                {
                    new SqlParameter("@entity", _entityValue)
                });
        }
    }

    public class DynamicQuery: IQuery
    {
        readonly object _parameters;
        readonly string _sql;
        
        SqlQuerySpec _sqlQuerySpec;
        SqlQuerySpec SqlQuerySpec => _sqlQuerySpec ?? (_sqlQuerySpec = ConvertToSqlQuerySpec(_sql, _parameters));

        public DynamicQuery()
        {
        }

        public DynamicQuery(string sql, object parameters = null)
        {
            _sql = sql;
            _parameters = parameters;
        }

        static SqlQuerySpec ConvertToSqlQuerySpec(string sql, object parameters)
        {
            if (string.IsNullOrEmpty(sql))
                return null;

            var sqlqs = new SqlQuerySpec(sql);

            if (parameters == null)
                return sqlqs;

            var sqlParameters = Tools.AnonymousObjectToSqlParameters(parameters);

            if (sqlParameters == null)
                return sqlqs;

            return new SqlQuerySpec(sql, sqlParameters);
        }

        public SqlQuerySpec ToSqlQuerySpec()
        {
            return SqlQuerySpec;
        }

        public SqlQuerySpec ToGetFirstOrDefault()
        {
            if (SqlQuerySpec == null)            
                return new SqlQuerySpec("select top 1 * from c");                            

            return SqlQuerySpec;
        }

        public SqlQuerySpec ToGetAll()
        {            
            return new SqlQuerySpec("select * from c");
        }
    }
}
