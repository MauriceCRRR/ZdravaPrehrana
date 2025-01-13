using System.ComponentModel.DataAnnotations;

namespace ZdravaPrehrana.Entitete
{
    public class ReceptSestavina
    {
        public int ReceptId { get; set; }
        public virtual Recept? Recept { get; set; }

        public int SestavinaId { get; set; }
        public virtual Sestavina? Sestavina { get; set; }

        [Required(ErrorMessage = "Količina je obvezna")]
        public double Kolicina { get; set; }

        [Required(ErrorMessage = "Enota je obvezna")]
        public string Enota { get; set; }
    }
}
