using System.Web.Mvc;
using DNF.Security.Bussines;

namespace DepositControl.Areas.Bcri.Controllers
{
     
    public class OutController : Controller
    {
        // GET: Out        
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult AccessDenied(string code)
        {
            return View(Access.Dao.GetByCode(code));
        }
        public ActionResult SessionExpired()
        {
            return View();
        }
        public ActionResult SessionDuplicate()
        {
            return View();
        }
        public ActionResult TabDuplicate()
        {
            return View();
        }
        public ActionResult SessionEnd()
        {
            return View();
        }
    }
}
