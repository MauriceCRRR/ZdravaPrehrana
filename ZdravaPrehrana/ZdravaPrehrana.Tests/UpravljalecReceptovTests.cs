﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ZdravaPrehrana.Controllers;
using ZdravaPrehrana.Data;
using ZdravaPrehrana.Entitete;

namespace ZdravaPrehrana.Tests
{
    [TestClass]
    public class UpravljalecReceptovTests
    {
        private ApplicationDbContext _context;
        private Mock<ILogger<UpravljalecReceptov>> _loggerMock;
        private UpravljalecReceptov _upravljalec;

        [TestInitialize]
        public async Task Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDB_" + Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _loggerMock = new Mock<ILogger<UpravljalecReceptov>>();
            _upravljalec = new UpravljalecReceptov(_context, _loggerMock.Object);

            // Dodamo testnega uporabnika
            var uporabnik = new Uporabnik
            {
                Id = 1,
                UporabniskoIme = "TestUser",
                Email = "test@example.com",
                Geslo = "TestGeslo123",
                Vloga = UporabniskaVloga.Uporabnik
            };
            _context.Uporabniki.Add(uporabnik);
            await _context.SaveChangesAsync();

            // Dodaj sestavino
            var sestavina = new Sestavina
            {
                Id = 1,
                Naziv = "TestSestavina",
                Kalorije = 100,
                Beljakovine = 10,
                Mascobe = 5,
                OgljikoviHidrati = 15
            };
            await _context.Sestavine.AddAsync(sestavina);
            await _context.SaveChangesAsync();

            // Preverimo če je uporabnik shranjen
            var shranjeniUporabnik = await _context.Uporabniki.FindAsync(1);
            if (shranjeniUporabnik == null)
            {
                throw new Exception("Uporabnik ni bil uspešno shranjen v setup fazi");
            }
        }

        [TestMethod]
        public async Task Test_DodajRecept_ValidInput_CreatedSuccessfully()
        {
            // Preveri če uporabnik obstaja
            var uporabnik = await _context.Uporabniki.FindAsync(1);
            Assert.IsNotNull(uporabnik, "Uporabnik mora obstajati pred testom");

            // Najdi sestavino
            var sestavina = await _context.Sestavine.FindAsync(1);
            Assert.IsNotNull(sestavina, "Sestavina mora obstajati pred testom");

            // Arrange
            var receptSestavina = new ReceptSestavina
            {
                SestavinaId = sestavina.Id,
                Sestavina = sestavina,
                Kolicina = 100,
                Enota = "g"
            };

            var recept = new Recept
            {
                Naziv = "Test recept",
                Postopek = "Test postopek priprave",
                Kalorije = 500,
                CasPriprave = 30,
                JeJaven = true,
                AvtorId = uporabnik.Id,
                DatumUstvarjanja = DateTime.Now,
                Avtor = uporabnik,
                ReceptSestavine = new List<ReceptSestavina> { receptSestavina }
            };

            // Act
            var rezultat = await _upravljalec.DodajRecept(recept, uporabnik.Id);

            // Assert
            Assert.IsTrue(rezultat);
            var shranjeniRecept = await _context.Recepti
                .Include(r => r.ReceptSestavine)
                .Include(r => r.Avtor)
                .FirstOrDefaultAsync(r => r.Naziv == "Test recept");
            Assert.IsNotNull(shranjeniRecept);
            Assert.AreEqual(recept.Naziv, shranjeniRecept.Naziv);
            Assert.AreEqual(recept.Postopek, shranjeniRecept.Postopek);
            Assert.AreEqual(1, shranjeniRecept.ReceptSestavine.Count);
        }

        [TestMethod]
        public async Task Test_PridobiJavneRecepte_ReturnsOnlyPublicRecipes()
        {
            // Arrange
            var uporabnik = await _context.Uporabniki.FindAsync(1);
            var sestavina = await _context.Sestavine.FindAsync(1);
            Assert.IsNotNull(sestavina);

            _context.Recepti.RemoveRange(_context.Recepti);
            await _context.SaveChangesAsync();

            var receptSestavina = new ReceptSestavina
            {
                SestavinaId = sestavina.Id,
                Sestavina = sestavina,
                Kolicina = 100,
                Enota = "g"
            };

            var javniRecept = new Recept
            {
                Naziv = "Javni recept",
                Postopek = "Test postopek 1",
                Kalorije = 300,
                CasPriprave = 30,
                JeJaven = true,
                AvtorId = 1,
                DatumUstvarjanja = DateTime.Now,
                Avtor = uporabnik,
                ReceptSestavine = new List<ReceptSestavina> { receptSestavina }
            };

            await _context.Recepti.AddAsync(javniRecept);
            await _context.SaveChangesAsync();

            // Act
            var recepti = await _upravljalec.PridobiJavneRecepte();

            // Assert
            Assert.AreEqual(1, recepti.Count);
            Assert.IsTrue(recepti.All(r => r.JeJaven));
        }

