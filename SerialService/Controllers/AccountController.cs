namespace SerialService.Controllers
{
	using AutoMapper;
	using DAL.Entities;
	using Microsoft.AspNet.Identity;
	using Microsoft.AspNet.Identity.Owin;
	using Microsoft.Owin.Security;
	using Models;
	using SerialService.Infrastructure.Exceptions;
	using Services.Interfaces;
	using System.Text;
	using System.Threading.Tasks;
	using System.Web;
	using System.Web.Mvc;
	using ViewModels;
	using Infrastructure;
	using System;
    using System.Linq;

    [Authorize, ExceptionHandler]
    public class AccountController : Controller
	{
		private readonly IUserService userService;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="userService"></param>
		public AccountController(IUserService userService)
		{
			this.userService = userService;
		}

        /// <summary>
        /// Открыть форму авторизации
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
		[AllowAnonymous]
		public ActionResult Login()
		{
			return this.View();
		}

        /// <summary>
        /// Авторизоваться
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
		[HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
		public async Task<ActionResult> Login(LoginViewModel model)
		{
			StringBuilder errors = new StringBuilder();

			if (!this.ModelState.IsValid)
				return this.Json(new { success = "login" });

			ApplicationSignInManager signInManager = this.HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
			var user = this.userService.GetScalarWithCondition(u => u.Email == model.Email);

			if (user != null)
			{
				if (user.EmailConfirmed == true)
				{
					var result = await signInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, shouldLockout: false);
					model.ReturnUrl = "films";

					switch (result)
					{
						case SignInStatus.Success:
							user.LastAuthorizationDateTime = DateTime.Now;
							Task.Run(() => this.userService.Update(user));
							return this.Json(new { success = model.ReturnUrl });
						case SignInStatus.LockedOut:
							errors.Append("Учетная запись заблокирована<br/>");
							break;
						case SignInStatus.Failure:
							errors.Append("Неверный адрес email или пароль<br/>");
							break;
						default:
							errors.Append("Неудачная попытка входа<br/>");
							break;
					}
				}
				else
				{
					errors.Append("Не подтвержден email<br/>");
				}
			}
			else
			{
				errors.Append("Неверный адрес email или пароль<br/>");
			}

			return this.Json(new { error = errors.ToString() });
		}

        /// <summary>
        /// Выйти из системы
        /// </summary>
        /// <returns></returns>
		[HttpPost, ValidateAntiForgeryToken]
		public ActionResult LogOff()
		{
			this.AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
			return this.RedirectToAction("Index", "User");
		}

		/// <summary>
        /// Открыть форму регистрации.
        /// </summary>
        /// <returns></returns>
		[AllowAnonymous]
		public ActionResult Register()
		{
			return this.View();
		}

		/// <summary>
        /// Зарегистрироваться
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
		[HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
		public ActionResult Register(RegisterViewModel model)
		{
			if (this.ModelState.IsValid)
			{
				ApplicationUser user = Mapper.Map<RegisterViewModel, ApplicationUser>(model);

				try
				{
					ApplicationSignInManager signInManager = this.HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
					IdentityResult result = this.userService.Create(user, model.Password, Resource.UserRoleName);

					if (result.Succeeded)
					{
						var code = this.userService.GenerateEmailConfirmationToken(user.Id); // токен для подтверждения регистрации
						var callbackUrl = this.Url.Action("ConfirmEmail",
									"Account",
									new ConfirmEmailViewModel { UserID = user.Id, Code = code },
									protocol: this.Request.Url.Scheme);
						Task.Run(() => this.userService.SendEmail(user.Id,
												   "Подтверждение адреса электронной почты",
												   string.Format("Для завершения регистрации перейдите по ссылке: <a href=\"{0}\">завершить регистрацию</a>",
																	callbackUrl)));
						return this.Json(new
						{
							success = this.Url.Action("DisplayEmailToConfirmation", "Account", new DisplayEmailToConfirmationViewModel { Email = model.Email })
						});
					}

					this.AddErrors(result);
				}
				catch (EntryAlreadyExistsException ex)
				{
					return this.Json(new { error = ex.Message });
				}
			}

			// Появление этого сообщения означает наличие ошибки; повторное отображение формы
			return this.View(model);
		}

        /// <summary>
        /// Открыть страницу с сообщением о необходимости подтверждения почты
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
		[AllowAnonymous]
		public ActionResult DisplayEmailToConfirmation(DisplayEmailToConfirmationViewModel model)
		{
			if (model == null || string.IsNullOrWhiteSpace(model.Email))
				HttpNotFound();

			return View(model);
		}

        /// <summary>
        /// Подтвердить почту
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
		public ActionResult ConfirmEmail(ConfirmEmailViewModel model)
		{
			if (model == null || string.IsNullOrWhiteSpace(model.UserID) || string.IsNullOrWhiteSpace(model.Code))
				return this.HttpNotFound();

			IdentityResult result = this.userService.ConfirmEmail(model.UserID, model.Code);
			return this.View(result.Succeeded ? "ConfirmEmail" : "Error");
		}

        /// <summary>
        /// Открыть страницу с выбором вариантов сброса пароля
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return this.View();
        }

        /// <summary>
        /// Сбросить пароль по email
        /// </summary>
        /// <param name="model">Object with email property</param>
        /// <returns></returns>
        [HttpPost, AllowAnonymous]
        public ActionResult EmailForgotPassword(EmailForgotPasswordViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                ApplicationUser user = this.userService.GetByMainStringProperty(model.Email);

                if (user == null)
                    return Json(new { error = string.Format("Пользователь с email {0} не найден", model.Email) });

                string code = this.userService.GeneratePasswordResetToken(user.Id);
                var callbackUrl = this.Url.Action("ResetPassword", "Account", new { UserID = user.Id, Code = code }, protocol: this.Request.Url.Scheme);

                Task.Run(() => this.userService.SendEmail(user.Id, "Сброс пароля",
                "Для сброса пароля, перейдите по ссылке <a href=\"" + callbackUrl + "\">сбросить</a>"));
                return Json(new
                {
                    success = Url.Action("ForgotPasswordConfirmation", new DisplayEmailToConfirmationViewModel
                    {
                        Email = model.Email
                    })
                });
            }
            else
            {
                return Json(new { error = "Поле Email некорректно" });
            }
        }

        /// <summary>
        /// Показать сообщение об отправке письма на почту с ссылкой на сброс.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation(DisplayEmailToConfirmationViewModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Email))
                HttpNotFound();

            return this.View(model);
        }

        /// <summary>
        /// Открыть страницу с формой сброса пароля
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult ResetPassword(ConfirmEmailViewModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Code) || string.IsNullOrWhiteSpace(model.UserID))
                return this.View("Error");

            var user = this.userService.Get(model.UserID);

            if (user == null)
                return HttpNotFound();

            ResetPasswordViewModel m = new ResetPasswordViewModel
            {
                Email = user.Email,
                Code = model.Code
            };

            return this.View(m);
        }

        /// <summary>
        /// Сбросить пароль
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user =  this.userService.GetByMainStringProperty(model.Email);

            if (user == null)
                return Json(new { error = "Пользователь с указанным адресом эл. почты не найден" });

            var result = this.userService.ResetPassword(user.Id, model.Code, model.Password);

            if (!result.Succeeded)
                return Json(new { error = string.Join(", ", result.Errors) });

            return Json(new { success = Url.Action("ResetPasswordConfirmation") });
        }

        /// <summary>
        /// Показать успешный результат сброса пароля
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return this.View();
        }

        /// <summary>
        /// Выполнить авторизацию через внешний сервис.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(ExternalLoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                return new ChallengeResult(model.Provider, this.Url.Action("ExternalLoginCallback", "Account", new ExternalLoginCallbackViewModel
                {
                    ExternalProviderName = model.Provider
                }));
            }
            else
            {
                return RedirectToAction("ExternalLoginFailure", new ExternalLoginFailureViewModel
                {
                    Errors = string.Join(", ", ModelState.Values.SelectMany(s => s.Errors.Select(e => e.ErrorMessage))),
                    ProviderDisplayName = model.Provider
                });
            }
        }

        /// <summary>
        /// Проверка авторизации, если успех, выполняется вход, если ошибка - переход на страницу ошибки, 
        /// если такой пользователь не зарегистрирован - переход на страницу регистрации.
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult ExternalLoginCallback(ExternalLoginCallbackViewModel model)
        {
            StringBuilder errors = new StringBuilder();
            var loginInfo = this.AuthenticationManager.GetExternalLoginInfo();

            if (loginInfo == null)
            {
                return RedirectToAction("ExternalLoginFailure", new ExternalLoginFailureViewModel
                {
                    Errors = "Не удалось получить данные авторизации от внешнего поставщика",
                    ProviderDisplayName = model.ExternalProviderName
                });
            }

            var user = this.userService.GetByMainStringProperty(loginInfo.Email);

            ApplicationSignInManager signInManager = this.HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            // Выполнение входа пользователя посредством данного внешнего поставщика входа, если у пользователя уже есть имя входа
            var result = signInManager.ExternalSignIn(loginInfo, isPersistent: false);

            switch (result)
            {
                case SignInStatus.Success:
                    user.LastAuthorizationDateTime = DateTime.Now;
                    Task.Run(() => this.userService.Update(user));
                    return RedirectToAction("Index", "User");
                case SignInStatus.LockedOut:
                    errors.Append("Учетная запись заблокирована<br/>");
                    break;
                case SignInStatus.Failure:
                default:
                    // Если у пользователя нет учетной записи, то ему предлагается создать ее
                    this.ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return this.View("ExternalRegister", new ExternalRegisterViewModel
                    {
                        Email = loginInfo.Email,
                        UserName = string.IsNullOrWhiteSpace(loginInfo.ExternalIdentity.Name) ? 
                                                                loginInfo.DefaultUserName : 
                                                                loginInfo.ExternalIdentity.Name,
                        LoginProvider = loginInfo.Login.LoginProvider,
                        ProviderKey = loginInfo.Login.ProviderKey
                    });
            }

            return RedirectToAction("ExternalLoginFailure", new ExternalLoginFailureViewModel { Errors = errors.ToString() });
        }

        /// <summary>
        /// Регистрация после получения параметров от внешнего сервиса.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public ActionResult ExternalRegister(ExternalRegisterViewModel model)
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("PersonalAccount", "User");

            StringBuilder errors = new StringBuilder();

            if (ModelState.IsValid)
            {
                // Получение сведений о пользователе от внешнего поставщика входа
                var login = new UserLoginInfo(model.LoginProvider, model.ProviderKey);
                var user = new ApplicationUser { UserName = model.UserName, Email = model.Email, Parole = model.Parole, EmailConfirmed = true };
                IdentityResult result = null;

                try
                {
                    result = this.userService.Create(user, model.Password, Resource.UserRoleName);
                }
                catch(EntryAlreadyExistsException ex)
                {
                    return Json(new { error = ex.Message });
                }
                catch
                {
                    return Json(new { error = "Не удалось зарегистрировать пользователя" });
                }
                
                if (result.Succeeded)
                {

                    result = this.userService.AddLogin(user.Id, login);

                    if (result.Succeeded)
                    {
                        ApplicationSignInManager signInManager = this.HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
                        signInManager.SignIn(user, isPersistent: false, rememberBrowser: false);
                        return Json(new { success = Url.Action("Index", "User") }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { error = string.Join("<br/>", result.Errors) });
                    }
                }
                else
                {
                    return Json(new { error = string.Join("<br/>", result.Errors) });
                }
            }
            else
            {
                return Json(new { error = ModelState.Values.SelectMany(s => s.Errors.Select(e => e.ErrorMessage)) });
            }
        }

        /// <summary>
        /// Вывести ошибку при неудачной авторизации через внешний сервис.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure(ExternalLoginFailureViewModel model)
        {
            return View(model);
        }



        [AllowAnonymous]
		public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
		{
			ApplicationSignInManager signInManager = this.HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
			// Требовать предварительный вход пользователя с помощью имени пользователя и пароля или внешнего имени входа
			if (!await signInManager.HasBeenVerifiedAsync())
				return this.View("Error");

			return this.View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
		{
			if (!this.ModelState.IsValid)
			{
				return this.View(model);
			}

			ApplicationSignInManager signInManager = this.HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
			// Приведенный ниже код защищает от атак методом подбора, направленных на двухфакторные коды. 
			// Если пользователь введет неправильные коды за указанное время, его учетная запись 
			// будет заблокирована на заданный период. 
			// Параметры блокирования учетных записей можно настроить в IdentityConfig
			var result = await signInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
			switch (result)
			{
				case SignInStatus.Success:
					return this.RedirectToLocal(model.ReturnUrl);
				case SignInStatus.LockedOut:
					return this.View("Lockout");
				case SignInStatus.Failure:
				default:
					this.ModelState.AddModelError("", "Неправильный код.");
					return this.View(model);
			}
		}

		[AllowAnonymous]
		public ActionResult ParoleForgotPassword()
		{
			return this.View();
		}

		/// <summary>
		/// Method for secret word confirmation
		/// </summary>
		/// <param name="model">Object with secret word property</param>
		/// <returns></returns>
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ParoleForgotPassword(ParoleForgotPasswordViewModel model)
		{
			//if (ModelState.IsValid)
			//{
			//var user = await UserManager.FindByEmailAsync(model.Email);
			//if (user == null ||model.Parole.CreateMD5()!=user.Parole)
			//{
			//    return RedirectToAction("ParoleForgotPassword", "Account");
			//}
			//string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
			//return RedirectToAction("ResetPassword", "Account", new { code = code });
			//}
			return this.View(model);
		}


		[AllowAnonymous]
		public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
		{
			//var userId = await SignInManager.GetVerifiedUserIdAsync();
			//if (userId == null)
			//{
			//    return View("Error");
			//}
			//var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
			//var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
			//return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });

			return this.View();
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> SendCode(SendCodeViewModel model)
		{
			if (!this.ModelState.IsValid)
			{
				return this.View();
			}
			ApplicationSignInManager signInManager = this.HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
			// Создание и отправка маркера
			if (!await signInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
			{
				return this.View("Error");
			}
			return this.RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
		}


		#region Вспомогательные приложения
		// Используется для защиты от XSRF-атак при добавлении внешних имен входа
		private const string XsrfKey = "XsrfId";

		private IAuthenticationManager AuthenticationManager
		{
			get
			{
				return this.HttpContext.GetOwinContext().Authentication;
			}
		}

		private void AddErrors(IdentityResult result)
		{
			foreach (var error in result.Errors)
			{
				this.ModelState.AddModelError("", error);
			}
		}

		private ActionResult RedirectToLocal(string returnUrl)
		{
			if (this.Url.IsLocalUrl(returnUrl))
				return this.Redirect(returnUrl);

			return this.RedirectToAction("Index", "Home");
		}

		internal class ChallengeResult : HttpUnauthorizedResult
		{
			public ChallengeResult(string provider, string redirectUri)
				: this(provider, redirectUri, null)
			{
			}

			public ChallengeResult(string provider, string redirectUri, string userId)
			{
				this.LoginProvider = provider;
				this.RedirectUri = redirectUri;
				this.UserId = userId;
			}

			public string LoginProvider { get; set; }
			public string RedirectUri { get; set; }
			public string UserId { get; set; }

			public override void ExecuteResult(ControllerContext context)
			{
				var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
				if (this.UserId != null)
				{
					properties.Dictionary[XsrfKey] = this.UserId;
				}
				context.HttpContext.GetOwinContext().Authentication.Challenge(properties, this.LoginProvider);
			}
		}
		#endregion
	}
}