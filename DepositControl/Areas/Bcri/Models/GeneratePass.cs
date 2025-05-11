using System.ComponentModel.DataAnnotations;

namespace DepositControl.Areas.Bcri.Models
{
    public class GeneratePass
    {
        public long id { get; set; }
        public string FullName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string ActualPassword { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The passwords are not equal")]
        public string ConfirmPassword { get; set; }
    }
}
