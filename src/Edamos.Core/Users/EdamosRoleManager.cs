using System;
using Microsoft.AspNetCore.Identity;

namespace Edamos.Core.Users
{
    public class EdamosRoleManager : IRoleManager<IdentityRole>
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public EdamosRoleManager(
            RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
        }       
    }
}