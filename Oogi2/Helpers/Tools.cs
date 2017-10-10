using System.ComponentModel;
using Microsoft.Azure.Documents;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Oogi2.Helpers;
using System.Collections.Generic;

namespace Oogi2
{
    /// <summary>
    /// Tools.
    /// </summary>
    internal static class Tools
    {
        /// <summary>
        /// Sets the json default settings.
        /// </summary>
        internal static void SetJsonDefaultSettings()
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                Formatting = Formatting.None,
                TypeNameHandling = TypeNameHandling.None,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Converters = new List<JsonConverter> { new JsonClaimConverter() }
            };
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
        internal static SqlParameterCollection AnonymousObjectToSqlParameters(object parameters)
        {
            if (parameters == null)
                return new SqlParameterCollection();

            var collection = new SqlParameterCollection();

            foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(parameters))
            {
                object obj2 = descriptor.GetValue(parameters);
                collection.Add(new SqlParameter("@" + descriptor.Name, obj2));
            }

            return collection;
        }
    }
}