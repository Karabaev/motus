namespace SerialService.Infrastructure.Managers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using SerialService.DAL;
    using SerialService.DAL.Entities;
    using SerialService.Services;
    using SerialService.Services.Interfaces;
    using NLog;

    public class VideoMaterialManager
    {
        public VideoMaterialManager()
        {
            this.unitOfWork = AppUnitOfWork.GetInstance();
        }

        public bool CreateNewVideoMaterial(VideoMaterial videoMaterial)
        {
            if (videoMaterial == null)
                throw new ArgumentNullException("videoMaterial");

            bool result = this.unitOfWork.VideoMaterials.Create(videoMaterial);

            
            return result;
        }

        public VideoMaterial CreateVideoMaterialInstance(VideoMaterialInitializer initializer)
        {
            var author = this.GetUserByEmail(initializer.AuthorMail);
            VideoMaterial result = new VideoMaterial();
            result.Title = initializer.Title;
            result.OriginalTitle = initializer.OriginalTitle;
            result.Text = initializer.Text;
            result.Tagline = initializer.Tagline;
            result.KinopoiskID = initializer.KinopoiskID;
            result.IDMB = initializer.IDMB;
            result.KinopoiskRating = initializer.KinopoiskRating;
            result.Duration = initializer.Duration;
            result.MoonWalkAddDate = initializer.MoonWalkAddDate;
            result.ReleaseDate = initializer.ReleaseDate;
            result.CheckStatus = initializer.CheckStatus;
            result.WatchForUpdates = initializer.WatchForUpdates;
            result.IsArchived = initializer.IsArchived;

            if (author != null)
            {
                result.Author = author;
                result.AuthorID = author.Id;
            }

            result.Countries = (List<Country>)this.GetCountryListByNameList(initializer.Countries);
            result.Genres = (List<Genre>)this.GetGenreListByNameList(initializer.Genres);
            result.Actors = (List<Person>)this.GetPersonListByNameList(initializer.Actors);
            result.FilmMakers = (List<Person>)this.GetPersonListByNameList(initializer.FilmMakers);
            result.Themes = (List<Theme>)this.GetThemeListByNameList(initializer.Themes);
            result.Pictures = new List<Picture> { this.GetPicture(initializer.PosterURL) };
            IEnumerable<SerialSeason> serialSeasonsInstances = this.CreateSerialSeasonInstanceList(initializer.SerialSeasonInitializers);
            result.SerialSeasons = (List<SerialSeason>)serialSeasonsInstances;
            return result;
        }

        /// <summary>
        /// Создать экземпляр SerialService Без записи в базу.
        /// </summary>
        /// <param name="initializer"></param>
        /// <returns></returns>
        private SerialSeason CreateSerialSeasonInstance(SerialSeasonInitializer initializer)
        {
            Translation translation = this.GetTranslationByName(initializer.TranslationName);
            SerialSeason result = new SerialSeason();
            result.SeasonNumber = initializer.SeasonNumber;
            result.EpisodesCount = initializer.EpisodesCount;
            result.LastEpisodeTime = initializer.LastEpisodeTime;
            result.TranslationID = translation.ID;
            result.Translation = translation;
            return result;
        }

        /// <summary>
        /// Создать лист SerialSeason без записив базу.
        /// </summary>
        /// <param name="initializerList"></param>
        /// <returns></returns>
        private IEnumerable<SerialSeason> CreateSerialSeasonInstanceList(IEnumerable<SerialSeasonInitializer> initializerList)
        {
            if (initializerList == null)
                throw new ArgumentNullException("initializerList");

            List<SerialSeason> result = new List<SerialSeason>();

            foreach (var item in initializerList)
                result.Add(this.CreateSerialSeasonInstance(item));

            return result;
        }

        /// <summary>
        /// Найти юзера по email. Если юзер не найден, новый юзер создан НЕ будет.
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        private ApplicationUser GetUserByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentNullException("email");

            return this.unitOfWork.Users.GetByMainStringProperty(email);
        }

        /// <summary>
        /// Найти список стран по списку названий без учета регистра. Если страна не найдена, будет создана новая.
        /// </summary>
        /// <param name="nameList"></param>
        /// <returns></returns>
        private IEnumerable<Country> GetCountryListByNameList(IEnumerable<string> nameList)
        {
            if (nameList == null)
                throw new ArgumentNullException("nameList");

            List<Country> result = new List<Country>();
            List<Country> notFoundCountries = new List<Country>();

            foreach (var item in nameList)
            {
                if (!string.IsNullOrWhiteSpace(item))
                {
                    Country country = this.unitOfWork.Countries.GetByMainStringProperty(item);

                    if(country == null)
                    {
                        country = new Country { Name = item };
                        notFoundCountries.Add(country);
                    }

                    result.Add(country);
                }
            }

            this.unitOfWork.Countries.Create(notFoundCountries);
            return result;
        }

        /// <summary>
        /// Найти список жанров по списку названий без учета регистра. Если жанр не найден, будет создан новый.
        /// </summary>
        /// <param name="nameList"></param>
        /// <returns></returns>
        private IEnumerable<Genre> GetGenreListByNameList(IEnumerable<string> nameList)
        {
            if (nameList == null)
                throw new ArgumentNullException("nameList");

            List<Genre> result = new List<Genre>();
            List<Genre> notFoundGenres = new List<Genre>();

            foreach (var item in nameList)
            {
                if (!string.IsNullOrWhiteSpace(item))
                {
                    Genre genre = this.unitOfWork.Genres.GetByMainStringProperty(item);

                    if (genre == null)
                    {
                        genre = new Genre { Name = item };
                        notFoundGenres.Add(genre);
                    }

                    result.Add(genre);
                }
            }

            this.unitOfWork.Genres.Create(notFoundGenres);
            return result;
        }

        /// <summary>
        /// Найти список людей по списку имен без учета регистра. Если человек не найден, будет создан новый.
        /// </summary>
        /// <param name="nameList"></param>
        /// <returns></returns>
        private IEnumerable<Person> GetPersonListByNameList(IEnumerable<string> nameList)
        {
            if (nameList == null)
                throw new ArgumentNullException("nameList");

            List<Person> result = new List<Person>();
            List<Person> notFoundPeople = new List<Person>();

            foreach (var item in nameList)
            {
                if (!string.IsNullOrWhiteSpace(item))
                {
                    Person person = this.unitOfWork.Persons.GetByMainStringProperty(item);

                    if (person == null)
                    {
                        person = new Person { FullName = item };
                        notFoundPeople.Add(person);
                    }

                    result.Add(person);
                }
            }

            this.unitOfWork.Persons.Create(notFoundPeople);
            return result;
        }

        /// <summary>
        /// Найти список тем по списку названий без учета регистра. Если тема не найдена, будет создана новая.
        /// </summary>
        /// <param name="nameList"></param>
        /// <returns></returns>
        private IEnumerable<Theme> GetThemeListByNameList(IEnumerable<string> nameList)
        {
            if (nameList == null)
                throw new ArgumentNullException("nameList");

            List<Theme> result = new List<Theme>();
            List<Theme> notFoundThemes = new List<Theme>();

            foreach (var item in nameList)
            {
                if (!string.IsNullOrWhiteSpace(item))
                {
                    Theme theme = this.unitOfWork.Themes.GetByMainStringProperty(item);

                    if (theme == null)
                    {
                        theme = new Theme { Name = item };
                        notFoundThemes.Add(theme);
                    }

                    result.Add(theme);
                }
            }

            this.unitOfWork.Themes.Create(notFoundThemes);
            return result;
        }

        /// <summary>
        /// Найти список картинок в базе по списку картинок. Если картинка не найдена, будет создана новая.
        /// </summary>
        /// <param name="nameList"></param>
        /// <returns></returns>
        private IEnumerable<Picture> GetPictureListByPictureList(IEnumerable<Picture> picList)
        {
            if (picList == null)
                throw new ArgumentNullException("picList");

            List<Picture> result = new List<Picture>();
            List<Picture> notFoundPictures = new List<Picture>();

            foreach (var item in picList)
            {
                Picture picture = this.unitOfWork.Pictures.GetByMainStringProperty(item.URL);

                if(picture == null)
                {
                    picture = new Picture { URL = item.URL, IsPoster = item.IsPoster };
                    notFoundPictures.Add(picture);
                }

                result.Add(picture);
            }

            this.unitOfWork.Pictures.Create(notFoundPictures);
            return result;
        }

        private Picture GetPicture(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                url = "/Media/Avatars/no-poster.png";

            Picture result = this.unitOfWork.Pictures.GetByMainStringProperty(url);

            if (result == null)
            {
                result = new Picture { URL = url, IsPoster = true };
                this.unitOfWork.Pictures.Create(result);
            }

            return result;
        }

        /// <summary>
        /// Получить перевод по его имени без учета регистра. Если перевод не найден, будет создан новый.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private Translation GetTranslationByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                name = "";

            Translation result = this.unitOfWork.Translations.GetByMainStringProperty(name);

            if(result == null)
            {
                result = new Translation { Name = name };
                this.unitOfWork.Translations.Create(result);
            }

            return result;
        }

        /// <summary>
        /// Найти список сезонов в базе по списку сезонов. Если сезон не найден, будет создан новый.
        /// </summary>
        /// <param name="seasonList"></param>
        /// <returns></returns>
        private IEnumerable<SerialSeason> GetSeasonListBySeasonList(IEnumerable<SerialSeason> seasonList)
        {
            if (seasonList == null)
                throw new ArgumentNullException("seasonList");

            List<SerialSeason> result = new List<SerialSeason>();
            List<SerialSeason> notFoundSeasons = new List<SerialSeason>();

            foreach (var item in seasonList)
            {
                SerialSeason season = this.unitOfWork.SerialSeasons.Get(item.SeasonNumber, item.VideoMaterialID, item.TranslationID);

                if (season == null)
                {
                    season = new SerialSeason
                    {
                        SeasonNumber = item.SeasonNumber,
                        EpisodesCount = item.EpisodesCount,
                        LastEpisodeTime = item.LastEpisodeTime,
                        Translation = item.Translation,
                        TranslationID = item.TranslationID
                    };
                    notFoundSeasons.Add(season);
                }

                result.Add(season);
            }

            this.unitOfWork.SerialSeasons.Create(notFoundSeasons);
            return result;
        }

        private readonly IAppUnitOfWork unitOfWork;
        private readonly Logger logger = LogManager.GetCurrentClassLogger();
    }
}