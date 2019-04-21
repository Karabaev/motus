namespace Updater.Shared
{
    using System;
    using System.Collections.Generic;
    using InfoAgent;
    using SerialService.DAL;
    using SerialService.DAL.Entities;
    using SerialService.Infrastructure;
    using SerialService.Infrastructure.Exceptions;
    using SerialService.Infrastructure.Helpers;

    public static class FilmInfoToVideoMaterialConverter
    {
        public static VideoMaterial Convert(FilmInfo info, string authorMail, IAppUnitOfWork unitOfWork)
        {
            VideoMaterial result = null;
            ApplicationUser user = unitOfWork.Users.GetByMainStringProperty(authorMail);

            if (user == null)
                throw new EntryNotFoundException(string.Format("Пользователь с email {0} не найден", authorMail));

            result = new VideoMaterial
            {
                Duration = info.Duration,
                IDMB = info.IDMB.HasValue ? info.IDMB.Value : 0,
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
                Actors = unitOfWork.Persons.AutoSave(info.Actors),
                FilmMakers = unitOfWork.Persons.AutoSave(info.FilmMakers),
                Genres = unitOfWork.Genres.AutoSave(info.Genres),
                Countries = unitOfWork.Countries.AutoSave(info.Countries),
                Author = user,
                AuthorID = user.Id,
                CheckStatus = CheckStatus.Checking,
                WatchForUpdates = info.IsSerial.HasValue && info.IsSerial.Value ? true : false,
                SerialSeasons = new List<SerialSeason>(),
                AddDateTime = DateTime.Now,
                MoonWalkAddDate = info.MoonWalkAddDate,
            };
            TranslationAddHelper translationHelper = new TranslationAddHelper();
            translationHelper.SaveTranslations(info, result, unitOfWork.Translations);
            return result;
        }
    }
}
