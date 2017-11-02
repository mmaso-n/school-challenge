using Contracts;
using Microsoft.WindowsAzure.Storage.Table;
using SchoolChallenge.Repository.Entities;
using System;

namespace SchoolChallenge.Repository
{
    public static class UtilitiesExtensions
    {
        public static TableContinuationToken ToRepositoryImplementation(this RepositoryContinationToken repositoryContination)
        {
            if (repositoryContination?.Value == null ||
                repositoryContination.Value.GetType() != typeof(TableContinuationToken))
            {
                return default(TableContinuationToken);
            }
            else
            {
                return (TableContinuationToken)repositoryContination.Value;
            }
        }

        public static StudentEntity ToRepositoryEntity(this Student student)
        {
            return new StudentEntity
            {
                PartitionKey = student.School, 
                RowKey = student.Id.ToString(),
                Number = student.Number,
                FirstName = student.FirstName,
                LastName = student.LastName,
                TeacherId = student.TeacherId,
                HasScholarship = student.HasScholarship
            };
        }

        public static Student ToStudent(this StudentEntity student)
        {
            return new Student
            {
                School = student.PartitionKey,
                Id = Convert.ToInt32(student.RowKey),
                Number = student.Number,
                FirstName = student.FirstName,
                LastName = student.LastName,
                TeacherId = student.TeacherId,
                HasScholarship = student.HasScholarship
            };
        }

        public static TeacherEntity ToRepositoryEntity(this Teacher teacher)
        {
            return new TeacherEntity
            {
                PartitionKey = teacher.School, 
                RowKey = teacher.Id.ToString(), 
                FirstName = teacher.FirstName, 
                LastName = teacher.LastName
            };
        }

        public static Teacher ToTeacher(this TeacherEntity teacher)
        {
            return new Teacher
            {
                School = teacher.PartitionKey,
                Id = Convert.ToInt32(teacher.RowKey),
                FirstName = teacher.FirstName,
                LastName = teacher.LastName
            };
        }
    }
}
