using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NavSat.Core.Abstrations.Models;
using NavSat.Core.Abstrations.Services;
using NavSat.Core.Services;
using System.Collections.Generic;
using System.Linq.Expressions;

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
            var mockConstellationService = Mock.Of<IConstellationService>();
            var exclSatellite = new SatelliteService(mockConstellationService);

            // ACT
            exclSatellite.CreateFrom(1);//Satellite id 1 is valid

            // ASSERT
            Mock.Get(mockConstellationService).Verify(p => p.For(1), Times.Once());

        }

        [TestMethod]
        public void SatelliteService_MockDependencyInvalidTest()
        {
            // ARRANGE
            //Use Mocked ConstellationService to prevent testing of this dependency
            var mockConstellationService = Mock.Of<IConstellationService>();
            var exclSatellite = new SatelliteService(mockConstellationService);

            // ACT
            exclSatellite.CreateFrom(0);//SateliteId 0 is invalid

            // ASSERT
            Mock.Get(mockConstellationService).Verify(p => p.For(0), Times.Once());
        }
    }
}
