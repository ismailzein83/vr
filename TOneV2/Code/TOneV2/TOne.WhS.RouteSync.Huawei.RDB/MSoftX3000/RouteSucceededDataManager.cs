using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using TOne.WhS.RouteSync.Huawei.Data;
using TOne.WhS.RouteSync.Huawei.Entities;
using Vanrise.Common;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace TOne.WhS.RouteSync.Huawei.RDB
{
    class RouteSucceededDataManager : IRouteSucceededDataManager
    {
        #region Properties/Ctor
        string TABLE_Schema;
        private ConcurrentDictionary<string, string> TableNames;

        const string RouteAddedTableName = "Route_Added";
        const string RouteUpdatedTableName = "Route_Updated";
        const string RouteDeletedTableName = "Route_Deleted";
        private string BCPTableName;

        const string COL_RSSN = "RSSN";
        const string COL_Code = "Code";
        const string COL_RSName = "RSName";
        const string COL_DNSet = "DNSet";

        public string SwitchId { get; set; }

        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("RouteSync", "TOneWhS_RouteSync_DBConnStringKey", "RouteSyncDBConnString");
        }

        #endregion

        #region Public Methods

        public void SaveRoutesSucceededToDB(Dictionary<string, List<HuaweiRouteWithCommands>> routesWithCommandsByRSSN)
        {
            if (routesWithCommandsByRSSN == null || routesWithCommandsByRSSN.Count == 0)
                return;

            TryRegisterTable(RouteAddedTableName);
            TryRegisterTable(RouteUpdatedTableName);
            TryRegisterTable(RouteDeletedTableName);

            BCPTableName = TableNames[RouteAddedTableName];
            Object dbApplyAddStream = InitialiazeStreamForDBApply();
            BCPTableName = TableNames[RouteUpdatedTableName];
            Object dbApplyUpdateStream = InitialiazeStreamForDBApply();
            BCPTableName = TableNames[RouteDeletedTableName];
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

            Object preparedAddRoutes = FinishDBApplyStream(dbApplyAddStream);
            ApplyRoutesSucceededForDB(preparedAddRoutes);

            BCPTableName = RouteUpdatedTableName;
            Object preparedUpdatedRoutes = FinishDBApplyStream(dbApplyUpdateStream);
            ApplyRoutesSucceededForDB(preparedUpdatedRoutes);

            BCPTableName = RouteDeletedTableName;
            Object preparedDeletedRoutes = FinishDBApplyStream(dbApplyDeleteStream);
            ApplyRoutesSucceededForDB(preparedDeletedRoutes);
        }


        #endregion

        #region IBulkApplyDataManager

        public object InitialiazeStreamForDBApply()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var streamForBulkInsert = queryContext.StartBulkInsert();
            streamForBulkInsert.IntoTable(BCPTableName, '^', COL_RSSN, COL_Code, COL_RSName, COL_DNSet);

            return streamForBulkInsert;
        }

        public void WriteRecordToStream(HuaweiConvertedRoute record, object dbApplyStream)
        {
            RDBBulkInsertQueryContext bulkInsertContext = dbApplyStream.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream");
            var recordContext = bulkInsertContext.WriteRecord();

            if (record.RSSN != null)
                recordContext.Value(record.RSSN);
            else
                recordContext.Value(string.Empty);

            if (record.Code != null)
                recordContext.Value(record.Code);
            else
                recordContext.Value(string.Empty);

            if (record.RSName != null)
                recordContext.Value(record.RSName);
            else
                recordContext.Value(string.Empty);

            recordContext.Value(record.DNSet);
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            RDBBulkInsertQueryContext bulkInsertContext = dbApplyStream.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream");
            bulkInsertContext.CloseStream();
            return bulkInsertContext;
        }
        public void ApplyRoutesSucceededForDB(object preparedRoute)
        {
            preparedRoute.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream").Apply();
        }

        #endregion

        #region Private Methods
        private void TryRegisterTable(string TableName)
        {
            TABLE_Schema = ($"WhS_RouteSync_Huawei_{SwitchId}");
            var TABLE_NAME = ($"{TABLE_Schema}.[{TableName}]");

            if (TableNames == null)
                TableNames = new ConcurrentDictionary<string, string>();
            TableNames.TryAdd(TableName, TABLE_NAME);

            RDBSchemaManager.Current.TryRegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = TABLE_Schema,
                DBTableName = TableName,
                Columns = GetRDBTableColumnDefinitionDictionary()
            });
        }

        private Dictionary<string, RDBTableColumnDefinition> GetRDBTableColumnDefinitionDictionary()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_RSSN, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
            columns.Add(COL_Code, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 20 });
            columns.Add(COL_RSName, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar });
            columns.Add(COL_DNSet, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            return columns;
        }
        #endregion
    }
}
