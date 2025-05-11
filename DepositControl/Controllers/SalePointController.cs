using DepositControl.Bussines;
using DNF.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DepositControl.Models;
using DNF.Security.Bussines;

namespace DepositControl.Controllers
{
    public class SalePointController : Controller
    {
        private List<SelectListItem> salePointList = new List<SelectListItem>();

        private void FillSalePoints()
        {
            List<SalePoint> salePoints = SalePoint.Dao.GetByFilter(new
            {
                Code = "Active"
            }).ToList();
            foreach (var item in salePoints)
            {
                salePointList.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Id.ToString()
                });
            }
            salePointList.Insert(0, new SelectListItem { Text = "--Seleccione--", Value = "" });
        }

        private void GetCodeList()
        {
            List<SelectListItem> codeList = new List<SelectListItem>
            {
                new SelectListItem { Text = "Activo", Value = "Active" },
                new SelectListItem { Text = "Inactivo", Value = "Inactive" }
            };
            ViewBag.CodeList = codeList;
        }

        // GET: SalePoint/Index
        [AccessCode("SalePoint")]
        [Authenticated]
        public ActionResult Index(string name = null)
        {
            try
            {
                List<SalePoint> salePoints = SalePoint.Dao.GetAll().ToList();
                ViewBag.Edit = Current.User.HasAccess("SalePointEdit");
                ViewBag.Delete = Current.User.HasAccess("SalePointDelete");
                ViewBag.New = Current.User.HasAccess("SalePointCreate");
                return View(salePoints);
            }
            catch (Exception ex)
            {
                ViewBag.Alert = $"Error: {ex.Message}";
                return View("Error");
            }
        }

        [HttpPost]
        [AccessCode("SalePoint")]
        [Authenticated]
        public ActionResult Index(FormCollection collection)
        {
            string name = collection["Name"];
            if (!string.IsNullOrEmpty(collection["clear"]))
            {
                return RedirectToAction("Index");
            }
            try
            {
                List<SalePoint> salePoints = SalePoint.Dao.GetByFilter(new
                {
                    Name = name
                }).ToList();
                ViewBag.Edit = Current.User.HasAccess("SalePointEdit");
                ViewBag.Delete = Current.User.HasAccess("SalePointDelete");
                ViewBag.New = Current.User.HasAccess("SalePointCreate");
                return View(salePoints);
            }
            catch (Exception ex)
            {
                ViewBag.Alert = $"Error: {ex.Message}";
                return View("Error");
            }
        }

        // GET: SalePoint/Create
        [AccessCode("SalePointCreate")]
        [Authenticated]
        public ActionResult Create()
        {
            FillSalePoints();
            ViewBag.salePointList = salePointList;
            return View();
        }

        // POST: SalePoint/Create
        [HttpPost]
        [AccessCode("SalePointCreate")]
        [Authenticated]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var existingSalePoint = SalePoint.Dao.GetByFilter(new
                    {
                        Name = collection["Name"]
                    }).FirstOrDefault();

                    if (existingSalePoint == null)
                    {
                        SalePoint salePoint = new SalePoint
                        {
                            Name = collection["Name"].Trim(),
                            Address = collection["Address"].Trim(),
                            Code = "Active",
                            Order = SalePoint.Dao.GetLastOrder() + 1
                        };
                        salePoint.Save();
                        TempData["Success"] = "Se ha creado correctamente";
                        return RedirectToAction("Index");
                    }

                    FillSalePoints();
                    ViewBag.salePointList = salePointList;
                    ViewBag.Alert = "Ya existe un punto de venta con ese nombre.";
                    return View();
                }

                FillSalePoints();
                ViewBag.salePointList = salePointList;
                return View();
            }
            catch
            {
                FillSalePoints();
                ViewBag.salePointList = salePointList;
                ViewBag.Alert = "Ocurrió un error al crear el punto de venta.";
                return View();
            }
        }

        // GET: SalePoint/Edit/5
        [AccessCode("SalePointEdit")]
        [Authenticated]
        public ActionResult Edit(long id)
        {
            try
            {
                SalePoint salePoint = SalePoint.Dao.Get(id);
                if (salePoint == null)
                {
                    ViewBag.Alert = "El punto de venta no existe.";
                    return RedirectToAction("Index");
                }

                FillSalePoints();
                ViewBag.salePointList = salePointList;
                GetCodeList();
                return View(salePoint);
            }
            catch
            {
                ViewBag.Alert = "Ocurrió un error al cargar el punto de venta.";
                return View("Error");
            }
        }

        // POST: SalePoint/Edit/5
        [HttpPost]
        [AccessCode("SalePointEdit")]
        [Authenticated]
        public ActionResult Edit(long id, FormCollection collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    SalePoint salePoint = SalePoint.Dao.Get(id);
                    if (salePoint == null)
                    {
                        ViewBag.Alert = "El punto de venta no existe.";
                        return RedirectToAction("Index");
                    }

                    var existingSalePoint = SalePoint.Dao.GetByFilter(new
                    {
                        Name = collection["Name"]
                    }).FirstOrDefault();

                    if (existingSalePoint == null || existingSalePoint.Id == id)
                    {
                        salePoint.Name = collection["Name"].Trim();
                        salePoint.Address = collection["Address"].Trim();
                        salePoint.Code = collection["Code"];
                        salePoint.Save();
                        TempData["Success"] = "Se ha editado correctamente";
                        return RedirectToAction("Index");
                    }

                    FillSalePoints();
                    ViewBag.salePointList = salePointList;
                    GetCodeList();
                    ViewBag.Alert = "Ya existe un punto de venta con ese nombre.";
                    return View(salePoint);
                }

                FillSalePoints();
                ViewBag.salePointList = salePointList;
                GetCodeList();
                ViewBag.Alert = "Los datos ingresados no son válidos. Por favor, revisa los campos.";
                return View(SalePoint.Dao.Get(id));
            }
            catch
            {
                FillSalePoints();
                ViewBag.salePointList = salePointList;
                GetCodeList();
                ViewBag.Alert = "Ocurrió un error al actualizar el punto de venta.";
                return View(SalePoint.Dao.Get(id));
            }
        }

        // GET: SalePoint/Delete/5
        [AccessCode("SalePointDelete")]
        [Authenticated]
        public ActionResult Delete(long id)
        {
            try
            {
                SalePoint salePoint = SalePoint.Dao.Get(id);
                if (salePoint == null)
                {
                    TempData["Alert"] = "El punto de venta no existe.";
                    return RedirectToAction("Index");
                }

                List<DeliveryNote> deliveryNotes = DeliveryNote.Dao.GetByFilter(new { SalePoint_Id = id }) ?? new List<DeliveryNote>();
                if (deliveryNotes.Count > 0)
                {
                    TempData["Alert"] = "No se puede eliminar el punto de venta porque está asociado a una o más remitos.";
                    return RedirectToAction("Index");
                }

                salePoint.Delete();
                TempData["Success"] = "Se ha eliminado correctamente";
                return RedirectToAction("Index");
            }
            catch
            {
                TempData["Alert"] = "Ocurrió un error al eliminar el punto de venta.";
                return RedirectToAction("Index");
            }
        }
    }
}
