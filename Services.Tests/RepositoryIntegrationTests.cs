using FluentAssertions;
using Microsoft.WindowsAzure.Storage.Table;
using SchoolChallenge;
using SchoolChallenge.Fixtures;
using SchoolChallenge.Repository;
using SchoolChallenge.Repository.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Services.Tests.Integration
{
    public class RepositoryIntegrationTests
    {
        string _storeConnectionString = "UseDevelopmentStorage=true";

        [Fact]
        public async void TestGetStudents()
        {
            // arrange
            var mockRecordCount = 2500;
            var mockSchoolName = "test school name goes here";

            var _configuration = new Config
            {
                StorageConnectionString = _storeConnectionString,
                StudentTable = "teststudenttable"
            };

            var classUnderTest = new DataRepository(_configuration);

            using (var fixture = new TableFixture(_configuration.StorageConnectionString, _configuration.StudentTable))
            {
                fixture.Table.Should().NotBeNull();

                // build mock data
                Enumerable.Range(1, mockRecordCount)
                    .Select(i => new Student
                    {
                        PartitionKey = mockSchoolName,
                        RowKey = i.ToString(),
                        FirstName = Guid.NewGuid().ToString(),
                        LastName = Guid.NewGuid().ToString()
                    })
                    .ToList()
                    .ForEach(rec => { fixture.Table.ExecuteAsync(TableOperation.Insert(rec)).GetAwaiter().GetResult(); });

                // act                
                TableContinuationToken tableContinuationToken = null;
                var results = new List<Student>();

                do
                {
                    var result = await classUnderTest.GetStudentsAsync(mockSchoolName, tableContinuationToken);
                    tableContinuationToken = result.ContinuationToken;
                    results.AddRange(result);
                }
                while (tableContinuationToken != null);

                // assert
                results.Should().HaveCount(mockRecordCount);
            }
        }

        [Fact]
        public async void TestGetTeachers()
        {
            // arrange
            var mockRecordCount = 2277;
            var mockSchoolName = "test school name goes here";

            var _configuration = new Config
            {
                StorageConnectionString = _storeConnectionString,
                TeacherTable = "testteachertable"
            };

            var classUnderTest = new DataRepository(_configuration);

            using (var fixture = new TableFixture(_configuration.StorageConnectionString, _configuration.TeacherTable))
            {
                fixture.Table.Should().NotBeNull();

                // build mock data
                Enumerable.Range(1, mockRecordCount)
                    .Select(i => new Teacher
                    {
                        PartitionKey = mockSchoolName,
                        RowKey = i.ToString(),
                        FirstName = Guid.NewGuid().ToString(),
                        LastName = Guid.NewGuid().ToString()
                    })
                    .ToList()
                    .ForEach(rec => { fixture.Table.ExecuteAsync(TableOperation.Insert(rec)).GetAwaiter().GetResult(); });

                // act                
                TableContinuationToken tableContinuationToken = null;
                var results = new List<Teacher>();

                do
                {
                    var result = await classUnderTest.GetTeachersAsync(mockSchoolName, tableContinuationToken);
                    tableContinuationToken = result.ContinuationToken;
                    results.AddRange(result);
                }
                while (tableContinuationToken != null);

                // assert
                results.Should().HaveCount(mockRecordCount);
            }
        }

        [Fact]
        public async void TestUpsert()
        {
            // arrange
            var mockSchoolName = "test school name goes here";

            var _configuration = new Config
            {
                StorageConnectionString = _storeConnectionString,
                StudentTable = "foobar"
            };

            var classUnderTest = new DataRepository(_configuration);

            using (var fixture = new TableFixture(_configuration.StorageConnectionString, _configuration.StudentTable))
            {
                fixture.Table.Should().NotBeNull();

                // perform initial write
                var record = new Student
                {
                    PartitionKey = mockSchoolName,
                    RowKey = "12345",
                    FirstName = "Mike",
                    LastName = "Smith",
                    HasScholarship = false,
                    TeacherId = 1234
                };

                classUnderTest.UpsertAsync(_configuration.StudentTable, record).GetAwaiter().GetResult();

                var initialResult = await classUnderTest.GetStudentsAsync(mockSchoolName);
                initialResult.Should().ContainSingle();
                initialResult.Single().PartitionKey.Should().BeEquivalentTo(mockSchoolName);
                initialResult.Single().RowKey.Should().BeEquivalentTo("12345");
                initialResult.Single().FirstName.Should().BeEquivalentTo("Mike");
                initialResult.Single().LastName.Should().BeEquivalentTo("Smith");
                initialResult.Single().HasScholarship.Should().BeFalse();
                initialResult.Single().TeacherId.Should().Be(1234);

                // perform update
                record = new Student
                {
                    PartitionKey = mockSchoolName,
                    RowKey = "12345",
                    FirstName = "Mike",
                    LastName = "Smith",
                    HasScholarship = true,
                    TeacherId = 999
                };

                classUnderTest.UpsertAsync(_configuration.StudentTable, record).GetAwaiter().GetResult();

                var updateResult = await classUnderTest.GetStudentsAsync(mockSchoolName);
                updateResult.Should().ContainSingle();
                updateResult.Single().PartitionKey.Should().BeEquivalentTo(mockSchoolName);
                updateResult.Single().RowKey.Should().BeEquivalentTo("12345");
                updateResult.Single().FirstName.Should().BeEquivalentTo("Mike");
                updateResult.Single().LastName.Should().BeEquivalentTo("Smith");
                updateResult.Single().HasScholarship.Should().BeTrue();
                updateResult.Single().TeacherId.Should().Be(999);

                // perform another update
                record = new Student
                {
                    PartitionKey = mockSchoolName,
                    RowKey = "12345",
                    FirstName = "That's Mr. Mike",
                    LastName = "Smith to you",
                    HasScholarship = true,
                    TeacherId = 999
                };

                classUnderTest.UpsertAsync(_configuration.StudentTable, record).GetAwaiter().GetResult();

                var subsequentUpdateResult = await classUnderTest.GetStudentsAsync(mockSchoolName);
                subsequentUpdateResult.Should().ContainSingle();
                subsequentUpdateResult.Single().PartitionKey.Should().BeEquivalentTo(mockSchoolName);
                subsequentUpdateResult.Single().RowKey.Should().BeEquivalentTo("12345");
                subsequentUpdateResult.Single().FirstName.Should().BeEquivalentTo("That's Mr. Mike");
                subsequentUpdateResult.Single().LastName.Should().BeEquivalentTo("Smith to you");
                subsequentUpdateResult.Single().HasScholarship.Should().BeTrue();
                subsequentUpdateResult.Single().TeacherId.Should().Be(999);
            }
        }

        [Fact]
        public async void TestDelete()
        {
            // arrange
            var mockSchoolName = "test school name goes here";

            var _configuration = new Config
            {
                StorageConnectionString = _storeConnectionString,
                StudentTable = "foobarbaz"
            };

            var classUnderTest = new DataRepository(_configuration);

            using (var fixture = new TableFixture(_configuration.StorageConnectionString, _configuration.StudentTable))
            {
                fixture.Table.Should().NotBeNull();

                var studentOne = new Student
                {
                    PartitionKey = mockSchoolName,
                    RowKey = "12345",
                    FirstName = "Mike",
                    LastName = "Smith",
                    HasScholarship = false,
                    TeacherId = 1234
                };

                var studentTwo = new Student
                {
                    PartitionKey = mockSchoolName,
                    RowKey = "7789",
                    FirstName = "Mary",
                    LastName = "Doe",
                    HasScholarship = true,
                    TeacherId = 1234
                };

                var studentThree = new Student
                {
                    PartitionKey = mockSchoolName,
                    RowKey = "4411",
                    FirstName = "Meridith",
                    LastName = "Smith",
                    HasScholarship = true,
                    TeacherId = 4488
                };

                classUnderTest.UpsertAsync(_configuration.StudentTable, studentOne).GetAwaiter().GetResult();
                classUnderTest.UpsertAsync(_configuration.StudentTable, studentTwo).GetAwaiter().GetResult();
                classUnderTest.UpsertAsync(_configuration.StudentTable, studentThree).GetAwaiter().GetResult();

                // act
                await classUnderTest.DeleteAsync(_configuration.StudentTable, studentTwo);

                // assert
                var result = await classUnderTest.GetStudentsAsync(mockSchoolName);
                result.Should().HaveCount(2);
                result.Should().Contain(x => x.PartitionKey == studentOne.PartitionKey && x.RowKey == studentOne.RowKey);
                result.Should().Contain(x => x.PartitionKey == studentThree.PartitionKey && x.RowKey == studentThree.RowKey);
            }
        }
    }
}
