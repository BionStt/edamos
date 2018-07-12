using System.Collections;
using System.Collections.Generic;

namespace Edamos.AspNetCore.Identity
{
    public class OpenIdCookieSettings
    {
        public string Authority { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public ICollection<string> Scopes { get; set; }
        public bool SaveTokens { get; set; } = false;
    }
}