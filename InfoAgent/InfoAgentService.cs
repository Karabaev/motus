namespace InfoAgent
{
    using System.IO;
    using System.Net;
	using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using NLog;
    using Exceptions;

    public class InfoAgentService 
    {
        public InfoAgentService(Logger logger)
        {
			this.logger = logger;
        }

		public InfoAgentService()
		{
			this.logger = LogManager.GetCurrentClassLogger();
		}

		public FilmInfo GetFilmInfo(string id, string token = "a3275d42cea4b2dfb65084eea682885d")
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(token))
                throw new ArgumentNullException("Ошибка вводных");

            try
            {
                var result = this.RequestInfo(id, token);
                return this.SelectDesired(result);
            }
            catch (WebException ex)
            {
				Task.Run(() => this.logger.Error(ex, "С KinoPoiskID - {1}.", id));
                throw new NotFoundInFilmBaseException("Фильм с данным KinoPoiskID не найден", id);
            }
        }

        public string RequestInfo(string id, string token)
        {
			string resp = string.Empty;
            string apiUrl = string.Format("http://moonwalk.cc/api/videos.json?kinopoisk_id={0}&api_token={1}", id, token);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiUrl);
			Task.Run(() => this.logger.Info("Оправка запроса: \"{0}\"", apiUrl));

			using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
			{
				Task.Run(() => this.logger.Info("Запрос отправлен"));
				request.ContentType = "application/json";
				Task.Run(() => this.logger.Info("Получение ответа"));

				using (Stream stream = response.GetResponseStream())
				{
					using (StreamReader reader = new StreamReader(stream))
					{
						resp = reader.ReadToEnd();
					}
				}

				Task.Run(() => this.logger.Info("Ответ получен. Ответ: \"{0}\"", resp));
			}
			
            return resp;
        }

        public FilmInfo SelectDesired(string json)
        {
			Task.Run(() => this.logger.Info("Начало парсинга ответа"));
            FilmInfo filmInfo = new FilmInfo();
            var jArr = (JArray)JsonConvert.DeserializeObject(json);
            var jObj = (JObject)jArr.First;
            filmInfo.Title = jObj["title_ru"].Value<string>();
            filmInfo.OriginalTitle = jObj["title_en"].Value<string>();
            filmInfo.Tagline = jObj["material_data"]["tagline"].Value<string>().Trim("«»".ToCharArray());
            filmInfo.ReleaseDate = jObj["year"].Value<int?>();
            filmInfo.IsSerial = jObj["type"].Value<string>() == "movie" ? false : true;
            filmInfo.KinopoiskID = jObj["kinopoisk_id"].Value<string>();
            filmInfo.IsBlocked = jObj["block"]["block_ru"].Value<bool?>();
            filmInfo.Actors = jObj["material_data"]["actors"].ToObject<List<string>>();
            filmInfo.FilmMakers = jObj["material_data"]["directors"].ToObject<List<string>>();
            filmInfo.Genres = jObj["material_data"]["genres"].ToObject<List<string>>();
            filmInfo.Countries = jObj["material_data"]["countries"].ToObject<List<string>>();
            try
            {
                filmInfo.IDMB = (float?)Math.Round(jObj["material_data"]["imdb_rating"].Value<double>(), 1);
            }
            catch
            {
                filmInfo.IDMB = null;
            }

            try
            {
                filmInfo.KinopoiskRating = (float?)Math.Round(jObj["material_data"]["kinopoisk_rating"].Value<double>(), 1);
            }
            catch
            {
                filmInfo.KinopoiskRating = null;
            }
            
            filmInfo.PosterHref = jObj["material_data"]["poster"].Value<string>();
            filmInfo.Description = jObj["material_data"]["description"].Value<string>();
            filmInfo.IframeUrl = jObj["iframe_url"].Value<string>();

            if (!filmInfo.IsSerial.HasValue)
                throw new NullReferenceException("Значение флага типа не может быть неопределенным");

            if (filmInfo.IsBlocked ?? false)
            {
                throw new IsBlockedException("Фильм заблокирован в регионе RU");
            }

            if ((!(bool)filmInfo.IsSerial) && jObj["duration"].HasValues)
            {
                //Если значение null или ноль, установить null;
                filmInfo.Duration = (jObj["duration"]["seconds"].Value<int?>() ?? 0) != 0
                ? jObj["duration"]["seconds"].Value<int?>()/60 : null;
                filmInfo.MoonWalkAddDate = jObj["added_at"].Value<DateTime>();
            }

            filmInfo.Translations = this.GetTranslations(jArr, (bool)filmInfo.IsSerial);
			Task.Run(() => this.logger.Info("Окончание парсинга ответа. Результат: \"{0}\"", JsonConvert.SerializeObject(filmInfo)));
			return filmInfo;
        }

        private List<Translation> GetTranslations(JArray jArr, bool isSerial)
        {
			Task.Run(() => this.logger.Info("Начало парсинга переводов"));
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
				catch(ArgumentNullException ex)
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
				catch(NullReferenceException ex)
				{
					this.logger.Info("У фильма сезонов нет");
				}

				result.Add(translation);
            }

			Task.Run(() => this.logger.Info("Окончание парсинга переводов. Результат: \"{0}\"", JsonConvert.SerializeObject(result)));
			return result;
        }

		private readonly Logger logger;
	}
}
