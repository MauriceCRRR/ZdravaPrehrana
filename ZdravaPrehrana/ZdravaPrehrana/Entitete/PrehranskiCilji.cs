using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZdravaPrehrana.Entitete
{
    public class PrehranskiCilji
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Ciljna teža je obvezna")]
        [Range(30, 300, ErrorMessage = "Ciljna teža mora biti med 30 in 300 kg")]
        public double CiljnaTeza { get; set; }

        [Required(ErrorMessage = "Tedenska izguba je obvezna")]
        [Range(0.1, 1.0, ErrorMessage = "Tedenska izguba mora biti med 0.1 in 1.0 kg")]
        public double TedenIzgubaKg { get; set; }

        [Required(ErrorMessage = "Èasovni okvir je obvezen")]
        [Range(1, 365, ErrorMessage = "Èasovni okvir mora biti med 1 in 365 dni")]
        public int CasovniOkvir { get; set; }

        [Required(ErrorMessage = "Dnevne kalorije so obvezne")]
        [Range(1200, 10000, ErrorMessage = "Dnevne kalorije morajo biti med 1200 in 10000")]
        public int DnevneKalorije { get; set; }

        [Required]
        public double BMR { get; set; }

        [Required]
        public double TDEE { get; set; }

        public int UporabnikId { get; set; }

        [ForeignKey("UporabnikId")]
        public virtual Uporabnik Uporabnik { get; set; }

        public void IzracunajKalorije(double visina, double teza, int starost = 30, string spol = "M", double aktivnost = 1.2)
        {
            if (visina <= 0 || teza <= 0)
                throw new ArgumentException("Višina in teža morata biti veèji od 0");

            // Harris-Benedict formula za BMR
            BMR = spol.ToUpper() == "M"
                ? 88.362 + (13.397 * teza) + (4.799 * visina) - (5.677 * starost)
                : 447.593 + (9.247 * teza) + (3.098 * visina) - (4.330 * starost);

            // Izraèun TDEE glede na nivo aktivnosti
            TDEE = BMR * aktivnost;

            // Izraèun dnevnega deficita
            double tedenskiDeficit = TedenIzgubaKg * 7700; // 1kg mašèobe = 7700 kalorij
            double dnevniDeficit = tedenskiDeficit / 7;

            // Izraèun dnevnih kalorij s deficitom
            DnevneKalorije = (int)(TDEE - dnevniDeficit);

            // Preveri minimalne kalorije
            int minKalorije = spol.ToUpper() == "M" ? 1500 : 1200;
            DnevneKalorije = Math.Max(DnevneKalorije, minKalorije);
        }

        public double PreveriNapredek()
        {
            if (Uporabnik?.Profil == null)
                return 0;

            var trenutnaTeza = Uporabnik.Profil.Teza;
            var razlika = Math.Abs(trenutnaTeza - CiljnaTeza);
            var celotnaRazlika = Math.Abs(Uporabnik.Profil.Teza - CiljnaTeza);

            if (celotnaRazlika == 0)
                return 100;

            return Math.Min(100, (1 - (razlika / celotnaRazlika)) * 100);
        }

        public void PosodobiCilj(PrehranskiCiljiPodatki podatki)
        {
            if (podatki == null)
                throw new ArgumentNullException(nameof(podatki));

            CiljnaTeza = podatki.CiljnaTeza;
            TedenIzgubaKg = podatki.TedenIzgubaKg;
            CasovniOkvir = podatki.CasovniOkvir;
            DnevneKalorije = podatki.DnevneKalorije;
            BMR = podatki.BMR;
            TDEE = podatki.TDEE;
        }
    }

    public class PrehranskiCiljiPodatki
    {
        [Required(ErrorMessage = "Ciljna teža je obvezna")]
        [Range(30, 300, ErrorMessage = "Ciljna teža mora biti med 30 in 300 kg")]
        public double CiljnaTeza { get; set; }

        [Required(ErrorMessage = "Tedenska izguba je obvezna")]
        [Range(0.1, 1.0, ErrorMessage = "Tedenska izguba mora biti med 0.1 in 1.0 kg")]
        public double TedenIzgubaKg { get; set; }

        [Required(ErrorMessage = "Èasovni okvir je obvezen")]
        [Range(1, 365, ErrorMessage = "Èasovni okvir mora biti med 1 in 365 dni")]
        public int CasovniOkvir { get; set; }

        [Required(ErrorMessage = "Dnevne kalorije so obvezne")]
        [Range(1200, 10000, ErrorMessage = "Dnevne kalorije morajo biti med 1200 in 10000")]
        public int DnevneKalorije { get; set; }

        [Required]
        public double BMR { get; set; }

        [Required]
        public double TDEE { get; set; }
    }
}