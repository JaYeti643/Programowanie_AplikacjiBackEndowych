using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppCore.Interfaces;
using AppCore.Models;

namespace AppCore.Services;

public class CompanyService(IContactUnitOfWork unitOfWork) : ICompanyService
{
    public async Task<CompanyDto> CreateCompanyAsync(CreateCompanyDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ArgumentException("Company name is required.");

        // Validate NIP if provided
        if (!string.IsNullOrWhiteSpace(dto.NIP) && !NIP.IsValid(dto.NIP))
            throw new ArgumentException($"Invalid NIP format: {dto.NIP}");

        var company = new Company
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            NIP = dto.NIP,
            REGON = dto.REGON,
            KRS = dto.KRS,
            Industry = dto.Industry,
            EmployeeCount = dto.EmployeeCount,
            AnnualRevenue = dto.AnnualRevenue,
            Website = dto.Website,
            Email = dto.Email,
            Phone = dto.Phone,
            Status = ContactStatus.Active,
            CreatedAt = DateTime.UtcNow,
            Employees = new List<Person>(),
            Tags = new List<Tag>(),
            Notes = new List<Note>(),
            Address = dto.Address != null ? new Address
            {
                Street = dto.Address.Street,
                City = dto.Address.City,
                PostalCode = dto.Address.PostalCode,
                Country = dto.Address.Country,
                Type = dto.Address.Type
            } : new Address()
        };

        var added = await unitOfWork.Companies.AddAsync(company);
        await unitOfWork.SaveChangesAsync();

