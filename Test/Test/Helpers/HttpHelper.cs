using EmployeeApiConsumer.Helpers;
using Newtonsoft.Json;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;

namespace ConnectAndSell.Common
{
    /// <summary>
    /// A generic wrapper class to REST API calls
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class HttpHelper
    {
        private IHttpClientFactory _httpClientFactory;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpHelper(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }


        private HttpClient HttpClient(Dictionary<string, string> headers = null!)
        {
            var token = _httpContextAccessor!.HttpContext!.Session.GetString("token")!;

            var httpClient = _httpClientFactory.CreateClient("HttpClient");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            if (headers != null && headers.Keys.Count > 0)
            {

                foreach (var item in headers)
                {
                    if (!httpClient.DefaultRequestHeaders.Contains(item.Key))
                        httpClient.DefaultRequestHeaders.Add(item.Key, item.Value);
                }

            }
            return httpClient;
        }


        /// <summary>
        /// For getting the resources from a web api
        /// </summary>
        /// <param name="url">API Url</param>
        /// <returns>A Task with result object of type T</returns>
        public async Task<T> Get<T>(string url, Dictionary<string, string> headers = null!) where T : class
        {
            T result = null!;
            using (var httpClient = this.HttpClient(headers))
            {
                var response = await httpClient.GetAsync(new Uri(url));

                response.EnsureSuccessStatusCode();
                await response.Content.ReadAsStringAsync().ContinueWith((Task<string> x) =>
                {
                    if (x.IsFaulted)
                        throw x.Exception!;
                    result = JsonConvert.DeserializeObject<T>(x.Result)!;
                });
            }

            return result;
        }

        /// <summary>
        /// For creating a new item over a web api using POST
        /// </summary>
        /// <param name="apiUrl">API Url</param>
        /// <param name="postObject">The object to be created</param>
        /// <returns>A Task with created item</returns>
        public async Task<TR> PostAsync<T, TR>(string apiUrl, T postObject, HttpContentType contentType, Dictionary<string, string> headers = null) where T : class
        {
            TR result = default!;

            using (var client = HttpClient(headers))
            {

                HttpResponseMessage response = null;

                if (contentType.Equals(HttpContentType.Json))
                {
                    response = await client.PostAsync(apiUrl, postObject, new JsonMediaTypeFormatter()).ConfigureAwait(false);
                }
                else
                {
                    if (typeof(T) != typeof(Dictionary<string, string>))
                        throw new Exception("Post object should be of type Dictionary<string,string> in case you are using Content type as FormUrlEncoded");

                    var request = new HttpRequestMessage(HttpMethod.Post, apiUrl)
                    {
                        Content = new FormUrlEncodedContent(postObject as Dictionary<string, string>)
                    };

                    response = await client.SendAsync(request);
                }

                response.EnsureSuccessStatusCode();

                await response.Content.ReadAsStringAsync().ContinueWith((x) =>
                {
                    if (x.IsFaulted)
                        throw x.Exception;
                    result = JsonConvert.DeserializeObject<TR>(x.Result);
                });


            }
            return result;
        }

        /// <summary>
        /// For updating an existing item over a web api using PUT
        /// </summary>
        /// <param name="apiUrl">API Url</param>
        /// <param name="putObject">The object to be edited</param>
        public async Task PutRequest<T>(string apiUrl, T putObject, Dictionary<string, string> headers = null!) where T : class
        {
            using (var client = this.HttpClient(headers))
            {
                var response = await client.PutAsync(apiUrl, putObject, new JsonMediaTypeFormatter()).ConfigureAwait(false);

                response.EnsureSuccessStatusCode();
            }
        }
    }
}
