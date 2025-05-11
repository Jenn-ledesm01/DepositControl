using DNF.Structure.Bussines;
using System.ComponentModel;

namespace DepositControl.Areas.Bcri.Models
{
    public class VDetailAccion
    {
        [DisplayName(@"Id Registro BD")]
        public long Id { get; set; }

        public Struct Struct { get; set; }

        [DisplayName(@"Campo")]
        public string StructDescription { get; set; }

        [DisplayName(@"Valor Anterior")]
        public string OldValue { get; set; }

        [DisplayName(@"Valor Actual")]
        public string Value { get; set; }

        public bool RowShow { get; set; }
    }
}
