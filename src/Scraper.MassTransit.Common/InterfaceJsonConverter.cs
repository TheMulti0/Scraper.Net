using System;
using Newtonsoft.Json;

namespace Scraper.MassTransit.Common
{
    internal class InterfaceJsonConverter : JsonConverter
    {
        private readonly Type _type;
        private readonly JsonSerializer _serializer;

        public InterfaceJsonConverter(Type type)
        {
            _type = type;
            _serializer = new JsonSerializer
            {
                TypeNameHandling = TypeNameHandling.All
            };
        }

        public override void WriteJson(
            JsonWriter writer,
            object value,
            JsonSerializer serializer)
        {
            _serializer.Serialize(writer, value, _type);
        }

        public override object ReadJson(
            JsonReader reader,
            Type objectType,
            object existingValue, 
            JsonSerializer serializer)
        {
            return _serializer.Deserialize(reader, _type);
        }

        public override bool CanConvert(Type objectType)
        {
            return _type.IsAssignableFrom(objectType);
        }
    }
}