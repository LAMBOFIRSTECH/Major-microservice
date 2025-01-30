using Microsoft.EntityFrameworkCore;
namespace TasksManagement_API.Models;

public class DailyTasksMigrationsContext : DbContext
{
	public DailyTasksMigrationsContext(DbContextOptions<DailyTasksMigrationsContext> options)
		: base(options)
	{
	}
	public virtual DbSet<Utilisateur> Utilisateurs { get; set; } = null!;
	public DbSet<Tache> Taches { get; set; } = null!;
	public DbSet<Projet> Projets { get; set; } = null!;
	public DbSet<Competence>  Competences { get; set; } = null!;
	public DbSet<Equipe> Equipes { get; set; } = null!;
	public DbSet<Fonction> Fonctions { get; set; } = null!;
	public DbSet<Employe> Employes { get; set; } = null!;
	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Utilisateur>()
		   .HasMany(u => u.LesTaches)
		   .WithOne(t => t.utilisateur)
		   .HasForeignKey(t => t.UserId)
		   .IsRequired()
		   .OnDelete(DeleteBehavior.Cascade);
		modelBuilder.Entity<Tache>();
		
		modelBuilder.Entity<Projet>()
		   .HasIndex(p => p.Code)
		   .IsUnique(); // Montre que c'est une clé candidate
		base.OnModelCreating(modelBuilder);
		
		modelBuilder.Entity<Employe>()
		   .HasIndex(e => e.Matricule)
		   .IsUnique(); // Montre que c'est une clé candidate
		base.OnModelCreating(modelBuilder);
	}

}

