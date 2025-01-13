using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using ZdravaPrehrana.Entitete;
using ZdravaPrehrana.Controllers;
using System.ComponentModel.DataAnnotations; // Dodaj to za Required in Range atribute
using Microsoft.AspNetCore.Authorization;

namespace ZdravaPrehrana.Pages.NakupovalniSeznami
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly UpravljalecNakupovanja _upravljalecNakupovanja;

        public IndexModel(UpravljalecNakupovanja upravljalecNakupovanja)
        {
            _upravljalecNakupovanja = upravljalecNakupovanja;
        }

        public NakupovalniSeznam TrenutniSeznam { get; set; }
        public List<NakupovalniSeznam> VsiSeznami { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Naziv seznama je obvezen")]
        public string NoviSeznamNaziv { get; set; }

        [BindProperty]
        public NoviIzdelekModel NoviIzdelek { get; set; }
        public async Task<IActionResult> OnGetAsync(int? seznamId)
        {
            var uporabnikId = 1; // Zaèasno
            VsiSeznami = await _upravljalecNakupovanja.PridobiSezname(uporabnikId);

            if (seznamId.HasValue)
            {
                TrenutniSeznam = await _upravljalecNakupovanja.PridobiSeznam(seznamId.Value);
            }
            else if (VsiSeznami.Any())
            {
                TrenutniSeznam = VsiSeznami.First();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostUstvariSeznamAsync()
        {
            if (string.IsNullOrWhiteSpace(NoviSeznamNaziv))
                return Page();

            var uporabnikId = 1; // Zaèasno
            var noviSeznam = await _upravljalecNakupovanja.UstvariSeznam(NoviSeznamNaziv, uporabnikId);

            if (noviSeznam != null)
            {
                return RedirectToPage(new { seznamId = noviSeznam.Id });
            }

            return Page();
        }

        public async Task<IActionResult> OnPostDodajIzdelekAsync(int seznamId)
        {
            // Odstranimo validacijo za NoviSeznamNaziv pri dodajanju izdelka
            ModelState.Remove("NoviSeznamNaziv");

            if (!ModelState.IsValid)
            {
                VsiSeznami = await _upravljalecNakupovanja.PridobiSezname(1);
                TrenutniSeznam = await _upravljalecNakupovanja.PridobiSeznam(seznamId);
                return Page();
            }

            var uspeh = await _upravljalecNakupovanja.DodajIzdelek(
                NoviIzdelek.Naziv,
                NoviIzdelek.Kolicina,
                NoviIzdelek.Enota,
                seznamId);

            if (!uspeh)
            {
                ModelState.AddModelError(string.Empty, "Napaka pri dodajanju izdelka.");
                VsiSeznami = await _upravljalecNakupovanja.PridobiSezname(1);
                TrenutniSeznam = await _upravljalecNakupovanja.PridobiSeznam(seznamId);
                return Page();
            }

            return RedirectToPage(new { seznamId });
        }

        public async Task<IActionResult> OnPostOdstraniIzdelekAsync(int seznamId, string naziv)
        {
            await _upravljalecNakupovanja.OdstraniIzdelek(seznamId, naziv);
            return RedirectToPage(new { seznamId });
        }

        public async Task<IActionResult> OnPostOznaciIzdelekAsync(int seznamId, string naziv)
        {
            await _upravljalecNakupovanja.OznaciIzdelek(seznamId, naziv);
            return RedirectToPage(new { seznamId });
        }

        public class NoviIzdelekModel
        {
            [Required(ErrorMessage = "Naziv izdelka je obvezen")]
            public string Naziv { get; set; }

            [Required(ErrorMessage = "Kolièina je obvezna")]
            [Range(0.01, double.MaxValue, ErrorMessage = "Kolièina mora biti veèja od 0")]
            public double Kolicina { get; set; }

            [Required(ErrorMessage = "Enota je obvezna")]
            public string Enota { get; set; }
        }
    }
}