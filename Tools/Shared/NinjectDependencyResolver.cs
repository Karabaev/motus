namespace Tools.Shared
{
    using System;
    using System.Collections.Generic;
    using Ninject;
    using SerialService.DAL.Context;
    using SerialService.DAL;

    public class NinjectDependencyResolver
    {
        private readonly IKernel kernel;

        private NinjectDependencyResolver()
        {
            this.kernel = new StandardKernel();
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
            this.kernel.Bind<IDbContext>().To<ApplicationDbContext>().InTransientScope();
            this.kernel.Bind<IAppUnitOfWork>().To<AppUnitOfWork>().InTransientScope();
        }

        public static NinjectDependencyResolver Instance
        {
            get
            {
                if (instance == null)
                    instance = new NinjectDependencyResolver();

                return instance;
            }
        }

        private static NinjectDependencyResolver instance;
    }
}
