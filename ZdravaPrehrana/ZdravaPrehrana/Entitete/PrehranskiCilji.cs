using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZdravaPrehrana.Entitete
{
    public class PrehranskiCilji
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Range(0, 500)]
        public double CiljnaTeza { get; set; }

        [Required]
        public int CasovniOkvir { get; set; } // v dnevih

        [Required]
        [Range(0, 10000)]
        public int DnevneKalorije { get; set; }

        // Navigacijske lastnosti
        public int UporabnikId { get; set; }
        public virtual Uporabnik Uporabnik { get; set; }

        // Metode
        public bool NastaviCilj(PrehranskiCiljiPodatki podatki)
        {
            try
            {
                CiljnaTeza = podatki.CiljnaTeza;
                CasovniOkvir = podatki.CasovniOkvir;
                DnevneKalorije = podatki.DnevneKalorije;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public double PreveriNapredek()
        {
            // Implementacija preverjanja napredka
            // Lahko dodate logiko za izraèun napredka glede na zaèetno težo in ciljno težo
            return 0.0;
        }

        public void PosodobiCilj(PrehranskiCiljiPodatki podatki)
        {
            if (podatki != null)
            {
                CiljnaTeza = podatki.CiljnaTeza;
                CasovniOkvir = podatki.CasovniOkvir;
                DnevneKalorije = podatki.DnevneKalorije;
            }
        }
    }

    // Pomožni razred za prenos podatkov
    public class PrehranskiCiljiPodatki
    {
        public double CiljnaTeza { get; set; }
        public int CasovniOkvir { get; set; }
        public int DnevneKalorije { get; set; }
    }
}