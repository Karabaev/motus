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
			if (entity == null)
				throw new ArgumentNullException("entity");

			if (string.IsNullOrWhiteSpace(pass))
				throw new ArgumentNullException("pass");

			if (this.GetScalarWithCondition(au => au.Id == entity.Id) != null)
                throw new EntryAlreadyExistsException("Пользователь с таким идентификатором уже существует.");

            if (this.GetByMainStringProperty(entity.Email) != null)
                throw new EntryAlreadyExistsException("Пользователь с такой почтой уже существует.");

			entity.RegisterDateTime = DateTime.Now;
			entity.ChangeDateTime = entity.RegisterDateTime;
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

		public void SendToCustomEmail(string email, string subject, string message)
		{
			if (string.IsNullOrWhiteSpace(email))
				throw new ArgumentNullException("email");

			if (string.IsNullOrWhiteSpace(subject))
				throw new ArgumentNullException("subject");

			if (string.IsNullOrWhiteSpace(message))
				throw new ArgumentNullException("message");

			this.manager.EmailService.Send(new IdentityMessage { Destination = email, Body = message, Subject = subject });
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
				throw new ArgumentNullException("id и userName");

            ApplicationUser user = this.manager.FindById(id);
			string oldName = user.UserName;
			DateTime oldChangeDateTime = user.ChangeDateTime;
            user.UserName = userName;
			var result = manager.Update(user);

			if (!result.Succeeded)
				user.UserName = oldName;

			return result;
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

			var result = this.manager.ChangePassword(id, currentPassword, newPassword);
			var user = this.Get(id);

			if (result.Succeeded)
			{
				user.ChangeDateTime = DateTime.Now;
				this.manager.Update(user);
			}
			return result;
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
			user.ChangeDateTime = DateTime.Now;
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

			var result = this.manager.SetEmail(id, newEmail);
			var user = this.Get(id);

			if (result.Succeeded)
			{
				user.ChangeDateTime = DateTime.Now;
				this.manager.Update(user);
			}
			return result;

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

			user.ChangeDateTime = DateTime.Now;
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

		public IdentityResult ChangeEmail(string userId, string newEmail, string key)
		{
			if (string.IsNullOrWhiteSpace(userId))
				throw new ArgumentNullException("userId");

			if (string.IsNullOrWhiteSpace(newEmail))
				throw new ArgumentNullException("newEmail");

			if (string.IsNullOrWhiteSpace(key))
				throw new ArgumentNullException("key");


			if (this.CheckKey(userId, key))
			{
				var result = this.SetEmail(userId, newEmail);

				if (!result.Succeeded)
					return result;

				string confirmationKey = this.manager.GenerateEmailConfirmationToken(userId);
				result = this.manager.ConfirmEmail(userId, confirmationKey);
				return result;
			}
			else
				return new IdentityResult("Не подходит ключ");
		
	}

	public IdentityResult SetKey(string userId, string key)
		{
			if (string.IsNullOrWhiteSpace(userId))
				throw new ArgumentNullException("userId");

			if (string.IsNullOrWhiteSpace(key))
				throw new ArgumentNullException("key");

			var user = this.Get(userId);

			if (user == null)
				throw new EntryNotFoundException("Пользователь не найден");

			user.LastConfirmationKey = key;
			return this.manager.Update(user);
		}

		public bool CheckKey(string userId, string key)
		{
			if (string.IsNullOrWhiteSpace(userId))
				throw new ArgumentNullException("userId");

			if (string.IsNullOrWhiteSpace(key))
				throw new ArgumentNullException("key");

			var user = this.Get(userId);

			if (user == null)
				throw new EntryNotFoundException("Пользователь не найден");

			if (user.LastConfirmationKey == key)
			{
				user.LastConfirmationKey = string.Empty;
				this.manager.Update(user);
				return true;
			}
			else
			{
				user.LastConfirmationKey = string.Empty;
				this.manager.Update(user);
				return false;
			}
		}

        public IdentityResult ResetPassword(string userId, string code, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentNullException("userId");

            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentNullException("code");

            if (string.IsNullOrWhiteSpace(newPassword))
                throw new ArgumentNullException("newPassword");

            return this.manager.ResetPassword(userId, code, newPassword);
        }


        public IdentityResult Update(ApplicationUser user)
		{
            if (user == null)
                throw new ArgumentNullException("user");

            return this.manager.Update(user);
		}

        public IdentityResult AddLogin(string userId, UserLoginInfo info)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentNullException("userId");

            if (info == null)
                throw new ArgumentNullException("info");

            var user = this.Get(userId);

            if (user == null)
                throw new EntryNotFoundException("Пользователь не найден");

            var result = this.manager.AddLogin(userId, info);

            if(result.Succeeded)
                user.ChangeDateTime = DateTime.Now;

            return result;
        }
    }
}