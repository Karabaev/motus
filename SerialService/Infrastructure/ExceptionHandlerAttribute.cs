namespace SerialService.Infrastructure
{
	using System.Web.Mvc;
	using NLog;

	/// <summary>
	/// Аттрибут обработки необработанных исключений в контроллере или экшене.
	/// </summary>
	public class ExceptionHandlerAttribute : FilterAttribute, IExceptionFilter
	{
		public void OnException(ExceptionContext filterContext)
		{
			if(!filterContext.ExceptionHandled)
			{
				filterContext.Result = new HttpStatusCodeResult(500);
				filterContext.ExceptionHandled = true;
				this.logger.Error(filterContext.Exception, "Необработаное исключение. Пользователю возвращается ошибка HTTP 500");
			}
		}

		private readonly Logger logger = LogManager.GetCurrentClassLogger();
	}
}