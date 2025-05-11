using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bcri.Core.Bussines;
using DepositControl;
using DNF.Entity;
using DepositControl.Areas.Bcri.Controllers;
using DNF.Structure.Bussines;
using DNF.Type.Bussines;
using DepositControl.Areas.Bcri.Utility;
using Type = DNF.Type.Bussines.Type;
using DNF.Security.Bussines;
using System.IO;
using System.Text;

namespace DepositControl.Areas.Bcri.Controllers
{
    public class ProcessInputConfigController : Controller
    {
        public ActionResult Index(int processConfigId)
        {
            return View(ProcessConfig.Dao.Get(processConfigId));
        }
        public ActionResult ProcessInputData(int ProcessConfigId)
        {
            var pc = ProcessConfig.Dao.Get(ProcessConfigId);
            RepositoryConfig repc = new RepositoryConfig();
            pc.InputConfigs = null; //force Refesh
            var ProcessInputData = pc.InputConfigs.Where(x => x.RepositoryConfig.Type.Code == "Periodic")
            .Select(x => new
            {
                x.Id,
                ProcessConfig = x.ProcessConfig.Id,
                RepositoryConfig = x.RepositoryConfig.Code,
                FrecuencySimpleInput = x.FrequencySimpleInput?.Id,
                Visibility = x.Visibility.Id,
                x.FrequencyAll,
                Frequency = x.Frequency?.Id,
                x.FrequencyIncrease,
                x.Deleted,
                x.Code,
                RelatedProcess = x.RepositoryConfig.OutputConfigs.Where(cfg => cfg.Id != ProcessConfigId).Select(i => new { i.ProcessConfig.Code, i.ProcessConfig.Name })
            });
            return Json(ProcessInputData, JsonRequestBehavior.AllowGet);
        }

        [ProcessAccessCode("Configuration")]
        public ActionResult ProcessInputConfigEdit(string oper, long id = 0)
        {
            ProcessInputConfig NewPic = new ProcessInputConfig();

            if (oper == "edit" || oper == "del")
            {
                NewPic = ProcessInputConfig.Dao.Get(id) ?? new ProcessInputConfig();
            }
            if (oper == "del")
            {
                try
                {
                    NewPic.Delete();
                    LogAccion.Dao.AddLog("ProcessConfigSave"
                     , Res.res.processinputconfigdelete + " " + NewPic.Code + " " + Res.res.toprocess + " " + NewPic.ProcessConfig.Name + ";#;"
                     , null
                     , NewPic.ProcessConfig.Name + NewPic.ProcessConfig.Id);
                }
                catch (Exception ex)
                {
                    HttpContext.Response.StatusCode = 500;
                    HttpContext.Response.SubStatusCode = 100;
                    return Content(ex.Message);
                }
            }
            else
            {
                NewPic.Code = Request.Form["Code"];
                NewPic.ProcessConfig =
                    ProcessConfig.Dao.Get(NewPic.ProcessConfig?.Id ?? Convert.ToInt64(Request.Form["ProcessConfigId"]));
                NewPic.RepositoryConfig = RepositoryConfig.Dao.GetByCode(Request.Form["RepositoryConfig"]);

                if (Request.Form["FrecuencySimpleInput"] != "")
                {
                    NewPic.FrequencySimpleInput =
                        FrequencySimpleInput.Dao.Get(long.Parse(Request.Form["FrecuencySimpleInput"]));
                    NewPic.Frequency = NewPic.FrequencySimpleInput.Frequency;
                    NewPic.FrequencyIncrease = NewPic.FrequencySimpleInput.FrequencyIncrease;
                    NewPic.FrequencyAll = NewPic.FrequencySimpleInput.FrequencyAll;

                }
                NewPic.Visibility = Type.Dao.Get(Convert.ToInt64(Request.Form["Visibility"]));
                NewPic.Deleted = false;

                try
                {
                    NewPic.Save();
                    LogAccion.Dao.AddLog("ProcessConfigSave"
                     , @Res.res.processinputconfigadd + " " + NewPic.Code + " " + @Res.res.toprocess + " " + NewPic.ProcessConfig.Name + ";#;"
                     , null
                     , NewPic.ProcessConfig.Name + NewPic.ProcessConfig.Id);
                }
                catch (Exception ex)
                {
                    HttpContext.Response.StatusCode = 500;
                    HttpContext.Response.SubStatusCode = 100;
                    return Content(ex.Message);
                }

            }
           
            return Json(new { ok = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetOptionsData()
        {
            var OptionsData = new
            {
                //respositoryConfig = RepositoryConfig.Dao.GetAllPeriodic().ToListItemJqGridString(" ", ""),
                respositoryConfig = ": ;" + RepositoryConfig.Dao.GetAllPeriodic().ToListItemCodeJqGridString(),
                visibilityConfig = TypeConfig.Dao.GetByCode("Visibility").Types.ToListItemJqGridString(),
                frecuencySimpleInput = FrequencySimpleInput.Dao.GetAll().ToListItemJqGridString()
            };
            return Json(OptionsData, JsonRequestBehavior.AllowGet);
        }

        public void DownloadCustomInputConfig(string ProcessInputConfigCode)
        {
            var content = $@"using Bcri.Core.Bussines;
using Bcri.Core.Frequency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace DepositControl.ProcessInputFrequency
{{
    public class {ProcessInputConfigCode} : ICustomPeriodicity
    {{
        public override List<DateTime> CalculateFrequency(DateTime Period, Periodicity OriginalFrequency)
        {{
        }}
    }}
}}";

            Response.ContentType = "text/plain";

            Response.AddHeader("content-disposition", "attachment;filename=" + ProcessInputConfigCode + ".cs");
            Response.Clear();
            var oString = new StringWriter();
            oString.Write(content);

            using (StreamWriter writer = new StreamWriter(Response.OutputStream, Encoding.UTF8))
            {
                writer.Write(oString.ToString());
            }
            Response.End();
        }
    }
}
