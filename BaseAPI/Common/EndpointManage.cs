namespace BaseAPI.Common
{
    public static class EndpointManage
    {
        public const string ApiVersion = "api/v1";

        public static class Auth
        {
            public const string Login = $"{ApiVersion}/auth/login";
            public const string ChangePassword = $"{ApiVersion}/auth/change-password";
        }

        public static class Users
        {
            public const string Base = $"{ApiVersion}/users";
            public const string Profile = $"{ApiVersion}/users/profile";
        }

        public static class Uploads
        {
            public const string Base = $"{ApiVersion}/uploads";
            public const string Upload = $"{Base}/upload";
            public const string UploadMultiple = $"{Base}/upload-multiple";
            public const string GoogleDrive = $"{Base}/google-drive";
        }
    }
}
