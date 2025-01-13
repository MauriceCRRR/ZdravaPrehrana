using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ZdravaPrehrana.Data;
using ZdravaPrehrana.Entitete;

namespace ZdravaPrehrana.Services
{
    public interface IUpravljalecCiljevService
    {
        Task<PrehranskiCilji> UstvariCilj(int uporabnikId, PrehranskiCiljiPodatki podatki);
        Task<bool> PosodobiCilj(int uporabnikId, PrehranskiCiljiPodatki podatki);
        Task<PrehranskiCilji> PridobiCilj(int uporabnikId);
        Task<double> IzracunajNapredek(int uporabnikId);
        Task<bool> UporabnikImaCilj(int uporabnikId);
        Task<bool> IzbrisiCilj(int uporabnikId);
    }

    public class UpravljalecCiljevService : IUpravljalecCiljevService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UpravljalecCiljevService> _logger;

        public UpravljalecCiljevService(
            ApplicationDbContext context,
            ILogger<UpravljalecCiljevService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        private bool ValidateData(PrehranskiCiljiPodatki podatki)
        {
            if (podatki == null) return false;
            if (podatki.CiljnaTeza < 30 || podatki.CiljnaTeza > 300) return false;
            if (podatki.TedenIzgubaKg < 0.1 || podatki.TedenIzgubaKg > 1.0) return false;
            if (podatki.DnevneKalorije < 1200 || podatki.DnevneKalorije > 10000) return false;
            if (podatki.BMR <= 0) return false;
            if (podatki.TDEE <= 0) return false;
            return true;
        }

        public async Task<PrehranskiCilji> UstvariCilj(int uporabnikId, PrehranskiCiljiPodatki podatki)
        {
            try
            {
                _logger.LogInformation("Začetek ustvarjanja cilja za uporabnika {UporabnikId}", uporabnikId);

                if (!ValidateData(podatki))
                {
                    _logger.LogWarning("Neveljavni podatki za uporabnika {UporabnikId}: {@Podatki}",
                        uporabnikId, podatki);
                    return null;
                }

                var noviCilj = new PrehranskiCilji
                {
                    UporabnikId = uporabnikId,
                    CiljnaTeza = podatki.CiljnaTeza,
                    TedenIzgubaKg = podatki.TedenIzgubaKg,
                    CasovniOkvir = podatki.CasovniOkvir,
                    DnevneKalorije = podatki.DnevneKalorije,
                    BMR = podatki.BMR,
                    TDEE = podatki.TDEE
                };

                _logger.LogInformation("Dodajanje novega cilja: {@NoviCilj}", noviCilj);

                await _context.PrehranskiCilji.AddAsync(noviCilj);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Cilj uspešno ustvarjen za uporabnika {UporabnikId}", uporabnikId);

                return await PridobiCilj(uporabnikId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Napaka pri ustvarjanju cilja za uporabnika {UporabnikId}", uporabnikId);
                throw;
            }
        }

        public async Task<bool> PosodobiCilj(int uporabnikId, PrehranskiCiljiPodatki podatki)
        {
            try
            {
                if (!ValidateData(podatki))
                {
                    _logger.LogWarning("Neveljavni podatki za uporabnika {UporabnikId}", uporabnikId);
                    return false;
                }

                var cilj = await _context.PrehranskiCilji
                    .Include(c => c.Uporabnik)
                    .ThenInclude(u => u.Profil)
                    .FirstOrDefaultAsync(c => c.UporabnikId == uporabnikId);

                if (cilj == null)
                {
                    _logger.LogWarning("Cilj za uporabnika {UporabnikId} ne obstaja", uporabnikId);
                    return false;
                }

                cilj.PosodobiCilj(podatki);

                _context.PrehranskiCilji.Update(cilj);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Cilj za uporabnika {UporabnikId} uspešno posodobljen", uporabnikId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Napaka pri posodabljanju cilja za uporabnika {UporabnikId}", uporabnikId);
                return false;
            }
        }

        public async Task<PrehranskiCilji> PridobiCilj(int uporabnikId)
        {
            try
            {
                var cilj = await _context.PrehranskiCilji
                    .Include(c => c.Uporabnik)
                    .ThenInclude(u => u.Profil)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.UporabnikId == uporabnikId);

                if (cilj == null)
                {
                    _logger.LogInformation("Uporabnik {UporabnikId} še nima nastavljenih ciljev", uporabnikId);
                }
                else
                {
                    _logger.LogInformation("Uspešno pridobljeni cilji za uporabnika {UporabnikId}", uporabnikId);
                }

                return cilj;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Napaka pri pridobivanju cilja za uporabnika {UporabnikId}", uporabnikId);
                throw;
            }
        }

        public async Task<double> IzracunajNapredek(int uporabnikId)
        {
            try
            {
                var cilj = await _context.PrehranskiCilji
                    .Include(c => c.Uporabnik)
                    .ThenInclude(u => u.Profil)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.UporabnikId == uporabnikId);

                if (cilj?.Uporabnik?.Profil == null)
                {
                    _logger.LogWarning("Cilj ali profil za uporabnika {UporabnikId} ne obstaja", uporabnikId);
                    return 0;
                }

                return cilj.PreveriNapredek();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Napaka pri izračunu napredka za uporabnika {UporabnikId}", uporabnikId);
                return 0;
            }
        }

        public async Task<bool> UporabnikImaCilj(int uporabnikId)
        {
            try
            {
                return await _context.PrehranskiCilji
                    .AsNoTracking()
                    .AnyAsync(c => c.UporabnikId == uporabnikId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Napaka pri preverjanju obstoja cilja za uporabnika {UporabnikId}", uporabnikId);
                return false;
            }
        }

        public async Task<bool> IzbrisiCilj(int uporabnikId)
        {
            try
            {
                var cilj = await _context.PrehranskiCilji
                    .FirstOrDefaultAsync(c => c.UporabnikId == uporabnikId);

                if (cilj == null)
                {
                    _logger.LogWarning("Cilj za uporabnika {UporabnikId} ne obstaja", uporabnikId);
                    return false;
                }

                _context.PrehranskiCilji.Remove(cilj);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Cilj za uporabnika {UporabnikId} uspešno izbrisan", uporabnikId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Napaka pri brisanju cilja za uporabnika {UporabnikId}", uporabnikId);
                return false;
            }
        }
    }
}