using ERPSystem.Common;
using ERPSystem.Common.Infrastructure;
using ERPSystem.DataAccess.Models;
using ERPSystem.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ERPSystem.Service;

public class HelperService
{
    private readonly ILogger _logger;
    private readonly IConfiguration _configuration;
    
    public HelperService(IConfiguration configuration)
    {
        _configuration = configuration;
        _logger = ApplicationVariables.LoggerFactory.CreateLogger<HelperService>();
    }
    
    public static IUnitOfWork CreateUnitOfWork(IConfiguration configuration, IHttpContextAccessor? contextAccessor = null)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>().EnableSensitiveDataLogging();

        options.UseNpgsql(configuration.GetConnectionString(Constants.Settings.DefaultConnection),
            sqlOptions =>
            {
                sqlOptions.MigrationsAssembly(Constants.Settings.ERPSystemDataAccess);
                sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(30), null);
                sqlOptions.CommandTimeout(3600);
            });
        options.UseLoggerFactory(new LoggerFactory()).EnableSensitiveDataLogging();

        var context = new AppDbContext(options.Options);
        return new UnitOfWork(context, contextAccessor ?? new HttpContextAccessor());
    }

  
    public static string JsonConvertCamelCase(object obj)
    {
        return JsonConvert.SerializeObject(obj, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
    }
}