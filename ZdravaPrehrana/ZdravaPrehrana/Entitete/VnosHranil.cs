using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZdravaPrehrana.Entitete
{
    public class VnosHranil
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime Datum { get; set; }

        [Required]
        [Range(0, 10000)]
        public int Kalorije { get; set; }

        [Required]
        [Range(0, 1000)]
        public double Beljakovine { get; set; }

        [Required]
        [Range(0, 1000)]
        public double Mascobe { get; set; }

        [Required]
        [Range(0, 1000)]
        public double OgljikoviHidrati { get; set; }

        // Navigacijske lastnosti
        public int UporabnikId { get; set; }
        public virtual Uporabnik Uporabnik { get; set; }

        // Metode
        public bool DodajVnos(VnosHranilPodatki podatki)
        {
            try
            {
                Datum = podatki.Datum;
                Kalorije = podatki.Kalorije;
                Beljakovine = podatki.Beljakovine;
                Mascobe = podatki.Mascobe;
                OgljikoviHidrati = podatki.OgljikoviHidrati;
                UporabnikId = podatki.UporabnikId;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public VnosHranilPodatki PridobiVrednosti()
        {
            return new VnosHranilPodatki
            {
                Datum = this.Datum,
                Kalorije = this.Kalorije,
                Beljakovine = this.Beljakovine,
                Mascobe = this.Mascobe,
                OgljikoviHidrati = this.OgljikoviHidrati
            };
        }
    }

    // Pomožni razred za prenos podatkov
    public class VnosHranilPodatki
    {
        public DateTime Datum { get; set; }
        public int Kalorije { get; set; }
        public double Beljakovine { get; set; }
        public double Mascobe { get; set; }
        public double OgljikoviHidrati { get; set; }

        public int UporabnikId { get; set; }
    }
}