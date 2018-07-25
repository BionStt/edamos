﻿using System;
using Microsoft.AspNetCore.Identity;

namespace Edamos.Core.Users
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    [Serializable]
    public class ApplicationUser : IdentityUser
    {
    }
}
