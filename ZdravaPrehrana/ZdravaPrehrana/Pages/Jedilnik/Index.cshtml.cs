using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using ZdravaPrehrana.Entitete;
using ZdravaPrehrana.Controllers;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;

namespace ZdravaPrehrana.Pages.Jedilnik
{
    [Authorize]
    public class NoviObrokModel
    {
        [Required(ErrorMessage = "Naziv obroka je obvezen")]
        public string Naziv { get; set; }

        [Required(ErrorMessage = "Èas obroka je obvezen")]
        public DateTime Cas { get; set; }
    }

    public class IndexModel : PageModel
    {
        private readonly UpravljalecJedilnika _upravljalecJedilnika;
        private readonly UpravljalecReceptov _upravljalecReceptov;
        private readonly UpravljalecOcen _upravljalecOcen;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(
            UpravljalecJedilnika upravljalecJedilnika,
            UpravljalecReceptov upravljalecReceptov,
            UpravljalecOcen upravljalecOcen,
            ILogger<IndexModel> logger)
        {
            _upravljalecJedilnika = upravljalecJedilnika;
            _upravljalecReceptov = upravljalecReceptov;
            _upravljalecOcen = upravljalecOcen;
            _logger = logger;
        }

        public ZdravaPrehrana.Entitete.Jedilnik TrenutniJedilnik { get; set; }
        public List<ZdravaPrehrana.Entitete.Jedilnik> MojiJedilniki { get; set; }
        public List<Recept> VsiRecepti { get; set; }

        [BindProperty]
        public string NoviJedilnikNaziv { get; set; }

        [BindProperty]
        public NoviObrokModel NoviObrok { get; set; }

        public string Sporocilo { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            var uporabnikId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            _logger.LogInformation("Nalaganje jedilnikov za uporabnika {UporabnikId}", uporabnikId);

            try
            {
                MojiJedilniki = await _upravljalecJedilnika.PridobiJedilnikeUporabnika(uporabnikId);

                if (id.HasValue)
                {
                    TrenutniJedilnik = await _upravljalecJedilnika.PridobiJedilnik(id.Value);
                    // Preveri èe ima uporabnik pravico do tega jedilnika
                    if (TrenutniJedilnik == null || TrenutniJedilnik.UporabnikId != uporabnikId)
                    {
                        _logger.LogWarning("Uporabnik {UporabnikId} nima dostopa do jedilnika {JedilnikId}",
                            uporabnikId, id.Value);
                        return RedirectToPage("/Error");
                    }
                }
                else if (MojiJedilniki.Any())
                {
                    TrenutniJedilnik = MojiJedilniki.First();
                }

                // Pridobi samo javne recepte in recepte trenutnega uporabnika
                VsiRecepti = await _upravljalecReceptov.PridobiVseRecepte(uporabnikId);

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Napaka pri nalaganju jedilnika");
                return RedirectToPage("/Error");
            }
        }

