using Microsoft.EntityFrameworkCore;
using ZdravaPrehrana.Data;
using ZdravaPrehrana.Entitete;
using ZdravaPrehrana.Controllers;

namespace ZdravaPrehrana.Controllers
{
    public class UpravljalecOcen
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UpravljalecOcen> _logger;

        public UpravljalecOcen(ApplicationDbContext context, ILogger<UpravljalecOcen> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> DodajOceno(int jedilnikId, int uporabnikId, int vrednost, string komentar)
        {
            try
            {
                var obstojocaOcena = await _context.JedilnikOcene
                    .FirstOrDefaultAsync(o => o.JedilnikId == jedilnikId && o.UporabnikId == uporabnikId);

                if (obstojocaOcena != null)
                {
                    // Posodobi obstoječo oceno
                    obstojocaOcena.Vrednost = vrednost;
                    obstojocaOcena.Komentar = komentar;
                    obstojocaOcena.DatumOcene = DateTime.Now;
                }
                else
                {
                    // Ustvari novo oceno
                    var novaOcena = new JedilnikOcena
                    {
                        JedilnikId = jedilnikId,
                        UporabnikId = uporabnikId,
                        Vrednost = vrednost,
                        Komentar = komentar,
                        DatumOcene = DateTime.Now
                    };
                    _context.JedilnikOcene.Add(novaOcena);
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Napaka pri dodajanju ocene za jedilnik {JedilnikId}", jedilnikId);
                return false;
            }
        }

        public async Task<List<JedilnikOcena>> PridobiOceneJedilnika(int jedilnikId)
        {
            try
            {
                return await _context.JedilnikOcene
                    .Include(o => o.Uporabnik)
                    .Where(o => o.JedilnikId == jedilnikId)
                    .OrderByDescending(o => o.DatumOcene)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Napaka pri pridobivanju ocen za jedilnik {JedilnikId}", jedilnikId);
                return new List<JedilnikOcena>();
            }
        }

        public async Task<double> IzracunajPovprecnoOceno(int jedilnikId)
        {
            try
            {
                var ocene = await _context.JedilnikOcene
                    .Where(o => o.JedilnikId == jedilnikId)
                    .Select(o => o.Vrednost)
                    .ToListAsync();

                if (!ocene.Any())
                    return 0;

                return Math.Round(ocene.Average(), 2);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Napaka pri računanju povprečne ocene za jedilnik {JedilnikId}", jedilnikId);
                return 0;
            }
        }
    }
}