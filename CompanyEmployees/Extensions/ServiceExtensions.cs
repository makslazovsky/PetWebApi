using CompanyEmployees.Formatters;
using Contracts;
using Entities;
using LoggerService;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Repository;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Versioning;
using Marvin.Cache.Headers;
using AspNetCoreRateLimit;
using System.Collections.Generic;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using System;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace CompanyEmployees.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                builder.AllowAnyOrigin()
                .AllowAnyOrigin()
                .AllowAnyHeader());
            });
        }
        public static void ConfigureIISIntegration(this IServiceCollection services) =>
            services.Configure<IISOptions>(options =>
            {
            });
        public static void ConfigureLoggerService(this IServiceCollection services)
        { 
            services.AddScoped<ILoggerManager, LoggerManager>();
        }


        public static void ConfigureSqlContext(this IServiceCollection services, IConfiguration configuration) => 
            services.AddDbContext<RepositoryContext>(opts => 
            opts.UseSqlServer(configuration.GetConnectionString("sqlConnection"), b =>
            b.MigrationsAssembly("CompanyEmployees")));

        public static void ConfigureRepositoryManager(this IServiceCollection services) => 
            services.AddScoped<IRepositoryManager, RepositoryManager>();

        public static IMvcBuilder AddCustomCSVFormatter(this IMvcBuilder builder) =>
            builder.AddMvcOptions(config =>
            config.OutputFormatters.Add(new CsvOutputFormatter()));

        public static void AddCustomMediaTypes(this IServiceCollection services)//24
        { 
            services.Configure<MvcOptions>(config => 
            { 
                var newtonsoftJsonOutputFormatter = config.OutputFormatters.OfType<NewtonsoftJsonOutputFormatter>()?.FirstOrDefault();
                if (newtonsoftJsonOutputFormatter != null)
                { 
                    newtonsoftJsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.codemaze.hateoas+json");
                    newtonsoftJsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.codemaze.apiroot+json");
                } 
                var xmlOutputFormatter = config.OutputFormatters.OfType<XmlDataContractSerializerOutputFormatter>()?.FirstOrDefault();
                if (xmlOutputFormatter != null)
                { 
                    xmlOutputFormatter.SupportedMediaTypes.Add("application/vnd.codemaze.hateoas+xml");
                    xmlOutputFormatter.SupportedMediaTypes.Add("application/vnd.codemaze.apiroot+xml");
                } 
            });
        }

        public static void ConfigureVersioning(this IServiceCollection services) 
        { 
            services.AddApiVersioning(opt => 
            { 
                opt.ReportApiVersions = true;
                opt.AssumeDefaultVersionWhenUnspecified = true;
                opt.DefaultApiVersion = new ApiVersion(1, 0);
                opt.ApiVersionReader = new HeaderApiVersionReader("api-version");
            });
        }

        public static void ConfigureResponseCaching(this IServiceCollection services) =>
            services.AddResponseCaching();

        public static void ConfigureHttpCacheHeaders(this IServiceCollection services) =>
            services.AddHttpCacheHeaders(
                (expirationOpt) => 
                { 
                    expirationOpt.MaxAge = 65;
                    expirationOpt.CacheLocation = CacheLocation.Private;
                }, 
                (validationOpt) => 
                { 
                    validationOpt.MustRevalidate = true; 
                });

        public static void ConfigureRateLimitingOptions(this IServiceCollection services)
        { 
            var rateLimitRules = new List<RateLimitRule> 
            { 
                new RateLimitRule 
                { 
                    Endpoint = "*",
                    Limit = 30,
                    Period = "5m"
                } 
            }; 
            services.Configure<IpRateLimitOptions>(opt =>
            { 
                opt.GeneralRules = rateLimitRules;
            }); 
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
            services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
        }

        public static void ConfigureIdentity(this IServiceCollection services)
        {
            var builder = services.AddIdentityCore<User>(o =>
            { 
                o.Password.RequireDigit = true;
                o.Password.RequireLowercase = false;
                o.Password.RequireUppercase = false;
                o.Password.RequireNonAlphanumeric = false;
                o.Password.RequiredLength = 10;
                o.User.RequireUniqueEmail = true;
            });
            builder = new IdentityBuilder(builder.UserType, typeof(IdentityRole), builder.Services);
            builder.AddEntityFrameworkStores<RepositoryContext>().AddDefaultTokenProviders();
        }

        public static void ConfigureJWT(this IServiceCollection services, IConfiguration configuration)
        { 
            var jwtSettings = configuration.GetSection("JwtSettings");
            var secretKey = Environment.GetEnvironmentVariable("SECRET");
            services.AddAuthentication(opt => 
            { 
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
            { 
                options.TokenValidationParameters = new TokenValidationParameters 
                { 
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.GetSection("validIssuer").Value,
                    ValidAudience = jwtSettings.GetSection("validAudience").Value,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
                }; 
            }); 
        }
    }


}
