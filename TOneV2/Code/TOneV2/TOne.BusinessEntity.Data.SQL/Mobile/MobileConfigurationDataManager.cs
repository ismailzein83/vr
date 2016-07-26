using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Entities;
using TOne.Data.SQL;

namespace TOne.BusinessEntity.Data.SQL
{
    public class MobileConfigurationDataManager : BaseTOneDataManager, IMobileConfigurationDataManager
    {
        public bool AddMobileConfiguration(MobileConfiguration configuration, out int insertedId)
        {
            object configId;
            int recordsAffected = ExecuteNonQuerySP("BEntity.sp_MobileConfiguration_Insert", out configId, configuration.Key, configuration.Value, configuration.Description);
            insertedId = (recordsAffected > 0) ? (Int32)configId : -1;
            return (recordsAffected > 0);
        }

        public bool UpdateMobileConfiguration(MobileConfiguration configuration)
        {
            int recordsAffected = ExecuteNonQuerySP("BEntity.sp_MobileConfiguration_Update", configuration.ID, configuration.Key, configuration.Value, configuration.Description);
            return (recordsAffected > 0);
        }

        public IEnumerable<MobileConfiguration> GetConfigurations(int? configId)
        {
            return GetItemsSP("BEntity.sp_MobileConfiguration_Get", MobileConfigurationMapper, configId);
        }

        MobileConfiguration MobileConfigurationMapper(IDataReader reader)
        {
            return new MobileConfiguration
            {
                ID = (int)reader["ID"],
                Key = reader["Key"] as string,
                Value = reader["Value"] as string,
                Description = reader["Description"] as string
            };
        }
    }
}
