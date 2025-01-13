using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using ZdravaPrehrana.Controllers;
using ZdravaPrehrana.Entitete;

namespace ZdravaPrehrana.Pages.Recepti
{
    [Authorize]
    public class IzbrisiModel : PageModel
    {
        private readonly UpravljalecReceptov _upravljalecReceptov;
        private readonly ILogger<IzbrisiModel> _logger;

        public IzbrisiModel(UpravljalecReceptov upravljalecReceptov, ILogger<IzbrisiModel> logger)
        {
            _upravljalecReceptov = upravljalecReceptov;
            _logger = logger;
        }

        [BindProperty]
        public Recept Recept { get; set; }
        public bool JePravilenLastnik { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                var uporabnikId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                Recept = await _upravljalecReceptov.PridobiRecept(id, uporabnikId);

                if (Recept == null)
                {
                    return NotFound();
                }

                JePravilenLastnik = Recept.AvtorId == uporabnikId;
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Napaka pri pridobivanju recepta za brisanje {id}");
                return RedirectToPage("/Error");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                var uporabnikId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var uspeh = await _upravljalecReceptov.IzbrisiRecept(Recept.Id, uporabnikId);

                if (uspeh)
                {
                    TempData["Sporocilo"] = "Recept je bil uspešno izbrisan.";
                    return RedirectToPage("./Index");
                }

                ModelState.AddModelError(string.Empty, "Napaka pri brisanju recepta.");
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Napaka pri brisanju recepta");
                ModelState.AddModelError(string.Empty, "Prišlo je do napake pri brisanju recepta.");
                return Page();
            }
        }
    }
}