using System;
using System.Linq;
using System.Threading.Tasks;
using AppCore.Interfaces;
using AppCore.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Security;

public class PersonSeeder : IDataSeeder
{
    private readonly ContactsDbContext _context;

    public PersonSeeder(ContactsDbContext context)
    {
        _context = context;
    }

    public int Order => 2;

    public async Task SeedAsync()
    {
        // Sprawdź czy są już osoby w bazie
        if (await _context.People.AnyAsync())
        {
            return;
        }

        var persons = new[]
        {
            new Person
            {
                Id = Guid.NewGuid(),
                FirstName = "Anna",
                LastName = "Nowak",
                Email = "anna.nowak@example.com",
                Phone = "+48 123 456 789",
                Position = "Manager",
                Gender = Gender.Female,
                BirthDate = new DateTime(1985, 5, 15),
                Status = ContactStatus.Active,
                CreatedAt = DateTime.UtcNow
            },
            new Person
            {
                Id = Guid.NewGuid(),
                FirstName = "Piotr",
                LastName = "Wiśniewski",
                Email = "piotr.wisniewski@example.com",
                Phone = "+48 987 654 321",
                Position = "Developer",
                Gender = Gender.Male,
                BirthDate = new DateTime(1990, 8, 22),
                Status = ContactStatus.Active,
                CreatedAt = DateTime.UtcNow
            },
            new Person
            {
                Id = Guid.NewGuid(),
                FirstName = "Maria",
                LastName = "Kowalczyk",
                Email = "maria.kowalczyk@example.com",
                Phone = "+48 555 666 777",
                Position = "Analyst",
                Gender = Gender.Female,
                BirthDate = new DateTime(1988, 3, 10),
                Status = ContactStatus.Active,
                CreatedAt = DateTime.UtcNow
            }
        };

        await _context.People.AddRangeAsync(persons);
        await _context.SaveChangesAsync();
    }
}

