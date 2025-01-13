using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace ZdravaPrehrana.Entitete
{
    public class UporabnikProfil
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Uporabnik")]
        public int UporabnikId { get; set; }

        [Required]
        [Range(0, 300)]
        public double Visina { get; set; }

        [Required]
        [Range(0, 500)]
        public double Teza { get; set; }

        // Shranjevanje seznamov kot JSON string v bazi
        private string AlergijeJson
        {
            get => JsonSerializer.Serialize(Alergije);
            set => Alergije = JsonSerializer.Deserialize<List<string>>(value ?? "[]");
        }

        private string OmejitveJson
        {
            get => JsonSerializer.Serialize(Omejitve);
            set => Omejitve = JsonSerializer.Deserialize<List<string>>(value ?? "[]");
        }

        [NotMapped]
        public List<string> Alergije { get; set; } = new List<string>();

        [NotMapped]
        public List<string> Omejitve { get; set; } = new List<string>();

        // Navigacijska lastnost
        public virtual Uporabnik Uporabnik { get; set; }

        // Metode
        public bool UrediProfil(UporabnikProfilPodatki podatki)
        {
            try
            {
                Visina = podatki.Visina;
                Teza = podatki.Teza;

                if (podatki.Alergije != null)
                    Alergije = podatki.Alergije;

                if (podatki.Omejitve != null)
                    Omejitve = podatki.Omejitve;

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void DodajAlergijo(string alergija)
        {
            if (!string.IsNullOrWhiteSpace(alergija) && !Alergije.Contains(alergija))
            {
                Alergije.Add(alergija);
            }
        }

        public void DodajOmejitev(string omejitev)
        {
            if (!string.IsNullOrWhiteSpace(omejitev) && !Omejitve.Contains(omejitev))
            {
                Omejitve.Add(omejitev);
            }
        }

        public void OdstraniAlergijo(string alergija)
        {
            Alergije.Remove(alergija);
        }

        public void OdstraniOmejitev(string omejitev)
        {
            Omejitve.Remove(omejitev);
        }
    }

   
    // Pomožni razred za urejanje profila
    public class UporabnikProfilPodatki
    {
        public double Visina { get; set; }
        public double Teza { get; set; }
        public List<string> Alergije { get; set; }
        public List<string> Omejitve { get; set; }
    }
}