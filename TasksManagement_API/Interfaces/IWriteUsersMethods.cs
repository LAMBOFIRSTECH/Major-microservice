using TasksManagement_API.Models;
namespace TasksManagement_API.Interfaces
{
	public interface IWriteUsersMethods
	{
		string EncryptUserSecret(string plainText);
		string DecryptUserSecret(string cipherText);
		Task<Utilisateur> CreateUser(Utilisateur utilisateur);
		Task SetUserPassword(string nom, string mdp);
		Task DeleteUserByDetails(string nom, Utilisateur.Privilege role);
	}
}