using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using AutoMapper;


namespace ERPSystem.Common;

public class ApplicationVariables
{
    public static IConfiguration Configuration;
    public static ILoggerFactory LoggerFactory;
    public static IHostingEnvironment Env;

    public static IMapper Mapper;
}