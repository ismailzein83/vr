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
    public class SchoolDataManager : BaseSQLDataManager, ISchoolDataManager
    {
        public SchoolDataManager() :
            base(GetConnectionStringName("DemoProject_DBConnStringKey", "DemoProject_DBConnStringKey"))
        {
        }

        public List<School> GetSchools()
        {
            return GetItemsSP("[dbo].[sp_School_GetAll]", SchoolMapper);
        }

        public bool Insert(School school, out int insertedId)
        {
            object id;
            int nbOfRecordsAffected = ExecuteNonQuerySP("[dbo].[sp_School_Insert]", out id, school.Name);
            insertedId = Convert.ToInt32(id);

            return (nbOfRecordsAffected > 0);
        }

        public bool Update(School school)
        {
            int nbOfRecordsAffected = ExecuteNonQuerySP("[dbo].[sp_School_Update]", school.SchoolId, school.Name);
            return (nbOfRecordsAffected > 0);
        }

        public bool Delete(int schoolId)
        {
            int nbOfRecordsAffected = ExecuteNonQuerySP("[dbo].[sp_School_Delete]", schoolId);
            return (nbOfRecordsAffected > 0);
        }

        public bool AreSchoolsUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[dbo].[School]", ref updateHandle);
        }

        School SchoolMapper(IDataReader reader)
        {
            return new School
            {
                SchoolId = GetReaderValue<int>(reader, "ID"),
                Name = GetReaderValue<string>(reader, "Name"),
                DemoCityId = GetReaderValue<int>(reader, "CityId"),

            };
        }
    }
}
