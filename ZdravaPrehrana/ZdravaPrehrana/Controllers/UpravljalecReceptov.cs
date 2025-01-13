using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ZdravaPrehrana.Data;
using ZdravaPrehrana.Entitete;

namespace ZdravaPrehrana.Controllers
{
    public class UpravljalecReceptov
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UpravljalecReceptov> _logger;

        public UpravljalecReceptov(ApplicationDbContext context, ILogger<UpravljalecReceptov> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<Recept>> PridobiJavneRecepte()
        {
            try
            {
                return await _context.Recepti
                    .Include(r => r.ReceptSestavine)
                        .ThenInclude(rs => rs.Sestavina)
                    .Include(r => r.Avtor)
                    .Where(r => r.JeJaven)
                    .OrderByDescending(r => r.DatumUstvarjanja)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Napaka pri pridobivanju javnih receptov");
                return new List<Recept>();
            }
        }

        public async Task<List<Recept>> PridobiRecepteUporabnika(int uporabnikId)
        {
            try
            {
                return await _context.Recepti
                    .Include(r => r.ReceptSestavine)
                        .ThenInclude(rs => rs.Sestavina)
                    .Where(r => r.AvtorId == uporabnikId)
                    .OrderByDescending(r => r.DatumUstvarjanja)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Napaka pri pridobivanju receptov uporabnika {uporabnikId}");
                return new List<Recept>();
            }
        }

        public async Task<List<Recept>> PridobiVseRecepte(int uporabnikId)
        {
            try
            {
                return await _context.Recepti
                    .Include(r => r.ReceptSestavine)
                        .ThenInclude(rs => rs.Sestavina)
                    .Include(r => r.Avtor)
                    .Include(r => r.Ocene)
                    .Where(r => r.JeJaven || r.AvtorId == uporabnikId) // Samo javni in uporabnikovi recepti
                    .OrderByDescending(r => r.DatumUstvarjanja)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Napaka pri pridobivanju vseh receptov");
                return new List<Recept>();
            }
        }

        public async Task<bool> DodajRecept(Recept noviRecept, int avtorId)
        {
            try
            {
                using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    _logger.LogInformation("Zaèetek dodajanja recepta: {Naziv}", noviRecept.Naziv);

                    noviRecept.AvtorId = avtorId;
                    noviRecept.DatumUstvarjanja = DateTime.Now;

                    // Dodamo recept brez sestavin
                    await _context.Recepti.AddAsync(noviRecept);
                    await _context.SaveChangesAsync();

                    // Dodamo sestavine posebej
                    if (noviRecept.ReceptSestavine != null)
                    {
                        foreach (var receptSestavina in noviRecept.ReceptSestavine)
                        {
                            receptSestavina.ReceptId = noviRecept.Id;
                            await _context.ReceptSestavine.AddAsync(receptSestavina);
                        }
                        await _context.SaveChangesAsync();
                    }

                    await transaction.CommitAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Napaka pri dodajanju recepta: {Message}", ex.Message);
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Napaka pri dodajanju recepta: {Message}", ex.Message);
                return false;
            }
        }

        public async Task<bool> UrediRecept(int id, Recept posodobljeniRecept, int avtorId)
        {
            try
            {
                using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    var receptZaUrejanje = await _context.Recepti
                        .Include(r => r.ReceptSestavine)
                        .FirstOrDefaultAsync(r => r.Id == id && r.AvtorId == avtorId);

                    if (receptZaUrejanje == null)
                    {
                        _logger.LogWarning("Recept {Id} ni bil najden ali uporabnik ni lastnik", id);
                        return false;
                    }

                    // Update basic recipe info
                    receptZaUrejanje.Naziv = posodobljeniRecept.Naziv;
                    receptZaUrejanje.Postopek = posodobljeniRecept.Postopek;
                    receptZaUrejanje.Kalorije = posodobljeniRecept.Kalorije;
                    receptZaUrejanje.CasPriprave = posodobljeniRecept.CasPriprave;
                    receptZaUrejanje.JeJaven = posodobljeniRecept.JeJaven;

                    // Najprej shranimo osnovne spremembe
                    await _context.SaveChangesAsync();

                    // Odstranimo stare sestavine
                    _context.ReceptSestavine.RemoveRange(receptZaUrejanje.ReceptSestavine);
                    await _context.SaveChangesAsync();

                    // Dodamo nove sestavine
                    foreach (var rs in posodobljeniRecept.ReceptSestavine)
                    {
                        rs.ReceptId = id;
                        await _context.ReceptSestavine.AddAsync(rs);
                    }
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Napaka pri urejanju recepta: {Message}", ex.Message);
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Napaka pri urejanju recepta: {Message}", ex.Message);
                return false;
            }
        }

        public async Task<bool> IzbrisiRecept(int id, int avtorId)
        {
            try
            {
                var receptZaBrisanje = await _context.Recepti
                    .FirstOrDefaultAsync(r => r.Id == id && r.AvtorId == avtorId);

                if (receptZaBrisanje == null)
                {
                    _logger.LogWarning("Recept {Id} ni bil najden ali uporabnik ni lastnik", id);
                    return false;
                }

                _context.Recepti.Remove(receptZaBrisanje);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Recept {Id} uspešno izbrisan", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Napaka pri brisanju recepta {Id}", id);
                return false;
            }
        }

        public async Task<List<Recept>> PoisciRecepte(string iskaniNiz, bool samoJavni = true)
        {
            try
            {
                var query = _context.Recepti
                    .Include(r => r.ReceptSestavine)
                        .ThenInclude(rs => rs.Sestavina)
                    .Include(r => r.Avtor)
                    .Where(r => r.Naziv.ToLower().Contains(iskaniNiz.ToLower()));

                if (samoJavni)
                {
                    query = query.Where(r => r.JeJaven);
                }

                return await query.OrderByDescending(r => r.DatumUstvarjanja)
                                .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Napaka pri iskanju receptov z nizom '{IskaniNiz}'", iskaniNiz);
                return new List<Recept>();
            }
        }

        public async Task<Recept> PridobiRecept(int id, int? uporabnikId = null)
        {
            try
            {
                var query = _context.Recepti
                    .Include(r => r.ReceptSestavine)
                        .ThenInclude(rs => rs.Sestavina)
                    .Include(r => r.Ocene)
                    .Include(r => r.Avtor);

                if (uporabnikId.HasValue)
                {
                    return await query.FirstOrDefaultAsync(r => r.Id == id &&
                        (r.JeJaven || r.AvtorId == uporabnikId.Value));
                }
                else
                {
                    return await query.FirstOrDefaultAsync(r => r.Id == id && r.JeJaven);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Napaka pri pridobivanju recepta {Id}", id);
                return null;
            }
        }
    }
}