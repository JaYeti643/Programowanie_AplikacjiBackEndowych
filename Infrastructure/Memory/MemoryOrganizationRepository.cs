using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppCore.Interfaces;
using AppCore.Models;

namespace Infrastructure.Memory;

public class MemoryOrganizationRepository : MemoryGenericRepository<Organization>, IOrganizationRepositoryAsync
{
    public Task<IEnumerable<Organization>> FindByTypeAsync(string type)
    {
        var result = _data.Values.Where(o => o.Type.ToString().Contains(type, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(result);
    }

    public Task<IEnumerable<Person>> GetMembersAsync(Guid organizationId)
    {
        // Assuming organizations have members, but since not implemented, return empty
        return Task.FromResult(Enumerable.Empty<Person>());
    }
}
