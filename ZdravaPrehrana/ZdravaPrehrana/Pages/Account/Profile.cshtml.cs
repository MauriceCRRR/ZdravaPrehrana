using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;
using ZdravaPrehrana.Services;
using ZdravaPrehrana.Entitete;

namespace ZdravaPrehrana.Pages.Account
{
    [Authorize]
    public class ProfileModel : PageModel
    {
        private readonly IUporabnikService _uporabnikService;
        private readonly ILogger<ProfileModel> _logger;

        public ProfileModel(IUporabnikService uporabnikService, ILogger<ProfileModel> logger)
        {
            _uporabnikService = uporabnikService;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string StatusMessage { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Višina je obvezna")]
            [Range(0, 300, ErrorMessage = "Višina mora biti med 0 in 300 cm")]
            [Display(Name = "Višina (cm)")]
            public double Visina { get; set; }

            [Required(ErrorMessage = "Teža je obvezna")]
            [Range(0, 500, ErrorMessage = "Teža mora biti med 0 in 500 kg")]
            [Display(Name = "Teža (kg)")]
            public double Teza { get; set; }

            [Display(Name = "Alergije")]
            public List<string> Alergije { get; set; } = new();

            [Display(Name = "Prehranske omejitve")]
            public List<string> Omejitve { get; set; } = new();
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var uporabnikId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var profil = await _uporabnikService.PridobiProfil(uporabnikId);

            if (profil == null)
            {
                return NotFound();
            }

            Input = new InputModel
            {
                Visina = profil.Visina,
                Teza = profil.Teza,
                Alergije = profil.Alergije,
                Omejitve = profil.Omejitve
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string novaAlergija = null, string novaOmejitev = null)
        {
            var uporabnikId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var profil = await _uporabnikService.PridobiProfil(uporabnikId);

            // Ohrani obstojeèe vrednosti
            if (Input.Alergije == null) Input.Alergije = profil.Alergije;
            if (Input.Omejitve == null) Input.Omejitve = profil.Omejitve;

            // Dodaj novo alergijo
            if (!string.IsNullOrWhiteSpace(novaAlergija))
            {
                if (!Input.Alergije.Contains(novaAlergija))
                {
                    Input.Alergije.Add(novaAlergija);
                }
                return Page();
            }

            // Dodaj novo omejitev
            if (!string.IsNullOrWhiteSpace(novaOmejitev))
            {
                if (!Input.Omejitve.Contains(novaOmejitev))
                {
                    Input.Omejitve.Add(novaOmejitev);
                }
                return Page();
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                profil.Visina = Input.Visina;
                profil.Teza = Input.Teza;
                profil.Alergije = Input.Alergije;
                profil.Omejitve = Input.Omejitve;

                await _uporabnikService.UrediProfil(profil);
                StatusMessage = "Profil uspešno posodobljen.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Napaka pri urejanju profila za uporabnika {UporabnikId}", uporabnikId);
                StatusMessage = "Napaka pri shranjevanju profila.";
            }

            return Page();
        }

        public async Task<IActionResult> OnPostOdstraniAlergijoAsync(string alergija)
        {
            var uporabnikId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var profil = await _uporabnikService.PridobiProfil(uporabnikId);

            if (Input.Alergije == null) Input.Alergije = profil.Alergije;
            if (Input.Omejitve == null) Input.Omejitve = profil.Omejitve;

            Input.Alergije.Remove(alergija);
            return Page();
        }

        public async Task<IActionResult> OnPostOdstraniOmejitevAsync(string omejitev)
        {
            var uporabnikId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var profil = await _uporabnikService.PridobiProfil(uporabnikId);

            if (Input.Alergije == null) Input.Alergije = profil.Alergije;
            if (Input.Omejitve == null) Input.Omejitve = profil.Omejitve;

            Input.Omejitve.Remove(omejitev);
            return Page();
        }
    }
}