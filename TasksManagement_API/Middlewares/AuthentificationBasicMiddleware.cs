using TasksManagement_API.Models;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Security.Claims;
namespace TasksManagement_API.Middlewares
{
	public class AuthentificationBasicMiddleware : AuthenticationHandler<AuthenticationSchemeOptions>
	{
		private readonly DailyTasksMigrationsContext dataBaseMemoryContext;
		public AuthentificationBasicMiddleware(DailyTasksMigrationsContext dataBaseMemoryContext, IOptionsMonitor<AuthenticationSchemeOptions> options,
		ILoggerFactory logger,
		UrlEncoder encoder,
		ISystemClock clock)
		: base(options, logger, encoder, clock)
		{
			this.dataBaseMemoryContext = dataBaseMemoryContext;
		}	
		protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
		{
			if (!Request.Headers.ContainsKey("Authorization"))
				return AuthenticateResult.Fail("Authorization header missing");
			try
			{
				var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
				if (authHeader.Scheme.Equals("Basic", StringComparison.OrdinalIgnoreCase))
				{
					var credentialBytes = Convert.FromBase64String(authHeader.Parameter!);
					var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':', 2);
					var email = credentials[0];
					var password = credentials[1];
					if (await IsValidCredentials(email, password))
					{
						var claims = new[]
						{
							new Claim(ClaimTypes.Name, email),
							new Claim(ClaimTypes.Role, Utilisateur.Privilege.Utilisateur.ToString())
							};

						var identity = new ClaimsIdentity(claims, Scheme.Name);
						var principal = new ClaimsPrincipal(identity);
						// Assigner le principal à la propriété Principal de l'objet context
						var ticket = new AuthenticationTicket(principal, Scheme.Name);

						return AuthenticateResult.Success(ticket);
					}
					else
					{
						return AuthenticateResult.Fail("Invalid username or password");
					}
				}
				else
				{
					return AuthenticateResult.NoResult();
				}
			}
			catch (Exception ex)
			{
				return AuthenticateResult.Fail($"Authentication failed: {ex.Message}");
			}
		}
		private async Task<bool> IsValidCredentials(string email, string password)
		{
			var utilisateur = dataBaseMemoryContext.Utilisateurs.FirstOrDefault(u => u.Email.Equals(email));
			if (utilisateur != null)
			{
				return utilisateur.CheckHashPassword(password);
			}
			await Task.Delay(1000);
			return false;
		}  
	}
}