        [TestMethod]
        public async Task Test_PridobiRecepteUporabnika_ReturnsUserRecipes()
        {
            // Arrange
            _context.Recepti.RemoveRange(_context.Recepti);
            await _context.SaveChangesAsync();
            await DodajTestneRecepte();
            int uporabnikId = 1;

            // Act
            var recepti = await _upravljalec.PridobiRecepteUporabnika(uporabnikId);

            // Assert
            Assert.AreEqual(2, recepti.Count);
            Assert.IsTrue(recepti.All(r => r.AvtorId == uporabnikId));
        }

        [TestMethod]
        public async Task Test_UrediRecept_ValidInput_UpdatedSuccessfully()
        {
            // Arrange
            var uporabnik = await _context.Uporabniki.FindAsync(1);
            Assert.IsNotNull(uporabnik, "Uporabnik mora obstajati pred testom");

            var sestavina = await _context.Sestavine.FindAsync(1);
            Assert.IsNotNull(sestavina, "Sestavina mora obstajati pred testom");

            var receptSestavina = new ReceptSestavina
            {
                SestavinaId = sestavina.Id,
                Sestavina = sestavina,
                Kolicina = 100,
                Enota = "g"
            };

            var recept = new Recept
            {
                Naziv = "Originalni recept",
                Postopek = "Originalni postopek",
                Kalorije = 300,
                CasPriprave = 30,
                JeJaven = false,
                AvtorId = uporabnik.Id,
                DatumUstvarjanja = DateTime.Now,
                Avtor = uporabnik,
                ReceptSestavine = new List<ReceptSestavina> { receptSestavina }
            };

            await _context.Recepti.AddAsync(recept);
            await _context.SaveChangesAsync();

            var posodobljenReceptSestavina = new ReceptSestavina
            {
                SestavinaId = sestavina.Id,
                Sestavina = sestavina,
                Kolicina = 150,
                Enota = "g"
            };

            var posodobljenRecept = new Recept
            {
                Id = recept.Id,
                Naziv = "Posodobljen recept",
                Postopek = "Posodobljen postopek",
                Kalorije = 400,
                CasPriprave = 45,
                JeJaven = true,
                AvtorId = uporabnik.Id,
                DatumUstvarjanja = recept.DatumUstvarjanja,
                ReceptSestavine = new List<ReceptSestavina> { posodobljenReceptSestavina }
            };

            // Act
            var rezultat = await _upravljalec.UrediRecept(recept.Id, posodobljenRecept, uporabnik.Id);

            // Assert
            Assert.IsTrue(rezultat);
            var shranjeniRecept = await _context.Recepti
                .Include(r => r.ReceptSestavine)
                .FirstOrDefaultAsync(r => r.Id == recept.Id);
            Assert.IsNotNull(shranjeniRecept);
            Assert.AreEqual("Posodobljen recept", shranjeniRecept.Naziv);
            Assert.AreEqual(1, shranjeniRecept.ReceptSestavine.Count);
        }

        private async Task DodajTestneRecepte()
        {
            var uporabnik = await _context.Uporabniki.FindAsync(1);
            Assert.IsNotNull(uporabnik, "Uporabnik mora obstajati pred testom");

            var sestavina = await _context.Sestavine.FindAsync(1);
            Assert.IsNotNull(sestavina, "Sestavina mora obstajati pred testom");

            var receptSestavina = new ReceptSestavina
            {
                SestavinaId = sestavina.Id,
                Sestavina = sestavina,
                Kolicina = 100,
                Enota = "g"
            };

            var recepti = new List<Recept>
           {
               new Recept
               {
                   Naziv = "Javni recept",
                   Postopek = "Test postopek 1",
                   Kalorije = 300,
                   CasPriprave = 30,
                   JeJaven = true,
                   AvtorId = 1,
                   DatumUstvarjanja = DateTime.Now,
                   Avtor = uporabnik,
                   ReceptSestavine = new List<ReceptSestavina> { receptSestavina }
               },
               new Recept
               {
                   Naziv = "Zasebni recept 1",
                   Postopek = "Test postopek 2",
                   Kalorije = 400,
                   CasPriprave = 45,
                   JeJaven = false,
                   AvtorId = 1,
                   DatumUstvarjanja = DateTime.Now,
                   Avtor = uporabnik,
                   ReceptSestavine = new List<ReceptSestavina> { receptSestavina }
               },
               new Recept
               {
                   Naziv = "Zasebni recept 2",
                   Postopek = "Test postopek 3",
                   Kalorije = 500,
                   CasPriprave = 60,
                   JeJaven = false,
                   AvtorId = 2,
                   DatumUstvarjanja = DateTime.Now,
                   ReceptSestavine = new List<ReceptSestavina> { receptSestavina }
               }
           };

            await _context.Recepti.AddRangeAsync(recepti);
            await _context.SaveChangesAsync();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}