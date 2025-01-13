using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using ZdravaPrehrana.Controllers;
using ZdravaPrehrana.Entitete;

namespace ZdravaPrehrana.Pages.Recepti
{
    [Authorize]
    public class UrediModel : PageModel
    {
        private readonly UpravljalecReceptov _upravljalecReceptov;
        private readonly ILogger<UrediModel> _logger;

        public UrediModel(UpravljalecReceptov upravljalecReceptov, ILogger<UrediModel> logger)
        {
            _upravljalecReceptov = upravljalecReceptov;
            _logger = logger;
        }

        [BindProperty]
        public Recept Recept { get; set; }

        [BindProperty]
        public List<ReceptSestavina> ReceptSestavine { get; set; }

        public bool JeAvtor { get; set; }

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

                JeAvtor = Recept.AvtorId == uporabnikId;
                if (!JeAvtor)
                {
                    return Unauthorized();
                }

                ReceptSestavine = Recept.ReceptSestavine.ToList();
                if (!ReceptSestavine.Any())
                {
                    ReceptSestavine.Add(new ReceptSestavina());
                }

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Napaka pri nalaganju recepta za urejanje {id}");
                return RedirectToPage("/Error");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Page();
                }

                var uporabnikId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                var obstojeciRecept = await _upravljalecReceptov.PridobiRecept(Recept.Id, uporabnikId);
                if (obstojeciRecept == null || obstojeciRecept.AvtorId != uporabnikId)
                {
                    return Unauthorized();
                }

                // Odstrani prazne sestavine
                ReceptSestavine = ReceptSestavine.Where(s => !string.IsNullOrWhiteSpace(s.Sestavina?.Naziv)).ToList();
                Recept.ReceptSestavine = ReceptSestavine;

                var uspeh = await _upravljalecReceptov.UrediRecept(Recept.Id, Recept, uporabnikId);
                if (uspeh)
                {
                    TempData["Sporocilo"] = "Recept je bil uspešno posodobljen.";
                    return RedirectToPage("./Podrobnosti", new { id = Recept.Id });
                }

                ModelState.AddModelError(string.Empty, "Napaka pri shranjevanju sprememb.");
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Napaka pri urejanju recepta {Recept?.Id}");
                ModelState.AddModelError(string.Empty, "Prišlo je do napake pri shranjevanju sprememb.");
                return Page();
            }
        }
    }
}