using DepositControl.Bussines;
using DNF.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DepositControl.Models;
using com.sun.org.apache.bcel.@internal.classfile;
using System.Diagnostics;
using DNF.Security.Bussines;
using NPOI.SS.Formula.Functions;
using static com.sun.management.VMOption;
using static com.sun.tools.@internal.xjc.reader.xmlschema.bindinfo.BIConversion;

namespace DepositControl.Controllers
{
    public class PurchaseOrderController : Controller
    {
        private List<SelectListItem> statePurchaseOrderList = new List<SelectListItem>();
        private List<SelectListItem> productList = new List<SelectListItem>();
        private List<SelectListItem> salePointList = new List<SelectListItem>();

        private void FillDropdowns()
        {
            var states = StatePurchaseOrder.Dao.GetByFilter(new { Code = "Active" }).ToList();
            foreach (var state in states)
            {
                statePurchaseOrderList.Add(new SelectListItem
                {
                    Text = state.Name,
                    Value = state.Id.ToString()
                });
            }
            statePurchaseOrderList.Insert(0, new SelectListItem { Text = "--Seleccione--", Value = "" });

            var allProducts = Product.Dao.GetAll().ToList();
            allProducts.LoadRelation(p => p.StateProduct);
            var filteredProducts = allProducts.Where(p =>
                p.StateProduct.Name == "Activo" ||
                p.StateProduct.Name == "Bajo Stock" ||
                p.StateProduct.Name == "Defectuoso").ToList();

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
            
            var salePoints = SalePoint.Dao.GetByFilter(new { Code = "Active" }).ToList();
            foreach (var salePoint in salePoints)
            {
                salePointList.Add(new SelectListItem
                {
                    Text = salePoint.Name,
                    Value = salePoint.Id.ToString()
                });
            }
            salePointList.Insert(0, new SelectListItem { Text = "--Seleccione--", Value = "" });

            ViewBag.StatePurchaseOrderList = statePurchaseOrderList;
            ViewBag.ProductList = productList;
            ViewBag.SalePointList = salePointList;
            ViewBag.ProductPrices = productsPrices;
        }

        private bool IsAdmin()
        {
            long userId = long.Parse(Session["User"].ToString());
            var userProfile = UserProfile.Dao.GetByFilter(new { User_Id = userId }).FirstOrDefault();
            return userProfile != null && userProfile.Profile.Id == 1;
        }

        // GET: PurchaseOrder/Index
        [AccessCode("PurchaseOrder")]
        [Authenticated]
        public ActionResult Index(DateTime? dateFilter = null, string stateFilter = null, string numero = null)
        {
            try
            {
                var filters = new { Code = "Active", Date = dateFilter, StatePurchaseOrder_Id = stateFilter, Number = numero };
                List<PurchaseOrder> purchaseOrders = PurchaseOrder.Dao.GetByFilter(filters).ToList();
                purchaseOrders.LoadRelation(po => po.StatePurchaseOrder);
                purchaseOrders.LoadRelation(po => po.SalePoint);

                purchaseOrders = purchaseOrders.OrderByDescending(po => po.Date).ToList();

                FillDropdowns();
                ViewBag.stateList = statePurchaseOrderList;
                ViewBag.Edit = Current.User.HasAccess("PurchaseOrderEdit");
                ViewBag.Delete = Current.User.HasAccess("PurchaseOrderDelete");
                ViewBag.New = Current.User.HasAccess("PurchaseOrderCreate");
                ViewBag.Detail = Current.User.HasAccess("PurchaseOrderDetails");
                return View(purchaseOrders);
            }
            catch (Exception ex)
            {
                ViewBag.Alert = $"Error: {ex.Message}";
                return View("Error");
            }
        }

