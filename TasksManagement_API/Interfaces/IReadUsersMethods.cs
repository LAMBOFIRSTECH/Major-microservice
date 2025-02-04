using TasksManagement_API.Models;
namespace TasksManagement_API.Interfaces;
public interface IReadUsersMethods
{
    bool CheckUserSecret(string secretPass);
    Task<ICollection<Utilisateur>> GetUsers(Func<IQueryable<Utilisateur>, IQueryable<Utilisateur>>? filter = null, bool includeTasks = false);
    Task<Utilisateur?> GetSingleUserByNameRole(string nom, Utilisateur.Privilege role);
    Task<string?> CheckExistedUser(Utilisateur Utilisateur);
}
