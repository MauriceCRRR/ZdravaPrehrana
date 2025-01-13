using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ZdravaPrehrana.Controllers;
using ZdravaPrehrana.Entitete;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;

namespace ZdravaPrehrana.Pages.Nasveti
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly UpravljalecNasvetov _upravljalecNasvetov;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(UpravljalecNasvetov upravljalecNasvetov, ILogger<IndexModel> logger)
        {
            _upravljalecNasvetov = upravljalecNasvetov;
            _logger = logger;
        }

        [BindProperty]
        [Required(ErrorMessage = "Vprašanje je obvezno")]
        public string NovoVprasanje { get; set; }

        public List<Nasvet> Nasveti { get; set; }

        [TempData]
        public string Sporocilo { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var uporabnikId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                Nasveti = await _upravljalecNasvetov.PridobiNasveteUporabnika(uporabnikId);
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Napaka pri pridobivanju nasvetov");
                return RedirectToPage("/Error");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                _logger.LogInformation("Zaèetek OnPost metode");

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Model state ni veljaven");
                    var uporabnikId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                    Nasveti = await _upravljalecNasvetov.PridobiNasveteUporabnika(uporabnikId);
                    return Page();
                }

                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                _logger.LogInformation($"Ustvarjanje vprašanja za uporabnika {userId}");

                await _upravljalecNasvetov.UstvariVprasanje(NovoVprasanje, userId);

                Sporocilo = "Vprašanje je bilo uspešno poslano.";
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Napaka pri pošiljanju vprašanja");
                ModelState.AddModelError(string.Empty, "Prišlo je do napake pri pošiljanju vprašanja.");
                var uporabnikId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                Nasveti = await _upravljalecNasvetov.PridobiNasveteUporabnika(uporabnikId);
                return Page();
            }
        }
    }
}