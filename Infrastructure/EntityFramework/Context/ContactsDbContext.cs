using System;
using AppCore.Models;
using Infrastructure.EntityFramework.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class ContactsDbContext: IdentityDbContext<CrmUser, CrmRole, string>
{

    public DbSet<Person> People { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<Organization> Organizations { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=CrmDb.db");
    }

    public ContactsDbContext()
    {
    }

    public ContactsDbContext(DbContextOptions<ContactsDbContext> options) :
        base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder); // wymagane przez Identity
        
        builder.Entity<CrmUser>(entity =>
        {
            entity.Property(u => u.FirstName).HasMaxLength(100);
            entity.Property(u => u.LastName).HasMaxLength(100);
            entity.Property(u => u.Department).HasMaxLength(100);
            entity.HasIndex(u => u.Email).IsUnique();
        });
        
        builder.Entity<CrmRole>(entity =>
        {
            entity.Property(r => r.Name).HasMaxLength(20);
        });
        
        // Konfiguracji mapowania dziedziczenia TPH
        // Jedna tabela do przechowywnia wszystkich typów kontaktów
        builder.Entity<Contact>()
            .HasDiscriminator<string>("ContactType")
            .HasValue<Person>("Person")
            .HasValue<Company>("Company")
            .HasValue<Organization>("Organization");

        builder.Entity<Contact>(entity =>
        {
            entity.Property(p => p.Email).HasMaxLength(200);
            entity.Property(p => p.Phone).HasMaxLength(20);
            // dodoj ograniczenia dla pozostałych właściwości 
        });
        
        builder.Entity<Person>(entity =>
        {
            entity.Property(p => p.BirthDate).HasColumnType("date");
            entity.Property(p => p.Gender).HasConversion<string>();
            entity.Property(p => p.Status).HasConversion<string>();
            // dodaj ograniczenia dla pozostałych właściwości
        });
        
        // definicja związku
        builder.Entity<Person>()
            .HasOne(p => p.Employer)
            .WithMany(e => e.Employees);

        
        // definicja związku        
        builder.Entity<Organization>()
            .HasMany(o => o.Members)
            .WithOne(p => p.Organization);
        
        // przykładowa firma
        builder.Entity<Company>(entity =>
        {
            entity.HasData(
                new Company()
                {
                    Id = Guid.Parse("516A34D7-CCFB-4F20-85F3-62BD0F3AF271"),
                    Name = "WSEI",
                    Industry = "edukacja",
                    Phone = "123567123",
                    Email = "biuro@wsei.edu.pl",
                    Website = "https://wsei.edu.pl",
                }
            );
        });
        
        var address = new
        {
            City = "Kraków",
            Country = "Poland",
            PostalCode = "25-009",
            Street = "ul. Św. Filipa 17",
            Type = AddressType.Correspondence,
            // id osoby, która dodana jest niżej
            ContactId = Guid.Parse("3d54091d-abc8-49ec-9590-93ad3ed5458f")
        };
        
        // przykładowe kontakty typu Person
        builder.Entity<Person>(entity =>
        {
            entity.HasData(
                new
                {
                    Id = Guid.Parse("3d54091d-abc8-49ec-9590-93ad3ed5458f"),
                    FirstName = "Adam",
                    LastName = "Nowak",
                    Gender = Gender.Male,
                    Status = ContactStatus.Active,
                    Email = "adam@wsei.edu.pl",
                    Phone = "123456789",
                    BirthDate = DateTime.Parse("2001-01-11"),
                    Position = "Programista",
                    CreatedAt = DateTime.Parse("2026-04-01"),
                    UpdatedAt = DateTime.Parse("2026-04-01")
                },
                new 
                {
                    Id = Guid.Parse("B4DCB17C-F875-43F8-9D66-36597895A466"),
                    FirstName = "Ewa",
                    LastName = "Kowalska",
                    Gender = Gender.Female,
                    Status = ContactStatus.Blocked,
                    Email = "ewa@wsei.edu.pl",
                    Phone = "123123123",
                    BirthDate = DateTime.Parse("2001-01-11"),
                    Position = "Tester",
                    CreatedAt = DateTime.Parse("2026-04-01"),
                    UpdatedAt = DateTime.Parse("2026-04-01")
                });
        });
        //mapowanie adresu jako typu osadzonej w encji Contact 
        builder.Entity<Contact>()
            .OwnsOne(c => c.Address)
            .HasData(address);
    }
}