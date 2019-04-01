namespace SerialService
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Web;
	using System.Web.Mvc;
	using System.Web.Routing;

	public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
			//routes.MapMvcAttributeRoutes();
				
			//routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			//routes.MapRoute(name: "UserIndex",
			//				url: "films",
			//				defaults: new { controller = "User", action = "Index" });

			//routes.MapRoute(name: "UserVideoMaterialDetailPage",
			//				url: "films/{id}",
			//				defaults: new { controller = "User", action = "VideoMaterialDetailPage", id = UrlParameter.Optional });

			//routes.MapRoute(name: "UserPersonalAccount",
			//				url: "personal_account_page",
			//				defaults: new { controller = "User", action = "PersonalAccount" });

			//routes.MapRoute(name: "AccountLogin",
			//				url: "login_page",
			//				defaults: new { controller = "Account", action = "Login" });

			//routes.MapRoute(name: "AccountRegister",
			//				url: "register_page",
			//				defaults: new { controller = "Account", action = "Register" });


			routes.MapRoute(name: "Default",
				url: "{controller}/{action}/{id}",
				defaults: new { controller = "User", action = "Index", id = UrlParameter.Optional });
		}
	}
}
