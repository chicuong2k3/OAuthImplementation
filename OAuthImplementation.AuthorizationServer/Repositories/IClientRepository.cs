using OAuthImplementation.AuthorizationServer;

namespace OAuthImplementation.AuthorizationServer.Repositories
{
    public interface IClientRepository
    {
        Client? GetClientById(string clientId);
    }
}
