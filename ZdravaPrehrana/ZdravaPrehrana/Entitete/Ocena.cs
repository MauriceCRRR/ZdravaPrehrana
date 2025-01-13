using System;
using System.ComponentModel.DataAnnotations;

namespace ZdravaPrehrana.Entitete
{
    public class Ocena
    {
        [Key]
        public int Id { get; set; }

        [Range(1, 5)]
        public int Vrednost { get; set; }

        public string Komentar { get; set; }

        public DateTime DatumOcene { get; set; }

        // Navigacijske lastnosti
        public int ReceptId { get; set; }
        public virtual Recept Recept { get; set; }

        public int UporabnikId { get; set; }
        public virtual Uporabnik Uporabnik { get; set; }
    }
}