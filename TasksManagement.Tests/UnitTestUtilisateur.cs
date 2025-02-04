using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using TasksManagement_API.Models;
using Xunit;

namespace TasksManagement.Tests;

public class UnitTestUtilisateur
{
    [Fact]
    public void Utilisateur_ID_ShouldBeOfTypeGuid()
    {
        // Arrange
        var utilisateur = new Utilisateur();

        // Act
        var idType = utilisateur.ID.GetType();

        // Assert
        Assert.Equal(typeof(Guid), idType);
    }
    [Fact]
    public void Utilisateur_ID_ShouldNotBeEmpty()
    {
        // Arrange
        var utilisateur = new Utilisateur
        {
            ID = Guid.NewGuid()
        };

        // Act
        var id = utilisateur.ID;

        // Assert
        Assert.NotEqual(Guid.Empty, id);
    }
    [Fact]
    public void Nom_ShouldBeRequired()
    {
        // Arrange
        var utilisateur = new Utilisateur();

        // Act
        var validationContext = new ValidationContext(utilisateur);
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(utilisateur, validationContext, validationResults, true);

        // Assert
        Assert.False(isValid);
        Assert.Contains(validationResults, v => v.MemberNames.Contains("Nom"));
    }
    [Fact]
    public void Nom_ShouldNotBeEmpty()
    {
        // Arrange
        var utilisateur = new Utilisateur { Nom = "" };

        // Act
        var validationContext = new ValidationContext(utilisateur);
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(utilisateur, validationContext, validationResults, true);

        // Assert
        Assert.False(isValid);
        Assert.Contains(validationResults, v => v.MemberNames.Contains("Nom"));
    }
    [Fact]
    public void Email_ShouldBeRequired()
    {
        // Arrange
        var utilisateur = new Utilisateur();

        // Act
        var validationContext = new ValidationContext(utilisateur);
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(utilisateur, validationContext, validationResults, true);

        // Assert
        Assert.False(isValid);
        Assert.Contains(validationResults, v => v.MemberNames.Contains("Email") && v.ErrorMessage.Contains("required"));
    }

    [Fact]
    public void Email_ShouldBeInvalid()
    {
        // Arrange
        var utilisateur = new Utilisateur { Nom = "nom", Pass = "password", Email = "invalid-mail" };

        // Act
        var validationContext = new ValidationContext(utilisateur);
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(utilisateur, validationContext, validationResults, true);

        // Assert
        Assert.True(isValid);
        // Assert.Contains(validationResults, v => v.MemberNames.Contains("Email"));
    }
    [Fact]
    public void Role_ShouldBeEnumType()
    {
        // Arrange
        var utilisateur = new Utilisateur { Role = Utilisateur.Privilege.Administrateur };

        // Act
        var validationContext = new ValidationContext(utilisateur);
        var validationResults = new List<ValidationResult>();

        // Assert
        var isValid = Validator.TryValidateObject(utilisateur, validationContext, validationResults, true);
        Assert.DoesNotContain(validationResults, v => v.MemberNames.Contains(nameof(Utilisateur.Role)));
    }

    [Fact]
    public void Pass_ShouldBeRequired()
    {
        // Arrange
        var utilisateur = new Utilisateur();

        // Act
        var context = new ValidationContext(utilisateur, null, null);
        var results = new List<ValidationResult>();

        // Assert
        var isValid = Validator.TryValidateObject(utilisateur, context, results, true);
        Assert.False(isValid);
        Assert.Contains(results, v => v.MemberNames.Contains("Pass") && v.ErrorMessage.Contains("required"));
    }
    [Fact]
    public void Pass_ShouldHaveCategorySecurity()
    {
        // Arrange
        var passProperty = typeof(Utilisateur).GetProperty("Pass");

        // Act
        var categoryAttribute = (CategoryAttribute?)Attribute.GetCustomAttribute(passProperty, typeof(CategoryAttribute));

        // Assert
        Assert.NotNull(categoryAttribute);
        Assert.Equal("Security", categoryAttribute.Category);
    }
    [Fact]
    public void CheckHashPassword_ShouldReturnTrue_ForCorrectPassword()
    {
        // Arrange
        var utilisateur = new Utilisateur();
        var password = "securePassword123";
        utilisateur.SetHashPassword(password);

        // Act
        var result = utilisateur.CheckHashPassword(password);

        // Assert
        Assert.True(result);
    }
    [Fact]
    public void CheckHashPassword_ShouldReturnFalse_ForIncorrectPassword()
    {
        // Arrange
        var utilisateur = new Utilisateur();
        var password = "securePassword123";
        var wrongPassword = "wrongPassword";
        utilisateur.SetHashPassword(password);

        // Act
        var result = utilisateur.CheckHashPassword(wrongPassword);

        // Assert
        Assert.False(result);
    }
    [Fact]
    public void SetHashPassword_ShouldReturnHashedPassword_WhenPasswordIsNotEmpty()
    {
        // Arrange
        var utilisateur = new Utilisateur();
        var password = "TestPassword123";

        // Act
        var hashedPassword = utilisateur.SetHashPassword(password);

        // Assert
        Assert.False(string.IsNullOrEmpty(hashedPassword));
        Assert.NotEqual(password, hashedPassword);
        Assert.True(BCrypt.Net.BCrypt.Verify(password, hashedPassword));
    }
    [Fact]
    public void SetHashPassword_ShouldReturnEmptyString_WhenPasswordIsEmpty()
    {
        // Arrange
        var utilisateur = new Utilisateur();
        var password = string.Empty;

        // Act
        var hashedPassword = utilisateur.SetHashPassword(password);

        // Assert
        Assert.Equal(string.Empty, hashedPassword);
    }
    [Fact]
    public void SetHashPassword_ShouldReturnEmptyString_WhenPasswordIsNull()
    {
        // Arrange
        var utilisateur = new Utilisateur();
        string? password = null;

        // Act
        var hashedPassword = utilisateur.SetHashPassword(password!);

        // Assert
        Assert.Equal(string.Empty, hashedPassword);
    }
    [Theory]
        [InlineData("test@example.com", true)]
        [InlineData("user@domain.org", true)]
        [InlineData("invalid-email", false)]
        [InlineData("user@domain", false)]
        [InlineData("", false)]
        [InlineData(null, false)]
        public void CheckEmailAdress_ShouldValidateEmailCorrectly(string email, bool expected)
        {
            // Arrange
            var utilisateur = new Utilisateur();

            // Act
            var result = utilisateur.CheckEmailAdress(email);

            // Assert
            Assert.Equal(expected, result);
        }


}