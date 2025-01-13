using Microsoft.EntityFrameworkCore;
using ZdravaPrehrana.Entitete;

namespace ZdravaPrehrana.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { }

        public DbSet<NakupovalniSeznam> NakupovalniSeznami { get; set; }
        public DbSet<Recept> Recepti { get; set; }
        public DbSet<Uporabnik> Uporabniki { get; set; }
        public DbSet<UporabnikProfil> Profili { get; set; }
        public DbSet<Sestavina> Sestavine { get; set; }
        public DbSet<VnosHranil> VnosiHranil { get; set; }
        public DbSet<PrehranskiCilji> PrehranskiCilji { get; set; }
        public DbSet<Ocena> Ocene { get; set; }
        public DbSet<Obrok> Obroki { get; set; }
        public DbSet<Jedilnik> Jedilniki { get; set; }
        public DbSet<Nasvet> Nasveti { get; set; }
        public DbSet<ReceptSestavina> ReceptSestavine { get; set; }

        public DbSet<JedilnikOcena> JedilnikOcene { get; set; }
        public DbSet<SeznamPostavka> SeznamPostavke { get; set; }  // Dodaj to vrstico

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Uporabnik konfiguracija
            modelBuilder.Entity<Uporabnik>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UporabniskoIme).IsRequired();
                entity.Property(e => e.Geslo).IsRequired();
                entity.Property(e => e.Email).IsRequired();
                entity.Property(e => e.Vloga).IsRequired();

                entity.HasOne(e => e.Profil)
                     .WithOne(p => p.Uporabnik)
                     .HasForeignKey<UporabnikProfil>(p => p.UporabnikId);

                entity.HasMany(u => u.UstvarjeniRecepti)
                      .WithOne(r => r.Avtor)
                      .HasForeignKey(r => r.AvtorId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // UporabnikProfil konfiguracija
            modelBuilder.Entity<UporabnikProfil>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Visina);
                entity.Property(e => e.Teza);
            });

            modelBuilder.Entity<UporabnikProfil>()
                .Property<string>("AlergijeJson")
                .HasColumnName("Alergije");

            modelBuilder.Entity<UporabnikProfil>()
                .Property<string>("OmejitveJson")
                .HasColumnName("Omejitve");

            // Recept konfiguracija
            modelBuilder.Entity<Recept>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Naziv).IsRequired();
                entity.Property(e => e.Postopek).IsRequired();
                entity.Property(e => e.CasPriprave).IsRequired();
                entity.Property(e => e.Kalorije).IsRequired();
                entity.Property(e => e.JeJaven).IsRequired().HasDefaultValue(false);
                entity.Property(e => e.DatumUstvarjanja).IsRequired().HasDefaultValue(DateTime.Now);

                entity.HasOne(e => e.Avtor)
                      .WithMany(u => u.UstvarjeniRecepti)
                      .HasForeignKey(e => e.AvtorId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(e => e.Ocene)
                      .WithOne(o => o.Recept)
                      .HasForeignKey(o => o.ReceptId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.Obroki)
                      .WithMany(o => o.Recepti)
                      .UsingEntity(j => j.ToTable("ReceptiObroki"));
            });

            // ReceptSestavina konfiguracija
            modelBuilder.Entity<ReceptSestavina>(entity =>
            {
                entity.HasKey(e => new { e.ReceptId, e.SestavinaId });

                entity.HasOne(rs => rs.Recept)
                      .WithMany(r => r.ReceptSestavine)
                      .HasForeignKey(rs => rs.ReceptId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(rs => rs.Sestavina)
                      .WithMany(s => s.ReceptSestavine)
                      .HasForeignKey(rs => rs.SestavinaId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Property(e => e.Kolicina).IsRequired();
                entity.Property(e => e.Enota).IsRequired();
            });

            // Sestavina konfiguracija
            modelBuilder.Entity<Sestavina>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Naziv).IsRequired();
                entity.Property(e => e.Kalorije).IsRequired();
                entity.Property(e => e.Beljakovine).IsRequired();
                entity.Property(e => e.Mascobe).IsRequired();
                entity.Property(e => e.OgljikoviHidrati).IsRequired();
            });

            // Ostale konfiguracije ostanejo enake
            modelBuilder.Entity<SeznamPostavka>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Naziv).IsRequired();
                entity.Property(e => e.Kolicina).IsRequired();
                entity.Property(e => e.Enota).IsRequired();

                entity.HasOne(e => e.NakupovalniSeznam)
                      .WithMany(s => s.Postavke)
                      .HasForeignKey(e => e.NakupovalniSeznamId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // VnosHranil konfiguracija
            modelBuilder.Entity<VnosHranil>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Datum).IsRequired();

                entity.HasOne(e => e.Uporabnik)
                      .WithMany(u => u.VnosiHranil)
                      .HasForeignKey(e => e.UporabnikId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // PrehranskiCilji konfiguracija
            modelBuilder.Entity<PrehranskiCilji>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(e => e.Uporabnik)
                      .WithMany(u => u.PrehranskiCilji)
                      .HasForeignKey(e => e.UporabnikId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Jedilnik>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Naziv).IsRequired();
                entity.Property(e => e.JeDeljiv).IsRequired().HasDefaultValue(false);
                entity.Property(e => e.DatumDeljenja);

                entity.HasOne(e => e.Uporabnik)
                      .WithMany(u => u.Jedilniki)
                      .HasForeignKey(e => e.UporabnikId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(j => j.DeliZ)
                      .WithMany()
                      .UsingEntity(j => j.ToTable("DeljeniJedilniki"));

                entity.HasMany(j => j.Ocene)
                     .WithOne(o => o.Jedilnik)
                     .HasForeignKey(o => o.JedilnikId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<JedilnikOcena>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Vrednost).IsRequired();
                entity.Property(e => e.DatumOcene).IsRequired();
                entity.Property(e => e.Komentar).HasMaxLength(500);

                entity.HasOne(e => e.Uporabnik)
                      .WithMany()
                      .HasForeignKey(e => e.UporabnikId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Jedilnik)
                      .WithMany(j => j.Ocene)
                      .HasForeignKey(e => e.JedilnikId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Ocena konfiguracija
            modelBuilder.Entity<Ocena>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Vrednost).IsRequired();
                entity.Property(e => e.DatumOcene).IsRequired();
                entity.Property(e => e.Komentar).HasMaxLength(500);

                entity.HasOne(e => e.Uporabnik)
                      .WithMany(u => u.Ocene)
                      .HasForeignKey(e => e.UporabnikId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Recept)
                      .WithMany(r => r.Ocene)
                      .HasForeignKey(e => e.ReceptId)
                      .OnDelete(DeleteBehavior.Cascade);
            });


            // Obrok konfiguracija
            modelBuilder.Entity<Obrok>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Naziv).IsRequired();

                entity.HasOne(e => e.Jedilnik)
                      .WithMany(j => j.Obroki)
                      .HasForeignKey(e => e.JedilnikId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.Recepti)
                      .WithMany(r => r.Obroki);
            });

            // Nasvet konfiguracija
            modelBuilder.Entity<Nasvet>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Vprasanje).IsRequired();
                entity.Property(e => e.Odgovor).IsRequired(false);
                entity.Property(e => e.DatumVprasanja).IsRequired();

                entity.HasOne(e => e.Uporabnik)
                      .WithMany(u => u.PrejetiNasveti)
                      .HasForeignKey(e => e.UporabnikId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Strokovnjak)
                      .WithMany()
                      .HasForeignKey(e => e.StrokovnjakId)
                      .OnDelete(DeleteBehavior.Restrict)
                      .IsRequired(false);
            });
        }
    }
}