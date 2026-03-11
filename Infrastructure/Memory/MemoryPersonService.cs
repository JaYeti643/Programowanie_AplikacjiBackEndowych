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
        return new PagedResult<PersonDto>(,people.TotalCount,people.Page, people.PageSize);
    }

    public Task<IAsyncEnumerable<PersonDto>> FindPeopleFromCompany(Guid companyId)
    {
        throw new NotImplementedException();
    }

    public Task<PersonDto> DeletePerson(Guid personId)
    {
        throw new NotImplementedException();
    }

    public Task<PersonDto> AddPerson(PersonDto person)
    {
        throw new NotImplementedException();
    }

    public Task<PersonDto> AddNote(Guid personId, Note note)
    {
        throw new NotImplementedException();
    }

    public Task<PersonDto> UpdatePerson(Guid personId, PersonDto person)
    {
        throw new NotImplementedException();
    }

    public Task<PersonDto> AddTag(Guid personId, Guid tagId)
    {
        throw new NotImplementedException();
    }

    Task<PagedResult<PersonDto>> IPersonService.FindAllPeoplePaged(int page, int pageSize)
    {
        throw new NotImplementedException();
    }
}
