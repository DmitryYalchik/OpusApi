using Microsoft.AspNetCore.Mvc;
using OpusApi.DbModels;
using OpusApi.Dtos;
using OpusApi.Repositories;

namespace OpusApi.Controllers;

/// <summary>
/// Управление группами — объединениями поисковиков на поиске.
/// </summary>
[ApiController]
[Route("[controller]")]
[Produces("application/json")]
public class GroupController(IEntityRepository<GroupEntity> groupRepository) : ControllerBase
{
    /// <summary>
    /// Возвращает список всех групп вместе с входящими в них поисковиками.
    /// </summary>
    /// <returns>Список групп.</returns>
    /// <response code="200">Список групп успешно получен.</response>
    /// <response code="204">Группы ещё не созданы.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<GroupResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult<IEnumerable<GroupResponse>>> GetAll()
    {
        var groups = await groupRepository.GetAllAsync();

        if (groups is null || !groups.Any())
            return NoContent();

        return Ok(groups.Select(g => g.ToResponse()));
    }

    /// <summary>
    /// Возвращает группу по идентификатору вместе с входящими в неё поисковиками.
    /// </summary>
    /// <param name="id">Уникальный идентификатор группы.</param>
    /// <returns>Данные группы.</returns>
    /// <response code="200">Группа найдена.</response>
    /// <response code="404">Группа с указанным идентификатором не найдена.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(GroupResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GroupResponse>> GetById(Guid id)
    {
        var group = await groupRepository.GetByIdAsync(id);

        if (group == null)
            return NotFound();

        return Ok(group.ToResponse());
    }

    /// <summary>
    /// Создаёт новую группу.
    /// </summary>
    /// <param name="request">Данные группы.</param>
    /// <returns>Созданная группа с присвоенным идентификатором.</returns>
    /// <response code="201">Группа успешно создана.</response>
    /// <response code="400">Данные группы не прошли валидацию.</response>
    [HttpPost]
    [ProducesResponseType(typeof(GroupResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<GroupResponse>> Create([FromBody] GroupRequest request)
    {
        var group = request.ToEntity();

        if (!group.Validate(out var error))
            return BadRequest(error);

        await groupRepository.AddAsync(group);

        return CreatedAtAction(nameof(GetById), new { id = group.Id }, group.ToResponse());
    }

    /// <summary>
    /// Обновляет данные существующей группы.
    /// </summary>
    /// <param name="id">Идентификатор обновляемой группы.</param>
    /// <param name="request">Новые данные группы.</param>
    /// <response code="204">Группа успешно обновлена.</response>
    /// <response code="400">Данные группы не прошли валидацию.</response>
    /// <response code="404">Группа с указанным идентификатором не найдена.</response>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] GroupRequest request)
    {
        var group = await groupRepository.GetByIdAsync(id);

        if (group == null)
            return NotFound();

        request.ApplyTo(group);

        if (!group.Validate(out var error))
            return BadRequest(error);

        await groupRepository.Update(group);

        return NoContent();
    }

    /// <summary>
    /// Удаляет группу.
    /// </summary>
    /// <param name="id">Идентификатор удаляемой группы.</param>
    /// <response code="204">Группа успешно удалена.</response>
    /// <response code="404">Группа с указанным идентификатором не найдена.</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        if (!await groupRepository.ExistsAsync(x => x.Id == id))
            return NotFound();

        await groupRepository.Delete(id);

        return NoContent();
    }
}
