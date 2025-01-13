using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ZdravaPrehrana.Entitete
{
    public class Obrok
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Naziv { get; set; }

        public DateTime Cas { get; set; }

        // Navigacijske lastnosti
        public int JedilnikId { get; set; }
        public virtual Jedilnik Jedilnik { get; set; }

        public virtual ICollection<Recept> Recepti { get; set; } = new List<Recept>();
    }

}