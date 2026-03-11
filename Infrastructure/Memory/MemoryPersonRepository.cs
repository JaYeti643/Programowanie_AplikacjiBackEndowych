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
        _data.Add(Guid.NewGuid(), new Person()
        {
            FirstName = "Adam",
            LastName = "Nowak",
            Gender = Gender.Male,
        });
        _data.Add(Guid.NewGuid(), new Person()
        {
            FirstName = "Maria",
            LastName = "Kowalska",
            Gender = Gender.Female,
        });

    }
    
    
}