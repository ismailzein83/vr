using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;
using Vanrise.Data.SQL;

namespace Vanrise.Analytic.Data.SQL
{
    public class AnalyticItemConfigDataManager : BaseSQLDataManager, IAnalyticItemConfigDataManager
    {
        public AnalyticItemConfigDataManager()
            : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnString"))
        {

        }
       
        #region Public Methods
        public bool AreAnalyticItemConfigUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[Analytic].[AnalyticItemConfig]", ref updateHandle);
        }
        public List<AnalyticItemConfig<T>> GetItemConfigs<T>(int tableId, AnalyticItemType itemType) where T : class
        {
            return GetItemsSP("[Analytic].[sp_AnalyticItemConfig_GetByTableIdAndItemType]", AnalyticItemConfigReader<T>, tableId, (int)itemType);
        }


        public bool AddAnalyticItemConfig<T>(AnalyticItemConfig<T> analyticItemConfig) where T:class
        {
            int recordesEffected = ExecuteNonQuerySP("Analytic.sp_AnalyticItemConfig_Insert", analyticItemConfig.AnalyticItemConfigId, analyticItemConfig.Name, analyticItemConfig.Title, analyticItemConfig.TableId, analyticItemConfig.ItemType, Vanrise.Common.Serializer.Serialize(analyticItemConfig.Config));

            return (recordesEffected > 0);
        }

        public bool UpdateAnalyticItemConfig<T>(AnalyticItemConfig<T> analyticItemConfig) where T : class
        {
            int recordesEffected = ExecuteNonQuerySP("Analytic.sp_AnalyticItemConfig_Update",analyticItemConfig.AnalyticItemConfigId, analyticItemConfig.Name, analyticItemConfig.Title, analyticItemConfig.TableId, analyticItemConfig.ItemType, Vanrise.Common.Serializer.Serialize(analyticItemConfig.Config));
            return (recordesEffected > 0);
        }
        #endregion

        #region Private Methods
      
        #endregion

        #region Mappers
        private AnalyticItemConfig<T> AnalyticItemConfigReader<T>(IDataReader reader) where T : class
        {
            return new AnalyticItemConfig<T>
            {
                AnalyticItemConfigId = GetReaderValue<Guid>(reader,"ID"),
                Config = Vanrise.Common.Serializer.Deserialize<T>(reader["Config"] as string),
                ItemType = (AnalyticItemType) reader["ItemType"],
                Name = reader["Name"] as string,
                TableId = GetReaderValue<int>(reader, "TableId"),
                Title = reader["Title"] as string,
            };
        }
        #endregion



       
    }
}
