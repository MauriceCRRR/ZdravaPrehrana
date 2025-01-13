using ZdravaPrehrana.Data;
using ZdravaPrehrana.Entitete;
using Microsoft.EntityFrameworkCore;

namespace ZdravaPrehrana.Controllers
{
    public class UpravljalecNakupovanja
    {
        private readonly ApplicationDbContext _context;

        public UpravljalecNakupovanja(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<NakupovalniSeznam> UstvariSeznam(string naziv, int uporabnikId)
        {
            try
            {
                var seznam = new NakupovalniSeznam
                {
                    Naziv = naziv,
                    DatumKreiranja = DateTime.Now,
                    UporabnikId = uporabnikId,
                    Postavke = new List<SeznamPostavka>()
                };

                _context.NakupovalniSeznami.Add(seznam);
                await _context.SaveChangesAsync();
                return await PridobiSeznam(seznam.Id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Napaka pri ustvarjanju seznama: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> DodajIzdelek(string naziv, double kolicina, string enota, int seznamId)
        {
            try
            {
                var seznam = await _context.NakupovalniSeznami
                    .Include(s => s.Postavke)
                    .FirstOrDefaultAsync(s => s.Id == seznamId);

                if (seznam == null)
                {
                    Console.WriteLine($"Seznam z ID {seznamId} ni bil najden.");
                    return false;
                }

                var novaPostavka = new SeznamPostavka
                {
                    Naziv = naziv,
                    Kolicina = kolicina,
                    Enota = enota,
                    JeObkljukana = false,
                    NakupovalniSeznamId = seznamId
                };

                // Dodamo postavko v DbSet in v seznam
                _context.SeznamPostavke.Add(novaPostavka);
                seznam.Postavke.Add(novaPostavka);

                await _context.SaveChangesAsync();

                Console.WriteLine($"Uspešno dodan izdelek {naziv} v seznam {seznamId}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Napaka pri dodajanju izdelka: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return false;
            }
        }

        public async Task<bool> OdstraniIzdelek(int seznamId, string naziv)
        {
            try
            {
                var seznam = await _context.NakupovalniSeznami
                    .Include(s => s.Postavke)
                    .FirstOrDefaultAsync(s => s.Id == seznamId);

                if (seznam == null) return false;

                var postavka = seznam.Postavke.FirstOrDefault(p => p.Naziv == naziv);
                if (postavka != null)
                {
                    seznam.Postavke.Remove(postavka);
                    _context.SeznamPostavke.Remove(postavka);
                    await _context.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Napaka pri odstranjevanju izdelka: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> OznaciIzdelek(int seznamId, string naziv)
        {
            try
            {
                var seznam = await _context.NakupovalniSeznami
                    .Include(s => s.Postavke)
                    .FirstOrDefaultAsync(s => s.Id == seznamId);

                if (seznam == null) return false;

                var postavka = seznam.Postavke.FirstOrDefault(p => p.Naziv == naziv);
                if (postavka != null)
                {
                    postavka.JeObkljukana = !postavka.JeObkljukana;
                    await _context.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Napaka pri oznaèevanju izdelka: {ex.Message}");
                return false;
            }
        }

        public async Task<NakupovalniSeznam> PridobiSeznam(int seznamId)
        {
            return await _context.NakupovalniSeznami
                .Include(s => s.Postavke)
                .FirstOrDefaultAsync(s => s.Id == seznamId);
        }

        public async Task<List<NakupovalniSeznam>> PridobiSezname(int uporabnikId)
        {
            return await _context.NakupovalniSeznami
                .Include(s => s.Postavke)
                .Where(s => s.UporabnikId == uporabnikId)
                .OrderByDescending(s => s.DatumKreiranja)
                .ToListAsync();
        }
    }
}