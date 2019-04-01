namespace SerialService.DAL
{
    using Context;
    using Services;
    using Services.Interfaces;
    using System;

    public class AppUnitOfWork : IAppUnitOfWork
    {
        private IVideoMaterialService videoMaterialService;
        private IPersonService personService;
        private ICountryService countryService;
        private IPictureService pictureService;
        private IGenreService genreService;
        private IThemeService themeService;
        private ITranslationService translationService;
		private ISerialSeasonService serialSeasonService;

        private ApplicationDbContext DB = new ApplicationDbContext();
        
        IVideoMaterialService IAppUnitOfWork.VideoMaterials
        {
            get
            {
                this.videoMaterialService = new VideoMaterialService(DB);
                return this.videoMaterialService;
            }
        }

        IPersonService IAppUnitOfWork.Persons
        {
            get
            {
                this.personService = new PersonService(DB);
                return this.personService;
            }
        }

        ICountryService IAppUnitOfWork.Countries
        {
            get
            {
                this.countryService = new CountryService(DB);
                return this.countryService;
            }
        }

        IPictureService IAppUnitOfWork.Pictures
        {
            get
            {
                this.pictureService = new PictureService(DB);
                return this.pictureService;
            }
        }

        IGenreService IAppUnitOfWork.Genres
        {
            get
            {
                this.genreService = new GenreService(DB);
                return this.genreService;
            }
        }

        IThemeService IAppUnitOfWork.Themes
        {
            get
            {
                this.themeService = new ThemeService(DB);
                return this.themeService;
            }
        }

        ITranslationService IAppUnitOfWork.Translations
        {
            get
            {
                this.translationService = new TranslationService(DB);
                return this.translationService;
            }
        }

        public IUserService Users
        {
            get
            {
                return new UserService(DB);
            }
        }

        public IVideoMarkService VideoMarks
        {
            get
            {
                return new VideoMarkService(DB);
            }
        }

        public IRoleService Roles
        {
            get
            {
                return new RoleService(DB);
			}
		}

		public ISerialSeasonService SerialSeasons
		{
			get
			{
				if (this.serialSeasonService == null)
					this.serialSeasonService = new SerialSeasonService(this.DB);

				return this.serialSeasonService;
			}
		}

		//далее копипаста, не выебываемся
		private bool disposed = false;

        public virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    DB.Dispose();
                }
                this.disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
	}
}