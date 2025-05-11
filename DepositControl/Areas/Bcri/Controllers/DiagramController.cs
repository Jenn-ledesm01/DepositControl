using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bcri.Core.Bussines;
using DNF.CodeGenerator;
using DNF.Entity;
using DepositControl.Areas.Bcri.Models;
using DNF.Security.Bussines;

namespace DepositControl.Areas.Bcri.Controllers
{
    public class DiagramController : DynamicRenderController
    {
        // GET: Bcri/Diagram
        public ActionResult Process(string entity)
        {
            ViewBag.Title = "Process";
            ViewBag.Process = entity;
            ViewBag.Processes = ProcessConfig.Dao.GetAll().Where(x => Current.User.HasAccess("Process" + x.Code)).ToListItemCode("");

            return View();
        }

        public ActionResult GetInfo(string entity)
        {
            object wizardProcess = null;

            if (!string.IsNullOrEmpty(entity))
            {
                Business business = new Wizard().GetBusinessLogic(entity);

                List<object> processInputConfigs = new List<object>();
                List<object> processOutputConfigs = new List<object>();

                // gets ProcessInputConfig
                foreach (ProcessInputConfig processInputConfig in business.ProcessInputConfigs)
                {
                    processInputConfigs.Add(new
                    {
                        id = processInputConfig.Id,
                        code = processInputConfig.Code,
                        name = processInputConfig.Name,
                        processConfigId = processInputConfig.ProcessConfig.Id,
                        repositoryConfigId = processInputConfig.RepositoryConfig.Id,
                        repositoryConfigName = processInputConfig.RepositoryConfig.Name
                    });
                }

                // gets ProcessOutputConfig
                foreach (ProcessOutputConfig processOutputConfig in business.ProcessOutputConfigs)
                {
                    processOutputConfigs.Add(new
                    {
                        id = processOutputConfig.Id,
                        code = processOutputConfig.Code,
                        name = processOutputConfig.Name,
                        processConfigId = processOutputConfig.ProcessConfig.Id,
                        repositoryConfigId = processOutputConfig.RepositoryConfig.Id,
                        repositoryConfigName = processOutputConfig.RepositoryConfig.Name
                    });
                }

                wizardProcess = new
                {
                    repositoryConfig = new
                    {
                        id = business.RepositoryConfig.Id,
                        code = business.RepositoryConfig.Code,
                        typeName = business.RepositoryConfig.Type.Name,
                        entityName = business.RepositoryConfig.Entity.Name
                    },
                    processConfig = new
                    {
                        id = business.ProcessConfig.Id,
                        code = business.ProcessConfig.Code,
                        typeId = business.ProcessConfig.Type.Id,
                        typeName = business.ProcessConfig.Type.Name,
                        frequency = business.ProcessConfig.Frequency.Id,
                        className = business.ProcessConfig.Class,
                        autoexecute = business.ProcessConfig.AutoExecute,
                        autoApprove = business.ProcessConfig.AutoApprove
                    },
                    processInputConfig = processInputConfigs,
                    processOutputConfig = processOutputConfigs
                };
                /*
                wizardProcess.RepositoryConfig = business.RepositoryConfig;
                wizardProcess.ProcessConfig = business.ProcessConfig;
                wizardProcess.ProcessInputConfigs = business.ProcessInputConfigs;
                wizardProcess.ProcessOutputConfigs = business.ProcessOutputConfigs;
                */
            }

            return Json(wizardProcess, JsonRequestBehavior.AllowGet);
        }
    }
}
