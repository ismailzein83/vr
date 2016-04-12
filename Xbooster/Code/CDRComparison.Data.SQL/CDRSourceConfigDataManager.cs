using CDRComparison.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;

namespace CDRComparison.Data.SQL
{
    public class CDRSourceConfigDataManager : BaseSQLDataManager, ICDRSourceConfigDataManager
    {
        #region Constructors / Fields

        public CDRSourceConfigDataManager()
            : base(GetConnectionStringName("CDRComparisonDBConnStringKey", "CDRComparisonDBConnString"))
        {

        }

        #endregion

        #region Public Methods

        public IEnumerable<CDRSourceConfig> GetCDRSourceConfigs()
        {
            return GetItemsSP("dbo.sp_CDRSourceConfig_GetAll", CDRSourceConfigMapper);
        }

        public bool AreCDRSourceConfigsUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("dbo.CDRSourceConfig", ref updateHandle);
        }

        public bool InsertCDRSourceConfig(CDRSourceConfig cdrSourceConfig, out int insertedObjectId)
        {
            object cdrSourceConfigId;

            int affectedRows = ExecuteNonQuerySP("dbo.sp_CDRSourceConfig_Insert", out cdrSourceConfigId, cdrSourceConfig.Name, Vanrise.Common.Serializer.Serialize(cdrSourceConfig.CDRSource), Vanrise.Common.Serializer.Serialize(cdrSourceConfig.SettingsTaskExecutionInfo), cdrSourceConfig.IsPartnerCDRSource, cdrSourceConfig.UserId);
            insertedObjectId = (affectedRows > 0) ? (int)cdrSourceConfigId : -1;

            return (affectedRows > 0);
        }

        public bool UpdateCDRSourceConfig(CDRSourceConfig cdrSourceConfig)
        {
            int affectedRows = ExecuteNonQuerySP("dbo.sp_CDRSourceConfig_Update", cdrSourceConfig.CDRSourceConfigId, cdrSourceConfig.Name, Vanrise.Common.Serializer.Serialize(cdrSourceConfig.CDRSource), Vanrise.Common.Serializer.Serialize(cdrSourceConfig.SettingsTaskExecutionInfo), cdrSourceConfig.IsPartnerCDRSource, cdrSourceConfig.UserId);
            return (affectedRows > 0);
        }

        #endregion

        #region Mappers

        CDRSourceConfig CDRSourceConfigMapper(IDataReader reader)
        {
            return new CDRSourceConfig()
            {
                CDRSourceConfigId = (int)reader["CDRSourceConfigId"],
                Name = reader["Name"] as string,
                CDRSource = Vanrise.Common.Serializer.Deserialize(reader["CDRSource"] as string) as CDRSource,
                SettingsTaskExecutionInfo = Vanrise.Common.Serializer.Deserialize(reader["SettingsTaskExecutionInfo"] as string) as SettingsTaskExecutionInfo,
                IsPartnerCDRSource = (bool)reader["IsPartnerCDRSource"],
                UserId = (int)reader["UserID"]
            };
        }

        #endregion
    }
}
