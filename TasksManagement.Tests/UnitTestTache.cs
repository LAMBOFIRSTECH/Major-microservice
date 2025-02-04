using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using TasksManagement_API.Models;
using Xunit;
namespace TasksManagement.Tests;

public class UnitTestTache
{

    [Fact]
    public void Tache_ID_ShouldBeOfTypeGuid()
    {
        // Arrange
        var tache = new Tache();

        // Act
        var idType = tache.ID.GetType();

        // Assert
        Assert.Equal(typeof(Guid), idType);
    }
    [Fact]
    public void Tache_ID_ShouldNotBeEmpty()
    {
        // Arrange
        var tache = new Tache();
        tache.ID = Guid.NewGuid();

        // Act
        var id = tache.ID;

        // Assert
        Assert.NotEqual(Guid.Empty, id);
    }
    [Fact]
    public void Titre_ShouldBeRequired()
    {
        // Arrange
        var tache = new Tache();

        // Act
        var validationContext = new ValidationContext(tache);
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(tache, validationContext, validationResults, true);

        // Assert
        Assert.False(isValid);
        Assert.Contains(validationResults, v => v.MemberNames.Contains("Titre") && v.ErrorMessage.Contains("required"));
    }
    [Fact]
    public void Titre_ShouldNotBeEmpty()
    {
        // Arrange
        var tache = new Tache { Titre = "" };

        // Act
        var validationContext = new ValidationContext(tache);
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(tache, validationContext, validationResults, true);

        // Assert
        Assert.False(isValid);
        Assert.Contains(validationResults, v => v.MemberNames.Contains("Titre") && v.ErrorMessage.Contains("required"));
    }
    [Fact]
    public void Titre_ShouldBeValid()
    {
        // Arrange
        var tache = new Tache { Titre = "Valid Title" };

        // Act
        var validationContext = new ValidationContext(tache);
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(tache, validationContext, validationResults, true);

        // Assert
        Assert.True(isValid);
    }
    [Fact]
    public void Summary_ShouldBeEmptyString_ByDefault()
    {
        // Arrange
        var tache = new Tache();

        // Act
        var summary = tache.Summary;

        // Assert
        Assert.Equal(string.Empty, summary);
    }
    [Fact]
    public void StartDate_ShouldHaveRequiredAttribute()
    {
        // Arrange
        var property = typeof(Tache).GetProperty("StartDate");

        // Act
        var attribute = Attribute.GetCustomAttribute(property, typeof(RequiredAttribute)) as RequiredAttribute;

        // Assert
        Assert.NotNull(attribute);
        Assert.Equal("Le format de date doit être comme l'exemple suivant : 01/01/2024", attribute.ErrorMessage);
    }
    [Fact]
    public void EndDate_ShouldHaveRequiredAttribute()
    {
        // Arrange
        var property = typeof(Tache).GetProperty("EndDate");

        // Act
        var attribute = Attribute.GetCustomAttribute(property, typeof(RequiredAttribute)) as RequiredAttribute;

        // Assert
        Assert.NotNull(attribute);
        Assert.Equal("Le format de date doit être comme l'exemple suivant : 01/01/2024", attribute.ErrorMessage);
    }

}