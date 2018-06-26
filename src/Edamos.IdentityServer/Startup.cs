﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Edamos.Core;
using IdentityServer4.Configuration;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Edamos.IdentityServer
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            IIdentityServerBuilder isb = services.AddIdentityServer(options =>
            {
                this.Configuration.Bind(nameof(IdentityServerOptions), options);
            });

            //TODO: use recommended approach for signing credentials
            isb.AddDeveloperSigningCredential();

            isb.AddConfigurationStore(options =>
            {
                //TODO: use real connection sting
                options.ConfigureDbContext = builder =>
                    builder.UseSqlServer(DebugConstants.ConnectionStrings.IdentityServerConfStore,
                        sql => sql.MigrationsAssembly(migrationsAssembly));
            });

            // this adds the operational data from DB (codes, tokens, consents)
            isb.AddOperationalStore(options =>
            {
                //TODO: use real connection sting
                options.ConfigureDbContext = builder =>
                    builder.UseSqlServer(DebugConstants.ConnectionStrings.IdentityServerOperationalStore,
                        sql => sql.MigrationsAssembly(migrationsAssembly));
            });

            MigrateDatabase(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseIdentityServer();

            app.UseStaticFiles();

            app.UseMvcWithDefaultRoute();            
        }

        private static void MigrateDatabase(IServiceCollection services)
        {
            using (var scope = services.BuildServiceProvider().GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                scope.ServiceProvider.GetService<PersistedGrantDbContext>().Database.Migrate();

                scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>().Database.Migrate();
            }
        }
    }
}