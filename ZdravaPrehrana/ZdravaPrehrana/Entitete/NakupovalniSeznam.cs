using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZdravaPrehrana.Entitete
{
    public class NakupovalniSeznam
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Naziv { get; set; }

        public DateTime DatumKreiranja { get; set; }

        // Navigacijske lastnosti
        public virtual ICollection<SeznamPostavka> Postavke { get; set; } = new List<SeznamPostavka>();

        public int UporabnikId { get; set; }
        public virtual Uporabnik Uporabnik { get; set; }

        // Metode
        public void DodajSestavino(string sestavina, double kolicina = 1, string enota = "kos")
        {
            if (!string.IsNullOrWhiteSpace(sestavina))
            {
                Postavke.Add(new SeznamPostavka
                {
                    Naziv = sestavina,
                    Kolicina = kolicina,
                    Enota = enota,
                    JeObkljukana = false,
                    NakupovalniSeznamId = this.Id
                });
            }
        }

        public void OdstraniSestavino(string sestavina)
        {
            var postavkaZaBrisanje = Postavke.FirstOrDefault(p =>
                p.Naziv.Equals(sestavina, StringComparison.OrdinalIgnoreCase));
            if (postavkaZaBrisanje != null)
            {
                Postavke.Remove(postavkaZaBrisanje);
            }
        }

        public void ObkljukajPostavko(string sestavina)
        {
            var postavka = Postavke.FirstOrDefault(p =>
                p.Naziv.Equals(sestavina, StringComparison.OrdinalIgnoreCase));
            if (postavka != null)
            {
                postavka.JeObkljukana = true;
            }
        }

        public void OdkljukajPostavko(string sestavina)
        {
            var postavka = Postavke.FirstOrDefault(p =>
                p.Naziv.Equals(sestavina, StringComparison.OrdinalIgnoreCase));
            if (postavka != null)
            {
                postavka.JeObkljukana = false;
            }
        }
    }
}