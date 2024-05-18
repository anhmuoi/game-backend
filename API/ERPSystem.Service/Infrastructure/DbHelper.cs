using ERPSystem.Common.Infrastructure;
using ERPSystem.DataAccess.Models;
using ERPSystem.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;

namespace ERPSystem.Service.Infrastructure;

public class DbHelper
{
    /// <summary>
    ///  Create and return an UnitOfWork instance
    /// </summary>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IUnitOfWork CreateUnitOfWork(IConfiguration configuration, IHttpContextAccessor contextAccessor = null)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .EnableSensitiveDataLogging(true);

        options.UseNpgsql(configuration.GetConnectionString(Constants.Settings.DefaultConnection),
            sqlOptions =>
            {
                sqlOptions.MigrationsAssembly(Constants.Settings.ERPSystemDataAccess);
                sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(30), null);
                sqlOptions.CommandTimeout(3600);
            });
        options.ConfigureWarnings(
            warnings => warnings.Ignore(CoreEventId.ContextInitialized));

        var context = new AppDbContext(options.Options);
        return new UnitOfWork(context, contextAccessor);
    }

    public static string MakeEncQuery(string tableName, string columnName, string data, int id)
    {
        var query = $"UPDATE \"{tableName}\" SET \"{columnName}\" = pdb.\"enc\"('normal', '{data}', '') WHERE \"Id\" = {id};";

        return query;
    }
}