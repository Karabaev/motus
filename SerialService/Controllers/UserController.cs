namespace SerialService.Controllers
{
    using AutoMapper;
    using DAL.Entities;
    using Infrastructure;
    using Infrastructure.Exceptions;
    using Infrastructure.Helpers;
    using Microsoft.AspNet.Identity;
    using Newtonsoft.Json;
    using NLog;
    using SerialService.Sitemap;
    using System.Text;
    using PagedList;
    using SerialService.DAL;
    using SerialService.Infrastructure.Core;
    using SerialService.Infrastructure.ElasticSearch;
    using SerialService.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using ViewModels;

    [ExceptionHandler]
	public class UserController : Controller
	{
		private readonly Logger logger;
		private readonly IAppUnitOfWork unitOfWork;
		private const int PageSize = 54;
        private const string mainTitle = "Motus-cinema";

        public UserController(IAppUnitOfWork unitOfWork) // инициализировтаь нинжект
		{
			this.unitOfWork = unitOfWork;
			this.logger = LogManager.GetCurrentClassLogger();
		}

		/// <summary>
		/// Домашняя страница.
		/// </summary>
		public ActionResult Index(int? page)
		{
			RedirectHelper.SaveLocalURL(this.ViewBag, this.ControllerContext);
			this.ViewBag.Title = mainTitle;
			this.ViewBag.Description = "Любимые фильмы и сериалы в хорошем качестве. Новинки кино, постоянное обновление базы фильмов и многое другое";
            Session["FilterSettings"] = null;
            return RenderFilmsList(page);
		}

        /// <summary>
        /// Вывести список фильмов
        /// </summary>
        public ActionResult RenderFilmsList(int? page)
        {
            if (Session["filter-lists"] == null)
            {
                Session["filter-lists"] = GlobalCache.GetItem("filter-lists");
            }

            List<ElasticVideoMaterial> videoMaterials;
            if (Session["FilterSettings"] == null)
            {
                videoMaterials = MotusElasticsearch.GetAll();
                ViewBag.Title = mainTitle;
            }
            else
            {
                ViewBag.Title = "Результаты фильтрации";
                videoMaterials = SearchWrapper.FilerResults(Session["FilterSettings"] as FilterData).ToList();
            }

            page = (page ?? 1);
            return View("Index", videoMaterials.ToPagedList(page.Value, PageSize));
        }

		/// <summary>
		/// Страница видеоматериала.
		/// </summary>
		[HttpGet]
		public ActionResult VideoMaterialDetailPage(int? id)
		{
			if (!id.HasValue)
				return this.HttpNotFound();

			VideoMaterial videoMaterial = this.unitOfWork.VideoMaterials.GetVisibleToUser(id);

			if (videoMaterial == null)
				return this.HttpNotFound();

			VideoMaterialDetailsViewModel dvm = Mapper.Map<VideoMaterial, VideoMaterialDetailsViewModel>(videoMaterial);
			ElasticVideoMaterial thisMaterial = Mapper.Map<VideoMaterial, ElasticVideoMaterial>(videoMaterial);
			dvm.Similar = MotusElasticsearch.GetSimilar(thisMaterial);
			this.ViewBag.Title = $"{dvm.Title} - cмотреть онлайн";
			if (videoMaterial.SerialSeasons.Any())
			{
				this.ViewBag.Description = $"{dvm.Title}. Самые свежие серии в HD-качестве и озвучках от популярных студий онлайн.";
			}
			else
			{
				this.ViewBag.Description = $"{dvm.Title}. В HD-качестве и озвучках от популярных студий онлайн.";
			}
			var user = this.unitOfWork.Users.Get(this.User.Identity.GetUserId());
			string commentsApiKey = "727fd347-51d7-4338-a8c2-33075a2f7c2f";

			if (user != null)
			{
                this.ViewBag.UserToken = UserTokenGenerator.GetUserSsoToken(commentsApiKey, user.Id, user.UserName, user.Email, user.AvatarURL);
				dvm.IsUserSubscribed = this.unitOfWork.VideoMaterials.IsUserSubscribed(id, user.Id);
			}
			else
			{
				this.ViewBag.UserToken = UserTokenGenerator.GetUserSsoToken(commentsApiKey);
			}

			RedirectHelper.SaveLocalURL(this.ViewBag, this.ControllerContext);
			return this.View("DetailPage/VideoMaterialDetailPage", dvm);
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
				return this.Json(new { Success = false, Message = "Фильм не найден." });

			return this.Json(new
			{
				Result = this.unitOfWork.VideoMaterials.IsUserSubscribed(id, this.User.Identity.GetUserId()),
				Success = true
			},
						JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public JsonResult Subscribe(int? id)
		{
			if (!id.HasValue)
				return this.Json(new { Success = false, Message = "Произошла ошибка, попробуйте позже." });

			ApplicationUser user = this.unitOfWork.Users.Get(this.User.Identity.GetUserId());

			if (user == null)
				return this.Json(new { Success = false, Message = "Вы должны авторизоваться." });

			VideoMaterial videoMaterial = this.unitOfWork.VideoMaterials.GetVisibleToUser(id);

			if (videoMaterial == null)
				return this.Json(new { Success = false, Message = "Произошла ошибка, попробуйте позже." });

			if (!this.unitOfWork.VideoMaterials.AddSubscribedUser(id, user))
				return this.Json(new { Success = false, Message = "Произошла ошибка, попробуйте позже." });

			return this.Json(new { Success = true });
		}

		[HttpPost]
		public JsonResult Unsubscribe(int? id)
		{
			if (!id.HasValue)
				return this.Json(new { Success = false, Message = "Произошла ошибка, попробуйте позже." });

			ApplicationUser user = this.unitOfWork.Users.Get(this.User.Identity.GetUserId());

			if (user == null)
				return this.Json(new { Success = false, Message = "Вы должны авторизоваться." });

			VideoMaterial videoMaterial = this.unitOfWork.VideoMaterials.GetVisibleToUser(id);

			if (videoMaterial == null)
				return this.Json(new { Success = false, Message = "Произошла ошибка, попробуйте позже." });

			if (!this.unitOfWork.VideoMaterials.RemoveSubscribedUser(id, user))
			{
				Task.Run(() => this.logger.Error("Не удалось добавить запись о подписке."));
				return this.Json(new { Success = false, Message = "Произошла ошибка, попробуйте позже." });
			}

			return this.Json(new { Success = true });
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
				return this.RedirectToAction("Index");
			}
            ViewBag.SearchResult = $"Результат поиска по \"{searchStr}\"";
			var result = MotusElasticsearch.Search(searchStr);
			RedirectHelper.SaveLocalURL(this.ViewBag, this.ControllerContext);
			return this.View("Index", result.ToPagedList(1, PageSize));
		}

		#region Поиск по свойствам

		public ActionResult SearchByCounty(string name)
		{
			Task.Run(() => this.logger.Info(""));
			var searchResult = this.unitOfWork.VideoMaterials.GetVisibleToUserWithCondition(vm => vm.Countries.Any(c => c.Name == name));
			var lvm = Mapper.Map<List<VideoMaterial>, List<VideoMaterialListViewModel>>(searchResult.ToList());
			return this.View("Search", lvm);
		}

		public ActionResult SearchByGenre(string name)
		{
			var searchResult = this.unitOfWork.VideoMaterials.GetVisibleToUserWithCondition(vm => vm.Genres.Any(g => g.Name == name));
			var lvm = Mapper.Map<List<VideoMaterial>, List<VideoMaterialListViewModel>>(searchResult.ToList());
			return this.View("Search", lvm);
		}

		public ActionResult SearchByActor(string fullName)
		{
			var searchResult = this.unitOfWork.VideoMaterials.GetVisibleToUserWithCondition(vm => vm.Actors.Any(a => a.FullName == fullName));
			var lvm = Mapper.Map<List<VideoMaterial>, List<VideoMaterialListViewModel>>(searchResult.ToList());
			return this.View("Search", lvm);
		}

		public ActionResult SearchByFilmMaker(string fullName)
		{
			var searchResult = this.unitOfWork.VideoMaterials.GetVisibleToUserWithCondition(vm => vm.FilmMakers.Any(fm => fm.FullName == fullName));
			var lvm = Mapper.Map<List<VideoMaterial>, List<VideoMaterialListViewModel>>(searchResult.ToList());
			return this.View("Search", lvm);
		}

		public ActionResult SearchByTheme(string name)
		{
			var searchResult = this.unitOfWork.VideoMaterials.GetVisibleToUserWithCondition(vm => vm.Themes.Any(t => t.Name == name));
			var lvm = Mapper.Map<List<VideoMaterial>, List<VideoMaterialListViewModel>>(searchResult.ToList());
			return this.View("Search", lvm);
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

			return this.Json(countMarks, JsonRequestBehavior.AllowGet);
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
				mark.UserIP = this.HttpContext.Request.UserHostAddress;
				mark.AuthorID = this.User.Identity.GetUserId();
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
				return this.Json(countMarks);
			}

			return null;
		}

		/// <summary>
		/// Страница личного кабинета.
		/// </summary>
		[HttpGet]
		public ActionResult PersonalAccount()
		{
			string id = this.User.Identity.GetUserId();

			if (string.IsNullOrWhiteSpace(id))
				return this.HttpNotFound();

			ApplicationUser user = this.unitOfWork.Users.Get(id);

			if (user == null)
				return this.HttpNotFound();

			PersonalAccountViewModel viewModel = Mapper.Map<ApplicationUser, PersonalAccountViewModel>(user);
			return this.View(viewModel);
		}

		[HttpPost]
		public ActionResult PersonalAccountSaveChanges(PersonalAccountViewModel account)
		{
			StringBuilder errors = new StringBuilder();
			string advancedMessage = string.Empty;

			if (account == null)
			{
				Task.Run(() => this.logger.Warn("Передан пустой параметр."));
				return this.HttpNotFound();
			}

			if (!this.ModelState.IsValid)
			{
				Task.Run(() => this.logger.Warn("Переданный параметр не валиден."));
				return Json(new { error = "Ошибка валидации" });
			}

			ApplicationUser user = this.unitOfWork.Users.Get(account.ID);

			if (user == null)
			{
				Task.Run(() => this.logger.Warn("Указанный пользователь не найден."));
				return this.HttpNotFound();
			}

			Task.Run(() => this.logger.Info(string.Format("Найден пользователь с ID {0}.", user.Id)));

			if (this.unitOfWork.Users.CheckPassword(user.Id, account.CurrentPassword))
			{
				if (!string.IsNullOrWhiteSpace(account.NewUserName))
				{
					IdentityResult result = this.unitOfWork.Users.SetUserName(user.Id, account.NewUserName);

					if (result.Succeeded)
					{
						Task.Run(() => this.logger.Info(string.Format("Имя пользователя успешно изменено на {0}.", account.NewUserName)));
					}
					else
					{
						errors.Append("Не удалось изменить публичное имя<br/>");
						Task.Run(() => this.logger.Warn(string.Format("Ошибки при изменении имени: {0}.", string.Join(", ", result.Errors))));
					}
				}

				if (!string.IsNullOrWhiteSpace(account.NewEmail))
				{
					if (user.Email.ToLower() != account.NewEmail.ToLower())
					{
						var checkUser = this.unitOfWork.Users.GetByMainStringProperty(account.NewEmail);

						if (checkUser == null)
						{
							var code = this.unitOfWork.Users.GenerateEmailConfirmationToken(user.Id);
							var result = this.unitOfWork.Users.SetKey(user.Id, code);

							if (result.Succeeded)
							{
								var callbackUrl = this.Url.Action("ConfirmNewEmail",
											"User",
											new ConfirmNewEmailViewModel { UserID = user.Id, Code = code, NewEmail = account.NewEmail, OldEmail = user.Email },
											protocol: this.Request.Url.Scheme);

								Task.Run(() => this.unitOfWork.Users.SendToCustomEmail(
									account.NewEmail,
									"Изменение адреса электронной почты",
									string.Format("Для изменения адреса электронной почты перейдите по <a href=\"{0}\">ссылке</a>",
													callbackUrl)));
								advancedMessage = string.Format("Для изменения эл. почты необходимо перейти по ссылке, отправленной по адресу {0}", account.NewEmail);
							}
							else
							{
								errors.Append(string.Format("Произошла ошибка при генерации ключа. {0}<br/>", string.Join(", ", result.Errors)));
							}
						}
						else
						{
							errors.Append(string.Format("Адрес эл. почты {0} занят<br/>", account.NewEmail));
						}
					}
					else
					{
						errors.Append(string.Format("У вас уже установлен адрес эл. почты {0}<br/>", account.NewEmail));
					}
				}

				if (!string.IsNullOrWhiteSpace(account.NewParole))
				{
					IdentityResult result = this.unitOfWork.Users.SetParole(user.Id, account.NewParole);

					if (result.Succeeded)
					{
						Task.Run(() => this.logger.Info(string.Format("Контрольное слово пользователя успешно изменено на .", account.NewParole)));
					}
					else
					{
						errors.Append("Не удалось изменить контрольное слово<br/>");
						Task.Run(() => this.logger.Warn(string.Format("Ошибки при изменении контрольного слова: {0}.", string.Join(", ", result.Errors))));
					}
				}

				if (!string.IsNullOrWhiteSpace(account.NewPassword))
				{
					if (account.NewPassword == account.ConfirmPassword)
					{
						IdentityResult result = this.unitOfWork.Users.SetPassword(user.Id, account.CurrentPassword, account.NewPassword);

						if (result.Succeeded)
						{
							Task.Run(() => this.logger.Info("Пароль пользователя успешно изменен."));
						}
						else
						{
							errors.Append("Не удалось изменить пароль<br/>");
							Task.Run(() => this.logger.Warn(string.Format("Ошибки при изменении пароля: {0}.", string.Join(", ", result.Errors))));
						}
					}
					else
					{
						errors.Append("Пароль и его подтвержение не совпадают<br/>");
						Task.Run(() => this.logger.Warn(string.Format("Пароль и его подтвержение не совпадают")));
					}
				}
			}
			else
			{
				Task.Run(() => this.logger.Warn(string.Format("Был введен неверный пароль для подтверждения. Завершение UserController.PersonalAccountSaveChanges.")));
				return this.Json(new { error = "Пароль для подтверждения не правильный." });
			}

			if(errors.Length > 0)
				return this.Json(new { error = errors.ToString(), message = advancedMessage });

			user = this.unitOfWork.Users.Get(account.ID);

			return this.Json(new { success = true, email = user.Email, name = user.UserName, message = advancedMessage });
		}

		public ActionResult ConfirmNewEmail(ConfirmNewEmailViewModel model)
		{
			if (model == null || !ModelState.IsValid)
				return HttpNotFound();

			IdentityResult result = this.unitOfWork.Users.ChangeEmail(model.UserID, model.NewEmail, model.Code);
			string oursEmail = "help@motus-cinema.com";

			if (result.Succeeded)
				model.ResultMessage = "Адрес электронной почты успешно изменен";
			else
				model.ResultMessage = string.Format("Не удалось изменить адрес электронной почты. Пожалуйста, сообщите о проблеме по адресу {0}", oursEmail);

			return View(model);
		}

		[HttpPost]
		public JsonResult UploadAvatar()
		{
			var files = this.Request.Files;
			JsonResult badResult = this.Json(new { error = "Не удалось загрузить аватарку." });

			if (files.Count == 0)
			{
				Task.Run(() => this.logger.Warn("Аватарка не была загружена. Завершение UserController.PersonalAccountSaveChanges."));
				return badResult;
			}

			string virtualAvatarPath = string.Format("{0}{1}_{2}", Resource.UserAvatarFolder, Guid.NewGuid(), files[0].FileName);
			string absoluteAvatarPath = this.Server.MapPath(virtualAvatarPath);

			if (files[0].ContentLength > 0)
			{
				try
				{
					files[0].SaveAs(absoluteAvatarPath);
				}
				catch (Exception ex)
				{
					Task.Run(() => this.logger.Error(ex, "Не удалось сохранить файл аватарки " + absoluteAvatarPath));
					return this.Json(new { error = "Не удалось загрузить файл на сервер. Попробуйте позже." });
				}
			}

			string oldAvatarPath = this.unitOfWork.Users.Get(this.User.Identity.GetUserId()).AvatarURL;
			IdentityResult result = this.unitOfWork.Users.SetAvatar(this.User.Identity.GetUserId(), virtualAvatarPath);

			if (!result.Succeeded)
			{
				Task.Run(() => this.logger.Warn("Ошибка при присвоении аватарки пользователю. Завершение UserController.PersonalAccountSaveChanges."));
				return badResult;
			}

			try
			{
				System.IO.File.Delete(this.Server.MapPath(oldAvatarPath));
			}
			catch (Exception ex)
			{
				Task.Run(() => this.logger.Warn(ex, "Не удалось удалить файл аватарки " + oldAvatarPath));
			}

			return this.Json(new { success = virtualAvatarPath });
		}

        public ActionResult Filter(FilterData data)
        {
            Session["FilterSettings"] = data;
            return RenderFilmsList(1);
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
				return this.Json(new { Array = result });
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