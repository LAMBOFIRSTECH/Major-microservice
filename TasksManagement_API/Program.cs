using System.Reflection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using TasksManagement_API.Models;
using TasksManagement_API.Interfaces;
using TasksManagement_API.Services;
using TasksManagement_API.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using TasksManagement_API.Mailing;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using TasksManagement_API.Repositories;
using TasksManagement_API.DataBaseContext;

var builder = WebApplication.CreateBuilder(args);
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
	opt.SwaggerDoc("1", new OpenApiInfo
	{
		Title = "DailyTasks | Api",
		Description = "An ASP.NET Core Web API for managing Tasks App",
		Version = "1",
		Contact = new OpenApiContact
		{
			Name = "Artur Lambo",
			Email = "lamboartur94@gmail.com"
		}
	});

	var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
	opt.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

builder.Services.AddCors(options =>
{
	options.AddPolicy(name: MyAllowSpecificOrigins,
					  policy =>
					  {
						  policy.AllowAnyOrigin()
						   .AllowAnyMethod()
						   .AllowAnyHeader();
					  });
});

builder.Configuration
	.SetBasePath(Directory.GetCurrentDirectory())
	.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: false, reloadOnChange: false);

var item = builder.Configuration.GetSection("ConnectionStrings");
var conStrings = item["DefaultConnection"];
// if (conStrings == null)
// {
// 	throw new Exception("La chaine de connection à la base de données est nulle");
// }
builder.Services.AddDbContext<DailyTasksMigrationsContext>(opt => opt.UseInMemoryDatabase(conStrings));
builder.Services.AddControllersWithViews();
builder.Services.AddRouting();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDataProtection();
builder.Services.AddHealthChecks();

ThreadPool.SetMinThreads(100, 100);
var kestrelSectionCertificate = builder.Configuration.GetSection("Kestrel:EndPoints:Https:Certificate");
var certificateFile = kestrelSectionCertificate["File"];
var certificatePassword = kestrelSectionCertificate["Password"];

builder.Services.Configure<KestrelServerOptions>(options =>
{
	if (string.IsNullOrEmpty(certificateFile) || string.IsNullOrEmpty(certificatePassword))
	{
		throw new InvalidOperationException("Certificate path or password not configured");
	}
	options.Limits.MaxConcurrentConnections = 100;
	options.Limits.MaxRequestBodySize = 10 * 1024;
	options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(2);
	options.ConfigureHttpsDefaults(opt =>
	{
		opt.ClientCertificateMode = ClientCertificateMode.NoCertificate; // Required Certificate dans les autres services c'est du allowCertificate
	});
});

// Conteneur d'enregistrement de dépendances-------------------------------- 
/*
	+----------------------------------------------------------------------+
	|Enregistrement de services Injectées lorsqu'une interface est démandée|
	+----------------------------------------------------------------------+
*/

builder.Services.AddScoped<IReadUsersMethods, UtilisateurService>();
builder.Services.AddScoped<IWriteUsersMethods, UtilisateurService>();
builder.Services.AddScoped<IReadTasksMethods, TacheService>();
builder.Services.AddScoped<IWriteTasksMethods, TacheService>();
builder.Services.AddScoped<UtilisateurService>(); // ça c'est parceque j'ai besoin de ce service directement dans NotificationService
/* 
	+----------------------------------------------------+
	| Enregistrement de repositories Injectés directement|
	+----------------------------------------------------+
*/

builder.Services.AddScoped<UtilisateurRepository>();
builder.Services.AddScoped<TacheRepository>();
/* 
	+-----------------------------------------------+
	| Enregistrement pour le système de notification|
	+-----------------------------------------------+
*/
// Enregistrement du gestionnaire d'événements (EventBus)
builder.Services.AddSingleton<EventBus>();

// Enregistrement du service de notification
builder.Services.AddScoped<NotificationService>();

//---------------------------FIN--------------------------------------------
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
builder.Services.AddLogging();
builder.Services.AddAuthorization();

// On va ajouter l'authentification basic avec le nom "BasicAuthentication" sans options
builder.Services.AddAuthentication("BasicAuthentication")
	.AddScheme<AuthenticationSchemeOptions, AuthentificationBasicMiddleware>("BasicAuthentication", _ => { });

// On va ajouter l'authentification JWT Bearer avec le nom "JwtAuthentification"
builder.Services.AddAuthentication("JwtAuthorization")
	// On va ajouter le schéma d'authentification personnalisé JwtBearer avec les options par défaut
	.AddScheme<JwtBearerOptions, JwtBearerAuthenticationMiddleware>("JwtAuthorization", options =>
	{
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuer = true,
			ValidateAudience = true,
			ValidateLifetime = true,
			RequireExpirationTime=true,
			ValidateIssuerSigningKey = true,
			ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
			ValidAudience = builder.Configuration["JwtSettings:Audience"],
			ClockSkew = TimeSpan.Zero // se documenter par rapport à ceci car le token .NET accorde un temps de latence de 5 minutes en mettant ceci on remet les compteurs à zero
		};
	});

builder.Services.AddAuthorization(options =>
 {
	 // Politique d'autorisation pour les administrateurs
	 options.AddPolicy("AdminPolicy", policy =>
		 policy.RequireRole(nameof(Utilisateur.Privilege.Administrateur))
			   .RequireAuthenticatedUser()
			   .AddAuthenticationSchemes("JwtAuthorization"));

	 // Politique d'autorisation pour les utilisateurs non-administrateurs
	 options.AddPolicy("UserPolicy", policy =>
		policy.RequireRole(nameof(Utilisateur.Privilege.Utilisateur))
			   .RequireAuthenticatedUser()  // L'utilisateur doit être authentifié
			   .AddAuthenticationSchemes("BasicAuthentication"));
 });
var app = builder.Build();
app.UseMiddleware<ContextPathMiddleware>("/lambo-tasks-management"); // On ne pourra plus accéder au swagger avec /index.html

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI(con =>
	 {
		 con.SwaggerEndpoint("/lambo-tasks-management/swagger/1/swagger.yml", "Daily Tasks Management API"); //lambo-tasks-management

		 con.RoutePrefix = string.Empty;
	 });
}
else if (app.Environment.IsProduction())
{
	// Gérer les erreurs dans un environnement de production
	app.UseExceptionHandler("/Error");
	app.UseHsts();
	app.UseSwagger();
	app.UseSwaggerUI(con =>
	 {
		 con.SwaggerEndpoint("/lambo-tasks-management/swagger/1/swagger.json", "Daily Tasks Management API");

		 con.RoutePrefix = string.Empty;
	 });
}

app.UseCors(MyAllowSpecificOrigins);
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
 {
	 endpoints.MapControllers();
	 endpoints.MapHealthChecks("/health");
	 endpoints.MapGet("/version", async context => await context.Response.WriteAsync("Version de l'API : 1"));
 });
app.Run();
