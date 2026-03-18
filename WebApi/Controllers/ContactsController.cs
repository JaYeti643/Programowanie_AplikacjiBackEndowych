using System.Threading.Tasks;
using AppCore.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("/api/contacts")]
public class ContactsController() : ControllerBase
{
    private readonly IPersonService _service;

    public ContactsController(IPersonService service) : this()
    {
        _service = service;
    }

    [HttpGet("persons")]
    public async Task<IActionResult> GetAllPersons(int page, int size)
    {
        return Ok(await _service.FindAllPeoplePaged(page, size));
    }
}