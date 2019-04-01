using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerialService.Services.Interfaces
{
    public interface IRoleService
    {
        /// <summary>
        /// Получить роль
        /// </summary>
        /// <param name="id">Id роли</param>
        IdentityRole Get(string id);

        /// <summary>
        /// Получить роль по названию
        /// </summary>
        IdentityRole GetByName(string roleName);

        /// <summary>
        /// Получить все роли
        /// </summary>
        /// <returns></returns>
        IEnumerable<IdentityRole> GetAll();


        /// <summary>
        /// Имеет ли пользователь роль
        /// </summary>
        bool UserIsInRole(string userId,string roleName);

        IEnumerable<IdentityRole> GetUserRoles(string id);
    }
}
