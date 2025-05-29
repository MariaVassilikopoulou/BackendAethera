using Microsoft.Azure.Cosmos;
using System.Text.Json;

namespace Aethera.Dependencies
{
    internal class SystemTextJsonCosmosSerializer : CosmosSerializer
    {
        private readonly JsonSerializerOptions _options;

        public SystemTextJsonCosmosSerializer(JsonSerializerOptions options)
        {
            _options = options;
        }

        public override T FromStream<T>(Stream stream)
        {
            if (stream == null || stream.Length == 0) return default!;

            using var reader = new StreamReader(stream);
            var json = reader.ReadToEnd();
            return JsonSerializer.Deserialize<T>(json, _options)!;
        }

        public override Stream ToStream<T>(T input)
        {
            var stream = new MemoryStream();
            using var writer = new Utf8JsonWriter(stream);
            JsonSerializer.Serialize(writer, input, _options);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}