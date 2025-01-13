using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZdravaPrehrana.Entitete
{
    public class Recept
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Naziv recepta je obvezen")]
        public string Naziv { get; set; }

        public virtual List<ReceptSestavina> ReceptSestavine { get; set; } = new List<ReceptSestavina>();

        [Required(ErrorMessage = "Postopek priprave je obvezen")]
        public string Postopek { get; set; }

        [Required]
        public int Kalorije { get; set; }

        [Required]
        [Display(Name = "Èas priprave (minute)")]
        public int CasPriprave { get; set; }

        public virtual List<Ocena> Ocene { get; set; } = new List<Ocena>();

        // Te relacije ne smejo biti Required
        public virtual Jedilnik? Jedilnik { get; set; }
        public int? JedilnikId { get; set; }

        public virtual ICollection<Obrok> Obroki { get; set; } = new List<Obrok>();

        [Required]
        public int AvtorId { get; set; }
        public virtual Uporabnik? Avtor { get; set; }

        [Required]
        public bool JeJaven { get; set; } = false;

        public DateTime DatumUstvarjanja { get; set; } = DateTime.Now;
    }
}