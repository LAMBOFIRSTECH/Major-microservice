using TasksManagement_API.Models;
using Microsoft.AspNetCore.DataProtection;
namespace TasksManagement_API.Reposirtories;
public class UtilisateurRepository
{
    private readonly DailyTasksMigrationsContext dataBaseSqlServerContext;
    private readonly IDataProtectionProvider provider;
    private const string Purpose = "my protection purpose";
    public UtilisateurRepository(DailyTasksMigrationsContext dataBaseSqlServerContext, IDataProtectionProvider provider)
    {
        this.dataBaseSqlServerContext = dataBaseSqlServerContext;
        this.provider = provider;
    }
    public IQueryable<Utilisateur> GetUsers()
    {
        return dataBaseSqlServerContext.Utilisateurs;
    }
    public string EncryptUserSecret(string plainText)
    {
        var protector = provider.CreateProtector(Purpose);
        return protector.Protect(plainText);
    }

    public string DecryptUserSecret(string cipherText)
    {
        var protector = provider.CreateProtector(Purpose);
        return protector.Unprotect(cipherText);
    }
    public async Task<Utilisateur> CreateUser(Utilisateur utilisateur)
    {
        await dataBaseSqlServerContext.Utilisateurs.AddAsync(utilisateur);
        await dataBaseSqlServerContext.SaveChangesAsync();
        return utilisateur;
    }
    public async Task SetUserPassword(Utilisateur utilisateur)
    {
        await dataBaseSqlServerContext.SaveChangesAsync();
    }
    public async Task DeleteUserByDetails(string nom, Utilisateur.Privilege role)
    {
        var utilisateur = dataBaseSqlServerContext.Utilisateurs.Where(user => user.Nom == nom && user.Role == role).FirstOrDefault();
        dataBaseSqlServerContext.Utilisateurs.Remove(utilisateur!);
        await dataBaseSqlServerContext.SaveChangesAsync();
    }
}
