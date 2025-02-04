using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace TasksManagement_API.Models;
/// <summary>
/// Représente une tache dans le système
/// </summary>
public class Tache
{
	/// <summary>
	/// Représente l'identifiant unique d'une tache.
	/// </summary>
	[JsonIgnore]
	[Key]
	public Guid ID { get; set; }
	[Required]
	public string Titre { get; set; } = string.Empty;
	public string Summary { get; set; } = string.Empty;
	//---- plustard une tache devra etre constituée des propriétées ci-haut
	[Required(ErrorMessage = "Le format de date doit être comme l'exemple suivant : 01/01/2024")]
	[DataType(DataType.Date)]
	public DateTime StartDate { get; set; }

	[Required(ErrorMessage = "Le format de date doit être comme l'exemple suivant : 01/01/2024")]
	[DataType(DataType.Date)]
	public DateTime EndDate { get; set; }
	// Supprimer les dates ici on les récupère dans le enfants de la classe
	[JsonIgnore]
	public Guid UserId { get; set; }
	public string? NomUtilisateur { get; set; }
	public string? EmailUtilisateur { get; set; }
	[JsonIgnore]
	public Utilisateur? Utilisateur { get; set; }
}
