// --- Contact ---

using System;
using System.Collections.Generic;
using System.Linq;
using AppCore;
using AppCore.Models;
using System.Text.Json.Serialization;



public abstract record ContactBaseDto
{
    public Guid Id { get; init; }
    public string Email { get; init; }
    public string Phone { get; init; }
    public AddressDto Address { get; init; }
    public ContactStatus Status { get; init; }
    public List<string> Tags { get; init; } = new();
    public DateTime CreatedAt { get; init; }
}

public record AddressDto(
    string Street,
    string City,
    string PostalCode,
    string Country,
    AddressType Type
);

// --- Person ---
public record PersonDto : ContactBaseDto
{
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string? Position { get; init; }
    public DateTime? BirthDate { get; init; }
    public Gender Gender { get; init; }
    public Guid? EmployerId { get; init; }
    public string Notes { get; set; }

    public static PersonDto FromEntity(Person person) => new()
    {
        Id = person.Id,
        FirstName = person.FirstName,
        LastName = person.LastName,
        Email = person.Email,
        Phone = person.Phone,
        Position = person.Position,
        BirthDate = person.BirthDate,
        Gender = person.Gender,
        EmployerId = person.Employer?.Id,
        Status = person.Status,
        CreatedAt = person.CreatedAt,
        Tags = person.Tags?.Select(t => t.Name).ToList() ?? new(),
        Address = person.Address != null ? new AddressDto(
            person.Address.Street,
            person.Address.City,
            person.Address.PostalCode,
            person.Address.Country,
            person.Address.Type
        ) : null
    };

}

public record CreatePersonDto(
    string FirstName,
    string LastName,
    string Email,
    string Phone,
    string? Position,
    DateTime? BirthDate,
    Gender Gender,
    Guid? EmployerId,
    AddressDto? Address
);

public record UpdatePersonDto(
    string? FirstName,
    string? LastName,
    string? Email,
    string? Phone,
    string? Position,
    DateTime? BirthDate,
    Gender? Gender,
    Guid? EmployerId,
    AddressDto? Address,
    ContactStatus? Status
);

public record ContactSearchDto(
    string? Query,
    ContactStatus? Status,
    string? Tag,
    string? ContactType,
    int Page = 1,
    int PageSize = 20
);


public record PagedResult<T>(
    IEnumerable<T> Items,
    int TotalCount,
    int Page,
    int PageSize
)
{


    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasNext => Page < TotalPages;
    public bool HasPrevious => Page > 1;
}

public record CreateNoteDto(
    string Content,
    string? CreatedBy = null
);

public record NoteDto(
    Guid Id,
    string Content,
    DateTime CreatedAt,
    string? CreatedBy
);


public record LoginDto
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}
	
public record AuthResponseDto
{
    public string AccessToken { get; init; } = string.Empty;
    public string RefreshToken { get; init; } = string.Empty;
    public DateTime ExpiresAt { get; init; }
    public UserDto User { get; init; } = null!;
}
	
	
public record RefreshTokenDto(
    string AccessToken,
    string RefreshToken
);

public record UserDto
{
    public string Id { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public string Department { get; init; } = string.Empty;
    public List<string> Roles { get; init; } = new();
    public SystemUserStatus Status { get; init; }
    public DateTime CreatedAt { get; init; }
}

// --- Company ---
public record CompanyDto : ContactBaseDto
{
    public string Name { get; init; }
    public string NIP { get; init; }
    public string REGON { get; init; }
    public string KRS { get; init; }
    public string Industry { get; init; }
    public int EmployeeCount { get; init; }
    public decimal? AnnualRevenue { get; init; }
    public string Website { get; init; }
    public List<PersonDto> Employees { get; init; } = new();
    public Guid? PrimaryContactId { get; init; }

    public static CompanyDto FromEntity(Company company) => new()
    {
        Id = company.Id,
        Name = company.Name,
        NIP = company.NIP,
        REGON = company.REGON,
        KRS = company.KRS,
        Industry = company.Industry,
        EmployeeCount = company.EmployeeCount,
        AnnualRevenue = company.AnnualRevenue,
        Website = company.Website,
        Email = company.Email,
        Phone = company.Phone,
        Status = company.Status,
        CreatedAt = company.CreatedAt,
        Tags = company.Tags?.Select(t => t.Name).ToList() ?? new(),
        Employees = company.Employees?.Select(e => new PersonDto
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
        }).ToList() ?? new(),
        PrimaryContactId = company.PrimaryContact?.Id,
        Address = company.Address != null ? new AddressDto(
            company.Address.Street,
            company.Address.City,
            company.Address.PostalCode,
            company.Address.Country,
            company.Address.Type
        ) : null
    };
}

public record CreateCompanyDto(
    string Name,
    string NIP,
    string REGON,
    string KRS,
    string Industry,
    int EmployeeCount,
    decimal? AnnualRevenue,
    string Website,
    string Email,
    string Phone,
    AddressDto? Address
);

public record UpdateCompanyDto(
    string? Name,
    string? NIP,
    string? REGON,
    string? KRS,
    string? Industry,
    int? EmployeeCount,
    decimal? AnnualRevenue,
    string? Website,
    string? Email,
    string? Phone,
    AddressDto? Address,
    ContactStatus? Status
);

public record GetCompanyEmployeesDto(
    string? FirstName = null,
    string? LastName = null,
    string? Position = null,
    DateTime? HiredFrom = null,
    DateTime? HiredTo = null,
    EmployeeSortBy? SortBy = null,
    bool Descending = false
);
