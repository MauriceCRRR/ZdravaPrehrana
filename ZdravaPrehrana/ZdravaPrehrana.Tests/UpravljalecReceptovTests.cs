using Microsoft.EntityFrameworkCore;
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
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDB_" + Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _loggerMock = new Mock<ILogger<UpravljalecReceptov>>();
            _upravljalec = new UpravljalecReceptov(_context, _loggerMock.Object);
        }

        [TestMethod]
        public async Task Test_DodajRecept_ValidInput_CreatedSuccessfully()
        {
            // Arrange
            var sestavina = new Sestavina { Naziv = "TestSestavina" };
            await _context.Sestavine.AddAsync(sestavina);
            await _context.SaveChangesAsync();

            var recept = new Recept
            {
                Naziv = "Test recept",
                Postopek = "Test postopek priprave",
                Kalorije = 500,
                CasPriprave = 30,
                JeJaven = true,
                ReceptSestavine = new List<ReceptSestavina>
                {
                    new ReceptSestavina
                    {
                        Sestavina = sestavina,
                        Kolicina = 2,
                        Enota = "kos"
                    }
                }
            };

            // Act
            var rezultat = await _upravljalec.DodajRecept(recept, 1);

            // Assert
            Assert.IsTrue(rezultat);
            var shranjeniRecept = await _context.Recepti
                .Include(r => r.ReceptSestavine)
                .ThenInclude(rs => rs.Sestavina)
                .FirstOrDefaultAsync(r => r.Naziv == "Test recept");
            Assert.IsNotNull(shranjeniRecept);
            Assert.AreEqual(recept.Naziv, shranjeniRecept.Naziv);
        }

        [TestMethod]
        public async Task Test_PridobiJavneRecepte_ReturnsOnlyPublicRecipes()
        {
            // Arrange
            await DodajTestneRecepte();

            // Act
            var recepti = await _upravljalec.PridobiJavneRecepte();

            // Assert
            Assert.IsTrue(recepti.Any());
            Assert.IsTrue(recepti.All(r => r.JeJaven));
            Assert.AreEqual(1, recepti.Count);
        }

        [TestMethod]
        public async Task Test_PridobiRecepteUporabnika_ReturnsUserRecipes()
        {
            // Arrange
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
            var originalniRecept = new Recept
            {
                Naziv = "Originalni recept",
                Postopek = "Originalni postopek",
                Kalorije = 300,
                CasPriprave = 30,
                JeJaven = false,
                AvtorId = 1
            };
            await _context.Recepti.AddAsync(originalniRecept);
            await _context.SaveChangesAsync();

            var posodobljenRecept = new Recept
            {
                Id = originalniRecept.Id,
                Naziv = "Posodobljen recept",
                Postopek = "Posodobljen postopek",
                Kalorije = 400,
                CasPriprave = 45,
                JeJaven = true,
                AvtorId = 1
            };

            // Act
            var rezultat = await _upravljalec.UrediRecept(originalniRecept.Id, posodobljenRecept, 1);

            // Assert
            Assert.IsTrue(rezultat);
            var shranjeniRecept = await _context.Recepti.FindAsync(originalniRecept.Id);
            Assert.IsNotNull(shranjeniRecept);
            Assert.AreEqual("Posodobljen recept", shranjeniRecept.Naziv);
            Assert.AreEqual(400, shranjeniRecept.Kalorije);
        }

        private async Task DodajTestneRecepte()
        {
            var recepti = new List<Recept>
            {
                new Recept
                {
                    Naziv = "Javni recept",
                    Postopek = "Test postopek 1",
                    Kalorije = 300,
                    CasPriprave = 30,
                    JeJaven = true,
                    AvtorId = 1
                },
                new Recept
                {
                    Naziv = "Zasebni recept 1",
                    Postopek = "Test postopek 2",
                    Kalorije = 400,
                    CasPriprave = 45,
                    JeJaven = false,
                    AvtorId = 1
                },
                new Recept
                {
                    Naziv = "Zasebni recept 2",
                    Postopek = "Test postopek 3",
                    Kalorije = 500,
                    CasPriprave = 60,
                    JeJaven = false,
                    AvtorId = 2
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