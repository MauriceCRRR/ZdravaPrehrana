using System;
using System.Collections.Generic;
using ZdravaPrehrana.Entitete;

namespace ZdravaPrehrana.Controllers
{
    public class HardKodedPodatki
    {
        private static HardKodedPodatki _instanca;
        public static HardKodedPodatki Instanca
        {
            get
            {
                if (_instanca == null)
                {
                    _instanca = new HardKodedPodatki();
                }
                return _instanca;
            }
        }
        public List<Recept> Recepti { get; private set; }
        public List<Sestavina> Sestavine { get; private set; }
        public List<Uporabnik> Uporabniki { get; private set; }
        public List<Jedilnik> Jedilniki { get; private set; }

        private HardKodedPodatki()
        {
            NapolniPodatke();
        }

        private void NapolniPodatke()
        {
            Sestavine = new List<Sestavina>
            {
                new Sestavina
                {
                    Id = 1,
                    Naziv = "Piščančje prsi",
                    Kolicina = 100,
                    Enota = "g",
                    Kalorije = 165,
                    Beljakovine = 31,
                    Mascobe = 3.6,
                    OgljikoviHidrati = 0
                },
                new Sestavina
                {
                    Id = 2,
                    Naziv = "Riž",
                    Kolicina = 100,
                    Enota = "g",
                    Kalorije = 130,
                    Beljakovine = 2.7,
                    Mascobe = 0.3,
                    OgljikoviHidrati = 28
                },
                new Sestavina
                {
                    Id = 3,
                    Naziv = "Brokoli",
                    Kolicina = 100,
                    Enota = "g",
                    Kalorije = 55,
                    Beljakovine = 3.7,
                    Mascobe = 0.6,
                    OgljikoviHidrati = 11.2
                }
            };

            Recepti = new List<Recept>
{
             new Recept
             {
                 naziv = "Piščanec z rižem",
                 sestavine = new List<Sestavina> { Sestavine[0], Sestavine[1] },
                  postopek = "1. Skuhaj riž po navodilih.\n2. Speči piščančje prsi na žaru.\n3. Postrezi skupaj.",
                 kalorije = 295,
                cas_priprave = 30
                },
              new Recept
               {
                    naziv = "Zelenjavna rižota",
                    sestavine = new List<Sestavina> { Sestavine[1], Sestavine[2] },
                    postopek = "1. Prepraži riž.\n2. Dodaj narezane brokoli.\n3. Kuhaj dokler riž ni kuhan.",
                    kalorije = 185,
                    cas_priprave = 25
                   }
               };

            Uporabniki = new List<Uporabnik>
{
              new Uporabnik
                 {
                   uporabniskoIme = "janezN",
                   geslo = "geslo123"
                  },
               new Uporabnik
               {
                   uporabniskoIme = "majaK",
                   geslo = "geslo456"
                  }
               };

            Jedilniki = new List<Jedilnik>
{
    new Jedilnik
    {
        naziv = "Tedenski jedilnik",
        datumKreiranja = DateTime.Now,
        obroki = new List<Obrok>
        {
            new Obrok
            {
                Naziv = "Kosilo",
                Cas = DateTime.Now.AddHours(2),
                Recepti = new List<Recept> { Recepti[0] }
            },
               new Obrok
            {
                Naziv = "Večerja",
                Cas = DateTime.Now.AddHours(8),
                Recepti = new List<Recept> { Recepti[1] }
            }
            },
               vsebujeRecept = new Recept[] { Recepti[0], Recepti[1] },
               ustvariJedilnik = Uporabniki[0],
               deliZ = new Uporabnik[] { Uporabniki[1] }
            }
          };
        }
    }
}