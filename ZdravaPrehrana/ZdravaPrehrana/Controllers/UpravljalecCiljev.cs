using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ZdravaPrehrana.Data;
using ZdravaPrehrana.Entitete;

namespace ZdravaPrehrana.Controllers
{
    public class UpravljalecCiljev
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UpravljalecCiljev> _logger;

        public UpravljalecCiljev(
            ApplicationDbContext context,
            ILogger<UpravljalecCiljev> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<PrehranskiCilji> PridobiCilj(int uporabnikId)
        {
            try
            {
                return await _context.PrehranskiCilji
                    .Include(c => c.Uporabnik)
                    .ThenInclude(u => u.Profil)
                    .FirstOrDefaultAsync(c => c.UporabnikId == uporabnikId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Napaka pri pridobivanju cilja za uporabnika {UporabnikId}", uporabnikId);
                throw;
            }
        }

        public async Task<bool> NastaviCilje(PrehranskiCiljiPodatki podatki, int uporabnikId)
        {
            try
            {
                if (podatki == null)
                    throw new ArgumentNullException(nameof(podatki));

                var obstojeci = await _context.PrehranskiCilji
                    .FirstOrDefaultAsync(c => c.UporabnikId == uporabnikId);

                if (obstojeci != null)
                {
                    obstojeci.PosodobiCilj(podatki);
                }
                else
                {
                    var cilj = new PrehranskiCilji
                    {
                        UporabnikId = uporabnikId,
                        CiljnaTeza = podatki.CiljnaTeza,
                        TedenIzgubaKg = podatki.TedenIzgubaKg,
                        CasovniOkvir = podatki.CasovniOkvir,
                        DnevneKalorije = podatki.DnevneKalorije,
                        BMR = podatki.BMR,
                        TDEE = podatki.TDEE
                    };
                    _context.PrehranskiCilji.Add(cilj);
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Napaka pri nastavljanju ciljev za uporabnika {UporabnikId}", uporabnikId);
                return false;
            }
        }

        public async Task<double> SpremljajNapredek(int uporabnikId)
        {
            try
            {
                var cilj = await _context.PrehranskiCilji
                    .Include(c => c.Uporabnik)
                    .ThenInclude(u => u.Profil)
                    .FirstOrDefaultAsync(c => c.UporabnikId == uporabnikId);

                if (cilj == null)
                    return 0;

                return cilj.PreveriNapredek();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Napaka pri spremljanju napredka za uporabnika {UporabnikId}", uporabnikId);
                return 0;
            }
        }

        public async Task<bool> PreveriDoseganjeCiljev(int uporabnikId)
        {
            try
            {
                var cilj = await _context.PrehranskiCilji
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Napaka pri preverjanju doseganja ciljev za uporabnika {UporabnikId}", uporabnikId);
                return false;
            }
        }
    }
}