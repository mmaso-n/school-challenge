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
        public void DeleteTeacher(int id, string number, string firstName, string lastName, bool hasScholarship, int teacherId)
        {
            var path = $"api/Teachers/delete";

            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("school", _tenantConfiguration.Tenant),
                new KeyValuePair<string, string>("id", id.ToString()),
                new KeyValuePair<string, string>("firstName", firstName),
                new KeyValuePair<string, string>("lastName", lastName)
            });

            _httpClient.PostAsync(path, formContent);
        }
    }
}
