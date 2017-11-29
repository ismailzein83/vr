using System;
using System.Data;
using System.Text;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.Data.SQL
{
    public class RoutingEntityDetailsDataManager : RoutingDataManager, IRoutingEntityDetailsDataManager
    {
        public RoutingEntityDetails GetRoutingEntityDetails(RoutingEntityType routingEntityType)
        {
            string query = string.Format(query_GetRoutingEntityDetails, (int)routingEntityType);
            return GetItemText(query, PartialRouteInfoMapper, null);
        }

        public void ApplyRoutingEntityDetails(RoutingEntityDetails routingEntityDetails)
        {
            string query = string.Format(query_ApplyPartialRouteInfo, (int)routingEntityDetails.RoutingEntityType, Vanrise.Common.Serializer.Serialize(routingEntityDetails.RoutingEntityInfo));
            ExecuteNonQueryText(query, null);
        }

        RoutingEntityDetails PartialRouteInfoMapper(IDataReader reader)
        {
            var info = reader["Info"] as string;

            return new RoutingEntityDetails()
            {
                RoutingEntityType = (RoutingEntityType)reader["Type"],
                RoutingEntityInfo = !string.IsNullOrEmpty(info) ? Vanrise.Common.Serializer.Deserialize<RoutingEntityInfo>(info) : null
            };
        }


        private string query_GetRoutingEntityDetails = "SELECT [Type], [Info] FROM [dbo].[RoutingEntityDetails] WHERE [TYPE] = {0}";

        private string query_ApplyPartialRouteInfo = @"IF NOT EXISTS(SELECT 1 FROM  [dbo].[RoutingEntityDetails] WHERE [TYPE] = {0})
                                                      Begin
                                                          Insert into [dbo].[RoutingEntityDetails] ([TYPE], [Info]) values({0}, '{1}')
                                                      END
			                                          ELSE
                                                      BEGIN
                                                          Update [dbo].[RoutingEntityDetails] set  [Info] = '{1}' WHERE [TYPE] = {0}
                                                      END";
    }
}