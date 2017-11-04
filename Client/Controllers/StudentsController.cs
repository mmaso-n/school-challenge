using Microsoft.AspNetCore.Mvc;
using SchoolChallenge.Client.Dependencies;
using SchoolChallenge.Contracts;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace SchoolChallenge.Client.Controllers
{
    [Route("api/[controller]")]
    public class StudentsController : Controller
    {
        IHttpClient _httpClient;

        public StudentsController(IHttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpGet("[action]")]
        public async Task<IEnumerable<Student>> GetAllStudents()
        {
            var results = new List<Student>();
            var path = "api/Students/getall/milwaukee";

            HttpResponseMessage response = await _httpClient.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                results = await response.Content.ReadAsAsync<List<Student>>();
            }
            return results;
        }
    }
}
