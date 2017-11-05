using SchoolChallenge.Contracts;
using System;
using System.Collections.Generic;
using System.IO;

namespace SchoolChallenge.Client.Functions
{
    public static class DataUploadFunctions
    {
        public static IEnumerable<Student> ParseStudentDataFile(Stream fileStream, string school)
        {
            var results = new List<Student>();
            var rowCounter = 0;
            var expectedFileHeader = "Student ID,Student Number,First Name,Last Name,Has Scholarship";

            using (var reader = new StreamReader(fileStream))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();

                    if (rowCounter == 0)
                    {
                        if (line != expectedFileHeader)
                            throw new Exception($"The file must have a header row like this: {expectedFileHeader}"); // validate that the header row exists
                    }
                    else
                    {
                        var lineContents = line.Split(',');

                        var student = new Student
                        {
                            School = school,
                            Id = Convert.ToInt32(lineContents[0]),
                            Number = lineContents[1],
                            FirstName = lineContents[2],
                            LastName = lineContents[3],
                            HasScholarship = string.Equals(lineContents[4], "Yes", StringComparison.InvariantCultureIgnoreCase) ? true : false
                        };
                        results.Add(student);
                    }

                    rowCounter++;
                }
            }

            return results;
        }

        public static IEnumerable<Teacher> ParseTeacherDataFile(Stream fileStream, string school)
        {
            var results = new List<Teacher>();
            var rowCounter = 0;
            var expectedFileHeader = "Teacher ID,First Name,Last Name";

            using (var reader = new StreamReader(fileStream))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();

                    if (rowCounter == 0)
                    {
                        if (line != expectedFileHeader)
                            throw new Exception($"The file must have a header row like this: {expectedFileHeader}"); // validate that the header row exists
                    }
                    else
                    {
                        var lineContents = line.Split(',');

                        var student = new Teacher
                        {
                            School = school,
                            Id = Convert.ToInt32(lineContents[0]),
                            FirstName = lineContents[1],
                            LastName = lineContents[2],
                        };
                        results.Add(student);
                    }

                    rowCounter++;
                }
            }

            return results;
        }
    }
}
