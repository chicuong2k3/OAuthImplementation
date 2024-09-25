using OAuthImplementation.AuthorizationServer;

namespace OAuthImplementation.AuthorizationServer.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly List<Client> _clients = new List<Client>()
        {
            new Client()
            {
                ClientId = "oauth-client-1",
                ClientSecret = "cuongdeptrai",
                RedirectUris = ["https://localhost:7001/callback"],
                AllowScopes = "openid profile"
            }
        };
        public Client? GetClientById(string clientId)
        {
            return _clients.Where(c => c.ClientId == clientId).FirstOrDefault();
        }
    }
}
