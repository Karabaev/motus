namespace SerialService
{
    using System.Data.Entity;
    using System.Web.Mvc;
    using System.Web.Optimization;
    using System.Web.Routing;
    using App_Start;
    using Infrastructure;
    using Ninject;
	using NLog;
	using System;
	using DAL;

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
			Database.SetInitializer(new AppDbInitializer());
			AreaRegistration.RegisterAllAreas();
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			NinjectDependencyResolver ninject = new NinjectDependencyResolver(new StandardKernel());
			DependencyResolver.SetResolver(ninject);
            AutoMapperConfig.AutoMapperInit();
			CacheFiller.FilterFillCache();
			ElasticIndex.Index(DependencyResolver.Current.GetService<IAppUnitOfWork>());
		}
    }
}
