using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZdravaPrehrana.Entitete
{
    public class Nasvet
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Vprašanje je obvezno")]
        public string Vprasanje { get; set; }

        public string Odgovor { get; set; }

        [Required]
        public DateTime DatumVprasanja { get; set; }

        public DateTime? DatumOdgovora { get; set; }

        [Required]
        public StatusNasveta Status { get; set; }

        // Navigacijske lastnosti za uporabnika, ki je postavil vprašanje
        [Required]
        public int UporabnikId { get; set; }

        [ForeignKey("UporabnikId")]
        public virtual Uporabnik Uporabnik { get; set; }

        // Navigacijske lastnosti za strokovnjaka, ki je odgovoril
        public int? StrokovnjakId { get; set; }

        [ForeignKey("StrokovnjakId")]
        public virtual Uporabnik Strokovnjak { get; set; }
    }

    public enum StatusNasveta
    {
        CakaNaOdgovor,
        Odgovorjen
    }
}