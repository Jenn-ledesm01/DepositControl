using DepositControl.Bussines;
using DNF.Entity;
using iTextSharp.text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DepositControl.Controllers
{
    public class StockMovementController : Controller
    {
        // GET: StockMovement/Index
        public ActionResult Index()
        {
            try
            {
                StockMovementDTO stockMovementDTO = new StockMovementDTO();
                List<StockMovementDTO> stockMovements = stockMovementDTO.GetStockMovement();
                foreach (var item in stockMovements)
                {
                    if (item.Origen == "Orden de Compra")
                    {
                        item.DetalleURL = Url.Action("Details", "PurchaseOrder", new { id = item.PurchaseOrder_Id });
                    }
                    else if (item.Origen == "Remito")
                    {
                        item.DetalleURL = Url.Action("Details", "DeliveryNote", new { id = item.DeliveryNote_Id });
                    }
                    else
                    {
                        item.DetalleURL = null;
                    }
                }
                return View(stockMovements);
            }
            catch (Exception ex)
            {
                ViewBag.Alert = $"Error: {ex.Message}";
                return View("Error");
            }
        }

        // POST: StockMovement/Index
        [HttpPost]
        public ActionResult Index(FormCollection collection)
        {
            string productName = collection["ProductName"];
            string tipoMovimiento = collection["TipoMovimiento"];
            string numero = collection["Numero"];
            try
            {
                if (!string.IsNullOrEmpty(collection["clear"]))
                {
                    return RedirectToAction("Index");
                }
                StockMovementDTO stockMovementDTO = new StockMovementDTO();
                List<StockMovementDTO> stockMovements = stockMovementDTO.GetStockMovementByFilter(productName, tipoMovimiento, numero);
                foreach (var item in stockMovements)
                {
                    if (item.Origen == "Orden de Compra")
                    {
                        item.DetalleURL = Url.Action("Details", "PurchaseOrder", new { id = item.PurchaseOrder_Id });
                    }
                    else if (item.Origen == "Remito")
                    {
                        item.DetalleURL = Url.Action("Details", "DeliveryNote", new { id = item.DeliveryNote_Id });
                    }
                    else
                    {
                        item.DetalleURL = null;
                    }
                }
                ViewBag.ProductName = productName;
                ViewBag.TipoMovimiento = tipoMovimiento;
                ViewBag.Numero = numero;
                return View(stockMovements);
            }
            catch (Exception ex)
            {
                ViewBag.Alert = $"Error: {ex.Message}";
                return View("Error");
            }
        }
    }
}