using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DepositControl.Bussines;
using DNF.Entity;
using DepositControl.Dao;
using System.ComponentModel.DataAnnotations;
using DepositControl.Models;
namespace DepositControl.Bussines
{
	public partial class StatePurchaseOrder : IEntityDao, IName, ICode, IOrder
    {
        public override long Id { get; set; }
        [Required(ErrorMessage = "El campo nombre es obligatorio")]
        [StringLength(255, ErrorMessage = "Debe tener como máximo 255 caracteres")]
        public string Name { get; set; }

        [StringLength(50, ErrorMessage = "Debe tener como máximo 50 caracteres")]
        public string Code { get; set; }
        public int Order { get; set; }

        public override void Save()
        {
            Dao.Save(this);
        }

        public override void Delete()
        {
            Dao.Delete(Id);
        }

        private static StatePurchaseOrderDao _dao;
        public static StatePurchaseOrderDao Dao
            => _dao ?? (_dao = new StatePurchaseOrderDao());
    }
}

namespace DepositControl.Dao
{
    public partial class StatePurchaseOrderDao : DaoDb<StatePurchaseOrder>
    {
        public int GetLastOrder()
               => int.Parse(GetScalarFromSP("GetLastOrder"));
    }
}