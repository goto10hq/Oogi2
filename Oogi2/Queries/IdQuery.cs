using System;
using Microsoft.Azure.Cosmos;
using Oogi2.Entities;

namespace Oogi2.Queries
{
    public class IdQuery<T> : IQuery<T> where T : BaseEntity
    {
        readonly string _id;
        const string EntityName = "entity";        

        public IdQuery(string id)
        {
            _id = id;
        }

        public QueryDefinition ToQueryDefinition(T item)
        {            
            return new QueryDefinition($"select * from c where c.id = @id and c[\"{EntityName}\"] = @entity")
                .WithParameter("@id", _id)
                .WithParameter("@entity", item.Entity);
        }

        public QueryDefinition ToGetFirstOrDefault(T item)
        {            
            return new QueryDefinition($"select top 1 * from c where c.id = @id and c[\"{EntityName}\"] = @entity")
                .WithParameter("@id", _id)
                .WithParameter("@entity", item.Entity);
        }

        public QueryDefinition ToGetAll() => throw new NotImplementedException();
    }    
}
