using Microsoft.AspNetCore.Mvc;
using OpusApi.DbModels;

namespace OpusApi.Controllers;

[ApiController]
[Route("[controller]")]
public class ConnectionController(InMemoryDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public ActionResult<List<ConnectionEntity>> GetAll()
    {
        var connections = dbContext.Connections.ToList();
        if (connections.Count == 0)
        {
            return NoContent();
        }

        return Ok(connections);
    }
}