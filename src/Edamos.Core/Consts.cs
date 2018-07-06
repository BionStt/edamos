namespace Edamos.Core
{
    public static class Consts
    {
        public static class Identity
        {
            public const string AdminUserId = "admin";
            public const string AdminRoleId = "admin";
        }

        public static class OpenId
        {
            public const string SchemaName = "oidc";
            public const string CallbackPath = "/signin-oidc";
            public const string SignOutCallbackPath = "/signout-callback-oidc";
        }
    }
}