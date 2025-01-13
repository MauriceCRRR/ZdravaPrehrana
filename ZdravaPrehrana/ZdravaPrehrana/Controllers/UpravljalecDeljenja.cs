using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using ZdravaPrehrana.Data;
using ZdravaPrehrana.Entitete;

namespace ZdravaPrehrana.Controllers
{
    public class UpravljalecDeljenja
    {
        private readonly ApplicationDbContext _context;

        public UpravljalecDeljenja(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> DeliVsebino(object vsebina, int prejemnikId)
        {
            try
            {
                var prejemnik = await _context.Uporabniki
                    .FirstOrDefaultAsync(u => u.Id == prejemnikId);

                if (prejemnik == null)
                    return false;

                // Preveri tip vsebine in ustrezno deli
                if (vsebina is Jedilnik jedilnik)
                {
                    return await DeliJedilnik(jedilnik, prejemnik);
                }
                else if (vsebina is Recept recept)
                {
                    return await DeliRecept(recept, prejemnik);
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        private async Task<bool> DeliJedilnik(Jedilnik jedilnik, Uporabnik prejemnik)
        {
            try
            {
                jedilnik.DeliZ.Add(prejemnik);
                await _context.SaveChangesAsync();
                await PosljiObvestiloODeljenju(prejemnik.Email, "jedilnik", jedilnik.Naziv);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private async Task<bool> DeliRecept(Recept recept, Uporabnik prejemnik)
        {
            try
            {
                // Implementacija deljenja recepta
                // TODO: Dodajte ustrezno logiko za deljenje receptov
                await PosljiObvestiloODeljenju(prejemnik.Email, "recept", recept.Naziv);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> PreveriDostop(int uporabnikId, int vsebinaId)
        {
            try
            {
                var uporabnik = await _context.Uporabniki
                    .Include(u => u.Jedilniki)
                    .FirstOrDefaultAsync(u => u.Id == uporabnikId);

                if (uporabnik == null)
                    return false;

                // Preveri èe ima uporabnik dostop do vsebine
                var imaDovoljenjeJedilnik = await _context.Jedilniki
                    .AnyAsync(j => j.Id == vsebinaId &&
                                 (j.UporabnikId == uporabnikId ||
                                  j.DeliZ.Any(u => u.Id == uporabnikId)));

                return imaDovoljenjeJedilnik;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> PosljiPovabilo(string email)
        {
            try
            {
                if (!IsValidEmail(email))
                    return false;

                await PosljiEmailPovabilo(email);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private async Task<bool> PosljiEmailPovabilo(string email)
        {
            try
            {
                // TODO: Implementirajte pošiljanje email-a
                // V produkciji bi uporabili pravi email servis
                return true;
            }
            catch
            {
                return false;
            }
        }

        private async Task<bool> PosljiObvestiloODeljenju(string email, string tipVsebine, string nazivVsebine)
        {
            try
            {
                // TODO: Implementirajte pošiljanje obvestila
                // V produkciji bi uporabili pravi email servis
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}