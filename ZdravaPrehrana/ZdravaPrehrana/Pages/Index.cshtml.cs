using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ZdravaPrehrana.Models;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace ZdravaPrehrana.Pages
{
    public class IndexModel : PageModel
{
    private readonly IUporabnikService _uporabnikService;
    private readonly ILogger<IndexModel> _logger;  // Dodaj logger

    public IndexModel(IUporabnikService uporabnikService, ILogger<IndexModel> logger)
    {
        _uporabnikService = uporabnikService;
        _logger = logger;
    }

    [BindProperty]
    public LoginInput LoginInput { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var uporabnik = await _uporabnikService.PreveriPrijavo(
                LoginInput.UporabniskoIme,
                LoginInput.Geslo);

            if (uporabnik == null)
            {
                ModelState.AddModelError(string.Empty, "Napaèno uporabniško ime ali geslo.");
                return Page();
            }

            _logger.LogInformation($"Uporabnik {uporabnik.UporabniskoIme} se je prijavil");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, uporabnik.UporabniskoIme),
                new Claim(ClaimTypes.NameIdentifier, uporabnik.Id.ToString()),
                new Claim(ClaimTypes.Role, uporabnik.Vloga.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, "CookieAuth");
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
            };


            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme, // Namesto "CookieAuth"
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return RedirectToPage("/Dashboard/Index");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Napaka pri prijavi: {ex.Message}");
            ModelState.AddModelError(string.Empty, "Prišlo je do napake pri prijavi.");
            return Page();
        }
    }
}
}