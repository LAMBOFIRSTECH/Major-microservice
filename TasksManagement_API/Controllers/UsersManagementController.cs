using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TasksManagement_API.Interfaces;
using TasksManagement_API.Models;
namespace TasksManagement_API.Controllers;
[ApiController]
[Route("api/v1/")]
public class UsersManagementController : ControllerBase
{
    private readonly IReadUsersMethods readUsersMethods;
    private readonly IWriteUsersMethods writeUsersMethods;
    public UsersManagementController(IReadUsersMethods readUsersMethods, IWriteUsersMethods writeUsersMethods)
    {
        this.readUsersMethods = readUsersMethods;
        this.writeUsersMethods = writeUsersMethods;
    }

    /// <summary>
    /// Affiche la liste de tous les utilisateurs.
    /// </summary>
    [HttpGet("users")] // vrai endpoint
    public async Task<ActionResult> GetUsers()
    {
		var listOfUsers = await readUsersMethods.GetUsers();
        if (listOfUsers.Any())
        {
            return Ok(listOfUsers);
        }
        return NoContent();
    }
    [Authorize(Policy = "AdminPolicy")]
    [HttpGet("all")]
    public async Task<ActionResult> All()
    {
        if ((await readUsersMethods.GetUsers()).Any()) { return Ok(await readUsersMethods.GetUsers()); }
        return NoContent();
    }

    /// <summary>
    /// Affiche les informations sur un utilisateur en fonction de son ID.
    /// </summary>
    /// <param name="Nom"></param>
    /// <param name="Role"></param>
    /// <returns></returns>
    [Authorize(Policy = "AdminPolicy")]
    [HttpGet("SingleUser/{Nom}/{Role}")]
    public async Task<ActionResult> GetSingleUser(string Nom, string Role)
    {
        if (Enum.TryParse(char.ToUpper(Role[0]) + Role[1..].ToLower(), out Utilisateur.Privilege result))
        {
            try
            {
                var utilisateur = await readUsersMethods.GetSingleUserByNameRole(Nom, result);
                if (utilisateur != null)
                {
                    return Ok(utilisateur);
                }
                return NotFound("Utilisateur non trouvé.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message.Trim());
            }
        }
        else
        {
            return BadRequest("Le rôle spécifié n'est pas valide.");
        }
    }

    /// <summary>
    /// Créée un utilisateur.
    /// </summary>
    /// <remarks>
    /// <para>Sample request:</para>
    /// <para>
    ///     POST /CreateUser
    ///     {
    ///        "nom": "username",
    ///        "email": "adress_name@mailing_server.domain"
    ///        "role": enum {Utilisateur, Administrateur},
    ///        "pass": "password",
    ///        "lesTaches": []
    ///     }
    /// </para>
    /// </remarks>
    [HttpPost("user")]
    public async Task<ActionResult> CreateUser([FromBody] Utilisateur utilisateur)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        try
        {
            if (!Enum.IsDefined(typeof(Utilisateur.Privilege), utilisateur.Role))
            {
                return BadRequest("Le rôle spécifié n'est pas valide.");
            }
            var nouveauNomUtilisateur = await readUsersMethods.CheckExistedUser(utilisateur);
            if (!string.IsNullOrEmpty(utilisateur.Pass))
            {
                utilisateur.SetHashPassword(utilisateur.Pass);
            }
            if (!utilisateur.CheckEmailAdress(utilisateur.Email))
            {
                const string message = "Adresse e-mail invalide";
                return StatusCode(StatusCodes.Status406NotAcceptable, message);
            }
            Utilisateur newUtilisateur = new()
            {
                Nom = nouveauNomUtilisateur!,
                Pass = utilisateur.Pass,
                Role = utilisateur.Role,
                Email = utilisateur.Email
            };
            await writeUsersMethods.CreateUser(newUtilisateur);
            return CreatedAtAction(nameof(GetUsers), new { newUtilisateur.Nom, newUtilisateur.Email, Role = newUtilisateur.Role.ToString() });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message.Trim());
        }
    }
    /// <summary>
    /// Supprime un utilisateur en fonction de son ID.
    /// </summary>
    /// <param name="Nom"></param>
    /// <param name="Role"></param>
    /// <returns></returns>
    //[Authorize(Policy = "AdminPolicy")]
    [HttpDelete("user/{Nom}/{Role}")]
    public async Task<ActionResult> DeleteUserByDetails(string Nom, string Role)
    {
        if (Enum.TryParse(char.ToUpper(Role[0]) + Role.Substring(1).ToLower(), out Utilisateur.Privilege result))
        {
            try
            {
                var utilisateur = await readUsersMethods.GetSingleUserByNameRole(Nom, result);
                if (utilisateur == null)
                {
                    return NotFound($"L'utilisateur [{Nom}] n'a pas été trouvé dans le contexte de base de données");
                }
                else
                {
                    await writeUsersMethods.DeleteUserByDetails(Nom, result);
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message.Trim());
            }
        }
        else
        {
            return BadRequest("Le rôle spécifié n'est pas valide.");
        }
    }
    /// <summary>
    /// Met à jour le mot de passe d'un utilisateur en fonction de son nom.
    /// </summary>
    /// <param name="nom"></param>
    /// <param name="currentpassword"></param>
    /// <param name="newpassword"></param>
    /// <returns></returns>
    [HttpPatch("user/{nom}/{currentpassword}/{newpassword}")]
    public async Task<ActionResult> UpdateUserPassword(string nom, [DataType(DataType.Password)] string currentpassword, [DataType(DataType.Password)] string newpassword)
    {
        try
        {
            var utilisateur = (await readUsersMethods.GetUsers(query => query.Where(u => u.Nom!.Equals(nom)))).FirstOrDefault();
            if (utilisateur == null)
            {
                return NotFound("Utilisateur non trouvé");
            }
            if (currentpassword == newpassword)
            {
                return Conflict("Ce mot de passe existe déjà Changez le !");
            }
            if (!utilisateur.CheckHashPassword(newpassword))
            {
                utilisateur.Pass = utilisateur.SetHashPassword(newpassword);
                await writeUsersMethods.SetUserPassword(nom, newpassword);
            }
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message.Trim());
        }
    }
}