namespace SerialService.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Threading.Tasks;
    using Microsoft.AspNet.Identity;
    using DAL;
    using DAL.Entities;
    using Infrastructure.Exceptions;
    using ViewModels.User;
    using ViewModels;
    using AutoMapper;
    using NLog;

    public class CommentsController : Controller
    {
        public CommentsController(IAppUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public ActionResult GetComments(int videoMaterialId)
        {
            IEnumerable<ShowCommentViewModel> comments = Mapper.Map<IEnumerable<Comment>, IEnumerable<ShowCommentViewModel>>(this.unitOfWork.Comments.GetWithCondition(c => c.VideoMaterialID == videoMaterialId));
            return PartialView("../User/DetailPage/CommentContainerPartial", comments);
        }

        [HttpPost, Authorize]
        public JsonResult AddComment(AddCommentViewModel model)
        {
            if (!HttpContext.User.Identity.IsAuthenticated)
                return Json(new { error = "Чтобы оставить комментарий, необходимо авторизоваться." });

            if (!ModelState.IsValid)
                return Json(new { error = "Ошибка валидации комментария." });

            Comment comment = Mapper.Map<AddCommentViewModel, Comment>(model);
            comment.AuthorID = HttpContext.User.Identity.GetUserId();
            comment.AddDateTime = DateTime.Now;

            try
            {
                if (this.unitOfWork.Comments.Create(comment))
                {
                    var author = this.unitOfWork.Users.Get(comment.AuthorID);
                    Comment parent = null;

                    if (comment.ParentID.HasValue)
                        parent = this.unitOfWork.Comments.Get(comment.ParentID);

                    return Json(new { success = "Комментарий добавлен." });
                }
                else
                {
                    Task.Run(() => this.logger.Error("Не удалось сохранить комментарий"));
                    return Json(new { error = "Не удалось сохранить комментарий. Обратитесь в поддержку сайта." });
                }
            }
            catch (EntryAlreadyExistsException ex)
            {
                Task.Run(() => this.logger.Error(ex));
                return Json(new { error = "Вы уже оставляли коментарий с таким текстом в данном видеоматериале." });
            }
            catch (Exception ex)
            {
                Task.Run(() => this.logger.Error(ex));
                return Json(new { error = "Не удалось сохранить комментарий. Обратитесь в поддержку сайта." });
            }
        }

        [HttpPost, Authorize]
        public JsonResult RemoveComment(RemoveCommentViewModel model)
        {
            if (!ModelState.IsValid)
                return Json(new { error = "Не указан идентификатор." });

            Comment comment = this.unitOfWork.Comments.Get(model.CommentID);

            if (comment == null)
                return Json(new { error = "Комментарий не найден." });

            if (comment.AuthorID != User.Identity.GetUserId())
                return Json(new { error = "Нельзя удалить не свой комментарий." });

            if (this.unitOfWork.Comments.Remove(comment))
                return Json(new { success = "Комментарий удален." });
            else
                return Json(new { error = "Не удалось удалить комментарий." });
        }

        [HttpPost, Authorize]
        public JsonResult EditComment(EditCommentViewModel model)
        {
            if (!ModelState.IsValid)
                return Json(new { error = "Идентификатор или текст не указаны." });

            try
            {
                if (this.unitOfWork.Comments.EditText(model.CommentID, model.NewText, User.Identity.GetUserId()))
                    return Json(new { success = model });
                else
                    return Json(new { error = "Не удалось изменить комментарий." });
            }
            catch (EntryAlreadyExistsException ex)
            {
                return Json(new { error = "Вы уже оставляли коментарий с таким текстом в данном видеоматериале." });
            }
            catch (EntryNotFoundException ex)
            {
                return Json(new { error = ex.Message });
            }
            catch (AccessDeniedException ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult VoteForComment(VoteForCommentViewModel model)
        {
            if (!ModelState.IsValid)
                return Json(new { error = "Неверные данные." });

            CommentMark mark = new CommentMark
            {
                CommentID = model.CommentID,
                Value = model.Value,
                UserIP = HttpContext.Request.UserHostAddress,
                AuthorID = User.Identity.GetUserId()
            };
            CountMarksViewModel countMarks = null;

            try
            {
                if (this.unitOfWork.CommentMarks.Create(mark))
                {
                    this.unitOfWork.Comments.AddVote(mark.CommentID, mark.Value);
                    Comment comment = this.unitOfWork.Comments.Get(mark.CommentID);
                    countMarks = new CountMarksViewModel
                    {
                        NegativeMarkCount = comment.NegativeVoteCount,
                        PositiveMarkCount = comment.PositiveVoteCount
                    };
                    return Json(new { success = countMarks });
                }
                else
                {
                    return Json(new { error = "Ошибка записи." });
                }
            }
            catch (EntryAlreadyExistsException ex)
            {
                CommentMark cache = this.unitOfWork.CommentMarks.GetScalarWithCondition(
                    cm => (cm.CommentID == mark.CommentID && cm.AuthorID == mark.AuthorID && cm.Value == mark.Value)
                    || (cm.CommentID == mark.CommentID && cm.UserIP == mark.UserIP && cm.Value == mark.Value));

                if (cache != null)
                {
                    if (this.unitOfWork.CommentMarks.Remove(cache))
                    {
                        this.unitOfWork.Comments.RemoveVote(mark.CommentID, mark.Value);
                        Comment comment = this.unitOfWork.Comments.Get(mark.CommentID);
                        countMarks = new CountMarksViewModel
                        {
                            NegativeMarkCount = comment.NegativeVoteCount,
                            PositiveMarkCount = comment.PositiveVoteCount
                        };
                        return Json(new { success = countMarks });
                    }
                    else
                    {
                        return Json(new { error = "Ошибка записи." });
                    }
                }

                cache = this.unitOfWork.CommentMarks.GetScalarWithCondition(
                    cm => (cm.CommentID == mark.CommentID && cm.AuthorID == mark.AuthorID)
                    || (cm.CommentID == mark.CommentID && cm.UserIP == mark.UserIP));

                if (cache != null)
                {
                    if (this.unitOfWork.CommentMarks.InvertValue(cache))
                    {
                        this.unitOfWork.Comments.InvertVote(cache.CommentID, !mark.Value);
                        Comment comment = this.unitOfWork.Comments.Get(mark.CommentID);
                        countMarks = new CountMarksViewModel
                        {
                            NegativeMarkCount = comment.NegativeVoteCount,
                            PositiveMarkCount = comment.PositiveVoteCount
                        };
                        return Json(new { success = countMarks });
                    }
                    else
                    {
                        return Json(new { error = "Ошибка записи." });
                    }
                }
            }

            return Json(new { error = "Ошибка записи." });
        }

        private readonly IAppUnitOfWork unitOfWork;
        private readonly Logger logger = LogManager.GetCurrentClassLogger();
    }
}