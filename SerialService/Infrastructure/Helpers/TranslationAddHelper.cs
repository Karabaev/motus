namespace SerialService.Infrastructure.Helpers
{
    using InfoAgent;
    using DAL.Entities;
    using Services.Interfaces;
    using System.Collections.Generic;

    public class TranslationAddHelper
    {
		public void SaveTranslations(FilmInfo data, VideoMaterial videoMaterial, ITranslationService translationService)
		{
			//List<DAL.Entities.Translation> result = new List<DAL.Entities.Translation>();

			foreach (var item in data.Translations)
			{
				DAL.Entities.Translation translation = translationService.GetByMainStringProperty(item.studioName);

				if (translation == null)
				{
					if (string.IsNullOrWhiteSpace(item.studioName))
						item.studioName = "Не определено";

					translation = new DAL.Entities.Translation
					{
						Name = item.studioName,
						SerialSeasons = new List<SerialSeason>(),
					};
				}

				if ((bool)data.IsSerial)
				{
					foreach (var seasonItem in item.listOfSeasons)
					{
						SerialSeason season = new SerialSeason
						{
							EpisodesCount = seasonItem.episodesCount,
							SeasonNumber = (int)seasonItem.seasonNumber,
							VideoMaterial = videoMaterial,
							Translation = translation,
							IsArchived = false,
						};
						translation.SerialSeasons.Add(season);
						videoMaterial.SerialSeasons.Add(season);
					}
				}
				else
				{
					SerialSeason season = new SerialSeason
					{
						EpisodesCount = 1,
						SeasonNumber = 1,
						VideoMaterial = videoMaterial,
						Translation = translation,
						IsArchived = false,
					};
					translation.SerialSeasons.Add(season);
					videoMaterial.SerialSeasons.Add(season);
				}
			}
		}
    }
}