using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TheMulti0.Console;

namespace PostsListener.Service
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            await CreateHostBuilder(args).RunConsoleAsync();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureHostConfiguration(builder => builder.AddEnvironmentVariables())
                .ConfigureAppConfiguration(
                    (context, builder) =>
                    {
                        IHostEnvironment env = context.HostingEnvironment;

                        builder
                            .SetBasePath(env.ContentRootPath)
                            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true)
                            .AddEnvironmentVariables();
                    })
                .ConfigureLogging(
                    (context, builder) => builder
                        .AddConfiguration(context.Configuration)
                        .AddTheMulti0Console()
                        .AddSentry())
                .ConfigureServices(
                    (context, services) => new Startup(context.Configuration).ConfigureServices(services));
        }
    }
}