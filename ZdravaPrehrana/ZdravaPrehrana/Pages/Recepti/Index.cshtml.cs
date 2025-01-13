using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using ZdravaPrehrana.Controllers;
using ZdravaPrehrana.Entitete;

namespace ZdravaPrehrana.Pages.Recepti
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly UpravljalecReceptov _upravljalecReceptov;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(UpravljalecReceptov upravljalecReceptov, ILogger<IndexModel> logger)
        {
            _upravljalecReceptov = upravljalecReceptov;
            _logger = logger;
        }

        public List<Recept> Recepti { get; set; } = new List<Recept>();
        public string IskalniNiz { get; set; }

        public async Task<IActionResult> OnGetAsync(string iskanje)
        {
            IskalniNiz = iskanje;
            var uporabnikId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            try
            {
                if (string.IsNullOrWhiteSpace(iskanje))
                {
                    // Pridobi vse javne recepte in uporabnikove zasebne recepte
                    var javniRecepti = await _upravljalecReceptov.PridobiJavneRecepte();
                    var zasebniRecepti = await _upravljalecReceptov.PridobiRecepteUporabnika(uporabnikId);
                    Recepti = javniRecepti.Union(zasebniRecepti).ToList();
                }
                else
                {
                    Recepti = await _upravljalecReceptov.PoisciRecepte(iskanje, false);
                }

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Napaka pri nalaganju receptov");
                return RedirectToPage("/Error");
            }
        }

        public bool JeTrenutniUporabnik(int avtorId)
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)) == avtorId;
        }
    }
}