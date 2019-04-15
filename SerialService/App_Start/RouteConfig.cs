namespace SerialService
{
    using System.Web.Mvc;
    using System.Web.Routing;

    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {

			#region Роуты User

			routes.MapRoute(name: "UserIndex",
				url: "",
				defaults: new { controller = "User", action = "Index" });

			routes.MapRoute(name: "UserIndex1",
				url: "films",
				defaults: new { controller = "User", action = "Index" });

			routes.MapRoute(name: "UserDetailPage",
				url: "films/{id}",
				defaults: new { controller = "User", action = "VideoMaterialDetailPage", id = UrlParameter.Optional });

			routes.MapRoute(name: "UserPersonalAccount",
				url: "personal_account",
				defaults: new { controller = "User", action = "PersonalAccount" });

			routes.MapRoute(name: "UserPersonalAccountPost",
				url: "personal_account/save_changes/{account}",
				defaults: new { controller = "User", action = "PersonalAccountSaveChanges", account = UrlParameter.Optional });

			routes.MapRoute(name: "UserAboutProject",
				url: "about",
				defaults: new { controller = "User", action = "AboutProject" });

			routes.MapRoute(name: "UserForHolders",
				url: "for_holders",
				defaults: new { controller = "User", action = "ForHolders" });

			routes.MapRoute(name: "UserGetMarks",
				url: "films/get_marks/{id}",
				defaults: new { controller = "User", action = "GetMarks", id = UrlParameter.Optional });

			routes.MapRoute(name: "UserIsSubscribed",
				url: "films/is_subscribed/{id}",
				defaults: new { controller = "User", action = "IsSubscribed", id = UrlParameter.Optional });

			routes.MapRoute(name: "UserAddMark",
				url: "User/AddMark/{mark}",
				defaults: new { controller = "User", action = "AddMark", mark = UrlParameter.Optional });

			routes.MapRoute(name: "UserSubscribe",
				url: "User/Subscribe/{id}",
				defaults: new { controller = "User", action = "Subscribe", id = UrlParameter.Optional });

			routes.MapRoute(name: "UserUnsubscribe",
				url: "User/Unsubscribe/{id}",
				defaults: new { controller = "User", action = "Unsubscribe", id = UrlParameter.Optional });

			routes.MapRoute(name: "UserFilter",
				url: "User/Filter",
				defaults: new { controller = "User", action = "Filter" });

			routes.MapRoute(name: "UserSearch",
				url: "User/Search/{searchStr}",
				defaults: new { controller = "User", action = "Search", searchStr = UrlParameter.Optional });

			routes.MapRoute(name: "UserGetSuggest",
				url: "User/GetSuggest/{part}",
				defaults: new { controller = "User", action = "GetSuggest", part = UrlParameter.Optional });

			routes.MapRoute(name: "UserUploadAvatar",
				url: "personal_account/upload_avatar",
				defaults: new { controller = "User", action = "UploadAvatar" });

			routes.MapRoute(name: "UserConfirmNewEmail",
				url: "personal_account/confirm_new_email",
				defaults: new { controller = "User", action = "ConfirmNewEmail" });

			#endregion

			#region Роуты Account

			routes.MapRoute(name: "AccountRegister",
				url: "registration",
				defaults: new { controller = "Account", action = "Register" });

			routes.MapRoute(name: "AccountRegisterPost",
				url: "registration/{model}",
				defaults: new { controller = "Account", action = "Register", model = UrlParameter.Optional });

			routes.MapRoute(name: "AccountLogin",
				url: "login",
				defaults: new { controller = "Account", action = "Login" });

			routes.MapRoute(name: "AccountLoginPost",
				url: "login/{model}",
				defaults: new { controller = "Account", action = "Login", model = UrlParameter.Optional });

			routes.MapRoute(name: "AccountLogoff",
				url: "logoff",
				defaults: new { controller = "Account", action = "Logoff" });

			routes.MapRoute(name: "AccountDisplayEmailToConfirmation",
				url: "confirmation/{model}",
				defaults: new { controller = "Account", action = "DisplayEmailToConfirmation", model = UrlParameter.Optional });

			routes.MapRoute(name: "AccountConfirmEmail",
				url: "confirmed/{model}",
				defaults: new { controller = "Account", action = "ConfirmEmail", model = UrlParameter.Optional });

            routes.MapRoute(name: "AccountForgotPassword",
                url: "forgot",
                defaults: new { controller = "Account", action = "ForgotPassword" });

            routes.MapRoute(name: "AccountEmailForgotPasswordGet",
                url: "email_forgot",
                defaults: new { controller = "Account", action = "EmailForgotPassword" });

            routes.MapRoute(name: "AccountEmailForgotPasswordPost",
                url: "Account/EmailForgotPassword/{model}",
                defaults: new { controller = "Account", action = "EmailForgotPassword", model = UrlParameter.Optional });

            routes.MapRoute(name: "AccountForgotPasswordConfirmation",
               url: "email_reset",
               defaults: new { controller = "Account", action = "ForgotPasswordConfirmation" });

            routes.MapRoute(name: "AccountResetPassword",
                url: "reset_password/{model}",
                defaults: new { controller = "Account", action = "ResetPassword", model = UrlParameter.Optional });

            routes.MapRoute(name: "AccountResetPasswordPost",
                url: "reset_password/{model}",
                defaults: new { controller = "Account", action = "ResetPassword", model = UrlParameter.Optional });

            routes.MapRoute(name: "AccountResetPasswordConfirmation",
                url: "reset_password_success",
                defaults: new { controller = "Account", action = "ResetPasswordConfirmation" });


            #endregion

            #region Роуты Redactor и Admin

            routes.MapRoute(name: "AdminTools",
				url: "admin_panel/{action}/{id}",
				defaults: new { controller = "AdminTools", action = "Index", id = UrlParameter.Optional });

			routes.MapRoute(name: "RedactorTools",
				url: "editor_panel/{action}/{id}",
				defaults: new { controller = "RedactorTools", action = "Index", id = UrlParameter.Optional });

			#endregion

			#region Роуты Error

			routes.MapRoute(name: "Error",
				url: "error/{action}",
				defaults: new { controller = "Error" });

            #endregion

            #region sitemap
            routes.MapRoute(name: "SiteMap",
                url: "sitemap.xml",
                defaults: new { controller = "User", action = "SitemapXml", id = UrlParameter.Optional });
            #endregion

            #region Нереализованные роуты

            //routes.MapRoute(name: "UserAddMarkPost",
            //	url: "films/add_mark/{mark}",
            //	defaults: new { controller = "User", action = "AddMark", mark = UrlParameter.Optional });

            //routes.MapRoute(name: "UserSubscribe",
            //	url: "films/subscribe/{id}",
            //	defaults: new { controller = "User", action = "Subscribe", id = UrlParameter.Optional });

            //routes.MapRoute(name: "UserUnsubscribe",
            //	url: "films/unsubscribe/{id}",
            //	defaults: new { controller = "User", action = "Unsubscribe", id = UrlParameter.Optional });

            //routes.MapRoute(name: "UserFilter",
            //	url: "films/filter",
            //	defaults: new { controller = "User", action = "Filter"});

            //routes.MapRoute(name: "UserSearch",
            //	url: "films/search",
            //	defaults: new { controller = "User", action = "Search" });

            #endregion
        }
    }
}
