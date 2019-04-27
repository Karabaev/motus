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
            ViewBag.Title = "Неверный запрос";
            ViewBag.ErrorText = "Ошибка 400. Неверный запрос";
			return View("Error");
		}

		public ActionResult Unauthorized()
		{
			Response.StatusCode = 401;
            ViewBag.Title = "Требуетя авторизация";
            ViewBag.ErrorText = "Ошибка 401. Требуетя авторизация";
            return View("Error");
		}

		public ActionResult Forbidden()
		{
			Response.StatusCode = 403;
            ViewBag.Title = "Не достаточно прав";
            ViewBag.ErrorText = "Ошибка 403. У вас не достаточно прав";
            return View("Error");
		}

		public ActionResult NotFound()
		{
			Response.StatusCode = 404;
            ViewBag.Title = "Страница не найдена";
            ViewBag.ErrorText = "Ошибка 404. Страница не найдена";
            return View("Error");
        }

		public ActionResult InternalServerError()
		{
			Response.StatusCode = 500;
            ViewBag.Title = "Внутренняя ошибка сервера";
            ViewBag.ErrorText = "Ошибка 500. Внутренняя ошибка сервера";
            return View("Error");
        }

		public ActionResult ServiceUnavailable()
		{
			Response.StatusCode = 503;
            ViewBag.Title = "Сервис не доступен";
            ViewBag.ErrorText = "Ошибка 503. Сервис не доступен. Повторите попытку позже";
            return View("Error");
        }
	}
}