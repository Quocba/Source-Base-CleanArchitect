namespace BaseAPI.DI
{
    using Application.Common.ElasticSearch;
    using Application.Features.Auth.Command.Login;
    using Application.IGenericRepository;
    using Application.IService;
    using Application.IUnitOfWork;
    using BaseAPI.Middleware.JWTMidlleware;
    using Domain;
    using Domain.Config;
    using Domain.Entities;
    using Domain.Payload.Base;
    using Domain.Share.Common;
    using Elastic.Clients.Elasticsearch;
    using EmailService.Config;
    using EmailService.Implement;
    using EmailService.Interface;
    using FluentValidation.AspNetCore;
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
    using Microsoft.Win32;
    using Newtonsoft.Json;
    using RabbitMQContract.Config;
    using RabbitMQContract.Consumer;
    using RabbitMQContract.Consumer.Email;
    using RabbitMQContract.Generic;
    using RedisService.IService;
    using RedisService.Service;
    using Serilog;
    using Serilog.Events;
    using Serilog.Sinks.Discord;
    using StackExchange.Redis;
    using System;
    using System.Reflection;
    using System.Text;
    using System.Threading.RateLimiting;
    using Domain.KeyHandle;
    using Microsoft.IdentityModel.Tokens;
    using Microsoft.OpenApi.Models;
    using Microsoft.AspNetCore.DataProtection;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.OpenApi;
    using MediatR;

#pragma warning disable
    public class DependencyInjection
    {
        public static void Register(IServiceCollection services, IConfiguration configuration, HttpContextAccessor contextAccessor, IWebHostEnvironment env)
        {
            #region MEDIATOR
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(IUnitOfWork).Assembly);
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(Application.Common.Behaviors.TransactionRollbackBehavior<,>));
            });
            #endregion

            #region Service Configuration
            services.AddSingleton<IJWTService, JWTService>();
            #endregion

            #region Repository Configuration
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IQueueRepository, QueueRepository>();
            #endregion

            #region Cache Configuration

            services.AddMemoryCache();
            services.AddScoped(typeof(GenericCacheInvalidator<>));

            #endregion

            #region Serilog Config
            services.AddSerilog();

            var webHookId = configuration["Discord:WebHookId"];
            var webHookToken = configuration["Discord:WebHookToken"];

            var hookId = configuration["DiscordChangeDataLog:HookID"];
            var hookToken = configuration["DiscordChangeDataLog:HookToken"];


            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console(
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"
                )
                .WriteTo.File(
                    path: "Logs/log-.txt",
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}",
                    restrictedToMinimumLevel: LogEventLevel.Information
                )
                .WriteTo.Discord(
                    webhookId: ulong.Parse(webHookId),
                    webhookToken: webHookToken,
                    restrictedToMinimumLevel: LogEventLevel.Error
                )
                .WriteTo.Logger(lc => lc
                    .Filter.ByIncludingOnly(le =>
                        le.Level == LogEventLevel.Information &&
                        le.MessageTemplate.Text.Contains("🧑‍💻"))
                    .WriteTo.Discord(
                        webhookId: ulong.Parse(hookId),
                        webhookToken: hookToken
                    )
                )
                .CreateLogger();

            #endregion

            #region Database Configuration

            services.AddDbContext<DBContext>(opt =>
            {
                opt.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
                opt.ConfigureWarnings(warnings => warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
            });

            services.AddDbContext<QueueDbContext>();

            #endregion

            #region Email Settings
            services.Configure<SendMailConfig>(configuration.GetSection("EmailSettings"));
            services.AddScoped<IEmailSender, EmailSender>();

            #endregion

            #region RabbitMQ
            services.Configure<RabbitMQConfig>(configuration.GetSection("RabbitMQ"));
            var rabbitSettings = configuration.GetSection("RabbitMQ").Get<RabbitMQConfig>();

                services.AddMassTransit(x =>
                {
                    // x.AddConsumer<EmailConsumer>();
                    // x.AddConsumer<EmailSendFileConsumer>();
                    // x.AddConsumer<DbActionConsumer>();
                    // x.AddConsumer<GenericQueueConsumer>();
                    x.UsingRabbitMq((context, cfg) =>
                    {
                        cfg.Host(rabbitSettings.HostName, rabbitSettings.VirtualHost, h =>
                        {
                            h.Username(rabbitSettings.UserName);
                            h.Password(rabbitSettings.Password);
                        });

/*
                        cfg.ReceiveEndpoint("email-queue", e =>
                        {
                            e.ConfigureConsumer<EmailConsumer>(context);
                        });

                        cfg.ReceiveEndpoint("email-send-file", e =>
                        {
                            e.ConfigureConsumer<EmailSendFileConsumer>(context);
                        });

                        cfg.ReceiveEndpoint("generic-queue", e =>
                        {
                            e.ConfigureConsumer<GenericQueueConsumer>(context);
                            e.PrefetchCount = 20;
                            e.ConcurrentMessageLimit = 10;
                        });
*/

                    });
                });


            #endregion

            // services.AddHostedService<DepreciationBackgroundService>();

            #region UNIT OF WORK
            services.AddScoped<IUnitOfWork>(provider =>
            {
                var context = provider.GetRequiredService<DBContext>();
                return new UnitOfWork<DBContext>(context);
            });
            #endregion

            #region RATE LIMIT
            //services.AddRateLimiter(options =>
            //{
            //    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
            //        RateLimitPartition.GetFixedWindowLimiter(
            //            partitionKey: context.User.Identity?.Name ?? context.Connection.RemoteIpAddress?.ToString(),
            //            factory: _ => new FixedWindowRateLimiterOptions
            //            {
            //                PermitLimit = 10,
            //                Window = TimeSpan.FromSeconds(1),
            //                QueueLimit = 100,
            //                AutoReplenishment = true
            //            }));
            //});
            #endregion

            #region REDIS
            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var redisConnectionString = configuration.GetSection("Redis:ConnectionString").Value;

                var options = ConfigurationOptions.Parse(redisConnectionString);
                options.AbortOnConnectFail = false;
                return ConnectionMultiplexer.Connect(options);
            });
            services.AddScoped<IRedisService, RedisServices>();
            #endregion

            #region Google Drive Configuration
            services.AddScoped<IGoogleDriveService, GoogleDriveService>();
            #endregion

            #region CUSTOM MODELSTATE RESPONSE
            services.Configure<ApiBehaviorOptions>(cf =>
            {
                cf.InvalidModelStateResponseFactory = context =>
                {
                    var firstError = context.ModelState
                        .SelectMany(x => x.Value.Errors)
                        .Select(e => e.ErrorMessage)
                        .FirstOrDefault();

                    var response = new ApiResponse<object>
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = firstError ?? "Dữ liệu không hợp lệ",
                        Data = null
                    };

                    return new BadRequestObjectResult(response);
                };
            });

            #endregion

            #region Elasticsearch

            services.AddSingleton<ElasticsearchClient>(sp =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                var uri = config["Elasticsearch:Uri"];

                var settings = new ElasticsearchClientSettings(new Uri(uri))
                    .EnableDebugMode();

                return new ElasticsearchClient(settings);
            });

            services.AddSingleton<IElasticIndexNameResolver, ElasticIndexNameResolver>();

            services.AddScoped(typeof(IElasticRepository<>), typeof(ElasticRepository<>));

            #endregion

            #region HTTP & JSON
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
            });
            services.AddHttpClient();
            services.AddHttpContextAccessor();
            services.AddSingleton<IApiKeyValidator, ApiKeyValidator>();
            services.AddEndpointsApiExplorer();
            services.AddDataProtection()
                    .PersistKeysToFileSystem(new DirectoryInfo(@"C:\keys"))
                    .SetApplicationName("SourceBase");
            #endregion

            #region JWT & AUTH
            var jwtSection = configuration.GetSection("JwtSettings");
            services.Configure<JwtSettings>(jwtSection);
            var jwtSettings = jwtSection.Get<JwtSettings>();
            services.AddSingleton(jwtSettings);

            services.AddSingleton<IAuthorizationPolicyProvider, DynamicPermissionPolicyProvider>();
            services.AddScoped<IAuthorizationHandler, AnyPolicyHandler>();

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
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
                };

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        context.NoResult();
                        context.Response.StatusCode = 401;
                        context.Response.ContentType = "application/json";
                        return context.Response.WriteAsync("{\"error\":\"Token không hợp lệ hoặc đã hết hạn.\"}");
                    },
                    OnChallenge = context =>
                    {
                        context.HandleResponse();
                        context.Response.StatusCode = 401;
                        context.Response.ContentType = "application/json";
                        return context.Response.WriteAsync(JsonConvert.SerializeObject(new ApiResponse<string>
                        {
                            StatusCode = StatusCode.Unauthorized,
                            Message = "Bạn chưa đăng nhập hoặc token không hợp lệ.",
                            Data = null
                        }));
                    },
                    OnForbidden = context =>
                    {
                        context.Response.StatusCode = 403;
                        context.Response.ContentType = "application/json";
                        var response = new ApiResponse<string>
                        {
                            StatusCode = StatusCode.Forbidden,
                            Message = "Bạn không có quyền truy cập vào tài nguyên này.",
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
                        Title = "BaseAPI",
                        Version = "v1",
                        Description = "API dùng JWT và test bằng Scalar"
                    };

                    document.Components ??= new OpenApiComponents();
                    document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer",
                        BearerFormat = "JWT",
                        In = ParameterLocation.Header,
                        Name = "Authorization",
                        Description = "Nhập 'Bearer {JWT_TOKEN}'"
                    };

                    document.SecurityRequirements.Add(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
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
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            });
            #endregion

        }
    }
}
