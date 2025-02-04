using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace TasksManagement_API.Models;
/// <summary>
/// Représente un utilisateur dans le système.
/// </summary>
public class Utilisateur
{
	/// <summary>
	/// Représente l'identifiant unique d'un utilisateur.
	/// </summary>
	[Key]
	public Guid ID { get; set; }
	[Required]
	public string Nom { get; set; } = string.Empty;

	[Required]
	public string Email { get; set; } = string.Empty;
	public enum Privilege { Administrateur, Utilisateur }
	[EnumDataType(typeof(Privilege))]
	[Required]
	public Privilege Role { get; set; }
	[Required]
	[Category("Security")]
	public string Pass { get; set; } = string.Empty;
	[System.Text.Json.Serialization.JsonIgnore]
	public ICollection<Tache>? LesTaches { get; set; } // On doit aussi etre capable de récupérer la liste des taches du user dans l'affichage
	public bool CheckHashPassword(string password)
	{
		return BCrypt.Net.BCrypt.Verify(password, Pass);
	}
	public string SetHashPassword(string password)
	{
		if (!string.IsNullOrEmpty(password))
		{
			Pass = BCrypt.Net.BCrypt.HashPassword($"{password}");
		}
		return Pass!;
	}
	public bool CheckEmailAdress(string email)
	{
		const string regexMatch = "(?<alpha>\\w+)@(?<mailing>[aA-zZ]+)\\.(?<domaine>[aA-zZ]+$)";
		if (string.IsNullOrEmpty(email))
		{
			return false;
		}
		Match check = Regex.Match(email, regexMatch);
		return check.Success;
	}
}