using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Bcri.Core.Bussines;
using DepositControl.Areas.Bcri.Models;
using DepositControl.Areas.Bcri.Utility;
using DNF.ExtendedDao;
using DNF.Structure.Bussines;
using DNF.Entity;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using DNF.Security.Bussines;

namespace DepositControl.Areas.Bcri.Controllers
{
    [Authenticated]
    public class RepositoryController : Controller
    {
        // GET: Repository
        public ActionResult Index()
        {
            // la mayoria de la logica de la grilla esta en GridController 
            ViewBag.RepositoryConfigs = RepositoryConfig.Dao.GetAll().ToListItem("");

            return View();
        }

        public ActionResult Edit(string repositoryCode)
        {
            var parametric = RepositoryConfig.Dao.GetByFilter(new { Code = repositoryCode });

            var acceso = SecurityUtility.GetParentAccess(Current.Access);

            LogAccion.Dao.AddLog("ParametricAccess"
                //el usuario X a navegado a Y
                , Current.User.Name + ";#;" + acceso
                );

            // la mayoria de la logica de la grilla esta en GridController 
            var repoConfi = RepositoryConfig.Dao.GetByCode(repositoryCode);

            ViewBag.Edit = true;
            if (repoConfi == null)
            {
                new Wizard().CreateRepository(repositoryCode, "Parametric");
                repoConfi = RepositoryConfig.Dao.GetByCode(repositoryCode);
            }

            if (repoConfi == null)
                return RedirectToAction("AccessDenied", "Out");


            ViewBag.Title = repoConfi.Name;
            return View(repoConfi.Current);

        }

        public ActionResult RecreateRepository(string repositoryCode)
        {
            new Wizard().CreateRepository(repositoryCode, "Parametric");
            return RedirectToAction("Edit");
        }

        public ActionResult Grid(long repositoryConfigId)
        {
            // la mayoria de la logica de la grilla esta en GridController 
            var config = RepositoryConfig.Dao.Get(repositoryConfigId);

            Repository repo = config.Type.Code == "Parametric" ? config.Current : Repository.Dao.GetLastValid(config);

            return PartialView("RepositoryDataGrid", repo);
        }

        protected override JsonResult Json(object data, string contentType, System.Text.Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new JsonResult()
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = behavior,
                MaxJsonLength = Int32.MaxValue
            };
        }
        public JsonResult LoadColModelAndData(string data)
        {
            var ser = new JavaScriptSerializer();
            var datos = ser.Deserialize<List<Dictionary<string, string>>>(data);
            var list = new List<Dictionary<string, object>>();

            if (datos != null)
            {
                foreach (var d in datos)
                {
                    var dic = new Dictionary<string, object>();

                    var repoConfig = RepositoryConfig.Dao.Get(int.Parse(d["repositoryconfigid"]));

                    dic.Add("ColModels", repoConfig.Entity.Structs.ToJqGridColModels());
                    dic.Add("LoadData", LoadData(int.Parse(d["repositoryid"]), int.Parse(d["repositoryconfigid"])));
                    list.Add(dic);

                }
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public Array LoadData(int repositoryId, int repositoryConfigId)
        // aca iria el paginado y mas cosas
        {
            //object result = null;

            var repository =
                repositoryId == 0
                    ? RepositoryConfig.Dao.Get(repositoryConfigId).Current
                    : Repository.Dao.Get(repositoryId);

            return repository.Reader.GetAll().ToJsonSerializeArray();

        }

    }
}

