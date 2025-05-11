using com.sun.xml.@internal.bind.v2.model.core;
using DepositControl.Bussines;
using DNF.Entity;
using DNF.Security.Bussines;
using DNF.Security.Dao;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Profile;

namespace DepositControl.Controllers
{
    public class WarehouseManagerController : Controller
    {
        private List<SelectListItem> stateList = new List<SelectListItem>();
        private List<SelectListItem> userList = new List<SelectListItem>();

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

        //private void llenarUsuarios()
        //{

        //    List<UserProfile> userprofiles = DNF.Security.Bussines.UserProfile.Dao.GetByFilter(new
        //    {
        //        Profile_Id = 2
        //    }).ToList();
        //    userprofiles.LoadRelation(u => u.User);
        //    foreach (var item in userprofiles)
        //    {
        //        userList.Add(new SelectListItem
        //        {
        //            Text = item.User.FullName,
        //            Value = item.User.Id.ToString()
        //        });
        //    }
        //    userList.Insert(0, new SelectListItem { Text = "--Seleccione--", Value = "" });
        //}

        private bool llenarUsuarios()
        {
            userList.Clear();
            List<WarehouseManager> existingManagers = WarehouseManager.Dao.GetAll().ToList();
            List<long> assignedUserIds = existingManagers.Select(m => m.User.Id).ToList();

            List<UserProfile> userprofiles = DNF.Security.Bussines.UserProfile.Dao.GetByFilter(new
            {
                Profile_Id = 2
            }).ToList();
            userprofiles.LoadRelation(u => u.User);

            foreach (var item in userprofiles)
            {
                // Solo usuarios habilitados y no asignados
                if (item.User.State.Code == "Enable" && !assignedUserIds.Contains(item.User.Id))
                {
                    userList.Add(new SelectListItem
                    {
                        Text = item.User.FullName,
                        Value = item.User.Id.ToString()
                    });
                }
            }

            userList.Insert(0, new SelectListItem { Text = "--Seleccione--", Value = "" });
            return userList.Count > 1;
        }


        private void llenarUsuariosParaEdicion(long currentUserId)
        {
            List<WarehouseManager> existingManagers = WarehouseManager.Dao.GetAll().ToList();
            List<long> assignedUserIds = existingManagers
                .Where(m => m.User.Id != currentUserId)
                .Select(m => m.User.Id)
                .ToList();

            List<UserProfile> userprofiles = DNF.Security.Bussines.UserProfile.Dao.GetByFilter(new
            {
                Profile_Id = 2
            }).ToList();

            userprofiles.LoadRelation(u => u.User);

            foreach (var item in userprofiles)
            {
                if (!assignedUserIds.Contains(item.User.Id))
                {
                    userList.Add(new SelectListItem
                    {
                        Text = item.User.FullName,
                        Value = item.User.Id.ToString()
                    });
                }
            }

            userList.Insert(0, new SelectListItem { Text = "--Seleccione--", Value = "" });
        }



        // GET: WarehouseManager/Index
        [AccessCode("WarehouseManager")]
        [Authenticated]
        public ActionResult Index(string dni = null)
        {
            try
            {
                List<WarehouseManager> warehouseManagers = WarehouseManager.Dao.GetByFilter(new
                {
                    DNI = dni
                }).ToList();
                warehouseManagers.LoadRelation(w => w.StateWarehouseManager);
                warehouseManagers.LoadRelation(w => w.User);
                ViewBag.Edit = Current.User.HasAccess("WarehouseManagerEdit");
                ViewBag.Delete = Current.User.HasAccess("WarehouseManagerDelete");
                ViewBag.New = Current.User.HasAccess("WarehouseManagerCreate");
                return View(warehouseManagers);
            }
            catch (Exception ex)
            {
                ViewBag.Alert = $"Error: {ex.Message}";
                return View("Error");
            }
        }

        //POST: WarehouseManager/Index
        [HttpPost]
        [AccessCode("WarehouseManager")]
        [Authenticated]
        public ActionResult Index(FormCollection collection)
        {
            string dni = collection["DNI"];
            if (!string.IsNullOrEmpty(collection["clear"]))
            {
                return RedirectToAction("Index");
            }
            try
            {
                List<WarehouseManager> warehouseManagers = WarehouseManager.Dao.GetByFilter(new
                {
                    DNI = dni
                }).ToList();
                warehouseManagers.LoadRelation(w => w.StateWarehouseManager);
                warehouseManagers.LoadRelation(w => w.User);
                ViewBag.Edit = Current.User.HasAccess("WarehouseManagerEdit");
                ViewBag.Delete = Current.User.HasAccess("WarehouseManagerDelete");
                ViewBag.New = Current.User.HasAccess("WarehouseManagerCreate");
                return View(warehouseManagers);
            }
            catch (Exception ex)
            {
                ViewBag.Alert = $"Error: {ex.Message}";
                return View("Error");
            }
        }

        // GET: WarehouseManager/Create
        [AccessCode("WarehouseManagerCreate")]
        [Authenticated]
        public ActionResult Create()
        {
            llenarEstados();
            bool hayUsuariosDisponibles = llenarUsuarios();
            llenarUsuarios();
            ViewBag.stateList = stateList;
            ViewBag.UsuarioList = userList;
            if (!hayUsuariosDisponibles)
            {
                ViewBag.NoUsersAlert = "No hay usuarios disponibles para asignar como jefe de depósito.";
            }

            return View();
        }

