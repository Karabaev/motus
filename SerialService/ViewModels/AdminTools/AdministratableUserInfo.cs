using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SerialService.ViewModels.AdminTools
{
    public class AdministratableUserInfo
    {
        public PersonalAccountViewModel PersonalUserData { get; set; }

        public List<IdentityRole> UserRoles { get; set; }
    }
}