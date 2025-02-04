using TasksManagement_API.Models;
using Microsoft.EntityFrameworkCore;
using TasksManagement_API.DataBaseContext;
namespace TasksManagement_API.Repositories;
public class TacheRepository
{
    private readonly DailyTasksMigrationsContext dataBaseSqlServerContext;
    public TacheRepository(DailyTasksMigrationsContext dataBaseSqlServerContext)
    {
        this.dataBaseSqlServerContext = dataBaseSqlServerContext;
    }
    public async Task<ICollection<Tache>> GetSingleOrAllTaches(Func<IQueryable<Tache>, IQueryable<Tache>> filter = null!)
    {
        IQueryable<Tache> query = dataBaseSqlServerContext.Taches;
        if (filter != null)
        {
            query = filter(query);
        }
        return await query.ToListAsync();
    }
    public async Task<Tache> CreateTask(Tache tache)
    {
        await dataBaseSqlServerContext.Taches.AddAsync(tache);
        await dataBaseSqlServerContext.SaveChangesAsync();
        return tache;
    }
    public async Task DeleteTaskByTitle(string titre)
    {
        var Tache = dataBaseSqlServerContext.Taches.SingleOrDefault(t => t.Titre.Equals(titre));
        dataBaseSqlServerContext.Taches.Remove(Tache!);
        await dataBaseSqlServerContext.SaveChangesAsync();
    }
    public async Task<Tache> UpdateTask(Tache tache)
    {
        await dataBaseSqlServerContext.SaveChangesAsync(); return tache;
    }
}