using Elasticsearch.Net;

namespace Edamos.AspNetCore
{
    public class OpenIdCookieSettings
    {
        public string Authority { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public bool SaveTokens { get; set; } = false;
    }
}