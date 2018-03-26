using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Data
{
    public interface IUniversityDataManager : IDataManager
    {
        bool AreUniversitiesUpdated(ref object updateHandle);

        List<University> GetUniversities();

        bool Insert(University university, out int insertedId);

        bool Update(University university);

        bool Delete(int Id);
    }
}
