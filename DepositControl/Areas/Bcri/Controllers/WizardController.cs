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
using System.Text;

namespace DepositControl.Areas.Bcri.Controllers
{
    
    public class WizardController : DynamicRenderController
    {
        // GET: Bcri/Generator
        public ActionResult Index()
        {
            //todo agregar pagina explicando como usar el controller o pagina que genere las cosa scon menu con las tablas etc 
            return Content($"<div class='alert alert-success'><a class='close' data-dismiss='alert'></a><strong style='width:12px'>Thanks!</strong>nothing to do</div>");
        }

        public ActionResult AllRepoSp()
        {
            var repos = RepositoryConfig.Dao.GetAll();
            repos.LoadRelation(x => x.Entity);
            var entities = repos.Select(x => x.Entity).ToList();
            entities.ForEach(x => new DbSqlBcri { Entity = x }.RunScript());
            return Content($"<div class='alert alert-success'><a class='close' data-dismiss='alert'></a><strong style='width:12px'>Thanks!</strong> Completed successfully!</div>");
        }

        public ActionResult Entity(string entity = "")
        {
             var mcg = new Generator
            {
                //DB = new DbSqlBcri(),
                DB = new DbSqlBcri(),
                Dao = new DaoFrameW(),
                RunScript = true,
                Using = string.Empty,
                NameSpace = "DepositControl",
                Abm = null,
                Path = @"C:\TEMP"
            };

                   
            if(!string.IsNullOrEmpty(entity))
                mcg.AddTable(entity);

            mcg.Generate();

            var ent = DNF.Structure.Bussines.Entity.Dao.GetCurrentByName(entity);


            return Content($"<div class='alert alert-success'><a class='close' data-dismiss='alert'>&times;</a><strong style='width:12px'>Thanks!</strong> Entity {entity} generated successfully!</div>");

        }

        public ActionResult CreateAccess(string entity)
        {
            ProcessConfig pc = ProcessConfig.Dao.GetByCode(entity);
            new Wizard().CreateAccess(pc);
            return Content($"<div class='alert alert-success'><a class='close' data-dismiss='alert'></a><strong style='width:12px'>Thanks!</strong>{entity} Completed successfully!</div>");
        }

        public ActionResult Report(string entity = "")
        {
            if (!string.IsNullOrEmpty(entity))
                new Wizard().CreateReport(entity);

            return Content($"<div class='alert alert-success'><a class='close' data-dismiss='alert'></a><strong style='width:12px'>Thanks!</strong>{entity} Completed successfully!</div>");
        }

        public ActionResult Repository(string entity = "")
        {
            if (!string.IsNullOrEmpty(entity))
                new Wizard().CreateRepository(entity, "Periodic");

            return Content($"<div class='alert alert-success'><a class='close' data-dismiss='alert'>&times;</a><strong style='width:12px'>Thanks!</strong>Repository {entity} generated successfully!</div>");
        }

        public ActionResult Import(string entity = "")
        {
            if (!string.IsNullOrEmpty(entity))
                new Wizard().CreateImport(entity);

            //new ProcessConfigBuilder().CreateRepository("", "Periodic");
            //new ProcessConfigBuilder().CreateReport("");

            return Content($"<div class='alert alert-success'><a class='close' data-dismiss='alert'></a><strong style='width:12px'>Thanks!</strong>{entity} Completed successfully!</div>");
        }

        public ActionResult SpProcess(string entity = "")
        {
            if (!string.IsNullOrEmpty(entity))
            {
                 new Wizard().CreateSpProcess(entity);
            }

            return Content($"<div class='alert alert-success'><a class='close' data-dismiss='alert'></a><strong style='width:12px'>Thanks! </strong>ProcessUnit_Sp_{ entity } completed successfully!</div>");
        }
        public ActionResult UpdateGetBy()
        {

            var gen = new Generator();
            gen.DB = new dbSql();
            gen.RunScript = true;
            foreach (var tabla in DNF.Enviroment.Current.Entorno.GetTables())
                gen.AddTable(tabla);

            StringBuilder SBFull = new StringBuilder();
            foreach (var enti in gen.LstEntity)
            {
                gen.DB.Entity = enti;
                SBFull.AppendLine(gen.DB.GetByRefEntity());
                SBFull.AppendLine(gen.DB.Get());

            }
            DNF.Enviroment.Current.Entorno.ExecuteScript(SBFull.ToString());

            return Content($"<div class='alert alert-success'><a class='close' data-dismiss='alert'></a><strong style='width:12px'>Thanks! </strong> All GetBy... sp has ben Update successfully!</div>");
        }
        
        public ActionResult ManageEntityDB()
        {

            //if (Current.User ==null || !Current.User.HasAccess("IsDeveloper") )
            //    return Content($"<div class='alert alert-success'><strong style='width:12px'>you cant do this</strong></div>");

            if (!Current.User?.HasAccess("IsDeveloper") == true)
                return Content($"<div class='alert alert-success'><strong style='width:12px'>you cant do this</strong></div>");

            var entityConfigPeriodic = RepositoryConfig.Dao.GetAllPeriodic();
            string strBuilder = "";

            foreach (var item in entityConfigPeriodic)
            {
                string sqlQuery = "";
                sqlQuery = $"TRUNCATE TABLE {item.Code} \r\n";
                strBuilder = strBuilder + sqlQuery;
            }

            var entityConfigParametric = RepositoryConfig.Dao.GetAll().Where(c => c.Type.Code.Trim().ToLower() == "parametric" && c.Type.Code.Trim().ToLower() == "parametricconfig");

            foreach (var item in entityConfigParametric)
            {
                string sqlQuery = "";
                sqlQuery = $"DELETE FROM {item.Code } WHERE = repository_id != 0 " + System.Environment.NewLine;
                strBuilder = strBuilder + sqlQuery;
            }
            
            return Content($"<div class='alert alert-success'><a class='close' data-dismiss='alert'></a><strong style='width:12px'>Thanks! </strong>Process Completed successfully!</div>");
        }
    }
}
