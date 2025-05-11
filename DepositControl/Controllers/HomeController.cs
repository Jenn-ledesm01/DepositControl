using System.Collections.Generic;
using System.Web.Mvc;
using System;
using System.Linq;
using System.Security.Principal;
using System.Web.Configuration;
using DepositControl.Areas.Bcri.Models;
using System.Web.Routing;
using DNF.Security.Bussines;
using System.Web;
using System.Configuration;
using DepositControl.Bussines;
using DNF.Entity;
using NPOI.SS.Formula.Functions;

namespace DepositControl.Controllers
{
    [Authenticated]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (Current.User != null)
            {
                var uderId = Current.User;
                LogAccion.Dao.AddLog("LogIn", uderId.FullName, null);
            }
            var allProducts = Product.Dao.GetAll().ToList();
            ViewBag.TotalProductos = allProducts.Count();

            var lowStockProducts = Product.Dao
            .GetByFilter(new { StateProductName = "Bajo Stock"}).ToList();
            ViewBag.ProductosBajoStock = lowStockProducts.Count();

            lowStockProducts.LoadRelation(p => p.Stock);
            //var products = Product.Dao
            //    .GetAll()
            //    .ToList();
            //products.LoadRelation(p => p.Stock);

            //var lowStockList = products
            //.Where(p => p.Stock.Quantity <= 5)
            //.ToList();

            return View(lowStockProducts);
        }

        [HttpPost]
        public JsonResult closeLogOut()
        {
           if (Convert.ToBoolean(ConfigurationManager.AppSettings["UseActiveDirectory"]))
           {
                LogAccion.Dao.AddLog("LogOut"                                           
                    , Current.User.Name
                    , null);
                            
           }
           Session.Clear();
           Session.Abandon();
           Request.Cookies["ASP.NET_SessionId"].Value = string.Empty;       
            return this.Json(new { success = true });
        }

    }
}


