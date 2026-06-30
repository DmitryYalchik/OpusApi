using Microsoft.AspNetCore.Mvc;
using OpusApi.DbModels;
using OpusApi.Repositories;

namespace OpusApi.Controllers;

[ApiController]
[Route("[controller]")]
public class SearcherController(SearcherRepository searcherRepository) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<SearcherEntity>>> GetAll()
    {
        if (await searcherRepository.CountAsync() == 0)
            return NoContent();

        return Ok(await searcherRepository.GetAllAsync());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<List<SearcherEntity>>> GetById(Guid id)
    {
        var searcher = await searcherRepository.GetByIdAsync(id);

        if (searcher == null)
            return NotFound();

        return Ok(searcher);
    }
}