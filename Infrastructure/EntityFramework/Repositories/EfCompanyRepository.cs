using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppCore.Interfaces;
using AppCore.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.EntityFramework.Repositories;

public class EfCompanyRepository(ContactsDbContext context) : 
    EfGenericRepository<Company>(context.Companies), 
    ICompanyRepositoryAsync
{
    public async Task<Company?> FindByNipAsync(string nip)
    {
        return await context.Companies.FirstOrDefaultAsync(c => c.NIP == nip);
    }

    public async Task<IEnumerable<Company>> FindByNameAsync(string nameOrPart)
    {
        return await context.Companies.Where(c => c.Name.Contains(nameOrPart)).ToListAsync();
    }

    public async Task<IEnumerable<Person>> FindEmployeesAsync(Guid companyId)
    {
        return await context.People.Where(p => p.Employer != null && p.Employer.Id == companyId).ToListAsync();
    }
}
