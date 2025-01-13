using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using ZdravaPrehrana.Controllers;
using ZdravaPrehrana.Entitete;
using Microsoft.EntityFrameworkCore;

namespace ZdravaPrehrana.Pages.Jedilnik
{
    [Authorize]
    public class DeljeniJedilnikiModel : PageModel
    {
        private readonly UpravljalecJedilnika _upravljalecJedilnika;
        private readonly UpravljalecOcen _upravljalecOcen;
        private readonly ILogger<DeljeniJedilnikiModel> _logger;

        public DeljeniJedilnikiModel(
            UpravljalecJedilnika upravljalecJedilnika,
            UpravljalecOcen upravljalecOcen,
            ILogger<DeljeniJedilnikiModel> logger)
        {
            _upravljalecJedilnika = upravljalecJedilnika;
            _upravljalecOcen = upravljalecOcen;
            _logger = logger;
        }

        public List<ZdravaPrehrana.Entitete.Jedilnik> DeljeniJedilniki { get; set; }
        public string Sporocilo { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                DeljeniJedilniki = await _upravljalecJedilnika.PridobiDeljeneJedilnike();
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Napaka pri nalaganju deljenih jedilnikov");
                return RedirectToPage("/Error");
            }
        }

        public async Task<IActionResult> OnPostOceniJedilnikAsync(int jedilnikId, int ocenaVrednost, string ocenaKomentar)
        {
            try
            {
                var uporabnikId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                var uspeh = await _upravljalecOcen.DodajOceno(jedilnikId, uporabnikId, ocenaVrednost, ocenaKomentar);
                if (!uspeh)
                {
                    Sporocilo = "Napaka pri oddaji ocene.";
                }

                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Napaka pri ocenjevanju jedilnika");
                return RedirectToPage("/Error");
            }
        }

        public async Task<IActionResult> OnPostShraniJedilnikAsync(int jedilnikId)
        {
            try
            {   
                var uporabnikId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                var uspeh = await _upravljalecJedilnika.ShraniKopijo(jedilnikId, uporabnikId);
                if (!uspeh)
                {
                    Sporocilo = "Napaka pri shranjevanju kopije jedilnika.";
                    return RedirectToPage();
                }

                return RedirectToPage("/Jedilnik/Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Napaka pri shranjevanju kopije jedilnika");
                return RedirectToPage("/Error");
            }
        }
    }
}