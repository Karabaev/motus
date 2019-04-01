using SerialService.DAL;
using SerialService.DAL.Entities;
using SerialService.Infrastructure;
using SerialService.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SerialService.Controllers
{
	[ExceptionHandler]
	public class ModeratorToolsController : Controller
    {
        private readonly IVideoMaterialService videoMaterialService;
        private readonly IAppUnitOfWork unitOfWork;

        public ModeratorToolsController(IAppUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            this.videoMaterialService = this.unitOfWork.VideoMaterials;
        }

        public ActionResult Index()
        {
            return View();
        }
        
        [HttpPost]
        public ActionResult Opinion(int id,int status)
        {
            VideoMaterial material = videoMaterialService.GetLoaded(id);
            material.CheckStatus = (CheckStatus)status;
            videoMaterialService.EditMaterial(material);
            string _message = material.CheckStatus == CheckStatus.Confirmed
                ? "одобрено"
                : "отклонено";
            return Json(new { message = _message }, JsonRequestBehavior.AllowGet);
        }

    }
}