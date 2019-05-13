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
    using Infrastructure.Helpers;
    using ViewModels.Account;
    using System.IO;

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
        public ActionResult Login(ReturnUrlViewModel model)
        {
            if(!ModelState.IsValid)
                model.ReturnUrl = Url.Action("Index", "User");

            ViewBag.ReturnUrl = model.ReturnUrl;
            return this.View();
        } 

        /// <summary>
        /// Авторизоваться
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
		[HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
            StringBuilder errors = new StringBuilder();

            if (!this.ModelState.IsValid)
                return this.Json(new { success = "login" });

            ApplicationSignInManager signInManager = this.HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            var user = this.userService.GetByMainStringProperty(model.Email);

            if (user != null)
            {
                if (user.EmailConfirmed == true)
                {
                    var result = signInManager.PasswordSignIn(user.UserName, model.Password, model.RememberMe, shouldLockout: false);

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
        public ActionResult LogOff(ReturnUrlViewModel model)
        {
            if (!ModelState.IsValid)
                model.ReturnUrl = Url.Action("Index", "User");

            this.AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return Redirect(model.ReturnUrl);
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
                user.AvatarURL = GetRandomDefaultAvatar();
                try
                {
                    ApplicationSignInManager signInManager = this.HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
                    IdentityResult result = this.RegisterMethod(user, new string[] { Resource.UserRoleName }, null, model.Password);

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
                    else
                    {
                        return this.Json(new { error = string.Join("<br/>", result.Errors) });
                    }
                }
                catch (EntryAlreadyExistsException ex)
                {
                    return this.Json(new { error = ex.Message });
                }
            }
            else
            {
                return this.Json(new { error = string.Join("<br/>", ModelState.Values.SelectMany(s => s.Errors.Select(e => e.ErrorMessage))) });
            }
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
        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
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
        /// Сбросить пароль по пароле.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public ActionResult ParoleForgotPassword(ParoleForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = this.userService.GetByMainStringProperty(model.Email);

                if (user == null)
                    return Json(new { error = string.Format("Пользователь с email {0} не найден", model.Email) });

                if (model.Parole.CreateMD5() != user.Parole)
                    return Json(new { error = string.Format("Введено некорректное секретное слово") });

                string code = this.userService.GeneratePasswordResetToken(user.Id);

                return Json(new
                {
                    success = Url.Action("ResetPassword", new ConfirmEmailViewModel { UserID = user.Id, Code = code })

                });
            }
            else
            {
                return Json( new { error = string.Join("<br/>", ModelState.Values.SelectMany(s => s.Errors.Select(e => e.ErrorMessage))) });
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

            var user = this.userService.GetByMainStringProperty(model.Email);

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
                    ExternalProviderName = model.Provider,
                    ReturnUrl = model.ReturnUrl
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
            if (string.IsNullOrWhiteSpace(model.ReturnUrl))
                model.ReturnUrl = Url.Action("Index", "User");

            var loginInfo = this.AuthenticationManager.GetExternalLoginInfo();
            ApplicationSignInManager signInManager = this.HttpContext.GetOwinContext().Get<ApplicationSignInManager>();

            if (loginInfo == null)
            {
                return RedirectToAction("ExternalLoginFailure", new ExternalLoginFailureViewModel
                {
                    Errors = "Не удалось получить данные авторизации от внешнего поставщика",
                    ProviderDisplayName = model.ExternalProviderName
                });
            }

            string userId = this.User.Identity.GetUserId();

            if(!string.IsNullOrWhiteSpace(userId))
            {
                var result = this.userService.AddLogin(userId, loginInfo.Login);

                if (result.Succeeded)
                    return Redirect(model.ReturnUrl);
                else
                    return RedirectToAction("ExternalLoginFailure", new ExternalLoginFailureViewModel { Errors = string.Join("<br/>", result.Errors) });
            }
            var user = this.userService.GetByExternalLogin(loginInfo.Login); // ищем юзера по логин

            if(user != null)
            {
                signInManager.SignIn(user, false, false);
                return this.UpdateSignedInUserAndRedirect(user, model.ReturnUrl);
            }

            user = this.userService.GetByMainStringProperty(loginInfo.Email); // ищем юзера по email
            
            if (user != null)
            {
                var result = this.userService.AddLogin(user.Id, loginInfo.Login);

                if(result.Succeeded)
                {
                    signInManager.SignIn(user, false, false);
                    return this.UpdateSignedInUserAndRedirect(user, model.ReturnUrl);
                }
                else
                {
                    return RedirectToAction("ExternalLoginFailure", new ExternalLoginFailureViewModel { Errors = string.Join("<br/>", result.Errors) });
                }
            }
            else
            {
                // Выполнение входа пользователя посредством данного внешнего поставщика входа, если у пользователя уже есть имя входа
                var result = signInManager.ExternalSignIn(loginInfo, isPersistent: false);

                switch (result)
                {
                    case SignInStatus.Success:
                        user = this.userService.GetByExternalLogin(loginInfo.Login);

                        if (user != null)
                            return this.UpdateSignedInUserAndRedirect(user, model.ReturnUrl);
                        else
                            return RedirectToAction("ExternalLoginFailure", new ExternalLoginFailureViewModel { Errors = "Пользователь не найден" });
                    case SignInStatus.LockedOut:
                        return RedirectToAction("ExternalLoginFailure", new ExternalLoginFailureViewModel { Errors = "Учетная запись заблокирована" });
                    case SignInStatus.Failure:
                    default:
                        IdentityResult createResult = null;

                        try
                        {
                            if (string.IsNullOrEmpty(loginInfo.Email))
                            {
                                return RedirectToAction("ExternalLoginFailure", new ExternalLoginFailureViewModel
                                {
                                    Errors = "Не удалось получить адрес эл. почты",
                                    ProviderDisplayName = model.ExternalProviderName
                                });
                            }

                            if (string.IsNullOrEmpty(loginInfo.DefaultUserName) && string.IsNullOrEmpty(loginInfo.ExternalIdentity.Name))
                            {
                                return RedirectToAction("ExternalLoginFailure", new ExternalLoginFailureViewModel
                                {
                                    Errors = "Не удалось получить адрес эл. почты",
                                    ProviderDisplayName = model.ExternalProviderName
                                });
                            }

                            user = new ApplicationUser
                            {
                                UserName = string.IsNullOrWhiteSpace(loginInfo.ExternalIdentity.Name) ?
                                                                        loginInfo.DefaultUserName :
                                                                        loginInfo.ExternalIdentity.Name,
                                Email = loginInfo.Email,
                                EmailConfirmed = true
                            };

                            createResult = this.RegisterMethod(user, new string[] { Resource.UserRoleName }, loginInfo.Login);

                            if (createResult.Succeeded)
                            {
                                signInManager.SignIn(user, isPersistent: false, rememberBrowser: false);
                                return this.UpdateSignedInUserAndRedirect(user, model.ReturnUrl);
                            }
                            else
                            {
                                return RedirectToAction("ExternalLoginFailure", new ExternalLoginFailureViewModel { Errors = string.Join("<br/>", createResult.Errors) });
                            }
                        }
                        catch (EntryAlreadyExistsException ex)
                        {
                            return RedirectToAction("ExternalLoginFailure", new ExternalLoginFailureViewModel { Errors = ex.Message });
                        }
                        catch (Exception ex)
                        {
                            return RedirectToAction("ExternalLoginFailure", new ExternalLoginFailureViewModel { Errors = "Не удалось зарегистрировать пользователя" });
                        }
                }
            }
        }

        /// <summary>
        /// Метод регистрации юзера.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="roles"></param>
        /// <param name="login"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private IdentityResult RegisterMethod(ApplicationUser user, string[] roles, UserLoginInfo login, string password = "")
        {
            if (user == null)
                throw new ArgumentNullException("user");

            IdentityResult result = null;

            if(string.IsNullOrWhiteSpace(password))
                result = this.userService.CreateWithoutPassword(user, roles);
            else
                result = this.userService.Create(user, password, roles);

            if(result.Succeeded && login != null)
                result = this.userService.AddLogin(user.Id, login);

            return result;
        }

        private RedirectToRouteResult UpdateSignedInUserAndRedirect(  ApplicationUser user, 
                                                                string controller,
                                                                string action, 
                                                                object routeValues)
        {
            user.LastAuthorizationDateTime = DateTime.Now;
            Task.Run(() => this.userService.Update(user));
            return RedirectToAction(action, controller, routeValues);
        }

        private RedirectResult UpdateSignedInUserAndRedirect(ApplicationUser user,
                                                        string url)
        {
            user.LastAuthorizationDateTime = DateTime.Now;
            Task.Run(() => this.userService.Update(user));
            return Redirect(url);
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

        #region Вспомогательные приложения

        /// <summary>
        /// Рандомный стандартный аватар
        /// </summary>
        /// <returns></returns>
        private string GetRandomDefaultAvatar()
        {
            string result;
            try
            {
                var virtualPath = @"UserFiles\default_avatars\";
                var absolutePath = Server.MapPath(virtualPath);
                result = $"{virtualPath}{new Random().Next(minValue: 1, maxValue: Directory.GetFiles(absolutePath, "*", SearchOption.AllDirectories).Length)}.jpeg";
            }
            catch
            {
                result = string.Empty;
            }
            return result;
        }

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