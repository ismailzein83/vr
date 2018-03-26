using Demo.Module.Entities;
using Demo.Module.Entities.Building;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Data
{
    public interface ICollegeDataManager : IDataManager
    {
        bool AreCollegesUpdated(ref object updateHandle);

        List<College> GetColleges();

        bool Insert(College college, out int insertedId);

        bool Update(College college);

        bool Delete(int Id);
    }
}
