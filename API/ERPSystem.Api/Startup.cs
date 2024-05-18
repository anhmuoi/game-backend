using System.Text;
using AutoMapper;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Prometheus;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using NSwag;
using NSwag.Generation.Processors.Security;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using ERPSystem.DataModel.API;
using ERPSystem.Common.Infrastructure;
using ERPSystem.DataAccess.Models;
using ERPSystem.Service.Handler;
using ERPSystem.Repository;
using ERPSystem.Service.Services;
using ERPSystem.Common;
using ERPSystem.Service;
using FluentScheduler;

namespace ERPSystem.Api
{
    /// <summary>
    /// 
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="env"></param>
        /// <param name="locOptions"></param>
        public Startup(IHostingEnvironment env)
        {
            Console.WriteLine(@"Environment: " + env.EnvironmentName);
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        /// <summary>
        /// Configuration
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            #region Bind Strongly Type Config

            var jwtSection = Configuration.GetSection(Constants.Settings.JwtSection);
            var jwtOptions = new JwtOptionsModel();
            jwtSection.Bind(jwtOptions);
            services.Configure<JwtOptionsModel>(jwtSection);


            #endregion

            #region framework services

            // configration for localiztion
            services.AddLocalization(options => options.ResourcesPath = Constants.Settings.ResourcesDir);
            services.Configure<RequestLocalizationOptions>(
                opts =>
                {
                    opts.RequestCultureProviders = new List<IRequestCultureProvider>
                    {
                        new QueryStringRequestCultureProvider(),
                        new CookieRequestCultureProvider()
                    };
                });

            // Service for init mapping
            services.AddAutoMapper(typeof(Startup).Assembly);

            // Config authentication
            services.AddAuthentication(options =>
            {
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            });
            
            // Config Authorize
            services.AddAuthorization(options =>
            {
                options.AddPolicy(Constants.Policy.PrimaryAdmin, policy =>
                    policy.RequireClaim(Constants.ClaimName.Role,
                        ((short) RoleType.PrimaryManager).ToString()));

                options.AddPolicy(Constants.Policy.Dynamic, policy =>
                    policy.RequireClaim(Constants.ClaimName.Role,
                        ((short) RoleType.DynamicRole).ToString()));

                options.AddPolicy(Constants.Policy.PrimaryAdminAndDynamic, policy =>
                    policy.RequireClaim(Constants.ClaimName.Role,
                        ((short) RoleType.PrimaryManager).ToString(),
                        ((short) RoleType.DynamicRole).ToString()));
            });

            services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(60);
            });

            // Add bearer authentication
            services.AddAuthentication()
                .AddJwtBearer(cfg =>
                {
                    //cfg.RequireHttpsMetadata = false;
                    cfg.SaveToken = true;
                    cfg.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = true,
                        ValidateIssuer = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey)),
                        ValidIssuer = jwtOptions.Issuer,
                        ValidAudience = jwtOptions.Issuer,
                        // Validate the token expiry  
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };
                });

            // Config the db connection string
            services.AddEntityFrameworkNpgsql().AddDbContext<AppDbContext>(options =>
            {
                var connection = Environment.GetEnvironmentVariable(Constants.Settings.DefaultEnvironmentConnection);
                if (string.IsNullOrEmpty(connection))
                {
                    connection = Configuration.GetConnectionString(Constants.Settings.DefaultConnection);
                }

                options.UseNpgsql(connection,
                    sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly(Constants.Settings.ERPSystemDataAccess);
                        // Configuring Connection Resiliency: 
                        // https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency 
                        sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(30), new List<string>());
                        sqlOptions.CommandTimeout(3600);
                    });
                // Changing default behavior when client evaluation occurs to throw. 
                // Default in EF Core would be to log a warning when client evaluation is performed.
                options.ConfigureWarnings(
                    warnings => warnings.Ignore(CoreEventId.ContextInitialized));
                // Check Client vs. Server evaluation: 
                // https://docs.microsoft.com/en-us/ef/core/querying/client-eval
                options.UseLoggerFactory(new LoggerFactory()).EnableSensitiveDataLogging();
            });

            // NSwag Tools
            services.AddOpenApiDocument(document =>
            {
                document.AddSecurity("JWT", Enumerable.Empty<string>(), new NSwag.OpenApiSecurityScheme
                {
                    Type = NSwag.OpenApiSecuritySchemeType.ApiKey,
                    Name = "Authorization",
                    In = NSwag.OpenApiSecurityApiKeyLocation.Header,
                    Description = Constants.Swagger.Description,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                document.Title = Constants.Swagger.Title;
                document.Version = Constants.Swagger.V1;
                document.Description = Constants.Swagger.Description;
                document.OperationProcessors.Add(
                    new AspNetCoreOperationSecurityScopeProcessor("JWT"));
            });

            // Cors Config
            services.AddCors(options =>
            {
                options.AddPolicy(Constants.Swagger.CorsPolicy,
                    builder => builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
            });
            services.AddLogging(builder => builder.AddConsole(options => options.TimestampFormat = "yyyy:MM:dd hh:mm:ss "));

            // Adds a default in-memory implementation of IDistributedCache
            services.AddDistributedMemoryCache();
            // Custom service
            services.AddRouting(options => options.LowercaseUrls = true);

            services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            #endregion

            #region Application services

            // Common services
            services.AddScoped<IJwtHandler, JwtHandler>();
            services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            //services.AddScoped<ValidateModelFilter>();
            services.AddSingleton<ITempDataProvider, CookieTempDataProvider>();

            // Services business
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IDepartmentService, DepartmentService>();
            services.AddScoped<IWorkScheduleService, WorkScheduleService>();
            services.AddScoped<IDriverService, DriverService>();
            services.AddScoped<IWorkLogService, WorkLogService>();
            services.AddScoped<IDailyReportService, DailyReportService>();
            services.AddScoped<IFolderLogService, FolderLogService>();
            services.AddScoped<IMailTemplateService, MailTemplateService>();
            services.AddScoped<IMailService, MailService>();
            services.AddScoped<IMeetingLogService, MeetingLogService>();
            services.AddScoped<IMeetingRoomService, MeetingRoomService>();

            services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ApiVersionReader = new HeaderApiVersionReader("x-api-version");
            });

            services.AddMvc(options => options.EnableEndpointRouting = false)
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                })
                .AddControllersAsServices()
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddSessionStateTempDataProvider()
                .AddFluentValidation(fvc =>
                {
                    fvc.RegisterValidatorsFromAssemblyContaining<Startup>();
                    fvc.DisableDataAnnotationsValidation = true;
                })
                .AddViewLocalization(
                    LanguageViewLocationExpanderFormat.Suffix,
                    opts => { opts.ResourcesPath = Constants.Settings.ResourcesDir; })
                .AddDataAnnotationsLocalization();
            services.AddControllers();

            #endregion
        }
        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="loggerFactory"></param>
        /// <param name="configuration"></param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory,
            IConfiguration configuration)
        {
            ApplicationVariables.Env = env;
            ApplicationVariables.Configuration = configuration;
            // ApplicationVariables.LicenseVerified = false;
            // Rate Limit
            //app.UseIpRateLimiting();
            // app.UseClientRateLimiting();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseOpenApi(a => a.PostProcess = (document, _) =>
                {
                    document.Schemes.Clear();
                    document.Schemes = new[] { OpenApiSchema.Http };
                });
            }
            else
            {
                app.UseExceptionHandler(Constants.Route.ErrorPage);
                app.UseHsts();
                app.UseOpenApi(a => a.PostProcess = (document, _) =>
                {
                    document.Schemes.Clear();
                    document.Schemes = new[] { OpenApiSchema.Https, OpenApiSchema.Http };
                });
            }

            app.UseSwaggerUi3();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMetricServer();
            app.UseHttpMetrics();

            // run cronjob 
            JobManager.Initialize(new CronjobService(Configuration));

            
            //RabbitMQ consumer registration
            IConsumerService consumerService = new ConsumerService(ApplicationVariables.Configuration);
            consumerService.Register();

            var logDir = Path.GetDirectoryName(Configuration[Constants.Logger.LogFile]);
            if (!Directory.Exists(logDir))
            {
                Directory.CreateDirectory(logDir);
            }

            //call ConfigureLogger in a centralized place in the code
            loggerFactory.AddFile(Configuration[Constants.Logger.LogFile], LogLevel.Error);
            loggerFactory.AddFile(Configuration[Constants.Logger.LogFile], LogLevel.Warning);
            var options = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(options.Value);
            app.UseMiddleware<RequestLocalizationMiddleware>();
            app.UseCors(Constants.Swagger.CorsPolicy);
            app.UseMvcWithDefaultRoute();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
            app.UseCookiePolicy();
        }
    }
}