using OpusApi.DbModels;

namespace OpusApi.Dtos;

/// <summary>
/// Преобразования между DTO и доменными сущностями.
/// </summary>
public static class DtoMappingExtensions
{
    // ----- Searcher -----

    public static SearcherEntity ToEntity(this SearcherRequest request) => new()
    {
        LastName = request.LastName,
        FirstName = request.FirstName,
        FatherName = request.FatherName,
        NickName = request.NickName,
        PhoneNumber = request.PhoneNumber
    };

    /// <summary>Переносит редактируемые поля запроса в существующую сущность, не трогая Id и CreatedAt.</summary>
    public static void ApplyTo(this SearcherRequest request, SearcherEntity entity)
    {
        entity.LastName = request.LastName;
        entity.FirstName = request.FirstName;
        entity.FatherName = request.FatherName;
        entity.NickName = request.NickName;
        entity.PhoneNumber = request.PhoneNumber;
    }

    public static SearcherResponse ToResponse(this SearcherEntity entity) => new()
    {
        Id = entity.Id,
        CreatedAt = entity.CreatedAt,
        LastName = entity.LastName,
        FirstName = entity.FirstName,
        FatherName = entity.FatherName,
        NickName = entity.NickName,
        PhoneNumber = entity.PhoneNumber
    };

    // ----- Group -----

    public static GroupEntity ToEntity(this GroupRequest request) => new()
    {
        Name = request.Name
    };

    /// <summary>Переносит редактируемые поля запроса в существующую группу, не трогая состав и метаданные.</summary>
    public static void ApplyTo(this GroupRequest request, GroupEntity entity)
    {
        entity.Name = request.Name;
    }

    public static GroupResponse ToResponse(this GroupEntity entity) => new()
    {
        Id = entity.Id,
        CreatedAt = entity.CreatedAt,
        Name = entity.Name,
        Searchers = entity.Searchers.Select(s => s.ToResponse()).ToList()
    };

    // ----- Incident -----

    public static IncidentEntity ToEntity(this IncidentRequest request) => new()
    {
        From = request.From,
        To = request.To,
        Message = request.Message
    };

    /// <summary>Переносит редактируемые поля запроса в существующую запись журнала, не трогая Id и CreatedAt.</summary>
    public static void ApplyTo(this IncidentRequest request, IncidentEntity entity)
    {
        entity.From = request.From;
        entity.To = request.To;
        entity.Message = request.Message;
    }

    public static IncidentResponse ToResponse(this IncidentEntity entity) => new()
    {
        Id = entity.Id,
        CreatedAt = entity.CreatedAt,
        From = entity.From,
        To = entity.To,
        Message = entity.Message
    };
}
