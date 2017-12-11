using System.Collections.Generic;
using System.Data;
using System.Text;
using TOne.WhS.Routing.Entities;
using System.Linq;

namespace TOne.WhS.Routing.Data.SQL
{
    public class SwitchSyncDataDataManager : RoutingDataManager, ISwitchSyncDataDataManager
    {
        #region Public Methods

        public List<SwitchSyncData> GetSwitchSyncDataByIds(List<string> switchIds)
        {
            if (switchIds == null || switchIds.Count == 0)
                return null;

            StringBuilder queryBuilder = new StringBuilder(query_GetSwitchSyncData);
            queryBuilder.Replace("#FILTER#", string.Format("Where SwitchId in ('{0}')", string.Join("','", switchIds)));

            return GetItemsText(queryBuilder.ToString(), SwitchSyncDataMapper, (cmd) => { });
        }

        public void ApplySwitchesSyncData(List<string> switchIds, int versionNumber)
        {
            if (switchIds == null || switchIds.Count == 0)
                return;

            StringBuilder queryBuilder = new StringBuilder();

            string applySwitchSyncDataQuery = query_ApplySwitchSyncData.Replace("#LastVersionNumber#", versionNumber.ToString());

            foreach (var switchId in switchIds)
                queryBuilder.Append(applySwitchSyncDataQuery.Replace("#SWITCHID#", switchId));

            ExecuteNonQueryText(queryBuilder.ToString(), (cmd) => { });
        }

        #endregion

        #region Private Methods

        private SwitchSyncData SwitchSyncDataMapper(IDataReader reader)
        {
            return new SwitchSyncData()
            {
                SwitchId = reader["SwitchId"] as string,
                LastVersionNumber = (int)reader["LastVersionNumber"]
            };
        }

        #endregion

        #region Queries

        private const string query_GetSwitchSyncData = @"Select SwitchId, LastVersionNumber 
                                                         From [dbo].[SwitchSyncData]
                                                         #FILTER#";

        private const string query_ApplySwitchSyncData = @"IF NOT EXISTS(SELECT SwitchId FROM [dbo].[SwitchSyncData] Where SwitchId = '#SWITCHID#')
                                                              Begin
        		                                                  Insert into [dbo].[SwitchSyncData] (SwitchId, LastVersionNumber) Values('#SWITCHID#', #LastVersionNumber#)
        	                                                  END
                                                           ELSE
        	                                                  BEGIN
        		                                                  Update [dbo].[SwitchSyncData] set LastVersionNumber = #LastVersionNumber# Where SwitchId = '#SWITCHID#'
        	                                                  END ";

        #endregion
    }
}