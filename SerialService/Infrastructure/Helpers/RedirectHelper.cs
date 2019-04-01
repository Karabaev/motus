namespace SerialService.Infrastructure.Helpers
{
    using System.Web.Mvc;

    public static class RedirectHelper
    {
        /// <summary>
        /// Записывает URL в ViewBag.CurrentURL
        /// </summary>
        /// <param name="viewBag">ViewBag для сохранения URL.</param>
        /// <param name="context">Контекст контроллера, откуда будет сформирован URL.</param>
        public static void  SaveLocalURL(dynamic viewBag, ControllerContext context)
        {
			var routeDataValues = context.RouteData.Values;

			if (routeDataValues.Count == 3)
				viewBag.CurrentURL = string.Format("/{0}/{1}/{2}", routeDataValues["controller"].ToString(), routeDataValues["action"].ToString(), routeDataValues["id"].ToString());
			else
				viewBag.CurrentURL = string.Format("/{0}/{1}", context.RouteData.Values["controller"].ToString(), context.RouteData.Values["action"].ToString());
			//viewBag.CurrentURL = string.Format("/{0}/{1}", context.RouteData.Values["controller"].ToString(), "Index");
			//viewBag.CurrentURL = context.RouteData.Route.GetVirtualPath();
		}
    }
}