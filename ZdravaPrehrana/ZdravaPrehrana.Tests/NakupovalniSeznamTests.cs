using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZdravaPrehrana.Entitete;

namespace ZdravaPrehrana.Tests
{
    [TestClass]
    public class NakupovalniSeznamTests
    {
        [TestMethod]
        public void Test_DodajSestavino_ValidInput_AddedSuccessfully()
        {
            // Arrange
            var seznam = new NakupovalniSeznam
            {
                Id = 1,
                Naziv = "Test seznam"
            };

            // Act
            seznam.DodajSestavino("Jabolka", 2, "kg");

            // Assert
            Assert.AreEqual(1, seznam.Postavke.Count);
            var postavka = seznam.Postavke.First();
            Assert.AreEqual("Jabolka", postavka.Naziv);
            Assert.AreEqual(2, postavka.Kolicina);
            Assert.AreEqual("kg", postavka.Enota);
            Assert.IsFalse(postavka.JeObkljukana);
        }

        [TestMethod]
        public void Test_DodajSestavino_EmptyInput_NotAdded()
        {
            // Arrange
            var seznam = new NakupovalniSeznam();

            // Act
            seznam.DodajSestavino("", 1, "kos");

            // Assert
            Assert.AreEqual(0, seznam.Postavke.Count);
        }

        [TestMethod]
        public void Test_ObkljukajPostavko_ExistingItem_StatusChanged()
        {
            // Arrange
            var seznam = new NakupovalniSeznam();
            seznam.DodajSestavino("Jabolka", 2, "kg");

            // Act
            seznam.ObkljukajPostavko("Jabolka");

            // Assert
            var postavka = seznam.Postavke.First();
            Assert.IsTrue(postavka.JeObkljukana);
        }
    }
}