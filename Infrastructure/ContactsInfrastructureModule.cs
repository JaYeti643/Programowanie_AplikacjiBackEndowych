using System;
using AppCore;
using AppCore.Authorization;
using AppCore.Interfaces;
using AppCore.Services;
using Infrastructure.EntityFramework.Entities;
using Infrastructure.EntityFramework.Repositories;
using Infrastructure.EntityFramework.UnitOfWork;
using Infrastructure.Memory;
using Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure;

public static class ContactsInfrastructureModule
{
    public static IServiceCollection AddContactsEfModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<ICompanyRepositoryAsync, EfCompanyRepository>();
        services.AddScoped<IPersonRepositoryAsync, EfPersonRepository>();
        services.AddScoped<IOrganizationRepositoryAsync, EfOrganizationRepository>();
        //zarejestruj pozostałe repozytoria podobnie jak wyżej
        services.AddScoped<IContactUnitOfWork, EfContactsUnitOfWork>();
        services.AddDbContext<ContactsDbContext>(options =>
            options.UseSqlite(
                configuration.GetConnectionString("CrmDb")));
	        
        services.AddIdentity<CrmUser, CrmRole>(options =>
            {
                options.Password.RequiredLength         = 8;
                options.Password.RequireUppercase       = true;
                options.Password.RequireNonAlphanumeric = true;
                options.User.RequireUniqueEmail         = true;
                options.User.AllowedUserNameCharacters  = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.SignIn.RequireConfirmedEmail    = false;
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan  = TimeSpan.FromMinutes(15);
            })
            .AddEntityFrameworkStores<ContactsDbContext>()
            .AddDefaultTokenProviders();
        services.AddScoped<IPersonService, PersonService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IDataSeeder, IdentityDbSeeder>();
        services.AddScoped<IDataSeeder, PersonSeeder>();
        return services;
    }

    public static IServiceCollection AddContactsMemoryModule(
        this IServiceCollection services)
    {
        services.AddSingleton<IPersonRepositoryAsync, MemoryPersonRepository>();
        services.AddSingleton<ICompanyRepositoryAsync, MemoryCompanyRepository>();
        services.AddSingleton<IOrganizationRepositoryAsync, MemoryOrganizationRepository>();
        services.AddSingleton<IContactUnitOfWork, MemoryContactUnitOfWork>();
        services.AddSingleton<IPersonService, PersonService>();
        return services;
    }
    
    public static IServiceCollection AddJwt(this IServiceCollection services, JwtSettings jwtOptions)
    {
        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtOptions.Issuer,
                        ValidAudience = jwtOptions.Audience,
                        IssuerSigningKey = jwtOptions.GetSymmetricKey(),
                        ClockSkew = TimeSpan.Zero // brak tolerancji czasu
                    };
                }
            );
        services.AddAuthorization(options =>
        {
            // Polityki oparte o role
            // metoda RequireRole akceptuje dowolną liczbę parametrów typu string
            options.AddPolicy(CrmPolicies.AdminOnly.ToString(), policy =>
                policy.RequireRole(UserRole.Administrator.ToString()));


            // dodaj politykę dla CrmPolicies.SalesAccess
            // która wymaga użytkownika z jedną z ról: Administrator, SalesManager i Salesperson
            options.AddPolicy(CrmPolicies.SalesAccess.ToString(), policy =>
                policy.RequireRole(UserRole.Administrator.ToString(), UserRole.SalesManager.ToString(), UserRole.Salesperson.ToString()));
    
            // dodaj politykę dla SalesManagerAccess
            // która wymaga róli Administratora lub SalesManagera
            options.AddPolicy(CrmPolicies.SalesManagerAccess.ToString(), policy=>
                policy.RequireRole(UserRole.Administrator.ToString(), UserRole.SalesManager.ToString()));
            // dodaj politykę dla SuppportAccess
            // która wymaga roli administratora lub SupportAgent'a
            options.AddPolicy(CrmPolicies.SupportAccess.ToString(), policy =>
                policy.RequireRole(UserRole.Administrator.ToString(), UserRole.SupportAgent.ToString()));
           
            // dodaj politykę ReadOnlyAccess
            // która wymaga dowolnej roli
            options.AddPolicy(CrmPolicies.ReadOnlyAccess.ToString(), policy =>
                policy.RequireRole(UserRole.Administrator.ToString(), UserRole.SalesManager.ToString(), UserRole.Salesperson.ToString(), UserRole.SupportAgent.ToString()));
           

            // Polityka złożona — wymaga roli i aktywnego konta
            options.AddPolicy(CrmPolicies.ActiveUser.ToString(), policy =>
                policy
                    .RequireAuthenticatedUser()
                    .RequireClaim("status", SystemUserStatus.Active.ToString()));

            // Polityka oparta o dział
            options.AddPolicy(CrmPolicies.SalesDepartment.ToString(), policy =>
                policy.RequireClaim("department", "Sales"));

            // Domyślna polityka — każdy zalogowany użytkownik
            options.DefaultPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();

            // FallbackPolicy ustawiona na null - endpointy bez [Authorize] są publiczne
            // Atrybut [AllowAnonymous] pozwala na dostęp bez autoryzacji
            options.FallbackPolicy = null;
        });
        return services;
    }
}
