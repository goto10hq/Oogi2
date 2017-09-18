using System.ComponentModel;
using Microsoft.Azure.Documents;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Oogi2
{
    public static class Tools
    {
        /// <summary>
        /// Set Json default settings.
        /// </summary>
        public static void SetJsonDefaultSettings()
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                Formatting = Formatting.None,
                TypeNameHandling = TypeNameHandling.None,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
        }

        /// <summary>
        /// Escape value.
        /// </summary>
        public static string ToEscapedString(this string value)
        {
            value = value ?? string.Empty;
            return value.Replace(@"\", @"\\").Replace("'", @"\'");
        }

        /// <summary>
        /// Convert anonymous object to parameters.
        /// </summary>
        public static SqlParameterCollection AnonymousObjectToSqlParameters(object parameters)
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
