using HealthChecks.UI.Client;
using k8s.KubeConfigModels;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace DempHealthCheck.WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            builder.Services.AddHealthChecksUI().AddInMemoryStorage();
            builder.Services.AddHealthChecks().AddSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), tags: new[] { "database" })
                .AddCheck<MyHealthCheck>("MyHealthCheck", tags: new[] { "custom" });
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            app.MapHealthChecks("/health", new HealthCheckOptions
            {
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
            app.MapHealthChecks("/health/custom", new HealthCheckOptions
            {
                Predicate = reg => reg.Tags.Contains("custom"),
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
            app.MapHealthChecksUI();

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
}