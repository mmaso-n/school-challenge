using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SchoolChallenge.Client.Dependencies
{
    public interface IHttpClient
    {
        Task<HttpResponseMessage> GetAsync(string requestUri);
    }

    public class ServicesHttpClient : IHttpClient, IDisposable
    {
        private readonly HttpClient _innerClient = new HttpClient();

        public ServicesHttpClient(string servicesBaseAddress)
        {
            _innerClient.BaseAddress = new Uri(servicesBaseAddress);
            _innerClient.DefaultRequestHeaders.Accept.Clear();
            _innerClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public void Dispose()
        {
            _innerClient.Dispose();
        }

        public async Task<HttpResponseMessage> GetAsync(string requestUri)
        {
            return await _innerClient.GetAsync(requestUri); 
        }
    }
}
