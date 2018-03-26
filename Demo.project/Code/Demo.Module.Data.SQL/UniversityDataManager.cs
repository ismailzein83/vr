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
    public class UniversityDataManager : BaseSQLDataManager, IUniversityDataManager
    {
        public UniversityDataManager() :
            base(GetConnectionStringName("DemoProject_DBConnStringKey", "DemoProject_DBConnStringKey"))
        {
        }

        public List<University> GetUniversities()
        {
            return GetItemsSP("[dbo].[sp_University_GetAll]", UniversityMapper);
        }

        public bool Insert(University university, out int insertedId)
        {
            object id;
            int nbOfRecordsAffected = ExecuteNonQuerySP("[dbo].[sp_University_Insert]", out id, university.Name);
            insertedId = Convert.ToInt32(id);
            return (nbOfRecordsAffected > 0);
        }
        
        public bool Update(University university)
        {
            int nbOfRecordsAffected = ExecuteNonQuerySP("[dbo].[sp_University_Update]", university.UniversityId, university.Name);
            return (nbOfRecordsAffected > 0);
        }
        
        public bool Delete(int universityId)
        {
            int nbOfRecordsAffected = ExecuteNonQuerySP("[dbo].[sp_University_Delete]", universityId);
            return (nbOfRecordsAffected > 0);
        }
        
        public bool AreUniversitiesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[dbo].[University]", ref updateHandle);
        }

        University UniversityMapper(IDataReader reader)
        {
            return new University
            {
                UniversityId = GetReaderValue<int>(reader, "ID"),
                Name = GetReaderValue<string>(reader, "Name")
            };
        }
    }
}
