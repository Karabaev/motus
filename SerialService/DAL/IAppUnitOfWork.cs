namespace SerialService.DAL
{
    using System;
    using Services.Interfaces;

    public interface IAppUnitOfWork: IDisposable
    {
        IVideoMaterialService VideoMaterials { get; }

        IPersonService Persons { get; }

        ICountryService Countries { get; }

        IPictureService Pictures { get; }

        IGenreService Genres { get; }

        IThemeService Themes { get; }

        ITranslationService Translations { get; }

        IUserService Users { get; }

        IVideoMarkService VideoMarks { get; }

        IRoleService Roles { get; }

		ISerialSeasonService SerialSeasons { get; }
    }
}
