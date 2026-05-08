using System;

namespace Application.Payload.Response.Auth
{
    public class LoginResponse
    {
        public string UserName { get; set; } = null!;
        public string Role { get; set; } = null!;
        public string Token { get; set; } = null!;
    }
}
