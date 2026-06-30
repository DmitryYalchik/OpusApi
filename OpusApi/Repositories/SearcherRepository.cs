using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using OpusApi.DbModels;

namespace OpusApi.Repositories;

public class SearcherRepository(SqliteDbContext dbContext) : IEntityRepository<SearcherEntity>
{
    public async Task<SearcherEntity?> GetByIdAsync(Guid id)
    {
        var searcher = await FindAsync(x => x.Id == id);
        return searcher?.FirstOrDefault();
    }

    public async Task<IEnumerable<SearcherEntity>?> GetAllAsync()
    {
        return await dbContext.Searchers.ToListAsync();
    }

    public async Task<IEnumerable<SearcherEntity>?> FindAsync(Expression<Func<SearcherEntity, bool>> predicate)
    {
        return await dbContext.Searchers.Where(predicate).ToListAsync();
    }

    public async Task AddAsync(SearcherEntity entity)
    {
        dbContext.Searchers.Add(entity);
        await dbContext.SaveChangesAsync();
    }

    public async Task AddRangeAsync(List<SearcherEntity> entities)
    {
        await dbContext.Searchers.AddRangeAsync(entities);
        await dbContext.SaveChangesAsync();
    }

    public async Task Update(SearcherEntity entity)
    {
        var existingEntity = await dbContext.Searchers.FindAsync(entity.Id);
        if (existingEntity != null)
        {
            dbContext.Entry(existingEntity).CurrentValues.SetValues(entity);
            await dbContext.SaveChangesAsync();
        }
    }

    public async Task Delete(Guid id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            dbContext.Remove(entity);
            await dbContext.SaveChangesAsync();
        }
    }

    public Task<bool> ExistsAsync(Expression<Func<SearcherEntity, bool>> predicate)
    {
        return dbContext.Searchers.AnyAsync(predicate);
    }

    public Task<int> CountAsync(Expression<Func<SearcherEntity, bool>>? predicate = null)
    {
        var query = dbContext.Searchers.AsQueryable();
        if (predicate != null)
        {
            query = query.Where(predicate);
        }
        return query.CountAsync();
    }
}