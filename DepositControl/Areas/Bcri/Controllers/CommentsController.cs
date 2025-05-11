/** @pp
 * rootnamespace: DepositControl
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Bcri.Core.Bussines;
using DNF.Entity;
using DNF.Security.Bussines;

namespace DepositControl.Areas.Bcri.Controllers
{

    [Authenticated]
    public class CommentsController : Controller
    {
        /*
            Returns the complete view(Page + comments + space to enter a new comment)
        */
        [ProcessAccessCode("Comment")]
        public  ActionResult List(int processId)
        {
            return View("List", GetFullComments(processId));
        }

        /*
            Fills the comment with every user and organize it by Date
        */
        [ProcessAccessCode("Comment")]//valida que el usuario tenga acceso a los comentarios en este process/processConfig , el processId viene como parametro en cada request
        public List<Comment> GetFullComments(int processId)
        {
            var comments = Comment.Dao.GetBy(new Process {Id = processId });
            comments.LoadRelation(x => x.User);
            comments = comments.OrderBy(x => x.Date).ToList();
            return comments;
        }

        public ActionResult Comments(int processId)
        {
            return View(new Process { Id = processId });
        }

        [HttpPost]
        [ProcessAccessCode("Comment")]
        public ActionResult Comments ( string message, int processId)
        {  
             if(string.IsNullOrWhiteSpace(message))
            {
                @ViewData["error"] = "Please write a comment";
                return Json(new {error = @ViewData["error"]}, JsonRequestBehavior.AllowGet);
            }
             if (message.Length > 10000)
            {
                @ViewData["error"] = "Your text is over the 10000 characters";
                return Json(new { error = @ViewData["error"] }, JsonRequestBehavior.AllowGet);
            }
            var lastComment = Comment.Dao.GetLast();
            if (lastComment == null || lastComment.Message != message)
            {
                //User user = DNF.Security.Bussines.User.Dao.GetByName(email);

                Process process = Process.Dao.Get(processId);
                var com = new Comment
                {
                    Date = DateTime.Now,
                    Message = message,
                    Process = process,
                    User = DNF.Security.Bussines.Current.User

                };
                com.Save();

                var sortmessage = message.Length > 23 ? message.Substring(0, 20) + "..." : message;
                LogAccion.Dao.AddLog("ProcessComment"
                    , sortmessage
                    , null 
                    , process.Config.Name + " " + process.CodeId
                    , process.Id);

            }
            return Json(new { ok = true }, JsonRequestBehavior.AllowGet);
        }
    }
}
