using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppCore.Interfaces;
using AppCore.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("/api/contacts")]
public class ContactsController(IPersonService service): ControllerBase
{

    [HttpGet]
    public  async Task<IActionResult> GetAllPersons(int page = 1, int size = 10)
    {
        return Ok(await service.FindAllPeoplePaged(page, size));
    }
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetPerson(Guid id)
    {
        try
        {
            var dto = await service.GetById(id);
            return Ok(dto);

        }
        catch (ContactNotFoundException)
        {
            return NotFound();
        }
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(CreatePersonDto dto)
    {
        var result = await service.AddPerson(dto);
        return CreatedAtAction(nameof(GetPerson), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdatePerson(Guid id, UpdatePersonDto dto)
    {
        
        try
        {
            await service.GetById(id);
        }
        catch (ContactNotFoundException)
        {
            return NotFound();
        }

     
        var updatedPerson = await service.UpdatePerson(dto);
        var personDto = PersonDto.FromEntity(updatedPerson);
        return Ok(personDto);
    }
    [HttpPost("{contactId:guid}/notes")]
    [ProducesResponseType(typeof(NoteDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddNote(
        [FromRoute] Guid contactId,
        [FromBody] CreateNoteDto dto)
    {
        
        
            var note = await service.AddNoteToPerson(contactId, dto);
            return CreatedAtAction(
                nameof(GetNotes),
                new { contactId },
                note);
        
      
    }

    [HttpGet("{contactId:guid}/notes")]
    [ProducesResponseType(typeof(IEnumerable<NoteDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetNotes([FromRoute] Guid contactId)
    {
        try
        {
            var notes = await service.GetNotesForPerson(contactId);
            return Ok(notes);
        }
        catch (ContactNotFoundException)
        {
            return NotFound();
        }
        catch (Exception)
        {
            return BadRequest();
        }
    }

    [HttpDelete("{contactId:guid}/notes/{noteId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteNote(
        [FromRoute] Guid contactId,
        [FromRoute] Guid noteId)
    {
        try
        {
            var result = await service.DeleteNote(contactId, noteId);
            if (result)
            {
                return NoContent();
            }
            return BadRequest();
        }
        catch (ContactNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
