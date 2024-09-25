using System.Web;

namespace OAuthImplementation.Client.Extensions
{
    public static class UriExtensions
    {
        public static string AddParamsToUrl(this string uri, Dictionary<string, string> queryParams)
        {
            var uriBuilder = new UriBuilder(uri);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);

            foreach (var param in queryParams)
            {
                query.Set(param.Key, param.Value);
            }

            uriBuilder.Query = query.ToString();
            return uriBuilder.ToString();
        }
    }
}
