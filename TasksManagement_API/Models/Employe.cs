using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TasksManagement_API.Models;
/// <summary>
/// Représente un employé dans le système.
/// </summary>
public class Employe : Utilisateur
{
	[Required]
	[StringLength(100)]
	public string? Matricule { get; set; }
	
	[Required]
	public DateTime? EmploymentDate { get; set; }
	
	[Required]
	[StringLength(30)]
	public string? Grade { get; set; }
}