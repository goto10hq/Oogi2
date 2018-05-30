using Microsoft.Azure.Documents;
using Oogi2.Attributes;
using Sushi2;

namespace Oogi2.Queries
{
    public class SqlQuerySpecQuery<T> : IQuery where T : class
    {
        readonly SqlQuerySpec _sqlQuerySpec;
        readonly string _entityName = typeof(T).GetAttributeValue((EntityTypeAttribute a) => a.Name);
        readonly string _entityValue = typeof(T).GetAttributeValue((EntityTypeAttribute a) => a.Value);

        public SqlQuerySpecQuery(SqlQuerySpec sqlQuerySpec = null)
        {
            _sqlQuerySpec = sqlQuerySpec;
        }

        public SqlQuerySpec ToSqlQuerySpec()
        {
            return _sqlQuerySpec;
        }

        public SqlQuerySpec ToGetFirstOrDefault()
        {
            if (_sqlQuerySpec == null)
            {
                if (_entityName == null)
                    return new SqlQuerySpec("select top 1 * from c");

                return new SqlQuerySpec($"select top 1 * from c where c[\"{_entityName}\"] = @entity",
                    new SqlParameterCollection
                    {
                        new SqlParameter("@entity", _entityValue)
                    });
            }

            return _sqlQuerySpec;
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
}