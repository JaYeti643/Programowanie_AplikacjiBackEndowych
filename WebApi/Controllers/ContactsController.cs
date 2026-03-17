using System.Threading.Tasks;
using AppCore.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("/api/contacts")]
public class ContactsController(IPersonService service) : ControllerBase
{
    [HttpGet("persons")]
    public async Task<IActionResult> GetAllPersons(int page, int size)
    {
        return Ok(await service.FindAllPeoplePaged(page, size));
    }
}