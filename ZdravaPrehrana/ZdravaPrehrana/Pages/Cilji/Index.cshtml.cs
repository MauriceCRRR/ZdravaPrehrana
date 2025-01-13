using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ZdravaPrehrana.Entitete;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using ZdravaPrehrana.Services;

namespace ZdravaPrehrana.Pages.Cilji
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IUpravljalecCiljevService _upravljalecCiljev;
        private readonly IUporabnikService _uporabnikService;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(
            IUpravljalecCiljevService upravljalecCiljev,
            IUporabnikService uporabnikService,
            ILogger<IndexModel> logger)
        {
            _upravljalecCiljev = upravljalecCiljev ?? throw new ArgumentNullException(nameof(upravljalecCiljev));
            _uporabnikService = uporabnikService ?? throw new ArgumentNullException(nameof(uporabnikService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public PrehranskiCilji TrenutniCilji { get; set; }
        public UporabnikProfil UporabnikProfil { get; set; }
        public double Napredek { get; set; }
        public string Opozorilo { get; set; }
        public string UspehSporocilo { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Ciljna teža je obvezna")]
            [Range(30, 300, ErrorMessage = "Ciljna teža mora biti med 30 in 300 kg")]
            public double CiljnaTeza { get; set; }

            [Required(ErrorMessage = "Želena tedenska sprememba je obvezna")]
            [Range(0.1, 1.0, ErrorMessage = "Tedenska sprememba mora biti med 0.1 in 1.0 kg")]
            public double TedenIzgubaKg { get; set; }

            [Required(ErrorMessage = "Nivo aktivnosti je obvezen")]
            public string NivoAktivnosti { get; set; }

            public bool JeHujsanje { get; set; }
            public double UporabnikovaTrenutnaTeza { get; set; }
        }

        private async Task LoadData(int uporabnikId)
        {
            UporabnikProfil = await _uporabnikService.PridobiProfil(uporabnikId);
            TrenutniCilji = await _upravljalecCiljev.PridobiCilj(uporabnikId);
            if (TrenutniCilji != null)
            {
                Napredek = await _upravljalecCiljev.IzracunajNapredek(uporabnikId);
            }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                _logger.LogInformation("Zaèetek OnGetAsync");
                var uporabnikId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                await LoadData(uporabnikId);

                if (UporabnikProfil == null)
                {
                    _logger.LogWarning("Uporabnik {UporabnikId} nima profila", uporabnikId);
                    return RedirectToPage("/Profil/Uredi", new { opozorilo = "Prosim, najprej izpolnite svoj profil." });
                }

                if (TrenutniCilji != null)
                {
                    _logger.LogInformation("Nalagam obstojeèe cilje za uporabnika {UporabnikId}", uporabnikId);
                    Input = new InputModel
                    {
                        CiljnaTeza = TrenutniCilji.CiljnaTeza,
                        TedenIzgubaKg = TrenutniCilji.TedenIzgubaKg,
                        NivoAktivnosti = "moderate",
                        UporabnikovaTrenutnaTeza = UporabnikProfil.Teza
                    };
                }
                else
                {
                    _logger.LogInformation("Nastavljam privzete vrednosti za novega uporabnika {UporabnikId}", uporabnikId);
                    Input = new InputModel
                    {
                        UporabnikovaTrenutnaTeza = UporabnikProfil.Teza,
                        TedenIzgubaKg = 0.5,
                        NivoAktivnosti = "moderate"
                    };
                }

                if (TempData["UspehSporocilo"] != null)
                {
                    UspehSporocilo = TempData["UspehSporocilo"].ToString();
                    _logger.LogInformation("Prikazujem sporoèilo o uspehu: {UspehSporocilo}", UspehSporocilo);
                }

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Napaka pri nalaganju ciljev");
                Opozorilo = "Prišlo je do napake pri nalaganju ciljev.";
                return Page();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                _logger.LogInformation("Zaèetek OnPostAsync");

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Model ni veljaven: {@ModelState}", ModelState);
                    return Page();
                }

                var uporabnikId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                await LoadData(uporabnikId);

                if (UporabnikProfil == null)
                {
                    _logger.LogWarning("Uporabnik {UporabnikId} nima profila", uporabnikId);
                    return RedirectToPage("/Profil/Uredi", new { opozorilo = "Prosim, najprej izpolnite svoj profil." });
                }

                Input.UporabnikovaTrenutnaTeza = UporabnikProfil.Teza;
                Input.JeHujsanje = Input.CiljnaTeza < UporabnikProfil.Teza;

                var razlikaVTezi = Math.Abs(UporabnikProfil.Teza - Input.CiljnaTeza);
                if (razlikaVTezi > UporabnikProfil.Teza * 0.3)
                {
                    _logger.LogWarning("Prevelika razlika v teži za uporabnika {UporabnikId}", uporabnikId);
                    ModelState.AddModelError(nameof(Input.CiljnaTeza),
                        "Ciljna teža se preveè razlikuje od trenutne teže. Postavite si manjši cilj.");
                    Opozorilo = $"Prevelika razlika v teži. Maksimalna dovoljena sprememba je {(UporabnikProfil.Teza * 0.3):F1} kg.";
                    await LoadData(uporabnikId);
                    return Page();
                }

                if (Input.JeHujsanje && Input.TedenIzgubaKg > 1.0)
                {
                    _logger.LogWarning("Prehitra izguba teže za uporabnika {UporabnikId}", uporabnikId);
                    ModelState.AddModelError(nameof(Input.TedenIzgubaKg),
                        "Tedenska izguba ne sme presegati 1 kg za zdravo hujšanje.");
                    return Page();
                }

                double aktivnostMultiplikator = Input.NivoAktivnosti switch
                {
                    "sedentary" => 1.2,
                    "light" => 1.375,
                    "moderate" => 1.55,
                    "active" => 1.725,
                    "very_active" => 1.9,
                    _ => 1.2
                };

                _logger.LogInformation("Izraèunavam cilje za uporabnika {UporabnikId} z aktivnostjo {Aktivnost}",
                    uporabnikId, aktivnostMultiplikator);

                var cilji = new PrehranskiCilji
                {
                    UporabnikId = uporabnikId,
                    CiljnaTeza = Input.CiljnaTeza,
                    TedenIzgubaKg = Input.TedenIzgubaKg,
                    CasovniOkvir = (int)(razlikaVTezi / Input.TedenIzgubaKg * 7)
                };

                cilji.IzracunajKalorije(
                    UporabnikProfil.Visina,
                    UporabnikProfil.Teza,
                    aktivnost: aktivnostMultiplikator
                );

                if (cilji.DnevneKalorije < 1200)
                {
                    _logger.LogWarning("Prenizke dnevne kalorije ({Kalorije}) za uporabnika {UporabnikId}",
                        cilji.DnevneKalorije, uporabnikId);
                    ModelState.AddModelError(string.Empty,
                        "Izraèunane dnevne kalorije so prenizke za zdravo hujšanje. Prilagodite cilje.");
                    return Page();
                }

                var podatki = new PrehranskiCiljiPodatki
                {
                    CiljnaTeza = cilji.CiljnaTeza,
                    TedenIzgubaKg = cilji.TedenIzgubaKg,
                    CasovniOkvir = cilji.CasovniOkvir,
                    DnevneKalorije = cilji.DnevneKalorije,
                    BMR = cilji.BMR,
                    TDEE = cilji.TDEE
                };

                _logger.LogInformation("Shranjujem cilje za uporabnika {UporabnikId}: {@Podatki}",
                    uporabnikId, podatki);

                if (TrenutniCilji != null)
                {
                    var uspeh = await _upravljalecCiljev.PosodobiCilj(uporabnikId, podatki);
                    if (!uspeh)
                    {
                        _logger.LogError("Napaka pri posodabljanju ciljev za uporabnika {UporabnikId}", uporabnikId);
                        Opozorilo = "Napaka pri posodabljanju ciljev.";
                        return Page();
                    }
                }
                else
                {
                    var rezultat = await _upravljalecCiljev.UstvariCilj(uporabnikId, podatki);
                    if (rezultat == null)
                    {
                        _logger.LogError("Napaka pri ustvarjanju ciljev za uporabnika {UporabnikId}", uporabnikId);
                        Opozorilo = "Napaka pri ustvarjanju ciljev.";
                        return Page();
                    }
                }

                _logger.LogInformation("Cilji uspešno shranjeni za uporabnika {UporabnikId}", uporabnikId);
                TempData["UspehSporocilo"] = "Cilji so bili uspešno shranjeni.";
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Neprièakovana napaka pri shranjevanju ciljev");
                Opozorilo = "Prišlo je do neprièakovane napake.";
                return Page();
            }
        }
    }
}