using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TasksManagement_API.Models;
/// <summary>
/// Représente une formation dans le système.
/// </summary>
public class Formation : Tache
{
	[Required]
	[DataType(DataType.Date)]
	public DateTime StartDate1 { get; set; }

	[Required]
	[DataType(DataType.Date)]
	public DateTime EndDate1 { get; set; }
	
	[Required]
	[StringLength(100)]
	public DateTime Lieu { get; set; }
}