namespace SerialService.Services.Interfaces
{
    using System;
    using DAL.Entities;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;

    public interface IUserService
    {
        ApplicationUser Get(string id);

        IdentityResult Create(ApplicationUser entity, string pass, params string[] roleNames);

        IdentityResult Remove(string id);

        List<ApplicationUser> GetAll();

        List<ApplicationUser> GetWithCondition(Func<ApplicationUser, bool> predicate);

        List<ApplicationUser> GetWithConditions(params Func<ApplicationUser, bool>[] predicates);

        ApplicationUser GetScalarWithCondition(Func<ApplicationUser, bool> predicate);

		/// <summary>
		/// Получить одного юзера по его главному строковому свойству (Email). Поиск не зависит от регистра.
		/// </summary>
		/// <param name="value">Значение для поиска.</param>
		ApplicationUser GetByMainStringProperty(string value);

        List<ApplicationUser> GetByRole(string roleName);

        IEnumerable<ApplicationUser> GetByUserNamePart(string namePart);

        IdentityResult Ban(string id, DateTime until);

        IdentityResult Unban(string id);

        IdentityResult ConfirmEmail(string id, string code);

		string GenerateEmailConfirmationToken(string id);

		string GeneratePasswordResetToken(string id);

        void SendEmail(string id, string subject, string message);

        IdentityResult SetUserName(string id, string userName);

        IdentityResult SetPassword(string id, string currentPassword, string newPassword);

        IdentityResult SetAvatar(string id, string newAvatarUrl);

        IdentityResult SetEmail(string id, string newEmail);

        IdentityResult SetParole(string id, string newParole);

        bool CheckPassword(string id, string password);

        ApplicationUser GetByUserName(string value);

        /// <summary>
        /// Изменить роли пользователя
        /// </summary>
        /// <param name="roleNames">список названий ролей</param>
        /// <returns></returns>
        void UpdateUserRoles(IEnumerable<IdentityRole> roles,string userId);

		bool CheckKey(string userId, string key);

		IdentityResult SetKey(string userId, string key);

		IdentityResult ChangeEmail(string userId, string newEmail, string key);

		void SendToCustomEmail(string email, string subject, string message);

		IdentityResult Update(ApplicationUser user);
	}
}
