namespace TasksManagement_API.Interfaces;
public interface IHashicorpVaultService
{
    Task<string> GetRabbitConnectionStringFromVault();
    Task<string> GetAppRoleTokenFromVault();
}