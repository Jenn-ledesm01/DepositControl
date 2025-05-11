using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Mvc;
using Bcri.Core.Bussines;
using Bcri.Core.Releaser;
using DepositControl.Areas.Bcri.Utility;
using DNF.Entity;
using DNF.Security.Bussines;
using Type = DNF.Type.Bussines.Type;

namespace DepositControl.Areas.Bcri.Controllers
{
    //valida que el usuario tenga acceso a este process/processConfig , el processCode viene en la url 


    public class ProcessController : Controller
    {
        private void CreatePresentation(ProcessConfig processConfig, DateTime? Period, bool async = false)
        {
            var process = Process.Dao.GetByFilter(new { ProcessConfig_Id = processConfig.Id, Period = Period }).FirstOrDefault();

            //2.1. Si existe si estado !Presentado
            if ((process != null) && (process.Status.Code != "Presented") && processConfig.Regenerate)
            {

                //2.1.1. Eliminar Instancia
                process.Delete();
                LogAccion.Dao.AddLog("ProcessInstanceDelete"
                , process.Config.Name + " " + process.CodeId + ";#;"
                , null
                , process.Config.Name + process.CodeId, process.Config.Id);

            }

            if (process?.Status.Code == "Presented") throw new Exception(Res.res.presentationcreateerror);

            //2.1.2. Crear Instancia y ejecutar
            process = processConfig.Create(Period.Value);
            LogAccion.Dao.AddLog("ProcessInstanceNew"
                , process.Config.Name + " " + process.CodeId + ";#;"
                , null
                , process.Config.Name + process.CodeId, process.Config.Id);

            process.Run(async);
        }

        // GET: Process
        public ActionResult Index()
        {
            ViewBag.Title = Res.res.process;
            ViewBag.tmpProcess = ProcessConfig.Dao.GetAll()
                                                .Where(x => Current.User.HasAccess("Process" + x.Code))
                                                .ToListItemCode("");
            ViewBag.ProcessTypes = new Type("ProcessConfig").AllTypes.ToListItem();

            return View();
        }


        [ProcessAccessCode]
        public ActionResult Processes(string processCode)
        {
            var process = Process.Dao.GetByFilter(new { Code = processCode });
           
            var acceso = SecurityUtility.GetParentAccess(Current.Access);

            LogAccion.Dao.AddLog("ProccessAccess"
                //el usuario X a navegado a Y
                , Current.User.Name + ";#;" + acceso
                );

            if (!string.IsNullOrEmpty(processCode))
            {
                var proccessconfig = ProcessConfig.Dao.GetByCode(processCode);
                if (proccessconfig != null) return View(proccessconfig);
            }

            return View();
        }
      
        [ProcessAccessCode]
        public ActionResult ProcessDetail(int id)
        {
            var process = Process.Dao.Get(id);     
            
            
                 
            LogAccion.Dao.AddLog("ProcessInstanceViewDetail"
                //, "View Details"
                , process.Config.Name + " " + process.CodeId
                , null
                , process.Config.Name
                , process.Id);
            process.Outputs.LoadRelation(x => x.Repository);
            process.Outputs.LoadRelation(x => x.Repository.Config);
            process.Inputs.LoadRelation(x => x.Repository);
            process.Inputs.LoadRelation(x => x.Repository.Config);

            return View(process);
        }

        [ProcessAccessCode]
        public ActionResult Data(long id, long rowid = 0)
        {
            var proccessconfig = ProcessConfig.Dao.Get(id);
            //proccessconfig.Processes;
            List<Process> processes;

            if (rowid != 0)
            {
                processes = new List<Process>() { Process.Dao.Get(rowid) };
            }
            else
            {
                proccessconfig.Processes = null;
                processes = proccessconfig.Processes;
            }

            var data = processes?
                .OrderByDescending(x => x.Period)
                .ThenByDescending(x => x.Version)
                .Select(x => new
                {
                    x.Id,
                    x.Period,
                    Version = "v" + x.Version.ToString(),
                    x.Date,
                    x.Error,
                    State = x.Status.Name

                });

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ProcessAccessCode("ChangeState")]
        public ActionResult Approve(int id)
        {
            var process = Process.Dao.Get(id);
            process.ChangeStatus("Approve");
            process.Save();
            LogAccion.Dao.AddLog("ProcessInstanceChangeState"
                     , process.Config.Name + " " + process.CodeId + ";#;" + @Res.res.approve
                     , null
                     , process.Config.Name + process.Period, process.Config.Id);
            return Data(0, id);
        }
        public ActionResult Presented(int id)
        {
            var process = Process.Dao.Get(id);
            process.ChangeStatus("Presented");
            process.Save();
            LogAccion.Dao.AddLog("ProcessInstanceChangeState"
                     , $"{process.Config.Name} {process.CodeId} ;#; {Res.res.presented}"
                     , null
                     , process.Config.Name + process.Period, process.Config.Id);
            return Data(0, id);
        }
        [ProcessAccessCode("ChangeState")]
        public ActionResult Refuse(int id)
        {
            var process = Process.Dao.Get(id);
            process.ChangeStatus("Rejected");
            process.Save();
            LogAccion.Dao.AddLog("ProcessInstanceChangeState"
                     , process.Config.Name + " " + process.CodeId + ";#;" + @Res.res.rejected
                     , null
                     , process.Config.Name + process.Period, process.Config.Id);
            return Data(0, id);
        }

