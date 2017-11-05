using Microsoft.AspNetCore.Mvc;
using SchoolChallenge.Contracts;
using SchoolChallenge.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SchoolChallenge.Services.Controllers
{
    [Produces("application/json")]
    [Route("api/Students")]
    public class StudentsController : Controller
    {
        private readonly IDataRepository _dataRepository;

        public StudentsController(IDataRepository dataRepository)
        {
            _dataRepository = dataRepository;
        }

        // GET: getall/{school}
        [Route("getall/{school}")]
        [HttpGet]
        public async Task<IEnumerable<Student>> GetAllAsync(string school)
        {
            var results = new List<Student>();
            RepositoryContinationToken tableContinuationToken = null;

            do
            {
                var result = await _dataRepository.GetAllStudentsAsync(school, tableContinuationToken);
                tableContinuationToken = result.ContinuationToken;
                results.AddRange(result.Results);
            }
            while (tableContinuationToken.Value != null);

            return results;
        }

        // GET: search/{school}
        [Route("search/{school}")]
        [HttpGet]
        public async Task<IEnumerable<Student>> SearchAsync(string school, int? studentId, string studentNumber = null, string firstName = null,
                   string lastName = null, int? teacherId = null, bool? hasScholarship = default(bool?))
        {
            var results = new List<Student>();
            RepositoryContinationToken tableContinuationToken = null;

            do
            {
                var result = await _dataRepository.SearchStudentsAsync(school, studentId, studentNumber, firstName, lastName, teacherId, hasScholarship, tableContinuationToken);
                tableContinuationToken = result.ContinuationToken;
                results.AddRange(result.Results);
            }
            while (tableContinuationToken.Value != null);

            return results;
        }

        // POST: insert/{school}/{id}/{number}/{firstName}/{lastName}/{hasScholarship}/{teacherId}
        [Route("insert")]
        [HttpPost]
        public async Task<IActionResult> InsertAsync(string school, int id, string number, string firstName, string lastName, bool hasScholarship, int teacherId)
        {
            var toInsert = new Student
            {
                School = school, 
                Id = id,
                Number = number,
                FirstName = firstName,
                LastName = lastName,
                HasScholarship = hasScholarship,
                TeacherId = teacherId
            };

            await _dataRepository.UpsertStudentAsync(toInsert);

            return Ok();
        }

        // POST: update/{school}/{id}/{number}/{firstName}/{lastName}/{hasScholarship}/{teacherId}
        [Route("update")]
        [HttpPost]
        public async Task<IActionResult> UpdateAsync(string school, int id, string number, string firstName, string lastName, bool hasScholarship, int teacherId)
        {
            var toUpdate = new Student
            {
                School = school,
                Id = id,
                Number = number,
                FirstName = firstName,
                LastName = lastName,
                HasScholarship = hasScholarship,
                TeacherId = teacherId
            };

            await _dataRepository.UpsertStudentAsync(toUpdate);

            return Ok();
        }

        // POST: delete/{school}/{id}/{number}/{firstName}/{lastName}/{hasScholarship}/{teacherId}
        [Route("delete")]
        [HttpPost]
        public async Task<IActionResult> DeleteAsync(string school, int id, string number, string firstName, string lastName, bool hasScholarship, int teacherId)
        {
            var toDelete = new Student
            {
                School = school,
                Id = id,
                Number = number,
                FirstName = firstName,
                LastName = lastName,
                HasScholarship = hasScholarship,
                TeacherId = teacherId
            };

            await _dataRepository.DeleteStudentAsync(toDelete);

            return Ok();
        }
    }
}