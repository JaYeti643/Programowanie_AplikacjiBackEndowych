using AppCore.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AppCore.Module;

public static class ContactsModule
{
    public static IServiceCollection AddContactsCoreModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Rejestracja walidatorów
        services.AddValidatorsFromAssemblyContaining<CreatePersonDtoValidator>();
        services.AddFluentValidationAutoValidation();
        return services;
    }
}
