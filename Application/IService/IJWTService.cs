using Domain.Config;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.IService
{
    public interface IJWTService
    {
        string GenerateToken(User user);
        string GenerateFingerprint(HttpContext context);
        string? GetUserId(ClaimsPrincipal user);
        string? GetUser();
        string GenerateActivationToken(string email);
    }
}
