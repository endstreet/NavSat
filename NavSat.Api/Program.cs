using NavSat.Core.Abstrations.ApiClients;
using NavSat.Core.Abstrations.Services;
using NavSat.Core.ApiClients.Mappers;
using NavSat.Core.ApiClients;
using NavSat.Core.Services;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Configuration;

namespace NavSat.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddSingleton<IOrbitApiClientConfig, OrbitApiClientConfig>(x =>
            new OrbitApiClientConfig(builder.Configuration.GetValue<string>("OrbitApi:BaseUrl")));
            builder.Services.AddScoped<ISatOrbitMapper, SatOrbitMapper>();
            builder.Services.AddScoped<IOrbitApiClient, OrbitApiClient>(x =>
            new OrbitApiClient(x.GetRequiredService<IOrbitApiClientConfig>(), x.GetRequiredService<ISatOrbitMapper>()));
            builder.Services.AddScoped<IConstellationService, ConstellationService>();
            builder.Services.AddScoped<IGeoMath, GeoMath>();
            builder.Services.AddScoped<ISatMath, SatMath>();
            builder.Services.AddScoped<ISatelliteService, SatelliteService>(x =>
            new SatelliteService(x.GetRequiredService<IConstellationService>()));
            builder.Services.AddScoped<ISatellitePathService, SatellitePathService>(x =>
            new SatellitePathService(x.GetRequiredService<IGeoMath>(), x.GetRequiredService<ISatMath>(), x.GetRequiredService<IOrbitApiClient>(), x.GetRequiredService<ISatelliteService>()));

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
    internal class OrbitApiClientConfig : IOrbitApiClientConfig
    {

        public string BaseUrl { get; }
        
        public OrbitApiClientConfig(string baseurl)
        {
            BaseUrl = baseurl;
        }
    }
}