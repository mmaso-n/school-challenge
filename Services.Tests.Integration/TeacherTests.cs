using Contracts;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage.Table;
using SchoolChallenge.Fixtures;
using SchoolChallenge.Repository;
using SchoolChallenge.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SchoolChallenge.Tests.Integration
{
    public class TeacherTests
    {
        private readonly string _repositoryConnectionString;

        public TeacherTests()
        {
            // Bind test configuration
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var testConfig = new TestConfiguration();
            config.GetSection("RepositorySettings").Bind(testConfig);

            _repositoryConnectionString = testConfig.StorageConnectionString;
        }

        [Fact]
        public async void TestGetAllTeachersWithPagination()
        {
            // arrange
            var mockRecordCount = 2277;
            var mockSchoolName = "test school name goes here";

            var _configuration = new Config
            {
                StorageConnectionString = _repositoryConnectionString,
                TeacherTable = "testteachertablepaging"
            };

            var classUnderTest = new DataRepository(_configuration);

            using (var fixture = new TableFixture(_configuration.StorageConnectionString, _configuration.TeacherTable))
            {
                fixture.Table.Should().NotBeNull();

                // build mock data
                Enumerable.Range(1, mockRecordCount)
                    .Select(i => new TeacherEntity
                    {
                        PartitionKey = mockSchoolName,
                        RowKey = i.ToString(),
                        FirstName = Guid.NewGuid().ToString(),
                        LastName = Guid.NewGuid().ToString()
                    })
                    .ToList()
                    .ForEach(rec => { fixture.Table.ExecuteAsync(TableOperation.Insert(rec)).GetAwaiter().GetResult(); });

                // act                
                RepositoryContinationToken tableContinuationToken = null;
                var results = new List<Teacher>();

                do
                {
                    var result = await classUnderTest.GetAllTeachersAsync(mockSchoolName, tableContinuationToken);
                    tableContinuationToken = result.ContinuationToken;
                    results.AddRange(result.Results);
                }
                while (tableContinuationToken.Value != null);

                // assert
                results.Should().HaveCount(mockRecordCount);
            }
        }

        [Fact]
        public async void TestGetAllTeachersWithoutPagination()
        {
            // arrange
            var mockRecordCount = 8;
            var mockSchoolName = "test school name goes here";

            var _configuration = new Config
            {
                StorageConnectionString = _repositoryConnectionString,
                TeacherTable = "testteachertablenopaging"
            };

            var classUnderTest = new DataRepository(_configuration);

            using (var fixture = new TableFixture(_configuration.StorageConnectionString, _configuration.TeacherTable))
            {
                fixture.Table.Should().NotBeNull();

                // build mock data
                Enumerable.Range(1, mockRecordCount)
                    .Select(i => new TeacherEntity
                    {
                        PartitionKey = mockSchoolName,
                        RowKey = i.ToString(),
                        FirstName = Guid.NewGuid().ToString(),
                        LastName = Guid.NewGuid().ToString()
                    })
                    .ToList()
                    .ForEach(rec => { fixture.Table.ExecuteAsync(TableOperation.Insert(rec)).GetAwaiter().GetResult(); });

                // act                
                RepositoryContinationToken tableContinuationToken = null;
                var results = new List<Teacher>();

                do
                {
                    var result = await classUnderTest.GetAllTeachersAsync(mockSchoolName, tableContinuationToken);
                    tableContinuationToken = result.ContinuationToken;
                    results.AddRange(result.Results);
                }
                while (tableContinuationToken.Value != null);

                // assert
                results.Should().HaveCount(mockRecordCount);
            }
        }

        [Fact]
        public async void TestUpsertTeacher()
        {
            // arrange
            var mockSchoolName = "test school name goes here";

            var _configuration = new Config
            {
                StorageConnectionString = _repositoryConnectionString,
                TeacherTable = "testupsertteacher"
            };

            var classUnderTest = new DataRepository(_configuration);

            using (var fixture = new TableFixture(_configuration.StorageConnectionString, _configuration.TeacherTable))
            {
                fixture.Table.Should().NotBeNull();

                // perform initial write
                var record = new Teacher
                {
                    School = mockSchoolName,
                    Id = 12345,
                    FirstName = "Mike",
                    LastName = "Smith"
                };

                classUnderTest.UpsertTeacherAsync(record).GetAwaiter().GetResult();

                var initialResult = await classUnderTest.GetAllTeachersAsync(mockSchoolName);
                initialResult.Results.Should().ContainSingle();
                initialResult.Results.Single().School.Should().BeEquivalentTo(mockSchoolName);
                initialResult.Results.Single().Id.ShouldBeEquivalentTo(12345);
                initialResult.Results.Single().FirstName.Should().BeEquivalentTo("Mike");
                initialResult.Results.Single().LastName.Should().BeEquivalentTo("Smith");

                // perform update
                record = new Teacher
                {
                    School = mockSchoolName,
                    Id = 12345,
                    FirstName = "Michael",
                    LastName = "Smith",
                };

                classUnderTest.UpsertTeacherAsync(record).GetAwaiter().GetResult();

                var updateResult = await classUnderTest.GetAllTeachersAsync(mockSchoolName);
                updateResult.Results.Should().ContainSingle();
                updateResult.Results.Single().School.Should().BeEquivalentTo(mockSchoolName);
                updateResult.Results.Single().Id.ShouldBeEquivalentTo(12345);
                updateResult.Results.Single().FirstName.Should().BeEquivalentTo("Michael");
                updateResult.Results.Single().LastName.Should().BeEquivalentTo("Smith");

                // perform another update
                record = new Teacher
                {
                    School = mockSchoolName,
                    Id = 12345,
                    FirstName = "That's Mr. Michael",
                    LastName = "Smith to you"
                };

                classUnderTest.UpsertTeacherAsync(record).GetAwaiter().GetResult();

                var subsequentUpdateResult = await classUnderTest.GetAllTeachersAsync(mockSchoolName);
                subsequentUpdateResult.Results.Should().ContainSingle();
                subsequentUpdateResult.Results.Single().School.Should().BeEquivalentTo(mockSchoolName);
                updateResult.Results.Single().Id.ShouldBeEquivalentTo(12345);
                subsequentUpdateResult.Results.Single().FirstName.Should().BeEquivalentTo("That's Mr. Michael");
                subsequentUpdateResult.Results.Single().LastName.Should().BeEquivalentTo("Smith to you");
            }
        }

        [Fact]
        public async void TestDeleteTeacher()
        {
            // arrange
            var mockSchoolName = "test school name goes here";

            var _configuration = new Config
            {
                StorageConnectionString = _repositoryConnectionString,
                TeacherTable = "testdeleteteacher"
            };

            var classUnderTest = new DataRepository(_configuration);

            using (var fixture = new TableFixture(_configuration.StorageConnectionString, _configuration.TeacherTable))
            {
                fixture.Table.Should().NotBeNull();

                var TeacherOne = new Teacher
                {
                    School = mockSchoolName,
                    Id = 12345,
                    FirstName = "Mike",
                    LastName = "Smith"
                };

                var TeacherTwo = new Teacher
                {
                    School = mockSchoolName,
                    Id = 7789,
                    FirstName = "Mary",
                    LastName = "Doe"
                };

                var TeacherThree = new Teacher
                {
                    School = mockSchoolName,
                    Id = 4411,
                    FirstName = "Meridith",
                    LastName = "Smith"
                };

                classUnderTest.UpsertTeacherAsync(TeacherOne).GetAwaiter().GetResult();
                classUnderTest.UpsertTeacherAsync(TeacherTwo).GetAwaiter().GetResult();
                classUnderTest.UpsertTeacherAsync(TeacherThree).GetAwaiter().GetResult();

                // act
                await classUnderTest.DeleteTeacherAsync(TeacherTwo);

                // assert
                var result = await classUnderTest.GetAllTeachersAsync(mockSchoolName);
                result.Results.Should().HaveCount(2);
                result.Results.Should().Contain(x => x.School == TeacherOne.School && x.Id == TeacherOne.Id);
                result.Results.Should().Contain(x => x.School == TeacherThree.School && x.Id == TeacherThree.Id);
            }
        }
    }
}
