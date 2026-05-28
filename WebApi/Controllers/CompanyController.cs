using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppCore.Authorization;
using AppCore.Interfaces;
using AppCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/companies")]
public class CompanyController(ICompanyService service) : ControllerBase
{
    /// <summary>
    /// Get all companies with pagination
    /// </summary>
    [HttpGet]
    [Authorize(Policy = nameof(CrmPolicies.ReadOnlyAccess))]
    [ProducesResponseType(typeof(PagedResult<CompanyDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllCompanies(int page = 1, int pageSize = 10)
    {
        var result = await service.GetAllCompaniesAsync(page, pageSize);
        return Ok(result);
    }

    /// <summary>
    /// Get company by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CompanyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCompanyById(Guid id)
    {
        try
        {
            var company = await service.GetCompanyByIdAsync(id);
            return Ok(company);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = $"Company with id {id} not found." });
        }
    }

    /// <summary>
    /// Create a new company
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CompanyDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCompany([FromBody] CreateCompanyDto dto)
    {
        try
        {
            var company = await service.CreateCompanyAsync(dto);
            return CreatedAtAction(nameof(GetCompanyById), new { id = company.Id }, company);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Update company
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(CompanyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateCompany(Guid id, [FromBody] UpdateCompanyDto dto)
    {
        try
        {
            var company = await service.UpdateCompanyAsync(id, dto);
            return Ok(company);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = $"Company with id {id} not found." });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Delete company
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCompany(Guid id)
    {
        try
        {
            await service.DeleteCompanyAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = $"Company with id {id} not found." });
        }
    }

    /// <summary>
    /// Search companies by name
    /// </summary>
    [HttpGet("search/name")]
    [ProducesResponseType(typeof(IEnumerable<CompanyDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchByName([FromQuery] string nameOrPart)
    {
        var results = await service.SearchByNameAsync(nameOrPart);
        return Ok(results);
    }

    /// <summary>
    /// Find company by NIP
    /// </summary>
    [HttpGet("search/nip")]
    [ProducesResponseType(typeof(CompanyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> FindByNip([FromQuery] string nip)
    {
        var company = await service.FindByNipAsync(nip);
        if (company == null)
            return NotFound(new { message = $"Company with NIP {nip} not found." });
        return Ok(company);
    }

    /// <summary>
    /// Get all employees of a company with optional filtering and sorting
    /// </summary>
    [HttpGet("{companyId:guid}/employees")]
    [ProducesResponseType(typeof(List<PersonDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEmployees(
        Guid companyId,
        [FromQuery] string? firstName = null,
        [FromQuery] string? lastName = null,
        [FromQuery] string? position = null,
        [FromQuery] DateTime? hiredFrom = null,
        [FromQuery] DateTime? hiredTo = null,
        [FromQuery] EmployeeSortBy? sortBy = null,
        [FromQuery] bool descending = false)
    {
        try
        {
            var employees = await service.GetEmployeesAsync(
                companyId,
                firstName,
                lastName,
                position,
                hiredFrom,
                hiredTo,
                sortBy,
                descending);
            return Ok(employees);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = $"Company with id {companyId} not found." });
        }
    }

    /// <summary>
    /// Search employees by criteria
    /// </summary>
    [HttpGet("{companyId:guid}/employees/search")]
    [ProducesResponseType(typeof(List<PersonDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SearchEmployees(
        Guid companyId,
        [FromQuery] string? firstName = null,
        [FromQuery] string? lastName = null,
        [FromQuery] string? position = null)
    {
        try
        {
            var employees = await service.SearchEmployeesAsync(
                companyId,
                firstName,
                lastName,
                position);
            return Ok(employees);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = $"Company with id {companyId} not found." });
        }
    }

    /// <summary>
    /// Add an employee to company
    /// </summary>
    [HttpPost("{companyId:guid}/employees/{personId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddEmployee(Guid companyId, Guid personId)
    {
        try
        {
            await service.AddEmployeeAsync(companyId, personId);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Remove an employee from company
    /// </summary>
    [HttpDelete("{companyId:guid}/employees/{personId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RemoveEmployee(Guid companyId, Guid personId)
    {
        try
        {
            await service.RemoveEmployeeAsync(companyId, personId);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Get employee count for a company
    /// </summary>
    [HttpGet("{companyId:guid}/employees/count")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEmployeeCount(Guid companyId)
    {
        try
        {
            var count = await service.GetEmployeeCountAsync(companyId);
            return Ok(new { count });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = $"Company with id {companyId} not found." });
        }
    }
}

