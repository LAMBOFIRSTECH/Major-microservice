using TasksManagement_API.Models;
namespace TasksManagement_API.Interfaces
{
	public interface IReadTasksMethods
	{
		Task<ICollection<Tache>> GetSingleOrAllTaches(Func<IQueryable<Tache>, IQueryable<Tache>>? filter = null);
	}
}