using DepositControl.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using DNF.Entity;
using DepositControl.Bussines;
using DepositControl.Dao;

namespace DepositControl.Bussines
{
	public partial class Product : IEntityDao, IName, ICode, IOrder
    {
        public override long Id { get; set; }

        [Required(ErrorMessage = "El campo nombre es obligatorio")]
        [StringLength(255, ErrorMessage = "Debe tener como máximo 255 caracteres")]
        public string Name { get; set; }

        [StringLength(255, ErrorMessage = "Debe tener como máximo 255 caracteres")]
        public string Description { get; set; }

        [Required(ErrorMessage = "El campo precio es obligatorio")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }

        [StringLength(50, ErrorMessage = "Debe tener como máximo 50 caracteres")]
        public string Code { get; set; }
        public int Order { get; set; }
        [Required]
        public  Stock Stock { get; set; }
        [Required(ErrorMessage = "El estado del producto es obligatorio")]
        public  StateProduct StateProduct { get; set; }
        public override void Save()
        {
            Dao.Save(this);
        }

        public override void Delete()
        {
            Dao.Delete(Id);
        }

        private static ProductDao _dao;
        public static ProductDao Dao
            => _dao ?? (_dao = new ProductDao());

    }
}

namespace DepositControl.Dao
{
    public partial class ProductDao : DaoDb<Product>
    {
        public int GetLastOrder()
            => int.Parse(GetScalarFromSP("GetLastOrder"));

        public int GetByDuplicate(string name, long excludeId = 0)
            => int.Parse(GetScalarFromSP("GetByDuplicate", new { Name = name, ExcludeId = excludeId }));


    }
}