using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DNF.Entity;
using DNF.Enviroment;
using DNF.Structure.Bussines;
using Bcri.Core.Bussines;
using DNF.CodeGenerator;
using Type = DNF.Type.Bussines.Type;
using System.IO;

namespace DepositControl.Areas.Bcri.Controllers
{
    public class EntityController : Controller
    {

        public ActionResult Index()
        {
            var Model = new List<Entity>();
            Current.Entorno.GetTables().ToList().ForEach(x => Model.Add(Entity.Dao.GetByName(x) ?? new Entity() { Name = x, Id = -1 }));
            return View("Index", Model);
        }

        public ActionResult EntityData(int EntityId)
        {
            Entity entity = Entity.Dao.Get(EntityId);
            ViewBag.repoCfgEntity = entity != null ? RepositoryConfig.Dao.GetBy(entity) : null;
            return PartialView("EntityData", entity);
        }

        public ActionResult getStructs(int entityId)
            => Json(Entity.Dao.Get(entityId)
                .Structs?
                .Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.Description,
                    DataType = x.DataType.C,
                    x.RefEntity,
                    Entity_Id = x.Entity.Id
                })
                .ToList(),
           JsonRequestBehavior.AllowGet);

        public ActionResult SaveEntity(Entity entity, string repositoryName, int repositoryType)
        {
            try
            {
                var oldEntity = Entity.Dao.Get(entity.Id);
                oldEntity.IsLoggeable = entity.IsLoggeable;
                oldEntity.LargeData = entity.LargeData;
                oldEntity.IsTranslatable = entity.IsTranslatable;
                oldEntity.Save();

                var repocfg = RepositoryConfig.Dao.GetBy(oldEntity);

                if (oldEntity.Structs?.Any(x => x.Name == "Repository_Id") ?? false && repocfg.Any())
                {
                    var repo = repocfg.FirstOrDefault();
                    repo.Entity = oldEntity;
                    repo.Name = repositoryName;
                    repo.Type = Type.Dao.Get(repositoryType);
                    repocfg.Save();
                }

                return Content($"<div class='alert alert-success'><a class='close' data-dismiss='alert'>&times;</a><strong style='width:12px'>" + Res.res.thanks + "</strong>" + Res.res.entity + oldEntity.Name + " " + Res.res.savedSuccessfully + "</div>");
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                HttpContext.Response.SubStatusCode = 100;
                return Content(ex.Message);
            }
        }

        public JsonResult ModifyStructDescription(int Id, int Entity_Id, string Name, string Description)
        {
            string mensageResponse = "";
            bool anyError = false;

            try
            {
                var struc = Struct.Dao.Get(Id);
                if (struc == null)
                {
                    throw new Exception($"Struc Id ({Id}) {Res.res.dontexist}");
                }
                //Valido que el estruct sea de verdad de la entidad, un poquito de robustes
                if (struc.Entity.Id != Entity_Id || struc.Name != Name)
                {
                    throw new Exception($"Struc {Name} Id({Id}), Entity Id({Entity_Id}) {Res.res.dontexist}");
                }

                struc.Description = Description;
                struc.Save();

                mensageResponse = Res.res.savedSuccessfully;

            }
            catch (Exception ex)
            {
                anyError = true;
                mensageResponse = ex.Message;
            }

            //La jqgrid celledit necesita que sea un array [TRUE|FALSE, mensaje]
            //http://www.trirand.com/jqgridwiki/doku.php?id=wiki:cell_editing ver afterSubmitCell
            return Json(new object[]
            {
                !anyError,
                mensageResponse,
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GenerateEntity(bool download = false, string entity = "")
        {
            try
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
                    Path = @"C:\TEMP",
                    MemoryOrServer = !download//true disco(server) false memoria(variable LstEntity)
                };


                if (!string.IsNullOrEmpty(entity))
                    mcg.AddTable(entity);

                mcg.Generate();

                if (download)
                {
                    var ms = new MemoryStream();
                    var sw = new StreamWriter(ms);
                    sw.Write(mcg.lstFile.FirstOrDefault().Content);
                    sw.Flush();
                    byte[] bin = ms.ToArray();
                    Response.AppendHeader("content-disposition", "attachment; filename=\"" + mcg.lstFile.FirstOrDefault().PathName + "\"");
                    Response.ContentType = "text/plain";
                    Response.OutputStream.Write(bin, 0, mcg.lstFile.FirstOrDefault().Content.Length);
                    Response.OutputStream.Flush();
                    Response.End();
                }

                Entity.Dao.GetCurrentByName(entity);

                return Content($"<div class='alert alert-success'><a class='close' data-dismiss='alert'>&times;</a><strong style='width:12px'>" + Res.res.thanks + "</strong>" + Res.res.entity + entity + Res.res.generatedSuccessfully + "</div>");
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                HttpContext.Response.SubStatusCode = 100;
                return Content(ex.Message);
            }
        }

        public ActionResult GenerateRepository(string entity = "")
        {
            try
            {
                if (!string.IsNullOrEmpty(entity))
                    new Wizard().CreateRepository(entity, "Periodic");

                return Content($"<div class='alert alert-success'><a class='close' data-dismiss='alert'>&times;</a><strong style='width:12px'>" + Res.res.thanks + "</strong>" + Res.res.repository + entity + Res.res.generatedSuccessfully + "</div>");
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                HttpContext.Response.SubStatusCode = 100;
                return Content(ex.Message);
            }
        }
    }
}
