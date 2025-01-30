using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TasksManagement_API.Mailing;
using TasksManagement_API.Models;
using TasksManagement_API.Reposirtories;

namespace TasksManagement_API.Services
{
	public class NotificationService
	{
		private readonly EventBus eventBus;

		private readonly UtilisateurService utilisateurService ;
		public NotificationService(EventBus eventBus, UtilisateurService utilisateurService)
		{
			this.eventBus = eventBus;
			this.utilisateurService = utilisateurService;
			// C'est ici qu'on va chercher à s'absonner à l'évènement TacheCreatedEvent
			eventBus.Subscribe<TacheCreatedEvent>(async e => await OnTaskCreated(e));
		}
		private async Task OnTaskCreated(TacheCreatedEvent taskCreatedEvent)
		{
			var tache = taskCreatedEvent.Tache;

			var userFilter = await GetUserTasksByFilter(tache);
			var utilisateur = (await utilisateurService.GetUsers(userFilter, includeTasks: true)).FirstOrDefault();

			if (utilisateur == null)
			{
				throw new ArgumentException("L'utilisateur associé à la tâche n'existe pas.");
			}

			utilisateur.LesTaches!.Add(tache);
			tache.utilisateur = utilisateur;
		}
		public async Task<Func<IQueryable<Utilisateur>, IQueryable<Utilisateur>>> GetUserTasksByFilter(Tache tache)
		{
			await Task.Delay(200);
			return query => query.Where(u => u.Nom.Equals(tache.NomUtilisateur) && u.Email.Equals(tache.EmailUtilisateur));
		}
	}
}