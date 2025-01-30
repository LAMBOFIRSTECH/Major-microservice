using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TasksManagement_API.Models;
/// <summary>
/// Représente une fonction dans le système.
/// </summary>
public class Fonction
{
	[Key]
	public Guid ID { get; set; }
	
	[Required]
	[StringLength(100)]
	public string? Libelle { get; set; }
}
