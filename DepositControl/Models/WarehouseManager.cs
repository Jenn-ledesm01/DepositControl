using DepositControl.Bussines;
using DepositControl.Dao;
using DNF.Entity;
using DNF.Security.Bussines;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DepositControl.Bussines
{
	public partial class WarehouseManager : IEntityDao, IName, ICode, IOrder
    {
        public override long Id { get; set; }
        [Required(ErrorMessage = "El campo fecha de inicio es obligatorio")]
        public DateTime StartDate { get; set; }
        [Required(ErrorMessage = "El campo DNI es obligatorio")]
        public int DNI { get; set; }
        [Required(ErrorMessage = "Debe seleccionar un jefe de depósito")]
        public StateWarehouseManager StateWarehouseManager { get; set; }
        [Required(ErrorMessage = "Debe seleccionar un usuario")]
        public User User { get; set; }
        public string Name { get ; set; }
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

        private static WarehouseManagerDao _dao;
        public static WarehouseManagerDao Dao
            => _dao ?? (_dao = new WarehouseManagerDao());

    }
}

namespace DepositControl.Dao
{
    public partial class WarehouseManagerDao : DaoDb<WarehouseManager>
    {
        public int GetLastOrder()
            => int.Parse(GetScalarFromSP("GetLastOrder"));

        public int GetByDuplicate(int dni)
            => int.Parse(GetScalarFromSP("GetByDuplicate", new { DNI = dni }));

    }
}