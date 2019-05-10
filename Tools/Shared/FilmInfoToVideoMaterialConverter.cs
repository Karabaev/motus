namespace Tools.Shared
{
    using System;
    using System.Collections.Generic;
    using InfoAgent;
    using SerialService.DAL;
    using SerialService.DAL.Entities;
    using SerialService.Infrastructure;
    using SerialService.Infrastructure.Exceptions;
    using SerialService.Infrastructure.Helpers;
    using SerialService.Infrastructure.Managers;

    public static class FilmInfoToVideoMaterialConverter
    {
        public static VideoMaterial Convert(FilmInfo info, string authorMail)
        {
            VideoMaterialManager manager = new VideoMaterialManager();

            bool watchForUpdates = info.IsSerial.HasValue && info.IsSerial.Value ? true : false;
            List<SerialSeasonInitializer> seasonInitializerList = new List<SerialSeasonInitializer>();

            foreach (var translation in info.Translations)
            {
                if (translation.listOfSeasons != null)
                {
                    foreach (var season in translation.listOfSeasons)
                    {
                        SerialSeasonInitializer seasonInitializer = new SerialSeasonInitializer(
                                                                season.seasonNumber ?? 0,
                                                                season.episodesCount, translation.lastEpisodeTime,
                                                                translation.studioName);
                        seasonInitializerList.Add(seasonInitializer);
                    }
                }
                else
                {
                    SerialSeasonInitializer seasonInitializer = new SerialSeasonInitializer(0, 1, 
                                                                translation.lastEpisodeTime, translation.studioName);
                    seasonInitializerList.Add(seasonInitializer);
                }
            }

            VideoMaterialInitializer initializer = new VideoMaterialInitializer(info.Title, 
                info.OriginalTitle,
                info.Description,
                info.Tagline,
                info.KinopoiskID,
                info.IDMB ?? 0,
                info.KinopoiskRating,
                info.Duration,
                authorMail,
                info.MoonWalkAddDate,
                info.ReleaseDate,
                info.Countries,
                info.Genres,
                info.PosterHref,
                info.FilmMakers,
                info.Actors,
                seasonInitializerList,
                new List<string>(),
                CheckStatus.Checking,
                watchForUpdates,
                false,
                info.IsSerial ?? false);

            VideoMaterial result = manager.CreateVideoMaterialInstance(initializer);
            return result;
        }
    }
}
