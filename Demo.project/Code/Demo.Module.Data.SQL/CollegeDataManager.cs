using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Newtonsoft.Json;

namespace Demo.Module.Data.SQL
{
    public class CollegeDataManager : BaseSQLDataManager, ICollegeDataManager
    {
        public CollegeDataManager() :
            base(GetConnectionStringName("DemoProject_DBConnStringKey", "DemoProject_DBConnStringKey"))
        {
        }

        public List<College> GetColleges()
        {
            return GetItemsSP("[dbo].[sp_College_GetAll]", CollegeMapper);
        }
       
        public bool Insert(College college, out int insertedId)
        {
            string infoSerializedString = null;
            if (college.CollegeInfo != null)
                infoSerializedString = Vanrise.Common.Serializer.Serialize(college.CollegeInfo);

            string descriptionSerializedString = null;
            if (college.DescriptionString != null)
            {
                descriptionSerializedString = Vanrise.Common.Serializer.Serialize(college.DescriptionString);
            }

            object id;
            int nbOfRecordsAffected = ExecuteNonQuerySP("[dbo].[sp_College_Insert]", out id, college.Name, college.UniversityId, infoSerializedString, descriptionSerializedString);
            insertedId = Convert.ToInt32(id);
            return (nbOfRecordsAffected > 0);
        }

        public bool Update(College college)
        {
            string infoSerializedString = null;
            if (college.CollegeInfo != null)
                infoSerializedString = Vanrise.Common.Serializer.Serialize(college.CollegeInfo);

            string descriptionSerializedString = null;
            if (college.DescriptionString != null)
                descriptionSerializedString = Vanrise.Common.Serializer.Serialize(college.DescriptionString);

            int nbOfRecordsAffected = ExecuteNonQuerySP("[dbo].[sp_College_Update]", college.CollegeId, college.Name, college.UniversityId, infoSerializedString, descriptionSerializedString);
            return (nbOfRecordsAffected > 0);
        }

        public bool Delete(int collegeId)
        {
            int nbOfRecordsAffected = ExecuteNonQuerySP("[dbo].[sp_College_Delete]", collegeId);
            return (nbOfRecordsAffected > 0);
        }

        public bool AreCollegesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[dbo].[College]", ref updateHandle);
        }

        College CollegeMapper(IDataReader reader)
        {
            return new College
            {
                CollegeId = GetReaderValue<int>(reader, "ID"),
                Name = GetReaderValue<string>(reader, "Name"),
                UniversityId = GetReaderValue<int>(reader, "UniversityId"),
                CollegeInfo = Vanrise.Common.Serializer.Deserialize<CollegeInfoType>(GetReaderValue<string>(reader, "Info Type")),
                DescriptionString = Vanrise.Common.Serializer.Deserialize<List<Description>>(GetReaderValue<string>(reader, "Description"))
            };
        }
    }
}
