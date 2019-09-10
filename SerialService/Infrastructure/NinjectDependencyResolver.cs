﻿namespace SerialService.Infrastructure
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
            this.kernel.Bind<IDbContext>().To<ApplicationDbContext>().InTransientScope();
            this.kernel.Bind<IAppUnitOfWork>().To<AppUnitOfWork>().InTransientScope();
        }
    }
}