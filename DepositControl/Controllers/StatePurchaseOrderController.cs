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
    public class StatePurchaseOrderController : Controller
    {
        private List<SelectListItem> stateList = new List<SelectListItem>();

        private void llenarEstados()
        {
            List<StatePurchaseOrder> states = StatePurchaseOrder.Dao.GetByFilter(new
            {
                Code = "Active"
            }).ToList();
            foreach (var item in states)
            {
                stateList.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Id.ToString()
                });
            }
            stateList.Insert(0, new SelectListItem { Text = "--Seleccione--", Value = "" });
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

        // GET: StatePurchaseOrder/Index
        [AccessCode("StatePurchaseOrder")]
        [Authenticated]
        public ActionResult Index(string name = null)
        {
            try
            {
                List<StatePurchaseOrder> statePurchaseOrders = StatePurchaseOrder.Dao.GetAll().ToList();
                ViewBag.Delete = Current.User.HasAccess("StatePurchaseOrderDelete");
                ViewBag.New = Current.User.HasAccess("StatePurchaseOrderCreate");
                ViewBag.Edit = Current.User.HasAccess("StatePurchaseOrderEdit");
                return View(statePurchaseOrders);
            }
            catch (Exception ex)
            {
                ViewBag.Alert = $"Error: {ex.Message}";
                return View("Error");
            }
        }

        [HttpPost]
        [AccessCode("StatePurchaseOrder")]
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
                List<StatePurchaseOrder> statePurchaseOrders = StatePurchaseOrder.Dao.GetByFilter(new
                {
                    Name = name
                }).ToList();
                ViewBag.Delete = Current.User.HasAccess("StatePurchaseOrderDelete");
                ViewBag.New = Current.User.HasAccess("StatePurchaseOrderCreate");
                ViewBag.Edit = Current.User.HasAccess("StatePurchaseOrderEdit");
                return View(statePurchaseOrders);
            }
            catch (Exception ex)
            {
                ViewBag.Alert = $"Error: {ex.Message}";
                return View("Error");
            }
        }

        // GET: StatePurchaseOrder/Create
        [AccessCode("StatePurchaseOrderCreate")]
        [Authenticated]
        public ActionResult Create()
        {
            llenarEstados();
            ViewBag.stateList = stateList;
            return View();
        }

        // POST: StatePurchaseOrder/Create
        [HttpPost]
        [AccessCode("StatePurchaseOrderCreate")]
        [Authenticated]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var existingStatePurchaseOrder = StatePurchaseOrder.Dao.GetByFilter(new
                    {
                        Name = collection["Name"]
                    }).FirstOrDefault();

                    if (existingStatePurchaseOrder == null)
                    {
                        StatePurchaseOrder statePurchaseOrder = new StatePurchaseOrder
                        {
                            Name = collection["Name"].Trim(),
                            Code = "Active",
                            Order = StatePurchaseOrder.Dao.GetLastOrder() + 1
                        };
                        statePurchaseOrder.Save();
                        TempData["Success"] = "Se ha creado correctamente";
                        return RedirectToAction("Index");
                    }

                    llenarEstados();
                    ViewBag.stateList = stateList;
                    ViewBag.Alert = "Ya existe un estado de orden de compra con ese nombre.";
                    return View();
                }

                llenarEstados();
                ViewBag.stateList = stateList;
                return View();
            }
            catch
            {
                llenarEstados();
                ViewBag.stateList = stateList;
                ViewBag.Alert = "Ocurrió un error al crear el estado de orden de compra.";
                return View();
            }
        }

        // GET: StatePurchaseOrder/Edit/5
        [AccessCode("StatePurchaseOrderEdit")]
        [Authenticated]
        public ActionResult Edit(long id)
        {
            try
            {
                StatePurchaseOrder statePurchaseOrder = StatePurchaseOrder.Dao.Get(id);
                if (statePurchaseOrder == null)
                {
                    ViewBag.Alert = "El estado de orden de compra no existe.";
                    return RedirectToAction("Index", "StatePurchaseOrder");
                }

                llenarEstados();
                ViewBag.stateList = stateList;
                GetCodeList();
                return View(statePurchaseOrder);
            }
            catch
            {
                ViewBag.Alert = "Ocurrió un error al cargar el estado de orden de compra.";
                return View("Error");
            }
        }

        // POST: StatePurchaseOrder/Edit/5
        [HttpPost]
        [AccessCode("StatePurchaseOrderEdit")]
        [Authenticated]
        public ActionResult Edit(long id, FormCollection collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    StatePurchaseOrder statePurchaseOrder = StatePurchaseOrder.Dao.Get(id);
                    if (statePurchaseOrder == null)
                    {
                        ViewBag.Alert = "El estado de orden de compra no existe.";
                        return RedirectToAction("Index", "StatePurchaseOrder");
                    }

                    var existingStatePurchaseOrder = StatePurchaseOrder.Dao.GetByFilter(new
                    {
                        Name = collection["Name"]
                    }).FirstOrDefault();

                    if (existingStatePurchaseOrder == null || existingStatePurchaseOrder.Id == id)
                    {
                        statePurchaseOrder.Name = collection["Name"].Trim();
                        statePurchaseOrder.Code = collection["Code"];
                        statePurchaseOrder.Save();
                        TempData["Success"] = "Se ha editado correctamente";
                        return RedirectToAction("Index");
                    }

                    llenarEstados();
                    ViewBag.stateList = stateList;
                    GetCodeList();
                    ViewBag.Alert = "Ya existe un estado de orden de compra con ese nombre.";
                    return View(statePurchaseOrder);
                }

                llenarEstados();
                ViewBag.stateList = stateList;
                GetCodeList();
                ViewBag.Alert = "Los datos ingresados no son válidos. Por favor, revisa los campos.";
                return View(StatePurchaseOrder.Dao.Get(id));
            }
            catch
            {
                llenarEstados();
                ViewBag.stateList = stateList;
                GetCodeList();
                ViewBag.Alert = "Ocurrió un error al actualizar el estado de orden de compra.";
                return View(StatePurchaseOrder.Dao.Get(id));
            }
        }

        // GET: StatePurchaseOrder/Delete/5
        [AccessCode("StatePurchaseOrderDelete")]
        [Authenticated]
        public ActionResult Delete(long id)
        {
            try
            {
                StatePurchaseOrder statePurchaseOrder = StatePurchaseOrder.Dao.Get(id);
                if (statePurchaseOrder == null)
                {
                    TempData["Alert"] = "El estado de orden de compra no existe.";
                    return RedirectToAction("Index");
                }
                List<PurchaseOrder> purchaseOrders = PurchaseOrder.Dao.GetByFilter(new { StatePurchaseOrder_Id = id }) ?? new List<PurchaseOrder>();

                if (purchaseOrders.Count > 0)
                {
                    TempData["Alert"] = "No se puede eliminar el estado de orden de compra porque está asociado a una o más órdenes de compra.";
                    return RedirectToAction("Index");
                }
                else
                {
                    if (statePurchaseOrder != null)
                    {
                        statePurchaseOrder.Delete();
                    }
                    TempData["Success"] = "Se ha eliminado correctamente";
                    return RedirectToAction("Index");
                }
            }
            catch
            {
                TempData["Alert"] = "Ocurrió un error al eliminar el estado de orden de compra.";
                return RedirectToAction("Index");
            }
        }
    }
}