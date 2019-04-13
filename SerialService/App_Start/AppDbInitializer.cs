namespace SerialService.App_Start
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System.Collections.Generic;
    using System.Data.Entity;
    using DAL.Context;
    using DAL.Entities;
    using Infrastructure.Helpers;
    using DAL.Repository;

    public class AppDbInitializer : CreateDatabaseIfNotExists<ApplicationDbContext> // DropCreateDatabaseIfModelChanges<ApplicationDbContext>
	{
        /// <summary>
        /// Создание первоначальной базы данных
        /// </summary>
        /// <param name="context">Контескст данных</param>
        protected override void Seed(ApplicationDbContext context)
        {
            var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            var adminRole = new IdentityRole { Name = "Admin" };
            var userRole = new IdentityRole { Name = "User" };
            var redactorRole = new IdentityRole { Name = "Redactor" };//Редакторы, имеют только возможность дополнения
            var moderatorRole = new IdentityRole { Name = "Moderator" };
            var locked = new IdentityRole { Name = "Locked" };//Забаненные пользователи
            List<IdentityRole> roles = new List<IdentityRole> { adminRole, userRole, redactorRole, moderatorRole, locked };

            for (int i = 0; i < roles.Count; i++)
            {
                roleManager.Create(roles[i]);
            }

            // Учетная запись главного администратора.
            var admin = new ApplicationUser
            {
                Email = "aartman@rambler.ru",
                UserName = "Admin",
                IsLocked = false,
                LockoutEnabled = false,//Админа нельзя заблокировать
                Parole = CalculatorMD5.CreateMD5("R2d0D1s8"),
				EmailConfirmed = true,
                AvatarURL = "https://avatars.mds.yandex.net/get-pdb/367895/9d25fd18-b8ce-4114-9429-26fb34ffa2ee/s1200"

            };

            string adminPass = "R2d0D1s8";
            var createResult = userManager.Create(admin, adminPass);

            if (createResult.Succeeded)
            {
                userManager.AddToRole(admin.Id, Resource.AdminRoleName);
                userManager.AddToRole(admin.Id, Resource.UserRoleName);
                userManager.AddToRole(admin.Id, Resource.RedactorRoleName);
                userManager.AddToRole(admin.Id, Resource.ModeratorRoleName);
            }

            var user1 = new ApplicationUser
            {
                Email = "maxkarab@yandex.ru",
                UserName = "RainyDays",
                IsLocked = false,
                LockoutEnabled = false,
                Parole = CalculatorMD5.CreateMD5("35356"),
				EmailConfirmed = true,
				AvatarURL = "https://pp.userapi.com/c849024/v849024490/4f6d4/mP4Cozt2mMw.jpg"
            };

            var result1 = userManager.Create(user1, "abc123");

            if (result1.Succeeded)
            {
                userManager.AddToRole(user1.Id, Resource.AdminRoleName);
                userManager.AddToRole(user1.Id, Resource.UserRoleName);
                userManager.AddToRole(user1.Id, Resource.RedactorRoleName);
                userManager.AddToRole(user1.Id, Resource.ModeratorRoleName);
            }

			var user2 = new ApplicationUser
			{
				Email = "maxkarab@motus-cinema.com",
				UserName = "SuperAdmin",
				IsLocked = false,
				LockoutEnabled = false,
				Parole = CalculatorMD5.CreateMD5("35356"),
				EmailConfirmed = true,
				AvatarURL = "https://pp.userapi.com/c849024/v849024490/4f6d4/mP4Cozt2mMw.jpg"
			};

			var result2 = userManager.Create(user2, "abc123");

			if (result2.Succeeded)
			{
				userManager.AddToRole(user2.Id, Resource.AdminRoleName);
				userManager.AddToRole(user2.Id, Resource.UserRoleName);
				userManager.AddToRole(user2.Id, Resource.RedactorRoleName);
				userManager.AddToRole(user2.Id, Resource.ModeratorRoleName);
			}
			base.Seed(context);
        }
    }
}