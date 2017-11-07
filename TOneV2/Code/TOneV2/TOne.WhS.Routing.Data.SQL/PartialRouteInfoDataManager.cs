using System;
using System.Data;
using System.Text;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.Data.SQL
{
    public class PartialRouteInfoDataManager : RoutingDataManager, IPartialRouteInfoDataManager
    {
        public PartialRouteInfo GetPartialRouteInfo()
        {
            return GetItemText(query_GetPartialRouteInfo.ToString(), PartialRouteInfoMapper, (cmd) => { });
        }
        public void ApplyPartialRouteInfo(PartialRouteInfo partialRouteInfo)
        {
            query_ApplyPartialRouteInfo.Replace("#DATA#", string.Format("'{0}'", Vanrise.Common.Serializer.Serialize(partialRouteInfo)));
            ExecuteNonQueryText(query_ApplyPartialRouteInfo.ToString(), (cmd) => { });
        }

        PartialRouteInfo PartialRouteInfoMapper(IDataReader reader)
        {
            var data = reader["Data"] as string;
            if (!string.IsNullOrEmpty(data))
                return Vanrise.Common.Serializer.Deserialize<PartialRouteInfo>(data);

            return null;
        }


        private StringBuilder query_GetPartialRouteInfo = new StringBuilder("SELECT data FROM [dbo].[PartialRouteInfo]");

        private StringBuilder query_ApplyPartialRouteInfo = new StringBuilder(@"
                                                            IF NOT EXISTS(SELECT 1 FROM  [dbo].[PartialRouteInfo])
                                                                Begin
					                                                Insert into [dbo].[PartialRouteInfo] (Data) values(#DATA#)
				                                                END
			                                                ELSE
				                                                BEGIN
					                                                Update [dbo].[PartialRouteInfo] set  Data = #DATA#
				                                                END");
    }
}
