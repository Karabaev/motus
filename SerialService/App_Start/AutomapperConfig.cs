namespace SerialService.App_Start
{
    using System;
    using AutoMapper;
    using DAL.Entities;
    using ViewModels;
    using System.Linq;
    using Models;
    using ViewModels.RedactorTools;
    using ViewModels.Account;

    public class AutoMapperConfig
    {
        private AutoMapperConfig()
        {
        }

        public static void AutoMapperInit()
        {
            Action<IMapperConfigurationExpression> config = cfg => cfg.CreateMap<VideoMaterial, VideoMaterialListViewModel>()
            .ForMember(lvm => lvm.ID, opt => opt.MapFrom(vm => vm.ID))
            .ForMember(lvm => lvm.Title, opt => opt.MapFrom(vm => vm.Title))
            .ForMember(lvm => lvm.Text, opt => opt.MapFrom(vm => vm.Text))
            .ForMember(lvm => lvm.PositiveMarkCount, opt => opt.MapFrom(vm => vm.PositiveMarkCount))
            .ForMember(lvm => lvm.NegativeMarkCount, opt => opt.MapFrom(vm => vm.NegativeMarkCount))
            .ForMember(lvm => lvm.PosterURL, opt => opt.MapFrom(vm => vm.Pictures.FirstOrDefault(p => p.IsPoster).URL))
            .ForMember(lvm => lvm.GenreTitles, opt => opt.MapFrom(vm => vm.Genres.Select(g => g.Name)))
            .ForMember(lvm => lvm.CountryNames, opt => opt.MapFrom(vm => vm.Countries.Select(c => c.Name)))
            .ForMember(lvm => lvm.KinopoiskRating, opt => opt.MapFrom(vm => vm.KinopoiskRating))
            .ForMember(lvm => lvm.Imdb, opt => opt.MapFrom(vm => vm.IDMB))
            .ForMember(lvm => lvm.Duration, opt => opt.MapFrom(vm => vm.Duration));

			config += cfg => cfg.CreateMap<VideoMaterial, VideoMaterialDetailsViewModel>()
			.ForMember(dvm => dvm.ID, opt => opt.MapFrom(vm => vm.ID))
			.ForMember(dvm => dvm.Title, opt => opt.MapFrom(vm => vm.Title))
			.ForMember(dvm => dvm.OriginalTitle, opt => opt.MapFrom(vm => vm.OriginalTitle))
			.ForMember(dvm => dvm.Text, opt => opt.MapFrom(vm => vm.Text))
			.ForMember(dvm => dvm.Duration, opt => opt.MapFrom(vm => vm.Duration))
			.ForMember(dvm => dvm.GenreTitles, opt => opt.MapFrom(vm => vm.Genres.Select(g => g.Name)))
			.ForMember(dvm => dvm.CountryNames, opt => opt.MapFrom(vm => vm.Countries.Select(c => c.Name)))
			.ForMember(dvm => dvm.AuthorName, opt => opt.MapFrom(vm => vm.Author.UserName))
			.ForMember(dvm => dvm.UpdateDateTime, opt => opt.MapFrom(vm => vm.UpdateDateTime))
			.ForMember(dvm => dvm.ReleaseDate, opt => opt.MapFrom(vm => vm.ReleaseDate))
			.ForMember(dvm => dvm.KinopoiskID, opt => opt.MapFrom(vm => vm.KinopoiskID))
			.ForMember(dvm => dvm.TranslationTitles, opt => opt.MapFrom(vm => vm.SerialSeasons.GroupBy(ss => ss.Translation.Name).Select(ss => ss.Key).Where(tn => !string.IsNullOrEmpty(tn))))
			.ForMember(dvm => dvm.PictureURLs, opt => opt.MapFrom(vm => vm.Pictures.Where(p => !p.IsPoster).Select(c => c.URL)))
			.ForMember(dvm => dvm.FilmMakerNames, opt => opt.MapFrom(vm => vm.FilmMakers.Select(c => c.FullName)))
			.ForMember(dvm => dvm.ActorNames, opt => opt.MapFrom(vm => vm.Actors.Select(c => c.FullName)))
			.ForMember(dvm => dvm.ThemeNames, opt => opt.MapFrom(vm => vm.Themes.Select(c => c.Name)))
			.ForMember(dvm => dvm.PosterURL, opt => opt.MapFrom(vm => vm.Pictures.FirstOrDefault(p => p.IsPoster).URL))
			.ForMember(dvm => dvm.KinopoiskRating, opt => opt.MapFrom(vm => vm.KinopoiskRating))
			.ForMember(dvm => dvm.Imdb, opt => opt.MapFrom(vm => vm.IDMB))
			.ForMember(dvm => dvm.SerialSeasonsCount, opt => opt.MapFrom(vm => vm.SerialSeasons.Count()))
			.ForMember(dvm => dvm.LastEpisodeTime, opt => opt.MapFrom(vm => vm.SerialSeasons.Max(ss => ss.LastEpisodeTime)))
			.ForMember(dvm => dvm.LastEpisodeTranslator, opt => opt.MapFrom(vm => vm.SerialSeasons.FirstOrDefault(_vm => _vm.LastEpisodeTime == vm.SerialSeasons.Max(ss => ss.LastEpisodeTime)).Translation.Name));

            config += cfg => cfg.CreateMap<RegisterViewModel, ApplicationUser>()
            .ForMember(rvm => rvm.UserName, opt => opt.MapFrom(vm => vm.UserName))
            .ForMember(rvm => rvm.Email, opt => opt.MapFrom(vm => vm.Email))
            .ForMember(rvm => rvm.Parole, opt => opt.MapFrom(vm => vm.Parole));

            config += cfg => cfg.CreateMap<ApplicationUser, PersonalAccountViewModel>()
            .ForMember(rvm => rvm.CurrentUserName, opt => opt.MapFrom(vm => vm.UserName))
            .ForMember(rvm => rvm.CurrentEmail, opt => opt.MapFrom(vm => vm.Email))
            .ForMember(rvm => rvm.CurrentAvatarURL, opt => opt.MapFrom(vm => vm.AvatarURL))
            .ForMember(vm => vm.LikedVideoMaterials, opt => opt.MapFrom(vm => vm.VideoMarks.Where(mark => mark.Value).Select(mark => mark.VideoMaterial)))
            .ForMember(vm => vm.DislikedVideoMaterials, opt => opt.MapFrom(vm => vm.VideoMarks.Where(mark => !mark.Value).Select(mark => mark.VideoMaterial)))
            .ForMember(vm => vm.SubscribedVideoMaterials, opt => opt.MapFrom(vm => vm.SubscribedVideoMaterials))
            .ForMember(vm => vm.IsHaveCurrentPassword, opt => opt.MapFrom(vm => !string.IsNullOrWhiteSpace(vm.PasswordHash)));

            config += cfg => cfg.CreateMap<VideoMaterial, VideoMaterialPersonalAccountViewModel>()
            .ForMember(vm => vm.ID, opt => opt.MapFrom(vm => vm.ID))
            .ForMember(vm => vm.Title, opt => opt.MapFrom(vm => vm.Title));

            config += cfg => cfg.CreateMap<VideoMaterial, AddFilmViewModel>()
            .ForMember(fvm => fvm.Actors, opt => opt.MapFrom(vm => vm.Actors.Select(p => p.FullName).ToList()))
            .ForMember(fvm => fvm.FilmMakers, opt => opt.MapFrom(vm => vm.FilmMakers.Select(p => p.FullName).ToList()))
            .ForMember(fvm => fvm.Countries, opt => opt.MapFrom(vm => vm.Countries.Select(c => c.Name).ToList()))
            .ForMember(fvm => fvm.Genres, opt => opt.MapFrom(vm => vm.Genres.Select(g => g.Name).ToList()))
            .ForMember(fvm => fvm.Pictures, opt => opt.MapFrom(vm => vm.Pictures.Where(p=>!(p.IsPoster)).Select(p => p.URL).ToList()))
            .ForMember(fvm => fvm.Themes, opt => opt.MapFrom(vm => vm.Themes.Select(t => t.Name).ToList()))
            .ForMember(fvm => fvm.PosterHref, opt => opt.MapFrom(vm => vm.Pictures.FirstOrDefault(p => p.IsPoster).URL));

            config += cfg => cfg.CreateMap<VideoMaterial, ElasticVideoMaterial>()
            .ForMember(dvm => dvm.ID, opt => opt.MapFrom(vm => vm.ID))
            .ForMember(dvm => dvm.Title, opt => opt.MapFrom(vm => vm.Title))
            .ForMember(dvm => dvm.OriginalTitle, opt => opt.MapFrom(vm => vm.OriginalTitle))
            .ForMember(dvm => dvm.Duration, opt => opt.MapFrom(vm => vm.Duration))
            .ForMember(dvm => dvm.GenreTitles, opt => opt.MapFrom(vm => vm.Genres.Select(g => g.Name)))
            .ForMember(dvm => dvm.CountryNames, opt => opt.MapFrom(vm => vm.Countries.Select(c => c.Name)))
            .ForMember(dvm => dvm.ReleaseDate, opt => opt.MapFrom(vm => vm.ReleaseDate))
			.ForMember(dvm => dvm.TranslationTitles, opt => opt.MapFrom(vm => vm.SerialSeasons.Select(ss => ss.Translation.Name)))
			.ForMember(dvm => dvm.FilmMakerNames, opt => opt.MapFrom(vm => vm.FilmMakers.Select(c => c.FullName)))
            .ForMember(dvm => dvm.ActorNames, opt => opt.MapFrom(vm => vm.Actors.Select(c => c.FullName)))
            .ForMember(dvm => dvm.ThemeNames, opt => opt.MapFrom(vm => vm.Themes.Select(c => c.Name)))
            .ForMember(dvm => dvm.KinopoiskRating, opt => opt.MapFrom(vm => vm.KinopoiskRating))
            .ForMember(dvm => dvm.Imdb, opt => opt.MapFrom(vm => vm.IDMB))
            .ForMember(dvm => dvm.Description, opt => opt.MapFrom(vm => vm.Text.Substring(0, Math.Min(vm.Text.Length, 100))))
            .ForMember(dvm => dvm.PosterURL, opt => opt.MapFrom(vm => vm.Pictures.FirstOrDefault(p => p.IsPoster).URL));

            Mapper.Initialize(config);
        }
    }
}