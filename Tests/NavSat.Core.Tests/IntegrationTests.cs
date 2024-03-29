using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NavSat.Core.Abstrations.ApiClients;
using NavSat.Core.Abstrations.Models;
using NavSat.Core.Abstrations.Services;
using NavSat.Core.ApiClients;
using NavSat.Core.Services;
//using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NavSat.Core.Tests
{

    [TestClass]
    public class IntegrationTests
    {
        private OrbitApiClient _orbitApiClient;
        private SatellitePathService _satellitePathService;

        public IntegrationTests()
        {
            var services = new ServiceCollection();
            services.AddSingleton<IOrbitApiClientConfig, TestConfig>(x =>
            new TestConfig());
            //services.AddScoped<ISatOrbitMapper, SatOrbitMapper>();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddScoped<IOrbitApiClient, OrbitApiClient>(x =>
            new OrbitApiClient(x.GetRequiredService<IOrbitApiClientConfig>(), x.GetRequiredService<IMapper>()));
            services.AddScoped<IConstellationService, ConstellationService>();
            services.AddScoped<IGeoMath, GeoMath>();
            services.AddScoped<ISatMath, SatMath>();
            services.AddScoped<ISatelliteService, SatelliteService>(x =>
            new SatelliteService(x.GetRequiredService<IConstellationService>()));

            services.AddScoped<ISatellitePathService, SatellitePathService>(x =>
            new SatellitePathService(x.GetRequiredService<IGeoMath>(), x.GetRequiredService<ISatMath>(), x.GetRequiredService<IOrbitApiClient>(), x.GetRequiredService<ISatelliteService>()));

            var serviceProvider = services.BuildServiceProvider();

            _orbitApiClient = (OrbitApiClient)serviceProvider.GetService<IOrbitApiClient>();
            _satellitePathService = (SatellitePathService)serviceProvider.GetService<ISatellitePathService>();
        }


        [TestMethod]
        public async Task OrbitApiClient_SmokeTest()
        {

            // Arrange
            //var client = CreateOrbitApiClient();

            // Act
            var orbits = await _orbitApiClient.GetOrbitsAsAtAsync(DateTimeOffset.UtcNow);

            // Assert
            Assert.IsNotNull(orbits);

        }


        [TestMethod]
        public async Task SatellitePathService_SmokeTest()
        {

            // Arrange

            //var service = CreateSatellitePathService();


            var capeTown = new GeoCoordinate()
            {
                Latitude = -33.9249,
                Longitude = 18.4241,
                Altitude = 100
            };

            var start = DateTimeOffset.UtcNow.AddMinutes(-10);
            var mid = DateTimeOffset.UtcNow;
            var end = DateTimeOffset.UtcNow.AddMinutes(10);

            var times = new[] { start, mid, end };

            // Act
            var result = await _satellitePathService.GetAsSeenFromDuringAsync(capeTown, times);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Any(s => s.Path.Count() == times.Count()));

        }

    }
}
