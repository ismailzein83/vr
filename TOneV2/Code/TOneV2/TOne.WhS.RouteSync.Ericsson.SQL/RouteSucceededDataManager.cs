﻿using System;
using System.Collections.Generic;
using TOne.WhS.RouteSync.Ericsson.Data;
using TOne.WhS.RouteSync.Ericsson.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.RouteSync.Ericsson.SQL
{
    class RouteSucceededDataManager : BaseSQLDataManager, IRouteSucceededDataManager
    {
        const string RouteAddedTableName = "Route_Added";
        const string RouteUpdatedTableName = "Route_Updated";
        const string RouteDeletedTableName = "Route_Deleted";

        readonly string[] columns = { "OBA", "Code", "RCNumber", "TRD", "NextBTable", "OriginCode", "RouteType" };
        public string SwitchId { get; set; }
        public string RouteSucceededTableName { get; set; }
        private string BCPTableName;
        public RouteSucceededDataManager()
            : base(GetConnectionStringName("TOneWhS_RouteSync_DBConnStringKey", "RouteSyncDBConnString"))
        { }


        public object FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                TableName = string.Format("[WhS_RouteSync_Ericsson_{0}].[{1}]", SwitchId, BCPTableName),
                Stream = streamForBulkInsert,
                TabLock = true,
                KeepIdentity = false,
                FieldSeparator = '^',
                ColumnNames = columns
            };
        }

        public object InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        public void WriteRecordToStream(EricssonConvertedRoute record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}", record.OBA, record.Code, record.RCNumber, record.TRD, record.NextBTable, record.OriginCode, (int)record.RouteType);
        }

        public void ApplyRoutesSucceededForDB(object preparedRoute)
        {
            InsertBulkToTable(preparedRoute as BaseBulkInsertInfo);
        }

        public void SaveRoutesSucceededToDB(Dictionary<int, List<EricssonRouteWithCommands>> routesWithCommandsByOBA)
        {
            if (routesWithCommandsByOBA == null || routesWithCommandsByOBA.Count == 0)
                return;

            Object dbApplyAddStream = InitialiazeStreamForDBApply();
            Object dbApplyUpdateStream = InitialiazeStreamForDBApply();
            Object dbApplyDeleteStream = InitialiazeStreamForDBApply();

            foreach (var routeKVP in routesWithCommandsByOBA)
            {
                foreach (var routeWithCommands in routeKVP.Value)
                {
                    switch (routeWithCommands.ActionType)
                    {
                        case RouteActionType.Add:
                            WriteRecordToStream(routeWithCommands.RouteCompareResult.Route, dbApplyAddStream);
                            break;
                        case RouteActionType.Update:
                            WriteRecordToStream(routeWithCommands.RouteCompareResult.Route, dbApplyUpdateStream);
                            break;
                        case RouteActionType.Delete:
                            WriteRecordToStream(routeWithCommands.RouteCompareResult.Route, dbApplyDeleteStream);
                            break;
                        default: break;
                    }
                }
            }

            BCPTableName = RouteAddedTableName;
            Object preparedAddRoutes = FinishDBApplyStream(dbApplyAddStream);
            ApplyRoutesSucceededForDB(preparedAddRoutes);

            BCPTableName = RouteUpdatedTableName;
            Object preparedUpdatedRoutes = FinishDBApplyStream(dbApplyUpdateStream);
            ApplyRoutesSucceededForDB(preparedUpdatedRoutes);

            BCPTableName = RouteDeletedTableName;
            Object preparedDeletedRoutes = FinishDBApplyStream(dbApplyDeleteStream);
            ApplyRoutesSucceededForDB(preparedDeletedRoutes);
        }
    }
}