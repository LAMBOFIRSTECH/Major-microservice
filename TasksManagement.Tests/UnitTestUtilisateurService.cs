using Xunit;
using TasksManagement_API.Models;
using TasksManagement_API.Interfaces;
using TasksManagement_API.Services;
using Moq;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using TasksManagement_API.Reposirtories;

namespace TasksManagement.Tests
{
    /* Nomenclature : 
	-	Pour les méthodes de test => ClassName_Method_Return(Ce que l'on souhaite retourner)_{i} i -> Grand N*
	-	Pour les noms des classes à tester => ClassName/ServiceName/ControllerName[Test]
	*/
    public class UnitTestUtilisateurService
    {
        private readonly UtilisateurService utilisateurService;
        private readonly Mock<UtilisateurRepository> mockReadUsersMethods;
        private readonly Mock<IConfiguration> configuration = new Mock<IConfiguration>();

        public UnitTestUtilisateurService()
        {
            mockReadUsersMethods = new Mock<UtilisateurRepository>();

            // Initialisation du service en lui injectant le mock
            utilisateurService = new UtilisateurService(
				configuration.Object,
                mockReadUsersMethods.Object,
                Mock.Of<IDataProtectionProvider>()
            );
            //SetupData();
        }
        // private void SetupData()
        // {
        //     Func<IQueryable<Utilisateur>, IQueryable<Utilisateur>>? filter = null;
        //     // Ajout de données de test à la base de données
        //     mockReadUsersMethods.Setup(m => m.GetUsers(filter)).ReturnsAsync(new List<Utilisateur>
        //     {
        //     new() { ID= new Guid(), Nom = "Alice", Email = "alice@example.com", Role = Utilisateur.Privilege.Administrateur },
        //     new() { ID = new Guid(), Nom = "Alice_1", Email = "alice@example.com", Role = Utilisateur.Privilege.Administrateur },
        //     new() { ID= new Guid(), Nom = "Bob", Email = "bob@example.com", Role = Utilisateur.Privilege.Utilisateur },
        //     new() { ID= new Guid(), Nom = "Charlie", Email = "charlie@example.com", Role = Utilisateur.Privilege.Administrateur }
        //     });
        // }
        // [Fact]
        // public async Task UtilisateurService_GetUsers_ReturnsAllUsersWhenNoFilterIsProvided_1()
        // {
        //     // Act
        //     var result = await utilisateurService.GetUsers(null);
        //     // Assert
        //     Assert.Equal(4, result.Count);
        // }
        // [Theory]
        // [InlineData("Alice")]
        // [InlineData("Bob")]
        // public async Task UtilisateurService_GetUsers_ReturnsSpecificUserWhenFilterIsProvided_2(string Nom)
        // {
        // 	// Arrange
        // 	Func<IQueryable<Utilisateur>, IQueryable<Utilisateur>>? filter = query => query.Where(u => u.Role == Utilisateur.Privilege.Administrateur && u.Nom.Equals(Nom) || u.Role == Utilisateur.Privilege.Utilisateur && u.Nom.Equals(Nom));
        // 	// Act
        // 	var result = await utilisateurService.GetUsers(filter);
        // 	// Assert
        // 	Assert.Equal(1, result.Count);
        // }
        // [Theory]
        // [InlineData("Alice", 2)]
        // public async Task CheckExistedUser_ReturnsUserNewName_3(string Nom, int i)
        // {
        // 	// Arrange
        // 	var expectedUsername = $"{Nom}_{i}";
        // 	Func<IQueryable<Utilisateur>, IQueryable<Utilisateur>>? filter = query => query.Where(u => u.Role == Utilisateur.Privilege.Administrateur && u.Nom.Equals(Nom) || u.Role == Utilisateur.Privilege.Utilisateur && u.Nom.Equals(Nom));
        // 	// Act
        // 	var utilisateurs = (await utilisateurService.GetUsers(filter)).ToList();
        // 	var currentUsername = utilisateurService.CheckExistedUser(utilisateurs.First()!).Result;
        // 	// Assert
        // 	Assert.Equal(expectedUsername, currentUsername);
        // }
        // [Theory]
        // [InlineData("Alice")]
        // public async Task GetUsers_ReturnsSpecificUser_WhenFilterIsProvided_WithTasks_4(string Nom) // Peut redondant mettre en évidence le service Tache
        // {
        // 	// Arrange
        // 	Func<IQueryable<Utilisateur>, IQueryable<Utilisateur>>? filter = query => query.Where(u => u.Role == Utilisateur.Privilege.Administrateur && u.Nom.Equals(Nom) || u.Role == Utilisateur.Privilege.Utilisateur && u.Nom.Equals(Nom));
        // 	// Act
        // 	var result = await utilisateurService.GetUsers(filter);
        // 	// Assert
        // 	Assert.Equal(1, result.Count);
        // 	Assert.Contains(result, u => u.Nom == "Alice"); // Ceci est la bonne assertion attendue
        // }

    }
}