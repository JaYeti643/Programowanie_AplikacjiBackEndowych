using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppCore.Interfaces;
using AppCore.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.EntityFramework.Repositories;

public class EfOrganizationRepository(ContactsDbContext context) : 
    EfGenericRepository<Organization>(context.Organizations), 
    IOrganizationRepositoryAsync
{
    public async Task<IEnumerable<Organization>> FindByTypeAsync(string type)
    {
        return await context.Organizations.Where(o => o.Type.ToString() == type).ToListAsync();
    }

    public async Task<IEnumerable<Person>> GetMembersAsync(Guid organizationId)
    {
        return await context.People.Where(p => p.Organization != null && p.Organization.Id == organizationId).ToListAsync();
    }
}
