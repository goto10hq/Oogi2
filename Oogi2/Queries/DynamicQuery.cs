using Microsoft.Azure.Cosmos;
using Oogi2.Entities;
using System;
using System.Collections.Generic;

namespace Oogi2.Queries
{
    public class DynamicQuery<T> : IQuery, IQuery<T> where T : BaseEntity
    {
        const string EntityName = "entity";

        readonly QueryDefinition _sqlQuerySpec;
        QueryDefinition SqlQuerySpec => _sqlQuerySpec;

        public DynamicQuery(string sql, object parameters = null)
        {
            _sqlQuerySpec = ConvertToSqlQuerySpec(sql, parameters);
        }

        static QueryDefinition ConvertToSqlQuerySpec(string sql, object parameters)
        {
            if (string.IsNullOrEmpty(sql))
                return null;

            var sqlqs = new QueryDefinition(sql);

            if (parameters == null)
                return sqlqs;

            var pc = parameters as IReadOnlyList<(string Name, object Value)>;
            
            if (pc != null)
            {
                var result = new QueryDefinition(sqlqs.QueryText);

                foreach (var p in pc)
                    result = result.WithParameter(p.Name, p.Value);

                return result;
            }
            else
            {
                var sqlParameters = Tools.AnonymousObjectToSqlParameters(parameters);

                if (sqlParameters == null)
                    return sqlqs;

                var result = new QueryDefinition(sqlqs.QueryText);

                foreach (var (Name, Value) in sqlParameters)
                {
                    result = result.WithParameter("@" + Name, Value);
                }

                return result;
            }            
        }

        public QueryDefinition ToQueryDefinition(T item) => SqlQuerySpec;

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

        public QueryDefinition ToGetFirstOrDefault()
        {
            throw new NotImplementedException();
        }
    }

    public class DynamicQuery : IQuery
    {        
        QueryDefinition _sqlQuerySpec;
        QueryDefinition SqlQuerySpec => _sqlQuerySpec;

        public DynamicQuery(string sql, object parameters = null)
        {            
            _sqlQuerySpec = ConvertToSqlQuerySpec(sql, parameters);
        }

        static QueryDefinition ConvertToSqlQuerySpec(string sql, object parameters)
        {
            if (string.IsNullOrEmpty(sql))
                return null;

            var sqlqs = new QueryDefinition(sql);

            if (parameters == null)
                return sqlqs;

            var pc = parameters as IReadOnlyList<(string Name, object Value)>;

            if (pc != null)
            {
                var result = new QueryDefinition(sqlqs.QueryText);

                foreach (var p in pc)
                    result = result.WithParameter(p.Name, p.Value);

                return result;
            }
            else
            {
                var sqlParameters = Tools.AnonymousObjectToSqlParameters(parameters);

                if (sqlParameters == null)
                    return sqlqs;

                var result = new QueryDefinition(sqlqs.QueryText);

                foreach (var (Name, Value) in sqlParameters)
                {
                    result = result.WithParameter("@" + Name, Value);
                }

                return result;
            }
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