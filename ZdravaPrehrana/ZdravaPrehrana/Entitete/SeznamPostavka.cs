using System.ComponentModel.DataAnnotations;

namespace ZdravaPrehrana.Entitete
{
    public class SeznamPostavka
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Naziv { get; set; }

        public double Kolicina { get; set; }

        public string Enota { get; set; }

        public bool JeObkljukana { get; set; }

        // Navigacijske lastnosti
        public int NakupovalniSeznamId { get; set; }
        public virtual NakupovalniSeznam NakupovalniSeznam { get; set; }
    }
}