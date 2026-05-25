using FluentValidation;
using Karakatsiya.Constants;
using Karakatsiya.Data;
using Karakatsiya.Models.Entities.Common;
using Karakatsiya.Services;
using Karakatsiya.Services.BackgroundServices;
using Karakatsiya.Services.Behaviors;
using Karakatsiya.Services.Infrastructure;
using Karakatsiya.Services.Interfaces;
using Karakatsiya.Services.Tracker;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace Karakatsiya.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            services.AddLocalization(options => options.ResourcesPath = "Resources");

            services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));
            services.AddCustomControllers();
            services.AddCustomDatabase(config);
            services.AddCustomCors(config);
            services.AddCustomIdentity(config);
            services.AddCustomAuth(config);

            return services;
        }

        private static void AddCustomAuth(this IServiceCollection services, IConfiguration config)
        {
            var jwtKey = config[AppConstants.Config.JWT_KEY]
                ?? throw new InvalidOperationException(AppConstants.Others.CONFIG_MISSING_JWT);

            var keyBytes = Encoding.UTF8.GetBytes(jwtKey);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = config[AppConstants.Config.JWT_ISSUER],
                    ValidAudience = config[AppConstants.Config.JWT_AUDIENCE],
                    IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
                    ClockSkew = TimeSpan.Zero,
                    RoleClaimType = ClaimTypes.Role
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        if (context.Request.Cookies.ContainsKey("X-Access-Token"))
                        {
                            context.Token = context.Request.Cookies["X-Access-Token"];
                        }
                        return Task.CompletedTask;
                    }
                };
            });
        }

        private static void AddCustomIdentity(this IServiceCollection services, IConfiguration config)
        {
            // Пока просто оставляем пустым
        }

        public static IServiceCollection AddBusinessServices(this IServiceCollection services)
        {
            var assembly = typeof(ServiceCollectionExtensions).Assembly;

            services.AddHostedService<UnconfirmedUserCleanupWorker>();

            services.AddValidatorsFromAssembly(assembly);

            services.AddMemoryCache();
            services.AddSingleton<ICacheTracker, CacheTracker>();

            services.AddSingleton<ISanitizerService, SanitizerService>();

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(assembly);

                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(SanitizationBehavior<,>));
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));
            });

            services.AddScoped<IFileService, LocalFileService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IPhotoService, PhotoService>();

            return services;
        }

        private static void AddCustomControllers(this IServiceCollection services)
        {
            services.AddControllers()
                .AddJsonOptions(options => {
                    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
                    options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
                });
        }

        private static void AddCustomDatabase(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(config.GetConnectionString(AppConstants.Config.DEFAULT_CONNECTION))
            );
        }

        private static void AddCustomCors(this IServiceCollection services, IConfiguration config)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(AppConstants.Shared.CORS_POLICY_NAME, policy =>
                    policy.WithOrigins(
                        AppConstants.Shared.LOCALHOST,
                        AppConstants.Shared.DEV_DOMAIN,
                        AppConstants.Shared.PWA_MOBILE
                    )
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });
        }
    }
}