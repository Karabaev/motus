namespace SerialService.Controllers
{
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.Owin;
    using Microsoft.Owin.Security;
    using Models;
    using System;
    using DAL.Entities;
    using Services.Interfaces;
    using AutoMapper;
    using SerialService.Infrastructure.Exceptions;

    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager signInManager;
        private readonly IUserService userService;

        public AccountController(IUserService userService)
        {
            this.userService = userService;
        }

        [Authorize(Roles = "Admin")]
        public ActionResult UserEditor(string searchArg=null)
        {
            if (string.IsNullOrWhiteSpace(searchArg))
                return HttpNotFound();

            return View(this.userService.GetByUserNamePart(searchArg));
        }

        [Authorize(Roles = "Admin")]
        public ActionResult ShowLockOutStore()
        {
            return View("UserEditor", this.userService.GetByRole(Resource.BannedRoleName));
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Search(string searchArg)
        {
            return RedirectToAction("UserEditor",routeValues:new { searchArg = searchArg});
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult DeleteUser(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return HttpNotFound();

            if (!this.userService.Remove(id).Succeeded)
                return HttpNotFound();

            return Redirect(Request.UrlReferrer.ToString()); 
        }

        [Authorize(Roles = "Admin")]
        public ActionResult LockOut(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return HttpNotFound();

            if (!this.userService.Ban(id, DateTime.MaxValue).Succeeded)
                return HttpNotFound();

            return Redirect(Request.UrlReferrer.ToString());
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult LetOff(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return HttpNotFound();

            if (this.userService.Unban(id).Succeeded)
                return HttpNotFound();

            return Redirect(Request.UrlReferrer.ToString());
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = "../../Account/Login"});
            }
            this.signInManager = HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            // Сбои при входе не приводят к блокированию учетной записи
            // Чтобы ошибки при вводе пароля инициировали блокирование учетной записи, замените на shouldLockout: true
            var user = this.userService.GetScalarWithCondition(u => u.Email == model.Email);

            var result = await signInManager.PasswordSignInAsync(user == null ? "":user.UserName, 
                model.Password, 
                model.RememberMe, 
                shouldLockout: false);
            if (string.IsNullOrEmpty(model.ReturnUrl))
            {
                model.ReturnUrl = "../User/Index";
            }
            switch (result)
            {
                case SignInStatus.Success:
                    return Json(new { success = model.ReturnUrl });
                case SignInStatus.LockedOut:
                    return Json(new { error = "Учетная запись заблокирована" });
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                    return Json(new { error = "Неверный адрес email или пароль" });
                default:
                    return Json(new { error = "Неудачная попытка входа" });
            }
        }

        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Требовать предварительный вход пользователя с помощью имени пользователя и пароля или внешнего имени входа
            if (!await signInManager.HasBeenVerifiedAsync())
                return View("Error");

            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Приведенный ниже код защищает от атак методом подбора, направленных на двухфакторные коды. 
            // Если пользователь введет неправильные коды за указанное время, его учетная запись 
            // будет заблокирована на заданный период. 
            // Параметры блокирования учетных записей можно настроить в IdentityConfig
            var result = await signInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent:  model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Неправильный код.");
                    return View(model);
            }
        }

        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = Mapper.Map<RegisterViewModel, ApplicationUser>(model);
                try
                {
                    IdentityResult result = this.userService.Create(user, model.Password, Resource.UserRoleName);
                    if (result.Succeeded)
                    {
                        this.signInManager = HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
                        await this.signInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return Json(new { success = "../User/Index" });
                    }

                    this.AddErrors(result);
                }
                catch(EntryAlreadyExistsException ex)
                {
                    return Json(new { error = ex.Message });
                }
            }

            // Появление этого сообщения означает наличие ошибки; повторное отображение формы
            return View(model);
        }

        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public ActionResult ConfirmEmail(string userId, string code)
        {
            if(string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(code))
                return View("Error");

            IdentityResult result = this.userService.ConfirmEmail(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }
        /// <summary>
        /// Select reset-action
        /// </summary>
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult EmailForgotPassword()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult ParoleForgotPassword()
        {
            return View();
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
            if(ModelState.IsValid)
            {
                ApplicationUser user = this.userService.GetByMainStringProperty(model.Email);
                if(user == null)
                    return RedirectToAction("EmailForgotPassword", "Account");

                string code = this.userService.GeneratePasswordResetToken(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);

                try
                {
                    await Task.Run(() => this.userService.SendEmail(user.Id, "Сброс пароля",
                    "Для сброса пароля, перейдите по ссылке <a href=\"" + callbackUrl + "\">сбросить</a>")); 
                    return RedirectToAction("ForgotPasswordConfirmation", "Account");
                }
                catch
                {
                    return HttpNotFound();
                }
            }
            return View(model);
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
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }


        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
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
            return View();
        }

        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Запрос перенаправления к внешнему поставщику входа
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
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

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Создание и отправка маркера
            if (!await signInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }


        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Выполнение входа пользователя посредством данного внешнего поставщика входа, если у пользователя уже есть имя входа
            var result = await signInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // Если у пользователя нет учетной записи, то ему предлагается создать ее
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
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
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "User");
        }

        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.signInManager != null)
                {
                    this.signInManager.Dispose();
                    this.signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Вспомогательные приложения
        // Используется для защиты от XSRF-атак при добавлении внешних имен входа
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}