        // POST: PurchaseOrder/Index
        [HttpPost]
        [AccessCode("PurchaseOrder")]
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
                var stateString = collection["StatePurchaseOrder.Id"];
                if (!string.IsNullOrWhiteSpace(stateString))
                {
                    if (long.TryParse(stateString, out var tmpId))
                        stateId = tmpId;
                    else
                        ModelState.AddModelError("StatePurchaseOrder.Id", "El valor de Estado no es válido.");
                }

                var number = collection["number"];

                var filters = new
                {
                    Code = "Active",
                    Date = dateFilter,
                    StatePurchaseOrder_Id = stateId,
                    Number = number
                };

                var purchaseOrders = PurchaseOrder.Dao
                                                  .GetByFilter(filters)
                                                  .ToList();

                purchaseOrders = purchaseOrders.OrderByDescending(po => po.Date).ToList();

                purchaseOrders.LoadRelation(po => po.StatePurchaseOrder);
                purchaseOrders.LoadRelation(po => po.SalePoint);
                FillDropdowns();
                ViewBag.Edit = Current.User.HasAccess("PurchaseOrderEdit");
                ViewBag.Delete = Current.User.HasAccess("PurchaseOrderDelete");
                ViewBag.New = Current.User.HasAccess("PurchaseOrderCreate");
                ViewBag.Detail = Current.User.HasAccess("PurchaseOrderDetails");
                return View(purchaseOrders);
            }
            catch (Exception ex)
            {
                ViewBag.Alert = $"Error: {ex.Message}";
                return View("Error");
            }
        }

        // GET: PurchaseOrder/Details
        [AccessCode("PurchaseOrderDetails")]
        [Authenticated]
        public ActionResult Details(int id)
        {
            List<PurchaseOrder> list = new List<PurchaseOrder>();
            PurchaseOrder purchaseOrder = PurchaseOrder.Dao.Get(id);
            if (purchaseOrder == null)
            {
                return HttpNotFound();
            }
            list.Add(purchaseOrder);
            list.LoadRelation(po => po.StatePurchaseOrder);
            list.LoadRelation(dn => dn.SalePoint);
            list.LoadRelationList(po => po.PurchaseOrderDetails);
            list.LoadRelation(po => po.WarehouseManager);
            
            foreach (var item in list)
            {
                item.PurchaseOrderDetails.LoadRelation(pd => pd.Product);
                if (item.WarehouseManager != null)
                {
                    item.WarehouseManager.User = DNF.Security.Bussines.User.Dao.Get(item.WarehouseManager.User.Id);
                }
            }
            return View(list[0]);
        }

        // GET: PurchaseOrder/Create
        [AccessCode("PurchaseOrderCreate")]
        [Authenticated]
        public ActionResult Create()
        {
            FillDropdowns();
            ViewBag.stateList = statePurchaseOrderList;
            ViewBag.ProductList = productList;
            ViewBag.SalePointList = salePointList;
            return View();
        }

        // POST: PurchaseOrder/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessCode("PurchaseOrderCreate")]
        [Authenticated]
        public ActionResult Create(FormCollection collection)
        {
            PurchaseOrder purchaseOrder = new PurchaseOrder();

            try
            {
                if (ModelState.IsValid)
                {
                    long userId = long.Parse(Session["User"].ToString());
                    var allWarehouseManagers = WarehouseManager.Dao.GetAll();
                    WarehouseManager wm = allWarehouseManagers.FirstOrDefault(x => x.User.Id == userId);
                  
                    const long adminUserId = 1;
                    // Si no existe un WarehouseManager para este usuario y es el admin, creao uno genérico
                    if (wm == null && userId == adminUserId)
                    {
                        var adminUser = DNF.Security.Bussines.User.Dao.Get(userId);
                        wm = new WarehouseManager 
                        {   User = adminUser,
                            StartDate = DateTime.Now.Date,
                            StateWarehouseManager = new StateWarehouseManager { Id = 1 },
                            Order = WarehouseManager.Dao.GetLastOrder() + 1,
                        };
                        wm.Save();
                    }
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
                        if (fullProduct.Stock == null || fullProduct.Stock.Quantity < quantity)
                            throw new Exception($"Stock insuficiente para el producto {fullProduct.Name}. Solo hay {fullProduct.Stock?.Quantity ?? 0} unidades disponibles.");

                        index++;
                    }

                    purchaseOrder.Date = Convert.ToDateTime(collection["Date"]).Date;
                    purchaseOrder.TotalAmount = Convert.ToDecimal(collection["TotalAmount"]);
                    purchaseOrder.Code = "Active";
                    purchaseOrder.Order = PurchaseOrder.Dao.GetLastOrder() + 1;
                    string input = collection["Number"];
                    if (!string.IsNullOrEmpty(input) && input.All(char.IsDigit) && input.Length <= 8)
                    {
                        string fullNumber = input.PadLeft(8, '0');

                        var allPurchaseOrders = PurchaseOrder.Dao.GetAll();
                        bool exists = allPurchaseOrders.Any(dn => dn.Number == fullNumber);

                        if (exists)
                        {
                            ViewBag.Alert = "El número ya está registrado.";
                            FillDropdowns();
                            return View(purchaseOrder);
                        }
                        else
                        {
                            purchaseOrder.Number = fullNumber;
                        }
                    }
                    else
                    {
                        ViewBag.Alert = "El número debe contener solo dígitos numéricos y tener hasta 8 caracteres.";
                        FillDropdowns();
                        return View(purchaseOrder);
                    }
                    purchaseOrder.StatePurchaseOrder = new StatePurchaseOrder { Id = long.Parse(collection["StatePurchaseOrder"]) };
                    purchaseOrder.WarehouseManager = new WarehouseManager { Id = wm.Id };
                    purchaseOrder.SalePoint = new SalePoint { Id = long.Parse(collection["SalePoint"]) };
                    purchaseOrder.Save();

                    long stateId = long.Parse(collection["StatePurchaseOrder"]);
                    index = 0;
                    while (true)
                    {
                        string prefix = $"Details[{index}]";
                        if (collection[$"{prefix}.Product"] == null)
                            break;
                        long productId = long.Parse(collection[$"{prefix}.Product"]);
                        int quantity = Convert.ToInt32(collection[$"{prefix}.Quantity"]);
                        Product fullProduct = Product.Dao.Get(productId);
                        PurchaseOrderDetail detail = new PurchaseOrderDetail
                        {
                            Product = fullProduct,
                            Quantity = quantity,
                            PurchaseOrder = new PurchaseOrder { Id = purchaseOrder.Id },
                            Code = "Active"
                        };
                        detail.Save();

                        if (stateId == 1 || stateId == 2) //Si está en pendiente o entregada, disminuyo el stock
                        {
                            fullProduct.Stock.Quantity -= quantity;
                            fullProduct.Stock.Save();
                            if (fullProduct.Stock.Quantity <= 5)
                            {
                                fullProduct.StateProduct = new StateProduct { Id = 4 };
                            }else
                            {
                                fullProduct.StateProduct = new StateProduct { Id = 1 };
                            }
                            fullProduct.Save();
                            StockMovement stockMovement = new StockMovement
                            {
                                DateStockMovement = purchaseOrder.Date.Date,
                                PurchaseOrderDetail_Product_Id = detail.Product.Id,
                                PurchaseOrderDetail_PurchaseOrder_Id = detail.PurchaseOrder.Id,
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
                    ViewBag.Alert = "Error al guardar la orden de compra. Verifique los datos ingresados.";
                    FillDropdowns();
                    return View(purchaseOrder);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Alert = $"Error al guardar la orden de compra: {ex.Message}";
                FillDropdowns();
                return View(purchaseOrder);
            }
        }

        // GET: PurchaseOrder/Edit
        [AccessCode("PurchaseOrderEdit")]
        [Authenticated]
        public ActionResult Edit(int id)
        {
            FillDropdowns();
            ViewBag.stateList = statePurchaseOrderList;
            ViewBag.ProductList = productList;
            ViewBag.SalePointList = salePointList;
            List<PurchaseOrder> list = new List<PurchaseOrder>();
            PurchaseOrder purchaseOrder = PurchaseOrder.Dao.Get(id);
            if (purchaseOrder == null)
            {
                return HttpNotFound();
            }
            list.Add(purchaseOrder);
            list.LoadRelation(po => po.StatePurchaseOrder);
            list.LoadRelation(dn => dn.SalePoint);
            list.LoadRelationList(po => po.PurchaseOrderDetails);
            foreach(var item in list)
            {
                item.PurchaseOrderDetails.LoadRelation(pd => pd.Product);
            }
            ViewBag.IsAdmin = IsAdmin();
            return View(purchaseOrder);
        }


        // POST: PurchaseOrder/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AccessCode("PurchaseOrderEdit")]
        [Authenticated]
        public ActionResult Edit(FormCollection collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    long purchaseOrderId = Convert.ToInt64(collection["Id"]);
                    PurchaseOrder purchaseOrder = PurchaseOrder.Dao.Get(purchaseOrderId);

                    if (purchaseOrder == null)
                    {
                        ViewBag.Alert = "La orden de compra no existe.";
                        FillDropdowns();
                        return View(new PurchaseOrder());
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
                        return View(new PurchaseOrder());
                    }
                    ViewBag.IsAdmin = IsAdmin();
                    long newStateId = long.Parse(collection["StatePurchaseOrder.Id"]);
                    long oldStateId = purchaseOrder.StatePurchaseOrder.Id;

                    var existingDetails = PurchaseOrderDetail.Dao.GetDetailsByPurchaseOrderId(purchaseOrderId).ToList();
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

                        // Validar stock solo si va a disminuir (estado entregada y pendiente)
                        if (newStateId == 1 || newStateId == 2)
                        {
                            int available = product.Stock?.Quantity ?? 0;
                            var existingDetail = existingDetails.FirstOrDefault(d => d.Product.Id == productId);
                            int oldQuantity = existingDetail?.Quantity ?? 0;
                            int diff = quantity - oldQuantity;
                            if (diff > 0 && available < diff)
                                throw new Exception($"Stock insuficiente para el producto {product.Name}. Solo hay {available} unidades disponibles.");
                            if (existingDetail == null && available < quantity)
                                throw new Exception($"Stock insuficiente para el producto {product.Name}. Solo hay {available} unidades disponibles.");
                        }

                        currentProductIds.Add(productId);
                        currentQuantities[productId] = quantity;
                        index++;
                    }

                    // De Cancelada/Rechazada a Pendiente/Entregada: descontar stock y crear movimientos
                    if ((oldStateId == 3 || oldStateId == 4) && (newStateId == 1 || newStateId == 2))
                    {
                        foreach (var detail in existingDetails)
                        {
                            PurchaseOrderDetail.Dao.DeleteByPurchaseOrderDetailId(purchaseOrderId, detail.Product.Id);
                        }
                        foreach (var kvp in currentQuantities)
                        {
                            long productId = kvp.Key;
                            int newQuantity = kvp.Value;
                            var product = Product.Dao.Get(productId);

                            var existingDetail = existingDetails.FirstOrDefault(d => d.Product.Id == productId);
                            if (existingDetail == null)
                            {
                                if (product.Stock.Quantity < newQuantity)
                                {
                                    TempData["Alert"] = "El stock no es suficiente para realizar esta operación.";
                                    return RedirectToAction("Index");
                                }
                                var detail = new PurchaseOrderDetail
                                {
                                    PurchaseOrder = new PurchaseOrder { Id = purchaseOrderId },
                                    Product = product,
                                    Quantity = newQuantity,
                                    Code = "Active"
                                };
                                detail.Save();
                                product.Stock.Quantity -= newQuantity;
                                product.Stock.Save();
                            }
                            else 
                            {
                                if (product.Stock.Quantity < newQuantity)
                                {
                                    TempData["Alert"] = "El stock no es suficiente para realizar esta operación.";
                                    return RedirectToAction("Index");
                                }
                                existingDetail.Quantity = newQuantity;
                                existingDetail.Save();
                                product.Stock.Quantity -= newQuantity;
                                product.Stock.Save();
                                PurchaseOrderDetail.Dao.DeleteByPurchaseOrderDetailId(purchaseOrderId, productId);
                                var detail = new PurchaseOrderDetail
                                {
                                    PurchaseOrder = new PurchaseOrder { Id = purchaseOrderId },
                                    Product = product,
                                    Quantity = newQuantity,
                                    Code = "Active"
                                };
                                detail.Save();
                            }
                            if (product.Stock.Quantity <= 5)
                                product.StateProduct = new StateProduct { Id = 4 };
                            else
                                product.StateProduct = new StateProduct { Id = 1 };
                            product.Save();
                            StockMovement stockMovement = new StockMovement
                            {
                                DateStockMovement = purchaseOrder.Date.Date,
                                PurchaseOrderDetail_Product_Id = productId,
                                PurchaseOrderDetail_PurchaseOrder_Id = purchaseOrderId,
                                Stock = product.Stock,
                                User = wm.User
                            };
                            StockMovement.Dao.SaveStockMovement(stockMovement);
                        }
                    }
                    // De Pendiente/Entregada a Cancelada/Rechazada: sumar stock y eliminar movimientos
                    else if ((oldStateId == 1 || oldStateId == 2 ) && (newStateId == 3 || newStateId == 4))
                    {
                        foreach (var detail in existingDetails)
                        {
                            var prod = Product.Dao.Get(detail.Product.Id);
                            prod.Stock.Quantity += detail.Quantity;
                            prod.Stock.Save();
                            if (prod.Stock.Quantity <= 5)
                                prod.StateProduct = new StateProduct { Id = 4 };
                            else
                                prod.StateProduct = new StateProduct { Id = 1 };
                            prod.Save();
                            if (!currentProductIds.Contains(detail.Product.Id))
                            {
                                PurchaseOrderDetail.Dao.DeleteByPurchaseOrderDetailId(detail.PurchaseOrder.Id, detail.Product.Id);
                            }
                            StockMovement.Dao.DeleteByPurchaseOrderDetailId(detail.Product.Id, detail.PurchaseOrder.Id);

                        }
                        foreach (var kvp in currentQuantities)
                        {
                            long productId = kvp.Key;
                            int newQuantity = kvp.Value;
                            var product = Product.Dao.Get(productId);
                            var existingDetail = existingDetails.FirstOrDefault(d => d.Product.Id == productId);
                            if (existingDetail == null)
                            {
                                var detail = new PurchaseOrderDetail
                                {
                                    PurchaseOrder = new PurchaseOrder { Id = purchaseOrderId },
                                    Product = product,
                                    Quantity = newQuantity,
                                    Code = "Active"
                                };
                                detail.Save();
                            }
                            else
                            {
                                existingDetail.Quantity = newQuantity;
                                existingDetail.Save();
                                PurchaseOrderDetail.Dao.DeleteByPurchaseOrderDetailId(purchaseOrderId, productId);
                                var detail = new PurchaseOrderDetail
                                {
                                    PurchaseOrder = new PurchaseOrder { Id = purchaseOrderId },
                                    Product = product,
                                    Quantity = newQuantity,
                                    Code = "Active"
                                };
                                detail.Save();
                            }
                        }
                    }
                    // Si pasa a Pendiente o Entregada (edición normal)
                    else if (newStateId == 1 || newStateId == 2)
                    {
                        foreach (var detail in existingDetails)
                        {
                            if (!currentProductIds.Contains(detail.Product.Id))
                            {
                                var prod = Product.Dao.Get(detail.Product.Id);
                                prod.Stock.Quantity += detail.Quantity;
                                prod.Stock.Save();
                                if (prod.Stock.Quantity <= 5)
                                    prod.StateProduct = new StateProduct { Id = 4 };
                                else
                                    prod.StateProduct = new StateProduct { Id = 1 };
                                prod.Save();
                                StockMovement.Dao.DeleteByPurchaseOrderDetailId(detail.Product.Id, detail.PurchaseOrder.Id);
                                PurchaseOrderDetail.Dao.DeleteByPurchaseOrderDetailId(detail.PurchaseOrder.Id, detail.Product.Id);
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
                                if (newQuantity > existingDetail.Quantity)
                                {
                                    if (product.Stock.Quantity < newQuantity)
                                    {
                                        TempData["Alert"] = "El stock no es suficiente para realizar esta operación.";
                                        return RedirectToAction("Index");
                                    }
                                    int diff = newQuantity - existingDetail.Quantity;
                                    product.Stock.Quantity -= diff;
                                    product.Stock.Save();
                                    existingDetail.Quantity = newQuantity;
                                    existingDetail.Save();
                                }
                                else if (existingDetail.Quantity > newQuantity)
                                {
                                    int diff = existingDetail.Quantity - newQuantity;
                                    product.Stock.Quantity += diff;
                                    product.Stock.Save();
                                    existingDetail.Quantity = newQuantity;
                                    existingDetail.Save();
                                }
                                StockMovement.Dao.DeleteByPurchaseOrderDetailId(productId, purchaseOrderId);
                                StockMovement stockMovement = new StockMovement
                                {
                                    DateStockMovement = purchaseOrder.Date.Date,
                                    PurchaseOrderDetail_Product_Id = productId,
                                    PurchaseOrderDetail_PurchaseOrder_Id = purchaseOrderId,
                                    Stock = product.Stock,
                                    User = wm.User
                                };
                                StockMovement.Dao.SaveStockMovement(stockMovement);
                            }
                            else
                            {
                                var detail = new PurchaseOrderDetail
                                {
                                    PurchaseOrder = new PurchaseOrder { Id = purchaseOrderId },
                                    Product = product,
                                    Quantity = newQuantity,
                                    Code = "Active"
                                };
                                detail.Save();

                                product.Stock.Quantity -= newQuantity;
                                product.Stock.Save();

                                StockMovement stockMovement = new StockMovement
                                {
                                    DateStockMovement = purchaseOrder.Date.Date,
                                    PurchaseOrderDetail_Product_Id = detail.Product.Id,
                                    PurchaseOrderDetail_PurchaseOrder_Id = detail.PurchaseOrder.Id,
                                    Stock = product.Stock,
                                    User = wm.User
                                };
                                StockMovement.Dao.SaveStockMovement(stockMovement);
                            }

                            if (product.Stock.Quantity <= 5)
                                product.StateProduct = new StateProduct { Id = 4 };
                            else
                                product.StateProduct = new StateProduct { Id = 1 };
                            product.Save();
                        }
                    }
                    else if (newStateId == 3 || newStateId == 4)
                    {
                        foreach (var detail in existingDetails)
                        { 
                            if (!currentProductIds.Contains(detail.Product.Id))
                            {
                                PurchaseOrderDetail.Dao.DeleteByPurchaseOrderDetailId(detail.PurchaseOrder.Id, detail.Product.Id);
                            }
                        }
                        foreach (var kvp in currentQuantities)
                        {
                            long productId = kvp.Key;
                            int newQuantity = kvp.Value;
                            var product = Product.Dao.Get(productId);
                            var existingDetail = existingDetails.FirstOrDefault(d => d.Product.Id == productId);
                            if (existingDetail == null)
                            {
                                var detail = new PurchaseOrderDetail
                                {
                                    PurchaseOrder = new PurchaseOrder { Id = purchaseOrderId },
                                    Product = product,
                                    Quantity = newQuantity,
                                    Code = "Active"
                                };
                                detail.Save();
                            }
                            else
                            {
                                existingDetail.Quantity = newQuantity;
                                existingDetail.Save();
                                PurchaseOrderDetail.Dao.DeleteByPurchaseOrderDetailId(purchaseOrderId, productId);
                                var detail = new PurchaseOrderDetail
                                {
                                    PurchaseOrder = new PurchaseOrder { Id = purchaseOrderId },
                                    Product = product,
                                    Quantity = newQuantity,
                                    Code = "Active"
                                };
                                detail.Save();
                            }
                        }
                    }
                    purchaseOrder.TotalAmount = Convert.ToDecimal(collection["TotalAmount"]);
                    purchaseOrder.StatePurchaseOrder = new StatePurchaseOrder { Id = newStateId };
                    purchaseOrder.SalePoint = new SalePoint { Id = long.Parse(collection["SalePoint.Id"]) };
                    purchaseOrder.WarehouseManager = new WarehouseManager { Id = wm.Id };
                    purchaseOrder.Save();

                    TempData["Success"] = "Se ha editado correctamente";
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Alert = "Error al actualizar la orden de compra. Verifique los datos ingresados.";
                    FillDropdowns();
                    return View(new PurchaseOrder());
                }
            }
            catch (Exception ex)
            {
                ViewBag.Alert = $"Error al actualizar la orden de compra: {ex.Message}";
                FillDropdowns();
                return View(new PurchaseOrder());
            }
        }

        // GET: PurchaseOrder/Delete
        [AccessCode("PurchaseOrderDelete")]
        [Authenticated]
        public ActionResult Delete(int id)
        {
            try
            {
                List<PurchaseOrder> list = new List<PurchaseOrder>();
                PurchaseOrder purchaseOrder = PurchaseOrder.Dao.Get(id);
                purchaseOrder.PurchaseOrderDetails = PurchaseOrderDetail.Dao.GetDetailsByPurchaseOrderId(id);
                list.Add(purchaseOrder);
                list.LoadRelation(po => po.StatePurchaseOrder);
                bool isAdmin = IsAdmin();
                int stateId = (int)purchaseOrder.StatePurchaseOrder.Id;
                if (!isAdmin && (stateId == 2 || stateId == 3 || stateId == 4))
                {
                    TempData["Alert"] = "No se puede eliminar una órden de compra que ya fue entregada, cancelada o rechazada.";
                    return RedirectToAction("Index");
                }
                foreach (var detail in purchaseOrder.PurchaseOrderDetails)
                {
                    StockMovement.Dao.DeleteByPurchaseOrderDetailId(detail.Product.Id, purchaseOrder.Id);
                    // Si está en pendiente o entregada, aumentar stock
                    if ((stateId == 1 || stateId == 2) && isAdmin)
                    {
                        var product = Product.Dao.Get(detail.Product.Id);
                        if (product != null)
                        {
                            product.Stock.Quantity += detail.Quantity;
                            product.Stock.Save();
                            if (product.Stock.Quantity <= 5)
                                product.StateProduct = new StateProduct { Id = 4 };
                            else
                                product.StateProduct = new StateProduct { Id = 1 };
                            product.Save();
                        }
                    }
                    // Si está en cancelada o rechazada
                    detail.Code = "Inactive";
                    detail.Save();
                }

                purchaseOrder.Code = "Inactive";
                purchaseOrder.Save();

                TempData["Success"] = "Se ha eliminado correctamente";
                return RedirectToAction("Index");
            }
            catch
            {
                TempData["Alert"] = "Ocurrió un error al eliminar la orden de compra.";
                return RedirectToAction("Index");
            }
        }

    }
}

