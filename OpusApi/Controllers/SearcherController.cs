using Microsoft.AspNetCore.Mvc;
using OpusApi.DbModels;
using OpusApi.Dtos;
using OpusApi.Repositories;

namespace OpusApi.Controllers;

/// <summary>
/// Управление поисковиками — участниками поисково-спасательных работ.
/// </summary>
[ApiController]
[Route("[controller]")]
[Produces("application/json")]
public class SearcherController(IEntityRepository<SearcherEntity> searcherRepository) : ControllerBase
{
    /// <summary>
    /// Возвращает список всех поисковиков.
    /// </summary>
    /// <returns>Список поисковиков.</returns>
    /// <response code="200">Список поисковиков успешно получен.</response>
    /// <response code="204">Поисковики ещё не заведены.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<SearcherResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult<IEnumerable<SearcherResponse>>> GetAll()
    {
        var searchers = await searcherRepository.GetAllAsync();

        if (searchers is null || !searchers.Any())
            return NoContent();

        return Ok(searchers.Select(s => s.ToResponse()));
    }

    /// <summary>
    /// Возвращает поисковика по идентификатору.
    /// </summary>
    /// <param name="id">Уникальный идентификатор поисковика.</param>
    /// <returns>Данные поисковика.</returns>
    /// <response code="200">Поисковик найден.</response>
    /// <response code="404">Поисковик с указанным идентификатором не найден.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(SearcherResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SearcherResponse>> GetById(Guid id)
    {
        var searcher = await searcherRepository.GetByIdAsync(id);

        if (searcher == null)
            return NotFound();

        return Ok(searcher.ToResponse());
    }

    /// <summary>
    /// Создаёт нового поисковика.
    /// </summary>
    /// <param name="request">Данные поисковика.</param>
    /// <returns>Созданный поисковик с присвоенным идентификатором.</returns>
    /// <response code="201">Поисковик успешно создан.</response>
    /// <response code="400">Данные поисковика не прошли валидацию.</response>
    [HttpPost]
    [ProducesResponseType(typeof(SearcherResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SearcherResponse>> Create([FromBody] SearcherRequest request)
    {
        var searcher = request.ToEntity();

        if (!searcher.Validate(out var error))
            return BadRequest(error);

        await searcherRepository.AddAsync(searcher);

        return CreatedAtAction(nameof(GetById), new { id = searcher.Id }, searcher.ToResponse());
    }

    /// <summary>
    /// Обновляет данные существующего поисковика.
    /// </summary>
    /// <param name="id">Идентификатор обновляемого поисковика.</param>
    /// <param name="request">Новые данные поисковика.</param>
    /// <response code="204">Поисковик успешно обновлён.</response>
    /// <response code="400">Данные поисковика не прошли валидацию.</response>
    /// <response code="404">Поисковик с указанным идентификатором не найден.</response>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] SearcherRequest request)
    {
        var searcher = await searcherRepository.GetByIdAsync(id);

        if (searcher == null)
            return NotFound();

        request.ApplyTo(searcher);

        if (!searcher.Validate(out var error))
            return BadRequest(error);

        await searcherRepository.Update(searcher);

        return NoContent();
    }

    /// <summary>
    /// Удаляет поисковика.
    /// </summary>
    /// <param name="id">Идентификатор удаляемого поисковика.</param>
    /// <response code="204">Поисковик успешно удалён.</response>
    /// <response code="404">Поисковик с указанным идентификатором не найден.</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        if (!await searcherRepository.ExistsAsync(x => x.Id == id))
            return NotFound();

        await searcherRepository.Delete(id);

        return NoContent();
    }
}
