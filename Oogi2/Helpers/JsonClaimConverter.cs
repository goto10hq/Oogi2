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
                { "type", claim.Type },
                { "value", claim.Value },
                { "valueType", claim.ValueType },
                { "issuer", claim.Issuer },
                { "originalIssuer", claim.OriginalIssuer }
            };

            jo.WriteTo(writer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);
            string type = (string)jo["type"];
            JToken token = jo["value"];
            var value = token.Type == JTokenType.String ? (string)token : token.ToString(Formatting.None);
            var valueType = (string)jo["valueType"];
            var issuer = (string)jo["issuer"];
            var originalIssuer = (string)jo["originalIssuer"];

            return new Claim(type, value, valueType, issuer, originalIssuer);
        }
    }
}