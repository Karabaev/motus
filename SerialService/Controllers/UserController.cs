﻿namespace SerialService.Controllers
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
    using System.Configuration;
    using System.Drawing;
    using System.IO;
    using System.Drawing.Imaging;
    using ViewModels.User;

    [ExceptionHandler]
	public class UserController : Controller
	{
		private readonly Logger logger;
		private readonly IAppUnitOfWork unitOfWork;
		private const int PageSize = 24;
        private readonly string mainTitle;

        public UserController(IAppUnitOfWork unitOfWork) // инициализировтаь нинжект
		{
			this.unitOfWork = unitOfWork;
			this.logger = LogManager.GetCurrentClassLogger();
            this.mainTitle = ConfigurationManager.AppSettings["MainTitle"];
        }

		/// <summary>
		/// Домашняя страница.
		/// </summary>
		public ActionResult Index(int? page)
		{
			this.ViewBag.Title = mainTitle;
			this.ViewBag.Description = ConfigurationManager.AppSettings["IndexDescription"];
            Session["FilterSettings"] = null;
            this.ViewBag.CurrentURL = this.GetCurrentURL(page);
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
            this.ViewBag.Title = string.Format("{0} - {1}", dvm.Title, ConfigurationManager.AppSettings["VideoMaterialTitlePart"]);
            string descriptionPart = videoMaterial.IsSerial ? ConfigurationManager.AppSettings["SerialDescriptionPart"]
                                                            : ConfigurationManager.AppSettings["FilmDescriptionPart"];
            this.ViewBag.Description = string.Format("{0}. {1}", dvm.Title, descriptionPart);
            this.ViewBag.CurrentURL = this.GetCurrentURL(id);
            var user = this.unitOfWork.Users.Get(this.User.Identity.GetUserId());

            if (user != null)
			{
				dvm.IsUserSubscribed = this.unitOfWork.VideoMaterials.IsUserSubscribed(id, user.Id);
            }

            VideoMaterialViewsByUsers viewInfo = videoMaterial.ViewsByUsers.FirstOrDefault(vu => vu.UserID == this.User.Identity.GetUserId());

            if (viewInfo == null)
                viewInfo = videoMaterial.ViewsByUsers.FirstOrDefault(vu => vu.UserIP == this.HttpContext.Request.UserHostAddress);

            if (viewInfo != null)
            {
                ViewBag.StartTime = viewInfo.EndTimeOfLastView;
                ViewBag.EpisodeNumber = viewInfo.EpisodeNumber;
                ViewBag.SeasonNumber = viewInfo.SerialSeason.SeasonNumber;
                ViewBag.Translator = viewInfo.SerialSeason.Translation.Name;
            }

            return this.View("DetailPage/VideoMaterialDetailPage", dvm);
		}

        [HttpPost]
        public JsonResult SaveViewTime(SaveViewTimeViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.UserID = User.Identity.GetUserId();
                model.UserIP = this.HttpContext.Request.UserHostAddress;
                VideoMaterial video = this.unitOfWork.VideoMaterials.Get(model.VideoMaterialID);
                SerialSeason season = null;

                if (string.IsNullOrWhiteSpace(model.TranslatorName))
                    season = video.SerialSeasons.FirstOrDefault(ss => ss.SeasonNumber == (model.SeasonNumber ?? 1));
                else
                    season = video.SerialSeasons.FirstOrDefault(ss => ss.SeasonNumber == (model.SeasonNumber ?? 1) && ss.Translation.Name == model.TranslatorName);


                if (season == null)
                {
                    Task.Run(() => this.logger.Error("SaveViewTime(SaveViewTimeViewModel). Сезон Номер: {0} ИД видеоматериала: {1} не найден", model.SeasonNumber ?? 1, model.VideoMaterialID));
                    return this.Json(new { error = "SaveViewTime(): ошибка инициализации." });
                }

                VideoMaterialViewsByUsers entity = null;

                if (User.Identity.IsAuthenticated)
                {
                    entity = this.unitOfWork.VideoMaterialViewsByUsers.GetScalarWithCondition(vu
                                 => vu.VideoMaterialID == model.VideoMaterialID && vu.UserID == model.UserID);
                }

                if (entity == null)
                {
                    entity = this.unitOfWork.VideoMaterialViewsByUsers.GetScalarWithCondition(vu
                                => vu.VideoMaterialID == model.VideoMaterialID && vu.UserIP == model.UserIP);
                }

                bool result = false;

                if (entity == null)
                {
                    entity = new VideoMaterialViewsByUsers
                    {
                        UserID = model.UserID,
                        UserIP = model.UserIP,
                        VideoMaterialID = model.VideoMaterialID,
                        EndTimeOfLastView = model.TimeSec,
                        EpisodeNumber = model.EpisodeNumber,
                        SerialSeason = season,
                        SerialSeasonID = season.ID,
                        UpdateDateTime = DateTime.Now
                    };

                    result = this.unitOfWork.VideoMaterialViewsByUsers.Create(entity);
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(model.UserID))
                        entity.UserID = model.UserID;

                    entity.UserIP = model.UserIP;
                    entity.EndTimeOfLastView = model.TimeSec;
                    entity.EpisodeNumber = model.EpisodeNumber;
                    entity.SerialSeason = season;
                    entity.SerialSeasonID = season.ID;
                    entity.UpdateDateTime = DateTime.Now;
                    result = this.unitOfWork.VideoMaterialViewsByUsers.UpdateEntity(entity);
                }

                if (result)
                {
                    return this.Json(new { success = "Время просмотра сохранено" });
                }
                else
                {
                    Task.Run(() => this.logger.Error("SaveViewTime(SaveViewTimeViewModel). Не удалось сохранить объект {0}", entity));
                    return this.Json(new { error = "SaveViewTime(): Не удалось сохранить время просмотра." });
                }
            }
            else
            {
                return this.Json(new { error = "SaveViewTime(): Данные некорректны." });
            }
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
            this.ViewBag.CurrentURL = this.GetCurrentURL(searchStr);
            var result = MotusElasticsearch.Search(searchStr);
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

            if (string.IsNullOrWhiteSpace(viewModel.CurrentAvatarURL))
                viewModel.CurrentAvatarURL = string.Format("{0}/{1}", Resource.MediaFolder, Resource.DefaultUserAvatarFileName);
                
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

        public ActionResult AddExternalLogin()
        {
            string id = this.User.Identity.GetUserId();

            if (string.IsNullOrWhiteSpace(id))
                return this.HttpNotFound();


            return View();
        }

		public ActionResult ConfirmNewEmail(ConfirmNewEmailViewModel model)
		{
			if (model == null || !ModelState.IsValid)
				return HttpNotFound();

			IdentityResult result = this.unitOfWork.Users.ChangeEmail(model.UserID, model.NewEmail, model.Code);
            string oursEmail = ConfigurationManager.AppSettings["SupportEmail"];

			if (result.Succeeded)
				model.ResultMessage = "Адрес электронной почты успешно изменен";
			else
				model.ResultMessage = string.Format("Не удалось изменить адрес электронной почты. Пожалуйста, сообщите о проблеме по адресу {0}", oursEmail);

			return View(model);
		}

		[HttpPost]
		public JsonResult UploadAvatar(string base64)
		{
            var file = Base64ToImage(base64);
			JsonResult badResult = this.Json(new { error = "Не удалось загрузить аватарку." });

			if (file == null)
			{
				Task.Run(() => this.logger.Warn("Аватарка не была загружена. Завершение UserController.PersonalAccountSaveChanges."));
				return badResult;
			}

			string virtualAvatarPath = string.Format("{0}{1}.jpeg", Resource.UserAvatarFolder, Guid.NewGuid());
			string absoluteAvatarPath = this.Server.MapPath(virtualAvatarPath);

            try
			{
                using (MemoryStream memory = new MemoryStream())
                {
                    using (FileStream fs = new FileStream(absoluteAvatarPath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                    {
                        var encoderParameters = new EncoderParameters(1);
                        var encoderParameter = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);
                        encoderParameters.Param[0] = encoderParameter;
                        file.Save(memory, GetEncoderInfo("image/jpeg"), encoderParameters);
                        byte[] bytes = memory.ToArray();
                        fs.Write(bytes, 0, bytes.Length);
                    }
                }
            }
            catch (Exception ex)
			{
				Task.Run(() => this.logger.Error(ex, "Не удалось сохранить файл аватарки " + absoluteAvatarPath));
				return this.Json(new { error = "Не удалось загрузить файл на сервер. Попробуйте позже." });
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
            this.ViewBag.CurrentURL = this.GetCurrentURL();
            return View();
        }

        public ActionResult ForHolders()
        {
            this.ViewBag.CurrentURL = this.GetCurrentURL();
            return View();
        }

        public ActionResult SitemapXml()
        {
            var generator = new SitemapGenerator();
            var sitemapNodes = generator.GetNodes(this.Url);
            string xml = generator.GetSitemapDocument(sitemapNodes);
            return this.Content(xml, "text/xml", Encoding.UTF8);
        }

        private Image Base64ToImage(string base64String)
        {
            // Convert Base64 String to byte[]
            const string ExpectedImagePrefix = "data:image/jpeg;base64,";

            if (base64String.StartsWith(ExpectedImagePrefix))
            {
                base64String = base64String.Substring(ExpectedImagePrefix.Length);
            }
            byte[] imageBytes = Convert.FromBase64String(base64String);
            MemoryStream ms = new MemoryStream(imageBytes, 0,
              imageBytes.Length);

            // Convert byte[] to Image
            ms.Write(imageBytes, 0, imageBytes.Length);
            Image image = Image.FromStream(ms, true);
            return image;
        }

        private static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            int j;
            ImageCodecInfo[] encoders = ImageCodecInfo.GetImageDecoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }

        private string GetCurrentURL(object routeValues = null)
        {
            var routeDataValues = ControllerContext.RouteData.Values;
            StringBuilder result = new StringBuilder(Url.Action(routeDataValues["action"].ToString(), routeDataValues["controller"].ToString(), routeValues));
            return result.ToString();
        }
    }
}