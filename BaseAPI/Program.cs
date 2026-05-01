using API.Middleware.GlobalLogger;
using API.Middleware.JWTMidlleware;
using BaseAPI.DI;
using BaseAPI.Middleware.JWTMidlleware;
using Domain.Config;
using Domain.KeyHandle;
using Domain.Payload.Base;
using Domain.Share.Common;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.Http;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Scalar.AspNetCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddSingleton<IApiKeyValidator, ApiKeyValidator>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(@"C:\keys"))
                                    .SetApplicationName("NgocDaiAPI");
#region  HTTP Configuration
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
});

builder.Services.AddHttpContextAccessor();
var httpContextAccessor = builder.Services.BuildServiceProvider()
    .GetRequiredService<IHttpContextAccessor>();
#endregion

#region JWT & SCALAR Configuration
var jwtSection = builder.Configuration.GetSection("JwtSettings");
builder.Services.Configure<JwtSettings>(jwtSection);
var jwtSettings = jwtSection.Get<JwtSettings>();
builder.Services.AddSingleton(jwtSettings);

builder.Services.AddSingleton<IAuthorizationPolicyProvider, DynamicPermissionPolicyProvider>();
builder.Services.AddScoped<IAuthorizationHandler, AnyPolicyHandler>();

builder.Services
    .AddAuthentication(options =>
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

builder.Services.AddAuthorization(options =>
{
    options.DefaultPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

builder.Services.AddOpenApi("v1", options =>
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

#region CROS
builder.Services.AddCors(cors =>
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

DependencyInjection.Register(builder.Services, builder.Configuration, new HttpContextAccessor(), builder.Environment);
var app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference(options =>
{
    options.Theme = ScalarTheme.Saturn;
});


app.UseCors("Allow");
app.UseMiddleware<SecurityMiddleware>();
//app.UseMiddleware<ExceptionLoggingMiddleware>();
app.UseStaticFiles();

var sharedRoot = @"C:\inetpub\wwwroot\NgocDaiBackend\Sharing";
var projectSharedPath = Path.Combine(sharedRoot, "NgocDai");
if (Directory.Exists(projectSharedPath))
{
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(projectSharedPath),
        RequestPath = ""
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
//app.UseMiddleware<TokenFingerprintMiddleware>();
app.UseAuthorization();
app.MapControllers();

app.Run();

public record UserInfo(string Name, int Age, string Email);

