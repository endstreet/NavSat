using Microsoft.OpenApi.Models;
using NavSat.Core.Abstrations.ApiClients;
using NavSat.Core.Abstrations.Services;
using NavSat.Core.ApiClients;
using NavSat.Core.Services;

namespace NavSat.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddSingleton<IOrbitApiClientConfig, OrbitApiClientConfig>(_ => new OrbitApiClientConfig(builder.Configuration.GetValue<string>("OrbitApi:BaseUrl")));
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            builder.Services.AddScoped<IOrbitApiClient, OrbitApiClient>();
            builder.Services.AddScoped<IConstellationService, ConstellationService>();
            builder.Services.AddScoped<IGeoMath, GeoMath>();
            builder.Services.AddScoped<ISatMath, SatMath>();
            builder.Services.AddScoped<ISatelliteService, SatelliteService>();
            builder.Services.AddScoped<ISatellitePathService, SatellitePathService>();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(
                c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo { Title = "NavSat API", Version = "v1" });
                    c.IncludeXmlComments(Path.Combine(System.AppContext.BaseDirectory, "SwaggerAnnotation.xml"));
                });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                                  policy =>
                                  {
                                      policy.WithOrigins("*");
                                  });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.UseCors(MyAllowSpecificOrigins);
            }
            else
            {
                app.UseHttpsRedirection();
                app.UseAuthorization();
            }

            app.MapControllers();

            app.Run();
        }
    }

}