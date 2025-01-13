using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using ZdravaPrehrana.Controllers;
using ZdravaPrehrana.Entitete;
using ZdravaPrehrana.Services;

namespace ZdravaPrehrana.Pages.VnosHranil
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IUpravljalecCiljevService _upravljalecCiljev;
        private readonly UpravljalecHranil _upravljalecHranil;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(
            IUpravljalecCiljevService upravljalecCiljev,
            UpravljalecHranil upravljalecHranil,
            ILogger<IndexModel> logger)
        {
            _upravljalecCiljev = upravljalecCiljev;
            _upravljalecHranil = upravljalecHranil;
            _logger = logger;
        }

        [BindProperty]
        public VnosHranilPodatki NoviVnos { get; set; }
        public ZdravaPrehrana.Entitete.VnosHranil DnevniVnos { get; set; }
        public VnosHranilStatistika Statistika { get; set; }
        public PrehranskiCilji TrenutniCilji { get; set; }
        public int PreostalKalorije { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var uporabnikId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                // Pridobi današnji vnos
                DnevniVnos = await _upravljalecHranil.PridobiDnevniVnos(DateTime.Today, uporabnikId);

                // Pridobi statistiko za zadnjih 30 dni
                Statistika = await _upravljalecHranil.PridobiStatistiko(
                    uporabnikId,
                    DateTime.Today.AddDays(-30),
                    DateTime.Today);

                // Pridobi prehranske cilje
                TrenutniCilji = await _upravljalecCiljev.PridobiCilj(uporabnikId);

                // Izraèunaj preostale kalorije za danes
                if (TrenutniCilji != null && DnevniVnos != null)
                {
                    PreostalKalorije = TrenutniCilji.DnevneKalorije - DnevniVnos.Kalorije;
                }
                else if (TrenutniCilji != null)
                {
                    PreostalKalorije = TrenutniCilji.DnevneKalorije;
                }

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Napaka pri nalaganju strani za vnos hranil");
                return RedirectToPage("/Error");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    // Ponovno naloži podatke v primeru napake
                    var uporabnikId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                    DnevniVnos = await _upravljalecHranil.PridobiDnevniVnos(DateTime.Today, uporabnikId);
                    Statistika = await _upravljalecHranil.PridobiStatistiko(
                        uporabnikId,
                        DateTime.Today.AddDays(-30),
                        DateTime.Today);
                    TrenutniCilji = await _upravljalecCiljev.PridobiCilj(uporabnikId);
                    return Page();
                }

                NoviVnos.UporabnikId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var uspeh = await _upravljalecHranil.DodajVnosHranil(NoviVnos);
                if (uspeh)
                {
                    return RedirectToPage();
                }

                ModelState.AddModelError(string.Empty, "Napaka pri dodajanju vnosa.");
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Napaka pri dodajanju vnosa hranil");
                return RedirectToPage("/Error");
            }
        }
    }
}