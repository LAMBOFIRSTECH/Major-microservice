using TasksManagement_API.Interfaces;
using TasksManagement_API.Models;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using TasksManagement_API.Repositories;
namespace TasksManagement_API.Services
{
    public class UtilisateurService : IReadUsersMethods, IWriteUsersMethods
	{
		private readonly UtilisateurRepository utilisateurRepository;
		private readonly IConfiguration configuration;
		private readonly IDataProtectionProvider provider;
		private const string Purpose = "my protection purpose";
		public UtilisateurService(IConfiguration configuration, UtilisateurRepository utilisateurRepository, IDataProtectionProvider provider)
		{
			this.configuration = configuration;
			this.utilisateurRepository = utilisateurRepository;
			this.provider = provider;
		}
		public bool CheckUserSecret(string secretPass)
		{
			//Env.Load("Services/.env");
			string secretUserPass = configuration["ConnectionStrings:SecretApiKey"]; // Dev configuration["JwtSettings:JwtSecretKey"]; // Prod Environment.GetEnvironmentVariable("PasswordSecret")!; //

			if (string.IsNullOrEmpty(secretUserPass))
			{
				throw new ArgumentException("La clé secrete est inexistante");
			}
			var Pass = BCrypt.Net.BCrypt.HashPassword(secretPass);
			var BCryptResult = BCrypt.Net.BCrypt.Verify(secretUserPass, Pass);
			if (!BCryptResult.Equals(true)) 
				return false;
			return true;
		}
		public async Task<ICollection<Utilisateur>> GetUsers(Func<IQueryable<Utilisateur>, IQueryable<Utilisateur>>? filter = null, bool includeTasks = false)
		{
			IQueryable<Utilisateur> query = utilisateurRepository.GetUsers();
			if (includeTasks)
			{
				query = query.Include(u => u.LesTaches);
			}
			if (filter != null)
			{
				query = filter(query);
			}
			return await query.ToListAsync();
		}
		public async Task<Utilisateur?> GetSingleUserByNameRole(string nom, Utilisateur.Privilege role)
		{ return (await GetUsers(query => query.Where(user => user.Nom == nom && user.Role == role))).FirstOrDefault(); }

		public async Task<string?> CheckExistedUser(Utilisateur utilisateur)
		{
			var listUtilisateurs = (await GetUsers()).ToList();
			var utilisateurExistant = await GetSingleUserByNameRole(utilisateur.Nom, utilisateur.Role);
			if (utilisateurExistant == null)
			{
				return utilisateur.Nom;
			}
			var i = 1;
			var nouveauNomUtilisateur = $"{utilisateur.Nom}_{i}";
			if (utilisateurExistant.Nom.Equals(utilisateur.Nom))
			{
				while (listUtilisateurs.Any(item => item.Nom == nouveauNomUtilisateur))
				{
					i++;
					nouveauNomUtilisateur = $"{utilisateur.Nom}_{i}";
				}
			}
			utilisateur.Nom = nouveauNomUtilisateur;
			return utilisateur.Nom;
		}
		public async Task<Utilisateur> CreateUser(Utilisateur utilisateur)
		{
			if (utilisateur.CheckHashPassword(utilisateur.Pass) && utilisateur.CheckEmailAdress(utilisateur.Email))
			{
				PrepareUserTasks(utilisateur);
			}
			return await utilisateurRepository.CreateUser(utilisateur);
		}
		private void PrepareUserTasks(Utilisateur utilisateur)
		{
			// Si l'utilisateur a des tâches, on les associe à l'utilisateur avant l'insertion
			if (utilisateur.LesTaches?.Count > 0)
			{
				foreach (var tache in utilisateur.LesTaches)
				{
					tache.NomUtilisateur = utilisateur.Nom;
					tache.EmailUtilisateur = utilisateur.Email;
				}
			}
		}
		public async Task SetUserPassword(string nom, string mdp)
		{
			var utilisateur = (await GetUsers(query => query.Where(u => u.Nom!.Equals(nom)))).FirstOrDefault();
			utilisateur!.Pass = utilisateur.SetHashPassword(mdp);
			await utilisateurRepository.SetUserPassword(utilisateur);
		}
		public async Task DeleteUserByDetails(string nom, Utilisateur.Privilege role)
		{
			await utilisateurRepository.DeleteUserByDetails(nom, role);
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
		public async Task<ICollection<Utilisateur>> GetUserByAnyFilter(Func<IQueryable<Utilisateur>, IQueryable<Utilisateur>>? filter = null, bool includeTasks = false)
		{
			IQueryable<Utilisateur> query = (IQueryable<Utilisateur>)GetUsers(filter);//utilisateurRepository.GetUsers(filter);
			if (includeTasks)
			{
				query = query.Include(u => u.LesTaches);
			}
			if (filter != null)
			{
				query = filter(query);
			}
			return await query.ToListAsync();
		}
	}
}