using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.RouteSync.Huawei.Data;
using TOne.WhS.RouteSync.Huawei.Entities;
using Vanrise.Common;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace TOne.WhS.RouteSync.Huawei.RDB
{
    public class RouteDataManager : IRouteDataManager
    {
        #region Properties/Ctor

        string TABLE_Schema;
        private ConcurrentDictionary<string, string> TableNames;

        const string RouteTableName = "Route";
        const string RouteTempTableName = "Route_temp";
        const string RouteAddedTableName = "Route_Added";
        const string RouteUpdatedTableName = "Route_Updated";
        const string RouteDeletedTableName = "Route_Deleted";

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


        public void CompareTables(IRouteCompareTablesContext context)
        {
            var differences = new Dictionary<HuaweiConvertedRouteIdentifier, List<HuaweiConvertedRouteByCompare>>();

            TryRegisterTable(RouteTableName);
            var Route_TABLE_NAME = TableNames[RouteTableName];

            TryRegisterTable(RouteTempTableName);
            var RouteTemp_TABLE_NAME = TableNames[RouteTempTableName];

            var checkIfTableExistsQueryContext = new RDBQueryContext(GetDataProvider());

            var checkIfTableExistsQuery = checkIfTableExistsQueryContext.AddCheckIfTableExistsQuery();
            checkIfTableExistsQuery.TableName(Route_TABLE_NAME);


            var queryContext = new RDBQueryContext(GetDataProvider());

            if (checkIfTableExistsQueryContext.ExecuteScalar().IntValue > 0)
            {

                var selectQuery = queryContext.AddSelectQuery();
                selectQuery.SelectColumns().Columns(COL_RSSN, COL_Code, COL_RSName, COL_DNSet);
                selectQuery.SelectAggregates().Aggregate(RDBNonCountAggregateType.MAX, "tableName");

                var unionSelect = selectQuery.FromSelectUnion("v");

                var firstSelect = unionSelect.AddSelect();
                firstSelect.From(Route_TABLE_NAME, RouteTableName);
                firstSelect.SelectColumns().AllTableColumns(RouteTableName);
                firstSelect.SelectColumns().Expression("tableName").Value(RouteTableName);

                var secondSelect = unionSelect.AddSelect();
                secondSelect.From(RouteTemp_TABLE_NAME, RouteTempTableName);
                secondSelect.SelectColumns().AllTableColumns(RouteTempTableName);
                secondSelect.SelectColumns().Expression("tableName").Value(RouteTempTableName);


                var groupByContext = selectQuery.GroupBy();
                groupByContext.Select().Columns(COL_RSSN, COL_Code, COL_RSName, COL_DNSet);
                groupByContext.Having().CompareCount(RDBCompareConditionOperator.Eq, 1);
            }
            else
            {
                var selectQuery = queryContext.AddSelectQuery();

                selectQuery.From(RouteTemp_TABLE_NAME, RouteTempTableName);
                selectQuery.SelectColumns().AllTableColumns(RouteTempTableName);
                selectQuery.SelectColumns().Expression("tableName").Value(RouteTempTableName);
            }

            queryContext.ExecuteReader(reader =>
            {
                while (reader.Read())
                {
                    var convertedRouteByCompare = new HuaweiConvertedRouteByCompare() { HuaweiConvertedRoute = HuaweiConvertedRouteMapper(reader), TableName = reader.GetString("tableName") };

                    var routeIdentifier = new HuaweiConvertedRouteIdentifier()
                    {
                        RSSN = convertedRouteByCompare.HuaweiConvertedRoute.RSSN,
                        Code = convertedRouteByCompare.HuaweiConvertedRoute.Code,
                        DNSet = convertedRouteByCompare.HuaweiConvertedRoute.DNSet
                    };
                    List<HuaweiConvertedRouteByCompare> tempRouteDifferences = differences.GetOrCreateItem(routeIdentifier);
                    tempRouteDifferences.Add(convertedRouteByCompare);
                }

            });

            if (differences.Count == 0)
                return;

            var differencesByRSSN = new Dictionary<string, HuaweiConvertedRouteDifferences>();

            foreach (var differenceKvp in differences)
            {
                var routeDifferences = differenceKvp.Value;
                var difference = differencesByRSSN.GetOrCreateItem(differenceKvp.Key.RSSN);

                if (routeDifferences.Count == 1)
                {
                    var singleRouteDifference = differenceKvp.Value[0];
                    if (singleRouteDifference.TableName == RouteTableName)
                        difference.RoutesToDelete.Add(new HuaweiConvertedRouteCompareResult() { Route = singleRouteDifference.HuaweiConvertedRoute });
                    else
                        difference.RoutesToAdd.Add(new HuaweiConvertedRouteCompareResult() { Route = singleRouteDifference.HuaweiConvertedRoute });
                }
                else //routeDifferences.Count = 2
                {
                    var newRoute = routeDifferences.FindRecord(item => (string.Compare(item.TableName, RouteTempTableName, true) == 0));
                    var existingRoute = routeDifferences.FindRecord(item => (string.Compare(item.TableName, RouteTableName, true) == 0));

                    difference.RoutesToUpdate.Add(new HuaweiConvertedRouteCompareResult()
                    {
                        Route = newRoute.HuaweiConvertedRoute,
                        ExistingRoute = existingRoute.HuaweiConvertedRoute
                    });
                }
            }

            context.RouteDifferencesByRSSN = differencesByRSSN;
        }

        public void Finalize(IRouteFinalizeContext context)
        {
            SwapTables(RouteTableName, RouteTempTableName);
            DeleteRouteTable(RouteAddedTableName);
            DeleteRouteTable(RouteUpdatedTableName);
            DeleteRouteTable(RouteDeletedTableName);
        }

        #region IBulkApplyDataManager

        public object InitialiazeStreamForDBApply()
        {
            TryRegisterTable(RouteTempTableName);
            var RouteTemp_TABLE_NAME = TableNames[RouteTempTableName];

            var queryContext = new RDBQueryContext(GetDataProvider());
            var streamForBulkInsert = queryContext.StartBulkInsert();
            streamForBulkInsert.IntoTable(RouteTemp_TABLE_NAME, '^', COL_RSSN, COL_Code, COL_RSName, COL_DNSet);

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

            recordContext.Value(record.Code);

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

        public void ApplyRouteForDB(object preparedRoute)
        {
            preparedRoute.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream").Apply();
        }

        #endregion

        public void Initialize(IRouteInitializeContext context)
        {
            Guid guid = Guid.NewGuid();

            CreateRouteTempTableQuery(guid);
            CreateRouteTableQuery(guid);

            #region Added

            SyncWithRouteAddedTable();
            CreateCommonRouteTableQuery(guid, RouteAddedTableName);

            #endregion

            #region Updated

            SyncWithRouteUpdatedTableQuery();

            CreateCommonRouteTableQuery(guid, RouteUpdatedTableName);
            #endregion

            #region Deleted
            SyncWithRouteDeletedTable();

            CreateCommonRouteTableQuery(guid, RouteDeletedTableName);
            #endregion
        }
        private void SyncWithRouteUpdatedTableQuery()
        {
            TryRegisterTable(RouteUpdatedTableName);
            var RouteUpdated_TABLE_NAME = TableNames[RouteUpdatedTableName];

            var checkIfTableExistsQueryContext = new RDBQueryContext(GetDataProvider());

            var checkIfTableExistsQuery = checkIfTableExistsQueryContext.AddCheckIfTableExistsQuery();
            checkIfTableExistsQuery.TableName(RouteUpdated_TABLE_NAME);

            if (checkIfTableExistsQueryContext.ExecuteScalar().IntValue <= 0)
                return;

            TryRegisterTable(RouteTableName);
            var Route_TABLE_NAME = TableNames[RouteTableName];

            var queryContext = new RDBQueryContext(GetDataProvider());

            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TableNames[RouteTableName]);

            var joinStatement = updateQuery.Join("routes");
            var joinContext = joinStatement.Join(RouteUpdated_TABLE_NAME, "updatedRoutes");
            updateQuery.Column(COL_RSName).Column("updatedRoutes", COL_RSName);

            joinContext.JoinType(RDBJoinType.Inner);
            var joinCondition = joinContext.On();
            joinCondition.EqualsCondition("updatedRoutes", COL_RSSN, "routes", COL_RSSN);
            joinCondition.EqualsCondition("updatedRoutes", COL_Code, "routes", COL_Code);
            joinCondition.EqualsCondition("updatedRoutes", COL_DNSet, "routes", COL_DNSet);

            queryContext.AddDropTableQuery().TableName(RouteUpdated_TABLE_NAME);

            queryContext.ExecuteNonQuery();
        }

        public void InsertRoutesToTempTable(IEnumerable<HuaweiConvertedRoute> routes)
        {
            if (routes != null && routes.Any())
            {
                object dbApplyStream = InitialiazeStreamForDBApply();
                foreach (var route in routes)
                {
                    WriteRecordToStream(route, dbApplyStream);
                }
                object obj = FinishDBApplyStream(dbApplyStream);
                ApplyRouteForDB(obj);
            }
        }

        public void RemoveRoutesFromTempTable(IEnumerable<HuaweiConvertedRoute> routes)
        {
            if (routes == null || !routes.Any())
                return;

            TryRegisterTable(RouteTempTableName);
            var RouteTempTABLE_NAME = TableNames[RouteTempTableName];

            var queryContext = new RDBQueryContext(GetDataProvider());

            var tempTableContext = CreateRouteTempTableForJoin(routes, RouteTempTABLE_NAME, queryContext);

            var deleteQuery = queryContext.AddDeleteQuery();
            deleteQuery.FromTable(RouteTempTABLE_NAME);

            var joinStatement = deleteQuery.Join("tempRoute");

            var joinCondition = joinStatement.Join(tempTableContext, "route").On();
            joinCondition.EqualsCondition("route", COL_RSSN, "tempRoute", COL_RSSN);
            joinCondition.EqualsCondition("route", COL_Code, "tempRoute", COL_Code);
            joinCondition.EqualsCondition("route", COL_RSName, "tempRoute", COL_RSName);
            joinCondition.EqualsCondition("route", COL_DNSet, "tempRoute", COL_DNSet);

            queryContext.ExecuteNonQuery();
        }

        public void UpdateRoutesInTempTable(IEnumerable<HuaweiConvertedRoute> routes)
        {
            if (routes == null || !routes.Any())
                return;

            TryRegisterTable(RouteTempTableName);
            var RouteTempTABLE_NAME = TableNames[RouteTempTableName];

            var queryContext = new RDBQueryContext(GetDataProvider());

            var tempTableContext = CreateRouteTempTableForJoin(routes, RouteTempTABLE_NAME, queryContext);

            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(RouteTempTABLE_NAME);
            updateQuery.Column(COL_RSName).Column("routesToUpdate", COL_RSName);
            updateQuery.Column(COL_DNSet).Column("routesToUpdate", COL_DNSet);

            var joinStatement = updateQuery.Join("tempRoutes");

            var joinCondition = joinStatement.Join(tempTableContext, "routesToUpdate").On();
            joinCondition.EqualsCondition("routesToUpdate", COL_RSSN, "tempRoutes", COL_RSSN);
            joinCondition.EqualsCondition("routesToUpdate", COL_Code, "tempRoutes", COL_Code);
            joinCondition.EqualsCondition("routesToUpdate", COL_DNSet, "tempRoutes", COL_DNSet);

            queryContext.ExecuteNonQuery();
        }

        private static RDBTempTableQuery CreateRouteTempTableForJoin(IEnumerable<HuaweiConvertedRoute> routes, string RouteTempTABLE_NAME, RDBQueryContext queryContext)
        {
            var tempTableContext = queryContext.CreateTempTable();
            tempTableContext.AddColumnsFromTable(RouteTempTABLE_NAME);

            var insertMultipleRowsQuery = queryContext.AddInsertMultipleRowsQuery();
            insertMultipleRowsQuery.IntoTable(tempTableContext);

            foreach (var route in routes)
            {
                var rowContext = insertMultipleRowsQuery.AddRow();

                rowContext.Column(COL_RSSN).Value(route.RSSN);
                rowContext.Column(COL_Code).Value(route.Code);
                rowContext.Column(COL_RSName).Value(route.RSName);
                rowContext.Column(COL_DNSet).Value(route.DNSet);
            }

            return tempTableContext;
        }

        #region Private Methods

        private void CreateRouteTempTableQuery(Guid guid)
        {
            TryRegisterTable(RouteTempTableName);
            var TABLE_NAME = TableNames[RouteTempTableName];

            var dropTableQueryContext = new RDBQueryContext(GetDataProvider());

            var dropTableQuery = dropTableQueryContext.AddDropTableQuery();
            dropTableQuery.TableName(TABLE_NAME);

            dropTableQueryContext.ExecuteNonQuery();

            CreateCommonRouteTableQuery(guid, RouteTempTableName);
        }

        private void CreateRouteTableQuery(Guid guid)
        {
            TryRegisterTable(RouteTableName);
            var TABLE_NAME = TableNames[RouteTableName];

            var checkIfTableExistsQueryContext = new RDBQueryContext(GetDataProvider());

            var checkIfTableExistsQuery = checkIfTableExistsQueryContext.AddCheckIfTableExistsQuery();
            checkIfTableExistsQuery.TableName(TABLE_NAME);

            if (checkIfTableExistsQueryContext.ExecuteScalar().IntValue > 0)
                return;
            CreateCommonRouteTableQuery(guid, RouteTableName);
        }

        private void CreateCommonRouteTableQuery(Guid guid, string TABLE_NAME)
        {
            var createTableQueryContext = new RDBQueryContext(GetDataProvider());

            var createTableQuery = createTableQueryContext.AddCreateTableQuery();

            createTableQuery.DBTableName(TABLE_Schema, TABLE_NAME);

            createTableQuery.AddColumn(COL_RSSN, RDBDataType.NVarchar, 255, null, true);
            createTableQuery.AddColumn(COL_Code, RDBDataType.Varchar, 20, null, true);
            createTableQuery.AddColumn(COL_RSName, RDBDataType.Varchar, true);
            createTableQuery.AddColumn(COL_DNSet, RDBDataType.Int, true);


            var createIndexQuery = createTableQueryContext.AddCreateIndexQuery();
            createIndexQuery.DBTableName(TABLE_Schema, TABLE_NAME);
            createIndexQuery.IndexName($"[PK_{TABLE_Schema}{SwitchId}.{TABLE_NAME}{guid}]");
            createIndexQuery.IndexType(RDBCreateIndexType.UniqueClustered);
            createIndexQuery.AddColumn(COL_RSSN);
            createIndexQuery.AddColumn(COL_Code);

            createTableQueryContext.ExecuteNonQuery();
        }

        private void SyncWithRouteAddedTable()
        {
            TryRegisterTable(RouteAddedTableName);
            var RouteAdded_TABLE_NAME = TableNames[RouteAddedTableName];

            TryRegisterTable(RouteTableName);
            var RouteTable_TABLE_NAME = TableNames[RouteTableName];


            var checkIfTableExistsQueryContext = new RDBQueryContext(GetDataProvider());

            var checkIfTableExistsQuery = checkIfTableExistsQueryContext.AddCheckIfTableExistsQuery();
            checkIfTableExistsQuery.TableName(RouteAdded_TABLE_NAME);

            if (checkIfTableExistsQueryContext.ExecuteScalar().IntValue <= 0)
                return;

            var queryContext = new RDBQueryContext(GetDataProvider());

            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(RouteTable_TABLE_NAME);

            var fromSelectQuery = insertQuery.FromSelect();
            fromSelectQuery.From(RouteAdded_TABLE_NAME, RouteAddedTableName);
            fromSelectQuery.SelectColumns().Columns(COL_RSSN, COL_Code, COL_RSName, COL_DNSet);

            queryContext.AddDropTableQuery().TableName(RouteAdded_TABLE_NAME);

            queryContext.ExecuteNonQuery(true);
        }

        private void SyncWithRouteDeletedTable()
        {
            TryRegisterTable(RouteDeletedTableName);
            var RouteDeleted_TABLE_NAME = TableNames[RouteDeletedTableName];

            var checkIfTableExistsQueryContext = new RDBQueryContext(GetDataProvider());

            var checkIfTableExistsQuery = checkIfTableExistsQueryContext.AddCheckIfTableExistsQuery();
            checkIfTableExistsQuery.TableName(RouteDeleted_TABLE_NAME);

            if (checkIfTableExistsQueryContext.ExecuteScalar().IntValue <= 0)
                return;

            var queryContext = new RDBQueryContext(GetDataProvider());

            var deleteQuery = queryContext.AddDeleteQuery();
            deleteQuery.FromTable(TableNames[RouteTableName]);

            var joinStatement = deleteQuery.Join(RouteTableName);
            var joinContext = joinStatement.Join(RouteDeleted_TABLE_NAME, RouteDeletedTableName);
            joinContext.JoinType(RDBJoinType.Inner);
            var joinCondition = joinContext.On();
            joinCondition.EqualsCondition(RouteDeletedTableName, COL_RSSN, RouteTableName, COL_RSSN);
            joinCondition.EqualsCondition(RouteDeletedTableName, COL_Code, RouteTableName, COL_Code);
            joinCondition.EqualsCondition(RouteDeletedTableName, COL_DNSet, RouteTableName, COL_DNSet);

            queryContext.AddDropTableQuery().TableName(RouteDeleted_TABLE_NAME);

            queryContext.ExecuteNonQuery();
        }

        private void DeleteRouteTable(string tableName)
        {
            TryRegisterTable(tableName);
            var TABLE_NAME = TableNames[tableName];

            var queryContext = new RDBQueryContext(GetDataProvider());
            queryContext.AddDropTableQuery().TableName(TABLE_NAME);

            queryContext.ExecuteNonQuery();
        }

        private void SwapTables(string existingTableName, string newTableName)
        {
            TryRegisterTable(existingTableName);
            var Existing_TABLE_NAME = TableNames[existingTableName];
            TryRegisterTable(newTableName);
            var New_TABLE_NAME = TableNames[newTableName];

            var queryContext = new RDBQueryContext(GetDataProvider());
            var swapTablesQuery = queryContext.AddSwapTablesQuery();
            swapTablesQuery.TableNames(Existing_TABLE_NAME, New_TABLE_NAME, true);

            queryContext.ExecuteNonQuery();
        }

        private static Dictionary<string, RDBTableColumnDefinition> GetRDBTableColumnDefinitionDictionary()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_RSSN, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
            columns.Add(COL_Code, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 20 });
            columns.Add(COL_RSName, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar });
            columns.Add(COL_DNSet, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            return columns;
        }


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

        #endregion

        #region Private Classes

        private class HuaweiConvertedRouteByCompare
        {
            public HuaweiConvertedRoute HuaweiConvertedRoute { get; set; }

            public string TableName { get; set; }
        }

        #endregion

        #region Mappers

        private HuaweiConvertedRoute HuaweiConvertedRouteMapper(IRDBDataReader reader)
        {
            return new HuaweiConvertedRoute()
            {
                RSSN = reader.GetString(COL_RSSN),
                Code = reader.GetString(COL_Code),
                RSName = reader.GetString(COL_RSName),
                DNSet = reader.GetInt(COL_DNSet)
            };
        }

        #endregion
    }
}
