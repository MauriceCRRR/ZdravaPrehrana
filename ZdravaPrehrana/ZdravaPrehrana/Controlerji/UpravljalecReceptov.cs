using System;
using System.Collections.Generic;
using System.Linq;
using ZdravaPrehrana.Entitete;

namespace ZdravaPrehrana.Controllers
{
    public class UpravljalecReceptov
    {
        private readonly HardKodedPodatki _podatki = HardKodedPodatki.Instanca;
        private Recept[] upravlja;

        public bool DodajRecept(Recept noviRecept)
        {
            try
            {
                _podatki.Recepti.Add(noviRecept);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool UrediRecept(string naziv, Recept posodobljeniRecept)
        {
            try
            {
                var receptZaUrejanje = _podatki.Recepti.FirstOrDefault(r => r.naziv == naziv);
                if (receptZaUrejanje != null)
                {
                    receptZaUrejanje.naziv = posodobljeniRecept.naziv;
                    receptZaUrejanje.sestavine = posodobljeniRecept.sestavine;
                    receptZaUrejanje.postopek = posodobljeniRecept.postopek;
                    receptZaUrejanje.kalorije = posodobljeniRecept.kalorije;
                    receptZaUrejanje.cas_priprave = posodobljeniRecept.cas_priprave;
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public bool IzbrisiRecept(string naziv)
        {
            try
            {
                var receptZaBrisanje = _podatki.Recepti.FirstOrDefault(r => r.naziv == naziv);
                if (receptZaBrisanje != null)
                {
                    _podatki.Recepti.Remove(receptZaBrisanje);
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public List<Recept> PoisciRecepte(string iskaniNiz)
        {
            try
            {
                return _podatki.Recepti
                    .Where(r => r.naziv.ToLower().Contains(iskaniNiz.ToLower()))
                    .ToList();
            }
            catch
            {
                return new List<Recept>();
            }
        }

        public Recept PridobiRecept(string naziv)
        {
            return _podatki.Recepti.FirstOrDefault(r => r.naziv == naziv);
        }
    }
}