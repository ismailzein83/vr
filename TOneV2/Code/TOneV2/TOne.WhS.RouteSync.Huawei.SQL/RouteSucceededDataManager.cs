using System;
using System.Collections.Generic;
using TOne.WhS.RouteSync.Huawei.Data;
using TOne.WhS.RouteSync.Huawei.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.RouteSync.Huawei.SQL
{
    public class RouteSucceededDataManager : BaseSQLDataManager, IRouteSucceededDataManager
    {
        #region Properties/Ctor

        const string RouteAddedTableName = "Route_Added";
        const string RouteUpdatedTableName = "Route_Updated";
        const string RouteDeletedTableName = "Route_Deleted";

        readonly string[] columns = { "RSSN", "Code", "RSName", "DNSet" };

        private string BCPTableName;

        public string SwitchId { get; set; }

        public RouteSucceededDataManager()
            : base(GetConnectionStringName("TOneWhS_RouteSync_DBConnStringKey", "RouteSyncDBConnString"))
        {

        }

        #endregion

        #region Public Methods

        public void SaveRoutesSucceededToDB(Dictionary<string, List<HuaweiRouteWithCommands>> routesWithCommandsByRSSN)
        {
            if (routesWithCommandsByRSSN == null || routesWithCommandsByRSSN.Count == 0)
                return;

            Object dbApplyAddStream = InitialiazeStreamForDBApply();
            Object dbApplyUpdateStream = InitialiazeStreamForDBApply();
            Object dbApplyDeleteStream = InitialiazeStreamForDBApply();

            foreach (var routeKvp in routesWithCommandsByRSSN)
            {
                foreach (var routeWithCommands in routeKvp.Value)
                {
                    switch (routeWithCommands.ActionType)
                    {
                        case RouteActionType.Add: WriteRecordToStream(routeWithCommands.RouteCompareResult.Route, dbApplyAddStream); break;
                        case RouteActionType.Update: WriteRecordToStream(routeWithCommands.RouteCompareResult.Route, dbApplyUpdateStream); break;
                        case RouteActionType.Delete: WriteRecordToStream(routeWithCommands.RouteCompareResult.Route, dbApplyDeleteStream); break;
                        default: throw new NotSupportedException(string.Format("routeWithCommands.ActionType {0}", routeWithCommands.ActionType));
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

        public void ApplyRoutesSucceededForDB(object preparedRoute)
        {
            InsertBulkToTable(preparedRoute as BaseBulkInsertInfo);
        }

        #endregion

        #region IBulkApplyDataManager

        public object InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        public void WriteRecordToStream(HuaweiConvertedRoute record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("{0}^{1}^{2}^{3}", record.RSSN, record.Code, record.RSName, record.DNSet);
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                TableName = string.Format("[WhS_RouteSync_Huawei_{0}].[{1}]", SwitchId, BCPTableName),
                Stream = streamForBulkInsert,
                TabLock = true,
                KeepIdentity = false,
                FieldSeparator = '^',
                ColumnNames = columns
            };
        }

        #endregion
    }
}