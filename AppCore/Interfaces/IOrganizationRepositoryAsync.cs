using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppCore.Models;

namespace AppCore.Interfaces;


public interface IOrganizationRepositoryAsync : IGenericRepositoryAsync<Organization>
{
    Task<IEnumerable<Organization>> FindByTypeAsync(string type);
    Task<IEnumerable<Person>> GetMembersAsync(Guid organizationId);
}