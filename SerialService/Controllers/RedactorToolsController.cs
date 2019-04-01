namespace SerialService.Controllers
{
    using System.Web.Mvc;
    using DAL.Entities;
    using Services.Interfaces;
    using Infrastructure;
    using System;
    using Microsoft.AspNet.Identity;
    using ViewModels.RedactorTools;
    using Infrastructure.Exceptions;
    using DAL;
    using InfoAgent;
    using InfoAgent.Exceptions;
    using System.Collections.Generic;
    using Infrastructure.Helpers;
    using System.Linq;
    using Newtonsoft.Json;
    using AutoMapper;
	using NLog;

	[ExceptionHandler]
	[Authorize]
	public class RedactorToolsController : Controller
    {
        private readonly IPersonService personService;
        private readonly IVideoMaterialService videoMaterialService;
        private readonly ICountryService countryService;
        private readonly IGenreService genreService;
        private readonly IPictureService pictureService;
        private readonly IThemeService themeService;
        private readonly ITranslationService translationService;
        private readonly IAppUnitOfWork unitOfWork;
        private readonly TranslationAddHelper translationHelper;
		private readonly Logger logger;
        FilmInfo filmInfo;

        public RedactorToolsController(IAppUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            this.personService = unitOfWork.Persons;
            this.videoMaterialService = unitOfWork.VideoMaterials;
            this.countryService = unitOfWork.Countries;
            this.genreService = unitOfWork.Genres;
            this.pictureService = unitOfWork.Pictures;
            this.themeService = unitOfWork.Themes;
            this.translationService = unitOfWork.Translations;
            this.translationHelper = new TranslationAddHelper();
			this.logger = LogManager.GetCurrentClassLogger();
        }

        [HttpPost]
		[Authorize(Roles = "Admin")]
		public ActionResult Index()
        {
            return PartialView("Index");
        }

        public ActionResult EditingManager()
        {
            ViewBag.IsModerator = User.IsInRole("Moderator");
            return View();
        }

        [HttpPost]
        public ActionResult EditMaterial(int id, AddFilmViewModel model)
        {
            try
            {
                if (!(ModelState.IsValid))
                {
                    return Json(new { message = "Ошибка. Данные некорректны. Соблюдайте условия валидации." });
                }
                VideoMaterial material = MapToFilmItem(model, videoMaterialService.GetLoaded(id));
                material.CheckStatus = CheckStatus.Checking;
                if (videoMaterialService.EditMaterial(material))
                {
                    return Json(new { message = "Материал успешно сохранен." });
                }
                return Json(new { message = "Ошибка сохранения." });
            }
            catch
            {
                return Json(new { message = "Неизветсня ошибка." });
            }
        }

        public ActionResult FilmEditor(int id)
        {
            VideoMaterial material = videoMaterialService.Get(id);
            ViewBag.Id = id;
            ViewBag.IsModerator = User.IsInRole("Moderator");
            ViewBag.MaterialStatus = (int)material.CheckStatus;
            AddFilmViewModel model = Mapper.Map<VideoMaterial, AddFilmViewModel>(material);
            return View(model);
        }

        [HttpGet]
        public ActionResult GetSortTable(string sortOrder, string searchFilter, string filters)
        {
            IEnumerable<VideoMaterial> allMaterials;
            if (User.IsInRole(Resource.AdminRoleName) || User.IsInRole(Resource.ModeratorRoleName))
            {
                allMaterials = videoMaterialService.GetAll().Where(vm=>!vm.IsArchived);
            }
            else
            {
                allMaterials = videoMaterialService.GetAll().Where(vm => vm.AuthorID == User.Identity.GetUserId()&&!vm.IsArchived);
            }
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "time" ? "time_desc" : "time";
            ViewBag.StatusSortParm = sortOrder == "status" ? "status_desc" : "status";
            if (!(string.IsNullOrEmpty(filters)))
            {
                List<string> filterList = JsonConvert.DeserializeObject<List<string>>(filters);
                var targetMaterials = allMaterials.Where(vm => filters.Contains(vm.CheckStatus.ToString().ToLower()));
                allMaterials = filterList.Count() == 0 ? allMaterials : targetMaterials;
            }
            if (!string.IsNullOrEmpty(searchFilter))
            {
                allMaterials = allMaterials.Where(f => (f.Title??"").ToLower().Contains(searchFilter.ToLower()) || 
                                                       (f.OriginalTitle??"").ToLower().Contains(searchFilter.ToLower()));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    allMaterials = allMaterials.OrderByDescending(s => s.Title);
                    break;
                case "time":
                    allMaterials = allMaterials.OrderBy(s => s.AddDateTime);
                    break;
                case "time_desc":
                    allMaterials = allMaterials.OrderByDescending(s => s.AddDateTime);
                    break;
                case "status":
                    allMaterials = allMaterials.OrderBy(s => s.CheckStatus);
                    break;
                case "status_desc":
                    allMaterials = allMaterials.OrderByDescending(s => s.CheckStatus);
                    break;
                default:
                    allMaterials = allMaterials.OrderBy(s => s.Title);
                    break;
            }
            return PartialView(allMaterials.ToList());
        }
        /// <summary>
        /// Метод для получения результата парсинга KPParser
        /// </summary>
        /// <param name="url">Адрес на Kinopoisk</param>
        /// <returns>JSON с результатами парсинга</returns>
        [HttpPost]
        public ActionResult Parse(string kinopoiskId)
        {
            try
            {
                filmInfo = new InfoAgentService().GetFilmInfo(kinopoiskId);

                return Json(filmInfo);
            }
            catch (NotFoundInFilmBaseException ex)
            {
                return Json(new { error = true, message = string.Format("{0}:{1}", ex.Message,ex.Value)});
            }
            catch
            {
                return Json(new { error = true, message = "Ошибка поиска." });
            }
        }

        /// <summary>
        /// Метод создания нового видеоматериала
        /// </summary>
        [HttpPost]
        public ActionResult CreateNewVideoItem(AddFilmViewModel model)
        {
            try
            {
                if (model != null && ModelState.IsValid)
                {
                    if (filmInfo == null)
                    {
                        filmInfo = new InfoAgentService().GetFilmInfo(model.KinopoiskID.ToString());
                    }
                    
                    VideoMaterial newMaterial = MapToFilmItem(model, new VideoMaterial());
                    newMaterial.AddDateTime = DateTime.Now;
					newMaterial.UpdateDateTime = newMaterial.AddDateTime;
					newMaterial.AuthorID = User.Identity.GetUserId();
                    newMaterial.CheckStatus = CheckStatus.Checking;
					newMaterial.WatchForUpdates = true;
					newMaterial.SerialSeasons = new List<SerialSeason>();
                    translationHelper.SaveTranslations(filmInfo, newMaterial, translationService);

                    if (newMaterial.Text == null)
                        newMaterial.Text = filmInfo.Description;

                    if (this.videoMaterialService.Create(newMaterial))
                        return Json(new { message = "Успешно. Материал ожидает проверки." });
                }
            }
            catch (NotFoundInFilmBaseException ex)
            {
                return Json(new { error = true, message = string.Format("{0}:{1}", ex.Message, ex.Value) });
            }
            catch (EntryAlreadyExistsException exp)
            {
				this.logger.Error(string.Format("Ошибка добавления: {0} KinopoiskID", exp.Message, model.KinopoiskID));
                return Json(new { error = true, message = exp.Message });
            }
            catch (Exception exp)
            {
				this.logger.Error(string.Format("Ошибка добавления: {0}", exp.Message));
                return Json(new { error = true, message = "Ошибка при попытке добавления." });
            }
            return Json(new { error = true, message = "Данные некорректы, соблюдайте условия валидации." });
        }

        /// <summary>
        /// Маппинг из ViewModel в VidoMaterial
        /// </summary>
        private VideoMaterial MapToFilmItem(AddFilmViewModel model, VideoMaterial target)
        {
            if (model != null)
            {
                target.Duration = model.Duration;
                target.IDMB = float.Parse(model.IDMB);
                target.KinopoiskRating = float.Parse(model.KinopoiskRating);
                target.KinopoiskID = model.KinopoiskID.ToString();
                target.OriginalTitle = model.OriginalTitle;
                target.ReleaseDate = model.ReleaseDate;
                target.Text = model.Text;
                target.Title = model.Title;
                target.Tagline = model.Tagline;
                target.Pictures = pictureService.AutoSave(model.Pictures);
                target.Pictures.Add(new Picture { IsPoster = true, URL = model.PosterHref });
                target.Themes = themeService.AutoSave(model.Themes);
                target.Actors = personService.AutoSave(model.Actors);
                target.FilmMakers = personService.AutoSave(model.FilmMakers);
                target.Genres = genreService.AutoSave(model.Genres);
                target.Countries = countryService.AutoSave(model.Countries);
                target.Text = model.Text;
                return target;
            }
            return null;
        }
    }
}