using Microsoft.EntityFrameworkCore;
using ZdravaPrehrana.Data;
using ZdravaPrehrana.Entitete;

namespace ZdravaPrehrana.Services
{
    public interface IUpravljalecCiljevService
    {
        Task<PrehranskiCilji> UstvariCilj(int uporabnikId, PrehranskiCiljiPodatki podatki);
        Task<bool> PosodobiCilj(int ciljId, PrehranskiCiljiPodatki podatki);
        Task<PrehranskiCilji> PridobiCilj(int uporabnikId);
        Task<double> IzracunajNapredek(int ciljId);
    }

    public class UpravljalecCiljevService : IUpravljalecCiljevService
    {
        private readonly ApplicationDbContext _context;

        public UpravljalecCiljevService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PrehranskiCilji> UstvariCilj(int uporabnikId, PrehranskiCiljiPodatki podatki)
        {
            var cilj = new PrehranskiCilji
            {
                UporabnikId = uporabnikId
            };

            cilj.NastaviCilj(podatki);

            _context.PrehranskiCilji.Add(cilj);
            await _context.SaveChangesAsync();

            return cilj;
        }

        public async Task<bool> PosodobiCilj(int ciljId, PrehranskiCiljiPodatki podatki)
        {
            var cilj = await _context.PrehranskiCilji.FindAsync(ciljId);
            if (cilj == null) return false;

            cilj.PosodobiCilj(podatki);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<PrehranskiCilji> PridobiCilj(int uporabnikId)
        {
            return await _context.PrehranskiCilji
                .FirstOrDefaultAsync(c => c.UporabnikId == uporabnikId);
        }

        public async Task<double> IzracunajNapredek(int ciljId)
        {
            var cilj = await _context.PrehranskiCilji.FindAsync(ciljId);
            if (cilj == null) return 0;

            return cilj.PreveriNapredek();
        }
    }
}