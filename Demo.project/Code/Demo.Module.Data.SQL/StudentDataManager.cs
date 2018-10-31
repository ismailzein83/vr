using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;

namespace Demo.Module.Data.SQL
{
    public class StudentDataManager : BaseSQLDataManager, IStudentDataManager
    {
        public StudentDataManager() :
            base(GetConnectionStringName("DemoProject_DBConnStringKey", "DemoProject_DBConnStringKey"))
        {
        }

        public List<Student> GetStudents()
        {
            return GetItemsSP("[dbo].[sp_Student_GetAll]", StudentMapper);
        }

        public bool Insert(Student student, out int insertedId)
        {
            object id;

            string serializedStudentSettings = null;

            if (student.Settings != null)
                serializedStudentSettings = Vanrise.Common.Serializer.Serialize(student.Settings);


            int nbOfRecordsAffected = ExecuteNonQuerySP("[dbo].[sp_Student_Insert]", out id, student.Name, student.Age, student.SchoolId, serializedStudentSettings,student.DemoCountryId,student.DemoCityId);
            insertedId = Convert.ToInt32(id);

            return (nbOfRecordsAffected > 0);
        }

        public bool Update(Student student)
        {
            string serializedStudentSettings = null;
            if (student.Settings != null)
                serializedStudentSettings = Vanrise.Common.Serializer.Serialize(student.Settings);

            int nbOfRecordsAffected = ExecuteNonQuerySP("[dbo].[sp_Student_Update]", student.StudentId, student.Name, student.Age, student.SchoolId, serializedStudentSettings, student.DemoCountryId, student.DemoCityId);
            return (nbOfRecordsAffected > 0);
        }

        public bool Delete(int studentId)
        {
            int nbOfRecordsAffected = ExecuteNonQuerySP("[dbo].[sp_Student_Delete]", studentId);
            return (nbOfRecordsAffected > 0);
        }

        public bool AreStudentsUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[dbo].[Student]", ref updateHandle);
        }

        Student StudentMapper(IDataReader reader)
        {
            return new Student
            {
                StudentId = GetReaderValue<int>(reader, "ID"),
                Name = GetReaderValue<string>(reader, "Name"),
                SchoolId = GetReaderValue<int>(reader, "SchoolId"),
                Age=GetReaderValue<int>(reader, "Age"),
                Settings = Vanrise.Common.Serializer.Deserialize<StudentSettings>(reader["Settings"] as string),
                DemoCountryId = GetReaderValue<int>(reader, "CountryId"),
                DemoCityId = GetReaderValue<int>(reader, "CityId")
            };
        }
    }
}
