using System;
using System.Collections.Generic;
using ZdravaPrehrana.Entitete;
using ZdravaPrehrana.Controllers;

namespace ZdravaPrehrana.Entitete
{

    public class Sestavina
    {
        public int Id { get; set; }
        public string Naziv { get; set; }
        public double Kolicina { get; set; }
        public string Enota { get; set; }
        public double Kalorije { get; set; }
        public double Beljakovine { get; set; }
         public double Mascobe { get; set; }
        public double OgljikoviHidrati { get; set; }
    }
    public class Obrok
        {
            public int Id { get; set; }
            public string Naziv { get; set; }
            public DateTime Cas { get; set; }
            public List<Recept> Recepti { get; set; } = new List<Recept>();
        }

        public class Ocena
        {
            public int Id { get; set; }
            public int Vrednost { get; set; }
            public string Komentar { get; set; }
            public DateTime DatumOcene { get; set; }
        }

        public class PrehranskiCilji
        {
            public int Id { get; set; }
            public int CiljneKalorije { get; set; }
            public double CiljneBeljakovine { get; set; }
            public double CiljneMascobe { get; set; }
            public double CiljniOgljikoviHidrati { get; set; }
        }

        public class Uporabnik
        {
            public int Id { get; set; }
            public string Ime { get; set; }
            public string Email { get; set; }
            public string Geslo { get; set; }
        }
}
