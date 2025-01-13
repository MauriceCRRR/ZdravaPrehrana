using System.ComponentModel.DataAnnotations;

namespace ZdravaPrehrana.Models
{
    public class RegisterInput
    {
        [Required(ErrorMessage = "Uporabniško ime je obvezno")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Uporabniško ime mora biti dolgo med 3 in 50 znakov")]
        public string UporabniskoIme { get; set; }

        [Required(ErrorMessage = "Email naslov je obvezen")]
        [EmailAddress(ErrorMessage = "Vnesite veljaven email naslov")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Geslo je obvezno")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Geslo mora biti dolgo vsaj 6 znakov")]
        [DataType(DataType.Password)]
        public string Geslo { get; set; }

        [Required(ErrorMessage = "Potrditev gesla je obvezna")]
        [Compare("Geslo", ErrorMessage = "Gesli se ne ujemata")]
        [DataType(DataType.Password)]
        public string PotrdiGeslo { get; set; }
        public string? StrokovnjakKoda { get; set; }
    }
}