using System;
using MassTransit;
using Newtonsoft.Json;

namespace Scraper.MassTransit.Common
{
    public static class JsonConfigurator
    {
        public static void ConfigureInterfaceJsonSerialization(
            this IBusFactoryConfigurator cfg,
            params Type[] interfaces)
        {
            foreach (Type type in interfaces)
            {
                cfg.ConfigureJsonSerializer(Configure(type));
                cfg.ConfigureJsonDeserializer(Configure(type));    
            }
        }

        private static Func<JsonSerializerSettings, JsonSerializerSettings> Configure(Type type)
        {
            return settings =>
            {
                settings.Converters.Add(new InterfaceJsonConverter(type));
                
                return settings;
            };
        }
    }
}