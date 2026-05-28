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
        var result = _data.Values.Where(c => !string.IsNullOrWhiteSpace(c.Name) && c.Name.Contains(nameOrPart ?? string.Empty, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(result);
    }

    // Returns company matching provided NIP (sanitized)
    public Task<Company?> FindByNipAsync(string nip)
    {
        if (string.IsNullOrWhiteSpace(nip)) return Task.FromResult<Company?>(null);

        var sanitized = NIP.Sanitize(nip);
        var company = _data.Values.FirstOrDefault(c => !string.IsNullOrWhiteSpace(c.NIP) && NIP.Sanitize(c.NIP) == sanitized);
        return Task.FromResult(company);
    }

    // Returns employees of the company (if present)
    public Task<IEnumerable<Person>> FindEmployeesAsync(Guid companyId)
    {
        var company = _data.TryGetValue(companyId, out var c) ? c : null;
        if (company == null || company.Employees == null) return Task.FromResult<IEnumerable<Person>>(Array.Empty<Person>());
        return Task.FromResult<IEnumerable<Person>>(company.Employees);
    }
}
