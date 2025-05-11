using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DNF.Structure.Bussines;
using Repository = Bcri.Core.Bussines.Repository;

namespace DepositControl.Areas.Bcri.Models
{
    public class GridModel
    {
        
        public long RepositoryId { get; set; }
        public long RepositoryConfigId { get; set; }
        public string EntityName { get; set; }

        public string GridId { get; set; }


        public bool ServerSide { get; set; }
        public bool Editable { get; set; } = true;
        public bool Exportable { get; set; }

    }
}
