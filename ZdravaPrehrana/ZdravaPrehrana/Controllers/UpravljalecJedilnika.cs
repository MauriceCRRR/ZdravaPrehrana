using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ZdravaPrehrana.Data;
using ZdravaPrehrana.Entitete;

namespace ZdravaPrehrana.Controllers
{
    public class UpravljalecJedilnika
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UpravljalecJedilnika> _logger;

        public UpravljalecJedilnika(ApplicationDbContext context, ILogger<UpravljalecJedilnika> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Jedilnik> KreirajJedilnik(string naziv, int uporabnikId)
        {
            try
            {
                _logger.LogInformation("Zaèetek kreiranja jedilnika za uporabnika {UporabnikId}", uporabnikId);

                var noviJedilnik = new Jedilnik
                {
                    Naziv = naziv,
                    DatumKreiranja = DateTime.Now,
                    UporabnikId = uporabnikId
                };

                _context.Jedilniki.Add(noviJedilnik);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Jedilnik uspešno kreiran z ID {JedilnikId}", noviJedilnik.Id);
                return noviJedilnik;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Napaka pri kreiranju jedilnika za uporabnika {UporabnikId}", uporabnikId);
                return null;
            }
        }

        public async Task<bool> DodajObrok(int jedilnikId, Obrok obrok)
        {
            try
            {
                var jedilnik = await _context.Jedilniki
                    .Include(j => j.Obroki)
                    .FirstOrDefaultAsync(j => j.Id == jedilnikId);

                if (jedilnik == null)
                {
                    _logger.LogWarning("Jedilnik z ID {JedilnikId} ne obstaja", jedilnikId);
                    return false;
                }

                obrok.JedilnikId = jedilnikId;
                jedilnik.Obroki.Add(obrok);

                await _context.SaveChangesAsync();
                _logger.LogInformation("Obrok dodan v jedilnik {JedilnikId}", jedilnikId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Napaka pri dodajanju obroka v jedilnik {JedilnikId}", jedilnikId);
                return false;
            }
        }

        public async Task<bool> DodajReceptVObrok(int obrokId, int receptId)
        {
            try
            {
                var obrok = await _context.Obroki
                    .Include(o => o.Recepti)
                    .FirstOrDefaultAsync(o => o.Id == obrokId);

                if (obrok == null)
                {
                    _logger.LogWarning("Obrok z ID {ObrokId} ne obstaja", obrokId);
                    return false;
                }

                var recept = await _context.Recepti
                    .FirstOrDefaultAsync(r => r.Id == receptId);

                if (recept == null)
                {
                    _logger.LogWarning("Recept z ID {ReceptId} ne obstaja", receptId);
                    return false;
                }

                if (obrok.Recepti.Any(r => r.Id == receptId))
                {
                    _logger.LogInformation("Recept je že dodan v obrok");
                    return true;
                }

                obrok.Recepti.Add(recept);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Recept {ReceptId} dodan v obrok {ObrokId}", receptId, obrokId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Napaka pri dodajanju recepta {ReceptId} v obrok {ObrokId}", receptId, obrokId);
                return false;
            }
        }

        public async Task<bool> ShraniJedilnik(Jedilnik jedilnik)
        {
            try
            {
                _context.Jedilniki.Update(jedilnik);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Jedilnik {JedilnikId} uspešno shranjen", jedilnik.Id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Napaka pri shranjevanju jedilnika {JedilnikId}", jedilnik.Id);
                return false;
            }
        }

