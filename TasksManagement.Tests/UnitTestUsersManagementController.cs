using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using TasksManagement_API.Controllers;
using TasksManagement_API.Interfaces;
using TasksManagement_API.Models;
using Xunit;

namespace TasksManagement.Tests;
public class UnitTestUsersManagementController
{
    private readonly Mock<IReadUsersMethods> mockReadUsersMethods;
    private readonly Mock<IWriteUsersMethods> mockWriteUsersMethods;
    private readonly UsersManagementController controller;

    public UnitTestUsersManagementController()
    {
        mockReadUsersMethods = new Mock<IReadUsersMethods>();
        mockWriteUsersMethods = new Mock<IWriteUsersMethods>();
        controller = new UsersManagementController(mockReadUsersMethods.Object, mockWriteUsersMethods.Object);
    }

    [Fact]
    public async Task GetUsers_ReturnsOkResult_WhenUsersExist()
    {
        // Arrange
        var users = new List<Utilisateur> { new Utilisateur { Nom = "TestUser" } };
        // Ici, on fournit explicitement les paramètres optionnels
        mockReadUsersMethods.Setup(m => m.GetUsers(It.IsAny<Func<IQueryable<Utilisateur>, IQueryable<Utilisateur>>>(), It.IsAny<bool>()))
            .ReturnsAsync(users);

        // Act
        var result = await controller.GetUsers();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<List<Utilisateur>>(okResult.Value);
        Assert.Single(returnValue);
    }

    [Fact]
    public async Task GetUsers_ReturnsNoContent_WhenNoUsersExist()
    {
        // Arrange
        var users = new List<Utilisateur>();
        mockReadUsersMethods.Setup(m => m.GetUsers(It.IsAny<Func<IQueryable<Utilisateur>, IQueryable<Utilisateur>>>(), It.IsAny<bool>())).ReturnsAsync(users);

        // Act
        var result = await controller.GetUsers();

        // Assert
        Assert.IsType<NoContentResult>(result);
    }
    [Fact]
    public async Task GetSingleUser_ReturnsOkResult_WhenUserExists()
    {
        // Arrange
        var user = new Utilisateur { Nom = "User1", Role = Utilisateur.Privilege.Utilisateur };
        mockReadUsersMethods.Setup(m => m.GetSingleUserByNameRole("User1", Utilisateur.Privilege.Utilisateur)).ReturnsAsync(user);

        // Act
        var result = await controller.GetSingleUser("User1", "Utilisateur");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(user, okResult.Value);
    }

    [Fact]
    public async Task GetSingleUser_ReturnsNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        mockReadUsersMethods.Setup(m => m.GetSingleUserByNameRole("User1", Utilisateur.Privilege.Utilisateur)).ReturnsAsync((Utilisateur?)null);

        // Act
        var result = await controller.GetSingleUser("User1", "Utilisateur");

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task CreateUser_ReturnsCreatedAtAction_WhenUserIsCreated()
    {
        // Arrange
        var user = new Utilisateur { Nom = "User1", Email = "user1@example.com", Role = Utilisateur.Privilege.Utilisateur, Pass = "password" };
        mockReadUsersMethods.Setup(m => m.CheckExistedUser(user)).ReturnsAsync("User1");
        mockWriteUsersMethods.Setup(m => m.CreateUser(It.IsAny<Utilisateur>())).ReturnsAsync(user);

        // Act
        var result = await controller.CreateUser(user);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal("GetUsers", createdAtActionResult.ActionName);
    }

    [Fact]
    public async Task DeleteUserByDetails_ReturnsNoContent_WhenUserIsDeleted()
    {
        // Arrange
        var user = new Utilisateur { Nom = "User1", Role = Utilisateur.Privilege.Utilisateur };
        mockReadUsersMethods.Setup(m => m.GetSingleUserByNameRole("User1", Utilisateur.Privilege.Utilisateur)).ReturnsAsync(user);
        mockWriteUsersMethods.Setup(m => m.DeleteUserByDetails("User1", Utilisateur.Privilege.Utilisateur)).Returns(Task.CompletedTask);

        // Act
        var result = await controller.DeleteUserByDetails("User1", "Utilisateur");

        // Assert
        Assert.IsType<NoContentResult>(result);
    }
}