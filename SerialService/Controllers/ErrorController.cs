namespace SerialService.Controllers
{
	using System.Web.Mvc;
	using Infrastructure;

	[ExceptionHandler]
	public class ErrorController : Controller
    {
		public ActionResult BadRequest()
		{
			Response.StatusCode = 400;
			return View();
		}

		public ActionResult Unauthorized()
		{
			Response.StatusCode = 401;
			return View();
		}

		public ActionResult Forbidden()
		{
			Response.StatusCode = 403;
			return View();
		}

		public ActionResult NotFound()
		{
			Response.StatusCode = 404;
			return View();
		}

		public ActionResult InternalServerError()
		{
			Response.StatusCode = 500;
			return View();
		}

		public ActionResult ServiceUnavailable()
		{
			Response.StatusCode = 503;
			return View();
		}
	}
}