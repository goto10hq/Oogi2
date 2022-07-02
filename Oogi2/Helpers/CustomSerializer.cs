using System.IO;
using System.Text;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Oogi2.Helpers
{
    public class CustomSerializer : CosmosSerializer
    {
        static readonly Encoding DefaultEncoding = new UTF8Encoding(false, true);
        readonly JsonSerializer _serializer;

        public CustomSerializer()
        {
            _serializer = new JsonSerializer
            {
                Formatting = Formatting.None,
                TypeNameHandling = TypeNameHandling.None,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Converters = { new JsonClaimConverter() }
            };
        }

        /// <summary>
        /// Logic for deserialization.
        /// </summary>
        public override T FromStream<T>(Stream stream)
        {
            using (stream)
            {
                if (typeof(Stream).IsAssignableFrom(typeof(T)))
                {
                    return (T)(object)stream;
                }

                using (var sr = new StreamReader(stream))
                {
                    using (var jsonTextReader = new JsonTextReader(sr))
                    {
                        return _serializer.Deserialize<T>(jsonTextReader);
                    }
                }
            }
        }

        /// <summary>
        /// Serialization logic.
        /// </summary>
        public override Stream ToStream<T>(T input)
        {
            var streamPayload = new MemoryStream();

            using (var streamWriter = new StreamWriter(streamPayload, encoding: DefaultEncoding, bufferSize: 1024, leaveOpen: true))
            {
                using (var writer = new JsonTextWriter(streamWriter))
                {
                    writer.Formatting = Formatting.None;
                    _serializer.Serialize(writer, input);
                    writer.Flush();
                    streamWriter.Flush();
                }
            }

            streamPayload.Position = 0;

            return streamPayload;
        }
    }
}