using BaseAPI.DI;
using BaseAPI.Middleware;
using BaseAPI.Middleware.GlobalLogger;
using BaseAPI.Middleware.JWTMidlleware;
using BaseAPI.Middleware.SecurityLog;
using Domain.Payload.Base;
using Application.Common.Share.Common;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Http;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = null;
    serverOptions.Limits.Http2.MaxStreamsPerConnection = 1000;
    serverOptions.Limits.Http2.InitialConnectionWindowSize = 2 * 1024 * 1024;
    serverOptions.Limits.Http2.InitialStreamWindowSize = 1024 * 1024;
});

builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = int.MaxValue;
});

DependencyInjection.Register(builder.Services, builder.Configuration, new HttpContextAccessor(), builder.Environment);
var app = builder.Build();

// Middleware tÃ­nh toÃ¡n thá»i gian thá»±c thi (Timing) cá»§a API
app.UseMiddleware<ResponseTimingMiddleware>();

app.UseCors("Allow");
app.UseMiddleware<SecurityMiddleware>();
app.UseMiddleware<ExceptionLoggingMiddleware>();
app.UseStaticFiles();
app.UseSharedStaticFiles(builder.Configuration);

app.UseAuthentication();
app.UseAuthorization();

app.MapOpenApi();
app.MapScalarApiReference(options =>
{
    options.WithTitle("Clean Architecture API")
           .WithTheme(ScalarTheme.Saturn)
           .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
});
app.MapControllers();

app.Run();

