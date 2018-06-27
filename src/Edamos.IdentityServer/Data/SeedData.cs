using System.Collections.Generic;
using System.Linq;
using Edamos.Core;
using IdentityServer4;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Client = IdentityServer4.EntityFramework.Entities.Client;

namespace Edamos.IdentityServer.Data
{
    public static class SeedData
    {
        private const string UiClientId = "ui";
        private const string UiLoginApiResource = "ui";

        public static void Clients(ConfigurationDbContext context)
        {
            //if (!context.ApiResources.Any(r => r.Name == UiLoginApiResource))
            //{
            //    ApiResource resource = new ApiResource();
            //    resource.Name = UiLoginApiResource;
            //    resource.DisplayName = "Main UI";
                
            //    context.ApiResources.Add(resource.ToEntity());

            //    context.SaveChanges();
            //}

            // TODO: configure clients            
            if (!context.Clients.Any(client => client.ClientId == UiClientId))
            {
                IdentityServer4.Models.Client client = new IdentityServer4.Models.Client();
                client.ClientId = UiClientId;
                client.ClientName = "EDAMOS UI";
                client.AllowedGrantTypes = GrantTypes.HybridAndClientCredentials;
                client.ClientSecrets = new[] { new Secret(DebugConstants.Ui.ClientSecret.Sha256()) }; // TODO: configure secret
                client.AllowedScopes = new[] { IdentityServerConstants.StandardScopes.OpenId };

                client.RedirectUris = new[] {"https://edamos.example.com/signin-oidc"};
                client.PostLogoutRedirectUris = new[] {"https://edamos.example.com/signout-callback-oidc"};

                context.Clients.Add(client.ToEntity());

                context.SaveChanges();
            }
        }

        public static void IdentityResources(ConfigurationDbContext context)
        {
            IdentityResources.OpenId openId = new IdentityResources.OpenId();

            if (!context.IdentityResources.Any(resource => resource.Name == openId.Name))
            {
                context.IdentityResources.Add(openId.ToEntity());

                context.SaveChanges();
            }            
        }
    }
}