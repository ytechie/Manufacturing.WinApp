using System.Net.Http;
using System.Net.Http.Headers;

namespace Manufacturing.WinApp.Common
{
    public class SecureHttpClient : HttpClient
    {
        public SecureHttpClient(string bearerToken)
        {
            DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
        }
    }
}
