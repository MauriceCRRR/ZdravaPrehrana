using System;
using System.Collections.Generic;
using ZdravaPrehrana.Entitete;
using ZdravaPrehrana.Controllers;

namespace ZdravaPrehrana.Entitete
{
    public class Recept
    {
        public string naziv;
        public List<Sestavina> sestavine;
        public string postopek;
        public int kalorije;
        public int cas_priprave;

        public void DodajSestavino(ref Sestavina sestavina)
        {
            throw new System.NotImplementedException("Not implemented");
        }

        public bool UrediRecept()
        {
            throw new System.NotImplementedException("Not implemented");
        }

        public double IzracunHranilneVrednosti()
        {
            throw new System.NotImplementedException("Not implemented");
        }

        public Feedback jeZaRecept;
        public UpravljalecReceptov upravlja;
        public Jedilnik vsebujeRecept;
    }
}