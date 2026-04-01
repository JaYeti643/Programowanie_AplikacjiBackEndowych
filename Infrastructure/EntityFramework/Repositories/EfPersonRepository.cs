using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppCore.Interfaces;
using AppCore.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.EntityFramework.Repositories;

public class EfPersonRepository(ContactsDbContext context) : 
    EfGenericRepository<Person>(context.People), 
    IPersonRepositoryAsync
{
    public async Task<IEnumerable<Person>> FindByCompanyAsync(Guid companyId)
    {
        return await context.People
            .Where(p => p.Employer != null && p.Employer.Id == companyId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Person>> FindByOrganizationAsync(Guid organizationId)
    {
        return await context.People
            .Where(p => p.Organization != null && p.Organization.Id == organizationId)
            .ToListAsync();
    }
}
