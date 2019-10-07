using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Serilog;
using Serilog.Events;

namespace WebStore
{
    public class Program
    {
        public static void Main(string[] args) =>
            CreateWebHostBuilder(args)
               .Build()
               .Run();

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                //.ConfigureLogging(
                //     (host, log) =>
                //     {
                //         log.AddFilter<ConsoleLoggerProvider>("System", LogLevel.Error);
                //         log.AddFilter<ConsoleLoggerProvider>("Microsoft", LogLevel.Warning);
                //     })
                //.UseUrls("http://0.0.0.0:8080")
                .UseStartup<Startup>()
                .UseSerilog(
                    (host, log) => log
                       .ReadFrom.Configuration(host.Configuration)
                       .MinimumLevel.Debug()
                       .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                       .Enrich.WithMachineName()
                       .Enrich.WithEnvironmentUserName()
                       .WriteTo.File("Logs\\WebStore-{Date}.log", outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}")
                       .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss.fff} {Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}"));
    }
}
