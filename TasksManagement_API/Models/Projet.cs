using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TasksManagement_API.Models;
/// <summary>
/// Représente un projet dans le système.
/// </summary>
public class Projet
{
	[Key]
	public Guid ID { get; set; }
	
	[StringLength(20)]
	public string? Code { get; set; }

	[Required]
	[StringLength(100)]
	public string? Nom { get; set; }
	
	[DataType(DataType.Date)]
	public DateTime? StartDate { get; set; }
	
	public int? Duree { get; set; }
	// Propriété de navigation après
}