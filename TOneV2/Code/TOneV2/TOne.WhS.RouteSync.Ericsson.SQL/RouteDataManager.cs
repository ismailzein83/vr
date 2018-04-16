using System;
using Vanrise.Data.SQL;
using TOne.WhS.RouteSync.Entities;
using TOne.WhS.RouteSync.Ericsson.Data;
using TOne.WhS.RouteSync.Ericsson.Entities;
using System.Data;
using System.Collections.Generic;
using Vanrise.Common;

namespace TOne.WhS.RouteSync.Ericsson.SQL
{
    public class RouteDataManager : BaseSQLDataManager, IRouteDataManager
    {
        const string RouteTableName = "Route";
        const string RouteTempTableName = "Route_temp";
        public string SwitchId { get; set; }
        public RouteDataManager()
            : base(GetConnectionStringName("TOneWhS_RouteSync_DBConnStringKey", "RouteSyncDBConnString"))
        {

        }

        public void Initialize(IRouteInitializeContext context)
        {
            Guid guid = Guid.NewGuid();
            string query = string.Format(query_CreateRouteTempTable, SwitchId, guid, RouteTempTableName);
            ExecuteNonQueryText(query, null);
        }

        public void Finalize(IRouteFinalizeContext context)
        {
            throw new NotImplementedException();
        }

        public void CompareTables(IRouteCompareTablesContext context)
        {
            var differences = new Dictionary<EricssonConvertedRouteIdentifier, List<EricssonConvertedRouteByCompare>>();

            string query = string.Format(query_CompareRouteTables, SwitchId, RouteTableName, RouteTempTableName);
            ExecuteReaderText(query, (reader) =>
            {
                while (reader.Read())
                {
                    var convertedRouteByCompare = new EricssonConvertedRouteByCompare() { EricssonConvertedRoute = EricssonConvertedRouteMapper(reader), TableName = reader["tableName"] as string };
                    var routeIdentifier = new EricssonConvertedRouteIdentifier() { BO = convertedRouteByCompare.EricssonConvertedRoute.BO, Code = convertedRouteByCompare.EricssonConvertedRoute.Code };
                    List<EricssonConvertedRouteByCompare> tempRouteDifferences = differences.GetOrCreateItem(routeIdentifier);
                    tempRouteDifferences.Add(convertedRouteByCompare);
                }
            }, null);

            if (differences.Count > 0)
            {
                List<EricssonConvertedRoute> routesToAdd = new List<EricssonConvertedRoute>();
                List<EricssonConvertedRoute> routesToUpdate = new List<EricssonConvertedRoute>();
                List<EricssonConvertedRoute> routesToDelete = new List<EricssonConvertedRoute>();
                foreach (var differenceKvp in differences)
                {
                    var routeDifferences = differenceKvp.Value;
                    if (routeDifferences.Count == 1)
                    {
                        var singleRouteDifference = differenceKvp.Value[0];
                        if (singleRouteDifference.TableName == RouteTableName)
                            routesToDelete.Add(singleRouteDifference.EricssonConvertedRoute);
                        else
                            routesToAdd.Add(singleRouteDifference.EricssonConvertedRoute);
                    }
                    else //routeDifferences.Count = 2
                    {
                        foreach (var routeDifference in routeDifferences)
                        {
                            if (routeDifference.TableName == RouteTempTableName)
                            {
                                routesToUpdate.Add(routeDifference.EricssonConvertedRoute);
                                continue;
                            }
                        }
                    }
                }

                if (routesToAdd.Count > 0)
                    context.RoutesToAdd = routesToAdd;

                if (routesToUpdate.Count > 0)
                    context.RoutesToUpdate = routesToUpdate;

                if (routesToDelete.Count > 0)
                    context.RoutesToDelete = routesToDelete;
            }
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            throw new NotImplementedException();
        }

        public object InitialiazeStreamForDBApply()
        {
            throw new NotImplementedException();
        }

        public void WriteRecordToStream(ConvertedRoute record, object dbApplyStream)
        {
            throw new NotImplementedException();
        }


        private class EricssonConvertedRouteByCompare
        {
            public EricssonConvertedRoute EricssonConvertedRoute { get; set; }
            public string TableName { get; set; }
        }


        EricssonConvertedRoute EricssonConvertedRouteMapper(IDataReader reader)
        {
            return new EricssonConvertedRoute()
            {
                BO = reader["BO"] as string,
                Code = reader["Code"] as string,
                RCNumber = (int)reader["RCNumber"]
            };
        }

        const string query_CreateRouteTempTable = @"IF EXISTS( SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'WhS_RouteSync_Ericsson_{0}.{2}') AND s.type in (N'U'))
                                                    BEGIN
                                                        DROP TABLE WhS_RouteSync_Ericsson_{0}.{2}
                                                    END
                                                    
                                                    CREATE TABLE [WhS_RouteSync_Ericsson_{0}].[{2}](
                                                          BO varchar(255) NOT NULL,
	                                                      Code varchar(20) NOT NULL,
	                                                      RCNumber int NOT NULL
                                                    CONSTRAINT [PK_WhS_RouteSync_Ericsson_{0}.Route_{1}] PRIMARY KEY CLUSTERED 
                                                    (
                                                        BO ASC,
	                                                    Code ASC
                                                    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                                                    ) ON [PRIMARY]";

        const string query_CompareRouteTables = @"IF EXISTS( SELECT * FROM sys.objects s WHERE s.OBJECT_ID = OBJECT_ID(N'WhS_RouteSync_Ericsson_{0}.{1}') AND s.type in (N'U'))
                                                  BEGIN
	                                                  SELECT [BO],[Code],[RCNumber], max(tableName) as tableName FROM (
		                                                  SELECT [BO],[Code],[RCNumber], '{1}' as tableName FROM [WhS_RouteSync_Ericsson_{0}].[{1}]
		                                                  UNION ALL
		                                                  SELECT [BO],[Code],[RCNumber], '{2}' as tableName FROM [WhS_RouteSync_Ericsson_{0}].[{2}]
	                                                  ) v
	                                                  GROUP BY [BO],[Code],[RCNumber]
	                                                  HAVING COUNT(1)=1
                                                  END
                                                  ELSE
                                                  BEGIN
	                                                  SELECT [BO],[Code],[RCNumber], '{2}' as tableName FROM [WhS_RouteSync_Ericsson_{0}].[{2}]
                                                  END";
    }
}