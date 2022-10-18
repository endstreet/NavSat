using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NavSat.Core.Abstrations.Models;
using NavSat.Core.Abstrations.Services;
using NavSat.Core.Services;

namespace NavSat.Core.Tests
{
    [TestClass]
    public class SatelliteServiceTests
    {

        [TestMethod]
        public void SatelliteService_MockDependencyValidTest()
        {
            // ARRANGE
            //Use Mocked ConstellationService to prevent testing of this dependency
            var mock = new Mock<IConstellationService>();
            //Return new Constellation for valid Sattelite Id
            mock.Setup(p => p.For(1)).Returns(new Constellation());
            //Return null for invalid Sattelite Id
            mock.Setup(p => p.For(0)).Returns((Constellation)null);
            var exclSatellite = new SatelliteService(mock.Object);

            // ACT
            var validResult = exclSatellite.CreateFrom(1);//Satellite id 1 is valid
            //var inValidResult = exclSatellite.CreateFrom(0);//SateliteId 0 is invalid

            // ASSERT
            Mock.Get(mock.Object).Verify(p => p.For(1), Times.Once());//Superfluos...
            //Mock.Get(mock.Object).Verify(p => p.For(0), Times.Once());//Superfluos...

            Assert.IsNotNull(validResult);
            //Assert.IsNull(inValidResult);
        }

        [TestMethod]
        public void SatelliteService_MockDependencyInvalidTest()
        {
            // ARRANGE
            //Use Mocked ConstellationService to prevent testing of this dependency
            var mock = new Mock<IConstellationService>();
            //Return null for invalid Sattelite Id
            mock.Setup(p => p.For(0)).Returns((Constellation)null);
            var exclSatellite = new SatelliteService(mock.Object);

            // ACT
            var result = exclSatellite.CreateFrom(0);//SateliteId 0 is invalid

            // ASSERT
            Mock.Get(mock.Object).Verify(p => p.For(0), Times.Once());//Superfluos...
            Assert.IsNull(result);
        }
    }
}
