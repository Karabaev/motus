using Shared.Mail;

namespace Updater
{
	using System;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using System.Collections.Generic;
	using SerialService.DAL.Entities;
	using SerialService.Infrastructure.Exceptions;
	using SerialService.Infrastructure.Core.Extensions;
	using SerialService.DAL;
	using InfoAgent;
	using Exceptions;
	using NLog;

	public enum MessageTypes
	{
		ADDED_NEW_EPISODE,
		ADDED_NEW_SEASON,
		NONE
	}


	/// <summary>
	/// Класс апдейтера видеоматериалов.
	/// </summary>
	public class VideoMaterialUpdater
	{
		/// <summary>
		/// Инициализирует объект в памяти.
		/// </summary>
		/// <param name="unitOfWork">Объект, агрегирующий сервисы для работы с данными.</param>
		/// /// <param name="logger">Объект логгера.</param>
		public VideoMaterialUpdater(IAppUnitOfWork unitOfWork, MailClient mailClient, ConfigManager configManager, Logger logger)
		{
			this.mailClient = mailClient;
			this.configManager = configManager;
			this.unitOfWork = unitOfWork;
			this.logger = logger;
			this.infoAgent = new InfoAgentService(this.logger);
		}

		/// <summary>
		/// Проверить обновления всех видеоматериалов, помеченных флагом проверки обновлений, обновить в случае необходимости и оповестить подписчиков.
		/// </summary>
		public int CheckUpdatesOfAllVideoMaterials()
		{
			var videoMaterials = this.unitOfWork.VideoMaterials.GetWithCondition(vm => vm.WatchForUpdates == true);
			Task.Run(() => this.logger.Info("Количество видеоматериалов для проверки: {0}", videoMaterials.Count()));

			foreach (var item in videoMaterials)
			{
				Task.Run(() => this.logger.Info("Начало проверки обновлений видеоматериала ID: {0} Title: {1}", item.ID, item.Title));
				this.CheckUpdate(item);
				Task.Run(() => this.logger.Info("Окончание проверки обновлений видеоматериала ID: {0} Title: {1}", item.ID, item.Title));
			}

			return videoMaterials.Count;
		}

