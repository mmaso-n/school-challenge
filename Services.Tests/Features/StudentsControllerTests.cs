using Moq;
using SchoolChallenge.Contracts;
using SchoolChallenge.Repository;
using SchoolChallenge.Services.Controllers;
using Xunit;
using static Services.Tests.TestHelpers;

namespace Services.Tests.Features
{
    public class StudentsControllerTests
    {
        const string SchoolName = "Milwaukee Public Schools";

        [Fact]
        public async void TestGetAllAsync()
        {
            var mockDataRepo = new Mock<IDataRepository>();

            mockDataRepo.Setup(m => m.GetAllStudentsAsync(SchoolName, It.IsAny<RepositoryContinationToken>()))
                .Returns(GetMockQueryResult<Student>());

            var classUnderTest = new StudentsController(mockDataRepo.Object);

            await classUnderTest.GetAllAsync(SchoolName);

            mockDataRepo.Verify(x => x.GetAllStudentsAsync(SchoolName, It.IsAny<RepositoryContinationToken>()), Times.Once);
        }

        [Fact]
        public async void TestSearchAsync()
        {
            var mockDataRepo = new Mock<IDataRepository>();

            mockDataRepo.Setup(m => m.SearchStudentsAsync(SchoolName, It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<bool>(), null))
                .Returns(GetMockQueryResult<Student>());

            var classUnderTest = new StudentsController(mockDataRepo.Object);

            await classUnderTest.SearchAsync(SchoolName, 123, "A1", "Mike", "Mason", 12, true);

            mockDataRepo.Verify(x => x.SearchStudentsAsync(SchoolName,
                It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<bool>(), null), 
                Times.Once);
        }

        [Fact]
        public async void TestInsertAsync()
        {
            var mockDataRepo = new Mock<IDataRepository>();

            var classUnderTest = new StudentsController(mockDataRepo.Object);

            await classUnderTest.InsertAsync("Milwaukee Public Schools", 1, "A1", "Mike", "Mason", true, 1);

            mockDataRepo.Verify(x => x.UpsertStudentAsync(It.IsAny<Student>()), Times.Once);
        }

        [Fact]
        public async void TestUpdateAsync()
        {
            var mockDataRepo = new Mock<IDataRepository>();

            var classUnderTest = new StudentsController(mockDataRepo.Object);

            await classUnderTest.UpdateAsync("Milwaukee Public Schools", 1, "A1", "Mike", "Mason", true, 1);

            mockDataRepo.Verify(x => x.UpsertStudentAsync(It.IsAny<Student>()), Times.Once);
        }

        [Fact]
        public async void TestDeleteAsync()
        {
            var mockDataRepo = new Mock<IDataRepository>();

            var classUnderTest = new StudentsController(mockDataRepo.Object);

            await classUnderTest.DeleteAsync("Milwaukee Public Schools", 1, "A1", "Mike", "Mason", true, 1);

            mockDataRepo.Verify(x => x.DeleteStudentAsync(It.IsAny<Student>()), Times.Once);
        }
    }
}
