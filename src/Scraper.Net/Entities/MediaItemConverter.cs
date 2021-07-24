using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Scraper.Net
{
    public class MediaItemConverter: JsonConverter<IMediaItem>
    {
        public override IMediaItem Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            return JsonSerializer.Deserialize(ref reader, typeToConvert) as IMediaItem;
        }

        public override void Write(
            Utf8JsonWriter writer,
            IMediaItem value, 
            JsonSerializerOptions options)
        {
            switch (value)
            {
                case null:
                    JsonSerializer.Serialize(writer, (IMediaItem) null, options);
                    break;
                
                default:
                {
                    Type type = value.GetType();
                    JsonSerializer.Serialize(writer, value, type, options);
                    break;
                }
            }
        }
    }
}