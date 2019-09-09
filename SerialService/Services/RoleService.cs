using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SerialService.DAL.Context;
using SerialService.DAL.Entities;
using SerialService.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace SerialService.Services
{
    public class RoleService : IRoleService
    {
        private IDbContext db;
        private RoleManager<IdentityRole> roleManager;
        private RoleStore<IdentityRole> roleStore;

        public RoleService(IDbContext context)
        {
            this.db = context;
            this.roleStore = new RoleStore<IdentityRole>((ApplicationDbContext)context);
            this.roleManager  = new RoleManager<IdentityRole>(roleStore);
        }

        /// <summary>
        /// Получить экземляр по ID
        /// </summary>
        public IdentityRole Get(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }
            return this.roleManager.FindById(id);
        }

        public IdentityRole GetByName(string roleName)
        {
            if (string.IsNullOrEmpty(roleName))
            {
                return null;
            }
            return this.roleManager.FindByName(roleName);
        }

        /// <summary>
        /// Получить все
        /// </summary>
        public IEnumerable<IdentityRole> GetAll()
        {
            return this.roleManager.Roles.ToList();
        }

        public bool UserIsInRole(string userId, string roleName)
        {
            if (string.IsNullOrEmpty(userId)||string.IsNullOrEmpty(roleName))
            {
                return false;
            }
            return this.GetByName(roleName).Users.Any(u => u.UserId == userId);
        }

        public IEnumerable<IdentityRole> GetUserRoles(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }
            return this.GetAll().Where(r => r.Users.Any(u => u.UserId == id));
        }
    }
}