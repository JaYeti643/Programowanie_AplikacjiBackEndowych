using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppCore.Exceptions;
using AppCore.Interfaces;
using AppCore.Models;

namespace AppCore.Services;

public class PersonService(IContactUnitOfWork unitOfWork) : IPersonService
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

    public async Task<Person> AddPerson(CreatePersonDto personDto)
    {
        var entity = new Person
        {
            Id = Guid.NewGuid(),
            FirstName = personDto.FirstName,
            LastName = personDto.LastName,
            Email = personDto.Email,
            Phone = personDto.Phone,
            Position = personDto.Position,
            BirthDate = personDto.BirthDate,
            Gender = personDto.Gender,
            Status = ContactStatus.Active,
            CreatedAt = DateTime.UtcNow,
            Tags = new List<Tag>(),
            Notes = new List<Note>(),
            Address = personDto.Address != null ? new Address
            {
                Street = personDto.Address.Street,
                City = personDto.Address.City,
                PostalCode = personDto.Address.PostalCode,
                Country = personDto.Address.Country,
                Type = personDto.Address.Type
            } : new Address()
        };
        if (personDto.EmployerId.HasValue)
        {
            entity.Employer = await unitOfWork.Companies.FindByIdAsync(personDto.EmployerId.Value);
        }
        entity = await unitOfWork.Persons.AddAsync(entity);
        await unitOfWork.SaveChangesAsync();
        return entity;
    }

    public async Task<Person> UpdatePerson(UpdatePersonDto person)
    {
        var entity = new Person
        {
            Id = Guid.NewGuid(),
            FirstName = person.FirstName ?? "",
            LastName = person.LastName ?? "",
            Email = person.Email ?? "",
            Phone = person.Phone ?? "",
            Position = person.Position,
            BirthDate = person.BirthDate,
        };
        entity = await unitOfWork.Persons.UpdateAsync(entity);
        await unitOfWork.SaveChangesAsync();
        return entity;
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

    public async Task<PersonDto> GetById(Guid id)
    {
        var person = await unitOfWork.Persons.FindByIdAsync(id);
        if (person == null)
        {
            throw new KeyNotFoundException($"Person with id {id} not found.");
        }
        return new PersonDto
        {
            Id = person.Id,
            FirstName = person.FirstName,
            LastName = person.LastName,
            Email = person.Email,
            Phone = person.Phone,
            Status = person.Status,
            Position = person.Position,
            BirthDate = person.BirthDate,
            Gender = person.Gender,
            EmployerId = person.Employer?.Id
        };
    }

    public async Task<Note> AddNoteToPerson(Guid personId, CreateNoteDto noteDto)
    {
        // Pobierz osobę o podanym id z repozytorium osób
        var person = await unitOfWork.Persons.FindByIdAsync(personId);
        
        // Jeśli osoba ma wartość null to zgłoś wyjątek
        if (person == null)
        {
            throw new Exception($"Person with id {personId} not found.");
        }
        
        // Jeśli właściwość Notes osoby jest null to przypisz do niej nową listę
        if (person.Notes == null)
        {
            person.Notes = new List<Note>();
        }
        
        // Utwórz notatkę klasy Note na podstawie CreateNoteDto
        var note = new Note()
        {
            Id = Guid.NewGuid(),
            Content = noteDto.Content,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = noteDto.CreatedBy
        };
        
        // Umieść notatkę w liście Notes encji
        person.Notes.Add(note);
        
        // Wywołaj metodę aktualizacji encji na repozytorium osób
        await unitOfWork.Persons.UpdateAsync(person);
        
        // Wywołaj metodę SaveChangesAsync z UnitOfWork
        await unitOfWork.SaveChangesAsync();
        
        // Zwróć encję notatki
        return note;
    }

    public async Task<PersonDto> GetPerson(Guid personId)
    {
        var person = await unitOfWork.Persons.FindByIdAsync(personId);
        if (person == null)
        {
            throw new KeyNotFoundException($"Perssdsdsdsdsdsdsdsdon with id {personId} not found.");
        }
        
        return new PersonDto
        {
            Id = person.Id,
            FirstName = person.FirstName,
            LastName = person.LastName,
            Email = person.Email,
            Phone = person.Phone,
            Status = person.Status,
            Position = person.Position,
            BirthDate = person.BirthDate,
            Gender = person.Gender,
            EmployerId = person.Employer?.Id
        };
    }

    public async Task<List<Note>> GetNotesForPerson(Guid personId)
    {
        var person = await unitOfWork.Persons.FindByIdAsync(personId);
        if (person == null)
        {
            throw new ContactNotFoundException($"Person with id={personId} not found!");
        }
        
        return person.Notes ?? new List<Note>();
    }

    public async Task<bool> DeleteNote(Guid personId, Guid noteId)
    {
        // Pobierz osobę o podanym id
        var person = await unitOfWork.Persons.FindByIdAsync(personId);
        if (person == null)
        {
            throw new ContactNotFoundException($"Person with id={personId} not found!");
        }
        
        // Sprawdź czy osoba ma notatki
        if (person.Notes == null || person.Notes.Count == 0)
        {
            throw new Exception($"Person with id={personId} has no notes!");
        }
        
        // Znajdź notatkę do usunięcia
        var note = person.Notes.FirstOrDefault(n => n.Id == noteId);
        if (note == null)
        {
            throw new Exception($"Note with id={noteId} not found for person with id={personId}!");
        }
        
        // Usuń notatkę z listy
        person.Notes.Remove(note);
        
        // Zaktualizuj encję osoby
        await unitOfWork.Persons.UpdateAsync(person);
        
        // Zapisz zmiany
        await unitOfWork.SaveChangesAsync();
        
        return true;
    }

    Task<PagedResult<PersonDto>> IPersonService.FindAllPeoplePaged(int page, int pageSize)
    {
        return FindAllPeoplePaged(page, pageSize);
    }
    
}
