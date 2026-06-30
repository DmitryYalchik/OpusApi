using Microsoft.AspNetCore.Mvc;
using OpusApi.DbModels;
using OpusApi.Dtos;
using OpusApi.Notifications;
using OpusApi.Repositories;

namespace OpusApi.Controllers;

/// <summary>
/// Журнал связи — записи входящих и исходящих сообщений оператора узла связи.
/// </summary>
[ApiController]
[Route("[controller]")]
[Produces("application/json")]
public class IncidentController(
    IEntityRepository<IncidentEntity> incidentRepository,
    IEntityNotifier notifier) : ControllerBase
{
    /// <summary>
    /// Возвращает все записи журнала связи в хронологическом порядке.
    /// </summary>
    /// <returns>Список записей журнала.</returns>
    /// <response code="200">Записи журнала успешно получены.</response>
    /// <response code="204">Журнал пуст.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<IncidentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult<IEnumerable<IncidentResponse>>> GetAll()
    {
        var incidents = await incidentRepository.GetAllAsync();

        if (incidents is null || !incidents.Any())
            return NoContent();

        return Ok(incidents.Select(i => i.ToResponse()));
    }

    /// <summary>
    /// Возвращает запись журнала по идентификатору.
    /// </summary>
    /// <param name="id">Уникальный идентификатор записи.</param>
    /// <returns>Запись журнала.</returns>
    /// <response code="200">Запись найдена.</response>
    /// <response code="404">Запись с указанным идентификатором не найдена.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(IncidentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IncidentResponse>> GetById(Guid id)
    {
        var incident = await incidentRepository.GetByIdAsync(id);

        if (incident == null)
            return NotFound();

        return Ok(incident.ToResponse());
    }

    /// <summary>
    /// Добавляет новую запись в журнал связи.
    /// </summary>
    /// <param name="request">Данные записи журнала.</param>
    /// <returns>Созданная запись с присвоенным идентификатором.</returns>
    /// <response code="201">Запись успешно добавлена.</response>
    /// <response code="400">Данные записи не прошли валидацию.</response>
    [HttpPost]
    [ProducesResponseType(typeof(IncidentResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IncidentResponse>> Create([FromBody] IncidentRequest request)
    {
        var incident = request.ToEntity();

        if (!incident.Validate(out var error))
            return BadRequest(error);

        await incidentRepository.AddAsync(incident);
        await notifier.EntityCreatedAsync("Incident", incident.Id);

        return CreatedAtAction(nameof(GetById), new { id = incident.Id }, incident.ToResponse());
    }

    /// <summary>
    /// Обновляет существующую запись журнала.
    /// </summary>
    /// <param name="id">Идентификатор обновляемой записи.</param>
    /// <param name="request">Новые данные записи.</param>
    /// <response code="204">Запись успешно обновлена.</response>
    /// <response code="400">Данные записи не прошли валидацию.</response>
    /// <response code="404">Запись с указанным идентификатором не найдена.</response>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] IncidentRequest request)
    {
        var incident = await incidentRepository.GetByIdAsync(id);

        if (incident == null)
            return NotFound();

        request.ApplyTo(incident);

        if (!incident.Validate(out var error))
            return BadRequest(error);

        await incidentRepository.Update(incident);
        await notifier.EntityUpdatedAsync("Incident", incident.Id);

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
        await notifier.EntityDeletedAsync("Incident", id);

        return NoContent();
    }
}
