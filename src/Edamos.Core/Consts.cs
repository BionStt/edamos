using System;

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
            public const string ResponseTypeCodeToken = "code id_token";
            public const string CallbackPath = "/signin-oidc";
            public const string SignOutCallbackPath = "/signout-callback-oidc";
        }

        public static class Api
        {
            public const string ResourceId = "edamosapi";
        }

        public static class Logs
        {
            public const string EdamosApiCategory = "Edamos.Api";
        }

        public static class Kibana
        {
            public const string AppPath = "/app/kibana";
            public const string Host = "kibana";
            public const int Port = 5601;
            public const string Scheme = "http";
        }

        public static class InfluxDb
        {
            public const string Host = "influxdb";
            public const int Port = 8086;
            public const string Scheme = "http";
            public const string MetricsDbName = "db0";
            public const string RetentionPolicy = "p15m";

            public static Uri Uri { get; } = new UriBuilder(Scheme, Host, Port).Uri;
        }

        public static class RabbitMq
        {
            public const string Host = "rabbitmq";
            public const int Port = 15672;
            public const string Scheme = "http";
        }

        public static class Grafana
        {
            public const string Host = "grafana";
            public const int Port = 3000;
            public const string Scheme = "http";
        }
        
        public static class UsersCache
        {
            public const long SizeLimitMemoryBytes = 209715200; //200 MB
        }
    }
}