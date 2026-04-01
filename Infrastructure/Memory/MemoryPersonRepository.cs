using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppCore.Interfaces;
using AppCore.Models;

namespace Infrastructure.Memory;

public class MemoryPersonRepository : MemoryGenericRepository<Person>,IPersonRepositoryAsync 
{
    public async Task<IEnumerable<Person>> FindByCompanyAsync(Guid companyId)
    {
       return _data.Values.Where(p=>p.Employer != null && p.Employer.Id == companyId);
    }

    public Task<IEnumerable<Person>> FindByOrganizationAsync(Guid organizationId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Person>> SearchAsync(string query)
    {
        throw new NotImplementedException();
    }

    //Dodawanie Person do repozytorium z losowym Guidem
    public MemoryPersonRepository() : base()
    {
        var person1 = Guid.Parse("B9C5842A-FA13-4A1A-97EC-8B4C2420432E");
        _data.Add(person1, new Person()
        {
            FirstName = "Jakub",
            LastName = "Kowalski",
            Gender = Gender.Male,
            Email = "JakKo@wsei.pl",
            BirthDate = DateTime.Today.AddYears(-20),
            Phone = "48123456789",
                Address = new Address()
                {
                    Street = "ul. Kwiatowa 1",
                    City = "Warszawa",
                    PostalCode = "00-001",
                    Country = "Polska",
                    
                }
        });
        _data.Add(Guid.NewGuid(), new Person()
        {
            FirstName = "Maria",
            LastName = "Kowalska",
            Gender = Gender.Female,
        });

    }
    
    
}