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
    public class StateWarehouseManagerController : Controller
    {
        private List<SelectListItem> stateList = new List<SelectListItem>();

        private void llenarEstados()
        {
            List<StateWarehouseManager> states = StateWarehouseManager.Dao.GetByFilter(new
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

        // GET: StateWarehouseManager/Index
        [AccessCode("StateWarehouseManager")]
        [Authenticated]
        public ActionResult Index(string name = null)
        {
            try
            {
                List<StateWarehouseManager> stateWarehouseManagers = StateWarehouseManager.Dao.GetAll().ToList();
                ViewBag.Delete = Current.User.HasAccess("StateWarehouseManagerDelete");
                ViewBag.Edit = Current.User.HasAccess("StateWarehouseManagerEdit");
                ViewBag.New = Current.User.HasAccess("StateWarehouseManagerCreate");
                return View(stateWarehouseManagers);
            }
            catch (Exception ex)
            {
                ViewBag.Alert = $"Error: {ex.Message}";
                return View("Error");
            }
        }

        [HttpPost]
        [AccessCode("StateWarehouseManager")]
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
                List<StateWarehouseManager> stateWarehouseManagers = StateWarehouseManager.Dao.GetByFilter(new
                {
                    Name = name,
                    Code = "Active"
                }).ToList();
                ViewBag.Delete = Current.User.HasAccess("StateWarehouseManagerDelete");
                ViewBag.New = Current.User.HasAccess("StateWarehouseManagerCreate");
                return View(stateWarehouseManagers);
            }
            catch (Exception ex)
            {
                ViewBag.Alert = $"Error: {ex.Message}";
                return View("Error");
            }
        }

        // GET: StateWarehouseManager/Create
        [AccessCode("StateWarehouseManagerCreate")]
        [Authenticated]
        public ActionResult Create()
        {
            llenarEstados();
            ViewBag.stateList = stateList;
            return View();
        }

        // POST: StateWarehouseManager/Create
        [HttpPost]
        [AccessCode("StateWarehouseManagerCreate")]
        [Authenticated]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var existingStateWarehouseManager = StateWarehouseManager.Dao.GetByFilter(new
                    {
                        Name = collection["Name"]
                    }).FirstOrDefault();

                    if (existingStateWarehouseManager == null)
                    {
                        StateWarehouseManager stateWarehouseManager = new StateWarehouseManager
                        {
                            Name = collection["Name"].Trim(),
                            Code = "Active",
                            Order = StateWarehouseManager.Dao.GetLastOrder() + 1
                        };
                        stateWarehouseManager.Save();
                        TempData["Success"] = "Se ha creado correctamente";
                        return RedirectToAction("Index");
                    }

                    llenarEstados();
                    ViewBag.stateList = stateList;
                    ViewBag.Alert = "Ya existe un estado de jefe de depósito con ese nombre.";
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
                ViewBag.Alert = "Ocurrió un error al crear el estado de jefe de depósito.";
                return View();
            }
        }

        // GET: StateWarehouseManager/Edit/5
        [AccessCode("StateWarehouseManagerEdit")]
        [Authenticated]
        public ActionResult Edit(long id)
        {
            try
            {
                StateWarehouseManager stateWarehouseManager = StateWarehouseManager.Dao.Get(id);
                if (stateWarehouseManager == null)
                {
                    ViewBag.Alert = "El estado de jefe de depósito no existe.";
                    return RedirectToAction("Index");
                }

                llenarEstados();
                ViewBag.stateList = stateList;
                GetCodeList();
                return View(stateWarehouseManager);
            }
            catch
            {
                ViewBag.Alert = "Ocurrió un error al cargar el estado de jefe de depósito.";
                return View("Error");
            }
        }

        // POST: StateWarehouseManager/Edit/5
        [HttpPost]
        [AccessCode("StateWarehouseManagerEdit")]
        [Authenticated]
        public ActionResult Edit(long id, FormCollection collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    StateWarehouseManager stateWarehouseManager = StateWarehouseManager.Dao.Get(id);
                    if (stateWarehouseManager == null)
                    {
                        ViewBag.Alert = "El estado de jefe de depósito no existe.";
                        return RedirectToAction("Index");
                    }

                    var existingStateWarehouseManager = StateWarehouseManager.Dao.GetByFilter(new
                    {
                        Name = collection["Name"]
                    }).FirstOrDefault();

                    if (existingStateWarehouseManager == null || existingStateWarehouseManager.Id == id)
                    {
                        stateWarehouseManager.Name = collection["Name"].Trim();
                        stateWarehouseManager.Code = collection["Code"];
                        stateWarehouseManager.Save();
                        TempData["Success"] = "Se ha editado correctamente";
                        return RedirectToAction("Index");
                    }

                    llenarEstados();
                    ViewBag.stateList = stateList;
                    GetCodeList();
                    ViewBag.Alert = "Ya existe un estado de jefe de depósito con ese nombre.";
                    return View(stateWarehouseManager);
                }

                llenarEstados();
                ViewBag.stateList = stateList;
                return View(StateWarehouseManager.Dao.Get(id));
            }
            catch
            {
                llenarEstados();
                ViewBag.stateList = stateList;
                ViewBag.Alert = "Ocurrió un error al actualizar el estado de jefe de depósito.";
                return View(StateWarehouseManager.Dao.Get(id));
            }
        }

        // GET: StateWarehouseManager/Delete/5
        [AccessCode("StateWarehouseManagerDelete")]
        [Authenticated]
        public ActionResult Delete(long id)
        {
            try
            {
                StateWarehouseManager stateWarehouseManager = StateWarehouseManager.Dao.Get(id);
                List<WarehouseManager> warehouseManagers = WarehouseManager.Dao.GetByFilter(new { StateWarehouseManager_Id = id }) ?? new List<WarehouseManager>();

                if (warehouseManagers.Count > 0)
                {
                    TempData["Alert"] = "No se puede eliminar el estado de jefe de depósito porque está asociado a uno o más jefes de depósito.";
                    return RedirectToAction("Index");
                }
                else
                {
                    if (stateWarehouseManager != null)
                    {
                        stateWarehouseManager.Delete();
                    }
                    TempData["Success"] = "Se ha eliminado correctamente";
                    return RedirectToAction("Index");
                }
            }
            catch
            {
                TempData["Alert"] = "Ocurrió un error al eliminar el estado de jefe de depósito.";
                return RedirectToAction("Index");
            }
        }
    }
}