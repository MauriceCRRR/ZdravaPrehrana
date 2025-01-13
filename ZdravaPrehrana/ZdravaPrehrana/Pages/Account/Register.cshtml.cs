using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using ZdravaPrehrana.Models;
using ZdravaPrehrana.Entitete;

namespace ZdravaPrehrana.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly IUporabnikService _uporabnikService;
        private const string STROKOVNJAK_KODA = "StrokovnjakSemZdaj";

        public RegisterModel(IUporabnikService uporabnikService)
        {
            _uporabnikService = uporabnikService;
        }

        [BindProperty]
        public RegisterInput RegisterInput { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            // Preveri, èe je vnesena koda za strokovnjaka
            var vloga = UporabniskaVloga.Uporabnik;
            if (!string.IsNullOrEmpty(RegisterInput.StrokovnjakKoda))
            {
                if (RegisterInput.StrokovnjakKoda == STROKOVNJAK_KODA)
                {
                    vloga = UporabniskaVloga.Strokovnjak;
                }
                else
                {
                    ModelState.AddModelError("RegisterInput.StrokovnjakKoda", "Neveljavna koda za strokovnjaka");
                    return Page();
                }
            }

            var result = await _uporabnikService.Registracija(
                RegisterInput.UporabniskoIme,
                RegisterInput.Email,
                RegisterInput.Geslo,
                vloga);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Error);
                return Page();
            }

            return RedirectToPage("/Index", new { registered = true });
        }
    }
}