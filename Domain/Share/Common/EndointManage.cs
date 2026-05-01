using AngleSharp.Text;

namespace Domain.Share.Common
{
    public static class EndpointManage
    {
        public const string ApiVersion = "api/v1";

        public static class Auth
        {
            public const string Login = $"{ApiVersion}/auth/login";
        }
    }
}
