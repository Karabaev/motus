namespace SerialService.DAL
{
    using Context;
    using Services;
    using Services.Interfaces;
    using System;

    public class AppUnitOfWork : IAppUnitOfWork
    {
        private ICommentService commentService;
        private ICommentMarkService commentMarkService;
        private IVideoMaterialService videoMaterialService;
        private IPersonService personService;
        private ICountryService countryService;
        private IPictureService pictureService;
        private IGenreService genreService;
        private IThemeService themeService;
        private ITranslationService translationService;
		private ISerialSeasonService serialSeasonService;
        private IUserService userService;
        private IVideoMarkService videoMarkService;
        private IRoleService roleService;
        private IVideoMaterialViewsByUsersService videoMaterialViewsByUsersService;

        private static IAppUnitOfWork instance;

        private ApplicationDbContext DB = new ApplicationDbContext();

        public ICommentService Comments
        {
            get
            {
                if (this.commentService == null)
                    this.commentService = new CommentService(DB);

                return this.commentService;
            }
        }

        public ICommentMarkService CommentMarks
        {
            get
            {
                if (this.commentMarkService == null)
                    this.commentMarkService = new CommentMarkService(DB);

                return this.commentMarkService;
            }
        }

        public IVideoMaterialService VideoMaterials
        {
            get
            {
                if(this.videoMaterialService == null)
                    this.videoMaterialService = new VideoMaterialService(DB);

                return this.videoMaterialService;
            }
        }

        public IPersonService Persons
        {
            get
            {
                if (this.personService == null)
                    this.personService = new PersonService(DB);

                return this.personService;
            }
        }

        public ICountryService Countries
        {
            get
            {
                if (this.countryService == null)
                    this.countryService = new CountryService(DB);

                return this.countryService;
            }
        }

        public IPictureService Pictures
        {
            get
            {
                if (this.pictureService == null)
                    this.pictureService = new PictureService(DB);

                return this.pictureService;
            }
        }

        public IGenreService Genres
        {
            get
            {
                if (this.genreService == null)
                    this.genreService = new GenreService(DB);

                return this.genreService;
            }
        }

        public IThemeService Themes
        {
            get
            {
                if (this.themeService == null)
                    this.themeService = new ThemeService(DB);

                return this.themeService;
            }
        }

        public ITranslationService Translations
        {
            get
            {
                if (this.translationService == null)
                    this.translationService = new TranslationService(DB);

                return this.translationService;
            }
        }

        public IUserService Users
        {
            get
            {
                if (this.userService == null)
                    this.userService = new UserService(DB);

                return this.userService;
            }
        }

        public IVideoMarkService VideoMarks
        {
            get
            {
                if (this.videoMarkService == null)
                    this.videoMarkService = new VideoMarkService(this.DB);

                return new VideoMarkService(DB);
            }
        }

        public IRoleService Roles
        {
            get
            {
                if(this.roleService == null)
                    this.roleService = new RoleService(DB);

                return this.roleService;
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

        public IVideoMaterialViewsByUsersService VideoMaterialViewsByUsers
        {
            get
            {
                if (this.videoMaterialViewsByUsersService == null)
                    this.videoMaterialViewsByUsersService = new VideoMaterialViewsByUsersService(this.DB);

                return this.videoMaterialViewsByUsersService;
            }
        }

        public static IAppUnitOfWork GetInstance()
        {
            if (AppUnitOfWork.instance == null)
                AppUnitOfWork.instance = new AppUnitOfWork();

            return AppUnitOfWork.instance;
        }

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