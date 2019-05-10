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

    public class VideoMaterialUrlDownloadModule : BaseModule
    {
        public int DownloadListByUrl(string url, string selector, string authorEmail)
        {
            Task.Run(() => this.logger.Info($"Начало загрузки информации о фильмах их списка {url}"));
            int result = 0;

            if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(selector) || string.IsNullOrEmpty(authorEmail))
            {
                Task.Run(() => this.logger.Info("Пустой аргумент"));
                return result;
            }

            var parser = new KPParser(url);

            IEnumerable<string> kpIdList = parser.GetAllId(selector);
            if (kpIdList == null || !kpIdList.Any())
            {
                Task.Run(() => this.logger.Info($"По данному url не найдено соответсвий {url}"));
                return result;
            }

            var dbIdList = unitOfWork.VideoMaterials.GetAll().Select(vm => vm.KinopoiskID).ToList();
            var diffList = kpIdList.Where(id => !dbIdList.Contains(id));

            string message;

            foreach (var id in diffList)
            {
                try
                {
                    VideoMaterial material = GetVideoMaterialFromInfo
                    (
                        this.infoAgentService.GetFilmInfo(id),
                        authorEmail
                    );

                    if (this.unitOfWork.VideoMaterials.Create(material))
                    {
                        message = $"[{material.Title}] успешно добавлен в базу";
                        result++;
                        Task.Run(() => this.logger
                        .Info(message));
                        Console.WriteLine(message);
                    }
                    else
                    {
                        message = $"Не удалось загрузить в базу [{material.Title}]";
                        Task.Run(() => this.logger
                        .Warn(message));
                        Console.WriteLine(message);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    continue;
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

            const string selector = " js-film-list-item";
            string url;

            Console.WriteLine("Введи url списка фильмов:");

            url = Console.ReadLine();

            try
            {
                this.DownloadListByUrl(url, selector, authorMail);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                this.Launch();
            }
        }

        private readonly IAppUnitOfWork unitOfWork = AppUnitOfWork.GetInstance();
        private readonly InfoAgentService infoAgentService = new InfoAgentService();
    }
}
