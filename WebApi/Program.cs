using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AppCore.Interfaces;
using Infrastructure.Memory;
using AppCore.Models;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddControllers();            
        builder.Services.AddSwaggerGen();

        builder.Services.AddSingleton<IPersonRepositoryAsync, MemoryPersonRepository>();
        builder.Services.AddSingleton<ICompanyRepositoryAsync, MemoryCompanyRepository>();
        builder.Services.AddSingleton<IOrganizationRepositoryAsync, MemoryOrganizationRepository>();
        builder.Services.AddSingleton<IContactUnitOfWork, MemoryContactUnitOfWork>();
        builder.Services.AddSingleton<IPersonService, MemoryPersonService>();

        var app = builder.Build();

       
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