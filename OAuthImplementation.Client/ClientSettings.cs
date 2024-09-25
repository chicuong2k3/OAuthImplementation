namespace OAuthImplementation.Client
{
    public class ClientSettings
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public List<string> RedirectUris { get; set; }
    }
}
