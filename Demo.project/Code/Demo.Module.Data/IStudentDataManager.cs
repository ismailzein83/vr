using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Data
{
    public interface IStudentDataManager : IDataManager
    {
        bool AreStudentsUpdated(ref object updateHandle);

        List<Student> GetStudents();

        bool Insert(Student student, out int insertedId);

        bool Update(Student student);

        bool Delete(int Id);
    }
}
