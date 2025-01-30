using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TasksManagement_API.Models
{
	/// <summary>
	/// Gestion du token JWT.
	/// </summary>

	public class TokenResult
	{
		public bool Success { get; set; }
		public string? Message { get; set; }
		public string? Token { get; set; }
	}
}