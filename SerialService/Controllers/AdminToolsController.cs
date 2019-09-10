namespace SerialService.Controllers
{
    using System;
    using System.Web.Mvc;
    using SerialService.DAL;
    using SerialService.Services.Interfaces;
    using SerialService.DAL.Entities;
    using System.Collections.Generic;
    using System.Linq;
    using ViewModels;
    using SerialService.ViewModels.AdminTools;
    using Microsoft.AspNet.Identity.EntityFramework;
	using Infrastructure;
	using App_Start;
    using System.Threading.Tasks;

    [ExceptionHandler]
	public class AdminToolsController : Controller
    {
        IAppUnitOfWork _unitOfWork;
        private readonly IUserService userService;
        private readonly IVideoMaterialService videoMaterialService;
        private readonly IRoleService roleService;

        public AdminToolsController(IAppUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
            this.userService = this._unitOfWork.Users;
            this.videoMaterialService = this._unitOfWork.VideoMaterials;
            this.roleService = this._unitOfWork.Roles;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetUsersTable(string name)
        {
            List<ApplicationUser> users = new List<ApplicationUser>();
            if (string.IsNullOrEmpty(name))
            {
                return PartialView(users);
            }
            users = userService.GetByUserNamePart(name).ToList();
            return PartialView(users);
        }

        [HttpPost]
        public ActionResult GetFilmsTable(string title)
        {
            List<VideoMaterial> films = new List<VideoMaterial>();
            if (string.IsNullOrEmpty(title))
            {
                return PartialView(films);
            }
            films = videoMaterialService.GetByPartOfTitle(title).ToList();
            return PartialView(films);
        }

        [HttpPost]
        public ActionResult UsersSearch()
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult FilmSearch()
        {
            return PartialView();
        }

        public ActionResult GetUserInfo(string uid)
        {
            ApplicationUser user = this.userService.Get(uid);
            ViewBag.IsLocked = user.IsLocked;
            ViewBag.IsAdmin = this.roleService.UserIsInRole(user.Id,Resource.AdminRoleName);
            ViewBag.Roles = this.roleService.GetAll()
                .Where(r=>r.Name!=Resource.BannedRoleName&&r.Name!=Resource.UserRoleName)
                .ToList();
            PersonalAccountViewModel userModel = AutoMapper.Mapper.Map<ApplicationUser, PersonalAccountViewModel>(user);
            AdministratableUserInfo userInfo = new AdministratableUserInfo
            {
                UserRoles = this.roleService.GetUserRoles(user.Id).ToList(),
                PersonalUserData = userModel
            };
            return View(userInfo); 
        }

        [HttpPost]
        public ActionResult SaveRoles(List<string> roleNames, string userId)
        {
            List<IdentityRole> roles = new List<IdentityRole>();
            if (roleNames!=null)
            {
                foreach (string roleName in roleNames)
                {
                    roles.Add(this.roleService.GetByName(roleName));
                }
            }
            try
            {
                this.userService.UpdateUserRoles(roles, userId);
            }
            catch
            {
                return Json(new { result = false });
            }
            return Json(new { result = true });
        }

        public ActionResult LockManageUser(string userId)
        {
            bool userIsLocked = this.userService.Get(userId).IsLocked;
            DateTime until = DateTime.MaxValue;
            if (userIsLocked)
            {
                this.userService.Unban(userId);
            }
            else
            {
                this.userService.Ban(userId,until);
            }
            return Redirect(Request.UrlReferrer.ToString());
        }

        [HttpPost]
        public ActionResult DeleteUser(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { result = false });
            }
            bool result = this.userService.Remove(userId).Succeeded;
            return Json(new { result = result });
        }

		public async Task<JsonResult> UpdateElasticIndexAsync()
		{
			try
			{
				await ElasticIndex.IndexAsync(this._unitOfWork);

				return Json(new { Message = "Индекс Эластика обновлен." });
			}
			catch(Exception ex)
			{
				return Json(new { Message = string.Format("{0}. Текст ошибки: {1}", "При обновлении индекса Эластика произошла ошибка", ex.Message) });
			}
		}
    }
}