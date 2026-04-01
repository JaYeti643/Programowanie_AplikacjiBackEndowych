using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AppCore.Interfaces;
using Infrastructure.Memory;
using AppCore.Models;
using AppCore.Module;
using AppCore.Services;
using FluentValidation.AspNetCore;
using Infrastructure;

namespace WebApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddAuthorization();
        builder.Services.AddContactsEfModule(builder.Configuration);
        builder.Services.AddContactsCoreModule(builder.Configuration);
        builder.Services.AddControllers();
        builder.Services.AddExceptionHandler<ProblemDetailsExceptionHandler>();    
        builder.Services.AddProblemDetails();
        builder.Services.AddSwaggerGen();


        var app = builder.Build();
        app.UseExceptionHandler(); // ta warstwa musi być przed mapowaniem kontrolerów
        app.MapControllers();
       
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