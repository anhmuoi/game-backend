using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using ERPSystem.Common;
using ERPSystem.Common.Infrastructure;
using ERPSystem.Repository;
using Microsoft.AspNetCore;

namespace ERPSystem.Api;
class Program
{
    public static void Main(string[] args)
    {
        var host = CreateWebHostBuilder(args).Build();
        using (var scope = host.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var loggerFactory = ApplicationVariables.LoggerFactory = services.GetRequiredService<ILoggerFactory>();
            
            var configuration = services.GetRequiredService<IConfiguration>();
            var unitOfWork = services.GetRequiredService<IUnitOfWork>();
            DbInitializer.Initialize(unitOfWork, configuration);
        }

        host.Run();
    }

    private static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
        WebHost.CreateDefaultBuilder(args)
            .ConfigureKestrel((context, options) =>
            {
                // Set properties and call methods on options
            })
            .UseUrls($"http://*:5000")
            .UseIISIntegration()
            .UseSentry()
            .UseStartup<Startup>();
}