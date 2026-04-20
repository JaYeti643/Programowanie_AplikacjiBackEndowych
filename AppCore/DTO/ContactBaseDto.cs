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
