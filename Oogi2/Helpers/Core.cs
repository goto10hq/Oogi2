using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Oogi2.Queries;
using System.Dynamic;
using System.Collections.Generic;
using Sushi2;
using Oogi2.Attributes;

namespace Oogi2
{
    public static class Core
    {        
        public static ExpandoObject CreateExpandoFromObject<T>(object source)
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

        /// <summary>
        /// Get sql query from dynamic query.
        /// </summary>        
        public static string ToSqlQuery(this DynamicQuery dq) 
        {
            if (dq == null)
                throw new ArgumentNullException(nameof(dq));

            var sqlqs = dq.ToSqlQuerySpec();

            return ToSqlQuery(sqlqs);
        }

        ///// <summary>
        ///// Get sql query from dynamic query.
        ///// </summary>        
        //public static string ToSqlQuery<T>(this DynamicQuery<T> dq) where T : class
        //{
        //    if (dq == null)
        //        throw new ArgumentNullException(nameof(dq));

        //    var sqlqs = dq.ToSqlQuerySpec();

        //    return ToSqlQuery(sqlqs);
        //}

        /// <summary>
        /// Get sql query from sql query spec.
        /// </summary>        
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

        /// <summary>
        /// Execute db action with retries.
        /// </summary>
        internal static async Task<T2> ExecuteWithRetriesAsync<T2>(Func<Task<T2>> function)
        {
            while (true)
            {
                TimeSpan sleepTime;

                try
                {
                    return await function();
                }
                catch (DocumentClientException de)
                {
                    if (de.StatusCode != null &&
                        ((int)de.StatusCode != 429 &&
                        (int)de.StatusCode != 503))
                    {
                        throw;
                    }
                    sleepTime = de.RetryAfter;
                }
                catch (AggregateException ae)
                {
                    if (!(ae.InnerException is DocumentClientException))
                    {
                        throw;
                    }

                    var de = (DocumentClientException)ae.InnerException;
                    if (de.StatusCode != null &&
                        ((int)de.StatusCode != 429 &&
                        (int)de.StatusCode != 503))
                    {
                        throw;
                    }

                    sleepTime = de.RetryAfter;
                }

                await Task.Delay(sleepTime);
            }
        }
    }
}
