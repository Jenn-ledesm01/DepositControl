using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Bcri.Core.Bussines;
using Bcri.Core.Releaser;
using DepositControl.Areas.Bcri.Utility;
using DNF.Entity;
using DNF.Security.Bussines;

namespace DepositControl.Areas.Bcri.Controllers
{
    public class SimulationController : Controller
    {
        // GET: Bcri/Simlation
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetAll()
        {

            var simulaciones = Simulation.Dao.GetAll();
            simulaciones.LoadRelation(x => x.Owner);
            simulaciones.ForEach(x=> x.Users = null);

            return Json(simulaciones.Select(x => new
            {
                x.Id,
                x.Name,
                Status = x.Status.Name ,
                CreationDate = x.CreationDate.ToShortDateString(),
                Owner = x.Owner.Name,
                Users = x.Users.Select(u => u.Name).ToJoin(),
                YouIn = x.Users.Any(u => u.Id == Current.User.Id),
                IsReady = x.Status.Code == "Ready"
            }).ToArray(), JsonRequestBehavior.AllowGet);

        }

        public JsonResult Create(string name, string SimulationData)
        {
            SimulationConfig.Dao.Get(1).CreateSimulation(name, (SimulationData == "Empty"));
        
            return Json(new { ok = true }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Join(long id)
        {
            Simulation.Dao.Get(id)?.Join();
            return Json(new { ok = true }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Left(long id)
        {
            Simulation.Dao.Get(id)?.Left();
            return Json(new { ok = true }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult Export(int id)
            => new ExportResult(ExportFormat.Compress, new SimulationExport(id));

        public ActionResult Delete(long id)
        {
            Simulation.Dao.Get(id)?.Delete();
            return Json(new { ok = true }, JsonRequestBehavior.AllowGet);

        }

    }
}