        return CompanyDto.FromEntity(added);
    }

    public async Task<CompanyDto> GetCompanyByIdAsync(Guid id)
    {
        var company = await unitOfWork.Companies.FindByIdAsync(id);
        if (company == null)
            throw new KeyNotFoundException($"Company with id {id} not found.");

        return CompanyDto.FromEntity(company);
    }

    public async Task<CompanyDto> UpdateCompanyAsync(Guid id, UpdateCompanyDto dto)
    {
        var company = await unitOfWork.Companies.FindByIdAsync(id);
        if (company == null)
            throw new KeyNotFoundException($"Company with id {id} not found.");

        // Update only provided fields
        if (!string.IsNullOrEmpty(dto.Name))
            company.Name = dto.Name;
        
        if (!string.IsNullOrEmpty(dto.NIP))
        {
            if (!NIP.IsValid(dto.NIP))
                throw new ArgumentException($"Invalid NIP format: {dto.NIP}");
            company.NIP = dto.NIP;
        }
        
        if (!string.IsNullOrEmpty(dto.REGON))
            company.REGON = dto.REGON;
        
        if (!string.IsNullOrEmpty(dto.KRS))
            company.KRS = dto.KRS;
        
        if (!string.IsNullOrEmpty(dto.Industry))
            company.Industry = dto.Industry;
        
        if (dto.EmployeeCount.HasValue)
            company.EmployeeCount = dto.EmployeeCount.Value;
        
        if (dto.AnnualRevenue.HasValue)
            company.AnnualRevenue = dto.AnnualRevenue;
        
        if (!string.IsNullOrEmpty(dto.Website))
            company.Website = dto.Website;
        
        if (!string.IsNullOrEmpty(dto.Email))
            company.Email = dto.Email;
        
        if (!string.IsNullOrEmpty(dto.Phone))
            company.Phone = dto.Phone;
        
        if (dto.Status.HasValue)
            company.Status = dto.Status.Value;
        
        if (dto.Address != null)
        {
            company.Address = new Address
            {
                Street = dto.Address.Street,
                City = dto.Address.City,
                PostalCode = dto.Address.PostalCode,
                Country = dto.Address.Country,
                Type = dto.Address.Type
            };
        }

        company.UpdatedAt = DateTime.UtcNow;

        var updated = await unitOfWork.Companies.UpdateAsync(company);
        await unitOfWork.SaveChangesAsync();

        return CompanyDto.FromEntity(updated);
    }

    public async Task<bool> DeleteCompanyAsync(Guid id)
    {
        var company = await unitOfWork.Companies.FindByIdAsync(id);
        if (company == null)
            throw new KeyNotFoundException($"Company with id {id} not found.");

        // Remove all employees from company
        if (company.Employees != null && company.Employees.Count > 0)
        {
            foreach (var employee in company.Employees.ToList())
            {
                company.RemoveEmployee(employee);
            }
        }

        await unitOfWork.Companies.RemoveByIdAsync(id);
        await unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<PagedResult<CompanyDto>> GetAllCompaniesAsync(int page = 1, int pageSize = 10)
    {
        var companies = await unitOfWork.Companies.FindPagedAsync(page, pageSize);
        var items = companies.Items.Select(c => CompanyDto.FromEntity(c));

        return new PagedResult<CompanyDto>(
            items,
            companies.TotalCount,
            companies.Page,
            companies.PageSize);
    }

    public async Task<IEnumerable<CompanyDto>> SearchByNameAsync(string nameOrPart)
    {
        if (string.IsNullOrWhiteSpace(nameOrPart))
            return new List<CompanyDto>();

        var companies = await unitOfWork.Companies.FindByNameAsync(nameOrPart);
        return companies.Select(c => CompanyDto.FromEntity(c));
    }

    public async Task<CompanyDto?> FindByNipAsync(string nip)
    {
        if (string.IsNullOrWhiteSpace(nip))
            return null;

        var company = await unitOfWork.Companies.FindByNipAsync(nip);
        return company != null ? CompanyDto.FromEntity(company) : null;
    }

    public async Task<List<PersonDto>> GetEmployeesAsync(
        Guid companyId,
        string? firstName = null,
        string? lastName = null,
        string? position = null,
        DateTime? hiredFrom = null,
        DateTime? hiredTo = null,
        EmployeeSortBy? sortBy = null,
        bool descending = false)
    {
        var company = await unitOfWork.Companies.FindByIdAsync(companyId);
        if (company == null)
            throw new KeyNotFoundException($"Company with id {companyId} not found.");

        var employees = company.GetEmployees(
            lastName: lastName,
            position: position,
            hiredFrom: hiredFrom,
            hiredTo: hiredTo,
            sortBy: sortBy,
            descending: descending);

        return employees.Select(e => new PersonDto
        {
            Id = e.Id,
            FirstName = e.FirstName,
            LastName = e.LastName,
            Email = e.Email,
            Phone = e.Phone,
            Position = e.Position,
            BirthDate = e.BirthDate,
            Gender = e.Gender,
            Status = e.Status,
            CreatedAt = e.CreatedAt,
            EmployerId = e.Employer?.Id
        }).ToList();
    }

    public async Task<List<PersonDto>> SearchEmployeesAsync(
        Guid companyId,
        string? firstName = null,
        string? lastName = null,
        string? position = null)
    {
        var company = await unitOfWork.Companies.FindByIdAsync(companyId);
        if (company == null)
            throw new KeyNotFoundException($"Company with id {companyId} not found.");

        var employees = company.SearchEmployees(
            firstName: firstName,
            lastName: lastName,
            position: position);

        return employees.Select(e => new PersonDto
        {
            Id = e.Id,
            FirstName = e.FirstName,
            LastName = e.LastName,
            Email = e.Email,
            Phone = e.Phone,
            Position = e.Position,
            BirthDate = e.BirthDate,
            Gender = e.Gender,
            Status = e.Status,
            CreatedAt = e.CreatedAt,
            EmployerId = e.Employer?.Id
        }).ToList();
    }

    public async Task AddEmployeeAsync(Guid companyId, Guid personId)
    {
        var company = await unitOfWork.Companies.FindByIdAsync(companyId);
        if (company == null)
            throw new KeyNotFoundException($"Company with id {companyId} not found.");

        var person = await unitOfWork.Persons.FindByIdAsync(personId);
        if (person == null)
            throw new KeyNotFoundException($"Person with id {personId} not found.");

        company.AddEmployee(person);
        
        var updated = await unitOfWork.Companies.UpdateAsync(company);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task RemoveEmployeeAsync(Guid companyId, Guid personId)
    {
        var company = await unitOfWork.Companies.FindByIdAsync(companyId);
        if (company == null)
            throw new KeyNotFoundException($"Company with id {companyId} not found.");

        var person = await unitOfWork.Persons.FindByIdAsync(personId);
        if (person == null)
            throw new KeyNotFoundException($"Person with id {personId} not found.");

        var removed = company.RemoveEmployee(person);
        if (!removed)
            throw new InvalidOperationException($"Person {personId} is not an employee of company {companyId}.");

        var updated = await unitOfWork.Companies.UpdateAsync(company);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task<int> GetEmployeeCountAsync(Guid companyId)
    {
        var company = await unitOfWork.Companies.FindByIdAsync(companyId);
        if (company == null)
            throw new KeyNotFoundException($"Company with id {companyId} not found.");

        return company.Employees?.Count ?? 0;
    }
}

