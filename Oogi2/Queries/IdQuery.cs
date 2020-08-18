using System;
using Microsoft.Azure.Documents;
using Oogi2.Attributes;
using Sushi2;

namespace Oogi2.Queries
{
    public class IdQuery<T> : IQuery where T : class
    {
        readonly string _id;
        readonly string _entityName = typeof(T).GetAttributeValue((EntityTypeAttribute a) => a.Name);
        readonly string _entityValue = typeof(T).GetAttributeValue((EntityTypeAttribute a) => a.Value);

        public IdQuery(string id)
        {
            _id = id;
        }

        public SqlQuerySpec ToSqlQuerySpec()
        {
            if (_entityName == null)
                return new SqlQuerySpec($"select * from c where c.id = @id",
                    new SqlParameterCollection
                    {
                        new SqlParameter("@id", _id)
                    });

            return new SqlQuerySpec($"select * from c where c.id = @id and c[\"{_entityName}\"] = @entity",
                new SqlParameterCollection
                {
                    new SqlParameter("@id", _id),
                    new SqlParameter("@entity", _entityValue)
                });
        }

        public SqlQuerySpec ToGetFirstOrDefault()
        {
            if (_entityName == null)
                return new SqlQuerySpec($"select top 1 * from c where c.id = @id",
                    new SqlParameterCollection
                    {
                        new SqlParameter("@id", _id)
                    });

            return new SqlQuerySpec($"select top 1 * from c where c.id = @id and c[\"{_entityName}\"] = @entity",
                new SqlParameterCollection
                {
                    new SqlParameter("@id", _id),
                    new SqlParameter("@entity", _entityValue)
                });
        }

        public SqlQuerySpec ToGetAll() => throw new NotImplementedException();
    }
}