		/// <summary>
		/// Проверить обновления видеоматериала, обновить в случае необходимости и оповестить подписчиков.
		/// </summary>
		/// <param name="videoMaterial">Объект видеоматериала.</param>
		public void CheckUpdate(VideoMaterial videoMaterial)
		{
			Task.Run(() => this.logger.Info("Начало получения и парсинга информации о видеоматериале"));

			FilmInfo filmInfo = this.infoAgent.GetFilmInfo(videoMaterial.KinopoiskID);
            if (filmInfo.IsBlocked??false)
            {
                this.unitOfWork.VideoMaterials.Update(videoMaterial.ID, videoMaterial);
                return;
            }

			Task.Run(() => this.logger.Info("Данные о видеоматериале получены"));
			List<SerialSeason> serialSeasons = this.GetSeasonsFromInfo(filmInfo);
			serialSeasons.ForEach(ss => { ss.VideoMaterialID = videoMaterial.ID; ss.VideoMaterial = videoMaterial; });
			MessageTypes messageType = MessageTypes.NONE;
			int lastEpisodeNumber = 0;
			int lastSeasonNumber = 0;
			string lastTranslation = string.Empty;

			if (serialSeasons.Count > videoMaterial.SerialSeasons.Count)
			{
				Task.Run(() => this.logger.Info("В ответе на запрос сезонов/переводов больше, чем в базе данных"));
				bool isAddNewSerialSeason = false;

				foreach (var item in serialSeasons)
				{
					item.VideoMaterialID = videoMaterial.ID;
					item.VideoMaterial = videoMaterial;

					try
					{
						if (!this.unitOfWork.SerialSeasons.GetAll().Contains(item, new SerialSeasonIsNewSeasonComparer()))
							isAddNewSerialSeason = true;	
						
						if (this.unitOfWork.SerialSeasons.Create(item))
						{
							if(isAddNewSerialSeason)
							{
								messageType = MessageTypes.ADDED_NEW_SEASON;
								lastSeasonNumber = item.SeasonNumber;
								lastTranslation = item.Translation?.Name;
								Task.Run(() => this.logger.Info("Добавлен новый сезон"));
							}
							else
							{
								Task.Run(() => this.logger.Info("Добавлен новый перевод"));
							}

							this.logger.Info("В базу данных добавлена новая запись о сезоне. SeasonNumber: {0} EpisodesCount: {1} LastEpisodeTime: {2} Translation.Name: {3}",
								item.SeasonNumber, item.EpisodesCount, item.LastEpisodeTime, item.Translation.Name);
						}
					}
					catch (EntryAlreadyExistsException)
					{
						continue;
					}

				}
			}
			else
			{
				Task.Run(() => this.logger.Info("Количество сезонов в ответе на запрос совпадает с количеством в базе данных"));
			}

			Task.Run(() => this.logger.Info("Начало проверки сезонов"));

			foreach (var seasonOld in videoMaterial.SerialSeasons)
			{
				foreach (var seasonNew in serialSeasons)
				{
					try
					{
						if (!this.CompareSeasons(seasonOld, seasonNew))
						{
							Task.Run(() => this.logger.Info("Для сезона ID: {0} найдено обновление", seasonOld.ID));

							if (!this.unitOfWork.SerialSeasons.Update(seasonOld.ID, seasonNew))
							{
								Task.Run(() => this.logger.Error("Не удалось обновить сезон. ID: {0}", seasonOld.ID));
								throw new Exception("Не удалось обновить сезон");
							}
							else
							{
								Task.Run(() => this.logger.Info("Сезон ID: {0} обновлен успешно", seasonOld.ID));

								if (!this.unitOfWork.VideoMaterials.Update(videoMaterial.ID, videoMaterial))
								{
									Task.Run(() => this.logger.Error("Не удалось обновить видеоматериал"));
									throw new Exception("Не удалось обновить видеоматериал");
								}
								else
								{
									Task.Run(() => this.logger.Info("Видеоматериал обновлен успешно"));
									lastEpisodeNumber = (int)seasonOld.EpisodesCount;
									lastSeasonNumber = seasonOld.SeasonNumber;
									lastTranslation = seasonOld.Translation?.Name;
									messageType = MessageTypes.ADDED_NEW_EPISODE;
								}

							}
						}
					}
					catch (NotMatchedSeasonsException)
					{
						continue;
					}
				}
			}

			Task.Run(() => this.logger.Info("Окончание проверки сезонов"));

			if (messageType != MessageTypes.NONE)
			{
				Task.Run(() => this.logger.Info("Начало отправки сообщений подписчикам messageType = {0}", messageType));

				switch (messageType)
				{
					case MessageTypes.ADDED_NEW_EPISODE:
						this.SendMessageAddedNewEpisodeToSubscribers(videoMaterial, lastEpisodeNumber, lastSeasonNumber, lastTranslation);
						break;
					case MessageTypes.ADDED_NEW_SEASON:
						this.SendMessageAddedNewSeasonToSubscribers(videoMaterial, lastSeasonNumber, lastTranslation);
						break;
					default:
						break;
				}

				Task.Run(() => this.logger.Info("Окончание отправки сообщений подписчикам"));
			}	
		}

		private void SendMessageAddedNewEpisodeToSubscribers(VideoMaterial videoMaterial, int lastEpNum, int lastSesNum, string lastTrans)
		{
			if (videoMaterial == null || lastEpNum <= 0 || lastSesNum <= 0 || string.IsNullOrWhiteSpace(lastTrans))
				throw new ArgumentOutOfRangeException("Один из параметров имеет неверное значение");

			string messageCaption = (string)this.configManager.Config[ConfigKeys.MAIL_MESSAGE_CAPTION];
			string messageBodyTemplate = (string)this.configManager.Config[ConfigKeys.MAIL_MESSAGE_BODY_ADDED_NEW_EPISODE];
			
			foreach (var user in videoMaterial.SubscribedUsers)
			{
				string messageBody = string.Format(messageBodyTemplate,user.UserName, videoMaterial.Title, lastEpNum, lastSesNum, lastTrans);
				Task.Run(() => this.logger.Info("Отправка сообщения {0} пользователю ID: {1} UserName: {2}", messageBody, user.Id, user.UserName));
				this.mailClient.SendMessage(user.Email, messageCaption, messageBody, true);
				Task.Run(() => this.logger.Info("Окончание отправки сообщения пользователю ID: {0} UserName: {1}.", user.Id, user.UserName));
			}
		}

		private void SendMessageAddedNewSeasonToSubscribers(VideoMaterial videoMaterial, int lastSesNum, string lastTrans)
		{
			if (videoMaterial == null || lastSesNum <= 0 || string.IsNullOrWhiteSpace(lastTrans))
				throw new ArgumentOutOfRangeException("Один из параметров имеет неверное значение");

			string messageCaption = (string)this.configManager.Config[ConfigKeys.MAIL_MESSAGE_CAPTION];
			string messageBodyTemplate = (string)this.configManager.Config[ConfigKeys.MAIL_MESSAGE_BODY_ADDED_NEW_SEASON];

			foreach (var user in videoMaterial.SubscribedUsers)
			{
				string messageBody = string.Format(messageBodyTemplate,user.UserName, videoMaterial.Title, lastSesNum, lastTrans);
				Task.Run(() => this.logger.Info("Отправка сообщения {0} пользователю ID: {1} UserName: {2}", messageBody, user.Id, user.UserName));
				this.mailClient.SendMessage(user.Email, messageCaption, messageBody, true);
				Task.Run(() => this.logger.Info("Окончание отправки сообщения пользователю ID: {0} UserName: {1}.", user.Id, user.UserName));
			}
		}

