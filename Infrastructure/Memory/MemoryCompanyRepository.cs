using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppCore.Interfaces;
using AppCore.Models;

namespace Infrastructure.Memory;

public class MemoryCompanyRepository : MemoryGenericRepository<Company>, ICompanyRepositoryAsync
{
    public Task<IEnumerable<Company>> FindByNameAsync(string nameOrPart)
    {
        var result = _data.Values.Where(c => c.Name.Contains(nameOrPart, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(result);
    }

    public Task<Company> FindByNipAsync(string nip)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Person>> FindEmployeesAsync(Guid companyId)
    {
        throw new NotImplementedException();
    }
}
