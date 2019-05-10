namespace Updater.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using InfoAgent;
    using InfoAgent.Exceptions;
    using SerialService.DAL;
    using SerialService.DAL.Entities;
    using SerialService.Infrastructure;
    using SerialService.Infrastructure.Exceptions;
    using SerialService.Infrastructure.Helpers;
    using KinoPoiskParser;
    using Shared;

    public class VideoMaterialRangeDownloadModule : BaseModule
    {
        private int DownloadAllVideoMaterials(int startKpId, int endKpId, string authorEmail)
        {
            Task.Run(() => this.logger.Info("Начало загрузки информации о фильмах с Кинопоиск ID {0}-{1}", startKpId, endKpId));
            int result = 0;

            foreach (var item in this.GetFilmInfoList(startKpId, endKpId))
            {
                bool createResult = false;

                try
                {
                    if (this.unitOfWork.VideoMaterials.Create(this.GetVideoMaterialFromInfo(item, authorEmail)))
                    {
                        createResult = true;
                    }
                    else
                    {
                        createResult = false;
                        Task.Run(() => this.logger.Warn("Не удалось загрузить в базу информацию о фильме"));
                    }

                }
                catch (EntryAlreadyExistsException ex)
                {
                    Task.Run(() => this.logger.Info("Информация о фильме загружена уже есть в базе"));
                }

                if (createResult)
                {
                    result++;
                    Task.Run(() => this.logger.Info("Информация о фильме {0} загружена в базу", item.Title));
                }
            }

            return result;
        }

        private List<FilmInfo> GetFilmInfoList(int startKpId, int endKpId)
        {
            List<FilmInfo> result = new List<FilmInfo>();

            for (int i = startKpId; i <= endKpId; i++)
            {
                try
                {
                    Task.Run(() => this.logger.Info("Загрузка информации о фильме с Кинопоиск ID {0}", i));
                    result.Add(this.infoAgentService.GetFilmInfo(i.ToString()));
                    Task.Run(() => this.logger.Info("Информация о фильме с Кинопоиск ID {0} загружена", i));
                }
                catch (NotFoundInFilmBaseException ex)
                {
                    Task.Run(() => this.logger.Warn("Не найдена информация о фильме с Кинопоиск ID {0}", i));
                }
                catch (Exception ex)
                {
                    Task.Run(() => this.logger.Error(ex, "Ошибка при загрузке информации о фильме с Кинопоиск ID {0}", i));
                }
            }

            return result;
        }

        private VideoMaterial GetVideoMaterialFromInfo(FilmInfo info, string authorMail)
        {
            VideoMaterial result = null;
            ApplicationUser user = this.unitOfWork.Users.GetByMainStringProperty(authorMail);

            if (user == null)
                throw new EntryNotFoundException(string.Format("Пользователь с email {0} не найден", authorMail));

            result = new VideoMaterial
            {
                Duration = info.Duration,
                IDMB = info.IDMB.Value,
                KinopoiskRating = info.KinopoiskRating,
                KinopoiskID = info.KinopoiskID,
                OriginalTitle = info.OriginalTitle,
                ReleaseDate = info.ReleaseDate,
                Text = info.Description,
                Title = info.Title,
                Tagline = info.Tagline,
                Pictures = new List<Picture>
                {
                    new Picture{ IsPoster = true, URL = info.PosterHref }
                },
                Actors = this.unitOfWork.Persons.AutoSave(info.Actors),
                FilmMakers = this.unitOfWork.Persons.AutoSave(info.FilmMakers),
                Genres = this.unitOfWork.Genres.AutoSave(info.Genres),
                Countries = this.unitOfWork.Countries.AutoSave(info.Countries),
                Author = user,
                AuthorID = user.Id,
                CheckStatus = CheckStatus.Checking,
                WatchForUpdates = info.IsSerial.HasValue && info.IsSerial.Value ? true : false,
                SerialSeasons = new List<SerialSeason>(),
                AddDateTime = DateTime.Now,
                MoonWalkAddDate = info.MoonWalkAddDate,
            };
            TranslationAddHelper translationHelper = new TranslationAddHelper();
            translationHelper.SaveTranslations(info, result, this.unitOfWork.Translations);
            return result;
        }

        public override void Launch()
        {
            int startID = -1;
            int endID = -1;
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

            try
            {
                Console.WriteLine("Введи начальное значение: ");
                startID = int.Parse(Console.ReadLine());
                Console.WriteLine("Введи последнее значение: ");
                endID = int.Parse(Console.ReadLine());
            }
            catch (FormatException)
            {
                Console.WriteLine("Неверный формат ID");
                this.Launch();
            }

            DateTime startDateTime = DateTime.Now;
            Task.Run(() => this.logger.Info("Даунлоадер запущен"));
            int totalDownloaded = -1;

            try
            {
                totalDownloaded = this.DownloadAllVideoMaterials(startID, endID, authorMail);
            }
            catch (EntryNotFoundException ex)
            {
                Task.Run(() => this.logger.Error(ex, "Ошибка при загрузке"));
            }
            catch (Exception ex)
            {
                Task.Run(() => this.logger.Error(ex, "Ошибка при загрузке"));
            }

            DateTime endDateTime = DateTime.Now;
            TimeSpan result = endDateTime - startDateTime;
            Task.Run(() => this.logger.Fatal("Фильмов было загружено: {0}", totalDownloaded));
            Task.Run(() => this.logger.Fatal("Потрачено времени на проверку обновлений: {0}", result));
        }

        private readonly IAppUnitOfWork unitOfWork = AppUnitOfWork.GetInstance();
        private readonly InfoAgentService infoAgentService = new InfoAgentService();
    }
}
