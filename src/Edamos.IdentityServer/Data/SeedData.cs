﻿using System;
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

namespace Edamos.IdentityServer.Data
{
    public static class SeedData
    {
        private const string UiLoginApiResource = "ui";

        public static void Clients(ConfigurationDbContext context)
        {
            if (!context.ApiResources.Any(r => r.Name == UiLoginApiResource))
            {
                ApiResource resource = new ApiResource(UiLoginApiResource, "Main UI");                

                context.ApiResources.Add(resource.ToEntity());

                context.SaveChanges();
            }

            if (!context.ApiResources.Any(r => r.Name == Consts.Api.ResourceId))
            {
                ApiResource resource = new ApiResource(Consts.Api.ResourceId, "Main API");

                context.ApiResources.Add(resource.ToEntity());

                context.SaveChanges();
            }

            // TODO: configure clients            
            if (!context.Clients.Any(client => client.ClientId == DebugConstants.Ui.ClientId))
            {
                IdentityServer4.Models.Client client = new IdentityServer4.Models.Client();
                client.ClientId = DebugConstants.Ui.ClientId;
                client.ClientName = "EDAMOS UI";
                client.AllowedGrantTypes = GrantTypes.HybridAndClientCredentials;
                client.ClientSecrets = new[] { new Secret(DebugConstants.Ui.ClientSecret.Sha256()) }; // TODO: configure secret
                client.AllowedScopes = new[]
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    Consts.Api.ResourceId
                };

                client.RedirectUris = new[] { DebugConstants.Ui.RootAddress + Consts.OpenId.CallbackPath };
                client.PostLogoutRedirectUris = new[] { DebugConstants.Ui.RootAddress + Consts.OpenId.SignOutCallbackPath };
                client.RequireConsent = false;
                context.Clients.Add(client.ToEntity());

                context.SaveChanges();
            }

            if (!context.Clients.Any(client => client.ClientId == DebugConstants.AdminUi.ClientId))
            {
                IdentityServer4.Models.Client client = new IdentityServer4.Models.Client();
                client.ClientId = DebugConstants.AdminUi.ClientId;
                client.ClientName = "EDAMOS ADMIN";
                client.AllowedGrantTypes = GrantTypes.HybridAndClientCredentials;
                client.ClientSecrets = new[] { new Secret(DebugConstants.AdminUi.ClientSecret.Sha256()) }; // TODO: configure secret
                client.AllowedScopes = new[]
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    Consts.Api.ResourceId
                };

                client.RedirectUris = new[] { DebugConstants.AdminUi.RootAddress + Consts.OpenId.CallbackPath };
                client.PostLogoutRedirectUris = new[] { DebugConstants.AdminUi.RootAddress + Consts.OpenId.SignOutCallbackPath };
                client.RequireConsent = false;
                context.Clients.Add(client.ToEntity());

                context.SaveChanges();
            }

            if (!context.Clients.Any(client => client.ClientId == DebugConstants.ProxyUi.ClientId))
            {
                Client client = new Client();
                client.ClientId = DebugConstants.ProxyUi.ClientId;
                client.ClientName = "EDAMOS PROXY UI";
                client.AllowedGrantTypes = GrantTypes.HybridAndClientCredentials;
                client.ClientSecrets = new[] { new Secret(DebugConstants.ProxyUi.ClientSecret.Sha256()) }; // TODO: configure secret
                client.AllowedScopes = new[]
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile
                };

                client.RedirectUris = new[]
                {
                    DebugConstants.ProxyUi.KibanaRootAddress + Consts.OpenId.CallbackPath,
                    DebugConstants.ProxyUi.GrafanaRootAddress + Consts.OpenId.CallbackPath,
                    DebugConstants.ProxyUi.RabbitMqRootAddress + Consts.OpenId.CallbackPath,
                };
                client.PostLogoutRedirectUris = new[]
                {
                    DebugConstants.ProxyUi.KibanaRootAddress + Consts.OpenId.SignOutCallbackPath,
                    DebugConstants.ProxyUi.GrafanaRootAddress + Consts.OpenId.SignOutCallbackPath,
                    DebugConstants.ProxyUi.RabbitMqRootAddress + Consts.OpenId.SignOutCallbackPath,
                };
                client.RequireConsent = false;
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

            IdentityResources.Profile profile = new IdentityResources.Profile();

            if (!context.IdentityResources.Any(resource => resource.Name == profile.Name))
            {
                context.IdentityResources.Add(profile.ToEntity());

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
#if DEBUG
            ApplicationUser testUser = userManager.FindByIdAsync("tid").Result;

            if (testUser == null)
            {
                IdentityResult result = userManager.CreateAsync(new ApplicationUser
                {
                    Id = "tid",
                    UserName = "tname"
                }, "EdamosTest!23").Result;
            }
#endif
        }        
    }
}