
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DNF.Multilanguaje;


namespace DepositControl.Areas.Bcri.Controllers
{
    public class ResourcesController : Controller
    {
        // GET: Bcri/Generator
        public ActionResult Index()
        {
            return View();
        }
        //[HttpGet]
        public ActionResult GenerateResources()
        {
            var builder = new ResourceBuilder();
            string filepath = builder.Create(new DBResourcesProvider());
            return Json(filepath, JsonRequestBehavior.AllowGet);
        }
    }
}
