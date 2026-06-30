using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using OpusApi.DbModels;

namespace OpusApi.Repositories;

public class IncidentRepository(SqliteDbContext dbContext) : IEntityRepository<IncidentEntity>
{
    public async Task<IncidentEntity?> GetByIdAsync(Guid id)
    {
        return await dbContext.Incidents.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<IncidentEntity>?> GetAllAsync()
    {
        return await dbContext.Incidents
            .OrderBy(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<IncidentEntity>?> FindAsync(Expression<Func<IncidentEntity, bool>> predicate)
    {
        return await dbContext.Incidents.Where(predicate).ToListAsync();
    }

    public async Task AddAsync(IncidentEntity entity)
    {
        dbContext.Incidents.Add(entity);
        await dbContext.SaveChangesAsync();
    }

    public async Task AddRangeAsync(List<IncidentEntity> entities)
    {
        await dbContext.Incidents.AddRangeAsync(entities);
        await dbContext.SaveChangesAsync();
    }

    public async Task Update(IncidentEntity entity)
    {
        var existingEntity = await dbContext.Incidents.FindAsync(entity.Id);
        if (existingEntity != null)
        {
            dbContext.Entry(existingEntity).CurrentValues.SetValues(entity);
            await dbContext.SaveChangesAsync();
        }
    }

    public async Task Delete(Guid id)
    {
        var entity = await dbContext.Incidents.FindAsync(id);
        if (entity != null)
        {
            dbContext.Remove(entity);
            await dbContext.SaveChangesAsync();
        }
    }

    public Task<bool> ExistsAsync(Expression<Func<IncidentEntity, bool>> predicate)
    {
        return dbContext.Incidents.AnyAsync(predicate);
    }

    public Task<int> CountAsync(Expression<Func<IncidentEntity, bool>>? predicate = null)
    {
        var query = dbContext.Incidents.AsQueryable();
        if (predicate != null)
        {
            query = query.Where(predicate);
        }
        return query.CountAsync();
    }
}
