﻿using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage.Table;
using SchoolChallenge.Contracts;
using SchoolChallenge.Fixtures;
using SchoolChallenge.Repository;
using SchoolChallenge.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Repository.Tests.Integration
{
    public class StudentTests
    {
        private readonly string _repositoryConnectionString;

        public StudentTests()
        {
            // Bind test configuration
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var testConfig = new TestConfiguration();
            config.GetSection("RepositorySettings").Bind(testConfig);

            _repositoryConnectionString = testConfig.StorageConnectionString;
        }        

        [Fact(Skip ="Run integration tests on-demand as necessary.")]
        public async void TestGetAllStudentsWithPagination()
        {
            // arrange
            var mockRecordCount = 2411;
            var mockSchoolName = "test school name goes here";
            var testTableName = "teststudenttablepaging";

            var classUnderTest = new DataRepository(_repositoryConnectionString, testTableName, null);

            using (var fixture = new TableFixture(_repositoryConnectionString, testTableName))
            {
                fixture.Table.Should().NotBeNull();

                // build mock data
                Enumerable.Range(1, mockRecordCount)
                    .Select(i => new StudentEntity
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
                var results = new List<Student>();

                do
                {
                    var result = await classUnderTest.GetAllStudentsAsync(mockSchoolName, tableContinuationToken);
                    tableContinuationToken = result.ContinuationToken;
                    results.AddRange(result.Results);
                }
                while (tableContinuationToken.Value != null);

                // assert
                results.Should().HaveCount(mockRecordCount);
            }
        }

        [Fact(Skip = "Run integration tests on-demand as necessary.")]
        public async void TestGetAllStudentsWithoutPagination()
        {
            // arrange
            var mockRecordCount = 309;
            var mockSchoolName = "test school name goes here";
            var testTableName = "teststudenttablenopaging";

            var classUnderTest = new DataRepository(_repositoryConnectionString, testTableName, null);

            using (var fixture = new TableFixture(_repositoryConnectionString, testTableName))
            {
                fixture.Table.Should().NotBeNull();

                // build mock data
                Enumerable.Range(1, mockRecordCount)
                    .Select(i => new StudentEntity
                    {
                        PartitionKey = mockSchoolName,
                        RowKey = i.ToString(),
                        FirstName = Guid.NewGuid().ToString(),
                        LastName = Guid.NewGuid().ToString()
                    })
                    .ToList()
                    .ForEach(rec => { fixture.Table.ExecuteAsync(TableOperation.Insert(rec)).GetAwaiter().GetResult(); });

                // act                
                var result = await classUnderTest.GetAllStudentsAsync(mockSchoolName);

                // assert
                result.Results.Should().HaveCount(mockRecordCount);
                result.ContinuationToken.Value.Should().BeNull();
            }
        }

        [Fact(Skip = "Run integration tests on-demand as necessary.")]
        public async void TestUpsertStudent()
        {
            // arrange
            var mockSchoolName = "test school name goes here";
            var testTableName = "testupsertstudent";

            var classUnderTest = new DataRepository(_repositoryConnectionString, testTableName, null);

            using (var fixture = new TableFixture(_repositoryConnectionString, testTableName))
            {
                fixture.Table.Should().NotBeNull();

                // perform initial write
                var record = new Student
                {
                    School = mockSchoolName,
                    Id = 12345,
                    FirstName = "Mike",
                    LastName = "Smith",
                    HasScholarship = false,
                    TeacherId = 1234
                };

                classUnderTest.UpsertStudentAsync(record).GetAwaiter().GetResult();

                var initialResult = await classUnderTest.GetAllStudentsAsync(mockSchoolName);
                initialResult.Results.Should().ContainSingle();
                initialResult.Results.Single().School.Should().BeEquivalentTo(mockSchoolName);
                initialResult.Results.Single().Id.ShouldBeEquivalentTo(12345);
                initialResult.Results.Single().FirstName.Should().BeEquivalentTo("Mike");
                initialResult.Results.Single().LastName.Should().BeEquivalentTo("Smith");
                initialResult.Results.Single().HasScholarship.Should().BeFalse();
                initialResult.Results.Single().TeacherId.Should().Be(1234);

                // perform update
                record = new Student
                {
                    School = mockSchoolName,
                    Id = 12345,
                    FirstName = "Mike",
                    LastName = "Smith",
                    HasScholarship = true,
                    TeacherId = 999
                };

                classUnderTest.UpsertStudentAsync(record).GetAwaiter().GetResult();

                var updateResult = await classUnderTest.GetAllStudentsAsync(mockSchoolName);
                updateResult.Results.Should().ContainSingle();
                updateResult.Results.Single().School.Should().BeEquivalentTo(mockSchoolName);
                updateResult.Results.Single().Id.ShouldBeEquivalentTo(12345);
                updateResult.Results.Single().FirstName.Should().BeEquivalentTo("Mike");
                updateResult.Results.Single().LastName.Should().BeEquivalentTo("Smith");
                updateResult.Results.Single().HasScholarship.Should().BeTrue();
                updateResult.Results.Single().TeacherId.Should().Be(999);

                // perform another update
                record = new Student
                {
                    School = mockSchoolName,
                    Id = 12345,
                    FirstName = "That's Mr. Mike",
                    LastName = "Smith to you",
                    HasScholarship = true,
                    TeacherId = 999
                };

                classUnderTest.UpsertStudentAsync(record).GetAwaiter().GetResult();

                var subsequentUpdateResult = await classUnderTest.GetAllStudentsAsync(mockSchoolName);
                subsequentUpdateResult.Results.Should().ContainSingle();
                subsequentUpdateResult.Results.Single().School.Should().BeEquivalentTo(mockSchoolName);
                updateResult.Results.Single().Id.ShouldBeEquivalentTo(12345);
                subsequentUpdateResult.Results.Single().FirstName.Should().BeEquivalentTo("That's Mr. Mike");
                subsequentUpdateResult.Results.Single().LastName.Should().BeEquivalentTo("Smith to you");
                subsequentUpdateResult.Results.Single().HasScholarship.Should().BeTrue();
                subsequentUpdateResult.Results.Single().TeacherId.Should().Be(999);
            }
        }

        [Fact(Skip = "Run integration tests on-demand as necessary.")]
        public async void TestDeleteStudent()
        {
            // arrange
            var mockSchoolName = "test school name goes here";
            var testTableName = "testdeletestudent";

            var classUnderTest = new DataRepository(_repositoryConnectionString, testTableName, null);

            using (var fixture = new TableFixture(_repositoryConnectionString, testTableName))
            {
                fixture.Table.Should().NotBeNull();

                var studentOne = new Student
                {
                    School = mockSchoolName,
                    Id = 12345,
                    FirstName = "Mike",
                    LastName = "Smith",
                    HasScholarship = false,
                    TeacherId = 1234
                };

                var studentTwo = new Student
                {
                    School = mockSchoolName,
                    Id = 7789,
                    FirstName = "Mary",
                    LastName = "Doe",
                    HasScholarship = true,
                    TeacherId = 1234
                };

                var studentThree = new Student
                {
                    School = mockSchoolName,
                    Id = 4411,
                    FirstName = "Meridith",
                    LastName = "Smith",
                    HasScholarship = true,
                    TeacherId = 4488
                };

                classUnderTest.UpsertStudentAsync(studentOne).GetAwaiter().GetResult();
                classUnderTest.UpsertStudentAsync(studentTwo).GetAwaiter().GetResult();
                classUnderTest.UpsertStudentAsync(studentThree).GetAwaiter().GetResult();

                // act
                await classUnderTest.DeleteStudentAsync(studentTwo);

                // assert
                var result = await classUnderTest.GetAllStudentsAsync(mockSchoolName);
                result.Results.Should().HaveCount(2);
                result.Results.Should().Contain(x => x.School == studentOne.School && x.Id == studentOne.Id);
                result.Results.Should().Contain(x => x.School == studentThree.School && x.Id == studentThree.Id);
            }
        }

        [Fact(Skip = "Run integration tests on-demand as necessary.")]
        public async void TestSearchStudents()
        {
            // arrange
            var mockSchoolName = "test school name goes here";
            var testTableName = "testsearchstudents";

            var classUnderTest = new DataRepository(_repositoryConnectionString, testTableName, null);

            using (var fixture = new TableFixture(_repositoryConnectionString, testTableName))
            {
                fixture.Table.Should().NotBeNull();

                // build mock data
                var student1 = new Student
                {
                    School = mockSchoolName,
                    Id = 12345,
                    Number = "12345",
                    FirstName = "Mike",
                    LastName = "Smith",
                    HasScholarship = false,
                    TeacherId = 1234
                };

                var student2 = new Student
                {
                    School = mockSchoolName,
                    Id = 1,
                    Number = "PT300",
                    FirstName = "Mike",
                    LastName = "Petty",
                    HasScholarship = false,
                    TeacherId = 1234
                };

                var student3 = new Student
                {
                    School = mockSchoolName,
                    Id = 5,
                    Number = "DC1500",
                    FirstName = "Celine",
                    LastName = "Dion",
                    HasScholarship = false,
                    TeacherId = null
                };

                var student4 = new Student
                {
                    School = mockSchoolName,
                    Id = 10,
                    Number = "JA3000",
                    FirstName = "Angelina",
                    LastName = "Dion",
                    HasScholarship = false,
                    TeacherId = 550
                };

                classUnderTest.UpsertStudentAsync(student1).GetAwaiter().GetResult();
                classUnderTest.UpsertStudentAsync(student2).GetAwaiter().GetResult();
                classUnderTest.UpsertStudentAsync(student3).GetAwaiter().GetResult();
                classUnderTest.UpsertStudentAsync(student4).GetAwaiter().GetResult();

                // assert search results agains each search field
                var searchResultsBySchoolStudentId = await classUnderTest.SearchStudentsAsync(mockSchoolName, 12345);
                searchResultsBySchoolStudentId.Results.Should().HaveCount(1);
                searchResultsBySchoolStudentId.Results.First().ShouldBeEquivalentTo(student1);

                var searchResultsBySchoolStudentNumber = await classUnderTest.SearchStudentsAsync(mockSchoolName, null, "JA3000");
                searchResultsBySchoolStudentNumber.Results.Should().HaveCount(1);
                searchResultsBySchoolStudentNumber.Results.First().ShouldBeEquivalentTo(student4);

                var searchResultsBySchoolStudentFistName = await classUnderTest.SearchStudentsAsync(mockSchoolName, null, null, "Mike");
                searchResultsBySchoolStudentFistName.Results.Should().HaveCount(2);
                searchResultsBySchoolStudentFistName.Results.ShouldAllBeEquivalentTo(new[] { student1, student2 });

                var searchResultsBySchoolStudentLastName = await classUnderTest.SearchStudentsAsync(mockSchoolName, null, null, null, "Dion");
                searchResultsBySchoolStudentLastName.Results.Should().HaveCount(2);
                searchResultsBySchoolStudentLastName.Results.ShouldAllBeEquivalentTo(new[] { student3, student4 });

                var searchResultsBySchoolStudentTeacher = await classUnderTest.SearchStudentsAsync(mockSchoolName, null, null, null, null, 550);
                searchResultsBySchoolStudentTeacher.Results.Should().HaveCount(1);
                searchResultsBySchoolStudentTeacher.Results.First().ShouldBeEquivalentTo(student4);

                var searchResultsBySchoolStudentScholarship = await classUnderTest.SearchStudentsAsync(mockSchoolName, null, null, null, null, null, false);
                searchResultsBySchoolStudentScholarship.Results.Should().HaveCount(4);
                searchResultsBySchoolStudentScholarship.Results.ShouldAllBeEquivalentTo(new[] { student1, student2, student3, student4 });

                var searchResultsBySchoolNoData = await classUnderTest.SearchStudentsAsync(mockSchoolName, 999, "bar", "baz");
                searchResultsBySchoolNoData.Results.Should().BeEmpty();
            }
        }
    }
}
