using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Scraper.Net
{
    public class InterfaceConverter<T> : JsonConverter<T> where T : class
    {
        private const string TypeDiscriminator = "$type";
        private const string ValueDiscriminator = "$value";

        public override T Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            using JsonDocument document = JsonDocument.ParseValue(ref reader);
            JsonElement rootElement = document.RootElement;

            string typeName = rootElement.GetProperty(TypeDiscriminator).GetString();
            
            Type type = Type.GetType(typeName!) 
                        ?? throw new InvalidOperationException();

            string media = rootElement.GetProperty(ValueDiscriminator).GetRawText();
            
            return JsonSerializer.Deserialize(media!, type, options) as T;
        }

        public override void Write(
            Utf8JsonWriter writer,
            T value, 
            JsonSerializerOptions options)
        {
            if (value is null)
            {
                writer.WriteNullValue();
                return;
            }

            writer.WriteStartObject();
            
            Type type = value.GetType();
            writer.WriteString(TypeDiscriminator, type.FullName);
            
            writer.WritePropertyName(ValueDiscriminator);
            JsonSerializer.Serialize(writer, value, value.GetType(), options);
            
            writer.WriteEndObject();
        }
    }
}