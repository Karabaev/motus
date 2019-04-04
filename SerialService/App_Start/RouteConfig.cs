namespace SerialService
{
    using System.Web.Mvc;
    using System.Web.Routing;

    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
			routes.MapRoute(name: "Default",
				url: "{controller}/{action}/{id}",
				defaults: new { controller = "User", action = "Index", id = UrlParameter.Optional });
		}
	}
}
