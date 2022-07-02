using System;
using System.Linq;
using Oogi2.Queries;
using Microsoft.Azure.Cosmos;

namespace Oogi2
{
    public static class Core
    {
        // internal static ExpandoObject CreateExpandoFromObject<T>(object source)
        // {
        //     var result = new ExpandoObject();

        //     IDictionary<string, object> dictionary = result;

        //     foreach (var property in source
        //         .GetType()
        //         .GetProperties()
        //         .Where(p => p.CanRead && p.GetMethod.IsPublic))
        //     {
        //         dictionary[property.Name] = property.GetValue(source, null);
        //     }

        //     var _entityName = typeof(T).GetAttributeValue((EntityTypeAttribute a) => a.Name);
        //     var _entityValue = typeof(T).GetAttributeValue((EntityTypeAttribute a) => a.Value);

        //     if (_entityName != null)
        //     {
        //         if (!dictionary.ContainsKey(_entityName))
        //             dictionary.Add(_entityName, _entityValue);
        //     }

        //     return result;
        // }

        public static string ToSqlQuery(this DynamicQuery dq)
        {
            if (dq == null)
                throw new ArgumentNullException(nameof(dq));

            var qd = dq.ToQueryDefinition();

            return ToSqlQuery(qd);
        }

        public static string ToSqlQuery(this QueryDefinition query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            var r = query.QueryText;
            var qpc = query.GetQueryParameters();

            if (qpc != null &&
                qpc.Any())
            {
                foreach (var (Name, Value) in qpc)
                {
                    var v = Converter.Process(Value);
                    r = r.Replace(Name, v);
                }
            }

            return r;
        }
    }
}