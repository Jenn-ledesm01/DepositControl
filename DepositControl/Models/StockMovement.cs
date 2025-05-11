using DepositControl.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using DepositControl.Dao;
using DepositControl.Bussines;
using DNF.Entity;
using DNF.Security.Bussines;

namespace DepositControl.Bussines
{
	public partial class StockMovement : IEntityDao, IName, ICode, IOrder
    {
        public override long Id { get; set; }
        public DateTime DateStockMovement { get; set; }
        public long? DeliveryNoteDetail_Product_Id { get; set; }
        public long? DeliveryNoteDetail_DeliveryNote_Id { get; set; }
        public DeliveryNoteDetail DeliveryNoteDetail { get; set; }
        public long? PurchaseOrderDetail_Product_Id { get; set; }
        public long? PurchaseOrderDetail_PurchaseOrder_Id { get; set; }
        public PurchaseOrderDetail PurchaseOrderDetail { get; set; }

        [Required]
        public  Stock Stock { get; set; }

        [Required]
        public User User { get; set; }

        public override void Save()
        {
            Dao.Save(this);
        }

        public override void Delete()
        {
            Dao.Delete(Id);
        }

        private static StockMovementDao _dao;
        public static StockMovementDao Dao
            => _dao ?? (_dao = new StockMovementDao());

        public string Name { get ; set ; }
        public string Code { get ; set ; }
        public int Order { get ; set; }
    }
}

namespace DepositControl.Dao
{
    public partial class StockMovementDao : DaoDb<StockMovement>
    {
        public void DeleteByPurchaseOrderDetailId(long productId, long purchaseOrderId)
        {
            GetScalarFromSP("DeleteByPurchaseOrderDetailId", new
            {
                PurchaseOrderDetail_Product_Id = productId,
                PurchaseOrderDetail_PurchaseOrder_Id = purchaseOrderId
            });
        }

        public void DeleteByDeliveryNoteDetailId(long productId, long deliveryNoteId)
        {
            GetScalarFromSP("DeleteByDeliveryNoteDetailId", new
            {
                DeliveryNoteDetail_Product_Id = productId,
                DeliveryNoteDetail_DeliveryNote_Id = deliveryNoteId
            });
        }

        public long SaveStockMovement(StockMovement sm)
        {
            int? dnProdId = null;
            int? dnNoteId = null;
            int? poProdId = null;
            int? poOrderId = null;
            if (sm.DeliveryNoteDetail_Product_Id.HasValue && sm.DeliveryNoteDetail_DeliveryNote_Id.HasValue)
            {
                dnProdId = (int?)sm.DeliveryNoteDetail_Product_Id;
                dnNoteId = (int?)sm.DeliveryNoteDetail_DeliveryNote_Id;
            }
            else if (sm.PurchaseOrderDetail_Product_Id.HasValue && sm.PurchaseOrderDetail_PurchaseOrder_Id.HasValue)
            {
                poProdId = (int?)sm.PurchaseOrderDetail_Product_Id;
                poOrderId = (int?)sm.PurchaseOrderDetail_PurchaseOrder_Id;
            }
            else
            {
                throw new InvalidOperationException(
                    "El StockMovement debe tener o PurchaseOrderDetail (ambos IDs) o DeliveryNoteDetail (ambos IDs).");
            }

            string scalar = GetScalarFromSP(
            "Save",
            new
            {
                Id = (int)sm.Id,
                DateStockMovement = sm.DateStockMovement.Date,
                DeliveryNoteDetail_Product_Id = dnProdId,
                DeliveryNoteDetail_DeliveryNote_Id = dnNoteId,
                PurchaseOrderDetail_Product_Id = poProdId,
                PurchaseOrderDetail_PurchaseOrder_Id = poOrderId,
                Stock_Id = (int)sm.Stock.Id,
                User_Id = (int)sm.User.Id
            }
        );
            sm.Id = long.Parse(scalar);
            return sm.Id;
        }

        public List<StockMovement> GetStockMovement()
        {
            return GetFromSP("GetStockMovement");
        }


    }
}