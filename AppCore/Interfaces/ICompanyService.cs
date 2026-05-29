using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppCore.Models;

namespace AppCore.Interfaces;

public interface ICompanyService
{
    Task<CompanyDto> CreateCompanyAsync(CreateCompanyDto dto);
    Task<CompanyDto> GetCompanyByIdAsync(Guid id);
    Task<CompanyDto> UpdateCompanyAsync(Guid id, UpdateCompanyDto dto);
    Task<bool> DeleteCompanyAsync(Guid id);
    
    Task<PagedResult<CompanyDto>> GetAllCompaniesAsync(int page = 1, int pageSize = 10);
    
    Task<IEnumerable<CompanyDto>> SearchByNameAsync(string nameOrPart);
    Task<CompanyDto?> FindByNipAsync(string nip);
    
    Task<List<PersonDto>> GetEmployeesAsync(
        Guid companyId,
        string? firstName = null,
        string? lastName = null,
        string? position = null,
        DateTime? hiredFrom = null,
        DateTime? hiredTo = null,
        EmployeeSortBy? sortBy = null,
        bool descending = false);
    
    Task<List<PersonDto>> SearchEmployeesAsync(
        Guid companyId,
        string? firstName = null,
        string? lastName = null,
        string? position = null);
    
    Task AddEmployeeAsync(Guid companyId, Guid personId);
    Task RemoveEmployeeAsync(Guid companyId, Guid personId);
    
    Task<int> GetEmployeeCountAsync(Guid companyId);
}

