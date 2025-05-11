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
    public class StateDeliveryNoteController : Controller
    {
        private List<SelectListItem> stateList = new List<SelectListItem>();

        private void llenarEstados()
        {
            List<StateDeliveryNote> states = StateDeliveryNote.Dao.GetByFilter(new
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

        //GET : StateDeliveryNote/Index
        [AccessCode("StateDeliveryNote")]
        [Authenticated]
        public ActionResult Index(string name = null)
        {
            try
            {
                List<StateDeliveryNote> stateDeliveryNotes = StateDeliveryNote.Dao.GetAll().ToList();
                ViewBag.Delete = Current.User.HasAccess("StateDeliveryNoteDelete");
                ViewBag.Edit = Current.User.HasAccess("StateDeliveryNoteEdit");
                ViewBag.New = Current.User.HasAccess("StateDeliveryNoteCreate");
                return View(stateDeliveryNotes);
            }
            catch (Exception ex)
            {
                ViewBag.Alert = $"Error: {ex.Message}";
                return View("Error");
            }
        }

        [HttpPost]
        [AccessCode("StateDeliveryNote")]
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
                List<StateDeliveryNote> stateDeliveryNotes = StateDeliveryNote.Dao.GetByFilter(new
                {
                    Name = name
                }).ToList();
                ViewBag.Delete = Current.User.HasAccess("StateDeliveryNoteDelete");
                ViewBag.Edit = Current.User.HasAccess("StateDeliveryNoteEdit");
                ViewBag.New = Current.User.HasAccess("StateDeliveryNoteCreate");
                return View(stateDeliveryNotes);
            }
            catch (Exception ex)
            {
                ViewBag.Alert = $"Error: {ex.Message}";
                return View("Error");
            }
        }


        // GET: StateDeliveryNote/Create
        [AccessCode("StateDeliveryNoteCreate")]
        [Authenticated]
        public ActionResult Create()
        {
            llenarEstados();
            ViewBag.stateList = stateList;
            return View();
        }

        // POST: StateDeliveryNote/Create
        [HttpPost]
        [AccessCode("StateDeliveryNoteCreate")]
        [Authenticated]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var existingStateDeliveryNote = StateDeliveryNote.Dao.GetByFilter(new
                    {
                        Name = collection["Name"]
                    }).FirstOrDefault();

                    if (existingStateDeliveryNote == null)
                    {
                        StateDeliveryNote stateDeliveryNote = new StateDeliveryNote
                        {
                            Name = collection["Name"].Trim(),
                            Code = "Active",
                            Order = StateDeliveryNote.Dao.GetLastOrder() + 1
                        };
                        stateDeliveryNote.Save();
                        TempData["Success"] = "Se ha creado correctamente";
                        return RedirectToAction("Index");
                    }

                    llenarEstados();
                    ViewBag.stateList = stateList;
                    ViewBag.Alert = "Ya existe un estado de remito con ese nombre.";
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
                ViewBag.Alert = "Ocurrió un error al crear el estado de remito.";
                return View();
            }
        }

        // GET: StateDeliveryNote/Edit/5
        [AccessCode("StateDeliveryNoteEdit")]
        [Authenticated]
        public ActionResult Edit(long id)
        {
            try
            {
                StateDeliveryNote stateDeliveryNote = StateDeliveryNote.Dao.Get(id);
                if (stateDeliveryNote == null)
                {
                    ViewBag.Alert = "El estado de remito no existe.";
                    return RedirectToAction("Index", "StateDeliveryNote");
                }

                llenarEstados();
                ViewBag.stateList = stateList;
                GetCodeList();
                return View(stateDeliveryNote);
            }
            catch
            {
                ViewBag.Alert = "Ocurrió un error al cargar el estado de remito.";
                return View("Error");
            }
        }

        // POST: StateDeliveryNote/Edit/5
        [HttpPost]
        [AccessCode("StateDeliveryNoteEdit")]
        [Authenticated]
        public ActionResult Edit(long id, FormCollection collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    StateDeliveryNote stateDeliveryNote = StateDeliveryNote.Dao.Get(id);
                    if (stateDeliveryNote == null)
                    {
                        ViewBag.Alert = "El estado de remito no existe.";
                        return RedirectToAction("Index", "StateDeliveryNote");
                    }

                    var existingStateDeliveryNote = StateDeliveryNote.Dao.GetByFilter(new
                    {
                        Name = collection["Name"]
                    }).FirstOrDefault();

                    if (existingStateDeliveryNote == null || existingStateDeliveryNote.Id == id)
                    {
                        stateDeliveryNote.Name = collection["Name"].Trim();
                        stateDeliveryNote.Code = collection["Code"];
                        stateDeliveryNote.Save();
                        TempData["Success"] = "Se ha editado correctamente";
                        return RedirectToAction("Index");
                    }

                    llenarEstados();
                    ViewBag.stateList = stateList;
                    GetCodeList();
                    ViewBag.Alert = "Ya existe un estado de remito con ese nombre.";
                    return View(stateDeliveryNote);
                }

                llenarEstados();
                ViewBag.stateList = stateList;
                GetCodeList();
                ViewBag.Alert = "Los datos ingresados no son válidos. Por favor, revisa los campos.";
                return View(StateDeliveryNote.Dao.Get(id));
            }
            catch
            {
                llenarEstados();
                ViewBag.stateList = stateList;
                GetCodeList();
                ViewBag.Alert = "Ocurrió un error al actualizar el estado de remito.";
                return View(StateDeliveryNote.Dao.Get(id));
            }
        }

        // GET: StateDeliveryNote/Delete/5
        [AccessCode("StateDeliveryNoteDelete")]
        [Authenticated]
        public ActionResult Delete(long id)
        {
            try
            {
                StateDeliveryNote stateDeliveryNote = StateDeliveryNote.Dao.Get(id);
                if (stateDeliveryNote == null)
                {
                    TempData["Alert"] = "El estado de remito no existe.";
                    return RedirectToAction("Index");
                }
                List<DeliveryNote> deliveryNotes = DeliveryNote.Dao.GetByFilter(new { StateDeliveryNote_Id = id }) ?? new List<DeliveryNote>();

                if (deliveryNotes.Count > 0)
                {
                    TempData["Alert"] = "No se puede eliminar el estado de remito porque está asociado a uno o más remitos.";
                    return RedirectToAction("Index");
                }
                else
                {
                    if (stateDeliveryNote != null)
                    {
                        stateDeliveryNote.Delete();
                    }
                    TempData["Success"] = "Se ha eliminado correctamente";
                    return RedirectToAction("Index");
                }
            }
            catch
            {
                TempData["Alert"] = "Ocurrió un error al eliminar el estado de remito.";
                return RedirectToAction("Index");
            }
        }
    }
}