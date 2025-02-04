using TasksManagement_API.Interfaces;
using TasksManagement_API.Mailing;
using TasksManagement_API.Models;
using TasksManagement_API.Repositories;

#nullable disable
namespace TasksManagement_API.Services
{
    public class TacheService : IReadTasksMethods, IWriteTasksMethods
	{
		private readonly TacheRepository tacheRepository;
		private readonly EventBus eventBus;
		public TacheService(TacheRepository tacheRepository, EventBus eventBus)
		{
			this.tacheRepository = tacheRepository;
			this.eventBus = eventBus;
		}
		public async Task<ICollection<Tache>> GetSingleOrAllTaches(Func<IQueryable<Tache>, IQueryable<Tache>> filter = null)
		{
			return await tacheRepository.GetSingleOrAllTaches(filter);
		}
		public async Task<Tache> CreateTask(Tache tache)
		{
			// var userFilter = await GetUserTasksByFilter(tache);
			// var utilisateur = (await utilisateurService.GetUsers(userFilter, includeTasks: true)).FirstOrDefault();
			// if (utilisateur == null)
			// {
			// 	throw new ArgumentException("L'utilisateur associé à la tâche n'existe pas.");
			// }
			// utilisateur.LesTaches.Add(tache);
			// tache.utilisateur = utilisateur;  // Associer la tâche à l'utilisateur grace à la navigation directe et inverse
			var createdTask = await tacheRepository.CreateTask(tache); // Qaund la tache est crée on déclenche un évènement
			var taskCreatedEvent = new TacheCreatedEvent(createdTask);
			eventBus.Publish(taskCreatedEvent);
			return createdTask;
		}
		public async Task DeleteTaskByTitle(string titre)
		{
			var Taches = await GetSingleOrAllTaches(query => query.Where(t => t.Titre.Equals(titre)));
			if (Taches.FirstOrDefault() == null)
			{
				throw new ArgumentException("La tache demandée n'existe pas.");
			}
			await tacheRepository.DeleteTaskByTitle(titre);
		}
		public async Task<Tache> UpdateTask(string username, Tache tache)
		{
			var newTache = (await GetSingleOrAllTaches(query => query.Where(t => t.NomUtilisateur.Equals(username)))).First();
			newTache.Titre = tache.Titre;
			newTache.Summary = tache.Summary;
			newTache.StartDate = tache.StartDate;
			newTache.EndDate = tache.EndDate;
			return await tacheRepository.UpdateTask(tache);
		}
	}
}