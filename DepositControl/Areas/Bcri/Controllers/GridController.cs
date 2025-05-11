using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bcri.Core.Bussines;
using DepositControl.Areas.Bcri.Models;
using DepositControl.Areas.Bcri.Utility;
using DNF.Security.Bussines;
using DNF.Structure.Bussines;
using Microsoft.Ajax.Utilities;

namespace DepositControl.Areas.Bcri.Controllers
{
    [Authenticated]
    public class GridController : Controller
    {


        public ActionResult Index(string entityName)
        {
            var acceso = SecurityUtility.GetParentAccess(Current.Access);

            //LogAccion.Dao.AddLog("ParametricAccess"
            //    //el usuario X a navegado a Y
            //    , Current.User.Name + ";#;" + acceso
            //    );

            // la mayoria de la logica de la grilla esta en GridController 
            var entity = Entity.Dao.GetByName(entityName);

            ViewBag.Edit = true;
            if (entity == null)
            {
                entity = new Wizard().CreateEntity(entityName);
            }

            ViewBag.Title = entity.Name;
            return View(entity);
        }

        // GET: Bcri/Grid
        public ActionResult Edit(string oper, string entityName, long? repository_Id = null, long id = 0, int repositoryConfigId= 0)
        {

            //todo: chekear permisos obviamente


            var reader = new DbReader(entityName, repository_Id);


            var formParse = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            var form = Request.Form;

            foreach (var key in form.AllKeys)
                formParse.Add(key, form[key] != "_empty" ? form[key] : null);



            try
            {
                if (oper == "add" || oper == "edit")
                    reader.Save(formParse);
                else if (oper == "del" && id != 0)
                    reader.Delete(id);


                return Json(new {ok = true}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                HttpContext.Response.StatusCode = 500;
                HttpContext.Response.SubStatusCode = 100;
                return Content(ex.Message);
            }
        }

        public ActionResult JqGrid(GridModel gridModel)
        {
            var enti = Entity.Dao.GetByName(gridModel.EntityName);

            gridModel.ServerSide = enti.LargeData;

            var jqGrid = new JqGrid
            {
                datatype = "json",
                colModel = enti.Structs.ToJqGridColModels(),
                loadonce = !gridModel.ServerSide,
                url = !gridModel.ServerSide
                        ? Url.Action("GetData", new { gridModel.EntityName, repository_Id = gridModel.RepositoryId })
                        : Url.Action("GetDataFilter", new { gridModel.EntityName, repository_Id = gridModel.RepositoryId }),
                editurl = Url.Action("Edit"),
                pager = gridModel.GridId + "-Pager",
                sortable = !gridModel.ServerSide,
                sortname = enti.Structs.Any(x => x.Name == "Name") ? "Name" : "Id",
                sortorder = "ASC",

                ignoreCase = true,
                viewrecords = true,
                rownumbers = false,

                shrinkToFit = false,
                forceFit = false,
                width = 450,
                height = 350,
                rowNum = 20
            };
            return Json(jqGrid, JsonRequestBehavior.AllowGet);

        }

        public ActionResult GetCsv(string entityName, int repository_Id)
            => new CsvEntityReaderResult(entityName, repository_Id);

        public ActionResult GetXls(string entityName, int repository_Id)
            => new XlsEntityReaderResult(entityName, repository_Id);

        public ActionResult GetData(string entityName, int repository_Id)
            => new JsonReaderResult(new DbReader(entityName, repository_Id).GetAll());

        public ActionResult GetDataFilter(string entityName, int repository_Id, bool _search = false)
        {
            var filters = new Dictionary<string, string>
            {
                {"Page", Request.QueryString["page"]},
                {"PageSize", Request.QueryString["rows"]}
            };

            if (_search) { 
                var enti = Entity.Dao.GetByName(entityName);

                var dicStuc = enti.Structs.Where(x => x.InTable)
                    .ToDictionary(x => x.Name, x => x, StringComparer.OrdinalIgnoreCase);
                //campos del querystring que esten fisicamente en la tbale
                foreach (var key in Request.QueryString.AllKeys.Where(key => dicStuc.ContainsKey(key)))
                {
                    var stru = dicStuc[key];

                    //todo ver como parciarlo mejor para el GetByFilter modificar el sp para que funcione con "like" en ves de igual
                    if (stru.DataType.C == "string")
                        filters.Add(key,$"%{Request.QueryString[key]}%");
                    else
                        filters.Add(key, Request.QueryString[key]);
                }
            }

            var reader = new DbReader(entityName, repository_Id).GetByFilter(filters);
            var data = reader.ToJsonSerializeList();
            var pageSize = int.Parse(Request.QueryString["rows"]);
            var rowsTotal = (data.Count > 0) ? (int)data[0]["RowsTotal"] : 0;
            

            var resul = new
            {
                page = Request.QueryString["page"],
                total = rowsTotal/pageSize +( (rowsTotal%pageSize) > 0 ? 1 : 0),
                records = rowsTotal,
                rows = data.ToArray()
            };
            
            return Json(resul,JsonRequestBehavior.AllowGet);
        }
    }
}
