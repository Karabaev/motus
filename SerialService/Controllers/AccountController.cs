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

	[Authorize, ExceptionHandler]
	public class AccountController : Controller
	{
		private readonly IUserService userService;

		public AccountController(IUserService userService)
		{
			this.userService = userService;
		}

		[AllowAnonymous]
		public ActionResult Login(string returnUrl = "")
		{
			this.ViewBag.ReturnUrl = returnUrl;
			return this.View();
		}

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

		//   [HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult LogOff()
		{
			this.AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
			return this.RedirectToAction("Index", "User");
		}

		// GET: /Account/Register
		[AllowAnonymous]
		public ActionResult Register()
		{
			return this.View();
		}

		// POST: /Account/Register
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

		[AllowAnonymous]
		public ActionResult DisplayEmailToConfirmation(DisplayEmailToConfirmationViewModel model)
		{
			if (model == null || string.IsNullOrWhiteSpace(model.Email))
				HttpNotFound();

			return View(model);
		}

		// GET: /Account/ConfirmEmail
		[AllowAnonymous]
		public ActionResult ConfirmEmail(ConfirmEmailViewModel model)
		{
			if (model == null || string.IsNullOrWhiteSpace(model.UserID) || string.IsNullOrWhiteSpace(model.Code))
				return this.HttpNotFound();

			IdentityResult result = this.userService.ConfirmEmail(model.UserID, model.Code);
			return this.View(result.Succeeded ? "ConfirmEmail" : "Error");
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

		/// <summary>
		/// Select reset-action
		/// </summary>
		[AllowAnonymous]
		public ActionResult ForgotPassword()
		{
			return this.View();
		}

		[AllowAnonymous]
		public ActionResult EmailForgotPassword()
		{
			return this.View();
		}

		[AllowAnonymous]
		public ActionResult ParoleForgotPassword()
		{
			return this.View();
		}

		/// <summary>
		/// Method for email-confirmation
		/// </summary>
		/// <param name="model">Object with email property</param>
		/// <returns></returns>
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> EmailForgotPassword(EmailForgotPasswordViewModel model)
		{
			if (this.ModelState.IsValid)
			{
				ApplicationUser user = this.userService.GetByMainStringProperty(model.Email);
				if (user == null)
					return this.RedirectToAction("EmailForgotPassword", "Account");

				string code = this.userService.GeneratePasswordResetToken(user.Id);
				var callbackUrl = this.Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: this.Request.Url.Scheme);

				try
				{
					await Task.Run(() => this.userService.SendEmail(user.Id, "Сброс пароля",
					"Для сброса пароля, перейдите по ссылке <a href=\"" + callbackUrl + "\">сбросить</a>"));
					return this.RedirectToAction("ForgotPasswordConfirmation", "Account");
				}
				catch
				{
					return this.HttpNotFound();
				}
			}
			return this.View(model);
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
		public ActionResult ForgotPasswordConfirmation()
		{
			return this.View();
		}


		[AllowAnonymous]
		public ActionResult ResetPassword(string code)
		{
			return code == null ? this.View("Error") : this.View();
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
		{
			//if (!ModelState.IsValid)
			//{
			//    return View(model);
			//}
			//var user = await UserManager.FindByNameAsync(model.Email);
			//if (user == null)
			//{
			//    // Не показывать, что пользователь не существует
			//    return RedirectToAction("ResetPasswordConfirmation", "Account");
			//}
			//var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
			//if (result.Succeeded)
			//{
			//    return RedirectToAction("ResetPasswordConfirmation", "Account");
			//}
			//AddErrors(result);
			return this.View();
		}

		[AllowAnonymous]
		public ActionResult ResetPasswordConfirmation()
		{
			return this.View();
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public ActionResult ExternalLogin(string provider, string returnUrl)
		{
			// Запрос перенаправления к внешнему поставщику входа
			return new ChallengeResult(provider, this.Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
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


		[AllowAnonymous]
		public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
		{
			var loginInfo = await this.AuthenticationManager.GetExternalLoginInfoAsync();
			if (loginInfo == null)
			{
				return this.RedirectToAction("Login");
			}
			ApplicationSignInManager signInManager = this.HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
			// Выполнение входа пользователя посредством данного внешнего поставщика входа, если у пользователя уже есть имя входа
			var result = await signInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
			switch (result)
			{
				case SignInStatus.Success:
					return this.RedirectToLocal(returnUrl);
				case SignInStatus.LockedOut:
					return this.View("Lockout");
				case SignInStatus.RequiresVerification:
					return this.RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
				case SignInStatus.Failure:
				default:
					// Если у пользователя нет учетной записи, то ему предлагается создать ее
					this.ViewBag.ReturnUrl = returnUrl;
					this.ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
					return this.View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
			}
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
		{
			//if (User.Identity.IsAuthenticated)
			//{
			//    return RedirectToAction("Index", "Manage");
			//}

			//if (ModelState.IsValid)
			//{
			//    // Получение сведений о пользователе от внешнего поставщика входа
			//    var info = await AuthenticationManager.GetExternalLoginInfoAsync();
			//    if (info == null)
			//    {
			//        return View("ExternalLoginFailure");
			//    }
			//    var user = new ApplicationUser { UserName = model.UserName, Email = model.Email };
			//    var result = await UserManager.CreateAsync(user);
			//    if (result.Succeeded)
			//    {
			//        result = await UserManager.AddLoginAsync(user.Id, info.Login);
			//        if (result.Succeeded)
			//        {
			//            await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
			//            return RedirectToLocal(returnUrl);
			//        }
			//    }
			//    AddErrors(result);
			//}

			//ViewBag.ReturnUrl = returnUrl;
			return this.View(model);
		}

		[AllowAnonymous]
		public ActionResult ExternalLoginFailure()
		{
			return this.View();
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