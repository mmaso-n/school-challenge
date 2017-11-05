using Microsoft.AspNetCore.Mvc;
using SchoolChallenge.Contracts;
using SchoolChallenge.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SchoolChallenge.Services.Controllers
{
    [Produces("application/json")]
    [Route("api/Teachers")]
    public class TeachersController : Controller
    {
        private readonly IDataRepository _dataRepository;

        public TeachersController(IDataRepository dataRepository)
        {
            _dataRepository = dataRepository;
        }

        // GET: getall/{school}
        [Route("getall/{school}")]
        [HttpGet]
        public async Task<IEnumerable<Teacher>> GetAllAsync(string school)
        {
            var results = new List<Teacher>();
            RepositoryContinationToken tableContinuationToken = null;

            do
            {
                var result = await _dataRepository.GetAllTeachersAsync(school, tableContinuationToken);
                tableContinuationToken = result.ContinuationToken;
                results.AddRange(result.Results);
            }
            while (tableContinuationToken.Value != null);

            return results;
        }

        // GET: search/{school}
        [Route("search/{school}")]
        [HttpGet]
        public async Task<IEnumerable<Teacher>> SearchAsync(string school, int? teacherId = null, string firstName = null,
                   string lastName = null)
        {
            var results = new List<Teacher>();
            RepositoryContinationToken tableContinuationToken = null;

            do
            {
                var result = await _dataRepository.SearchTeachersAsync(school, teacherId, lastName, firstName, tableContinuationToken);
                tableContinuationToken = result.ContinuationToken;
                results.AddRange(result.Results);
            }
            while (tableContinuationToken.Value != null);

            return results;
        }

        // POST: insert/{school}/{id}/{firstName}/{lastName}}
        [Route("insert")]
        [HttpPost]
        public async Task<IActionResult> InsertAsync(string school, int id, string firstName, string lastName)
        {
            var toInsert = new Teacher
            {
                School = school,
                Id = id,
                FirstName = firstName,
                LastName = lastName
            };

            await _dataRepository.UpsertTeacherAsync(toInsert);

            return Ok();
        }

        // POST: update/{school}/{id}/{firstName}/{lastName}}
        [Route("update")]
        [HttpPost]
        public async Task<IActionResult> UpdateAsync(string school, int id, string firstName, string lastName)
        {
            var toUpdate = new Teacher
            {
                School = school,
                Id = id,
                FirstName = firstName,
                LastName = lastName
            };

            await _dataRepository.UpsertTeacherAsync(toUpdate);

            return Ok();
        }

        // DELETE: delete/{school}/{id}/{firstName}/{lastName}
        [HttpDelete("delete/{school}/{id}/{firstName}/{lastName}")]
        public async Task<IActionResult> DeleteAsync(string school, int id, string firstName, string lastName)
        {
            var toDelete = new Teacher
            {
                School = school,
                Id = id,
                FirstName = firstName,
                LastName = lastName
            };

            await _dataRepository.DeleteTeacherAsync(toDelete);

            return Ok();
        }
    }
}
