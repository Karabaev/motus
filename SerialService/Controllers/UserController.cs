namespace SerialService.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
	using System.Threading.Tasks;
    using DAL.Entities;
    using PagedList;
    using ViewModels;
    using AutoMapper;
    using Microsoft.AspNet.Identity;
    using Infrastructure.Helpers;
    using Infrastructure.Exceptions;
    using Infrastructure;
    using SerialService.DAL;
    using SerialService.Models;
    using SerialService.Infrastructure.ElasticSearch;
    using SerialService.Infrastructure.Core;
    using Newtonsoft.Json;
	using NLog;
    using SerialService.Sitemap;
    using System.Net.Mime;
    using System.Text;

    [ExceptionHandler]
	public class UserController : Controller
    {
		private readonly Logger logger;
		private readonly IAppUnitOfWork unitOfWork;
        private const int PageSize = 54;
        private const int RandomVideoCount = 3;

		public UserController(IAppUnitOfWork unitOfWork) // инициализировтаь нинжект
        {
            this.unitOfWork = unitOfWork;
			this.logger = LogManager.GetCurrentClassLogger();
		}

		/// <summary>
		/// Домашняя страница.
		/// </summary>
		/// <param name="page"></param>
		/// <returns></returns>
		public ActionResult Index(int? page)
        {
            Session["filter-lists"] = GlobalCache.GetItem("filter-lists");
            int pageNumber = (page ?? 1);
            List<ElasticVideoMaterial> videoMaterials = MotusElasticsearch.GetAll();
            RedirectHelper.SaveLocalURL(ViewBag, ControllerContext);
			ViewBag.Title = "Motus-cinema";
            ViewBag.Description = "Любимые фильмы и сериалы в хорошем качестве. Новинки кино, постоянное обновление базы фильмов и многое другое";
            return View(videoMaterials.ToPagedList(pageNumber, PageSize));
        }

		/// <summary>
		/// Страница видеоматериала.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpGet]
        public ActionResult VideoMaterialDetailPage(int? id)
        {
            if (!id.HasValue)
                return HttpNotFound();

            VideoMaterial videoMaterial = this.unitOfWork.VideoMaterials.GetVisibleToUser(id);

            if (videoMaterial == null)
                return HttpNotFound();

            VideoMaterialDetailsViewModel dvm = Mapper.Map<VideoMaterial, VideoMaterialDetailsViewModel>(videoMaterial);
            ElasticVideoMaterial thisMaterial = Mapper.Map<VideoMaterial, ElasticVideoMaterial>(videoMaterial);
            dvm.Similar = MotusElasticsearch.GetSimilar(thisMaterial);
			ViewBag.Title = $"{dvm.Title} - cмотреть онлайн";
            if (videoMaterial.SerialSeasons.Any())
            {
                ViewBag.Description = $"{dvm.Title}. Самые свежие серии в HD-качестве и озвучках от популярных студий онлайн.";
            }
            else
            {
                ViewBag.Description = $"{dvm.Title}. В HD-качестве и озвучках от популярных студий онлайн.";
            }
            var user = unitOfWork.Users.Get(User.Identity.GetUserId());
			string commentsApiKey = "V2vt2ul0QFpPnsa71RPNXnQ38gH5CWHkhKFJsQIFe9DCzHN2YLyiHZtU2UOIU4c4";

			if (user != null)
            {
				 //ViewBag.UserToken = UserTokenGenerator.GetUserSsoToken("V2vt2ul0QFpPnsa71RPNXnQ38gH5CWHkhKFJsQIFe9DCzHN2YLyiHZtU2UOIU4c4", "7", "Max", "maxkarab@yandex.ru", "https://img.icons8.com/ios/1600/folder-invoices.png");
				ViewBag.UserToken = UserTokenGenerator.GetUserSsoToken(commentsApiKey, user.Id, user.PublicName, user.Email, user.AvatarURL);
				dvm.IsUserSubscribed = this.unitOfWork.VideoMaterials.IsUserSubscribed(id, user.Id);
            }
			else
			{
				ViewBag.UserToken = UserTokenGenerator.GetUserSsoToken(commentsApiKey);
			}

            RedirectHelper.SaveLocalURL(ViewBag, ControllerContext);
            return View("DetailPage/VideoMaterialDetailPage", dvm);
        }

		/// <summary>
		/// Проверка, подписал ли авторизованный юзер на фильм.
		/// </summary>
		/// <param name="id">Идентификатор материала.</param>
		/// <returns></returns>
        [HttpGet]
        public JsonResult IsSubscribed(int? id)
        {
            if (!id.HasValue)
                return Json(new { Success = false, Message = "Фильм не найден." });

            return Json(new
						{
							Result = this.unitOfWork.VideoMaterials.IsUserSubscribed(id, User.Identity.GetUserId()),
							Success = true
						},
						JsonRequestBehavior.AllowGet);
        }

		[HttpPost]
        public JsonResult Subscribe(int? id)
        {
            if (!id.HasValue)
                return Json(new { Success = false, Message = "Произошла ошибка, попробуйте позже." });

            ApplicationUser user = this.unitOfWork.Users.Get(User.Identity.GetUserId());

            if (user == null)
                return Json(new { Success = false, Message = "Вы должны авторизоваться." });

            VideoMaterial videoMaterial = this.unitOfWork.VideoMaterials.GetVisibleToUser(id);

            if (videoMaterial == null)
                return Json(new { Success = false, Message = "Произошла ошибка, попробуйте позже." });

            if (!this.unitOfWork.VideoMaterials.AddSubscribedUser(id, user))
                return Json(new { Success = false, Message = "Произошла ошибка, попробуйте позже." });

            return Json(new { Success = true });
        }

		[HttpPost]
		public JsonResult Unsubscribe(int? id)
		{
			if (!id.HasValue)
				return Json(new { Success = false, Message = "Произошла ошибка, попробуйте позже." });

			ApplicationUser user = this.unitOfWork.Users.Get(User.Identity.GetUserId());

			if (user == null)
				return Json(new { Success = false, Message = "Вы должны авторизоваться." });

			VideoMaterial videoMaterial = this.unitOfWork.VideoMaterials.GetVisibleToUser(id);

			if (videoMaterial == null)
				return Json(new { Success = false, Message = "Произошла ошибка, попробуйте позже." });

			if (!this.unitOfWork.VideoMaterials.RemoveSubscribedUser(id, user))
			{
				Task.Run(() => this.logger.Error("Не удалось добавить запись о подписке."));
				return Json(new { Success = false, Message = "Произошла ошибка, попробуйте позже." });
			}

			return Json(new { Success = true });
		}

        /// <summary>
        /// Страница с результатами поиска.
        /// </summary>
        /// <param name="searchStr"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Search(string searchStr)
        {
            if (string.IsNullOrEmpty(searchStr))
            {
                return RedirectToAction("Index");
            }
            var result = MotusElasticsearch.Search(searchStr);
            RedirectHelper.SaveLocalURL(ViewBag, ControllerContext);
			return View("Index", result.ToPagedList(1, PageSize));
        }

        #region Поиск по свойствам

        public ActionResult SearchByCounty(string name)
        {
            Task.Run(() => this.logger.Info(""));
            var searchResult = this.unitOfWork.VideoMaterials.GetVisibleToUserWithCondition(vm => vm.Countries.Any(c => c.Name == name));
            var lvm = Mapper.Map<List<VideoMaterial>, List<VideoMaterialListViewModel>>(searchResult.ToList());
            return View("Search", lvm);
        }

        public ActionResult SearchByGenre(string name)
        {
            var searchResult = this.unitOfWork.VideoMaterials.GetVisibleToUserWithCondition(vm => vm.Genres.Any(g => g.Name == name));
            var lvm = Mapper.Map<List<VideoMaterial>, List<VideoMaterialListViewModel>>(searchResult.ToList());
            return View("Search", lvm);
        }

        public ActionResult SearchByActor(string fullName)
        {
            var searchResult = this.unitOfWork.VideoMaterials.GetVisibleToUserWithCondition(vm => vm.Actors.Any(a => a.FullName == fullName));
            var lvm = Mapper.Map<List<VideoMaterial>, List<VideoMaterialListViewModel>>(searchResult.ToList());
            return View("Search", lvm);
        }

        public ActionResult SearchByFilmMaker(string fullName)
        {
            var searchResult = this.unitOfWork.VideoMaterials.GetVisibleToUserWithCondition(vm => vm.FilmMakers.Any(fm => fm.FullName == fullName));
            var lvm = Mapper.Map<List<VideoMaterial>, List<VideoMaterialListViewModel>>(searchResult.ToList());
            return View("Search", lvm);
        }

        public ActionResult SearchByTheme(string name)
        {
            var searchResult = this.unitOfWork.VideoMaterials.GetVisibleToUserWithCondition(vm => vm.Themes.Any(t => t.Name == name));
            var lvm = Mapper.Map<List<VideoMaterial>, List<VideoMaterialListViewModel>>(searchResult.ToList());
            return View("Search", lvm);
        }

        #endregion

        /// <summary>
        /// Метод возвращающий количество лайков/дизлайков на страницу.
        /// </summary>
        /// <param name="id"></param>
        [HttpGet]
        public ActionResult GetMarks(int? id)
        {
            CountMarksViewModel countMarks = null;
            if (id.HasValue)
            {
                VideoMaterial vm = this.unitOfWork.VideoMaterials.GetVisibleToUser(id);
                countMarks = new CountMarksViewModel
                {
                    NegativeMarkCount = vm.NegativeMarkCount,
                    PositiveMarkCount = vm.PositiveMarkCount
                };
            }
            else
            {
                countMarks = new CountMarksViewModel();
            }

            return Json(countMarks, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Метод добавления лайка/дизлайка или меняющий их значение на обратное.
        /// </summary>
        /// <param name="mark"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddMark(VideoMark mark)
        {
            if (mark != null)
            {
                CountMarksViewModel countMarks = null;
                mark.UserIP = HttpContext.Request.UserHostAddress;
                mark.AuthorID = User.Identity.GetUserId();
                try
                {
                    if (this.unitOfWork.VideoMarks.Create(mark))
                    {
                        this.unitOfWork.VideoMaterials.AddMark(mark.VideoMaterialID, mark.Value);
                        VideoMaterial vm = this.unitOfWork.VideoMaterials.GetVisibleToUser(mark.VideoMaterialID);
                        countMarks = new CountMarksViewModel
                        {
                            NegativeMarkCount = vm.NegativeMarkCount,
                            PositiveMarkCount = vm.PositiveMarkCount
                        };
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (EntryAlreadyExistsException)
                {
                    VideoMark cache = this.unitOfWork.VideoMarks.GetScalarWithCondition(m => m.UserIP == mark.UserIP
                                                                                && m.VideoMaterialID == mark.VideoMaterialID);
                    if (cache == null)
                        return null;

                    // если ранее стоял лайк и еще раз пытаемся лайкнуть, то ничего не делать
                    if (mark.Value == cache.Value)
                        return null;

                    VideoMaterial vm = this.unitOfWork.VideoMaterials.GetVisibleToUser(mark.VideoMaterialID);
                    this.unitOfWork.VideoMaterials.InvertMark(vm.ID, !mark.Value); // изменить количество отметок.
                    this.unitOfWork.VideoMarks.InverValue(cache); // изменить запись пометки.
                    countMarks = new CountMarksViewModel
                    {
                        NegativeMarkCount = vm.NegativeMarkCount,
                        PositiveMarkCount = vm.PositiveMarkCount
                    };
                }

                Task.Run(() => this.logger.Info(string.Format("{0} оценен видеоматериал с ID {1}, с IP-адреса {2}",
                    mark.Value ? "Положительно" : "Отрицательно", mark.VideoMaterialID, mark.UserIP)));
                return Json(countMarks);
            }

            return null;
        }

        /// <summary>
        /// Страница личного кабинета.
        /// </summary>
        [HttpGet]
        public ActionResult PersonalAccount()
        {
            string id = User.Identity.GetUserId();

            if (string.IsNullOrWhiteSpace(id))
                return HttpNotFound();

            ApplicationUser user = this.unitOfWork.Users.Get(id);

            if (user == null)
                return HttpNotFound();

            PersonalAccountViewModel viewModel = Mapper.Map<ApplicationUser, PersonalAccountViewModel>(user);
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult PersonalAccountSaveChanges(PersonalAccountViewModel account)
        {
            Task.Run(() => this.logger.Info("Вызов UserController.PersonalAccountSaveChanges..."));

            if (account == null)
            {
                Task.Run(() => this.logger.Warn("Передан пустой параметр."));
                return HttpNotFound();
            }

            if (!ModelState.IsValid) // todo: тут надо как то по другому обработать
            {
                Task.Run(() => this.logger.Warn("Переданный параметр не валиден."));
                return HttpNotFound();
            }

            ApplicationUser user = this.unitOfWork.Users.Get(account.ID);

            if (user == null)
            {
                Task.Run(() => this.logger.Warn("Указанный пользователь не найден."));
                return HttpNotFound();
            }

            Task.Run(() => this.logger.Info(string.Format("Найден пользователь с ID {0}.", user.Id)));

            if (this.unitOfWork.Users.CheckPassword(user.Id, account.CurrentPassword))
            {
                if (!string.IsNullOrWhiteSpace(account.NewUserName))
                {
                    IdentityResult result = this.unitOfWork.Users.SetUserName(user.Id, account.NewUserName);

                    if (result.Succeeded)
                        Task.Run(() => this.logger.Info(string.Format("Имя пользователя успешно изменено на {0}.", account.NewUserName)));
                    else
                        Task.Run(() => this.logger.Warn(string.Format("Ошибки при изменении имени: {0}.", string.Join(", ", result.Errors))));
                }

                if (!string.IsNullOrWhiteSpace(account.NewAvatarURL))
                {
                    IdentityResult result = this.unitOfWork.Users.SetAvatar(user.Id, account.NewAvatarURL);

                    if (result.Succeeded)
                        Task.Run(() => this.logger.Info(string.Format("Аватарка пользователя успешно изменена на {0}.", account.NewAvatarURL)));
                    else
                        Task.Run(() => this.logger.Warn(string.Format("Ошибки при изменении аватарки: {0}.", string.Join(", ", result.Errors))));
                }

                if (!string.IsNullOrWhiteSpace(account.NewEmail))
                {
                    IdentityResult result = this.unitOfWork.Users.SetEmail(user.Id, account.NewEmail);

                    if (result.Succeeded)
                        Task.Run(() => this.logger.Info(string.Format("Email пользователя успешно изменен на {0}.", account.NewEmail)));
                    else
                        Task.Run(() => this.logger.Warn(string.Format("Ошибки при изменении email: {0}.", string.Join(", ", result.Errors))));
                }

                if (!string.IsNullOrWhiteSpace(account.NewParole))
                {
                    IdentityResult result = this.unitOfWork.Users.SetParole(user.Id, account.NewParole);

                    if (result.Succeeded)
                        Task.Run(() => this.logger.Info(string.Format("Контрольное слово пользователя успешно изменено на .", account.NewParole)));
                    else
                        Task.Run(() => this.logger.Warn(string.Format("Ошибки при изменении контрольного слова: {0}.", string.Join(", ", result.Errors))));
                }

                if (!string.IsNullOrWhiteSpace(account.NewPassword))
                {
                    if (account.NewPassword == account.ConfirmPassword)
                    {
                        IdentityResult result = this.unitOfWork.Users.SetPassword(user.Id, account.CurrentPassword, account.NewPassword);

                        if (result.Succeeded)
                            Task.Run(() => this.logger.Info("Пароль пользователя успешно изменен."));
                        else
                            Task.Run(() => this.logger.Warn(string.Format("Ошибки при изменении пароля: {0}.", string.Join(", ", result.Errors))));
                    }
                    else
                    {
                        Task.Run(() => this.logger.Warn(string.Format("Пароли не совпадают.")));
                    }
                }
            }
            else
            {
                Task.Run(() => this.logger.Warn(string.Format("Был введен неверный пароль для подтверждения. Завершение UserController.PersonalAccountSaveChanges.")));
                //todo: вывести ошибку 
                return Json(new
                {
					error = "Пароль для подтверждения не правильный.",
                });
            }
            user = this.unitOfWork.Users.Get(account.ID);
            Task.Run(() => this.logger.Info("Завершение UserController.PersonalAccountSaveChanges."));

            return Json(new
            {
				success = "Что то там"
            });
        }

        [HttpPost]
        public JsonResult UploadAvatar()
        {
            Task.Run(() => this.logger.Info("Вызов UserController.UploadAvatar. User ID: " + User.Identity.GetUserId()));
            var files = Request.Files;
            JsonResult badResult = Json(new
            {
                Success = false,
                Message = "Не удалось загрузить аватарку."
            });


            if (files.Count == 0)
            {
                Task.Run(() => this.logger.Warn("Аватарка не была загружена. Завершение UserController.PersonalAccountSaveChanges."));
                return badResult;
            }

            string virtualAvatarPath = string.Format("{0}{1}_{2}", Resource.UserAvatarFolder, Guid.NewGuid(), files[0].FileName);
            string absoluteAvatarPath = Server.MapPath(virtualAvatarPath);

            if (files[0].ContentLength > 0)
            {
                try
                {
                    files[0].SaveAs(absoluteAvatarPath);
                    Task.Run(() => this.logger.Info("Файл аватарки " + absoluteAvatarPath + " был успешно сохранен."));
                }
                catch (Exception ex)
                {
                    Task.Run(() => this.logger.Error(ex, "Не удалось сохранить файл аватарки " + absoluteAvatarPath));
                    return Json(new { Success = false, Message = "Не удалось загрузить файл на сервер. Попробуйте позже." });
                }
            }

            string oldAvatarPath = this.unitOfWork.Users.Get(User.Identity.GetUserId()).AvatarURL;
            IdentityResult result = this.unitOfWork.Users.SetAvatar(User.Identity.GetUserId(), virtualAvatarPath);

            if (!result.Succeeded)
            {
                Task.Run(() => this.logger.Warn("Ошибка при присвоении аватарки пользователю. Завершение UserController.PersonalAccountSaveChanges."));
                return badResult;
            }

            try
            {
                System.IO.File.Delete(Server.MapPath(oldAvatarPath));
                Task.Run(() => this.logger.Info("Файл старой аватарки " + oldAvatarPath + " успшно удален."));
            }
            catch (Exception ex)
            {
                Task.Run(() => this.logger.Warn(ex, "Не удалось удалить файл аватарки " + oldAvatarPath));
            }

            Task.Run(() => this.logger.Info("Успешное завершение UserController.UploadAvatar. User ID: " + User.Identity.GetUserId()));
            return Json(new
            {
                Success = true,
                AvatarPath = virtualAvatarPath
            });
        }

		[HttpGet]
		public ActionResult Filter(string json)
		{
			var data = JsonConvert.DeserializeObject<FilterData>(json);
			var result = SearchWrapper.FilerResults(data).ToList();
			return View("Index", result.ToPagedList(1, PageSize));
		}

        [HttpPost]
        public JsonResult GetSuggest(string part)
        {
            if (string.IsNullOrEmpty(part))
            {
                return null;
            }
            List<string> result;
            try
            {
                result = MotusElasticsearch.GetSuggest(part);
                return Json(new { Array = result });
            }
            catch
            {
                return null;
            }
        }

        public ActionResult AboutProject()
        {
            return View();
        }

        public ActionResult ForHolders()
        {
            return View();
        }

        public ActionResult SitemapXml()
        {
            var generator = new SitemapGenerator();
            var sitemapNodes = generator.GetNodes(this.Url);
            string xml = generator.GetSitemapDocument(sitemapNodes);
            return this.Content(xml, "text/xml", Encoding.UTF8);
        }
    }
}