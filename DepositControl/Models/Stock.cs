using DepositControl.Bussines;
using DepositControl.Dao;
using DepositControl.Models;
using DNF.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DepositControl.Models
{
    public partial class Stock : IEntityDao, IName, ICode, IOrder
    {
        public override long Id { get; set; }

        [Required(ErrorMessage = "El campo cantidad es obligatorio")]
        public int Quantity { get; set; }

        private List<StockMovement> _stockMovements;
        public List<StockMovement> StockMovements
        {
            get => _stockMovements ?? (_stockMovements = StockMovement.Dao.GetBy(this));
            set => _stockMovements = value;
        }

        public override void Save()
        {
            Dao.Save(this);
        }

        public override void Delete()
        {
            Dao.Delete(Id);
        }

        private static StockDao _dao;
        public static StockDao Dao
            => _dao ?? (_dao = new StockDao());
        public string Name { get ; set ; }
        public string Code { get ; set ; }
        public int Order { get ; set; }
    }
}

namespace DepositControl.Dao
{
    public partial class StockDao : DaoDb<Stock>
    {

    }
}