        public async Task<IActionResult> OnPostDodajReceptAsync(int obrokId, int receptId)
        {
            try
            {
                var uporabnikId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                _logger.LogInformation("Dodajanje recepta {ReceptId} v obrok {ObrokId}", receptId, obrokId);

                // Preveri èe ima uporabnik dostop do recepta
                var recept = await _upravljalecReceptov.PridobiRecept(receptId, uporabnikId);
                if (recept == null)
                {
                    _logger.LogWarning("Recept {ReceptId} ne obstaja ali uporabnik nima dostopa", receptId);
                    Sporocilo = "Recept ne obstaja ali nimate dostopa do njega.";
                    return RedirectToPage();
                }

                var obrok = await _upravljalecJedilnika.PridobiObrok(obrokId);
                if (obrok == null || obrok.Jedilnik.UporabnikId != uporabnikId)
                {
                    _logger.LogWarning("Obrok {ObrokId} ne obstaja ali uporabnik nima dostopa", obrokId);
                    Sporocilo = "Obrok ne obstaja ali nimate dostopa do njega.";
                    return RedirectToPage();
                }

                var uspeh = await _upravljalecJedilnika.DodajReceptVObrok(obrokId, receptId);
                if (!uspeh)
                {
                    _logger.LogWarning("Neuspešno dodajanje recepta {ReceptId} v obrok {ObrokId}",
                        receptId, obrokId);
                    Sporocilo = "Napaka pri dodajanju recepta v obrok.";
                }

                _logger.LogInformation("Recept {ReceptId} uspešno dodan v obrok {ObrokId}",
                    receptId, obrokId);
                return RedirectToPage(new { id = obrok.JedilnikId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Napaka pri dodajanju recepta v obrok");
                Sporocilo = "Prišlo je do neprièakovane napake.";
                return RedirectToPage();
            }
        }

        public async Task<IActionResult> OnPostOdstraniReceptAsync(int obrokId, int receptId)
        {
            try
            {
                // Poèistimo ModelState
                ModelState.Clear();
                var uporabnikId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                var obrok = await _upravljalecJedilnika.PridobiObrok(obrokId);
                if (obrok == null || obrok.Jedilnik.UporabnikId != uporabnikId)
                {
                    _logger.LogWarning("Uporabnik {UporabnikId} nima dostopa do obroka {ObrokId}",
                        uporabnikId, obrokId);
                    return RedirectToPage("/Error");
                }

                var uspeh = await _upravljalecJedilnika.OdstraniReceptIzObroka(obrokId, receptId);
                if (!uspeh)
                {
                    Sporocilo = "Napaka pri odstranjevanju recepta.";
                }

                // Ponovno naložimo podatke
                MojiJedilniki = await _upravljalecJedilnika.PridobiJedilnikeUporabnika(uporabnikId);
                TrenutniJedilnik = await _upravljalecJedilnika.PridobiJedilnik(obrok.JedilnikId);
                VsiRecepti = await _upravljalecReceptov.PridobiVseRecepte(uporabnikId);

                return RedirectToPage(new { id = obrok.JedilnikId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Napaka pri brisanju recepta iz obroka");
                return RedirectToPage("/Error");
            }
        }

        public async Task<IActionResult> OnPostDodajObrokAsync(int jedilnikId)
        {
            try
            {
                var uporabnikId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                // Preveri èe ima uporabnik dostop do jedilnika
                var jedilnik = await _upravljalecJedilnika.PridobiJedilnik(jedilnikId);
                if (jedilnik == null || jedilnik.UporabnikId != uporabnikId)
                {
                    _logger.LogWarning("Uporabnik {UporabnikId} nima dostopa do jedilnika {JedilnikId}",
                        uporabnikId, jedilnikId);
                    return RedirectToPage("/Error");
                }

                if (!ModelState.IsValid)
                {
                    ModelState.Remove("NoviJedilnikNaziv");
                    if (!ModelState.IsValid)
                    {
                        TrenutniJedilnik = jedilnik;
                        VsiRecepti = await _upravljalecReceptov.PridobiVseRecepte(uporabnikId);
                        return Page();
                    }
                }

                var obrok = new Obrok
                {
                    Naziv = NoviObrok.Naziv,
                    Cas = NoviObrok.Cas
                };

                var uspeh = await _upravljalecJedilnika.DodajObrok(jedilnikId, obrok);
                if (uspeh)
                {
                    _logger.LogInformation("Obrok uspešno dodan v jedilnik {JedilnikId}", jedilnikId);
                    return RedirectToPage(new { id = jedilnikId });
                }

                _logger.LogWarning("Neuspešno dodajanje obroka v jedilnik {JedilnikId}", jedilnikId);
                Sporocilo = "Napaka pri dodajanju obroka.";
                TrenutniJedilnik = jedilnik;
                VsiRecepti = await _upravljalecReceptov.PridobiVseRecepte(uporabnikId);
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Napaka pri dodajanju obroka");
                return RedirectToPage("/Error");
            }
        }

        public async Task<IActionResult> OnPostOdstraniObrokAsync(int jedilnikId, int obrokId)
        {
            try
            {
                // Poèistimo ModelState
                ModelState.Clear();
                var uporabnikId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                var jedilnik = await _upravljalecJedilnika.PridobiJedilnik(jedilnikId);
                if (jedilnik == null || jedilnik.UporabnikId != uporabnikId)
                {
                    _logger.LogWarning("Uporabnik {UporabnikId} nima dostopa do jedilnika {JedilnikId}",
                        uporabnikId, jedilnikId);
                    return RedirectToPage("/Error");
                }

                var uspeh = await _upravljalecJedilnika.OdstraniObrok(jedilnikId, obrokId);
                if (!uspeh)
                {
                    Sporocilo = "Napaka pri brisanju obroka.";
                }

                // Ponovno naložimo podatke
                MojiJedilniki = await _upravljalecJedilnika.PridobiJedilnikeUporabnika(uporabnikId);
                TrenutniJedilnik = await _upravljalecJedilnika.PridobiJedilnik(jedilnikId);
                VsiRecepti = await _upravljalecReceptov.PridobiVseRecepte(uporabnikId);

                return RedirectToPage(new { id = jedilnikId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Napaka pri brisanju obroka");
                return RedirectToPage("/Error");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                var uporabnikId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                if (string.IsNullOrWhiteSpace(NoviJedilnikNaziv))
                {
                    ModelState.AddModelError("NoviJedilnikNaziv", "Naziv je obvezen");
                    VsiRecepti = await _upravljalecReceptov.PridobiVseRecepte(uporabnikId);
                    return Page();
                }

                var noviJedilnik = await _upravljalecJedilnika.KreirajJedilnik(NoviJedilnikNaziv, uporabnikId);

                if (noviJedilnik != null)
                {
                    _logger.LogInformation("Ustvarjen nov jedilnik {JedilnikId} za uporabnika {UporabnikId}",
                        noviJedilnik.Id, uporabnikId);
                    return RedirectToPage("/Jedilnik/Index", new { id = noviJedilnik.Id });
                }

                _logger.LogWarning("Neuspešno ustvarjanje jedilnika za uporabnika {UporabnikId}",
                    uporabnikId);
                Sporocilo = "Napaka pri ustvarjanju jedilnika.";
                VsiRecepti = await _upravljalecReceptov.PridobiVseRecepte(uporabnikId);
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Napaka pri ustvarjanju jedilnika");
                return RedirectToPage("/Error");
            }
        }

        public async Task<IActionResult> OnPostDeliJedilnikAsync(int jedilnikId)
        {
            try
            {
                var uporabnikId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                var jedilnik = await _upravljalecJedilnika.PridobiJedilnik(jedilnikId);
                if (jedilnik == null || jedilnik.UporabnikId != uporabnikId)
                {
                    _logger.LogWarning("Uporabnik {UporabnikId} nima dostopa do jedilnika {JedilnikId}",
                        uporabnikId, jedilnikId);
                    return RedirectToPage("/Error");
                }

                var uspeh = await _upravljalecJedilnika.DeliJedilnik(jedilnikId, uporabnikId);
                if (!uspeh)
                {
                    Sporocilo = "Napaka pri deljenju jedilnika.";
                }
                else
                {
                    Sporocilo = "Jedilnik je bil uspešno deljen.";
                }

                return RedirectToPage(new { id = jedilnikId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Napaka pri deljenju jedilnika");
                return RedirectToPage("/Error");
            }
        }

        public async Task<IActionResult> OnPostPrekiniDeljenjeAsync(int jedilnikId)
        {
            try
            {
                var uporabnikId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                var jedilnik = await _upravljalecJedilnika.PridobiJedilnik(jedilnikId);
                if (jedilnik == null || jedilnik.UporabnikId != uporabnikId)
                {
                    _logger.LogWarning("Uporabnik {UporabnikId} nima dostopa do jedilnika {JedilnikId}",
                        uporabnikId, jedilnikId);
                    return RedirectToPage("/Error");
                }

                var uspeh = await _upravljalecJedilnika.OdstraniDeljenje(jedilnikId, uporabnikId);
                if (!uspeh)
                {
                    Sporocilo = "Napaka pri preklicu deljenja jedilnika.";
                }
                else
                {
                    Sporocilo = "Deljenje jedilnika je bilo preklicano.";
                }

                return RedirectToPage(new { id = jedilnikId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Napaka pri preklicu deljenja jedilnika");
                return RedirectToPage("/Error");
            }
        }
    }
}