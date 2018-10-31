using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Data
{
    public interface ISchoolDataManager : IDataManager
    {
        bool AreSchoolsUpdated(ref object updateHandle);

        List<School> GetSchools();

        bool Insert(School school, out int insertedId);

        bool Update(School school);

        bool Delete(int Id);
    }
}
