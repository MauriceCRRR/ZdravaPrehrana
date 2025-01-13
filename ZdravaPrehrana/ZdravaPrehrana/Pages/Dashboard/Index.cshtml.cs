using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ZdravaPrehrana.Entitete;
using ZdravaPrehrana.Services;

namespace ZdravaPrehrana.Pages
{
    [Authorize]
    public class DashboardModel : PageModel
    {
        private readonly IUporabnikService _uporabnikService;
        private readonly ILogger<DashboardModel> _logger;

        public DashboardModel(IUporabnikService uporabnikService, ILogger<DashboardModel> logger)
        {
            _uporabnikService = uporabnikService;
            _logger = logger;
        }

        public string UporabnikIme { get; set; }
        public UporabnikProfil UporabnikProfil { get; set; }
        public bool JeStrokovnjak { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var uporabnikId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(uporabnikId))
                {
                    return RedirectToPage("/Index");
                }

                UporabnikIme = User.Identity?.Name ?? "Uporabnik";
                JeStrokovnjak = User.IsInRole("Strokovnjak");

                UporabnikProfil = await _uporabnikService.PridobiProfil(int.Parse(uporabnikId));

                if (UporabnikProfil == null)
                {
                    var novProfil = new UporabnikProfil
                    {
                        UporabnikId = int.Parse(uporabnikId),
                        Visina = 0,
                        Teza = 0,
                        Alergije = new List<string>(),
                        Omejitve = new List<string>()
                    };
                    await _uporabnikService.UrediProfil(novProfil);
                    UporabnikProfil = novProfil;
                }

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Napaka pri nalaganju dashboarda");
                return RedirectToPage("/Error");
            }
        }
    }
}