using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolChallenge.Client.Dependencies;
using SchoolChallenge.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using static SchoolChallenge.Client.Functions.DataUploadFunctions;

namespace SchoolChallenge.Client.Controllers
{
    [Route("api/[controller]")]
    public class UploadController : Controller
    {
        IHttpClient _httpClient;
        ITenantConfiguration _tenantConfiguration;

        public UploadController(IHttpClient httpClient, ITenantConfiguration tenantConfiguration)
        {
            _httpClient = httpClient;
            _tenantConfiguration = tenantConfiguration;
        }

        [HttpPost("[action]")]
        public async void UploadFileAsync(IFormFile file, string dataType)
        {
            if (file == null) throw new Exception("File is null");
            if (file.Length == 0) throw new Exception("File is empty");

            var path = $"api/{dataType}/insert";
            var response = new HttpResponseMessage();

            switch (dataType)
            {
                case "Students":
                    List<Student> studentsToUpload;

                    using (var stream = file.OpenReadStream())
                    {
                        studentsToUpload =
                            ParseStudentDataFile(stream, _tenantConfiguration.Tenant)
                            .ToList();
                    }

                    if (studentsToUpload != null)
                    {
                        foreach (var recordToUpload in studentsToUpload)
                        {
                            var formContent = new FormUrlEncodedContent(new[]
                            {
                                new KeyValuePair<string, string>("school", _tenantConfiguration.Tenant),
                                new KeyValuePair<string, string>("id", recordToUpload.Id.ToString()),
                                new KeyValuePair<string, string>("number", recordToUpload.Number),
                                new KeyValuePair<string, string>("firstName", recordToUpload.FirstName),
                                new KeyValuePair<string, string>("lastName", recordToUpload.LastName),
                                new KeyValuePair<string, string>("hasScholarship", recordToUpload.HasScholarship.ToString()),
                                new KeyValuePair<string, string>("teacherId", recordToUpload.TeacherId.ToString())
                            });
                            response = await _httpClient.PostAsync(path, formContent);
                        }
                    }
                    break;

                case "Teachers":
                    List<Teacher> teachersToUpload;

                    using (var stream = file.OpenReadStream())
                    {
                        teachersToUpload =
                            ParseTeacherDataFile(stream, _tenantConfiguration.Tenant)
                            .ToList();
                    }

                    if (teachersToUpload != null)
                    {
                        foreach (var recordToUpload in teachersToUpload)
                        {
                            var formContent = new FormUrlEncodedContent(new[]
                            {
                                new KeyValuePair<string, string>("school", _tenantConfiguration.Tenant),
                                new KeyValuePair<string, string>("id", recordToUpload.Id.ToString()),
                                new KeyValuePair<string, string>("firstName", recordToUpload.FirstName),
                                new KeyValuePair<string, string>("lastName", recordToUpload.LastName),
                            });
                            response = await _httpClient.PostAsync(path, formContent);
                        }
                    }
                    break;
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed with message: {response.StatusCode}");
            }
        }
    }
}
