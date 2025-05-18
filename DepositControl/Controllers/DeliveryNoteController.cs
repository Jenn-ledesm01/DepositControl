using DepositControl.Bussines;
using DNF.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DepositControl.Models;
using NPOI.HSSF.Record;
using org.omg.CosNaming.NamingContextPackage;
using System.IO;
using System.Web;
using DNF.Security.Bussines;
using NPOI.SS.Formula.Functions;
using javax.xml.soap;
using com.sun.swing.@internal.plaf.metal.resources;

namespace DepositControl.Controllers
{
    public class DeliveryNoteController : Controller
    {
        private List<SelectListItem> stateDeliveryNoteList = new List<SelectListItem>();
        private List<SelectListItem> productList = new List<SelectListItem>();

        private void FillDropdowns()
        {
            var states = StateDeliveryNote.Dao.GetByFilter(new { Code = "Active" }).ToList();
            foreach (var state in states)
            {
                stateDeliveryNoteList.Add(new SelectListItem
                {
                    Text = state.Name,
                    Value = state.Id.ToString()
                });
            }
            stateDeliveryNoteList.Insert(0, new SelectListItem { Text = "--Seleccione--", Value = "" });

            var allProducts = Product.Dao.GetAll().ToList();
            allProducts.LoadRelation(p => p.StateProduct);
            var filteredProducts = allProducts.Where(p =>
                p.StateProduct.Name == "Activo" ||
                p.StateProduct.Name == "Bajo Stock" ).ToList();

            foreach (var product in filteredProducts)
            {
                productList.Add(new SelectListItem
                {
                    Text = product.Name,
                    Value = product.Id.ToString()
                });
            }

            productList.Insert(0, new SelectListItem { Text = "--Seleccione--", Value = "" });

            var productsPrices = allProducts.ToDictionary(p => p.Id.ToString(), p => p.Price);

            ViewBag.StateDeliveryNoteList = stateDeliveryNoteList;
            ViewBag.ProductList = productList;
            ViewBag.ProductPrices = productsPrices;
        }

        private bool IsAdmin()
        {
            long userId = long.Parse(Session["User"].ToString());
            var userProfile = UserProfile.Dao.GetByFilter(new { User_Id = userId }).FirstOrDefault();
            return userProfile != null && userProfile.Profile.Id == 1;
        }

        // GET: DeliveryNote/Index
        [AccessCode("DeliveryNote")]
        [Authenticated]
        public ActionResult Index(DateTime? dateFilter = null, string stateFilter = null, string numero = null)
        {
            try
            {
                var filters = new 
                { 
                    Code = IsAdmin() ? null : "Active",
                    Date = dateFilter, 
                    StateDeliveryNote_Id = stateFilter, 
                    Number = numero 
                };
                List<DeliveryNote> deliveryNotes = DeliveryNote.Dao.GetByFilter(filters).ToList();
                deliveryNotes.LoadRelation(dn => dn.StateDeliveryNote);

                deliveryNotes = deliveryNotes.OrderByDescending(dn => dn.Date).ToList();

                FillDropdowns();
                ViewBag.StateDeliveryNoteList = stateDeliveryNoteList;
                ViewBag.Edit = Current.User.HasAccess("DeliveryNoteEdit");
                ViewBag.Delete = Current.User.HasAccess("DeliveryNoteDelete");
                ViewBag.New = Current.User.HasAccess("DeliveryNoteCreate");
                ViewBag.Detail = Current.User.HasAccess("DeliveryNoteDetails");
                return View(deliveryNotes);
            }
            catch (Exception ex)
            {
                ViewBag.Alert = $"Error: {ex.Message}";
                return View("Error");
            }
        }

        //POST: DeliveryNote/Index
        [HttpPost]
        [AccessCode("DeliveryNote")]
        [Authenticated]
        public ActionResult Index(FormCollection collection)
        {
            try
            {
                if (collection["clear"] == "true")
                {
                    return RedirectToAction("Index");
                }
                DateTime? dateFilter = null;
                var dateString = collection["dateFilter"];
                if (!string.IsNullOrWhiteSpace(dateString))
                    dateFilter = DateTime.Parse(dateString);

                long? stateId = null;
                var stateString = collection["StateDeliveryNote.Id"];
                if (!string.IsNullOrWhiteSpace(stateString))
                {
                    if (long.TryParse(stateString, out var tmpId))
                        stateId = tmpId;
                    else
                        ModelState.AddModelError("StateDeliveryNote.Id", "El valor de Estado no es válido.");
                }

                var number = collection["number"];

                var filters = new
                {
                    Code = IsAdmin() ? null : "Active",
                    Date = dateFilter,
                    StateDeliveryNote_Id = stateId,
                    Number = number
                };

                var deliveryNotes = DeliveryNote.Dao
                                                  .GetByFilter(filters)
                                                  .ToList();

                deliveryNotes = deliveryNotes.OrderByDescending(dn => dn.Date).ToList();
                deliveryNotes.LoadRelation(dn => dn.StateDeliveryNote);
                FillDropdowns();
                ViewBag.Edit = Current.User.HasAccess("DeliveryNoteEdit");
                ViewBag.Delete = Current.User.HasAccess("DeliveryNoteDelete");
                ViewBag.New = Current.User.HasAccess("DeliveryNoteCreate");
                ViewBag.Detail = Current.User.HasAccess("DeliveryNoteDetails");
                return View(deliveryNotes);
            }
            catch (Exception ex)
            {
                ViewBag.Alert = $"Error: {ex.Message}";
                return View("Error");
            }
        }


