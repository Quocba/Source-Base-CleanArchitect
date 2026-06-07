using API.Middleware.GlobalLogger;
using API.Middleware.JWTMidlleware;
using BaseAPI.DI;
using BaseAPI.Middleware.JWTMidlleware;
using BaseAPI.Middleware.SecurityLog;
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
app.UseSharedStaticFiles(builder.Configuration);

//app.UseHttpsRedirection();
app.UseAuthentication();
//app.UseMiddleware<TokenFingerprintMiddleware>();
app.UseAuthorization();
app.MapControllers();

app.Run();

public record UserInfo(string Name, int Age, string Email);

