using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bcri.Core.Bussines;
using DepositControl;
using com.sun.tools.@internal.xjc.reader.gbind;
using DNF.Entity;
using DepositControl.Areas.Bcri.Controllers;
using DNF.Structure.Bussines;
using DNF.Type.Bussines;
using java.awt.font;
using DepositControl.Areas.Bcri.Utility;
using com.sun.org.apache.bcel.@internal.generic;
using Type = DNF.Type.Bussines.Type;
using DNF.Security.Bussines;

namespace DepositControl.Areas.Bcri.Controllers
{

    public class ParametricInputConfigController : Controller
    {
        public ActionResult Index(int processConfigId)
        {
            return View(ProcessConfig.Dao.Get(processConfigId));
        }
        public ActionResult ProcessInputData(int ProcessConfigId)
        {
            var pc = ProcessConfig.Dao.Get(ProcessConfigId);
            pc.InputConfigs = null; //force Refesh
            var ProcessInputData = pc.InputConfigs.Where(x=> x.RepositoryConfig.Type.Code == "Parametric")
            .Select(x => new
            {
                x.Id,
                ProcessConfig = x.ProcessConfig.Id,
                RepositoryConfig = x.RepositoryConfig.Id,
                FrecuencySimpleInput = x.FrequencySimpleInput?.Id,
                Visibility = x.Visibility.Id,
                x.FrequencyAll,
                Frequency = x.Frequency?.Id,
                x.FrequencyIncrease,
                x.Deleted,
                x.Code
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
                   , @Res.res.parametricinputconfigdelete + " " + NewPic.Code + " " + @Res.res.toprocess + " " + NewPic.ProcessConfig.Name + ";#;"
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
                NewPic.RepositoryConfig = RepositoryConfig.Dao.Get(Convert.ToInt64(Request.Form["RepositoryConfig"]));
                NewPic.Visibility = Type.Dao.Get(Convert.ToInt64(Request.Form["Visibility"]));
                NewPic.Deleted = false;

                try
                {
                    NewPic.Save();
                    LogAccion.Dao.AddLog("ProcessConfigSave"
                    , @Res.res.parametricinputconfigadd + " " + NewPic.Code + " " + @Res.res.toprocess + " " + NewPic.ProcessConfig.Name + ";#;"
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
                respositoryConfig = RepositoryConfig.Dao.GetAllParametric().ToListItemJqGridString(" ", ""),
                visibilityConfig = TypeConfig.Dao.GetByCode("Visibility").Types.ToListItemJqGridString(),
                frecuencySimpleInput = FrequencySimpleInput.Dao.GetAll().ToListItemJqGridString(" ", "")
            };
            return Json(OptionsData, JsonRequestBehavior.AllowGet);
        }


    }
}
