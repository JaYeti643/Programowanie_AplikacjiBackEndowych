using AppCore.Models;

namespace AppCore.Interfaces;

public interface IContactRepositoryAsync : IGenericRepositoryAsync<Contact>
{
    Task<PagedResult<Contact>> SearchContactAsync(ContactSearchDto searchDto, int page, int pagesize);
    Task<IEnumerable<Contact>> SearchContactByTagAsync(Guid tagId);
    Task AddNoteAsync(Guid contactId, Note note);
    Task<IEnumerable<Note>> GetNotesAsync(Guid contactId);
    Task AddTagAsync(Guid contactId, Guid tagId);
    Task RemoveTagAsync(Guid contactId, Guid tagId);
}