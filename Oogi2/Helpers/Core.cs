using System;
using System.Linq;
using Microsoft.Azure.Documents;
using Oogi2.Queries;
using System.Dynamic;
using System.Collections.Generic;
using Sushi2;
using Oogi2.Attributes;

namespace Oogi2
{
    static class Core
    {
        internal static ExpandoObject CreateExpandoFromObject<T>(object source)
        {
            var result = new ExpandoObject();

            IDictionary<string, object> dictionary = result;

            foreach (var property in source
                .GetType()
                .GetProperties()
                .Where(p => p.CanRead && p.GetMethod.IsPublic))
            {
                dictionary[property.Name] = property.GetValue(source, null);
            }

            var _entityName = typeof(T).GetAttributeValue((EntityType a) => a.Name);
            var _entityValue = typeof(T).GetAttributeValue((EntityType a) => a.Value);

            if (_entityName != null)
            {
                if (!dictionary.ContainsKey(_entityName))
                    dictionary.Add(_entityName, _entityValue);
            }

            return result;
        }

        public static string ToSqlQuery(this DynamicQuery dq)
        {
            if (dq == null)
                throw new ArgumentNullException(nameof(dq));

            var sqlqs = dq.ToSqlQuerySpec();

            return ToSqlQuery(sqlqs);
        }


        public static string ToSqlQuery(this SqlQuerySpec sqs)
        {
            if (sqs == null)
                throw new ArgumentNullException(nameof(sqs));

            var r = sqs.QueryText;

            if (sqs.Parameters != null &&
                sqs.Parameters.Any())
            {
                foreach (var p in sqs.Parameters)
                {
                    var v = Converter.Process(p.Value);
                    r = r.Replace(p.Name, v);
                }
            }

            return r;
        }
    }
}
