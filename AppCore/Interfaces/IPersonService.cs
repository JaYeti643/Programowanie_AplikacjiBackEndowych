using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppCore.Models;

namespace AppCore.Interfaces;

public interface IPersonService
{
    Task<PagedResult<PersonDto>> FindAllPeoplePaged(int page, int pageSize);
    Task<IAsyncEnumerable<PersonDto>> FindPeopleFromCompany(Guid companyId);
    Task<PersonDto> DeletePerson(Guid personId);
    Task<PersonDto> AddPerson(PersonDto person);
    Task<PersonDto> AddNote (Guid personId, Note note);
    Task<PersonDto> UpdatePerson(Guid personId, PersonDto person);
    Task<PersonDto> AddTag(Guid personId, Guid tagId);
    Task<PersonDto> GetById(Guid id);
    Task<Person> AddPerson(CreatePersonDto personDto);
    Task<Person> UpdatePerson(UpdatePersonDto personDto);
  
}