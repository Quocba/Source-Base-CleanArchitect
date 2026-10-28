namespace BaseAPI.DI
{
    using Serilog.Sinks.Discord;
    using Application.Common.Caching;
    using Application.Common.ElasticSearch;
    using Application.Interfaces;
    using Application.IService;
    using BaseAPI.Middleware.JWTMidlleware;
    using Domain;
    using Infrastructure.Config;
    using BaseAPI.Middleware.SecurityLog;
    using Domain.Entities;
    using Domain.Payload.Base;
    using Application.Common.Share.Common;
    using Elastic.Clients.Elasticsearch;
    using EmailService.Config;
    using EmailService.Implement;
    using EmailService.Interface;
    using Infrastructure.Context;
    using Infrastructure.Elasticsearch;
    using Infrastructure.GenericRepository;
    using Infrastructure.Service;
    using Infrastructure.UnitOfWork;
    using MassTransit;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Http;
    using Microsoft.IdentityModel.Tokens;
    using Microsoft.OpenApi;
    using Newtonsoft.Json;
    using RabbitMQContract.Config;
    using RabbitMQContract.Consumer.Email;
    using RabbitMQContract.Generic;
    using RedisService.IService;
    using RedisService.Service;
    using Serilog;
    using Serilog.Events;
    using StackExchange.Redis;
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;
    using Infrastructure.KeyHandle;
    using Application.Helper;

    public class DependencyInjection
    {
        public static void Register(IServiceCollection services, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment environment)
        {
            #region Serilog Configuration

            var webHookId = configuration["DiscordWebHook:WebHookId"];
            var webHookToken = configuration["DiscordWebHook:WebHookToken"];
            var hookId = configuration["DiscordWebHook:DevHookId"];
            var hookToken = configuration["DiscordWebHook:DevHookToken"];

            var loggerConfig = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Console(
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}"
                )
                .WriteTo.File(
                    path: "Logs/app-.log",
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff}] [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}",
                    retainedFileCountLimit: 10
                );

            if (ulong.TryParse(webHookId, out var parsedWebHookId) && !string.IsNullOrWhiteSpace(webHookToken))
            {
                loggerConfig.WriteTo.Discord(
                    webhookId: parsedWebHookId,
                    webhookToken: webHookToken,
                    restrictedToMinimumLevel: LogEventLevel.Error
                );
            }

            if (ulong.TryParse(hookId, out var parsedHookId) && !string.IsNullOrWhiteSpace(hookToken))
            {
                loggerConfig.WriteTo.Logger(lc => lc
                    .Filter.ByIncludingOnly(le =>
                        le.Level == LogEventLevel.Information &&
                        le.MessageTemplate.Text.Contains("ðŸ§‘â€ðŸ’»"))
                    .WriteTo.Discord(
                        webhookId: parsedHookId,
                        webhookToken: hookToken
                    )
                );
            }

            Log.Logger = loggerConfig.CreateLogger();

            #endregion

            #region Database Configuration

            #region Google Drive Configuration
            services.AddScoped<IGoogleDriveService, GoogleDriveService>();
            #endregion

            services.AddDbContext<DBContext>(opt =>
            {
                opt.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
                opt.ConfigureWarnings(warnings => warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
            });

            #endregion

            #region Email Settings
            services.Configure<SendMailConfig>(configuration.GetSection("EmailSettings"));
            services.AddScoped<IEmailSender, EmailSender>();
            #endregion

            #region RabbitMQ
            //services.Configure<RabbitMQConfig>(configuration.GetSection("RabbitMQ"));
            //var rabbitSettings = configuration.GetSection("RabbitMQ").Get<RabbitMQConfig>();

            //services.AddMassTransit(x =>
            //{
            //    x.AddConsumer<EmailConsumer>();
            //    x.AddConsumer<EmailSendFileConsumer>();

            //    x.UsingRabbitMq((context, cfg) =>
            //    {
            //        if (rabbitSettings != null && !string.IsNullOrEmpty(rabbitSettings.HostName))
            //        {
            //            cfg.Host(rabbitSettings.HostName, (ushort)rabbitSettings.Port, rabbitSettings.VirtualHost, h =>
            //            {
            //                h.Username(rabbitSettings.UserName);
            //                h.Password(rabbitSettings.Password);
            //            });

            //            cfg.ReceiveEndpoint("send_email_queue", e =>
            //            {
            //                e.ConfigureConsumer<EmailConsumer>(context);
            //            });

            //            cfg.ReceiveEndpoint("send_file_email_queue", e =>
            //            {
            //                e.ConfigureConsumer<EmailSendFileConsumer>(context);
            //            });
            //        }
            //    });
            //});
            #endregion

            #region UnitOfWork & Generic Repository
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            //services.AddScoped<IQueueRepository, QueueRepository>();

            services.AddScoped<IUnitOfWork>(provider =>
            {
                var context = provider.GetRequiredService<DBContext>();
                return new UnitOfWork<DBContext>(context);
            });
            #endregion

            #region Security Settings
            services.Configure<SecuritySettings>(configuration.GetSection("SecuritySettings"));
            #endregion

            #region REDIS
            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var redisConnectionString = configuration.GetSection("Redis:ConnectionString").Value;
                if (string.IsNullOrWhiteSpace(redisConnectionString))
                {
                    redisConnectionString = "localhost:6379";
                }
                var options = ConfigurationOptions.Parse(redisConnectionString);
                options.AbortOnConnectFail = false;
                return ConnectionMultiplexer.Connect(options);
            });
            services.AddScoped<IRedisService, RedisServices>();
            #endregion

            #region Elasticsearch
            services.AddSingleton<ElasticsearchClient>(sp =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                var uri = config["Elasticsearch:Uri"] ?? "http://localhost:9200";

                var settings = new ElasticsearchClientSettings(new Uri(uri))
                    .EnableDebugMode();

                return new ElasticsearchClient(settings);
            });

            services.AddSingleton<IElasticIndexNameResolver, ElasticIndexNameResolver>();
            services.AddScoped(typeof(IElasticRepository<>), typeof(ElasticRepository<>));
            #endregion

            #region HTTP & JSON
            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
                    options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.Never;
                });

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = new List<string>();
                    foreach (var state in context.ModelState.Values)
                    {
                        foreach (var error in state.Errors)
                        {
                            errors.Add(error.ErrorMessage);
                        }
                    }

                    var response = new ApiResponse<object>
                    {
                        StatusCode = StatusCode.BadRequest,
                        Message = string.Join(", ", errors),
                        Data = null
                    };

                    return new BadRequestObjectResult(response);
                };
            });
            #endregion

            #region MediatR & Caching
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));
            services.AddMemoryCache();
            services.AddScoped(typeof(GenericCacheInvalidator<>));
            services.AddScoped<GenericCacheInvalidator<User>>();
            #endregion

            #region Key Handles
            services.AddScoped<IApiKeyValidator, ApiKeyValidator>();
            #endregion

            #region General ASP.NET Core Services
            services.AddHttpContextAccessor();
            services.AddHttpClient();
            services.AddEndpointsApiExplorer();
            services.AddSerilog();
            #endregion

            #region JWT Service & Settings
            var jwtSection = configuration.GetSection("JwtSettings");
            services.Configure<JwtSettings>(jwtSection);
            var jwtSettings = jwtSection.Get<JwtSettings>() ?? new JwtSettings();
            services.AddSingleton(jwtSettings);
            services.AddSingleton<IJWTService, JWTService>();
            services.AddScoped<IGenerateCodeService, GenerateCodeService>();
            #endregion

            #region Dynamic Policies & Authentication
            services.AddSingleton<IAuthorizationPolicyProvider, DynamicPermissionPolicyProvider>();
            services.AddScoped<IAuthorizationHandler, PermissionHandler>();
            services.AddScoped<IAuthorizationHandler, AnyPolicyHandler>();

            var key = Encoding.UTF8.GetBytes(!string.IsNullOrEmpty(jwtSettings.Key) ? jwtSettings.Key : "YourDefaultSecretKeyHere_MustBeAtLeast32BytesLong!");

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = !string.IsNullOrEmpty(jwtSettings.Issuer),
                    ValidIssuer = jwtSettings?.Issuer,
                    ValidateAudience = !string.IsNullOrEmpty(jwtSettings.Audience),
                    ValidAudience = jwtSettings?.Audience,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero,
                    RoleClaimType = ClaimTypes.Role
                };

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception is SecurityTokenExpiredException)
                        {
                            context.Response.Headers.Append("Token-Expired", "true");
                        }
                        return Task.CompletedTask;
                    },
                    OnChallenge = context =>
                    {
                        context.HandleResponse();
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";

                        var response = new ApiResponse<string>
                        {
                            StatusCode = StatusCode.Unauthorized,
                            Message = "Báº¡n khÃ´ng cÃ³ quyá»n truy cáº­p vÃ o tÃ i nguyÃªn nÃ y",
                            Data = null
                        };
                        return context.Response.WriteAsync(JsonConvert.SerializeObject(response));
                    }
                };
            });

            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
            });
            #endregion

            #region OPENAPI
            services.AddOpenApi("v1", options =>
            {
                options.AddDocumentTransformer((document, context, cancellationToken) =>
                {
                    document.Info = new OpenApiInfo
                    {
                        Title = "Clean Architecture Source Base API",
                        Version = "v1",
                        Description = "Source Base Clean Architecture chuáº©n má»±c vá»›i Authentication, Dynamic Policy, Caching vÃ  Stored Procedure phÃ¢n trang"
                    };

                    document.Components ??= new OpenApiComponents();
                    document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
                    document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer",
                        BearerFormat = "JWT",
                        In = ParameterLocation.Header,
                        Name = "Authorization",
                        Description = "Nháº­p 'Bearer {JWT_TOKEN}'"
                    };

                    document.Security ??= new List<OpenApiSecurityRequirement>();
                    document.Security.Add(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecuritySchemeReference("Bearer", document),
                            new List<string>()
                        }
                    });

                    return Task.CompletedTask;
                });
            });
            #endregion

            #region CORS
            services.AddCors(cors =>
            {
                cors.AddPolicy("Allow", policy =>
                {
                    policy
                        .SetIsOriginAllowed(_ => true)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                });
            });
            #endregion

            #region Helper
            services.AddScoped<DataHelper>();
            #endregion
        }
    }
}




