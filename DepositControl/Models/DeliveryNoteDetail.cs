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

namespace DepositControl.Bussines
{
    public partial class DeliveryNoteDetail : IEntityDao, IName, ICode, IOrder
    {
        public override long Id { get; set; }

        [Required(ErrorMessage = "El campo cantidad es obligatorio")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un producto")]
        public  Product Product { get; set; }

        [Required(ErrorMessage = "El remito es obligatorio")]
        public  DeliveryNote DeliveryNote { get; set; }

        public override void Save()
        {
            Dao.Save(this);
        }

        public override void Delete()
        {
            Dao.Delete(Id);
        }

        private static DeliveryNoteDetailDao _dao;
        public static DeliveryNoteDetailDao Dao
            => _dao ?? (_dao = new DeliveryNoteDetailDao());
        public string Name { get { return "0"; } set { } }
        public string Code { get ; set ; }
        public int Order {
            get { return 0; }
            set { /* No hace nada */ }
        }
    }
}

namespace DepositControl.Dao
{
    public partial class DeliveryNoteDetailDao : DaoDb<DeliveryNoteDetail>
    {
        public List<DeliveryNoteDetail> GetDetailsByDeliveryNoteId(long deliveryNoteId)
        {
            var details = GetFromSP("GetDeliveryNoteDetails", new { DeliveryNoteID = deliveryNoteId });

            foreach (var detail in details)
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

        public List<DeliveryNoteDetail> GetDetailsByDeliveryNoteIds(List<long> deliveryNotesIds)
        {
            var details = new List<DeliveryNoteDetail>();
            foreach (var id in deliveryNotesIds)
            {
                details.AddRange(GetDetailsByDeliveryNoteId(id));
            }

            return details;
        }

        public void DeleteByDeliveryNoteId(long deliveryNoteId)
        {
            GetScalarFromSP("DeleteByDeliveryNoteId", new { DeliveryNote_Id = deliveryNoteId });
        }

        public void DeleteByDeliveryNoteIds(List<long> deliveryNoteIds)
        {
            foreach (var id in deliveryNoteIds)
            {
                DeleteByDeliveryNoteId(id);
            }
        }

        public void DeleteByDeliveryNoteDetailId(long deliveryNoteId, long productId)
        {
            GetScalarFromSP("DeleteDetail", new
            {
                Product_Id = productId,
                DeliveryNote_Id = deliveryNoteId
            });
        }

        public DeliveryNoteDetail GetById(long productId)
        {
            var details = GetFromSP("GetById", new { ProductId = productId });

            return details.FirstOrDefault();
        }
    }
}