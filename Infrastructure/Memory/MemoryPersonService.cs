using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppCore.Interfaces;
using AppCore.Models;

namespace Infrastructure.Memory;

public class MemoryPersonService(IContactUnitOfWork unitOfWork) : IPersonService
{
    public async Task<PagedResult<PersonDto>> FindAllPeoplePaged(int page, int pageSize)
    {
        var people = await unitOfWork.Persons.FindPagedAsync(page, pageSize);
        var items = people.Items.Select(p => new PersonDto()
            {
                Id = p.Id,
                FirstName = p.FirstName,
                LastName = p.LastName,
                Email = p.Email,
                Phone = p.Phone,
                Status = p.Status

            }
        );
        return new PagedResult<PersonDto>(items,people.TotalCount,people.Page, people.PageSize);
    }

    public async Task<IAsyncEnumerable<PersonDto>> FindPeopleFromCompany(Guid companyId)
    {
        var people = await unitOfWork.Persons.FindByCompanyAsync(companyId);
        return people.ToAsyncEnumerable().Select(p => new PersonDto
        {
            Id = p.Id,
            FirstName = p.FirstName,
            LastName = p.LastName,
            Email = p.Email,
            Phone = p.Phone,
            Status = p.Status
        });
    }

    public async Task<PersonDto> DeletePerson(Guid personId)
    {
        var person = await unitOfWork.Persons.FindByIdAsync(personId);
        if (person == null)
        {
            throw new KeyNotFoundException($"Person with id {personId} not found.");
        }
        await unitOfWork.Persons.RemoveByIdAsync(personId);
        await unitOfWork.SaveChangesAsync();
        return new PersonDto
        {
            Id = person.Id,
            FirstName = person.FirstName,
            LastName = person.LastName,
            Email = person.Email,
            Phone = person.Phone,
            Status = person.Status
        };
    }

    public async Task<PersonDto> AddPerson(PersonDto personDto)
    {
        var person = new Person
        {
            Id = Guid.NewGuid(),
            FirstName = personDto.FirstName,
            LastName = personDto.LastName,
            Email = personDto.Email,
            Phone = personDto.Phone,
            Status = personDto.Status,
            Position = personDto.Position,
            BirthDate = personDto.BirthDate,
            Gender = personDto.Gender,
            CreatedAt = DateTime.UtcNow,
            Tags = new List<Tag>(),
            Notes = new List<Note>(),
            Address = new Address()
        };
        if (personDto.EmployerId.HasValue)
        {
            var company = await unitOfWork.Companies.FindByIdAsync(personDto.EmployerId.Value);
            if (company != null)
            {
                person.Employer = company;
            }
        }
        var added = await unitOfWork.Persons.AddAsync(person);
        await unitOfWork.SaveChangesAsync();
        return new PersonDto
        {
            Id = added.Id,
            FirstName = added.FirstName,
            LastName = added.LastName,
            Email = added.Email,
            Phone = added.Phone,
            Status = added.Status,
            Position = added.Position,
            BirthDate = added.BirthDate,
            Gender = added.Gender,
            EmployerId = added.Employer?.Id
        };
    }

    public async Task<PersonDto> AddNote(Guid personId, Note note)
    {
        var person = await unitOfWork.Persons.FindByIdAsync(personId);
        if (person == null)
        {
            throw new KeyNotFoundException($"Person with id {personId} not found.");
        }
        person.Notes.Add(note);
        var updated = await unitOfWork.Persons.UpdateAsync(person);
        await unitOfWork.SaveChangesAsync();
        return new PersonDto
        {
            Id = updated.Id,
            FirstName = updated.FirstName,
            LastName = updated.LastName,
            Email = updated.Email,
            Phone = updated.Phone,
            Status = updated.Status
        };
    }

    public async Task<PersonDto> UpdatePerson(Guid personId, PersonDto personDto)
    {
        var person = await unitOfWork.Persons.FindByIdAsync(personId);
        if (person == null)
        {
            throw new KeyNotFoundException($"Person with id {personId} not found.");
        }
        person.FirstName = personDto.FirstName;
        person.LastName = personDto.LastName;
        person.Email = personDto.Email;
        person.Phone = personDto.Phone;
        person.Status = personDto.Status;
        person.Position = personDto.Position;
        person.BirthDate = personDto.BirthDate;
        person.Gender = personDto.Gender;
        if (personDto.EmployerId.HasValue)
        {
            var company = await unitOfWork.Companies.FindByIdAsync(personDto.EmployerId.Value);
            if (company != null)
            {
                person.Employer = company;
            }
        }
        var updated = await unitOfWork.Persons.UpdateAsync(person);
        await unitOfWork.SaveChangesAsync();
        return new PersonDto
        {
            Id = updated.Id,
            FirstName = updated.FirstName,
            LastName = updated.LastName,
            Email = updated.Email,
            Phone = updated.Phone,
            Status = updated.Status,
            Position = updated.Position,
            BirthDate = updated.BirthDate,
            Gender = updated.Gender,
            EmployerId = updated.Employer?.Id
        };
    }

    public async Task<PersonDto> AddTag(Guid personId, Guid tagId)
    {
        var person = await unitOfWork.Persons.FindByIdAsync(personId);
        if (person == null)
        {
            throw new KeyNotFoundException($"Person with id {personId} not found.");
        }
        var tag = new Tag { Id = tagId, Name = "", Color = "" };
        person.Tags.Add(tag);
        var updated = await unitOfWork.Persons.UpdateAsync(person);
        await unitOfWork.SaveChangesAsync();
        return new PersonDto
        {
            Id = updated.Id,
            FirstName = updated.FirstName,
            LastName = updated.LastName,
            Email = updated.Email,
            Phone = updated.Phone,
            Status = updated.Status
        };
    }

    Task<PagedResult<PersonDto>> IPersonService.FindAllPeoplePaged(int page, int pageSize)
    {
        return FindAllPeoplePaged(page, pageSize);
    }
}
