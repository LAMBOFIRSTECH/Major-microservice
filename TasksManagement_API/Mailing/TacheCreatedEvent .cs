using TasksManagement_API.Interfaces;
using TasksManagement_API.Models;

namespace TasksManagement_API.Mailing
{
	public class TacheCreatedEvent : IEvent
	{
		public Tache Tache { get; }

		public TacheCreatedEvent(Tache tache)
		{
			Tache = tache;
			
		}


		public DateTime Timestamp => throw new NotImplementedException();
	}
}