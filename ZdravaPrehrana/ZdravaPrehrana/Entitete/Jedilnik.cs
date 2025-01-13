// Jedilnik.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZdravaPrehrana.Entitete
{
    public class Jedilnik
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Naziv { get; set; }

        public DateTime DatumKreiranja { get; set; }

        // Lastnosti za deljenje
        public bool JeDeljiv { get; set; } = false;
        public DateTime? DatumDeljenja { get; set; }

        // Navigacijske lastnosti
        public virtual List<Obrok> Obroki { get; set; } = new List<Obrok>();
        public virtual List<Recept> Recepti { get; set; } = new List<Recept>();
        public int UporabnikId { get; set; }
        public virtual Uporabnik Uporabnik { get; set; }
        public virtual ICollection<Uporabnik> DeliZ { get; set; } = new List<Uporabnik>();
        public virtual ICollection<JedilnikOcena> Ocene { get; set; } = new List<JedilnikOcena>();

        // Obstojeèe metode
        public bool KreirajJedilnik()
        {
            try
            {
                DatumKreiranja = DateTime.Now;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void DodajObrok(Obrok obrok)
        {
            if (obrok != null && !Obroki.Contains(obrok))
            {
                Obroki.Add(obrok);
            }
        }

        public void OdstraniObrok(Obrok obrok)
        {
            if (obrok != null)
            {
                Obroki.Remove(obrok);
            }
        }

        public void DeliJedilnik(Uporabnik uporabnik)
        {
            if (uporabnik != null && !DeliZ.Contains(uporabnik))
            {
                DeliZ.Add(uporabnik);
                JeDeljiv = true;
                DatumDeljenja = DateTime.Now;
            }
        }
    }

    // Nov razred za ocene jedilnikov
    public class JedilnikOcena
    {
        [Key]
        public int Id { get; set; }

        [Range(1, 5)]
        public int Vrednost { get; set; }

        [MaxLength(500)]
        public string Komentar { get; set; }

        public DateTime DatumOcene { get; set; }

        // Navigacijske lastnosti
        public int JedilnikId { get; set; }
        public virtual Jedilnik Jedilnik { get; set; }

        public int UporabnikId { get; set; }
        public virtual Uporabnik Uporabnik { get; set; }
    }
}