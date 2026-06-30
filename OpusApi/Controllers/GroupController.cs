using Microsoft.AspNetCore.Mvc;
using OpusApi.DbModels;
using OpusApi.Repositories;

namespace OpusApi.Controllers;

/// <summary>
/// Управление группами — объединениями поисковиков на поиске.
/// </summary>
[ApiController]
[Route("[controller]")]
[Produces("application/json")]
public class GroupController(GroupRepository groupRepository) : ControllerBase
{
    /// <summary>
    /// Возвращает список всех групп вместе с входящими в них поисковиками.
    /// </summary>
    /// <returns>Список групп.</returns>
    /// <response code="200">Список групп успешно получен.</response>
    /// <response code="204">Группы ещё не созданы.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<GroupEntity>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult<IEnumerable<GroupEntity>>> GetAll()
    {
        if (await groupRepository.CountAsync() == 0)
            return NoContent();

        return Ok(await groupRepository.GetAllAsync());
    }

    /// <summary>
    /// Возвращает группу по идентификатору вместе с входящими в неё поисковиками.
    /// </summary>
    /// <param name="id">Уникальный идентификатор группы.</param>
    /// <returns>Данные группы.</returns>
    /// <response code="200">Группа найдена.</response>
    /// <response code="404">Группа с указанным идентификатором не найдена.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(GroupEntity), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GroupEntity>> GetById(Guid id)
    {
        var group = await groupRepository.GetByIdAsync(id);

        if (group == null)
            return NotFound();

        return Ok(group);
    }

    /// <summary>
    /// Создаёт новую группу.
    /// </summary>
    /// <param name="group">Данные группы.</param>
    /// <returns>Созданная группа с присвоенным идентификатором.</returns>
    /// <response code="201">Группа успешно создана.</response>
    /// <response code="400">Данные группы не прошли валидацию.</response>
    [HttpPost]
    [ProducesResponseType(typeof(GroupEntity), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<GroupEntity>> Create([FromBody] GroupEntity group)
    {
        if (!group.Validate(out var error))
            return BadRequest(error);

        await groupRepository.AddAsync(group);

        return CreatedAtAction(nameof(GetById), new { id = group.Id }, group);
    }

    /// <summary>
    /// Обновляет данные существующей группы.
    /// </summary>
    /// <param name="id">Идентификатор обновляемой группы.</param>
    /// <param name="group">Новые данные группы.</param>
    /// <response code="204">Группа успешно обновлена.</response>
    /// <response code="400">Данные группы не прошли валидацию.</response>
    /// <response code="404">Группа с указанным идентификатором не найдена.</response>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] GroupEntity group)
    {
        group.Id = id;

        if (!group.Validate(out var error))
            return BadRequest(error);

        if (!await groupRepository.ExistsAsync(x => x.Id == id))
            return NotFound();

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
