using DepositControl.Bussines;
using DepositControl.Dao;
using DNF.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DepositControl.Bussines
{
	public partial class StateWarehouseManager : IEntityDao, IName, ICode, IOrder
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

        private static StateWarehouseManagerDao _dao;
        public static StateWarehouseManagerDao Dao
            => _dao ?? (_dao = new StateWarehouseManagerDao());
    }
}

namespace DepositControl.Dao
{
    public partial class StateWarehouseManagerDao : DaoDb<StateWarehouseManager>
    {
        public int GetLastOrder()
               => int.Parse(GetScalarFromSP("GetLastOrder"));
    }
}