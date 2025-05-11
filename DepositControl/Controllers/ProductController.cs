using DepositControl.Bussines;
using DNF.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DepositControl.Models;
using NPOI.SS.Formula.Functions;
using com.sun.xml.@internal.bind.v2.model.core;
using DNF.Security.Bussines;

namespace DepositControl.Controllers
{
    public class ProductController : Controller
    {
        private List<SelectListItem> stockList = new List<SelectListItem>();
        private List<SelectListItem> stateProductList = new List<SelectListItem>();

        private void FillDropdowns()
        {
            List<StateProduct> stateProducts = StateProduct.Dao.GetByFilter(new { }).ToList();
            foreach (var item in stateProducts)
            {
                stateProductList.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Id.ToString()
                });
            }
            stateProductList.Insert(0, new SelectListItem { Text = "--Seleccione--", Value = "" });
        }

        // GET: Product/Index
        [AccessCode("Product")]
        [Authenticated]
        public ActionResult Index(string name = null)
        {
            try
            {
                List<Product> products = Product.Dao.GetByFilter(new
                {
                    Name = name
                }).ToList();
                products.LoadRelation(p => p.StateProduct);
                products.LoadRelation(p => p.Stock);
                ViewBag.Edit = Current.User.HasAccess("ProductEdit");
                ViewBag.Delete = Current.User.HasAccess("ProductDelete");
                ViewBag.New = Current.User.HasAccess("ProductCreate");
                return View(products);
            }
            catch (Exception ex)
            {
                ViewBag.Alert = $"Error: {ex.Message}";
                return View("Error");
            }
        }

        [HttpPost]
        [AccessCode("Product")]
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
                List<Product> products = Product.Dao.GetByFilter(new
                {
                    Name = name
                }).ToList();
                products.LoadRelation(p => p.StateProduct);
                products.LoadRelation(p => p.Stock);
                ViewBag.Edit = Current.User.HasAccess("ProductEdit");
                ViewBag.Delete = Current.User.HasAccess("ProductDelete");
                ViewBag.New = Current.User.HasAccess("ProductCreate");
                return View(products);
            }
            catch (Exception ex)
            {
                ViewBag.Alert = $"Error: {ex.Message}";
                return View("Error");
            }
        }

        // GET: Product/Create
        [AccessCode("ProductCreate")]
        [Authenticated]
        public ActionResult Create()
        {
            FillDropdowns();
            ViewBag.StateProductList = stateProductList;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessCode("ProductCreate")]
        [Authenticated]
        public ActionResult Create(FormCollection collection)
        {
            Product product = new Product();
            try
            {
                if (ModelState.IsValid)
                {
                    var existingProduct = Product.Dao.GetByFilter(new
                    {
                        Name = collection["Name"]
                    }).FirstOrDefault();

                    if (existingProduct == null)
                    {
                        product.Name = collection["Name"].Trim();
                        product.Description = collection["Description"].Trim();
                        product.Price = decimal.Parse(collection["Price"]);
                        product.Code = "Active";
                        product.Order = Product.Dao.GetLastOrder() + 1;
                        product.StateProduct = new StateProduct { Id = 4 };

                        // Crear un nuevo Stock con Quantity = 0
                        var stock = new Stock
                        {
                            Quantity = 0
                        };
                        stock.Save();
                        product.Stock = stock;

                        product.Save();
                        TempData["Success"] = "Se ha creado correctamente";
                        return RedirectToAction("Index");
                    }

                    FillDropdowns();
                    ViewBag.StateProductList = stateProductList;
                    ViewBag.Alert = "Ya existe un producto con ese nombre.";
                    return View();
                }
                else
                {
                    ViewBag.Alert = "Error al guardar el producto. Verifique los datos ingresados.";
                    FillDropdowns();
                    ViewBag.StateProductList = stateProductList;
                    return View(product);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Alert = $"Error al guardar el producto: {ex.Message}";
                FillDropdowns();
                ViewBag.StateProductList = stateProductList;
                return View(product);
            }
        }

        // GET: Product/Edit/5
        [AccessCode("ProductEdit")]
        [Authenticated]
        public ActionResult Edit(int id)
        {
            FillDropdowns();
            ViewBag.StateProductList = stateProductList;
            List<Product> list = new List<Product>();
            Product product = Product.Dao.Get(id);
            list.Add(product);
            list.LoadRelation(p => p.StateProduct);
            return View(product);
        }

        // POST: Product/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessCode("ProductEdit")]
        [Authenticated]
        public ActionResult Edit(FormCollection collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    long productId = Convert.ToInt64(collection["Id"]);
                    Product product = Product.Dao.Get(productId);

                    if (product == null)
                    {
                        ViewBag.Alert = "El producto no existe.";
                        FillDropdowns();
                        return View(new Product());
                    }

                    var existingProduct = Product.Dao.GetByFilter(new
                    {
                        Name = collection["Name"]
                    }).FirstOrDefault();

                    if (existingProduct == null || existingProduct.Id == productId)
                    {
                        
                        
                        var stock = Stock.Dao.Get(productId);
                        product.Name = collection["Name"].Trim();
                        product.Description = collection["Description"].Trim();
                        product.Price = decimal.Parse(collection["Price"]);
                        product.Code = collection["Code"];
                        //product.Stock = new Stock { Id = long.Parse(collection["Stock.Id"]) };
                        //var stock = Stock.Dao.Get(1);
                        product.Stock = stock;
                        product.StateProduct = new StateProduct { Id = long.Parse(collection["StateProduct.Id"]) };
                        product.Save();
                        TempData["Success"] = "Se ha editado correctamente";
                        return RedirectToAction("Index");
                    }

                    FillDropdowns();
                    ViewBag.StateProductList = stateProductList;
                    ViewBag.Alert = "Ya existe un producto con ese nombre.";
                    return View(product);
                }
                else
                {
                    FillDropdowns();
                    ViewBag.StateProductList = stateProductList;
                    ViewBag.Alert = "Error al actualizar el producto. Verifique los datos ingresados.";
                    return View(new Product());
                }
            }
            catch(Exception ex)
            {
                ViewBag.Alert = $"Error al actualizar el producto: {ex.Message}";
                FillDropdowns();
                ViewBag.StockList = stockList;
                ViewBag.StateProductList = stateProductList;
                return View(new Product());
            }
        }

        [AccessCode("ProductDelete")]
        [Authenticated]
        public ActionResult Delete(int id)
        {
            try
            {
                Product product = Product.Dao.Get(id);
                // Verificamos si el producto tiene stock
                if (product.Stock.Quantity > 0)
                {
                    TempData["Alert"] = "No se puede eliminar un producto que tiene stock.";
                    return RedirectToAction("Index");
                }
                StateProduct stateProduct = StateProduct.Dao.GetByFilter(new { Name = "Inactivo" }).FirstOrDefault();
                product.Code = "Inactive";
                product.StateProduct = stateProduct;
                product.Save();
                TempData["Success"] = "Se ha eliminado correctamente";
                return RedirectToAction("Index");
            }
            catch
            {
                TempData["Alert"] = "Ocurrió un error al eliminar el producto.";
                return RedirectToAction("Index");
            }
        }
    }
}