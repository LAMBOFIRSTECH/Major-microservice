using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using VaultSharp;
using VaultSharp.V1.AuthMethods.Token;
using VaultSharp.V1.Commons;

namespace TasksManagement_API.Middlewares;

public class JwtBearerAuthenticationMiddleware : AuthenticationHandler<JwtBearerOptions>
{
	private readonly IConfiguration configuration;
	//private RsaSecurityKey rsaSecurityKey;
	private readonly ILogger<IConfiguration> log;

	public JwtBearerAuthenticationMiddleware(ILogger<IConfiguration> log, IConfiguration configuration, IOptionsMonitor<JwtBearerOptions> options,
	ILoggerFactory logger,
	UrlEncoder encoder,
	ISystemClock clock)
	: base(options, logger, encoder, clock)
	{
		this.configuration = configuration;
		this.log = log;

	}
	// rsaSecurityKey= await GetOrCreateSigningKey();

	protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
	{

		if (!Request.Headers.ContainsKey("Authorization"))
			return await Task.FromResult(AuthenticateResult.Fail("Authorization header missing"));
		try
		{
			var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
			if (!authHeader.Scheme.Equals("Bearer", StringComparison.OrdinalIgnoreCase))
			{
				return await Task.FromResult(AuthenticateResult.Fail("Invalid authentication scheme"));
			}
			// Récupérer le jeton JWT à partir de l'en-tête d'autorisation
			var jwtToken = authHeader.Parameter;
			if (string.IsNullOrEmpty(jwtToken))
			{
				return AuthenticateResult.Fail("Token is missing.");
			}

			var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = Options.TokenValidationParameters;
            validationParameters.IssuerSigningKey = await GetSigningKeyFromVaultServer();
			var principal = tokenHandler.ValidateToken(jwtToken, validationParameters, out SecurityToken securityToken);
			var ticket = new AuthenticationTicket(principal, Scheme.Name);
			return await Task.FromResult(AuthenticateResult.Success(ticket));

		}
		catch (Exception ex)
		{
			return await Task.FromResult(AuthenticateResult.Fail($"Authentication failed: {ex.Message}"));
		}
	}

	private async Task<RsaSecurityKey> GetSigningKeyFromVaultServer()
	{

		var hashiCorpToken = configuration["HashiCorp:VaultToken"];
		var hashiCorpHttpClient = configuration["HashiCorp:HttpClient:BaseAddress"];
		if (string.IsNullOrEmpty(hashiCorpToken) || string.IsNullOrEmpty(hashiCorpHttpClient))
		{
			log.LogWarning("La configuration de HashiCorp Vault est manquante ou invalide.");
			throw new InvalidOperationException("La configuration de HashiCorp Vault est manquante ou invalide.");
		}
		var authMethod = new TokenAuthMethodInfo(hashiCorpToken);
		var vaultClientSettings = new VaultClientSettings($"{hashiCorpHttpClient}", authMethod);
		var vaultClient = new VaultClient(vaultClientSettings);
		try
		{
			var secretPath = configuration["HashiCorp:SecretPath"];
			//var secretPath = "secret/auth-service"; // La façon de récupérer le secretPath n'est pas identique entre l'envoie et le récupération du secret dans vault
			var secret = await vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync(secretPath);
			if (secret == null)
			{
				log.LogError("Le secret Vault est introuvable.");
				throw new InvalidOperationException("Le secret Vault est introuvable.");
			}
			var secretData = secret.Data.Data;
			if (!secretData.ContainsKey("authenticationPublicKey"))
			{
				log.LogError("La clé publique 'authenticationPublicKey' est manquante dans le secret Vault.");
				throw new InvalidOperationException("La clé publique 'authenticationPublicKey' est introuvable.");
			}
			string rawPublicKeyPem = secretData["authenticationPublicKey"].ToString()!;

			// Étape 2 : Nettoyer la clé (enlever les espaces ou caractères supplémentaires autour)
			rawPublicKeyPem = rawPublicKeyPem.Trim(); // Supprimer espaces inutiles

			// Vérifier que la clé contient bien les balises PEM
			if (!rawPublicKeyPem.Contains("-----BEGIN RSA PUBLIC KEY-----") ||
				!rawPublicKeyPem.Contains("-----END RSA PUBLIC KEY-----"))
			{
				log.LogWarning("La clé récupérée n'a pas le bon format PEM.");
				throw new Exception("La clé récupérée n'a pas le bon format PEM.");
			}

			// Extraire uniquement le contenu de la clé entre les balises
			string keyBody = rawPublicKeyPem
				.Replace("-----BEGIN RSA PUBLIC KEY-----", "")
				.Replace("-----END RSA PUBLIC KEY-----", "")
				.Replace("\r", "") // Supprimer retours chariot (Windows)
				.Replace("\n", "") // Supprimer sauts de ligne

				.Trim(); // Nettoyage final des espaces en début/fin

			// Vérifier que le contenu n'est pas vide
			if (string.IsNullOrEmpty(keyBody))
			{
				throw new Exception("Le contenu de la clé est vide après le nettoyage.");
			}

			// Réinsérer les balises et formater la clé avec des lignes de 64 caractères
			string formattedPublicKeyPem = "-----BEGIN RSA PUBLIC KEY-----\n" +
				string.Join("\n", Enumerable.Range(0, (keyBody.Length + 63) / 64)
					.Select(i => keyBody.Substring(i * 64, Math.Min(64, keyBody.Length - i * 64)))) +
				"\n-----END RSA PUBLIC KEY-----";

			// Étape 3 : Importer la clé dans un objet RSA
			var rsa = RSA.Create();
			rsa.ImportFromPem(formattedPublicKeyPem);

			// Étape 4 : Créer un RsaSecurityKey
			var rsaSecurityKey = new RsaSecurityKey(rsa);
			log.LogInformation("La clé publique a été récupérée et formatée avec succès.");

			return rsaSecurityKey;
		}
		catch (FormatException ex)
		{
			// Gérer les erreurs de format Base64
			log.LogError($"Erreur lors de la conversion de la clé publique Base64 : {ex.Message}");
			throw;
		}
		catch (Exception ex)
		{
			log.LogError($"Erreur lors de la récupération de la clé publique dans Vault : {ex.Message}");
			throw;
		}
	}
}
