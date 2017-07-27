using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using System.Data;
using Demo.Module.Entities;
using Newtonsoft.Json;

namespace Demo.Module.Data.SQL
{
    public class StudentDataManager : BaseSQLDataManager, IStudentDataManager
    {
        public StudentDataManager() :
            base(GetConnectionStringName("DemoProject_DBConnStringKey", "DemoProject_DBConnStringKey"))
        {
        }

        JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
        public List<Student> GetStudents()
        {
            return GetItemsSP("[dbo].[sp_Student_GetAll]", StudentMapper);
        }
        public bool Insert(Student student, out int insertedId)
        {
            object id;
            string serializedPayment = JsonConvert.SerializeObject(student.Payment, settings);
            int nbOfRecordsAffected = ExecuteNonQuerySP("[dbo].[sp_Student_Insert]", out id, student.Name, serializedPayment,student.RoomId,student.BuildingId);
            insertedId = Convert.ToInt32(id);
            return (nbOfRecordsAffected > 0);
        }
        public bool Update(Student student)
        {
            string serializedPayment = JsonConvert.SerializeObject(student.Payment, settings);
            int nbOfRecordsAffected = ExecuteNonQuerySP("[dbo].[sp_Student_Update]", student.StudentId, student.Name, serializedPayment, student.RoomId, student.BuildingId);
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
            Student student= new Student();
            student.StudentId=GetReaderValue<int>(reader, "ID");
           student.Name=GetReaderValue<string>(reader, "Name");
           student.Payment = (reader["PaymentType"] != null ? JsonConvert.DeserializeObject<PaymentMethod>(reader["PaymentType"] as string, settings) : null);
           student.RoomId = GetReaderValue<int>(reader, "RoomId");
           student.BuildingId = GetReaderValue<int>(reader,"BuildingId");
          return student;
        }
    }
}