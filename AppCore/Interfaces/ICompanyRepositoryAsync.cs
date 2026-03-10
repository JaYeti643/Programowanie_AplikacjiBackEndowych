using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppCore.Models;

namespace AppCore.Interfaces;

public interface ICompanyRepositoryAsync : IGenericRepositoryAsync<Company>
{
    Task<IEnumerable<Company>> FindByNameAsync(string nameOrPart);
    Task<Company?> FindByNipAsync(string nip);
    Task<IEnumerable<Person>> FindEmployeesAsync (Guid companyId);
}