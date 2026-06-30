using Microsoft.AspNetCore.Mvc;
using OpusApi.DbModels;
using OpusApi.Repositories;

namespace OpusApi.Controllers;

/// <summary>
/// Журнал связи — записи входящих и исходящих сообщений оператора узла связи.
/// </summary>
[ApiController]
[Route("[controller]")]
[Produces("application/json")]
public class IncidentController(IncidentRepository incidentRepository) : ControllerBase
{
    /// <summary>
    /// Возвращает все записи журнала связи в хронологическом порядке.
    /// </summary>
    /// <returns>Список записей журнала.</returns>
    /// <response code="200">Записи журнала успешно получены.</response>
    /// <response code="204">Журнал пуст.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<IncidentEntity>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult<IEnumerable<IncidentEntity>>> GetAll()
    {
        if (await incidentRepository.CountAsync() == 0)
            return NoContent();

        return Ok(await incidentRepository.GetAllAsync());
    }

    /// <summary>
    /// Возвращает запись журнала по идентификатору.
    /// </summary>
    /// <param name="id">Уникальный идентификатор записи.</param>
    /// <returns>Запись журнала.</returns>
    /// <response code="200">Запись найдена.</response>
    /// <response code="404">Запись с указанным идентификатором не найдена.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(IncidentEntity), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IncidentEntity>> GetById(Guid id)
    {
        var incident = await incidentRepository.GetByIdAsync(id);

        if (incident == null)
            return NotFound();

        return Ok(incident);
    }

    /// <summary>
    /// Добавляет новую запись в журнал связи.
    /// </summary>
    /// <param name="incident">Данные записи журнала.</param>
    /// <returns>Созданная запись с присвоенным идентификатором.</returns>
    /// <response code="201">Запись успешно добавлена.</response>
    /// <response code="400">Данные записи не прошли валидацию.</response>
    [HttpPost]
    [ProducesResponseType(typeof(IncidentEntity), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IncidentEntity>> Create([FromBody] IncidentEntity incident)
    {
        if (!incident.Validate(out var error))
            return BadRequest(error);

        await incidentRepository.AddAsync(incident);

        return CreatedAtAction(nameof(GetById), new { id = incident.Id }, incident);
    }

    /// <summary>
    /// Обновляет существующую запись журнала.
    /// </summary>
    /// <param name="id">Идентификатор обновляемой записи.</param>
    /// <param name="incident">Новые данные записи.</param>
    /// <response code="204">Запись успешно обновлена.</response>
    /// <response code="400">Данные записи не прошли валидацию.</response>
    /// <response code="404">Запись с указанным идентификатором не найдена.</response>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] IncidentEntity incident)
    {
        incident.Id = id;

        if (!incident.Validate(out var error))
            return BadRequest(error);

        if (!await incidentRepository.ExistsAsync(x => x.Id == id))
            return NotFound();

        await incidentRepository.Update(incident);

        return NoContent();
    }

    /// <summary>
    /// Удаляет запись из журнала связи.
    /// </summary>
    /// <param name="id">Идентификатор удаляемой записи.</param>
    /// <response code="204">Запись успешно удалена.</response>
    /// <response code="404">Запись с указанным идентификатором не найдена.</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        if (!await incidentRepository.ExistsAsync(x => x.Id == id))
            return NotFound();

        await incidentRepository.Delete(id);

        return NoContent();
    }
}
