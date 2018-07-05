using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Edamos.Core;
using Edamos.Core.Users;
using Edamos.Core.Users.Data;
using IdentityServer4;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Identity;
using Client = IdentityServer4.EntityFramework.Entities.Client;

namespace Edamos.IdentityServer.Data
{
    public static class SeedData
    {
        private const string UiClientId = "ui";
        private const string UiLoginApiResource = "ui";

        public static void Clients(ConfigurationDbContext context)
        {
            if (!context.ApiResources.Any(r => r.Name == UiLoginApiResource))
            {
                ApiResource resource = new ApiResource(UiLoginApiResource, "Main UI");                

                context.ApiResources.Add(resource.ToEntity());

                context.SaveChanges();
            }

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

        public static void Users(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            ApplicationUser user = userManager.FindByIdAsync(Consts.Identity.AdminUserId).Result;

            if (user == null)
            {
                IdentityResult result = userManager.CreateAsync(new ApplicationUser
                {
                    Id = Consts.Identity.AdminUserId,
                    UserName = Consts.Identity.AdminUserId
                }, "EdamosAdmin!23").Result;                
            }

            IdentityRole role = roleManager.FindByIdAsync(Consts.Identity.AdminRoleId).Result;

            if (role == null)
            {
                IdentityResult result = roleManager.CreateAsync(new IdentityRole
                {
                    Id = Consts.Identity.AdminRoleId,
                    Name = Consts.Identity.AdminRoleId,
                    ConcurrencyStamp = DateTime.UtcNow.Ticks.ToString("D")
                }).Result;
            }

            user = userManager.FindByIdAsync(Consts.Identity.AdminUserId).Result;

            if (!userManager.IsInRoleAsync(user, Consts.Identity.AdminRoleId).Result)
            {
                IdentityResult result = userManager.AddToRoleAsync(user, Consts.Identity.AdminRoleId).Result;
            }
        }        
    }
}