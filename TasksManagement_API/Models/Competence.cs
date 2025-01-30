using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TasksManagement_API.Models;
/// <summary>
/// Représente une compétence dans le système.
/// </summary>
public class Competence
{
	[Key]
	public Guid ID { get; set; }
	
	[Required]
	[StringLength(100)]
	public string? Libelle { get; set; }
}
