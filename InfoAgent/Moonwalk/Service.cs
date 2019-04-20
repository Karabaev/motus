namespace InfoAgent.Moonwalk
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Net;
    using Newtonsoft.Json;
    using NLog;
    using Model;

    public class Service
    {
        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="apiKey"></param>
        public Service(string apiKey)
        {
            this.apiKey = apiKey;
        }

        /// <summary>
        /// Получить URL запроса по его типу.
        /// </summary>
        /// <param name="requestType"></param>
        /// <returns></returns>
        private string GetRequestURL(RequestTypes requestType)
        {
            string result = string.Empty;

            switch (requestType)
            {
                case RequestTypes.ForeignFilms:
                    result = string.Format("http://moonwalk.cc/api/movies_foreign.json?api_token={0}", apiKey);
                    break;
                case RequestTypes.RussianFilms:
                    result = string.Format("http://moonwalk.cc/api/movies_russian.json?api_token={0}", apiKey);
                    break;
                case RequestTypes.ForeignSerials:
                    result = string.Format("http://moonwalk.cc/api/serials_foreign.json?api_token={0}", apiKey);
                    break;
                case RequestTypes.RussianSerials:
                    result = string.Format("http://moonwalk.cc/api/serials_russian.json?api_token={0}", apiKey);
                    break;
                case RequestTypes.AnimeFilms:
                    result = string.Format("http://moonwalk.cc/api/movies_anime.json?api_token={0}", apiKey);
                    break;
                case RequestTypes.AnimeSerials:
                    result = string.Format("http://moonwalk.cc/api/serials_anime.json?api_token={0}", apiKey);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Некорректный тип запроса");
            }

            return result;
        }

        /// <summary>
        /// Получить фид.
        /// </summary>
        /// <param name="requestType"></param>
        /// <returns></returns>
        private Response GetResponseObject(RequestTypes requestType)
        {
            try
            {
                string response = this.GetRequestResponse(requestType);
                return JsonConvert.DeserializeObject<Response>(response);
            }
            catch (WebException ex)
            {
                this.logger.Error(ex, "Не удалось выполнить запрос {0}", requestType);
                return null;
            }
        }

        /// <summary>
        /// Получить список FilmInfo.
        /// </summary>
        /// <returns></returns>
        public List<FilmInfo> GetAllFilmInfoList()
        {
            List<FilmInfo> result = new List<FilmInfo>();
            List<Response> responses = new List<Response>();
            responses.Add(this.GetResponseObject(RequestTypes.ForeignFilms));
            responses.Add(this.GetResponseObject(RequestTypes.RussianFilms));
            responses.Add(this.GetResponseObject(RequestTypes.AnimeFilms));
            responses.Add(this.GetResponseObject(RequestTypes.ForeignSerials));
            responses.Add(this.GetResponseObject(RequestTypes.RussianSerials));
            responses.Add(this.GetResponseObject(RequestTypes.AnimeSerials));
            return result;
        }

        private List<FilmInfo> MapResponseToFilmInfoList(Response response)
        {
            if (response == null)
                return null;

            List<FilmInfo> result = new List<FilmInfo>();


            return result;
        }

        private FilmInfo GetFilmInfoFromModelEntity(IVideoMaterial modelEntity)
        {
            if (modelEntity == null)
                return null;

            FilmInfo result = new FilmInfo
            {
                Actors = new List<string>(modelEntity.material_data.actors),
                Countries = new List<string>(modelEntity.material_data.countries),
                Description = modelEntity.material_data.description,
                Duration = ((Movie)modelEntity)?.duration.seconds,
                FilmMakers = new List<string>(modelEntity.material_data.directors),
                Genres = new List<string>(modelEntity.material_data.genres),
                IDMB = modelEntity.material_data.imdb_rating,
                IsBlocked = !modelEntity.block.blocked_at.HasValue,
                IsSerial = (Movie)modelEntity == null,
                KinopoiskID = modelEntity.kinopoisk_id?.ToString(),
                KinopoiskRating = modelEntity.material_data.kinopoisk_rating,
                MoonWalkAddDate = ((Movie)modelEntity)?.added_at,
                OriginalTitle = modelEntity.title_en,
                PosterHref = modelEntity.material_data.poster,
                ReleaseDate = modelEntity.material_data.year,
                Tagline = modelEntity.material_data.tagline == "-" ? string.Empty : modelEntity.material_data.tagline,
                Title = modelEntity.title_ru,
                Translations = new List<Translation>()
            };

            return result;
        }

        private List<Translation> GetTranslations(JArray jArr)
        {
            List<Translation> result = new List<Translation>();

            foreach (var trn in jArr)
            {
                Translation translation = new Translation();
                translation.listOfSeasons = new List<SeasonInfo>();
                translation.studioName = trn["translator"].Value<string>();

                try
                {
                    translation.lastEpisodeTime = trn["last_episode_time"].Value<DateTime?>();
                }
                catch (ArgumentNullException ex)
                {
                    this.logger.Warn(ex, "Значение last_episode_time не определено");
                }

                try
                {
                    translation.updateTime = trn["material_data"]["updated_at"].Value<DateTime?>();
                }
                catch (ArgumentNullException ex)
                {
                    this.logger.Warn(ex, "Значение material_data.updated_at не определено");
                }

                try
                {
                    foreach (var season in trn["season_episodes_count"])
                    {
                        translation.listOfSeasons.Add(new SeasonInfo
                        {
                            seasonNumber = season["season_number"].Value<int?>(),
                            episodesCount = season["episodes_count"].Value<int?>()
                        });
                    }
                }
                catch (NullReferenceException ex)
                {
                    this.logger.Info("У фильма сезонов нет");
                }

                result.Add(translation);
            }

            Task.Run(() => this.logger.Info("Окончание парсинга переводов. Результат: \"{0}\"", JsonConvert.SerializeObject(result)));
            return result;
        }

        /// <summary>
        /// Получить ответ на запрос в виде Json текста.
        /// </summary>
        /// <param name="requestType"></param>
        /// <returns></returns>
        private string GetRequestResponse(RequestTypes requestType)
        {
            string result = string.Empty;
            string requestUrl = this.GetRequestURL(requestType);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUrl);

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                request.ContentType = "application/json";

                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        result = reader.ReadToEnd();
                    }
                }
            }

            return result;
        }

        private readonly string apiKey = "a3275d42cea4b2dfb65084eea682885d";
        private readonly Logger logger = LogManager.GetCurrentClassLogger();
    }
}