        public ActionResult New()
        {
            return View();
        }

        [HttpPost]
        [ProcessAccessCode("New")]
        public async Task<ActionResult> AddMulti(string processCode, DateTime? Period, DateTime? PeriodFrom, DateTime? PeriodTo, bool refresh = false)
        {
            //1. Buscar configuración de Proceso
            var proccessConfig = ProcessConfig.Dao.GetByCode(processCode);

            Process process = null;
            //PROCESS IMPORT Y PRESENTATION MULTI
            if (PeriodFrom.HasValue && PeriodTo.HasValue)
            {
                try
                {
                    List<Process> preocesss = proccessConfig.Create(PeriodFrom.Value, PeriodTo.Value, refresh);
                    if (preocesss != null)
                    {
                        LogAccion.Dao.AddLog("ProcessInstanceNew"
                          , proccessConfig.Name + " " + PeriodFrom.Value.ToString("dd/MM/yyyy") + " - " + PeriodTo.Value.ToString("dd/MM/yyyy") + ";#;"
                          , null
                          , proccessConfig.Name + PeriodFrom.Value.ToString("dd/MM/yyyy") + "-" + PeriodTo.Value.ToString("dd/MM/yyyy"));
                    }
                }
                catch (Exception ex)
                {
                    HttpContext.Response.StatusCode = 500;
                    HttpContext.Response.SubStatusCode = 100;
                    return Content(ex.Message);
                }

            }
            else if (Period.HasValue)
            {
                //IMPORT UNITARY
                if (!proccessConfig.Regenerate && proccessConfig.Type.Code != "Presentation")
                {

                    try
                    {
                        process = proccessConfig.Create(Period.Value);
                        LogAccion.Dao.AddLog("ProcessInstanceNew"
                          , process.Config.Name + " " + process.CodeId + ";#;"
                          , null
                          , process.Config.Name + process.CodeId, process.Config.Id);

                        if (process == null)
                            return Content("");

                        await Task.Run(() => process.Run(true));
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
                    //PRESENTATION UNITARY
                    //2. Buscar si existe instancia de Proceso para el período indicado
                    try
                    {
                        CreatePresentation(proccessConfig, Period, true);
                    }
                    catch (Exception ex)
                    {
                        HttpContext.Response.StatusCode = 500;
                        HttpContext.Response.SubStatusCode = 100;
                        return Content(ex.Message);
                    }
                }
            }
            return Content("");

        }

        [HttpPost]
        [ProcessAccessCode("New")]
        public async Task<ActionResult> Add(string oper, DateTime? period, string processCode = null, long id = 0)
        {
            if (oper == "add")
            {
                var proccessConfig = ProcessConfig.Dao.GetByCode(processCode);
                Process process = null;
                try
                {
                    process = proccessConfig.Create(Convert.ToDateTime(period));
                    
                }
                catch (Exception ex)
                {
                    HttpContext.Response.StatusCode = 500;
                    HttpContext.Response.SubStatusCode = 100;
                    return Content(ex.Message);
                }
                try
                {
                    await Task.Run(() => process.Run(true));
                    LogAccion.Dao.AddLog("ProcessInstanceNew"
                       , process.Config.Name + " " + process.CodeId + ";#;"
                       , null
                       , process.Config.Name + process.CodeId, process.Config.Id);
                }
                catch (Exception ex)
                {
                    HttpContext.Response.StatusCode = 500;
                    HttpContext.Response.SubStatusCode = 100;
                    return Content("unexpected Error during process execution: " + ex.Message);
                }
                return Data(proccessConfig.Id, process.Id);
            }
            else
            {
                var process = Process.Dao.Get(id);
                LogAccion.Dao.AddLog("ProcessInstanceDelete"
                  , process.Config.Name + " " + process.CodeId + ";#;" + @Res.res.delete
                  , null
                  , process.Config.Name + process.CodeId, process.Config.Id);

                if (process.Config.Type.Code == "Presentation" && process.Status.Code == "Presented")
                {
                    throw new Exception(Res.res.presentationdeleteerror);
                }

                process.Delete();

                return Json(new { rowId = process.Id }, JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult Export(int id) 
            => new ExportResult(ExportFormat.Compress, new ProcessExport(id));

        public ActionResult Reprocess(int id)
        {
            var process = Process.Dao.Get(id);
            return Data(0, id);
        }
        public ActionResult Delete(int id)
        {
            var process = Process.Dao.Get(id);
            process.Delete();
           
            return Json(new { rowId = process.Id }, JsonRequestBehavior.AllowGet);
        }


        [ProcessAccessCode]
        public ActionResult RunIfNotPresented(string processCode, DateTime Period)
        {
            try
            {
                CreatePresentation(ProcessConfig.Dao.GetByCode(processCode), Period, false);
            }
            catch (Exception ex)
            {
                if (Res.res.presentationcreateerror != ex.Message)
                {
                    HttpContext.Response.StatusCode = 500;
                    HttpContext.Response.SubStatusCode = 100;
                    return Content(ex.Message);
                }
            }
            return new EmptyResult();
        }
    }
}
