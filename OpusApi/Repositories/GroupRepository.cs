using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using OpusApi.DbModels;

namespace OpusApi.Repositories;

public class GroupRepository(SqliteDbContext dbContext) : IEntityRepository<GroupEntity>
{
    public async Task<GroupEntity?> GetByIdAsync(Guid id)
    {
        return await dbContext.Groups
            .Include(x => x.Searchers)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<GroupEntity>?> GetAllAsync()
    {
        return await dbContext.Groups
            .Include(x => x.Searchers)
            .ToListAsync();
    }

    public async Task<IEnumerable<GroupEntity>?> FindAsync(Expression<Func<GroupEntity, bool>> predicate)
    {
        return await dbContext.Groups
            .Include(x => x.Searchers)
            .Where(predicate)
            .ToListAsync();
    }

    public async Task AddAsync(GroupEntity entity)
    {
        dbContext.Groups.Add(entity);
        await dbContext.SaveChangesAsync();
    }

    public async Task AddRangeAsync(List<GroupEntity> entities)
    {
        await dbContext.Groups.AddRangeAsync(entities);
        await dbContext.SaveChangesAsync();
    }

    public async Task Update(GroupEntity entity)
    {
        var existingEntity = await dbContext.Groups.FindAsync(entity.Id);
        if (existingEntity != null)
        {
            dbContext.Entry(existingEntity).CurrentValues.SetValues(entity);
            await dbContext.SaveChangesAsync();
        }
    }

    public async Task Delete(Guid id)
    {
        var entity = await dbContext.Groups.FindAsync(id);
        if (entity != null)
        {
            dbContext.Remove(entity);
            await dbContext.SaveChangesAsync();
        }
    }

    public Task<bool> ExistsAsync(Expression<Func<GroupEntity, bool>> predicate)
    {
        return dbContext.Groups.AnyAsync(predicate);
    }

    public Task<int> CountAsync(Expression<Func<GroupEntity, bool>>? predicate = null)
    {
        var query = dbContext.Groups.AsQueryable();
        if (predicate != null)
        {
            query = query.Where(predicate);
        }
        return query.CountAsync();
    }
}
