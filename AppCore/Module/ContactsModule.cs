using System.ComponentModel.Design;
using AppCore.Validators;
namespace AppCore.Module;

public static class ContactsModule
{
    public static IServiceCollection AddContactsModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddValidatorsFromAssemblyContaining<CreatePersonDtoValidator>();
        return services;
    }
}

public interface IConfiguration
{
}

public interface IServiceCollection
{
    void AddValidatorsFromAssemblyContaining<T>();
}