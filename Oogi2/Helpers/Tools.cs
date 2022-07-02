using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Oogi2.Helpers;
using Oogi2.Queries;

namespace Oogi2
{
    /// <summary>
    /// Tools.
    /// </summary>
    public static class Tools
    {
        internal static JsonSerializerSettings DefaultJsonSerializerSettings
        {
            get
            {
                return new JsonSerializerSettings
                {
                    Formatting = Formatting.None,
                    TypeNameHandling = TypeNameHandling.None,
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    Converters = { new JsonClaimConverter() }
                };
            }
        }

        /// <summary>
        /// Sets the json default settings.
        /// </summary>
        public static void SetJsonDefaultSettings()
        {
            JsonConvert.DefaultSettings = () => DefaultJsonSerializerSettings;
        }

        /// <summary>
        /// Escapes string.
        /// </summary>
        /// <returns>The escaped string.</returns>
        /// <param name="text">String.</param>
        internal static string ToEscapedString(this string text)
        {
            text = text ?? string.Empty;
            return text.Replace(@"\", @"\\").Replace("'", @"\'");
        }

        /// <summary>
        /// Converts anonymous object to sql parameters collection.
        /// </summary>
        /// <returns>Sql parameters collections.</returns>
        /// <param name="parameters">Anonymous object.</param>
        internal static IReadOnlyList<(string Name, object Value)> AnonymousObjectToSqlParameters(object parameters)
        {
            if (parameters == null)
                return new List<(string Name, object Value)>();

            var collection = new List<(string Name, object Value)>();

            foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(parameters))
            {
                object obj2 = descriptor.GetValue(parameters);
                collection.Add((descriptor.Name, obj2));
            }

            return collection;
        }

        internal static DynamicQuery ToDynamicQuery(this QueryDefinition query)
        {
            if (query == null)
                return null;

            var dq = new DynamicQuery(query.QueryText);
            var parameters = query.GetQueryParameters();
            
            if (parameters == null || !parameters.Any())
                return dq;

            var collection = new List<(string Name, object Value)>();

            foreach(var (Name, Value) in parameters)
                collection.Add((Name, Value));

            dq = new DynamicQuery(query.QueryText, collection);

            return dq;
        }
    }
}