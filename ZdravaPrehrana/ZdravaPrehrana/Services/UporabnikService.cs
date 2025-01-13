using Microsoft.EntityFrameworkCore;
using ZdravaPrehrana.Data;
using ZdravaPrehrana.Entitete;

public interface IUporabnikService
{
    Task<Uporabnik> PreveriPrijavo(string uporabniskoIme, string geslo);
    Task<UporabnikProfil> PridobiProfil(int uporabnikId);
    Task<bool> UrediProfil(UporabnikProfil profil);
    Task<(bool Success, string Error)> Registracija(string uporabniskoIme, string email, string geslo, UporabniskaVloga vloga);
}

public class UporabnikService : IUporabnikService
{
    private readonly ApplicationDbContext _context;

    public UporabnikService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Uporabnik> PreveriPrijavo(string uporabniskoIme, string geslo)
    {
        // V produkciji bi uporabljali ustrezno zgoščevanje gesel
        return await _context.Uporabniki
            .Include(u => u.Profil)
            .FirstOrDefaultAsync(u => u.UporabniskoIme == uporabniskoIme && u.Geslo == geslo);
    }

    public async Task<UporabnikProfil> PridobiProfil(int uporabnikId)
    {
        return await _context.Profili
            .FirstOrDefaultAsync(p => p.UporabnikId == uporabnikId);
    }

    public async Task<bool> UrediProfil(UporabnikProfil profil)
    {
        try
        {
            _context.Profili.Update(profil);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<(bool Success, string Error)> Registracija(string uporabniskoIme, string email, string geslo, UporabniskaVloga vloga)
    {
        // Preveri če uporabnik že obstaja
        var obstojeci = await _context.Uporabniki
            .FirstOrDefaultAsync(u => u.UporabniskoIme == uporabniskoIme || u.Email == email);

        if (obstojeci != null)
        {
            if (obstojeci.UporabniskoIme == uporabniskoIme)
                return (false, "Uporabniško ime že obstaja");
            return (false, "Email naslov že obstaja");
        }

        // Ustvari novega uporabnika
        var uporabnik = new Uporabnik
        {
            UporabniskoIme = uporabniskoIme,
            Email = email,
            Geslo = geslo, // V produkciji uporabite zgoščevanje gesel!
            Vloga = vloga
        };

        // Ustvari še profil za uporabnika
        var profil = new UporabnikProfil
        {
            Uporabnik = uporabnik,
            Visina = 0,
            Teza = 0,
            Alergije = new List<string>(),
            Omejitve = new List<string>()
        };

        try
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    await _context.Uporabniki.AddAsync(uporabnik);
                    await _context.SaveChangesAsync();

                    await _context.Profili.AddAsync(profil);
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();
                    return (true, string.Empty);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return (false, $"Napaka pri shranjevanju: {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            return (false, $"Prišlo je do napake pri registraciji: {ex.Message}");
        }
    }

    // Pomožna metoda za preverjanje vloge uporabnika
    public async Task<bool> JeStrokovnjak(int uporabnikId)
    {
        var uporabnik = await _context.Uporabniki
            .FirstOrDefaultAsync(u => u.Id == uporabnikId);
        return uporabnik?.Vloga == UporabniskaVloga.Strokovnjak;
    }
}