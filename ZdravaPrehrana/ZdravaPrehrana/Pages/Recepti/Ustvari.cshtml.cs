using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using ZdravaPrehrana.Controllers;
using ZdravaPrehrana.Entitete;

namespace ZdravaPrehrana.Pages.Recepti
{
    [Authorize]
    public class UstvariModel : PageModel
    {
        private readonly UpravljalecReceptov _upravljalecReceptov;
        private readonly ILogger<UstvariModel> _logger;

        public UstvariModel(UpravljalecReceptov upravljalecReceptov, ILogger<UstvariModel> logger)
        {
            _upravljalecReceptov = upravljalecReceptov;
            _logger = logger;
        }

        [BindProperty]
        public Recept Recept { get; set; } = new Recept();

        [BindProperty]
        public List<ReceptSestavina> ReceptSestavine { get; set; } = new List<ReceptSestavina>
    {
        new ReceptSestavina { Sestavina = new Sestavina() }
    };

        public async Task<IActionResult> OnPostAsync()
        {
            _logger.LogInformation("Zaèetek OnPostAsync");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Model ni veljaven. Napake: {Errors}",
                    string.Join(", ", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)));
                return Page();
            }

            try
            {
                var uporabnikId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                _logger.LogInformation("UporabnikId: {UporabnikId}", uporabnikId);

                // Odstrani prazne sestavine in preveri podatke
                ReceptSestavine = ReceptSestavine
                    .Where(s => !string.IsNullOrWhiteSpace(s.Sestavina?.Naziv))
                    .ToList();

                _logger.LogInformation("Število veljavnih sestavin: {StSestavin}", ReceptSestavine.Count);
                foreach (var rs in ReceptSestavine)
                {
                    _logger.LogInformation("Sestavina: {Naziv}, Kolièina: {Kolicina}, Enota: {Enota}",
                        rs.Sestavina?.Naziv, rs.Kolicina, rs.Enota);
                }

                // Nastavi vrednosti za sestavine
                foreach (var rs in ReceptSestavine)
                {
                    if (rs.Sestavina != null)
                    {
                        rs.Sestavina.Kalorije = 0;
                        rs.Sestavina.Beljakovine = 0;
                        rs.Sestavina.Mascobe = 0;
                        rs.Sestavina.OgljikoviHidrati = 0;
                    }
                }

                Recept.ReceptSestavine = ReceptSestavine;
                _logger.LogInformation("Podatki recepta: Naziv: {Naziv}, CasPriprave: {CasPriprave}, Kalorije: {Kalorije}",
                    Recept.Naziv, Recept.CasPriprave, Recept.Kalorije);

                var uspeh = await _upravljalecReceptov.DodajRecept(Recept, uporabnikId);
                _logger.LogInformation("Rezultat shranjevanja: {Uspeh}", uspeh);

                if (uspeh)
                {
                    TempData["Sporocilo"] = "Recept je bil uspešno ustvarjen.";
                    return RedirectToPage("./Index");
                }

                _logger.LogWarning("Neuspeh pri shranjevanju recepta");
                ModelState.AddModelError(string.Empty, "Prišlo je do napake pri shranjevanju recepta.");
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Napaka pri ustvarjanju recepta");
                ModelState.AddModelError(string.Empty, "Prišlo je do napake pri shranjevanju recepta.");
                return Page();
            }
        }
    }
}