using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ZdravaPrehrana.Entitete;

namespace ZdravaPrehrana.Boundary
{
    public class ReceptiOkno 
    {
        public void PrikaziSeznamReceptov([Optional, DefaultParameterValue(null)] List<Recept> recepti) 
        {
            throw new System.NotImplementedException("Not implemented");
        }

        public void PrikaziPodrobnostiRecepta(Recept recept) 
        {
            throw new System.NotImplementedException("Not implemented");
        }

        public void PrikaziObrazecZaDodajanje() 
        {
            throw new System.NotImplementedException("Not implemented");
        }

        public void PrikaziObrazecZaUrejanje(Recept recept) 
        {
            throw new System.NotImplementedException("Not implemented");
        }

        private GlavnoOkno odpreReceptiOkno;
    }
}