        // GET: DeliveryNote/Details
        [AccessCode("DeliveryNoteDetails")]
        [Authenticated]
        public ActionResult Details(int id)
        {
            List<DeliveryNote> list = new List<DeliveryNote>();
            DeliveryNote deliveryNote = DeliveryNote.Dao.Get(id);
            if (deliveryNote == null)
            {
                return HttpNotFound();
            }
            list.Add(deliveryNote);
            list.LoadRelation(dn => dn.StateDeliveryNote);
            list.LoadRelationList(dn => dn.DeliveryNoteDetails);
            list.LoadRelation(po => po.WarehouseManager);
            foreach (var item in list)
            {
                item.DeliveryNoteDetails.LoadRelation(dd => dd.Product);
                if (item.WarehouseManager != null)
                {
                    item.WarehouseManager.User = DNF.Security.Bussines.User.Dao.Get(item.WarehouseManager.User.Id);
                }
            }
            return View(list[0]);
        }

        // GET: DeliveryNote/Create
        [AccessCode("DeliveryNoteCreate")]
        [Authenticated]
        public ActionResult Create()
        {
            FillDropdowns();
            ViewBag.StateList = stateDeliveryNoteList;
            ViewBag.ProductList = productList;
            return View();
        }

        // POST: DeliveryNote/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessCode("DeliveryNoteCreate")]
        [Authenticated]
        public ActionResult Create(FormCollection collection, HttpPostedFileBase file)
        {
            DeliveryNote deliveryNote = new DeliveryNote();

            try
            {
                if (ModelState.IsValid)
                {
                    if (file != null && file.ContentLength > 0)
                    {
                        var extension = Path.GetExtension(file.FileName).ToLower();
                        if (extension != ".pdf")
                        {
                            ViewBag.Alert = "Solo se permiten archivos PDF.";
                            FillDropdowns();
                            return View(deliveryNote);
                        }

                        if (file.ContentLength > 1048576)
                        {
                            ViewBag.Alert = "El archivo no puede superar los 1 MB.";
                            FillDropdowns();
                            return View(deliveryNote);
                        }

                        byte[] fileData;
                        using (var binaryReader = new BinaryReader(file.InputStream))
                        {
                            fileData = binaryReader.ReadBytes(file.ContentLength);
                        }

                        deliveryNote.FileDeliveryNote = Convert.ToBase64String(fileData);
                    }

                    long userId = long.Parse(Session["User"].ToString());
                    var allWarehouseManagers = WarehouseManager.Dao.GetAll();
                    WarehouseManager wm = allWarehouseManagers.FirstOrDefault(x => x.User.Id == userId);

                    const long adminUserId = 1;
                    // Si no existe un WarehouseManager para este usuario y es el admin, creao uno genérico
                    if (wm == null && userId == adminUserId)
                    {
                        var adminUser = DNF.Security.Bussines.User.Dao.Get(userId);
                        wm = new WarehouseManager
                        {
                            User = adminUser,
                            StartDate = DateTime.Now.Date,
                            StateWarehouseManager = new StateWarehouseManager { Id = 1 },
                            Order = WarehouseManager.Dao.GetLastOrder() + 1,
                        };
                        wm.Save();
                    }

                    long stateId = long.Parse(collection["StateDeliveryNote"]);
                    int index = 0;
                    while (true)
                    {
                        string prefix = $"Details[{index}]";
                        if (collection[$"{prefix}.Product"] == null)
                            break;
                        if (string.IsNullOrEmpty(collection[$"{prefix}.Product"]))
                            throw new Exception("Debe seleccionar al menos un producto.");

                        if (string.IsNullOrEmpty(collection[$"{prefix}.Quantity"]))
                            throw new Exception("La cantidad no puede ser cero.");
                        long productId = long.Parse(collection[$"{prefix}.Product"]);
                        int quantity = Convert.ToInt32(collection[$"{prefix}.Quantity"]);
                        Product fullProduct = Product.Dao.Get(productId);
                        if (fullProduct == null)
                            throw new Exception($"El producto con ID {productId} no se encontró.");
                        index++;
                    }

                    deliveryNote.Date = Convert.ToDateTime(collection["Date"]).Date;
                    deliveryNote.TotalAmount = Convert.ToDecimal(collection["TotalAmount"]);
                    deliveryNote.Code = "Active";
                    deliveryNote.Order = PurchaseOrder.Dao.GetLastOrder() + 1;
                    string input = collection["Number"];
                    if (!string.IsNullOrEmpty(input) && input.All(char.IsDigit) && input.Length <= 8)
                    {
                        string fullNumber = input.PadLeft(8, '0');

                        var allDeliveryNotes = DeliveryNote.Dao.GetAll();
                        bool exists = allDeliveryNotes.Any(dn => dn.Number == fullNumber);

                        if (exists)
                        {
                            ViewBag.Alert = "El número ya está registrado.";
                            FillDropdowns();
                            return View(deliveryNote);
                        }
                        else
                        {
                            deliveryNote.Number = fullNumber;
                        }
                    }
                    else
                    {
                        ViewBag.Alert = "El número debe contener solo dígitos numéricos y tener hasta 8 caracteres.";
                    }
                    deliveryNote.StateDeliveryNote = new StateDeliveryNote { Id = long.Parse(collection["StateDeliveryNote"]) };
                    deliveryNote.WarehouseManager = new WarehouseManager { Id = wm.Id };
                    deliveryNote.Save();

                    index = 0;
                    while (true)
                    {
                        string prefix = $"Details[{index}]";
                        if (collection[$"{prefix}.Product"] == null)
                            break;

                        long productId = long.Parse(collection[$"{prefix}.Product"]);
                        int quantity = Convert.ToInt32(collection[$"{prefix}.Quantity"]);

                        Product fullProduct = Product.Dao.Get(productId);
                        if (fullProduct == null)
                            throw new Exception($"El producto con ID {productId} no se encontró.");

                        DeliveryNoteDetail detail = new DeliveryNoteDetail
                        {
                            Product = fullProduct,
                            Quantity = quantity,
                            DeliveryNote = new DeliveryNote { Id = deliveryNote.Id },
                            Code = "Active"
                        };
                        detail.Save();

                        if (stateId == 2 )
                        {
                            fullProduct.Stock.Quantity += quantity;
                            fullProduct.Stock.Save();
                            if (fullProduct.Stock.Quantity <= 5)
                            {
                                fullProduct.StateProduct = new StateProduct { Id = 4 };
                            }
                            else
                            {
                                fullProduct.StateProduct = new StateProduct { Id = 1 };
                            }
                            fullProduct.Save();
                            StockMovement stockMovement = new StockMovement
                            {
                                DateStockMovement = deliveryNote.Date.Date,
                                DeliveryNoteDetail_Product_Id = detail.Product.Id,
                                DeliveryNoteDetail_DeliveryNote_Id = detail.DeliveryNote.Id,
                                Stock = fullProduct.Stock,
                                User = wm.User
                            };
                            long generatedId = StockMovement.Dao.SaveStockMovement(stockMovement);
                        }
                        index++;
                    }
                    TempData["Success"] = "Se ha creado correctamente";
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Alert = "Error al guardar el remito. Verifique los datos ingresados.";
                    FillDropdowns();
                    return View(deliveryNote);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Alert = $"Error al guardar el remito: {ex.Message}";
                FillDropdowns();
                return View(deliveryNote);
            }
        }

        // GET: DeliveryNote/Edit
        [AccessCode("DeliveryNoteEdit")]
        [Authenticated]
        public ActionResult Edit(int id)
        {
            FillDropdowns();
            ViewBag.StateList = stateDeliveryNoteList;
            ViewBag.ProductList = productList;
            List<DeliveryNote> list = new List<DeliveryNote>();
            DeliveryNote deliveryNote = DeliveryNote.Dao.Get(id);
            if (deliveryNote == null)
            {
                return HttpNotFound();
            }
            list.Add(deliveryNote);
            list.LoadRelation(dn => dn.StateDeliveryNote);
            list.LoadRelationList(dn => dn.DeliveryNoteDetails);
            foreach (var item in list)
            {
                item.DeliveryNoteDetails.LoadRelation(dd => dd.Product);
            }
            ViewBag.IsAdmin = IsAdmin();
            return View(deliveryNote);
        }

        // POST: DeliveryNote/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessCode("DeliveryNoteEdit")]
        [Authenticated]
        public ActionResult Edit(FormCollection collection, HttpPostedFileBase file)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    long deliveryNoteId = Convert.ToInt64(collection["Id"]);
                    DeliveryNote deliveryNote = DeliveryNote.Dao.Get(deliveryNoteId);

                    if (deliveryNote == null)
                    {
                        ViewBag.Alert = "El remito no existe.";
                        FillDropdowns();
                        return View(new DeliveryNote());
                    }

                    long newStateId = long.Parse(collection["StateDeliveryNote.Id"]);
                    long oldStateId = deliveryNote.StateDeliveryNote?.Id ?? 0;

                    if (file != null && file.ContentLength > 0)
                    {
                        var extension = Path.GetExtension(file.FileName).ToLower();
                        if (extension != ".pdf")
                        {
                            ViewBag.Alert = "Solo se permiten archivos PDF.";
                            FillDropdowns();
                            return View(deliveryNote);
                        }

                        if (file.ContentLength > 1048576)
                        {
                            ViewBag.Alert = "El archivo no puede superar los 1 MB.";
                            FillDropdowns();
                            return View(deliveryNote);
                        }

                        byte[] fileData;
                        using (var binaryReader = new BinaryReader(file.InputStream))
                        {
                            fileData = binaryReader.ReadBytes(file.ContentLength);
                        }

                        deliveryNote.FileDeliveryNote = Convert.ToBase64String(fileData);
                    }

                    long userId = long.Parse(Session["User"].ToString());
                    var allWarehouseManagers = WarehouseManager.Dao.GetAll();
                    WarehouseManager wm = allWarehouseManagers.FirstOrDefault(x => x.User.Id == userId);
                    const long adminUserId = 1;
                    if (wm == null && userId == adminUserId)
                    {
                        var adminUser = DNF.Security.Bussines.User.Dao.Get(userId);
                        wm = new WarehouseManager
                        {
                            User = adminUser,
                            StartDate = DateTime.Now.Date,
                            StateWarehouseManager = new StateWarehouseManager { Id = 1 },
                            Order = WarehouseManager.Dao.GetLastOrder() + 1,
                        };
                        wm.Save();
                    }
                    if (wm == null)
                    {
                        ViewBag.Alert = "No se encontró un jefe de depósito para el usuario actual.";
                        FillDropdowns();
                        return View(deliveryNote);
                    }
                    ViewBag.IsAdmin = IsAdmin();
                    var existingDetails = DeliveryNoteDetail.Dao.GetDetailsByDeliveryNoteId(deliveryNoteId).ToList();
                    var currentProductIds = new HashSet<long>();
                    var currentQuantities = new Dictionary<long, int>();

                    // --- VALIDACIÓN DE STOCK Y RECOLECCIÓN DE DETALLES NUEVOS ---
                    int index = 0;
                    while (true)
                    {
                        string prefix = $"Details[{index}]";
                        if (collection[$"{prefix}.Product"] == null)
                            break;
                        if (string.IsNullOrEmpty(collection[$"{prefix}.Product"]))
                            throw new Exception("Debe seleccionar al menos un producto.");

                        if (string.IsNullOrEmpty(collection[$"{prefix}.Quantity"]))
                            throw new Exception("La cantidad no puede ser cero.");

                        long productId = long.Parse(collection[$"{prefix}.Product"]);
                        int quantity = Convert.ToInt32(collection[$"{prefix}.Quantity"]);
                        var product = Product.Dao.Get(productId);
                        if (product == null)
                            throw new Exception($"El producto con ID {productId} no se encontró.");

                        currentProductIds.Add(productId);
                        currentQuantities[productId] = quantity;
                        index++;
                    }

                    // Si pasa a Recibida y antes no estaba en recibido => aumentar stock y crear movimientos
                    // si pasa a Recibida y antes estaba en recibido => actualizar detalles, movimientos y stock
                    if (newStateId == 2 && oldStateId != 2 || newStateId == 2 && oldStateId == 2)
                    {
                        // Eliminar detalles que ya no están
                        foreach (var detail in existingDetails)
                        {
                            if (!currentProductIds.Contains(detail.Product.Id))
                            {
                                // Eliminar movimiento de stock asociado
                                StockMovement.Dao.DeleteByDeliveryNoteDetailId(detail.Product.Id, detail.DeliveryNote.Id);
                                var product = Product.Dao.Get(detail.Product.Id);
                                product.Stock.Quantity -= detail.Quantity;
                                product.Stock.Save();
                                if (product.Stock.Quantity <= 5)
                                    product.StateProduct = new StateProduct { Id = 4 };
                                else
                                    product.StateProduct = new StateProduct { Id = 1 };
                                product.Save();
                                DeliveryNoteDetail.Dao.DeleteByDeliveryNoteDetailId(detail.DeliveryNote.Id, detail.Product.Id);
                            }
                        }
                        // Agregar o actualizar detalles
                        foreach (var kvp in currentQuantities)
                        {
                            long productId = kvp.Key;
                            int quantity = kvp.Value;
                            var product = Product.Dao.Get(productId);
                            var existingDetail = existingDetails.FirstOrDefault(d => d.Product.Id == productId);
                            if (existingDetail != null)
                            {
                                if (oldStateId == 2)
                                {
                                    if (quantity > existingDetail.Quantity)
                                    {
                                        var diff = quantity - existingDetail.Quantity;
                                        product.Stock.Quantity += diff;
                                        product.Stock.Save();

                                        if (product.Stock.Quantity <= 5)
                                            product.StateProduct = new StateProduct { Id = 4 };
                                        else
                                            product.StateProduct = new StateProduct { Id = 1 };
                                        product.Save();
                                        DeliveryNoteDetail.Dao.DeleteByDeliveryNoteDetailId(existingDetail.DeliveryNote.Id, existingDetail.Product.Id);
                                        var detail = new DeliveryNoteDetail
                                        {
                                            DeliveryNote = new DeliveryNote { Id = deliveryNoteId },
                                            Product = product,
                                            Quantity = quantity,
                                            Code = "Active"
                                        };
                                        detail.Save();
                                    }
                                    else if (product.Stock.Quantity < quantity)
                                    {
                                        TempData["Alert"] = "El stock no es suficiente para realizar esta operación.";
                                        return RedirectToAction("Index");
                                    }
                                    else
                                    {
                                        var difference = existingDetail.Quantity - quantity;
                                        product.Stock.Quantity -= difference;
                                        if (product.Stock.Quantity <= 5)
                                            product.StateProduct = new StateProduct { Id = 4 };
                                        else
                                            product.StateProduct = new StateProduct { Id = 1 };
                                        product.Stock.Save();
                                        DeliveryNoteDetail.Dao.DeleteByDeliveryNoteDetailId(existingDetail.DeliveryNote.Id, existingDetail.Product.Id);
                                        var detail = new DeliveryNoteDetail
                                        {
                                            DeliveryNote = new DeliveryNote { Id = deliveryNoteId },
                                            Product = product,
                                            Quantity = quantity,
                                            Code = "Active"
                                        };
                                        detail.Save();
                                    }
                                    StockMovement.Dao.DeleteByDeliveryNoteDetailId(productId, deliveryNoteId);
                                    StockMovement stockMovement = new StockMovement
                                    {
                                        DateStockMovement = deliveryNote.Date.Date,
                                        DeliveryNoteDetail_Product_Id = productId,
                                        DeliveryNoteDetail_DeliveryNote_Id = deliveryNoteId,
                                        Stock = product.Stock,
                                        User = wm.User
                                    };
                                    StockMovement.Dao.SaveStockMovement(stockMovement);
                                }
                                else if (oldStateId == 3)
                                {
                                    if (quantity > existingDetail.Quantity)
                                    {
                                        var diff = quantity - existingDetail.Quantity;
                                        product.Stock.Quantity += diff;
                                        product.Stock.Save();
                                        if (product.Stock.Quantity <= 5)
                                            product.StateProduct = new StateProduct { Id = 4 };
                                        else
                                            product.StateProduct = new StateProduct { Id = 1 };
                                        product.Save();
                                        DeliveryNoteDetail.Dao.DeleteByDeliveryNoteDetailId(existingDetail.DeliveryNote.Id, existingDetail.Product.Id);
                                        var detail = new DeliveryNoteDetail
                                        {
                                            DeliveryNote = new DeliveryNote { Id = deliveryNoteId },
                                            Product = product,
                                            Quantity = quantity,
                                            Code = "Active"
                                        };
                                        detail.Save();
                                    } 
                                    else if (quantity < existingDetail.Quantity)
                                    {
                                        var diff = existingDetail.Quantity - quantity;
                                        if (product.Stock.Quantity < diff)
                                        {
                                            TempData["Alert"] = "El stock no es suficiente para realizar esta operación.";
                                            return RedirectToAction("Index");
                                        }
                                        product.Stock.Quantity -= diff;
                                        product.Stock.Save();
                                        if (product.Stock.Quantity <= 5)
                                            product.StateProduct = new StateProduct { Id = 4 };
                                        else
                                            product.StateProduct = new StateProduct { Id = 1 };
                                        product.Save();
                                        DeliveryNoteDetail.Dao.DeleteByDeliveryNoteDetailId(existingDetail.DeliveryNote.Id, existingDetail.Product.Id);
                                        var detail = new DeliveryNoteDetail
                                        {
                                            DeliveryNote = new DeliveryNote { Id = deliveryNoteId },
                                            Product = product,
                                            Quantity = quantity,
                                            Code = "Active"
                                        };
                                        detail.Save();
                                    } 
                                    else
                                    {
                                        product.Stock.Quantity += quantity;
                                        product.Stock.Save();
                                        if (product.Stock.Quantity <= 5)
                                            product.StateProduct = new StateProduct { Id = 4 };
                                        else
                                            product.StateProduct = new StateProduct { Id = 1 };
                                        product.Save();
                                    }
                                    StockMovement.Dao.DeleteByDeliveryNoteDetailId(productId, deliveryNoteId);
                                    StockMovement stockMovement = new StockMovement
                                        {
                                            DateStockMovement = deliveryNote.Date.Date,
                                            DeliveryNoteDetail_Product_Id = productId,
                                            DeliveryNoteDetail_DeliveryNote_Id = deliveryNoteId,
                                            Stock = product.Stock,
                                            User = wm.User
                                        };
                                    StockMovement.Dao.SaveStockMovement(stockMovement);
                                } 
                                else if (oldStateId == 1) 
                                {
                                    if (quantity != existingDetail.Quantity)
                                    {
                                        DeliveryNoteDetail.Dao.DeleteByDeliveryNoteDetailId(existingDetail.DeliveryNote.Id, existingDetail.Product.Id);
                                        var detail = new DeliveryNoteDetail
                                        {
                                            DeliveryNote = new DeliveryNote { Id = deliveryNoteId },
                                            Product = product,
                                            Quantity = quantity,
                                            Code = "Active"
                                        };
                                        detail.Save();
                                        product.Stock.Quantity += quantity;
                                        product.Stock.Save();
                                        if (product.Stock.Quantity <= 5)
                                            product.StateProduct = new StateProduct { Id = 4 };
                                        else
                                            product.StateProduct = new StateProduct { Id = 1 };
                                        product.Save();
                                    }
                                    StockMovement.Dao.DeleteByDeliveryNoteDetailId(productId, deliveryNoteId);
                                    StockMovement stockMovement = new StockMovement
                                    {
                                        DateStockMovement = deliveryNote.Date.Date,
                                        DeliveryNoteDetail_Product_Id = productId,
                                        DeliveryNoteDetail_DeliveryNote_Id = deliveryNoteId,
                                        Stock = product.Stock,
                                        User = wm.User
                                    };
                                    StockMovement.Dao.SaveStockMovement(stockMovement);

                                }

                            }
                            else
                            {
                                var detail = new DeliveryNoteDetail
                                {
                                    DeliveryNote = new DeliveryNote { Id = deliveryNoteId },
                                    Product = product,
                                    Quantity = quantity,
                                    Code = "Active"
                                };
                                detail.Save();

                                product.Stock.Quantity += quantity;
                                product.Stock.Save();

                                StockMovement stockMovement = new StockMovement
                                {
                                    DateStockMovement = deliveryNote.Date.Date,
                                    DeliveryNoteDetail_Product_Id = detail.Product.Id,
                                    DeliveryNoteDetail_DeliveryNote_Id = detail.DeliveryNote.Id,
                                    Stock = product.Stock,
                                    User = wm.User
                                };
                                StockMovement.Dao.SaveStockMovement(stockMovement);
                            }

                            // Actualizar estado del producto
                            if (product.Stock.Quantity <= 5)
                                product.StateProduct = new StateProduct { Id = 4 };
                            else
                                product.StateProduct = new StateProduct { Id = 1 };
                            product.Save();
                        }
                    }
                    // Si cambia de Pendiente de emisión (1) a Rechazado (3): solo guardar estado, eliminar detalles y movimientos
                    else if (oldStateId == 1 && newStateId == 3)
                    {
                        foreach (var detail in existingDetails)
                        {
                            DeliveryNoteDetail.Dao.DeleteByDeliveryNoteDetailId(detail.DeliveryNote.Id, detail.Product.Id);
                        }
                        foreach (var kvp in currentQuantities)
                        {
                            long productId = kvp.Key;
                            int newQuantity = kvp.Value;
                            var product = Product.Dao.Get(productId);
                            var existingDetail = existingDetails.FirstOrDefault(d => d.Product.Id == productId);

                            if (existingDetail != null)
                            {
                                existingDetail.Quantity = newQuantity;
                                existingDetail.Save();
                            }
                            else
                            {
                                var detail = new DeliveryNoteDetail
                                {
                                    DeliveryNote = new DeliveryNote { Id = deliveryNoteId },
                                    Product = product,
                                    Quantity = newQuantity,
                                    Code = "Active"
                                };
                                detail.Save();
                            }
                        }
                    }
                    // Otros casos: actualizar detalles y movimientos, pero no tocar stock
                    else
                    {
                        if(oldStateId == 2 && newStateId != 2) // el admin cambiar de recibido a cualquier otro estado
                        {
                            foreach (var detail in existingDetails)
                            {
                                var product = Product.Dao.Get(detail.Product.Id);
                                if (product.Stock.Quantity < detail.Quantity)
                                {
                                    TempData["Alert"] = "El stock no es suficiente para realizar esta operación.";
                                    return RedirectToAction("Index");
                                }
                                product.Stock.Quantity -= detail.Quantity;
                                product.Stock.Save();
                                if (product.Stock.Quantity <= 5)
                                    product.StateProduct = new StateProduct { Id = 4 };
                                else
                                    product.StateProduct = new StateProduct { Id = 1 };
                                product.Save();
                                StockMovement.Dao.DeleteByDeliveryNoteDetailId(detail.Product.Id, detail.DeliveryNote.Id);
                                if (!currentProductIds.Contains(detail.Product.Id))
                                {
                                    DeliveryNoteDetail.Dao.DeleteByDeliveryNoteDetailId(detail.DeliveryNote.Id, detail.Product.Id);
                                }
                            }
                            if (file != null && file.ContentLength > 0)
                            {
                                var extension = Path.GetExtension(file.FileName).ToLower();
                                if (extension != ".pdf")
                                {
                                    ViewBag.Alert = "Solo se permiten archivos PDF.";
                                    FillDropdowns();
                                    return View(deliveryNote);
                                }

                                if (file.ContentLength > 1048576)
                                {
                                    ViewBag.Alert = "El archivo no puede superar los 1 MB.";
                                    FillDropdowns();
                                    return View(deliveryNote);
                                }

                                byte[] fileData;
                                using (var binaryReader = new BinaryReader(file.InputStream))
                                {
                                    fileData = binaryReader.ReadBytes(file.ContentLength);
                                }

                                deliveryNote.FileDeliveryNote = Convert.ToBase64String(fileData);
                            }
                            foreach (var kvp in currentQuantities)
                            {
                                long productId = kvp.Key;
                                int newQuantity = kvp.Value;
                                var product = Product.Dao.Get(productId);
                                var existingDetail = existingDetails.FirstOrDefault(d => d.Product.Id == productId);

                                if (existingDetail != null)
                                {
                                    existingDetail.Quantity = newQuantity;
                                    existingDetail.Save();
                                }
                                else
                                {
                                    var detail = new DeliveryNoteDetail
                                    {
                                        DeliveryNote = new DeliveryNote { Id = deliveryNoteId },
                                        Product = product,
                                        Quantity = newQuantity,
                                        Code = "Active"
                                    };
                                    detail.Save();
                                }
                            }
                            deliveryNote.Date = Convert.ToDateTime(collection["Date"]).Date;
                            deliveryNote.TotalAmount = Convert.ToDecimal(collection["TotalAmount"]);
                            string input = collection["Number"];
                            if (!string.IsNullOrEmpty(input) && input.All(char.IsDigit) && input.Length <= 8)
                            {
                                string fullNumber = input.PadLeft(8, '0');

                                if (deliveryNote.Number != fullNumber)
                                {
                                    var allDeliveryNotes = DeliveryNote.Dao.GetAll();
                                    bool exists = allDeliveryNotes.Any(dn => dn.Number == fullNumber);

                                    if (exists)
                                    {
                                        ViewBag.Alert = "El número ya está registrado.";
                                        FillDropdowns();
                                        return View(deliveryNote);
                                    }
                                    else
                                    {
                                        deliveryNote.Number = fullNumber;
                                    }
                                }
                            }
                            else
                            {
                                ViewBag.Alert = "El número debe contener solo dígitos numéricos y tener hasta 8 caracteres.";
                            }
                            deliveryNote.StateDeliveryNote = new StateDeliveryNote { Id = long.Parse(collection["StateDeliveryNote.Id"]) };
                            deliveryNote.WarehouseManager = new WarehouseManager { Id = wm.Id };
                            deliveryNote.Save();

                            TempData["Success"] = "Se ha editado correctamente";
                            return RedirectToAction("Index");

                        }
                        // de rechazado a pendiente
                        else
                        {
                            foreach (var detail in existingDetails)
                            {
                                if (!currentProductIds.Contains(detail.Product.Id))
                                {
                                    StockMovement.Dao.DeleteByDeliveryNoteDetailId(detail.Product.Id, detail.DeliveryNote.Id);
                                    DeliveryNoteDetail.Dao.DeleteByDeliveryNoteDetailId(detail.DeliveryNote.Id, detail.Product.Id);
                                }
                            }
                            foreach (var kvp in currentQuantities)
                            {
                                long productId = kvp.Key;
                                int newQuantity = kvp.Value;
                                var product = Product.Dao.Get(productId);
                                var existingDetail = existingDetails.FirstOrDefault(d => d.Product.Id == productId);

                                if (existingDetail != null)
                                {
                                    existingDetail.Quantity = newQuantity;
                                    existingDetail.Save();
                                }
                                else
                                {
                                    var detail = new DeliveryNoteDetail
                                    {
                                        DeliveryNote = new DeliveryNote { Id = deliveryNoteId },
                                        Product = product,
                                        Quantity = newQuantity,
                                        Code = "Active"
                                    };
                                    detail.Save();
                                }
                            }
                        }
                            
                    }
                    deliveryNote.TotalAmount = Convert.ToDecimal(collection["TotalAmount"]);
                    deliveryNote.StateDeliveryNote = new StateDeliveryNote { Id = newStateId };
                    deliveryNote.WarehouseManager = new WarehouseManager { Id = wm.Id };
                    deliveryNote.Save();

                    TempData["Success"] = "Se ha editado correctamente";
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Alert = "Error al actualizar el remito. Verifique los datos ingresados.";
                    FillDropdowns();
                    return View(new DeliveryNote());
                }
            }
            catch (Exception ex)
            {
                ViewBag.Alert = $"Error al actualizar el remito: {ex.Message}";
                FillDropdowns();
                return View(new DeliveryNote());
            }
        }

        public ActionResult DownloadFile(long id, bool inline = false)
        {
            DeliveryNote deliveryNote = DeliveryNote.Dao.Get(id);
            if (deliveryNote == null || string.IsNullOrEmpty(deliveryNote.FileDeliveryNote))
            {
                return HttpNotFound("Archivo no encontrado.");
            }

            byte[] fileBytes = Convert.FromBase64String(deliveryNote.FileDeliveryNote);
            string contentDisposition = inline ? "inline" : "attachment";
            Response.AppendHeader("Content-Disposition", $"{contentDisposition}; filename=DeliveryNote_{id}.pdf");
            return File(fileBytes, "application/pdf");
        }

        // GET: DeliveryNote/Delete
        [AccessCode("DeliveryNoteDelete")]
        [Authenticated]
        public ActionResult Delete(int id)
        {
            try
            {
                List<DeliveryNote> list = new List<DeliveryNote>();
                DeliveryNote deliveryNote = DeliveryNote.Dao.Get(id);
                deliveryNote.DeliveryNoteDetails = DeliveryNoteDetail.Dao.GetDetailsByDeliveryNoteId(id);
                list.Add(deliveryNote);
                list.LoadRelation(dn => dn.StateDeliveryNote);
                bool isAdmin = IsAdmin();
                int stateId = (int)deliveryNote.StateDeliveryNote.Id;
                if ((stateId == 2 || stateId == 3) && !isAdmin)
                {
                    TempData["Alert"] = "No se puede eliminar un remito recibido o rechazado si no es administrador.";
                    return RedirectToAction("Index");
                }
                foreach (var detail in deliveryNote.DeliveryNoteDetails)
                {
                    // Si está en estado 2 y es admin, eliminar movimientos y disminuir stock
                    if (stateId == 2 && isAdmin)
                    {
                        var product = Product.Dao.Get(detail.Product.Id);
                        if (product != null)
                        {
                            if (product.Stock.Quantity>= detail.Quantity)
                            {
                                product.Stock.Quantity -= detail.Quantity;
                                product.Stock.Save();
                                if (product.Stock.Quantity <= 5)
                                    product.StateProduct = new StateProduct { Id = 4 };
                                else
                                    product.StateProduct = new StateProduct { Id = 1 };
                                product.Save();
                            } else
                            {
                                TempData["Alert"] = "No se puede eliminar un remito recibido sin stock de respaldo.";
                                return RedirectToAction("Index");
                            }
                            
                        }
                        StockMovement.Dao.DeleteByDeliveryNoteDetailId(detail.Product.Id, deliveryNote.Id);
                    }

                    detail.Code = "Inactive";
                    detail.Save();
                }

                deliveryNote.Code = "Inactive";
                deliveryNote.Save();

                TempData["Success"] = "Se ha eliminado correctamente";
                return RedirectToAction("Index");
            }
            catch
            {
                TempData["Alert"] = "Ocurrió un error al eliminar el remito.";
                return RedirectToAction("Index");
            }
        }
    }
}