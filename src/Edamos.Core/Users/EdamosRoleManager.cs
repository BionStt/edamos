using System;
using System.Threading.Tasks;
using App.Metrics;
using Edamos.Core.Cache;
using Microsoft.AspNetCore.Identity;

namespace Edamos.Core.Users
{
    public class EdamosRoleManager : DistributedCacheWrapper<EdamosRoleManager, IdentityRole>, IRoleManager<IdentityRole>
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public EdamosRoleManager(
            RoleManager<IdentityRole> roleManager,
            IDistributedCache<IdentityRole> distributedCache,
            IMetrics metrics) : base(distributedCache, metrics)
        {
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
        }       
    }
}