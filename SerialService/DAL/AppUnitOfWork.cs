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
        private IUserParamService userParamService;

        private static IAppUnitOfWork instance;

        private IDbContext db;

        public AppUnitOfWork(IDbContext context)
        {
            this.db = context;
        }

        public ICommentService Comments
        {
            get
            {
                if (this.commentService == null)
                    this.commentService = new CommentService(db);

                return this.commentService;
            }
        }

        public ICommentMarkService CommentMarks
        {
            get
            {
                if (this.commentMarkService == null)
                    this.commentMarkService = new CommentMarkService(db);

                return this.commentMarkService;
            }
        }

        public IVideoMaterialService VideoMaterials
        {
            get
            {
                if(this.videoMaterialService == null)
                    this.videoMaterialService = new VideoMaterialService(db);

                return this.videoMaterialService;
            }
        }

        public IPersonService Persons
        {
            get
            {
                if (this.personService == null)
                    this.personService = new PersonService(db);

                return this.personService;
            }
        }

        public ICountryService Countries
        {
            get
            {
                if (this.countryService == null)
                    this.countryService = new CountryService(db);

                return this.countryService;
            }
        }

        public IPictureService Pictures
        {
            get
            {
                if (this.pictureService == null)
                    this.pictureService = new PictureService(db);

                return this.pictureService;
            }
        }

        public IGenreService Genres
        {
            get
            {
                if (this.genreService == null)
                    this.genreService = new GenreService(db);

                return this.genreService;
            }
        }

        public IThemeService Themes
        {
            get
            {
                if (this.themeService == null)
                    this.themeService = new ThemeService(db);

                return this.themeService;
            }
        }

        public ITranslationService Translations
        {
            get
            {
                if (this.translationService == null)
                    this.translationService = new TranslationService(db);

                return this.translationService;
            }
        }

        public IUserService Users
        {
            get
            {
                if (this.userService == null)
                    this.userService = new UserService(db);

                return this.userService;
            }
        }

        public IVideoMarkService VideoMarks
        {
            get
            {
                if (this.videoMarkService == null)
                    this.videoMarkService = new VideoMarkService(this.db);

                return new VideoMarkService(db);
            }
        }

        public IRoleService Roles
        {
            get
            {
                if(this.roleService == null)
                    this.roleService = new RoleService(db);

                return this.roleService;
            }
		}

		public ISerialSeasonService SerialSeasons
		{
			get
			{
				if (this.serialSeasonService == null)
					this.serialSeasonService = new SerialSeasonService(this.db);

				return this.serialSeasonService;
			}
		}

        public IVideoMaterialViewsByUsersService VideoMaterialViewsByUsers
        {
            get
            {
                if (this.videoMaterialViewsByUsersService == null)
                    this.videoMaterialViewsByUsersService = new VideoMaterialViewsByUsersService(this.db);

                return this.videoMaterialViewsByUsersService;
            }
        }

        public IUserParamService UserParams
        {
            get
            {
                if (this.userParamService == null)
                    this.userParamService = new UserParamService(this.db);

                return this.userParamService;
            }
        }

        public static IAppUnitOfWork GetInstance(IDbContext context)
        {
            if (AppUnitOfWork.instance == null)
                AppUnitOfWork.instance = new AppUnitOfWork(context);

            return AppUnitOfWork.instance;
        }

		private bool disposed = false;

        public virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    db.Dispose();
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