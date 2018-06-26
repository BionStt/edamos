namespace Edamos.Core
{
    public static class DebugConstants
    {
        public static class ConnectionStrings
        {
            public const string IdentityServerConfStore =
                "Data Source=db; Initial Catalog=model; User Id=sa; Password=edamoslocal@123; Enlist=false;";

            public const string IdentityServerOperationalStore = IdentityServerConfStore;
        }

        public static class ElasticSearch
        {
            public const string LoggingUri =
                "http://elasticsearch:9200";
        }
    }
}