namespace Updater.Moonwalk
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NLog;
    using InfoAgent;
    using SerialService.DAL;
    using SerialService.Infrastructure.Exceptions;
    using InfoAgent.Moonwalk;
    using Shared;

    public class MoonwalkDownloader : IFilmDownloader
    {
        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="unitOfWork"></param>
        private MoonwalkDownloader(IAppUnitOfWork unitOfWork, string filmAuthorEmail)
        {
            this.unitOfWork = unitOfWork;
            this.service = new Service();
            this.authorEmail = filmAuthorEmail;
        }

        /// <summary>
        /// Получить лист объектов FilmInof по всей базе Мунвалка.
        /// </summary>
        /// <returns></returns>
        private List<FilmInfo> GetFullBaseFilmInfoList()
        {
            return this.service.GetAllFilmInfoList();
        }

        /// <summary>
        /// Загрузить в базу фильмы из списка filmInfoList.
        /// </summary>
        /// <param name="authorEmail"></param>
        /// <param name="filmInfoList"></param>
        /// <returns></returns>
        private int DownloadVideoMaterials(IEnumerable<FilmInfo> filmInfoList)
        {
            int result = 0;

            foreach (var item in filmInfoList)
            {
                try
                {
                    if (this.unitOfWork.VideoMaterials.Create(FilmInfoToVideoMaterialConverter.Convert(item, this.authorEmail, this.unitOfWork)))
                    {
                        result++;
                        Task.Run(() => this.logger.Info("Информация о фильме {0} загружена в базу", item.Title));
                    }
                    else
                    {
                        Task.Run(() => this.logger.Warn("Не удалось загрузить в базу информацию о фильме"));
                    }

                }
                catch(ArgumentOutOfRangeException ex)
                {
                    Task.Run(() => this.logger.Error(ex, "Фильм {0}", item.Title));
                }
                catch (EntryAlreadyExistsException ex)
                {
                    Task.Run(() => this.logger.Info("Информация о фильме {0} уже есть в базе", item.Title));
                }
                catch(EntryNotFoundException ex)
                {
                    Task.Run(() => this.logger.Error(ex));
                }
            }

            return result;
        }

        /// <summary>
        /// Загрузить в базу всю базу фильмов/сериалов Мунвалка.
        /// </summary>
        /// <returns></returns>
        public int DownloadFilms()
        {
            return this.DownloadVideoMaterials(this.GetFullBaseFilmInfoList());
        }

        /// <summary>
        /// Получить экземплар синглтон класса.
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <returns></returns>
        public static MoonwalkDownloader GetInstance(IAppUnitOfWork unitOfWork, string filmAuthorEmail)
        {
            if (MoonwalkDownloader.instance == null)
                MoonwalkDownloader.instance = new MoonwalkDownloader(unitOfWork, filmAuthorEmail);

            return MoonwalkDownloader.instance;
        }

        private readonly IAppUnitOfWork unitOfWork;
        private readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly Service service;
        private readonly string authorEmail;
        private static MoonwalkDownloader instance;
    }
    
}
