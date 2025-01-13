using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ZdravaPrehrana.Data;
using ZdravaPrehrana.Entitete;

namespace ZdravaPrehrana.Controllers
{
    public class UpravljalecCiljev
    {
        private readonly ApplicationDbContext _context;

        public UpravljalecCiljev(ApplicationDbContext context)
        {
            _context = context;
        }

        // Dodana metoda PridobiCilj
        public async Task<PrehranskiCilji> PridobiCilj(int uporabnikId)
        {
            return await _context.PrehranskiCilji
                .Include(c => c.Uporabnik)
                .FirstOrDefaultAsync(c => c.UporabnikId == uporabnikId);
        }

        public async Task<bool> NastaviCilje(PrehranskiCiljiPodatki podatki, int uporabnikId)
        {
            try
            {
                var cilj = new PrehranskiCilji
                {
                    UporabnikId = uporabnikId,
                    CiljnaTeza = podatki.CiljnaTeza,
                    CasovniOkvir = podatki.CasovniOkvir,
                    DnevneKalorije = podatki.DnevneKalorije
                };
                _context.PrehranskiCilji.Add(cilj);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<double> SpremljajNapredek(int uporabnikId)
        {
            var cilj = await _context.PrehranskiCilji
                .Include(c => c.Uporabnik)
                .FirstOrDefaultAsync(c => c.UporabnikId == uporabnikId);

            if (cilj == null)
                return 0;

            var zadnjiVnosi = await _context.VnosiHranil
                .Where(v => v.UporabnikId == uporabnikId)
                .OrderByDescending(v => v.Datum)
                .Take(7)
                .ToListAsync();

            return cilj.PreveriNapredek();
        }

        public async Task<bool> PosodobiCilje(PrehranskiCiljiPodatki podatki, int uporabnikId)
        {
            try
            {
                var cilj = await _context.PrehranskiCilji
                    .FirstOrDefaultAsync(c => c.UporabnikId == uporabnikId);

                if (cilj == null)
                    return false;

                cilj.PosodobiCilj(podatki);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> PreveriDoseganjeCiljev(int uporabnikId)
        {
            var cilj = await _context.PrehranskiCilji
                .Include(c => c.Uporabnik)
                .FirstOrDefaultAsync(c => c.UporabnikId == uporabnikId);

            if (cilj == null)
                return false;

            var zadnjiVnos = await _context.VnosiHranil
                .Where(v => v.UporabnikId == uporabnikId)
                .OrderByDescending(v => v.Datum)
                .FirstOrDefaultAsync();

            if (zadnjiVnos == null)
                return false;

            return zadnjiVnos.Kalorije <= cilj.DnevneKalorije;
        }
    }
}