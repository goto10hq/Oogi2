using Microsoft.Azure.Cosmos;
using Oogi2.Entities;

namespace Oogi2.Queries
{
    public class SqlQuerySpecQuery<T> : IQuery<T> where T : BaseEntity
    {
        readonly QueryDefinition _queryDefinition;
        const string EntityName = "entity";        

        public SqlQuerySpecQuery(QueryDefinition queryDefinition = null)
        {
            _queryDefinition = queryDefinition;
        }

        public QueryDefinition ToQueryDefinition(T item)
        {
            return _queryDefinition;
        }

        public QueryDefinition ToGetFirstOrDefault(T item)
        {
            if (_queryDefinition == null)
            {                
                return new QueryDefinition($"select top 1 * from c where c[\"{EntityName}\"] = @entity")
                    .WithParameter("@entity", item.Entity);
            }

            return _queryDefinition;
        }

        public QueryDefinition ToGetAll(T item)
        {            
            return new QueryDefinition($"select * from c where c[\"{EntityName}\"] = @entity")
                .WithParameter("@entity", item.Entity);
        }
    }
}