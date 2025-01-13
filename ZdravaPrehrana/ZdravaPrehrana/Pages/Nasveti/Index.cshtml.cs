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
        [Required(ErrorMessage = "Vpra�anje je obvezno")]
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
                _logger.LogInformation("Za�etek OnPost metode");

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Model state ni veljaven");
                    var uporabnikId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                    Nasveti = await _upravljalecNasvetov.PridobiNasveteUporabnika(uporabnikId);
                    return Page();
                }

                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                _logger.LogInformation($"Ustvarjanje vpra�anja za uporabnika {userId}");

                await _upravljalecNasvetov.UstvariVprasanje(NovoVprasanje, userId);

                Sporocilo = "Vpra�anje je bilo uspe�no poslano.";
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Napaka pri po�iljanju vpra�anja");
                ModelState.AddModelError(string.Empty, "Pri�lo je do napake pri po�iljanju vpra�anja.");
                var uporabnikId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                Nasveti = await _upravljalecNasvetov.PridobiNasveteUporabnika(uporabnikId);
                return Page();
            }
        }
    }
}