using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ZdravaPrehrana.Entitete;

namespace ZdravaPrehrana.Entitete
{
    public class Uporabnik
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UporabniskoIme { get; set; }

        [Required]
        public string Geslo { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public UporabniskaVloga Vloga { get; set; }

        // Relacije
        public virtual UporabnikProfil Profil { get; set; }
        public virtual ICollection<Jedilnik> Jedilniki { get; set; } = new List<Jedilnik>();
        public virtual ICollection<PrehranskiCilji> PrehranskiCilji { get; set; } = new List<PrehranskiCilji>();
        public virtual ICollection<VnosHranil> VnosiHranil { get; set; } = new List<VnosHranil>();
        public virtual ICollection<Nasvet> PrejetiNasveti { get; set; } = new List<Nasvet>();
        public virtual ICollection<Ocena> Ocene { get; set; } = new List<Ocena>(); // Sprememba iz PodaniFeedbacki v Ocene
        public virtual ICollection<NakupovalniSeznam> NakupovalniSeznami { get; set; } = new List<NakupovalniSeznam>();
        public virtual ICollection<Recept> UstvarjeniRecepti { get; set; } = new List<Recept>();

        // Metode
        public async Task<bool> PreveriPrijavo(string uporabniskoIme, string geslo)
        {
            return UporabniskoIme == uporabniskoIme && Geslo == geslo;
        }

        public async Task<UporabnikProfil> PridobiProfil()
        {
            return Profil;
        }

        // Dodatne metode za upravljanje z relacijami
        public async Task<Jedilnik> UstvariJedilnik(Jedilnik jedilnik)
        {
            Jedilniki.Add(jedilnik);
            return jedilnik;
        }

        public async Task<PrehranskiCilji> UstvariCilj(PrehranskiCilji cilj)
        {
            PrehranskiCilji.Add(cilj);
            return cilj;
        }

        public async Task<VnosHranil> DodajVnosHranil(VnosHranil vnos)
        {
            VnosiHranil.Add(vnos);
            return vnos;
        }

        public async Task<Ocena> DodajOceno(Ocena ocena) // Spremenjeno iz DodajFeedback
        {
            Ocene.Add(ocena);
            return ocena;
        }
        public async Task<NakupovalniSeznam> UstvariNakupovalniSeznam(NakupovalniSeznam seznam)
        {
            NakupovalniSeznami.Add(seznam);
            return seznam;
        }

        public async Task<bool> DeliJedilnik(Jedilnik jedilnik, Uporabnik prejemnik)
        {
            if (jedilnik != null && Jedilniki.Contains(jedilnik))
            {
                // Implementacija deljenja jedilnika
                return true;
            }
            return false;
        }
    }
}