using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZdravaPrehrana.Controllers;
using ZdravaPrehrana.Data;
using ZdravaPrehrana.Entitete;

namespace ZdravaPrehrana.Tests
{
    [TestClass]
    public class UpravljalecNakupovanjaTests
    {
        private ApplicationDbContext _context;
        private UpravljalecNakupovanja _upravljalec;

        [TestInitialize]
        public void Setup()
        {
            // Nastavimo in-memory database za testiranje
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDB_" + Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _upravljalec = new UpravljalecNakupovanja(_context);
        }

        [TestMethod]
        public async Task Test_UstvariSeznam_ValidInput_CreatedSuccessfully()
        {
            // Arrange
            string naziv = "Moj testni seznam";
            int uporabnikId = 1;

            // Act
            var rezultat = await _upravljalec.UstvariSeznam(naziv, uporabnikId);

            // Assert
            Assert.IsNotNull(rezultat);
            Assert.AreEqual(naziv, rezultat.Naziv);
            Assert.AreEqual(uporabnikId, rezultat.UporabnikId);
            Assert.IsNotNull(rezultat.Postavke);
            Assert.AreEqual(0, rezultat.Postavke.Count);
        }

        [TestMethod]
        public async Task Test_DodajIzdelek_ValidInput_AddedSuccessfully()
        {
            // Arrange
            var seznam = await _upravljalec.UstvariSeznam("Test seznam", 1);
            string naziv = "Jabolka";
            double kolicina = 2;
            string enota = "kg";

            // Act
            var rezultat = await _upravljalec.DodajIzdelek(naziv, kolicina, enota, seznam.Id);

            // Assert
            Assert.IsTrue(rezultat);
            var posodobljenSeznam = await _upravljalec.PridobiSeznam(seznam.Id);
            Assert.AreEqual(1, posodobljenSeznam.Postavke.Count);
            var postavka = posodobljenSeznam.Postavke.First();
            Assert.AreEqual(naziv, postavka.Naziv);
            Assert.AreEqual(kolicina, postavka.Kolicina);
            Assert.AreEqual(enota, postavka.Enota);
        }

        [TestMethod]
        public async Task Test_OdstraniIzdelek_ExistingItem_RemovedSuccessfully()
        {
            // Arrange
            var seznam = await _upravljalec.UstvariSeznam("Test seznam", 1);
            await _upravljalec.DodajIzdelek("Jabolka", 2, "kg", seznam.Id);

            // Act
            var rezultat = await _upravljalec.OdstraniIzdelek(seznam.Id, "Jabolka");

            // Assert
            Assert.IsTrue(rezultat);
            var posodobljenSeznam = await _upravljalec.PridobiSeznam(seznam.Id);
            Assert.AreEqual(0, posodobljenSeznam.Postavke.Count);
        }

        [TestMethod]
        public async Task Test_OznaciIzdelek_ExistingItem_ToggledSuccessfully()
        {
            // Arrange
            var seznam = await _upravljalec.UstvariSeznam("Test seznam", 1);
            await _upravljalec.DodajIzdelek("Jabolka", 2, "kg", seznam.Id);

            // Act
            var rezultat = await _upravljalec.OznaciIzdelek(seznam.Id, "Jabolka");

            // Assert
            Assert.IsTrue(rezultat);
            var posodobljenSeznam = await _upravljalec.PridobiSeznam(seznam.Id);
            var postavka = posodobljenSeznam.Postavke.First();
            Assert.IsTrue(postavka.JeObkljukana);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}