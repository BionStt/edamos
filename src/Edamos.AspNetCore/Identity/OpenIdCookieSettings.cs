namespace Edamos.AspNetCore.Identity
{
    public class OpenIdCookieSettings
    {
        public string Authority { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public bool SaveTokens { get; set; } = false;
    }
}