using System;
using ZdravaPrehrana.Boundary;

namespace ZdravaPrehrana.Boundary { 
 
    public class GlavnoOkno
    {
        public void PrikazGlavniMenu()
        {
            throw new System.NotImplementedException("Not implemented");
        }

        public void PrikaziSporocilo(ref string sporocilo)
        {
            throw new System.NotImplementedException("Not implemented");
        }

        private JedilnikOkno odpreJedilnikOkno;
        private NakupovalniSeznamOkno odpreNakupovalniSeznamOkno;
        private NasvetiOkno odpreNasvetiOkno;
        private CiljiOkno odpreCiljiOkno;
        private HranilaOkno odpreHranilaOkno;
        private ReceptiOkno odpreReceptiOkno;
    }
}