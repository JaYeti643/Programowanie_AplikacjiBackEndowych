using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AppCore.Interfaces;
using Infrastructure.Memory;
using AppCore.Models;
using AppCore.Module;
using AppCore.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Infrastructure;
using System.Text.Json.Serialization;
using Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace WebApi;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddDbContext<ContactsDbContext>(Options => Options.UseSqlite(builder.Configuration.GetConnectionString("CrmDb")));
        builder.Services.AddAuthorization();
        builder.Services.AddContactsEfModule(builder.Configuration);
        builder.Services.AddContactsCoreModule(builder.Configuration);
        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
        builder.Services.AddFluentValidationAutoValidation();
        builder.Services.AddValidatorsFromAssemblyContaining<CreatePersonDtoValidator>();
        builder.Services.AddExceptionHandler<ProblemDetailsExceptionHandler>();    
        builder.Services.AddProblemDetails();
        builder.Services.AddSwaggerGen();
        builder.Services.AddSingleton<JwtSettings>();
        builder.Services.AddJwt(new JwtSettings(builder.Configuration));//Dodawanie danych


        var app = builder.Build();
        app.UseExceptionHandler(); // ta warstwa musi być przed mapowaniem kontrolerów
       
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            
            // Wywołanie seederów w trybie Development
            using var scope = app.Services.CreateScope();
            
            // Pobieramy wszystkie seedery i sortujemy po Order
            var seeders = scope.ServiceProvider
                .GetServices<IDataSeeder>()
                .OrderBy(s => s.Order)
                .ToList();

            Console.WriteLine($"Znaleziono {seeders.Count} seederów");
            
            foreach (var seeder in seeders)
            {
                Console.WriteLine($"Uruchamiam seeder: {seeder.GetType().Name}");
                await seeder.SeedAsync();
            }
        }
        
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();                        
        await app.RunAsync();
    }
    
}