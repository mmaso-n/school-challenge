using Moq;
using SchoolChallenge.Contracts;
using SchoolChallenge.Repository;
using SchoolChallenge.Services.Controllers;
using Xunit;
using static Services.Tests.TestHelpers;

namespace Services.Tests.Features
{
    public class TeachersControllerTests
    {
        const string SchoolName = "Milwaukee Public Schools";

        [Fact]
        public async void TestGetAllAsync()
        {
            var mockDataRepo = new Mock<IDataRepository>();

            mockDataRepo.Setup(m => m.GetAllTeachersAsync(SchoolName, It.IsAny<RepositoryContinationToken>()))
                .Returns(GetMockQueryResult<Teacher>());

            var classUnderTest = new TeachersController(mockDataRepo.Object);

            await classUnderTest.GetAllAsync(SchoolName);

            mockDataRepo.Verify(x => x.GetAllTeachersAsync(SchoolName, It.IsAny<RepositoryContinationToken>()), Times.Once);
        }

        [Fact]
        public async void TestSearchAsync()
        {
            var mockDataRepo = new Mock<IDataRepository>();

            mockDataRepo.Setup(m => m.SearchTeachersAsync(SchoolName, It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), null))
                .Returns(GetMockQueryResult<Teacher>());

            var classUnderTest = new TeachersController(mockDataRepo.Object);

            await classUnderTest.SearchAsync(SchoolName, 123, "Mike", "Mason");

            mockDataRepo.Verify(x => x.SearchTeachersAsync(SchoolName, It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), null),
                Times.Once);
        }

        [Fact]
        public async void TestInsertAsync()
        {
            var mockDataRepo = new Mock<IDataRepository>();

            var classUnderTest = new TeachersController(mockDataRepo.Object);

            await classUnderTest.InsertAsync("Milwaukee Public Schools", 1, "Mike", "Mason");

            mockDataRepo.Verify(x => x.UpsertTeacherAsync(It.IsAny<Teacher>()), Times.Once);
        }

        [Fact]
        public async void TestUpdateAsync()
        {
            var mockDataRepo = new Mock<IDataRepository>();

            var classUnderTest = new TeachersController(mockDataRepo.Object);

            await classUnderTest.UpdateAsync("Milwaukee Public Schools", 1, "Mike", "Mason");

            mockDataRepo.Verify(x => x.UpsertTeacherAsync(It.IsAny<Teacher>()), Times.Once);
        }

        [Fact]
        public async void TestDeleteAsync()
        {
            var mockDataRepo = new Mock<IDataRepository>();

            var classUnderTest = new TeachersController(mockDataRepo.Object);

            await classUnderTest.DeleteAsync("Milwaukee Public Schools", 1, "Mike", "Mason");

            mockDataRepo.Verify(x => x.DeleteTeacherAsync(It.IsAny<Teacher>()), Times.Once);
        }
    }
}
