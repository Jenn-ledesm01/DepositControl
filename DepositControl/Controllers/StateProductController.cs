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
    public class StateProductController : Controller
    {
        private List<SelectListItem> stateList = new List<SelectListItem>();

        private void llenarEstados()
        {
            List<StateProduct> states = StateProduct.Dao.GetByFilter(new
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

        //GET : StateProduct/Index
        [AccessCode("StateProduct")]
        [Authenticated]
        public ActionResult Index(string name = null)
        {
            try
            {
                List<StateProduct> stateProducts = StateProduct.Dao.GetAll().ToList();
                ViewBag.Delete = Current.User.HasAccess("StateProductDelete");
                ViewBag.Edit = Current.User.HasAccess("StateProductEdit");
                ViewBag.New = Current.User.HasAccess("StateProductCreate");
                return View(stateProducts);
            }
            catch (Exception ex)
            {
                ViewBag.Alert = $"Error: {ex.Message}";
                return View("Error");
            }
        }

        [HttpPost]
        [AccessCode("StateProduct")]
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
                List<StateProduct> stateProducts = StateProduct.Dao.GetByFilter(new
                {
                    Name = name
                }).ToList();
                ViewBag.Delete = Current.User.HasAccess("StateProductDelete");
                ViewBag.Edit = Current.User.HasAccess("StateProductEdit");
                ViewBag.New = Current.User.HasAccess("StateProductCreate");
                return View(stateProducts);
            }
            catch (Exception ex)
            {
                ViewBag.Alert = $"Error: {ex.Message}";
                return View("Error");
            }
        }


        // GET: StateProduct/Create
        [AccessCode("StateProductCreate")]
        [Authenticated]
        public ActionResult Create()
        {
            llenarEstados();
            ViewBag.stateList = stateList;
            return View();
        }

        // POST: StateProduct/Create
        [HttpPost]
        [AccessCode("StateProductCreate")]
        [Authenticated]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var existingStateProduct = StateProduct.Dao.GetByFilter(new
                    {
                        Name = collection["Name"]
                    }).FirstOrDefault();

                    if (existingStateProduct == null)
                    {
                        StateProduct stateProduct = new StateProduct
                        {
                            Name = collection["Name"].Trim(),
                            Code = "Active",
                            Order = StateProduct.Dao.GetLastOrder() + 1
                        };
                        stateProduct.Save();
                        TempData["Success"] = "Se ha creado correctamente";
                        return RedirectToAction("Index");
                    }

                    llenarEstados();
                    ViewBag.stateList = stateList;
                    ViewBag.Alert = "Ya existe un estado de producto con ese nombre.";
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
                ViewBag.Alert = "Ocurrió un error al crear el estado de producto.";
                return View();
            }
        }

        // GET: StateProduct/Edit/5
        [AccessCode("StateProductEdit")]
        [Authenticated]
        public ActionResult Edit(long id)
        {
            try
            {
                StateProduct stateProduct = StateProduct.Dao.Get(id);
                if (stateProduct == null)
                {
                    ViewBag.Alert = "El estado de producto no existe.";
                    return RedirectToAction("Index", "Product");
                }

                llenarEstados();
                ViewBag.stateList = stateList;
                GetCodeList();
                return View(stateProduct);
            }
            catch
            {
                ViewBag.Alert = "Ocurrió un error al cargar el estado de producto.";
                return View("Error");
            }
        }

        // POST: StateProduct/Edit/5
        [HttpPost]
        [AccessCode("StateProductEdit")]
        [Authenticated]
        public ActionResult Edit(long id, FormCollection collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    StateProduct stateProduct = StateProduct.Dao.Get(id);
                    if (stateProduct == null)
                    {
                        ViewBag.Alert = "El estado de producto no existe.";
                        return RedirectToAction("Index", "StateProduct");
                    }

                    var existingStateProduct = StateProduct.Dao.GetByFilter(new
                    {
                        Name = collection["Name"]
                    }).FirstOrDefault();

                    if (existingStateProduct == null || existingStateProduct.Id == id)
                    {
                        stateProduct.Name = collection["Name"].Trim();
                        stateProduct.Code = collection["Code"];
                        stateProduct.Save();
                        TempData["Success"] = "Se ha editado correctamente";
                        return RedirectToAction("Index");
                    }

                    llenarEstados();
                    ViewBag.stateList = stateList;
                    GetCodeList();
                    ViewBag.Alert = "Ya existe un estado de producto con ese nombre.";
                    return View(stateProduct);
                }

                llenarEstados();
                ViewBag.stateList = stateList;
                GetCodeList();
                return View(StateProduct.Dao.Get(id));
            }
            catch
            {
                llenarEstados();
                ViewBag.stateList = stateList;
                GetCodeList();
                ViewBag.Alert = "Ocurrió un error al actualizar el estado de producto.";
                return View(StateProduct.Dao.Get(id));
            }
        }

        // GET: StateProduct/Delete/5
        [AccessCode("StateProductDelete")]
        [Authenticated]
        public ActionResult Delete(long id)
        {
            try
            {
                StateProduct stateProduct = StateProduct.Dao.Get(id);
                List<Product> products = Product.Dao.GetByFilter(new { StateProduct_Id = id }) ?? new List<Product>();

                if (products.Count > 0)
                {
                    TempData["Alert"] = "No se puede eliminar el estado de producto porque está asociado a uno o más productos.";
                    return RedirectToAction("Index");
                }
                else
                {
                    if (stateProduct != null)
                    {
                        stateProduct.Delete();
                    }
                    TempData["Success"] = "Se ha eliminado correctamente";
                    return RedirectToAction("Index");
                }
            }
            catch
            {
                TempData["Alert"] = "Ocurrió un error al eliminar el estado de producto.";
                return RedirectToAction("Index");
            }
        }
    }
}