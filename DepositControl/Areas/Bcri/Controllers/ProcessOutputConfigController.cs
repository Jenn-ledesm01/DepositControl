using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bcri.Core.Bussines;
using com.sun.tools.@internal.xjc.reader.gbind;

using DNF.Entity;
using DepositControl.Areas.Bcri.Controllers;
using DNF.Structure.Bussines;
using DNF.Type.Bussines;
using DepositControl.Areas.Bcri.Utility;
using Type = DNF.Type.Bussines.Type;
using DNF.Security.Bussines;

namespace DepositControl.Areas.Bcri.Controllers
{
    public class ProcessOutputConfigController : Controller
    {
        // GET: ProcessInputCofing
        public ActionResult Index(int processConfigId)
        {
            return View(ProcessConfig.Dao.Get(processConfigId));
        }

        public ActionResult ProcessOutputData(int ProcessConfigId)
        {
            var pc = ProcessConfig.Dao.Get(ProcessConfigId);
            pc.OutputConfigs = null; //force Refesh
            var ProcessOutputData = pc.OutputConfigs
                .Select(x => new
                {
                    x.Id,
                    RepositoryConfig = x.RepositoryConfig.Id,
                    x.Deleted,
                    x.Name,
                    x.Code,
                    ProcessConfig = x.ProcessConfig.Id,
                    Visibility =  x.Visibility.Id,
                    RelatedProcess = x.RepositoryConfig.InputConfigs.Where(cfg => cfg.Id != ProcessConfigId).Select(i => new { i.ProcessConfig.Code, i.ProcessConfig.Name })
                });
            return Json(ProcessOutputData, JsonRequestBehavior.AllowGet);

           //var ob  =  new
           //{
           //    respositorys = RepositoryConfig.Dao.GetAll().Select(x=> x.Id.ToString() + ":" +x.Name).ToJoin(";"),
           //    FrecuencyType = RepositoryConfig.Dao.GetAll().Select(x => x.Id.ToString() + ":" + x.Name).ToJoin(";")

           //}
        }
        [ProcessAccessCode("Configuration")]

        public ActionResult ProcessOutputConfigEdit(string oper,long id = 0)
        {
            ProcessOutputConfig NewPoc = (oper == "edit" || oper == "del") 
                                            ? ProcessOutputConfig.Dao.Get(id) ?? new ProcessOutputConfig()
                                            : new ProcessOutputConfig();

            if (oper == "del")
            {
                try
                {
                    NewPoc.Delete();
                    LogAccion.Dao.AddLog("ProcessConfigSave"
                     , @Res.res.processoutputconfigdelete + " " + NewPoc.Code + " " + @Res.res.toprocess + " " + NewPoc.ProcessConfig.Name + ";#;"
                     , null
                     , NewPoc.ProcessConfig.Name + NewPoc.ProcessConfig.Id);
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
                //segun repository config alta de la baja logica 
                NewPoc.Deleted = false;
                NewPoc.ProcessConfig = ProcessConfig.Dao.Get(Convert.ToInt64(Request.Form["ProcessConfigId"]));
                NewPoc.RepositoryConfig = RepositoryConfig.Dao.Get(Convert.ToInt64(Request.Form["RepositoryConfig"]));
                NewPoc.Visibility = Type.Dao.Get(Convert.ToInt64(Request.Form["Visibility"]));
                try
                {
                    NewPoc.Save(); // aca hace algunas validaciones
                    LogAccion.Dao.AddLog("ProcessConfigSave"
                  , @Res.res.processoutputconfigadd + " " + NewPoc.Code + " " + @Res.res.toprocess + " " + NewPoc.ProcessConfig.Name + ";#;"
                  , null
                  , NewPoc.ProcessConfig.Name + NewPoc.ProcessConfig.Id);
                }
                catch (Exception ex) // si no pasa las validaciones tira error
                {
                    HttpContext.Response.StatusCode = 500;
                    HttpContext.Response.SubStatusCode = 100;
                    return Content(ex.Message);
                }

            }
            
            return Json(new { ok = true }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult  GetOptionsData()
        {
            var OptionsData = new
            {
                respositoryConfig =  RepositoryConfig.Dao.GetAllPeriodic().ToListItemJqGridString(" ",""),
                visibilityConfig = TypeConfig.Dao.GetByCode("Visibility").Types.ToListItemJqGridString()
            };
            return Json(OptionsData, JsonRequestBehavior.AllowGet);
        }

    }
}


