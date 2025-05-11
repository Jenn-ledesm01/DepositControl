using DepositControl.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using DNF.Entity;
using DepositControl.Dao;
using DepositControl.Bussines;
using com.sun.org.apache.xpath.@internal.objects;

namespace DepositControl.Bussines
{
	public partial class PurchaseOrderDetail : IEntityDao, IName, ICode, IOrder
    {
		public override long Id { get; set; }

        [Required(ErrorMessage = "El campo cantidad es obligatorio")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un producto")]
        public Product Product { get; set; }

        [Required(ErrorMessage = "La orden de compra es obligatorio")]
        public PurchaseOrder PurchaseOrder { get; set; }

        public override void Save()
        {
            Dao.Save(this);
        }

        public override void Delete()
        {
            Dao.Delete(Id);
        }

        private static PurchaseOrderDetailDao _dao; 
        public static PurchaseOrderDetailDao Dao
            => _dao ?? (_dao = new PurchaseOrderDetailDao());

        public string Name { get; set; }
        public string Code { get ; set ; }

        public int Order { get; set; }

    }
}

namespace DepositControl.Dao
{
    public partial class PurchaseOrderDetailDao : DaoDb<PurchaseOrderDetail>
    {
        // Obtener detalles de orden de compra por id
        public List<PurchaseOrderDetail> GetDetailsByPurchaseOrderId(long purchaseOrderId)
        {
            var details = GetFromSP("GetPurchaseOrderDetails", new { PurchaseOrderID = purchaseOrderId });

            foreach(var detail in details)
            {
                detail.Product = new Product
                {
                    Id = detail.Product.Id,
                    Name = detail.Product.Name,
                    Price = detail.Product.Price
                };
            }
            return details;
        }

        public List<PurchaseOrderDetail> GetDetailsByPurchaseOrderIds(List<long> purchaseOrderIds)
        {
            var details = new List<PurchaseOrderDetail>();
            foreach (var id in purchaseOrderIds)
            {
                details.AddRange(GetDetailsByPurchaseOrderId(id));
            }

            return details;
        }

        //Eliminar ordenes de compra por id
        public void DeleteByPurchaseOrderId(long purchaseOrderId)
        {
            GetScalarFromSP("DeleteByPurchaseOrderId", new { PurchaseOrder_Id = purchaseOrderId });
        }

        public void DeleteByPurchaseOrderIds(List<long> purchaseOrderIds)
        {
            foreach (var id in purchaseOrderIds)
            {
                DeleteByPurchaseOrderId(id);
            }
        }

        public void DeleteByPurchaseOrderDetailId(long purchaseOrderId, long productId)
        {
            GetScalarFromSP("DeleteDetail", new
            {
                PurchaseOrder_Id = purchaseOrderId,
                Product_Id = productId
            });
        }

        public PurchaseOrderDetail GetById(long productId)
        {
            var details = GetFromSP("GetById", new { ProductId = productId });

            return details.FirstOrDefault();
        }


    }
}