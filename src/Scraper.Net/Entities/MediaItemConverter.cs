using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Scraper.Net
{
    public class MediaItemConverter: JsonConverter<IMediaItem>
    {
        private const string TypeDiscriminator = "$type";

        public override IMediaItem Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            using JsonDocument document = JsonDocument.ParseValue(ref reader);
            JsonElement rootElement = document.RootElement;
            string rawText = rootElement.GetRawText();

            var type = Type.GetType(rootElement.GetProperty(TypeDiscriminator).GetString());

            return JsonSerializer.Deserialize(ref reader, type) as IMediaItem;
        }

        public override void Write(
            Utf8JsonWriter writer,
            IMediaItem value, 
            JsonSerializerOptions options)
        {
            if (value is null)
            {
                writer.WriteNullValue();
                return;
            }

            writer.WriteStartObject();
            
            Type type = value.GetType();

            writer.WriteString(TypeDiscriminator, type.Name);
            
            foreach (PropertyInfo property in type.GetProperties())
            {
                if (!property.CanRead)
                {
                    continue;
                }
                
                writer.WritePropertyName(property.Name);

                JsonSerializer.Serialize(writer, property.GetValue(value), options);
            }
            writer.WriteEndObject();
        }
    }
}