namespace Edamos.Core
{
    public static class DebugConstants
    {
        public static class ConnectionStrings
        {
            public const string IdentityServerConfStore =
                "Data Source=db; Initial Catalog=model; User Id=sa; Password=edamoslocal@123; Enlist=false;";

            public const string IdentityServerOperationalStore = IdentityServerConfStore;

            public const string IdentityUsersStore = IdentityServerConfStore;
        }

        public static class Redis
        {
            public const string DataProtectionHost = "redisdp";

            public const string UsersCacheHost = "redis";

            public const int DataProtectionPort = 6379;
        }

        public static class ElasticSearch
        {
            public const string LoggingUri =
                "http://elasticsearch:9200";
        }

        public static class Ui
        {
            public const string RootAddress = "https://edamos.example.com";
            public const string ClientSecret = "secret";
            public const string ClientId = "ui";
        }

        public static class AdminUi
        {
            public const string RootAddress = "https://admin.edamos.example.com";
            public const string ClientSecret = "secretAdmin";
            public const string ClientId = "adm";
        }

        public static class KibanaUi
        {
            public const string RootAddress = "https://kibana.edamos.example.com";
            public const string ClientSecret = "secretKibana";
            public const string ClientId = "kbn";
        }

        public static class IdentityServer
        {
            public const string Authority = "https://login.edamos.example.com";
        }
    }
}