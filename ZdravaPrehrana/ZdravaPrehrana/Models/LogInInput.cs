using System.ComponentModel.DataAnnotations;

namespace ZdravaPrehrana.Models
{
    public class LoginInput
    {
        [Required(ErrorMessage = "Uporabniško ime je obvezno")]
        public string UporabniskoIme { get; set; }

        [Required(ErrorMessage = "Geslo je obvezno")]
        public string Geslo { get; set; }
    }
}