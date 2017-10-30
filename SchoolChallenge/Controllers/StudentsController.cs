using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SchoolChallenge.Repository;
using SchoolChallenge.Repository.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolChallenge.Controllers
{
    [Route("api/[controller]")]
    public class StudentsController : Controller
    {
        private readonly Config _settings;
        private readonly IDataRepository _dataRepository;

        public StudentsController(IOptions<Config> settings, IDataRepository dataRepository)
        {
            _settings = settings.Value;
            _dataRepository = dataRepository;
        }

        [HttpGet("{school}")]
        public async Task<IEnumerable<string>> Get(string school)
        {
            var students = await _dataRepository.GetStudentsAsync(school, null);

            return new[] { students.Count().ToString() };
        }
    }
}
