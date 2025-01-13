using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using ZdravaPrehrana.Data;
using ZdravaPrehrana.Services;
using ZdravaPrehrana.Controllers;
using ZdravaPrehrana.Entitete;
using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddRazorPages();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
   options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register services
builder.Services.AddScoped<IUporabnikService, UporabnikService>();
builder.Services.AddScoped<UpravljalecJedilnika>();
builder.Services.AddScoped<UpravljalecCiljev>();
builder.Services.AddScoped<UpravljalecHranil>();
builder.Services.AddScoped<UpravljalecNakupovanja>();
builder.Services.AddScoped<UpravljalecNasvetov>();
builder.Services.AddScoped<UpravljalecReceptov>();
builder.Services.AddScoped<UpravljalecDeljenja>();
builder.Services.AddScoped<UpravljalecOcen>();

// Dodajanje logginga
builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
    logging.AddDebug();
});

// Nastavitve avtentikacije
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Index";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.Cookie.Name = "ZdravaPrehrana.Auth";
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("JeStrokovnjak", policy =>
        policy.RequireRole("Strokovnjak"));
});

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, "DataProtection-Keys")))
    .SetApplicationName("ZdravaPrehrana");
    
var app = builder.Build();

// Inicializacija baze
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate(); // Dodana migracija

        // Dodaj testne uporabnike èe baza nima uporabnikov
        if (!context.Uporabniki.Any())
        {
            // Navaden uporabnik
            var testniUporabnik = new Uporabnik
            {
                UporabniskoIme = "test",
                Email = "test@test.com",
                Geslo = "test123",
                Vloga = UporabniskaVloga.Uporabnik
            };

            // Strokovnjak
            var strokovnjak = new Uporabnik
            {
                UporabniskoIme = "strokovnjak",
                Email = "strokovnjak@test.com",
                Geslo = "test123",
                Vloga = UporabniskaVloga.Strokovnjak
            };

            context.Uporabniki.AddRange(testniUporabnik, strokovnjak);

            // Ustvari profile
            var testniProfil = new UporabnikProfil
            {
                Uporabnik = testniUporabnik,
                Visina = 180,
                Teza = 80,
                Alergije = new List<string>(),
                Omejitve = new List<string>()
            };

            var strokovnjakProfil = new UporabnikProfil
            {
                Uporabnik = strokovnjak,
                Visina = 175,
                Teza = 75,
                Alergije = new List<string>(),
                Omejitve = new List<string>()
            };

            context.Profili.AddRange(testniProfil, strokovnjakProfil);
            context.SaveChanges();
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Napaka pri inicializaciji baze");
    }
}

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();