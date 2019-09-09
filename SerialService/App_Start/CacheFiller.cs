namespace SerialService.App_Start
{
    using SerialService.DAL;
    using SerialService.Infrastructure.Core;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    public static class CacheFiller
    {
        public static void FilterFillCache()
        {
            IAppUnitOfWork unitOfWork = DependencyResolver.Current.GetService<IAppUnitOfWork>();
            Dictionary<string, List<string>> filterLists = new Dictionary<string, List<string>>
            {
                {"genres",unitOfWork.Genres.GetAll().Select(g=>g.Name).ToList()},
                {"countries",unitOfWork.Countries.GetAll().Select(c=>c.Name).ToList()},
                {"translations",unitOfWork.Translations.GetAll().Where(t => !string.IsNullOrEmpty(t.Name)).Select(t=>t.Name).ToList()}
            };
            GlobalCache.AddOrGetExisting("filter-lists", filterLists);
        }
    }
}