using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace Manufacturing.WinApp.Common
{
    public class ApiClient
    {
        private readonly string _baseUrl;
        private readonly string _bearerToken;

        public ApiClient(string baseUrl, string bearerToken)
        {
            _baseUrl = baseUrl;
            _bearerToken = bearerToken;
        }

        public async Task<T> GetData<T>(string resource)
        {
            using (var client = new SecureHttpClient(_bearerToken))
            {
                var dataUrl = _baseUrl + resource;
                var request = new HttpRequestMessage(HttpMethod.Get, dataUrl);
                request.Headers.Accept.Add(
                    new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                var response = await client.SendAsync(request);

                // continue processing after the async task has completed
                if (response != null && response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    if (string.IsNullOrEmpty(json))
                    {
                        return default(T);
                    }
                    else
                    {
                        var records = JsonConvert.DeserializeObject<T>(json);
                        return records;
                    }
                }
                else
                {
                    //Error making server call
                    return default(T);
                }
            }
        }

        public async Task<TOut> PostData<TIn, TOut>(string resource, TIn toPost)
        {
            using (var client = new SecureHttpClient(_bearerToken))
            {
                var dataUrl = _baseUrl + resource;
                var request = new HttpRequestMessage(HttpMethod.Post, dataUrl);
                request.Content = new StringContent(JsonConvert.SerializeObject(toPost), Encoding.UTF8, "application/json");
                var response = await client.SendAsync(request);

                // continue processing after the async task has completed
                if (response != null && response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    if (string.IsNullOrEmpty(json))
                    {
                        return default(TOut);
                    }
                    else
                    {
                        var record = JsonConvert.DeserializeObject<TOut>(json);
                        return record;
                    }
                }
                else
                {
                    //Error making server call
                    return default(TOut);
                }
            }
        }

        public async Task<T> PutData<T>(string resource, T toPut)
        {
            using (var client = new SecureHttpClient(_bearerToken))
            {
                var dataUrl = _baseUrl + resource;
                var request = new HttpRequestMessage(HttpMethod.Put, dataUrl);
                request.Content = new StringContent(JsonConvert.SerializeObject(toPut), Encoding.UTF8, "application/json");
                var response = await client.SendAsync(request);

                // continue processing after the async task has completed
                if (response != null && response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    if (string.IsNullOrEmpty(json))
                    {
                        return default(T);
                    }
                    else
                    {
                        return toPut;
                    }
                }
                else
                {
                    //Error making server call
                    return default(T);
                }
            }
        }
    }
}