        public async Task<bool> OdstraniObrok(int jedilnikId, int obrokId)
        {
            try
            {
                var jedilnik = await _context.Jedilniki
                    .Include(j => j.Obroki)
                    .FirstOrDefaultAsync(j => j.Id == jedilnikId);

                if (jedilnik == null)
                {
                    _logger.LogWarning("Jedilnik z ID {JedilnikId} ne obstaja", jedilnikId);
                    return false;
                }

                var obrok = jedilnik.Obroki.FirstOrDefault(o => o.Id == obrokId);
                if (obrok == null)
                {
                    _logger.LogWarning("Obrok z ID {ObrokId} ne obstaja v jedilniku {JedilnikId}", obrokId, jedilnikId);
                    return false;
                }

                jedilnik.Obroki.Remove(obrok);
                _context.Obroki.Remove(obrok);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Obrok {ObrokId} odstranjen iz jedilnika {JedilnikId}", obrokId, jedilnikId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Napaka pri odstranjevanju obroka {ObrokId} iz jedilnika {JedilnikId}",
                    obrokId, jedilnikId);
                return false;
            }
        }

        public async Task<Jedilnik> PridobiJedilnik(int id)
        {
            try
            {
                var jedilnik = await _context.Jedilniki
                    .Include(j => j.Obroki)
                        .ThenInclude(o => o.Recepti)
                    .FirstOrDefaultAsync(j => j.Id == id);

                if (jedilnik == null)
                {
                    _logger.LogWarning("Jedilnik z ID {JedilnikId} ne obstaja", id);
                }

                return jedilnik;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Napaka pri pridobivanju jedilnika {JedilnikId}", id);
                return null;
            }
        }

        public async Task<bool> OdstraniReceptIzObroka(int obrokId, int receptId)
        {
            try
            {
                _logger.LogInformation("Odstranjevanje recepta {ReceptId} iz obroka {ObrokId}", receptId, obrokId);

                var obrok = await _context.Obroki
                    .Include(o => o.Recepti)
                    .FirstOrDefaultAsync(o => o.Id == obrokId);

                if (obrok == null)
                {
                    _logger.LogWarning("Obrok {ObrokId} ne obstaja", obrokId);
                    return false;
                }

                var recept = obrok.Recepti.FirstOrDefault(r => r.Id == receptId);
                if (recept == null)
                {
                    _logger.LogWarning("Recept {ReceptId} ni najden v obroku {ObrokId}", receptId, obrokId);
                    return false;
                }

                obrok.Recepti.Remove(recept);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Recept {ReceptId} uspešno odstranjen iz obroka {ObrokId}", receptId, obrokId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Napaka pri odstranjevanju recepta {ReceptId} iz obroka {ObrokId}", receptId, obrokId);
                return false;
            }
        }

        public async Task<List<Jedilnik>> PridobiJedilnikeUporabnika(int uporabnikId)
        {
            try
            {
                return await _context.Jedilniki
                    .Where(j => j.UporabnikId == uporabnikId)
                    .Include(j => j.Obroki)
                        .ThenInclude(o => o.Recepti)
                    .OrderByDescending(j => j.DatumKreiranja)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Napaka pri pridobivanju jedilnikov za uporabnika {UporabnikId}", uporabnikId);
                return new List<Jedilnik>();
            }
        }

        public async Task<Obrok> PridobiObrok(int obrokId)
        {
            try
            {
                return await _context.Obroki
                    .Include(o => o.Jedilnik)
                    .Include(o => o.Recepti)
                    .FirstOrDefaultAsync(o => o.Id == obrokId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Napaka pri pridobivanju obroka {ObrokId}", obrokId);
                return null;
            }
        }

        public async Task<bool> PreveriSkladnostCiljev(Jedilnik jedilnik)
        {
            // Implementacija preverjanja skladnosti s cilji
            return true;
        }


        public async Task<bool> DeliJedilnik(int jedilnikId, int uporabnikId)
        {
            try
            {
                _logger.LogInformation("Deljenje jedilnika {JedilnikId}", jedilnikId);

                var jedilnik = await _context.Jedilniki
                    .Include(j => j.DeliZ)
                    .FirstOrDefaultAsync(j => j.Id == jedilnikId && j.UporabnikId == uporabnikId);

                if (jedilnik == null)
                {
                    _logger.LogWarning("Jedilnik {JedilnikId} ne obstaja ali uporabnik ni lastnik", jedilnikId);
                    return false;
                }

                jedilnik.JeDeljiv = true;
                jedilnik.DatumDeljenja = DateTime.Now;
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Napaka pri deljenju jedilnika {JedilnikId}", jedilnikId);
                return false;
            }
        }

        public async Task<bool> ShraniKopijo(int jedilnikId, int uporabnikId)
        {
            try
            {
                var izvorniJedilnik = await _context.Jedilniki
                    .Include(j => j.Obroki)
                        .ThenInclude(o => o.Recepti)
                    .FirstOrDefaultAsync(j => j.Id == jedilnikId && j.JeDeljiv);

                if (izvorniJedilnik == null)
                {
                    _logger.LogWarning("Jedilnik {JedilnikId} ne obstaja ali ni deljiv", jedilnikId);
                    return false;
                }

                var noviJedilnik = new Jedilnik
                {
                    Naziv = izvorniJedilnik.Naziv + " (Kopija)",
                    DatumKreiranja = DateTime.Now,
                    UporabnikId = uporabnikId,
                    JeDeljiv = false
                };

                foreach (var izvorniObrok in izvorniJedilnik.Obroki)
                {
                    var noviObrok = new Obrok
                    {
                        Naziv = izvorniObrok.Naziv,
                        Cas = izvorniObrok.Cas,
                        Recepti = new List<Recept>(izvorniObrok.Recepti)
                    };
                    noviJedilnik.Obroki.Add(noviObrok);
                }

                _context.Jedilniki.Add(noviJedilnik);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Ustvarjena kopija jedilnika {IzvorniId} za uporabnika {UporabnikId}",
                    jedilnikId, uporabnikId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Napaka pri kopiranju jedilnika {JedilnikId}", jedilnikId);
                return false;
            }
        }

        public async Task<List<Jedilnik>> PridobiDeljeneJedilnike()
        {
            try
            {
                return await _context.Jedilniki
                    .Include(j => j.Uporabnik)
                    .Include(j => j.Obroki)
                        .ThenInclude(o => o.Recepti)
                    .Include(j => j.Ocene)  // Dodamo Include za ocene
                        .ThenInclude(o => o.Uporabnik)  // In uporabnika za vsako oceno
                    .Where(j => j.JeDeljiv)
                    .OrderByDescending(j => j.DatumDeljenja)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Napaka pri pridobivanju deljenih jedilnikov");
                return new List<Jedilnik>();
            }
        }

        public async Task<bool> OdstraniDeljenje(int jedilnikId, int uporabnikId)
        {
            try
            {
                var jedilnik = await _context.Jedilniki
                    .Include(j => j.Uporabnik)
                    .FirstOrDefaultAsync(j => j.Id == jedilnikId);

                if (jedilnik == null)
                {
                    return false;
                }

                var uporabnik = await _context.Uporabniki
                    .FirstOrDefaultAsync(u => u.Id == uporabnikId);

                // Preveri èe je uporabnik lastnik ali strokovnjak
                if (jedilnik.UporabnikId != uporabnikId &&
                    uporabnik?.Vloga != UporabniskaVloga.Strokovnjak)
                {
                    return false;
                }

                jedilnik.JeDeljiv = false;
                jedilnik.DatumDeljenja = null;
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Napaka pri odstranjevanju deljenja jedilnika {JedilnikId}", jedilnikId);
                return false;
            }
        }
    }
}