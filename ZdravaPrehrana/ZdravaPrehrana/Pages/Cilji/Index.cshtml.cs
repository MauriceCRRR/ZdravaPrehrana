using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ZdravaPrehrana.Entitete;
using ZdravaPrehrana.Controllers;
using Microsoft.AspNetCore.Authorization;

namespace ZdravaPrehrana.Pages.Cilji
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly UpravljalecCiljev _upravljalecCiljev;

        public IndexModel(UpravljalecCiljev upravljalecCiljev)
        {
            _upravljalecCiljev = upravljalecCiljev;
        }

        public PrehranskiCilji TrenutniCilji { get; set; }

        [BindProperty]
        public PrehranskiCilji NoviCilji { get; set; }

        public double Napredek { get; set; }

        public string Opozorilo { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // TODO: Pridobi ID trenutnega uporabnika
            var uporabnikId = 1; // Zaèasno

            try
            {
                TrenutniCilji = await _upravljalecCiljev.PridobiCilj(uporabnikId);
                if (TrenutniCilji != null)
                {
                    var napredek = await _upravljalecCiljev.SpremljajNapredek(uporabnikId);
                    Napredek = napredek;
                }
            }
            catch (Exception ex)
            {
                Opozorilo = "Prišlo je do napake pri pridobivanju ciljev.";
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // TODO: Pridobi ID trenutnega uporabnika
            var uporabnikId = 1; // Zaèasno

            try
            {
                var podatki = new PrehranskiCiljiPodatki
                {
                    CiljnaTeza = NoviCilji.CiljnaTeza,
                    CasovniOkvir = NoviCilji.CasovniOkvir,
                    DnevneKalorije = NoviCilji.DnevneKalorije
                };

                var uspeh = await _upravljalecCiljev.NastaviCilje(podatki, uporabnikId);
                if (uspeh)
                {
                    return RedirectToPage("/Cilji/Index");
                }
                else
                {
                    Opozorilo = "Prišlo je do napake pri shranjevanju ciljev.";
                    return Page();
                }
            }
            catch (Exception ex)
            {
                Opozorilo = "Prišlo je do napake pri shranjevanju ciljev.";
                return Page();
            }
        }
    }
}