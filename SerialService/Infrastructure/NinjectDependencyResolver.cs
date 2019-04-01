namespace SerialService.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Ninject;
    using DAL.Repository;
    using Services.Interfaces;
    using Services;
    using DAL.Context;
    using DAL;

    public class NinjectDependencyResolver : IDependencyResolver
    {
        private IKernel kernel;
        private ApplicationDbContext DB;

        public NinjectDependencyResolver(IKernel kernel)
        {
            this.kernel = kernel;
            this.AddBindings();
        }
        /// <summary>
        /// Возвращает реализацию
        /// </summary>
        /// <param name="serviceType">Необходимый тип реализации</param>
        /// <returns></returns>
        public object GetService(Type serviceType)
        {
            return this.kernel.TryGet(serviceType);
        }
        /// <summary>
        /// Возвращает список реализаций
        /// </summary>
        /// <param name="serviceType">Необходимый тип реализации</param>
        /// <returns></returns>
        public IEnumerable<object> GetServices(Type serviceType)
        {
            return this.kernel.GetAll(serviceType);
        }

        private void AddBindings()
        {
            this.DB = new ApplicationDbContext();
            this.kernel.Bind<IAppUnitOfWork>().To<AppUnitOfWork>();
            this.kernel.Bind<IUserService>().To<UserService>();
            this.kernel.Bind<IPersonService>().To<PersonService>().WithConstructorArgument("context", DB);
            this.kernel.Bind<ICountryService>().To<CountryService>().WithConstructorArgument("context", DB);
            this.kernel.Bind<IGenreService>().To<GenreService>().WithConstructorArgument("context", DB);
            this.kernel.Bind<IThemeService>().To<ThemeService>().WithConstructorArgument("context", DB);
            this.kernel.Bind<ITranslationService>().To<TranslationService>().WithConstructorArgument("context", DB);
            this.kernel.Bind<IVideoMarkService>().To<VideoMarkService>();
            this.kernel.Bind<IPictureService>().To<PictureService>().WithConstructorArgument("context", DB);
            this.kernel.Bind<IVideoMaterialRepository>().To<VideoMaterialRepository>().WithConstructorArgument("context", DB);
            this.kernel.Bind<IVideoMaterialService>().To<VideoMaterialService>().WithConstructorArgument("context", DB);
        }
    }
}