		/// <summary>
		/// Получить информацию по переводам и сезонам.
		/// </summary>
		/// <param name="filmInfo"></param>
		private List<SerialSeason> GetSeasonsFromInfo(FilmInfo filmInfo)
		{
			List<SerialSeason> result = new List<SerialSeason>();

			foreach (var trans in filmInfo.Translations)
			{
				SerialService.DAL.Entities.Translation translation = null;

				try
				{
					translation = this.unitOfWork.Translations.GetByMainStringProperty(trans.studioName);
				}
				catch (NullReferenceException ex)
				{
					Task.Run(() => this.logger.Error(ex, "Произошла ошибка при поиске перевода с названием {0}", trans.studioName));
				}

				if (translation == null)
				{
					translation = new SerialService.DAL.Entities.Translation { Name = trans.studioName };
					Task.Run(() => this.logger.Info("Будет создан новый объект перевода Name: {0}", translation.Name));
				}
				else
				{
					Task.Run(() => this.logger.Info("Перевод Name: {0} найден в базе данных", translation.Name));
				}

				foreach (var season in trans.listOfSeasons)
				{
					result.Add(new SerialSeason
					{
						EpisodesCount = season.episodesCount,
						LastEpisodeTime = trans.lastEpisodeTime,
						SeasonNumber = (int)season.seasonNumber,
						TranslationID = translation.ID,
						Translation = translation
					});
				}
			}

			return result;
		}

		/// <summary>
		/// Сравнить сезоны. Если сезоны не соответствуют друг другу, выкидывается исключение NotMatchedSeasonsException.
		/// </summary>
		/// <param name="a">Объект сезона a.</param>
		/// <param name="b">Объект сезона b.</param>
		/// <returns>false в случае, если количество серий или дата последней серии не совпадают, иначе true.</returns>
		private bool CompareSeasons(SerialSeason a, SerialSeason b)
		{
			if (!a.Alike(b))
				throw new NotMatchedSeasonsException("Попытка сравнить сезоны разных фильмов или с разными номерами или с разными переводами.");

			if (a.EpisodesCount != b.EpisodesCount || a.LastEpisodeTime != b.LastEpisodeTime)
			{
				this.logger.Info("Сезон a SeasonNumber: {0} EpisodesCount: {1} LastEpisodeTime: {2} Translation.Name: {3} отличается от сезона b SeasonNumber: {4} EpisodesCount: {5} LastEpisodeTime: {6} Translation.Name: {7}",
					a.SeasonNumber, a.EpisodesCount, a.LastEpisodeTime, a.Translation.Name, b.SeasonNumber, b.EpisodesCount, b.LastEpisodeTime, b.Translation.Name);
				return false;
			}

			return true;
		}

		/// <summary>
		/// Вернет true, если сезон y будет с тем же номером, но с другим переводом.
		/// </summary>
		private class SerialSeasonIsNewTranslationComparer : IEqualityComparer<SerialSeason>
		{
			public bool Equals(SerialSeason x, SerialSeason y)
			{
				return	x.VideoMaterialID == y.VideoMaterialID && 
						x.SeasonNumber == y.SeasonNumber && 
						x.TranslationID != y.TranslationID;
			}

			public int GetHashCode(SerialSeason obj)
			{
				return obj.GetHashCode();
			}
		}

		/// <summary>
		/// Вернет true, если у фильма найдется сезон с таким же номером.
		/// </summary>
		private class SerialSeasonIsNewSeasonComparer : IEqualityComparer<SerialSeason>
		{
			public bool Equals(SerialSeason x, SerialSeason y)
			{
				return x.VideoMaterialID == y.VideoMaterialID &&
						x.SeasonNumber == y.SeasonNumber;
			}

			public int GetHashCode(SerialSeason obj)
			{
				return obj.GetHashCode();
			}
		}



		private readonly MailClient mailClient;
		private readonly InfoAgentService infoAgent;
		private readonly IAppUnitOfWork unitOfWork;
		private readonly Logger logger;
		private readonly ConfigManager configManager;
	}
}
