using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Bcri.Core.Bussines;
using DNF.Security.Bussines;
using Type = DNF.Type.Bussines.Type;

namespace DepositControl.Areas.Bcri.Controllers
{
    [Authenticated]
    public class ConfigController : Controller
    {
        // GET: Config
        [ProcessAccessCode("Configuration")]
        public ActionResult Index(string processCode)
        {


            var process = ProcessConfig.Dao.GetByCode(processCode);

            ViewBag.HolyDayss = new Type("HolyDaysBehavior").AllTypes.ToListItem();
            ViewBag.Frequencys = new Type("Frequency").AllTypes.ToListItem();
            ViewBag.ProcessTypes = new Type("ProcessConfig").AllTypes.ToListItem();
            ViewBag.ExecuteDelayDayss = new List<SelectListItem>
                                         {
                                             new SelectListItem{Value = "0",Text = "0 Days"},
                                             new SelectListItem{Value = "1",Text = "1 Day", Selected = true},
                                             new SelectListItem{Value = "2",Text = "2 Days"},
                                             new SelectListItem{Value = "4",Text = "4 Days"},
                                             new SelectListItem{Value = "7",Text = "7 Days"}
                                         };
            ViewBag.Parametrics = RepositoryConfig.Dao.GetBy(new Type("RepositoryConfig", "Parametric")).ToListItem("");
            ViewBag.Periodics = RepositoryConfig.Dao.GetBy(new Type("RepositoryConfig", "Periodic")).ToListItem("");

            return View(process);
        }


        public ActionResult CreateConfig(string Name, string Code, int Type_Id)
        {

            var config = new ProcessConfig
            {
                Name = Name,
                Code = Code,
                Type = Type.Dao.Get(Type_Id),
                Frequency = new Type("Frequency", "Daily"),
                HolyDays = new Type("HolyDaysBehavior", "Run"),
                Class= "Bcri.Core.ProcessUnit.Multi"
            };
            try
            {
                config.Save();
                LogAccion.Dao.AddLog("ProcessNew"
                      , config.Name + ";#;"
                      , null
                      , config.Name + config.Id);
            }
            catch (Exception ex)
            {
                HttpContext.Response.StatusCode = 500;
                HttpContext.Response.SubStatusCode = 100;
                return Content(ex.Message);
            }
            return Content(config.Code);
        }



        [HttpPost]
        [ProcessAccessCode("Configuration")]
        public ActionResult Save(ProcessConfig config)
        {

            if (config.AutoExecute && !string.IsNullOrEmpty(Request.Form["ExecuteOnTime"]))
            {
                var hm = Request.Form["ExecuteOnTime"]
                                            .Split(char.Parse(":"))
                                            .Select(int.Parse)
                                            .ToArray();
                config.ExecuteOnHour = hm[0];
                config.ExecuteOnMinute = hm[1];
            }

            ProcessConfig OrigConfig = config.IsNew()
                                            ? new ProcessConfig()
                                            : ProcessConfig.Dao.Get(config.Id);
            OrigConfig.Save(config);

          
           
            LogAccion.Dao.AddLog("ProcessConfigSave"
                    , OrigConfig.Name + ";#;"
                    , null
                    , OrigConfig.Name + OrigConfig.Id);
            return Json(new { ok = true }, JsonRequestBehavior.AllowGet);
        }
    }
}
