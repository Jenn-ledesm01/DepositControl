using DepositControl.Bussines;
using DNF.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using DepositControl.Dao;
using DepositControl.Models;
using java.beans;
using System.Data.SqlClient;

namespace DepositControl.Bussines
{
	public partial class PurchaseOrder : IEntityDao, IName, ICode, IOrder
    {
        public override long Id { get; set ; }

        [Required(ErrorMessage = "El campo fecha es obligatorio")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "El campo monto total es obligatorio")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalAmount { get; set; }
        [Required(ErrorMessage = "El número del remito es obligatorio")]
        [StringLength(8, ErrorMessage = "El número del remito no puede tener más de 8 caracteres")]
        public string Number { get; set; }
        [Required(ErrorMessage = "El estado de la orden de compra es obligatorio")]
        public StatePurchaseOrder StatePurchaseOrder { get; set; }
        [Required(ErrorMessage = "El punto de venta es obligatorio")]
        public SalePoint SalePoint { get; set; }
        [Required]
        public WarehouseManager WarehouseManager { get; set; }

        private List<PurchaseOrderDetail> _purchaseOrderDetails;
        public List<PurchaseOrderDetail> PurchaseOrderDetails
        {
            get => _purchaseOrderDetails ?? (_purchaseOrderDetails = PurchaseOrderDetail.Dao.GetBy(this));
            set => _purchaseOrderDetails = value;
        }

        public string Name { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Code { get ; set ; }
        public int Order { get ; set; }

        public override void Save()
        {
            Dao.Save(this);
        }

        public override void Delete()
        {
            Dao.Delete(Id);
        }

        private static PurchaseOrderDao _dao;
        public static PurchaseOrderDao Dao
            => _dao ?? (_dao = new PurchaseOrderDao());
    }
}

namespace DepositControl.Dao
{
    public partial class PurchaseOrderDao : DaoDb<PurchaseOrder>
    {
        public int GetLastOrder()
               => int.Parse(GetScalarFromSP("GetLastOrder"));
    

    }


}