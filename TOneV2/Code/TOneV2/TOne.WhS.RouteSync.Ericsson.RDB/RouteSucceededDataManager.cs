using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.RouteSync.Ericsson.Data;
using TOne.WhS.RouteSync.Ericsson.Entities;
using Vanrise.Common;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace TOne.WhS.RouteSync.Ericsson.RDB
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

        const string COL_BO = "BO";
        const string COL_Code = "Code";
        const string COL_RCNumber = "RCNumber";
        const string COL_RouteType = "RouteType";

        public string SwitchId { get; set; }

        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("RouteSync", "TOneWhS_RouteSync_DBConnStringKey", "RouteSyncDBConnString");
        }

        #endregion

        #region Public Methods

        public void SaveRoutesSucceededToDB(Dictionary<int, List<EricssonRouteWithCommands>> routesWithCommandsByBO)
        {
            if (routesWithCommandsByBO == null || routesWithCommandsByBO.Count == 0)
                return;

            TryRegisterTable(RouteAddedTableName);
            Object dbApplyAddStream = InitialiazeStreamForDBApply();

            TryRegisterTable(RouteUpdatedTableName);
            Object dbApplyUpdateStream = InitialiazeStreamForDBApply();

            TryRegisterTable(RouteDeletedTableName);
            Object dbApplyDeleteStream = InitialiazeStreamForDBApply();

            foreach (var routeKVP in routesWithCommandsByBO)
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


            Object preparedAddRoutes = FinishDBApplyStream(dbApplyAddStream);
            ApplyRoutesSucceededForDB(preparedAddRoutes);

            Object preparedUpdatedRoutes = FinishDBApplyStream(dbApplyUpdateStream);
            ApplyRoutesSucceededForDB(preparedUpdatedRoutes);

            Object preparedDeletedRoutes = FinishDBApplyStream(dbApplyDeleteStream);
            ApplyRoutesSucceededForDB(preparedDeletedRoutes);
        }

        #endregion

        #region IBulkApplyDataManager

        public object InitialiazeStreamForDBApply()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var streamForBulkInsert = queryContext.StartBulkInsert();
            streamForBulkInsert.IntoTable(BCPTableName, '^', COL_BO, COL_Code, COL_RCNumber, COL_RouteType);

            return streamForBulkInsert;
        }

        public void WriteRecordToStream(EricssonConvertedRoute record, object dbApplyStream)
        {
            RDBBulkInsertQueryContext bulkInsertContext = dbApplyStream.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream");
            var recordContext = bulkInsertContext.WriteRecord();

            if (record.OBA != null)
                recordContext.Value(record.OBA);
            else
                recordContext.Value(string.Empty);

            if (record.Code != null)
                recordContext.Value(record.Code);
            else
                recordContext.Value(string.Empty);

            recordContext.Value(record.RCNumber);

            recordContext.Value((int)record.RouteType);
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
            TABLE_Schema = ($"WhS_RouteSync_Ericsson_{SwitchId}");
            var TABLE_NAME = ($"{TABLE_Schema}.[{TableName}]");
            BCPTableName = TABLE_NAME;

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
            columns.Add(COL_BO, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
            columns.Add(COL_Code, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 20 });
            columns.Add(COL_RCNumber, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_RouteType, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            return columns;
        }

        #endregion
    }
}
