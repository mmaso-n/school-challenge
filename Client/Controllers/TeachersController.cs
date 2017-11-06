using Microsoft.AspNetCore.Mvc;
using SchoolChallenge.Client.Dependencies;
using SchoolChallenge.Contracts;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace SchoolChallenge.Client.Controllers
{
    [Route("api/[controller]")]
    public class TeachersController : Controller
    {
        IHttpClient _httpClient;
        ITenantConfiguration _tenantConfiguration;

        public TeachersController(IHttpClient httpClient, ITenantConfiguration tenantConfiguration)
        {
            _httpClient = httpClient;
            _tenantConfiguration = tenantConfiguration;
        }

        [HttpGet("[action]")]
        public async Task<IEnumerable<Teacher>> GetAllTeachersAsync()
        {
            var results = new List<Teacher>();
            var path = $"api/Teachers/getall/{_tenantConfiguration.Tenant}";

            HttpResponseMessage response = await _httpClient.GetAsync(path);

            if (response.IsSuccessStatusCode)
            {
                results = await response.Content.ReadAsAsync<List<Teacher>>();
            }

            return results;
        }

        [HttpPost("[action]")]
        public async void DeleteTeacherAsync(int id, string number, string firstName, string lastName)
        {
            var path = $"api/Teachers/delete";

            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("school", _tenantConfiguration.Tenant),
                new KeyValuePair<string, string>("id", id.ToString()),
                new KeyValuePair<string, string>("firstName", firstName),
                new KeyValuePair<string, string>("lastName", lastName)
            });

            await _httpClient.PostAsync(path, formContent);
        }

        [HttpPost("[action]")]
        public void InsertTeacher(int id, string number, string firstName, string lastName)
        {
            var path = $"api/Teachers/insert";

            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("school", _tenantConfiguration.Tenant),
                new KeyValuePair<string, string>("id", id.ToString()),
                new KeyValuePair<string, string>("number", number),
                new KeyValuePair<string, string>("firstName", firstName),
                new KeyValuePair<string, string>("lastName", lastName)
            });

            _httpClient.PostAsync(path, formContent);
        }

        [HttpPost("[action]")]
        public async Task<IEnumerable<Teacher>> SearchTeacherAsync(string school, int? teacherId = null, string firstName = null, string lastName = null)
        {
            var results = new List<Teacher>();
            var path = $"api/Teachers/search";

            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("school", _tenantConfiguration.Tenant),
                new KeyValuePair<string, string>("teacherId", teacherId.ToString()),
                new KeyValuePair<string, string>("firstName", firstName),
                new KeyValuePair<string, string>("lastName", lastName)
            });

            HttpResponseMessage response = await _httpClient.PostAsync(path, formContent);

            if (response.IsSuccessStatusCode)
            {
                results = await response.Content.ReadAsAsync<List<Teacher>>();
            }

            return results;
        }
    }
}
