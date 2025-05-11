using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bcri.Core.Bussines;
using DNF.Type.Bussines;
using Type = DNF.Type.Bussines.Type;

namespace DepositControl.Areas.Bcri.Controllers
{
    public class BcpImportController : Controller
    {
        // GET: Bcri/BcpImport
        public ActionResult Index(int processConfigId)
        {

            var process = ProcessConfig.Dao.Get(processConfigId);

            ViewBag.Repositorys = process.OutputConfigs.Select(x => x.RepositoryConfig).ToListItemCode();
            ViewBag.Conexions = Conexion.Dao.GetAll().ToListItem();
            var bcps = BcpImport.Dao.GetBy(process);

            return View(bcps.Count > 0 ? bcps[0] : new  BcpImport() {ProcessConfig = process });
        }

        public ActionResult Save(BcpImport bcpImport)
        {
            var bcpfinal = bcpImport.IsNew()
                            ? new BcpImport()
                            : BcpImport.Dao.Get(bcpImport.Id);


            bcpfinal.RepositoryCode = bcpImport.RepositoryCode;
            bcpfinal.Name = bcpImport.Name ?? bcpImport.RepositoryCode;

            bcpfinal.Path = bcpImport.Path;
            bcpfinal.ProcessConfig = bcpImport.ProcessConfig;
            bcpfinal.HasHeaderRow = bcpImport.HasHeaderRow;
            bcpfinal.EnclosedInQuotes = bcpImport.EnclosedInQuotes;
            bcpfinal.Delimiter = bcpImport.Delimiter;
            bcpfinal.Delimited = Request.Form["filetype"] == "Delimited";
            bcpfinal.SqlSource = Request.Form["bcptype"] == "Db";

            bcpfinal.Conexion = bcpImport.Conexion;
            bcpfinal.SqlCommand = bcpImport.SqlCommand;


            bcpfinal.Save();

            var pConfig = ProcessConfig.Dao.Get(bcpfinal.ProcessConfig.Id);

            if (pConfig.InputConfigs.All(x => x.RepositoryConfig.Code != "BcpImport"))
            {
                // si no tiene el bcp como imput se lo agrego para la trazabilidad
                var inputbcp = new ProcessInputConfig
                {
                    ProcessConfig = pConfig,
                    RepositoryConfig = RepositoryConfig.Dao.GetByCode("BcpImport"),
                    Code = "BcpImport",
                    Visibility = new Type("Visibility", "Normal")
                };
                inputbcp.Save();
                pConfig.InputConfigs.Add(inputbcp);

            }


            return Json(new { ok = true }, JsonRequestBehavior.AllowGet);
        }
    }
}