        // POST: WarehouseManager/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessCode("WarehouseManagerCreate")]
        [Authenticated]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                WarehouseManager warehouseManager = new WarehouseManager();
                if (ModelState.IsValid)
                {
                    var existingWarehouseManager = WarehouseManager.Dao.GetByFilter(new
                    {
                        DNI = collection["DNI"]
                    }).FirstOrDefault();

                    if (existingWarehouseManager == null)
                    {
                        warehouseManager.StartDate = Convert.ToDateTime(collection["StartDate"]);
                        warehouseManager.DNI = int.Parse(collection["DNI"]);
                        warehouseManager.Order = WarehouseManager.Dao.GetLastOrder() + 1;
                        warehouseManager.StateWarehouseManager = new StateWarehouseManager{ Id = long.Parse(collection["StateWarehouseManager"])};
                        warehouseManager.User = new User { Id = long.Parse(collection["User"]) };
                        warehouseManager.Save();
                        TempData["Success"] = "Se ha creado correctamente";
                        return RedirectToAction("Index");
                    }

                    llenarEstados();
                    llenarUsuarios();
                    ViewBag.stateList = stateList;
                    ViewBag.UsuarioList = userList;
                    ViewBag.Alert = "Ya existe un jefe de depósito con ese DNI.";
                    return View();
                }

                llenarEstados();
                llenarUsuarios();
                ViewBag.stateList = stateList;
                ViewBag.UsuarioList = userList;
                return View();
            }
            catch (Exception ex)
            {
                llenarEstados();
                llenarUsuarios();
                ViewBag.stateList = stateList;
                ViewBag.UsuarioList = userList;
                ViewBag.Alert = $"Ocurrió un error al crear el jefe de depósito: {ex.Message}";
                return View();
            }
        }

        // GET: WarehouseManager/Edit/5
        [AccessCode("WarehouseManagerEdit")]
        [Authenticated]
        public ActionResult Edit(int id)
        {
            llenarEstados();
            ViewBag.stateList = stateList;
            List<WarehouseManager> list = new List<WarehouseManager>();
            WarehouseManager warehouseManager = WarehouseManager.Dao.Get(id);
            userList.Clear();
            llenarUsuariosParaEdicion(warehouseManager.User.Id);
            ViewBag.UsuarioList = userList;
            list.Add(warehouseManager);
            list.LoadRelation(w => w.StateWarehouseManager);
            list.LoadRelation(w => w.User);
            return View(warehouseManager);
        }

        // POST: WarehouseManager/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessCode("WarehouseManagerEdit")]
        [Authenticated]
        public ActionResult Edit(long id, FormCollection collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    WarehouseManager warehouseManager = WarehouseManager.Dao.Get(id);
                    if (warehouseManager == null)
                    {
                        ViewBag.Alert = "El jefe de depósito no existe.";
                        return RedirectToAction("Index");
                    }

                    var existingWarehouseManager = WarehouseManager.Dao.GetByFilter(new
                    {
                        DNI = collection["DNI"]
                    }).FirstOrDefault();

                    if (existingWarehouseManager == null || existingWarehouseManager.Id == id)
                    {
                        warehouseManager.StartDate = Convert.ToDateTime(collection["StartDate"]);
                        warehouseManager.DNI = int.Parse(collection["DNI"]);
                        warehouseManager.Order = WarehouseManager.Dao.GetLastOrder() + 1;
                        warehouseManager.StateWarehouseManager = new StateWarehouseManager { Id = long.Parse(collection["StateWarehouseManager.Id"]) };
                        warehouseManager.User = new User { Id = long.Parse(collection["User.Id"]) };
                        TempData["Success"] = "Se ha editado correctamente";
                        warehouseManager.Save();
                        return RedirectToAction("Index");
                    }

                    llenarEstados();
                    ViewBag.stateList = stateList;
                    ViewBag.Alert = "Ya existe un jefe de depósito con ese DNI.";
                    return View(warehouseManager);
                }

                llenarEstados();
                ViewBag.stateList = stateList;
                return View(WarehouseManager.Dao.Get(id));
            }
            catch (Exception ex)
            {
                llenarEstados();
                ViewBag.stateList = stateList;
                ViewBag.Alert = $"Ocurrió un error al actualizar el jefe de depósito: {ex.Message}";
                return View(WarehouseManager.Dao.Get(id));
            }
        }

        // GET: WarehouseManager/Delete/5
        [AccessCode("WarehouseManagerDelete")]
        [Authenticated]
        public ActionResult Delete(long id)
        {
            try
            {
                WarehouseManager warehouseManager = WarehouseManager.Dao.Get(id);
                if (warehouseManager == null)
                {
                    TempData["Alert"] = "El jefe de depósito no existe.";
                    return RedirectToAction("Index");
                }

                var purchaseOrders = PurchaseOrder.Dao.GetByFilter(new { WarehouseManager_Id = id }).ToList();
                if (purchaseOrders.Any())
                {
                    TempData["Alert"] = "No se puede eliminar el jefe de depósito porque tiene órdenes de compra asociadas.";
                    return RedirectToAction("Index");
                }

                var deliveryNotes = DeliveryNote.Dao.GetByFilter(new { WarehouseManager_Id = id }).ToList();
                if (deliveryNotes.Any())
                {
                    TempData["Alert"] = "No se puede eliminar el jefe de depósito porque tiene notas de entrega asociadas.";
                    return RedirectToAction("Index");
                }

                warehouseManager.Delete();
                TempData["Success"] = "Se ha eliminado correctamente";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Alert"] = $"Ocurrió un error al eliminar el jefe de depósito: {ex.Message}";
                return RedirectToAction("Index");
            }
        }
    }
}