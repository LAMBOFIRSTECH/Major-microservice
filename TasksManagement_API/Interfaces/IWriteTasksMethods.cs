using TasksManagement_API.Models;
namespace TasksManagement_API.Interfaces
{
	public interface IWriteTasksMethods
	{
		Task<Tache> CreateTask(Tache Tache);
		Task<Tache> UpdateTask(string username,Tache Tache);
		Task DeleteTaskByTitle(string titre);
	}
}