using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Security.Claims;

namespace Oogi2.Helpers
{
    public class JsonClaimConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(Claim));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var claim = (Claim)value;

            JObject jo = new JObject
            {
                { "Type", claim.Type },
                { "Value", claim.Value },
                { "ValueType", claim.ValueType },
                { "Issuer", claim.Issuer },
                { "OriginalIssuer", claim.OriginalIssuer }
            };

            jo.WriteTo(writer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);
            string type = (string)jo["Type"];
            JToken token = jo["Value"];
            var value = token.Type == JTokenType.String ? (string)token : token.ToString(Formatting.None);
            var valueType = (string)jo["ValueType"];
            var issuer = (string)jo["Issuer"];
            var originalIssuer = (string)jo["OriginalIssuer"];

            return new Claim(type, value, valueType, issuer, originalIssuer);
        }
    }
}