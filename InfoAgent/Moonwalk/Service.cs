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
    using AdditionalModel;
    using System.Configuration;

    public class Service
    {
        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="apiKey"></param>
        public Service()
        {
            this.apiKey = ConfigurationManager.AppSettings["MoonwalkApiKey"];
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
                    result = string.Format("http://moonwalk.cc/api/movies_foreign.json?api_token={0}", this.apiKey);
                    break;
                case RequestTypes.RussianFilms:
                    result = string.Format("http://moonwalk.cc/api/movies_russian.json?api_token={0}", this.apiKey);
                    break;
                case RequestTypes.ForeignSerials:
                    result = string.Format("http://moonwalk.cc/api/serials_foreign.json?api_token={0}", this.apiKey);
                    break;
                case RequestTypes.RussianSerials:
                    result = string.Format("http://moonwalk.cc/api/serials_russian.json?api_token={0}", this.apiKey);
                    break;
                case RequestTypes.AnimeFilms:
                    result = string.Format("http://moonwalk.cc/api/movies_anime.json?api_token={0}", this.apiKey);
                    break;
                case RequestTypes.AnimeSerials:
                    result = string.Format("http://moonwalk.cc/api/serials_anime.json?api_token={0}", this.apiKey);
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
            List<VideoMaterialWithTranslations> videoMaterials = new List<VideoMaterialWithTranslations>();
            Response response = this.GetResponseObject(RequestTypes.ForeignFilms);
            videoMaterials.AddRange(this.GetVideoMaterial(response.report.movies));
            response = this.GetResponseObject(RequestTypes.RussianFilms);
            videoMaterials.AddRange(this.GetVideoMaterial(response.report.movies));
            response = this.GetResponseObject(RequestTypes.AnimeFilms);
            videoMaterials.AddRange(this.GetVideoMaterial(response.report.movies));
            response = this.GetResponseObject(RequestTypes.ForeignSerials);
            videoMaterials.AddRange(this.GetVideoMaterial(response.report.serials));
            response = this.GetResponseObject(RequestTypes.RussianSerials);
            videoMaterials.AddRange(this.GetVideoMaterial(response.report.serials));
            response = this.GetResponseObject(RequestTypes.AnimeSerials);
            videoMaterials.AddRange(this.GetVideoMaterial(response.report.serials));
            List<FilmInfo> result = this.GetFilmInfoList(videoMaterials);
            return result;
        }

        /// <summary>
        /// Получить лист объетов видеометериалов промежуточной сущности.
        /// </summary>
        /// <param name="videoMaterials"></param>
        /// <returns></returns>
        private List<VideoMaterialWithTranslations> GetVideoMaterial(IVideoMaterial[] videoMaterials)
        {
            if (videoMaterials == null)
                return null;

            List<VideoMaterialWithTranslations> result = new List<VideoMaterialWithTranslations>();

            foreach (var item in videoMaterials)
            {
                VideoMaterialWithTranslations videoTrans = result.FirstOrDefault(r => r.KinopoiskID == item.kinopoisk_id);

                if (videoTrans != null)
                {
                    videoTrans.Translations.Add(item);
                }
                else
                {
                    videoTrans = new VideoMaterialWithTranslations
                    {
                        KinopoiskID = item.kinopoisk_id,
                        Translations = new List<IVideoMaterial> { item }
                    };
                    result.Add(videoTrans);
                }
            }

            return result;
        }

        private List<FilmInfo> GetFilmInfoList(List<VideoMaterialWithTranslations> videoMaterials)
        {
            if (videoMaterials == null)
                return null;

            List<FilmInfo> result = new List<FilmInfo>();

            foreach (var item in videoMaterials)
                result.Add(this.GetFilmInfo(item));

            return result;
        }

        /// <summary>
        /// Получить объект выходной сущности FilmInfo из объекта промежуточной сущности VideoMaterialWithTranslations.
        /// </summary>
        /// <param name="videoMaterial"></param>
        /// <returns></returns>
        private FilmInfo GetFilmInfo(VideoMaterialWithTranslations videoMaterial)
        {
            if (videoMaterial == null)
                return null;

            if (!videoMaterial.Translations.Any())
                throw new ArgumentOutOfRangeException("videoMaterial", "Свойство Translations не имеет ни одного элемента");

            if(videoMaterial.Translations.Where(t => t == null).Any())
                throw new ArgumentOutOfRangeException("videoMaterial", "Свойство Translations имеет элемент со значением null");

            IVideoMaterial modelEntity = videoMaterial.Translations[0];
            FilmInfo result = new FilmInfo();

            result.Actors = modelEntity.material_data?.actors == null ?
                            new List<string>() : 
                            new List<string>(modelEntity.material_data?.actors);
            result.Countries = modelEntity.material_data?.countries == null ?
                               new List<string>() :
                               new List<string>(modelEntity.material_data?.countries);
            result.Description = modelEntity.material_data?.description;
            result.Duration = (modelEntity as Movie)?.duration?.seconds;
            result.FilmMakers = modelEntity.material_data?.directors == null ?
                                new List<string>() :
                                new List<string>(modelEntity.material_data?.directors);
            result.Genres = modelEntity.material_data?.genres == null ?
                            new List<string>() :
                            new List<string>(modelEntity.material_data?.genres);
            result.IDMB = modelEntity.material_data?.imdb_rating;
            result.IsBlocked = !modelEntity.block.blocked_at.HasValue;
            result.IsSerial = (modelEntity as Movie) == null;
            result.KinopoiskID = modelEntity.kinopoisk_id?.ToString();
            result.KinopoiskRating = modelEntity.material_data?.kinopoisk_rating;
            result.MoonWalkAddDate = (modelEntity as Movie)?.added_at;
            result.OriginalTitle = modelEntity.title_en;
            result.PosterHref = modelEntity.material_data?.poster;
            result.ReleaseDate = modelEntity.material_data?.year;
            result.Tagline = modelEntity.material_data?.tagline == "-" ? string.Empty : modelEntity.material_data?.tagline;
            result.Title = modelEntity.title_ru;
            result.Translations = new List<Translation>();
            result.IframeUrl = videoMaterial.Translations[0].iframe_url;

            foreach (var item in videoMaterial.Translations)
            {
                Translation translation = new Translation
                {
                    studioName = item.translator,
                    updateTime = item.material_data?.updated_at,
                    lastEpisodeTime = null,
                    listOfSeasons = null
                };
                
                if(item is Serial serial)
                {
                    translation.lastEpisodeTime = serial.last_episode_time;
                    translation.listOfSeasons = new List<SeasonInfo>();

                    foreach (var season in serial.season_episodes_count)
                    {
                        SeasonInfo info = new SeasonInfo
                        {
                            episodesCount = season.episodes_count,
                            seasonNumber = season.season_number
                        };

                        translation.listOfSeasons.Add(info);
                    }
                }

                result.Translations.Add(translation);

            }

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
