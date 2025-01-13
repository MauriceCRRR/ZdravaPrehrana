using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ZdravaPrehrana.Data;
using ZdravaPrehrana.Entitete;

namespace ZdravaPrehrana.Controllers
{
    public class UpravljalecNasvetov
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UpravljalecNasvetov> _logger;

        public UpravljalecNasvetov(ApplicationDbContext context, ILogger<UpravljalecNasvetov> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Nasvet> UstvariVprasanje(string vprasanje, int uporabnikId)
        {
            try
            {
                _logger.LogInformation($"Ustvarjanje novega vprašanja za uporabnika {uporabnikId}");

                var nasvet = new Nasvet
                {
                    Vprasanje = vprasanje,
                    DatumVprasanja = DateTime.Now,
                    UporabnikId = uporabnikId,
                    Status = StatusNasveta.CakaNaOdgovor
                };

                await _context.Nasveti.AddAsync(nasvet);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Vprašanje uspešno ustvarjeno z ID {nasvet.Id}");
                return nasvet;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Napaka pri ustvarjanju vprašanja za uporabnika {uporabnikId}");
                throw;
            }
        }

        public async Task<bool> OdgovoriNaNasvet(int nasvetId, string odgovor, int strokovnjakId)
        {
            try
            {
                var nasvet = await _context.Nasveti
                    .FirstOrDefaultAsync(n => n.Id == nasvetId && n.Status == StatusNasveta.CakaNaOdgovor);

                if (nasvet == null)
                {
                    _logger.LogWarning($"Nasvet z ID {nasvetId} ne obstaja ali je že odgovorjen");
                    return false;
                }

                nasvet.Odgovor = odgovor;
                nasvet.DatumOdgovora = DateTime.Now;
                nasvet.StrokovnjakId = strokovnjakId;
                nasvet.Status = StatusNasveta.Odgovorjen;

                await _context.SaveChangesAsync();
                _logger.LogInformation($"Uspešno odgovorjeno na nasvet {nasvetId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Napaka pri odgovarjanju na nasvet {nasvetId}");
                return false;
            }
        }

        public async Task<List<Nasvet>> PridobiVsaNasvetaVprasanja()
        {
            try
            {
                return await _context.Nasveti
                    .Include(n => n.Uporabnik)
                    .Include(n => n.Strokovnjak)
                    .OrderByDescending(n => n.DatumVprasanja)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Napaka pri pridobivanju vseh nasvetov");
                throw;
            }
        }

        public async Task<List<Nasvet>> PridobiNeodgovorjenaNasvetaVprasanja()
        {
            try
            {
                return await _context.Nasveti
                    .Include(n => n.Uporabnik)
                    .Where(n => n.Status == StatusNasveta.CakaNaOdgovor)
                    .OrderByDescending(n => n.DatumVprasanja)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Napaka pri pridobivanju neodgovorjenih nasvetov");
                throw;
            }
        }

        public async Task<List<Nasvet>> PridobiNasveteUporabnika(int uporabnikId)
        {
            try
            {
                return await _context.Nasveti
                    .Include(n => n.Strokovnjak)
                    .Where(n => n.UporabnikId == uporabnikId)
                    .OrderByDescending(n => n.DatumVprasanja)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Napaka pri pridobivanju nasvetov za uporabnika {uporabnikId}");
                throw;
            }
        }

        public async Task<bool> PreveriAliJeNasvetOdgovorjen(int nasvetId)
        {
            try
            {
                var nasvet = await _context.Nasveti.FindAsync(nasvetId);
                return nasvet?.Status == StatusNasveta.Odgovorjen;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Napaka pri preverjanju statusa nasveta {nasvetId}");
                throw;
            }
        }
    }
}