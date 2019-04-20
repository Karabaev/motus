namespace Spammer.Moonwalk
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NLog;
    using InfoAgent;
    using InfoAgent.Exceptions;
    using SerialService.DAL;
    using SerialService.DAL.Entities;
    using SerialService.Infrastructure;
    using SerialService.Infrastructure.Exceptions;
    using SerialService.Infrastructure.Helpers;
    using KinoPoiskParser;

    public class MoonwalkDownloader
    {
        private MoonwalkDownloader(IAppUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        private List<FilmInfo> GetFilmInfoList()
        {
            List<FilmInfo> result = new List<FilmInfo>();



            return result;
        }

        public static MoonwalkDownloader GetInstance(IAppUnitOfWork unitOfWork)
        {
            if (MoonwalkDownloader.instance == null)
                MoonwalkDownloader.instance = new MoonwalkDownloader(unitOfWork);

            return MoonwalkDownloader.instance;
        }

        private readonly IAppUnitOfWork unitOfWork;
        private readonly Logger logger = LogManager.GetCurrentClassLogger();
        private static MoonwalkDownloader instance;
    }
    
}
