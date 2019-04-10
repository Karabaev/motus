namespace SerialService.Services
{
    using Interfaces;
    using System.Collections.Generic;
    using System.Linq;
    using DAL.Context;
    using SerialService.DAL.Entities;
    using System;
    using Infrastructure.Exceptions;
    using Microsoft.AspNet.Identity;
    using System.Threading.Tasks;
    using Infrastructure.Helpers;
    using DAL.Repository;
    using Infrastructure;
    using Microsoft.AspNet.Identity.EntityFramework;

    public class UserService : IUserService // todo: надо полностью перевести логику на сервис
    {
        private ApplicationDbContext context;
        private ApplicationUserManager manager;

        public UserService(ApplicationDbContext context)
        {
            this.context = context;
            this.manager = ApplicationUserManager.Create(context);
        }


        /// <summary>
        /// Создать новую запись пользователя.
        /// </summary>
        /// <param name="entity">Объект пользователя.</param>
        /// <param name="pass">Пароль пользователя.</param>
        public IdentityResult Create(ApplicationUser entity, string pass, params string[] roleNames)
        {
            if (entity == null || string.IsNullOrWhiteSpace(pass))
                return null;

            if (this.GetScalarWithCondition(au => au.Id == entity.Id) != null)
                throw new EntryAlreadyExistsException("Пользователь с таким идентификатором уже существует.");

            if (this.GetByMainStringProperty(entity.Email) != null)
                throw new EntryAlreadyExistsException("Пользователь с такой почтой уже существует.");

            if (this.GetByUserName(entity.UserName) != null)
                throw new EntryAlreadyExistsException("Пользователь с таким именем уже существует.");

            entity.Parole = entity.Parole.CreateMD5();
            IdentityResult result = this.manager.Create(entity, pass);

            foreach (var item in roleNames)
                this.manager.AddToRole(entity.Id, item);

            return result;
        }

        /// <summary>
        /// Получить объект по его идентификатору.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ApplicationUser Get(string id)
        {
            return this.manager.FindById(id);
        }

        /// <summary>
        /// Вернуть всех юзеров.
        /// </summary>
        /// <returns></returns>
        public List<ApplicationUser> GetAll()
        {
            return this.context.Users.ToList();
        }

        /// <summary>
        /// Получить одного юзера по его главному строковому свойству (Email). Поиск не зависит от регистра.
        /// </summary>
        /// <param name="value">Значение для поиска.</param>
        public ApplicationUser GetByMainStringProperty(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            return this.GetScalarWithCondition(au => au.Email.ToLower() == value.ToLower());
        }

        /// <summary>
        /// Получить одного юзера по его главному строковому свойству (Email). Поиск не зависит от регистра.
        /// </summary>
        /// <param name="value">Значение для поиска.</param>
        public ApplicationUser GetByUserName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            return this.GetScalarWithCondition(au => au.UserName.ToLower() == value.ToLower());
        }

        /// <summary>
        /// Получить одного юзера, подходящего под условие.
        /// </summary>
        /// <param name="predicate">Усовие выборки.</param>
        public ApplicationUser GetScalarWithCondition(Func<ApplicationUser, bool> predicate)
        {
            if (predicate == null)
                return null;

            return this.GetAll().FirstOrDefault(predicate);
        }

        public List<ApplicationUser> GetWithCondition(Func<ApplicationUser, bool> predicate)
        {
            if (predicate == null)
                return null;

            return this.GetAll().Where(predicate).ToList();
        }

        public List<ApplicationUser> GetWithConditions(params Func<ApplicationUser, bool>[] predicates)
        {
            throw new NotImplementedException();
        }

        public List<ApplicationUser> GetByRole(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                return null;

            var role = this.context.Roles.FirstOrDefault(r => r.Name.ToLower() == roleName.ToLower());

            if (role == null)
                return null;

            return this.GetWithCondition(au => au.Roles.Any(r => r.RoleId == role.Id));
        }

        public IdentityResult Remove(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return IdentityResult.Failed();

            ApplicationUser user = this.manager.FindById(id);

            if (user == null)
                return IdentityResult.Failed();

            return manager.Delete(user);
        }

        /// <summary>
        /// Поиск по части имени (не зависит от регистра).
        /// </summary>
        /// <param name="namePart"></param>
        /// <returns></returns>
        public IEnumerable<ApplicationUser> GetByUserNamePart(string namePart)
        {
            if (string.IsNullOrWhiteSpace(namePart))
                return null;

            IEnumerable<ApplicationUser> result = this.GetWithCondition(u => u.UserName.ToLower().Contains(namePart.ToLower()));
            return result;
        }

        public IdentityResult Ban(string id, DateTime until)
        {
            if (string.IsNullOrWhiteSpace(id))
                return IdentityResult.Failed();

            ApplicationUser user = this.manager.FindById(id);

            if (user == null)
                return IdentityResult.Failed();

            user.LockoutEndDateUtc = until;
            user.IsLocked = true;
            IdentityResult result = this.manager.RemoveFromRole(id, Resource.UserRoleName);

            if (!result.Succeeded)
                return result;

            result = this.manager.AddToRole(user.Id, Resource.BannedRoleName);
            return result;
        }

        public IdentityResult Unban(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return IdentityResult.Failed();

            ApplicationUser user = this.manager.FindById(id);

            if (user == null)
                return IdentityResult.Failed();

            user.LockoutEndDateUtc = null;
            user.IsLocked = false;
            IdentityResult result = this.manager.RemoveFromRole(user.Id, Resource.BannedRoleName);

            if (!result.Succeeded)
                return result;

			result = this.manager.AddToRole(user.Id, Resource.UserRoleName);
            return result;
        }

        public IdentityResult ConfirmEmail(string id, string code)
        {
			if (string.IsNullOrWhiteSpace(id))
				throw new ArgumentNullException("id");

			if (string.IsNullOrWhiteSpace(code))
				throw new ArgumentNullException("code");

			return this.manager.ConfirmEmail(id, code);
        }

		public string GenerateEmailConfirmationToken(string id)
		{
			if (string.IsNullOrWhiteSpace(id))
				throw new ArgumentNullException("id");

			return this.manager.GenerateEmailConfirmationToken(id);
		}

		public string GeneratePasswordResetToken(string id)
        {
			if (string.IsNullOrWhiteSpace(id))
				throw new ArgumentNullException("id");

            return this.manager.GeneratePasswordResetToken(id);
        }

        public void SendEmail(string id, string subject, string message)
        {
			if (string.IsNullOrWhiteSpace(id))
				throw new ArgumentNullException("id");

			if (string.IsNullOrWhiteSpace(subject))
				throw new ArgumentNullException("subject");

			if (string.IsNullOrWhiteSpace(message))
				throw new ArgumentNullException("message");

			this.manager.SendEmail(id, subject, message);
        }

        /// <summary>
        /// Назначить новое публичное имя.
        /// </summary>
        /// <param name="id">Идентификатор пользователя.</param>
        /// <param name="publicName">Новое публичное имя.</param>
        /// <returns></returns>
        public IdentityResult SetUserName(string id, string userName)
        {
            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(userName))
                return IdentityResult.Failed();

            ApplicationUser user = this.manager.FindById(id);
            user.UserName = userName;
            return manager.Update(user);
        }

        /// <summary>
        /// Назначить новый пароль.
        /// </summary>
        /// <param name="id">Идентификатор юзера.</param>
        /// <param name="currentPassword">Актуальный пароль.</param>
        /// <param name="newPassword">Новый пароль.</param>
        /// <returns></returns>
        public IdentityResult SetPassword(string id, string currentPassword, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(newPassword) || currentPassword == null)
                return IdentityResult.Failed();

            return this.manager.ChangePassword(id, currentPassword, newPassword);
        }

        /// <summary>
        /// Назначить новую аватарку.
        /// </summary>
        /// <param name="id">Идентификатор юзера.</param>
        /// <param name="newAvatarUrl">URL новой аватарки.</param>
        /// <returns></returns>
        public IdentityResult SetAvatar(string id, string newAvatarUrl)
        {
            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(newAvatarUrl))
                return IdentityResult.Failed();

            ApplicationUser user = this.manager.FindById(id);
            user.AvatarURL = newAvatarUrl;
            return this.manager.Update(user);
        }

        /// <summary>
        /// Назначить новую электронную почту.
        /// </summary>
        /// <param name="id">Идентификатор юзера.</param>
        /// <param name="newEmail">Новая эл. почта.</param>
        /// <returns></returns>
        public IdentityResult SetEmail(string id, string newEmail)
        {
            if (string.IsNullOrWhiteSpace(id) || newEmail == null)
                return IdentityResult.Failed();

            return this.manager.SetEmail(id, newEmail);
        }

        /// <summary>
        /// Назначить новое контрольное слово.
        /// </summary>
        /// <param name="id">Идентификатор юзера.</param>
        /// <param name="newParole">Новое контрольное слово.</param>
        /// <returns></returns>
        public IdentityResult SetParole(string id, string newParole)
        {
            if (string.IsNullOrWhiteSpace(id) || newParole == null)
                return IdentityResult.Failed();

            ApplicationUser user = this.manager.FindById(id);

            if (user == null)
                return IdentityResult.Failed();

            user.Parole = newParole.CreateMD5();
            return this.manager.Update(user);
        }

        /// <summary>
        /// Проверить пароль на правильность.
        /// </summary>
        /// <param name="user">Юзер для проверки.</param>
        /// <param name="password">Парль для проверки.</param>
        /// <returns></returns>
        public bool CheckPassword(string id, string password)
        {
            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(password))
                return false;

            ApplicationUser user = this.manager.FindById(id);

            if (user == null)
                return false;

            return this.manager.CheckPassword(user, password);
        }

        public void UpdateUserRoles(IEnumerable<IdentityRole> roles,string userId)
        {
            var user = this.Get(userId);
            string[] ignoreRoles = { Resource.AdminRoleName, Resource.BannedRoleName, Resource.UserRoleName };
            foreach(IdentityRole role in roles)
            {
                if (role.Users.Select(u => u.UserId).Contains(user.Id))
                {
                    continue;
                }
                this.manager.AddToRole(user.Id, role.Name);
            }

            IEnumerable<string> diffRoles = this.manager.GetRoles(userId).Except(roles.Select(r=>r.Name).Concat(ignoreRoles));

            if (diffRoles!=null&&diffRoles.Any())
            {
                foreach (string roleName in diffRoles)
                {
                    this.manager.RemoveFromRole(user.Id, roleName);
                }
            }
        }
    }
}