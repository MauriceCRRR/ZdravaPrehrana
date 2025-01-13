using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using ZdravaPrehrana.Controllers;
using ZdravaPrehrana.Entitete;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Claims;

namespace ZdravaPrehrana.Pages.Nasveti
{
    [Authorize(Roles = "Strokovnjak")]
    public class StrokovnjakModel : PageModel
    {
        private readonly UpravljalecNasvetov _upravljalecNasvetov;
        private readonly ILogger<StrokovnjakModel> _logger;

        public StrokovnjakModel(UpravljalecNasvetov upravljalecNasvetov, ILogger<StrokovnjakModel> logger)
        {
            _upravljalecNasvetov = upravljalecNasvetov;
            _logger = logger;
        }

        public List<Nasvet> NeodgovorjenaVprasanja { get; set; }

        [TempData]
        public string Sporocilo { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                NeodgovorjenaVprasanja = await _upravljalecNasvetov.PridobiNeodgovorjenaNasvetaVprasanja();
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Napaka pri pridobivanju neodgovorjenih vprašanj");
                ModelState.AddModelError(string.Empty, "Napaka pri nalaganju vprašanj.");
                return Page();
            }
        }

        public async Task<IActionResult> OnPostOdgovoriNaVprasanjeAsync(int nasvetId, string odgovor)
        {
            if (string.IsNullOrWhiteSpace(odgovor))
            {
                ModelState.AddModelError(string.Empty, "Odgovor ne more biti prazen.");
                return Page();
            }

            try
            {
                var strokovnjakId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var uspeh = await _upravljalecNasvetov.OdgovoriNaNasvet(nasvetId, odgovor, strokovnjakId);

                if (uspeh)
                {
                    Sporocilo = "Odgovor je bil uspešno poslan.";
                    return RedirectToPage();
                }

                ModelState.AddModelError(string.Empty, "Napaka pri shranjevanju odgovora.");
                NeodgovorjenaVprasanja = await _upravljalecNasvetov.PridobiNeodgovorjenaNasvetaVprasanja();
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Napaka pri odgovarjanju na vprašanje");
                ModelState.AddModelError(string.Empty, "Prišlo je do napake pri pošiljanju odgovora.");
                NeodgovorjenaVprasanja = await _upravljalecNasvetov.PridobiNeodgovorjenaNasvetaVprasanja();
                return Page();
            }
        }
    }
}