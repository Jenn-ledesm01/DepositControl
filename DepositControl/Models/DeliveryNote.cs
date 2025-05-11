using DepositControl.Bussines;
using DepositControl.Models;
using DNF.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using DepositControl.Dao;

namespace DepositControl.Bussines
{
	public partial class DeliveryNote : IEntityDao, IName, ICode, IOrder
    {
        public override long Id { get; set; }

        [Required(ErrorMessage = "El campo fecha es obligatorio")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "El campo monto total es obligatorio")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalAmount { get; set; }

        [Required(ErrorMessage = "El archivo del remito es obligatorio")]
        public string FileDeliveryNote { get; set; }

        [Required(ErrorMessage = "El número del remito es obligatorio")]
        [StringLength(8, ErrorMessage = "El número del remito no puede tener más de 8 caracteres")]
        public string Number { get; set; }

        [Required(ErrorMessage = "El estado del remito es obligatorio")]
        public StateDeliveryNote StateDeliveryNote { get; set; }
        [Required]
        public WarehouseManager WarehouseManager { get; set; }

        private List<DeliveryNoteDetail> _deliveryNoteDetails;
        public List<DeliveryNoteDetail> DeliveryNoteDetails
        {
            get => _deliveryNoteDetails ?? (_deliveryNoteDetails = DeliveryNoteDetail.Dao.GetBy(this));
            set => _deliveryNoteDetails = value;
        }
        public string Name { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Code { get; set ; }
        public int Order { get ; set ; }

        public override void Save()
        {
            Dao.Save(this);
        }

        public override void Delete()
        {
            Dao.Delete(Id);
        }

        private static DeliveryNoteDao _dao;
        public static DeliveryNoteDao Dao
            => _dao ?? (_dao = new DeliveryNoteDao());
    }
}

namespace DepositControl.Dao
{
    public partial class DeliveryNoteDao : DaoDb<DeliveryNote>
    {
        public int GetLastOrder()
               => int.Parse(GetScalarFromSP("GetLastOrder"));

    }
}