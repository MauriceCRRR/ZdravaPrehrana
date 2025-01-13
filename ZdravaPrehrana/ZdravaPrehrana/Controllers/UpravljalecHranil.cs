using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ZdravaPrehrana.Data;
using ZdravaPrehrana.Entitete;

namespace ZdravaPrehrana.Controllers
{
    public class UpravljalecHranil
    {
        private readonly ApplicationDbContext _context;

        public UpravljalecHranil(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> DodajVnosHranil(VnosHranilPodatki podatki)
        {
            try
            {
                if (!PreveriVnos(podatki))
                    return false;

                var vnos = new VnosHranil
                {
                    Datum = podatki.Datum,
                    Kalorije = podatki.Kalorije,
                    Beljakovine = podatki.Beljakovine,
                    Mascobe = podatki.Mascobe,
                    OgljikoviHidrati = podatki.OgljikoviHidrati,
                    UporabnikId = podatki.UporabnikId
                };

                _context.VnosiHranil.Add(vnos);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool PreveriVnos(VnosHranilPodatki podatki)
        {
            // Validacija podatkov
            if (podatki == null) return false;

            // Preveri ali so vrednosti v smiselnih mejah
            if (podatki.Kalorije < 0 || podatki.Kalorije > 10000) return false;
            if (podatki.Beljakovine < 0 || podatki.Beljakovine > 1000) return false;
            if (podatki.Mascobe < 0 || podatki.Mascobe > 1000) return false;
            if (podatki.OgljikoviHidrati < 0 || podatki.OgljikoviHidrati > 1000) return false;

            // Preveri ali je datum veljaven
            if (podatki.Datum > DateTime.Now) return false;

            return true;
        }

        public async Task<double> IzracunHranilneVrednosti(int uporabnikId, DateTime datum)
        {
            var dnevniVnos = await PridobiDnevniVnos(datum, uporabnikId);
            if (dnevniVnos == null) return 0;

            // Izraèun celotne hranilne vrednosti
            // Primer izraèuna: kalorije + (beljakovine * 4) + (mascobe * 9) + (ogljikoviHidrati * 4)
            return dnevniVnos.Kalorije +
                   (dnevniVnos.Beljakovine * 4) +
                   (dnevniVnos.Mascobe * 9) +
                   (dnevniVnos.OgljikoviHidrati * 4);
        }

        public async Task<VnosHranil> PridobiDnevniVnos(DateTime datum, int uporabnikId)
        {
            return await _context.VnosiHranil
                .FirstOrDefaultAsync(v =>
                    v.UporabnikId == uporabnikId &&
                    v.Datum.Date == datum.Date);
        }

        public async Task<VnosHranilStatistika> PridobiStatistiko(int uporabnikId, DateTime odDatum, DateTime doDatum)
        {
            var vnosi = await _context.VnosiHranil
                .Where(v =>
                    v.UporabnikId == uporabnikId &&
                    v.Datum >= odDatum &&
                    v.Datum <= doDatum)
                .ToListAsync();

            // Èe ni vnosov, vrni prazno statistiko
            if (!vnosi.Any())
            {
                return new VnosHranilStatistika
                {
                    PovprecneKalorije = 0,
                    PovprecneBeljakovine = 0,
                    PovprecneMascobe = 0,
                    PovprecniOgljikoviHidrati = 0,
                    SteviloVnosov = 0
                };
            }

            var statistika = new VnosHranilStatistika
            {
                PovprecneKalorije = vnosi.Average(v => v.Kalorije),
                PovprecneBeljakovine = vnosi.Average(v => v.Beljakovine),
                PovprecneMascobe = vnosi.Average(v => v.Mascobe),
                PovprecniOgljikoviHidrati = vnosi.Average(v => v.OgljikoviHidrati),
                SteviloVnosov = vnosi.Count
            };

            return statistika;
        }
    }

    

    public class VnosHranilStatistika
    {
        public double PovprecneKalorije { get; set; }
        public double PovprecneBeljakovine { get; set; }
        public double PovprecneMascobe { get; set; }
        public double PovprecniOgljikoviHidrati { get; set; }
        public int SteviloVnosov { get; set; }
    }
}