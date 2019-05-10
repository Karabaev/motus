namespace Updater.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using InfoAgent;
    using SerialService.DAL;
    using SerialService.DAL.Entities;
    using SerialService.Infrastructure.Exceptions;
    using InfoAgent.Moonwalk;
    using Shared;

    public class MoonwalkDownloadModule : BaseModule
    {
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
        private int DownloadVideoMaterials(IEnumerable<FilmInfo> filmInfoList, string authorEmail)
        {
            int result = 0;

            foreach (var item in filmInfoList)
            {
                try
                {
                    VideoMaterial videoMaterial = null;

                    try
                    {
                        videoMaterial = FilmInfoToVideoMaterialConverter.Convert(item, authorEmail);
                    }
                    catch(ArgumentNullException ex)
                    {
                        logger.Warn(ex, "Передан пустой параметр");
                    }

                    if (this.unitOfWork.VideoMaterials.Create(videoMaterial))
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
        public int DownloadFilms(string authorEmail)
        {
            return this.DownloadVideoMaterials(this.GetFullBaseFilmInfoList(), authorEmail);
        }

        public override void Launch()
        {
            ConfigManager configManager = ConfigManager.GetInstance();
            string authorMail = string.Empty;

            try
            {
                authorMail = (string)configManager.Config[ConfigKeys.VIDEO_MATERIAL_AUTHOR_MAIL];
            }
            catch (NullReferenceException ex)
            {
                Task.Run(() => this.logger.Error(ex, "Файл конфигурациине не загружен"));
                return;
            }
            catch (InvalidCastException ex)
            {
                Task.Run(() => this.logger.Error(ex, "Один из параметров имеет неверный формат"));
                return;
            }
            catch (KeyNotFoundException ex)
            {
                Task.Run(() => this.logger.Error(ex, "Неверный файл конфигурации"));
                return;
            }
            int totalDownloaded = -1;
            DateTime startDateTime = DateTime.Now;
            Task.Run(() => this.logger.Info("Мунвалк Даунлоадер запущен"));
            
            try
            {
                totalDownloaded = this.DownloadFilms(authorMail);
            }
            catch (Exception ex)
            {
                this.logger.Error(ex, "Ошибка при загрузке фильмов из базы Мунвалка");
            }

            DateTime endDateTime = DateTime.Now;
            TimeSpan result = endDateTime - startDateTime;
            Task.Run(() => this.logger.Fatal("Фильмов было загружено: {0}", totalDownloaded));
            Task.Run(() => this.logger.Fatal("Потрачено времени на проверку обновлений: {0}", result));
        }

        private readonly IAppUnitOfWork unitOfWork = AppUnitOfWork.GetInstance();
        private readonly Service service = new Service();
    }
    
}
