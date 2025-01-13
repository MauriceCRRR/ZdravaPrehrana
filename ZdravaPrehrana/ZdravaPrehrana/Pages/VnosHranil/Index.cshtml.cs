using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using ZdravaPrehrana.Controllers;
using ZdravaPrehrana.Entitete;

namespace ZdravaPrehrana.Pages.VnosHranil
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly UpravljalecHranil _upravljalecHranil;

        public IndexModel(UpravljalecHranil upravljalecHranil)
        {
            _upravljalecHranil = upravljalecHranil;
        }

        [BindProperty]
        public VnosHranilPodatki NoviVnos { get; set; }
        public ZdravaPrehrana.Entitete.VnosHranil DnevniVnos { get; set; }
        public VnosHranilStatistika Statistika { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var uporabnikId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Pridobi današnji vnos
            DnevniVnos = await _upravljalecHranil.PridobiDnevniVnos(DateTime.Today, uporabnikId);

            // Pridobi statistiko za zadnjih 30 dni
            Statistika = await _upravljalecHranil.PridobiStatistiko(
                uporabnikId,
                DateTime.Today.AddDays(-30),
                DateTime.Today);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
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
    }
}