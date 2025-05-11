using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bcri.Core.Bussines;
using DNF.Elastic.Enviroment;
using Type = DNF.Type.Bussines.Type;

namespace DepositControl.Areas.Bcri.Controllers
{
    public class SpProcessController : Controller
    {
        // GET: Bcri/SpProcess
        public ActionResult Index(int processConfigId)
        {
            var process = ProcessConfig.Dao.Get(processConfigId);

            var sp = SpProcess.Dao.GetBy(process);
            return View(sp.Count > 0 ? sp[0] : new SpProcess() { ProcessConfig = new ProcessConfig() { Id = processConfigId } });

        }

        [HttpPost]
        public ActionResult ReCreate(SpProcess spProcess)
        {
            try
            {
                SpProcess sp = spProcess.IsNew() ? new SpProcess() : SpProcess.Dao.Get(spProcess.Id);
                sp.ProcessConfig = ProcessConfig.Dao.Get(spProcess.ProcessConfig.Id);
                return Content(sp.GetUpdateContent());
            }
            catch (Exception ex)
            {
                HttpContext.Response.StatusCode = 500;
                HttpContext.Response.SubStatusCode = 100;
                return Content(ex.Message);
            }
        }
        [HttpPost]
        public ActionResult Apply(SpProcess spProcess)
        {
            try
            {
                DNF.Enviroment.Current.Entorno.ExecuteNonQuery(spProcess.Content);
                return Json(new { ok = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                HttpContext.Response.StatusCode = 500;
                HttpContext.Response.SubStatusCode = 100;
                return Content(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult Save(SpProcess spProcess)
        {
            try
            {
                var spfinal = spProcess.IsNew()
                    ? new SpProcess()
                    : SpProcess.Dao.Get(spProcess.Id);

                spfinal.Content = spProcess.Content;
                spfinal.ProcessConfig = spProcess.ProcessConfig;

                spfinal.Save();

                var pConfig = ProcessConfig.Dao.Get(spfinal.ProcessConfig.Id);

                if (pConfig.InputConfigs.All(x => x.RepositoryConfig.Code != "SpProcess"))
                {
                    // si no tiene el bcp como imput se lo agrego para la trazabilidad
                    var input = new ProcessInputConfig
                    {
                        ProcessConfig = pConfig,
                        RepositoryConfig = RepositoryConfig.Dao.GetByCode("SpProcess"),
                        Code = "SpProcess",
                        Visibility = new Type("Visibility", "Normal")
                    };
                    input.Save();
                    pConfig.InputConfigs.Add(input);
                }

                return Json(new { ok = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                HttpContext.Response.StatusCode = 500;
                HttpContext.Response.SubStatusCode = 100;
                return Content(ex.Message);
            }
        }
    }
}
