using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using FluentAssertions;
using System.IO;
using SchoolChallenge.Client.Functions;
using System.Linq;
using SchoolChallenge.Contracts;

namespace Client.Tests.Functions
{
    public class DataUploadFunctionsTests
    {
        [Fact]
        public void TestParseStudentDataFile_Given_ValidInput()
        {
            var schoolName = "MPS";

            using (var stream = new MemoryStream(Encoding.ASCII.GetBytes(TestData.TestData.Interview_Data_Students)))
            {
                var results = DataUploadFunctions.ParseStudentDataFile(stream, schoolName).ToList();

                results.Should().HaveCount(11);
                results.Should().Contain(x => x.Id == 1 && x.Number == "PT300" && x.FirstName == "Tom" && x.LastName == "Petty" && x.HasScholarship == false);
                results.Should().Contain(x => x.Id == 2 && x.Number == "JB600" && x.FirstName == "Billy " && x.LastName == "Joel" && x.HasScholarship == false);
                results.Should().Contain(x => x.Id == 3 && x.Number == "JM900" && x.FirstName == "Michael" && x.LastName == "Jorden" && x.HasScholarship == true);
                results.Should().Contain(x => x.Id == 4 && x.Number == "FH1200" && x.FirstName == "Harrison" && x.LastName == "Ford" && x.HasScholarship == false);
                results.Should().Contain(x => x.Id == 5 && x.Number == "DC1500" && x.FirstName == "Celine" && x.LastName == "Dion" && x.HasScholarship == false);
                results.Should().Contain(x => x.Id == 6 && x.Number == "HT1800" && x.FirstName == "Tom" && x.LastName == "Hanks" && x.HasScholarship == false);
                results.Should().Contain(x => x.Id == 7 && x.Number == "HH2100" && x.FirstName == "Helen" && x.LastName == "Hunt" && x.HasScholarship == false);
                results.Should().Contain(x => x.Id == 8 && x.Number == "RJ2400" && x.FirstName == "Julia" && x.LastName == "Roberts" && x.HasScholarship == true);
                results.Should().Contain(x => x.Id == 9 && x.Number == "PK2700" && x.FirstName == "Katy" && x.LastName == "Perry" && x.HasScholarship == false);
                results.Should().Contain(x => x.Id == 10 && x.Number == "JA3000" && x.FirstName == "Angelina" && x.LastName == "Jolie" && x.HasScholarship == false);
                results.Should().Contain(x => x.Id == 11 && x.Number == "JL3300" && x.FirstName == "LeBron" && x.LastName == "James" && x.HasScholarship == false);
                results.TrueForAll(x => x.School == schoolName).Should().BeTrue();
            }
        }

        [Fact]
        public void TestParseStudentDataFile_Given_InvalidInput()
        {
            using (var stream = new MemoryStream(Encoding.ASCII.GetBytes(TestData.TestData.junkFile)))
            {
                Action action = () => 
                    DataUploadFunctions.ParseStudentDataFile(stream, "MPS").ToList();

                action.ShouldThrow<Exception>();
            }
        }

        [Fact]
        public void TestParseTeacherDataFile_Given_ValidInput()
        {
            var schoolName = "MPS";

            using (var stream = new MemoryStream(Encoding.ASCII.GetBytes(TestData.TestData.Interview_Data_Teachers)))
            {
                var results = DataUploadFunctions.ParseTeacherDataFile(stream, schoolName).ToList();

                results.Should().HaveCount(4);                
                results.Should().Contain(x => x.Id == 1 && x.FirstName == "Elliot" && x.LastName == "Gould");
                results.Should().Contain(x => x.Id == 2 && x.FirstName == "Christina" && x.LastName == "Pickles");
                results.Should().Contain(x => x.Id == 3 && x.FirstName == "Larry" && x.LastName == "Hankin");
                results.Should().Contain(x => x.Id == 4 && x.FirstName == "Lisa" && x.LastName == "Kudrow");
                results.TrueForAll(x => x.School == schoolName).Should().BeTrue();
            }
        }

        [Fact]
        public void TestParseTeacherDataFile_Given_InvalidInput()
        {
            using (var stream = new MemoryStream(Encoding.ASCII.GetBytes(TestData.TestData.junkFile)))
            {
                Action action = () =>
                    DataUploadFunctions.ParseTeacherDataFile(stream, "MPS").ToList();

                action.ShouldThrow<Exception>();
            }
        }
    }
}
