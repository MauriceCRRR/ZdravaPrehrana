using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using ZdravaPrehrana.Controllers;
using ZdravaPrehrana.Entitete;

namespace ZdravaPrehrana.Pages.Recepti
{
    [Authorize]
    public class PodrobnostiModel : PageModel
    {
        private readonly UpravljalecReceptov _upravljalecReceptov;
        private readonly ILogger<PodrobnostiModel> _logger;

        public PodrobnostiModel(UpravljalecReceptov upravljalecReceptov, ILogger<PodrobnostiModel> logger)
        {
            _upravljalecReceptov = upravljalecReceptov;
            _logger = logger;
        }

        public Recept Recept { get; set; }
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
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Napaka pri pridobivanju recepta {id}");
                return RedirectToPage("/Error");
            }
        }
    }
}