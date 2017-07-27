using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demo.Module.Entities;

namespace Demo.Module.Data
{
    public interface IStudentDataManager : IDataManager
    {
        List<Student> GetStudents();
        bool Insert(Student student, out int insertedId);
        bool Update(Student student);
        bool Delete(int Id);
        bool AreStudentsUpdated(ref object updateHandle);

    }
}
