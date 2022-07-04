using System;
using Microsoft.Azure.Cosmos;
using Oogi2.Entities;

namespace Oogi2.Queries
{
    public class IdQuery<T> : IQuery<T> where T : BaseEntity
    {
        readonly string _id;
        readonly string _partitionKey;
        const string EntityName = "entity";        

        public IdQuery(string id, string partitionKey)
        {
            _id = id;
            _partitionKey = partitionKey;
        }

        public QueryDefinition ToQueryDefinition(T item)
        {
            var qd = new QueryDefinition($"select * from c where c.id = @id and c[\"{EntityName}\"] = @entity")
                .WithParameter("@id", _id)
                .WithParameter("@entity", item.Entity);

            if (!string.IsNullOrEmpty(item.PartitionKey))
                qd = qd.WithParameter("@partitionKey", _partitionKey);

            return qd;
        }

        public QueryDefinition ToGetFirstOrDefault(T item)
        {
            var qd = new QueryDefinition($"select * from c where c.id = @id and c[\"{EntityName}\"] = @entity")
                .WithParameter("@id", _id)
                .WithParameter("@entity", item.Entity);

            if (!string.IsNullOrEmpty(item.PartitionKey))
                qd = qd.WithParameter("@partitionKey", _partitionKey);

            return qd;
        }

        public QueryDefinition ToGetFirstOrDefault()
        {
            var qd = new QueryDefinition($"select * from c where c.id = @id")
                .WithParameter("@id", _id);

            if (!string.IsNullOrEmpty(_partitionKey))
                qd = qd.WithParameter("@partitionKey", _partitionKey);

            return qd;
        }

        public QueryDefinition ToGetAll() => throw new NotImplementedException();

        
    }    
}
