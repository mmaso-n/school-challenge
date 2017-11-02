using FluentAssertions;
using Microsoft.WindowsAzure.Storage.Table;
using SchoolChallenge.Contracts;
using SchoolChallenge.Repository;
using SchoolChallenge.Repository.Entities;
using Xunit;

namespace Repository.Tests
{
    public class UtilitiesExtensionsTests
    {
        [Fact]
        public void TestRepositoryContinationTokenTokenToRepositoryImplementation()
        {
            var value = new TableContinuationToken
            {
                NextPartitionKey = "#!26680f",
                NextRowKey = "da33!",
                NextTableName = "mytable",
                TargetLocation = Microsoft.WindowsAzure.Storage.StorageLocation.Primary
            };

            var repositoryContinationToken = new RepositoryContinationToken();
            repositoryContinationToken.Value = value;

            var result = repositoryContinationToken.ToRepositoryImplementation();

            result.Should().BeSameAs(value);
        }

        [Fact]
        public void TestRepositoryContinationTokenTokenToRepositoryImplementation_Given_Null()
        {
            var repositoryContinationToken = new RepositoryContinationToken();
            var result = repositoryContinationToken.ToRepositoryImplementation();

            result.Should().BeNull();
        }

        [Theory]
        [InlineData("some invalid input goes here")]
        [InlineData(1234)]
        [InlineData(1234123003412341234)]
        [InlineData(99.99)]
        public void TestRepositoryContinationTokenTokenToRepositoryImplementation_Given_UnsupporedValue(object value)
        {
            var repositoryContinationToken = new RepositoryContinationToken();
            repositoryContinationToken.Value = value;

            var result = repositoryContinationToken.ToRepositoryImplementation();

            result.Should().BeNull();
        }

        [Fact]
        public void TestStudentContractToStudentEntity()
        {
            var student = new Student
            {
                School = "MPS", 
                Id = 770,
                Number = "11009-B",
                FirstName = "Johnny", 
                LastName = "Cash", 
                HasScholarship = true, 
                TeacherId = 4
            };

            var result = student.ToRepositoryEntity();

            result.PartitionKey.Should().BeEquivalentTo("MPS");
            result.RowKey.Should().BeEquivalentTo("770");
            result.Number.Should().BeEquivalentTo("11009-B");
            result.FirstName.Should().BeEquivalentTo("Johnny");
            result.LastName.Should().BeEquivalentTo("Cash");
            result.HasScholarship.Should().BeTrue();
            result.TeacherId.ShouldBeEquivalentTo(4);
        }

        [Fact]
        public void TestStudentEntityToStudentContract()
        {
            var student = new StudentEntity
            {
                PartitionKey = "MPS",
                RowKey = "770",
                Number = "11009-B",
                FirstName = "Johnny",
                LastName = "Cash",
                HasScholarship = true,
                TeacherId = 4
            };

            var result = student.ToStudent();

            result.School.Should().BeEquivalentTo("MPS");
            result.Id.ShouldBeEquivalentTo(770);
            result.Number.Should().BeEquivalentTo("11009-B");
            result.FirstName.Should().BeEquivalentTo("Johnny");
            result.LastName.Should().BeEquivalentTo("Cash");
            result.HasScholarship.Should().BeTrue();
            result.TeacherId.ShouldBeEquivalentTo(4);
        }

        [Fact]
        public void TestTeacherContractToTeacherEntity()
        {
            var teacher = new Teacher
            {
                School = "MPS", 
                Id = 4009, 
                FirstName = "Johnny", 
                LastName = "Cash"
            };

            var result = teacher.ToRepositoryEntity();

            result.PartitionKey.Should().BeEquivalentTo("MPS");
            result.RowKey.Should().BeEquivalentTo("4009");
            result.FirstName.Should().BeEquivalentTo("Johnny");
            result.LastName.Should().BeEquivalentTo("Cash");
        }

        [Fact]
        public void TestTeacherEntityToTeacherContract()
        {
            var teacher = new TeacherEntity
            {
                PartitionKey = "MPS",
                RowKey = "4009",
                FirstName = "Johnny",
                LastName = "Cash"
            };

            var result = teacher.ToTeacher();

            result.School.Should().BeEquivalentTo("MPS");
            result.Id.ShouldBeEquivalentTo(4009);
            result.FirstName.Should().BeEquivalentTo("Johnny");
            result.LastName.Should().BeEquivalentTo("Cash");
        }
    }
}
