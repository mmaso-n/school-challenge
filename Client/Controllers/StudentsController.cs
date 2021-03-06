﻿using Microsoft.AspNetCore.Mvc;
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
        ITenantConfiguration _tenantConfiguration;

        public StudentsController(IHttpClient httpClient, ITenantConfiguration tenantConfiguration)
        {
            _httpClient = httpClient;
            _tenantConfiguration = tenantConfiguration;
        }

        [HttpGet("[action]")]
        public async Task<IEnumerable<Student>> GetAllStudentsAsync()
        {
            var results = new List<Student>();
            var path = $"api/Students/getall/{_tenantConfiguration.Tenant}";

            HttpResponseMessage response = await _httpClient.GetAsync(path);

            if (response.IsSuccessStatusCode)
            {
                results = await response.Content.ReadAsAsync<List<Student>>();
            }

            return results;
        }

        [HttpPost("[action]")]
        public async void DeleteStudentAsync(int id, string number, string firstName, string lastName, bool hasScholarship, int teacherId)
        {
            var path = $"api/Students/delete";

            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("school", _tenantConfiguration.Tenant),
                new KeyValuePair<string, string>("id", id.ToString()),
                new KeyValuePair<string, string>("number", number),
                new KeyValuePair<string, string>("firstName", firstName),
                new KeyValuePair<string, string>("lastName", lastName),
                new KeyValuePair<string, string>("hasScholarship", hasScholarship.ToString()),
                new KeyValuePair<string, string>("teacherId", teacherId.ToString())
            });

            await _httpClient.PostAsync(path, formContent);
        }

        [HttpPost("[action]")]
        public void InsertStudent(int id, string number, string firstName, string lastName, bool hasScholarship, int teacherId)
        {
            var path = $"api/Students/insert";

            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("school", _tenantConfiguration.Tenant),
                new KeyValuePair<string, string>("id", id.ToString()),
                new KeyValuePair<string, string>("number", number),
                new KeyValuePair<string, string>("firstName", firstName),
                new KeyValuePair<string, string>("lastName", lastName),
                new KeyValuePair<string, string>("hasScholarship", hasScholarship.ToString()),
                new KeyValuePair<string, string>("teacherId", teacherId.ToString())
            });

            _httpClient.PostAsync(path, formContent);
        }

        [HttpPost("[action]")]
        public async Task<IEnumerable<Student>> SearchStudentAsync(string school, int? studentId, string studentNumber = null, string firstName = null,
                   string lastName = null, int? teacherId = null, bool? hasScholarship = default(bool?))
        {
            var results = new List<Student>();
            var path = $"api/Students/search";

            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("school", _tenantConfiguration.Tenant),
                new KeyValuePair<string, string>("studentId", studentId.ToString()),
                new KeyValuePair<string, string>("studentNumber", studentNumber),
                new KeyValuePair<string, string>("firstName", firstName),
                new KeyValuePair<string, string>("lastName", lastName),
                new KeyValuePair<string, string>("teacherId", teacherId.ToString()),
                new KeyValuePair<string, string>("hasScholarship", hasScholarship.ToString())
            });

            HttpResponseMessage response = await _httpClient.PostAsync(path, formContent);

            if (response.IsSuccessStatusCode)
            {
                results = await response.Content.ReadAsAsync<List<Student>>();
            }

            return results;
        }
    }
}
