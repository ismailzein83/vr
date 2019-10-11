using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.RouteSync.Huawei.SoftX3000.Data;
using TOne.WhS.RouteSync.Huawei.SoftX3000.Entities;
using Vanrise.Common;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace TOne.WhS.RouteSync.Huawei.SoftX3000.RDB
{
    public class RouteDataManager : IRouteDataManager
    {
        #region Properties/Ctor

        private string TABLE_Schema;
        private ConcurrentDictionary<string, string> TableNames;

        const string RouteTableName = "Route";
        const string RouteTempTableName = "Route_temp";
        const string RouteAddedTableName = "Route_Added";
        const string RouteUpdatedTableName = "Route_Updated";
        const string RouteDeletedTableName = "Route_Deleted";

        const string COL_DNSet = "DNSet";
        const string COL_Code = "Code";
        const string COL_RouteCaseId = "RouteCaseId";
        const string COL_RSSC = "RSSC";
        const string COL_MINL = "MINL";
        const string COL_MAXL = "MAXL";
        const string COL_CC = "CC";

        private Dictionary<string, RDBTableColumnDefinition> RDBTableColumnDefinitionDictionary = new Dictionary<string, RDBTableColumnDefinition>()
            {
                { COL_DNSet, new RDBTableColumnDefinition { DataType = RDBDataType.Int }},
                { COL_Code, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 20 }},
                { COL_RouteCaseId, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt }},
                { COL_RSSC, new RDBTableColumnDefinition { DataType = RDBDataType.Int }},
                { COL_MINL, new RDBTableColumnDefinition { DataType = RDBDataType.Int }},
                { COL_MAXL, new RDBTableColumnDefinition { DataType = RDBDataType.Int }},
                { COL_CC, new RDBTableColumnDefinition { DataType = RDBDataType.Int }},
            };

        string _switchId;
        public string SwitchId
        {
            get
            {
                return _switchId;
            }
            set
            {
                _switchId = value;
                TABLE_Schema = $"WhS_RouteSync_HuaweiSoftX3000_{value}";
            }
        }

        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("RouteSync", "TOneWhS_RouteSync_DBConnStringKey", "RouteSyncDBConnString");
        }

        #endregion

        #region First Step

        #region Public Methods

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

        #endregion

        #region Private Methods

        private void CreateRouteTempTableQuery(Guid guid)
        {
            TryRegisterTable(RouteTempTableName);
            var tableName = TableNames.GetRecord(RouteTempTableName);

            var dropTableQueryContext = new RDBQueryContext(GetDataProvider());

            var dropTableQuery = dropTableQueryContext.AddDropTableQuery();
            dropTableQuery.TableName(tableName);

            dropTableQueryContext.ExecuteNonQuery();

            CreateCommonRouteTableQuery(guid, RouteTempTableName);
        }

        private void CreateRouteTableQuery(Guid guid)
        {
            TryRegisterTable(RouteTableName);
            var tableName = TableNames.GetRecord(RouteTableName);

            var checkIfTableExistsQueryContext = new RDBQueryContext(GetDataProvider());

            var checkIfTableExistsQuery = checkIfTableExistsQueryContext.AddCheckIfTableExistsQuery();
            checkIfTableExistsQuery.TableName(tableName);

            if (checkIfTableExistsQueryContext.ExecuteScalar().IntValue > 0)
                return;

            CreateCommonRouteTableQuery(guid, RouteTableName);
        }

        private void CreateCommonRouteTableQuery(Guid guid, string tableName)
        {
            var createTableQueryContext = new RDBQueryContext(GetDataProvider());

            var createTableQuery = createTableQueryContext.AddCreateTableQuery();

            createTableQuery.DBTableName(TABLE_Schema, tableName);

            createTableQuery.AddColumn(COL_DNSet, RDBDataType.Int, true);
            createTableQuery.AddColumn(COL_Code, RDBDataType.Varchar, 20, null, true);
            createTableQuery.AddColumn(COL_RouteCaseId, RDBDataType.BigInt, true);
            createTableQuery.AddColumn(COL_RSSC, RDBDataType.Int, true);
            createTableQuery.AddColumn(COL_MINL, RDBDataType.Int, true);
            createTableQuery.AddColumn(COL_MAXL, RDBDataType.Int, true);
            createTableQuery.AddColumn(COL_CC, RDBDataType.Int, true);
            

            var createIndexQuery = createTableQueryContext.AddCreateIndexQuery();
            createIndexQuery.DBTableName(TABLE_Schema, tableName);
            createIndexQuery.IndexName($"[PK_{TABLE_Schema}.{tableName}{guid}]");
            createIndexQuery.IndexType(RDBCreateIndexType.UniqueClustered);
            createIndexQuery.AddColumn(COL_RSSC);
            createIndexQuery.AddColumn(COL_Code);

            createTableQueryContext.ExecuteNonQuery();
        }

        private void SyncWithRouteAddedTable()
        {
            TryRegisterTable(RouteAddedTableName);
            var routeAdded_TABLE_NAME = TableNames.GetRecord(RouteAddedTableName);
            var routeTable_TABLE_NAME = TableNames.GetRecord(RouteTableName);

            var checkIfTableExistsQueryContext = new RDBQueryContext(GetDataProvider());

            var checkIfTableExistsQuery = checkIfTableExistsQueryContext.AddCheckIfTableExistsQuery();
            checkIfTableExistsQuery.TableName(routeAdded_TABLE_NAME);

            if (checkIfTableExistsQueryContext.ExecuteScalar().IntValue <= 0)
                return;

            var queryContext = new RDBQueryContext(GetDataProvider());

            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(routeTable_TABLE_NAME);

            var fromSelectQuery = insertQuery.FromSelect();
            fromSelectQuery.From(routeAdded_TABLE_NAME, RouteAddedTableName);
            fromSelectQuery.SelectColumns().Columns(COL_DNSet, COL_Code, COL_RouteCaseId, COL_RSSC, COL_MINL, COL_MAXL, COL_CC);

            queryContext.AddDropTableQuery().TableName(routeAdded_TABLE_NAME);

            queryContext.ExecuteNonQuery(true);
        }

        private void SyncWithRouteUpdatedTableQuery()
        {
            TryRegisterTable(RouteUpdatedTableName);
            var routeUpdated_TABLE_NAME = TableNames.GetRecord(RouteUpdatedTableName);

            var checkIfTableExistsQueryContext = new RDBQueryContext(GetDataProvider());

            var checkIfTableExistsQuery = checkIfTableExistsQueryContext.AddCheckIfTableExistsQuery();
            checkIfTableExistsQuery.TableName(routeUpdated_TABLE_NAME);

            if (checkIfTableExistsQueryContext.ExecuteScalar().IntValue <= 0)
                return;

            var routeTable_TABLE_NAME = TableNames.GetRecord(RouteTableName);

            var queryContext = new RDBQueryContext(GetDataProvider());

            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(routeTable_TABLE_NAME);

            var joinStatement = updateQuery.Join(RouteTableName);
            var joinContext = joinStatement.Join(routeUpdated_TABLE_NAME, RouteUpdatedTableName);

            updateQuery.Column(COL_RouteCaseId).Column(RouteUpdatedTableName, COL_RouteCaseId);

            joinContext.JoinType(RDBJoinType.Inner);
            var joinCondition = joinContext.On();
            joinCondition.EqualsCondition(RouteUpdatedTableName, COL_RSSC, RouteTableName, COL_RSSC);
            joinCondition.EqualsCondition(RouteUpdatedTableName, COL_Code, RouteTableName, COL_Code);
            joinCondition.EqualsCondition(RouteUpdatedTableName, COL_DNSet, RouteTableName, COL_DNSet);

            queryContext.AddDropTableQuery().TableName(routeUpdated_TABLE_NAME);

            queryContext.ExecuteNonQuery();
        }

        private void SyncWithRouteDeletedTable()
        {
            TryRegisterTable(RouteDeletedTableName);
            var routeDeleted_TABLE_NAME = TableNames.GetRecord(RouteDeletedTableName);

            var checkIfTableExistsQueryContext = new RDBQueryContext(GetDataProvider());

            var checkIfTableExistsQuery = checkIfTableExistsQueryContext.AddCheckIfTableExistsQuery();
            checkIfTableExistsQuery.TableName(routeDeleted_TABLE_NAME);

            if (checkIfTableExistsQueryContext.ExecuteScalar().IntValue <= 0)
                return;

            var queryContext = new RDBQueryContext(GetDataProvider());

            var routeTable_TABLE_NAME = TableNames.GetRecord(RouteTableName);

            var deleteQuery = queryContext.AddDeleteQuery();
            deleteQuery.FromTable(routeTable_TABLE_NAME);

            var joinStatement = deleteQuery.Join(RouteTableName);
            var joinContext = joinStatement.Join(routeDeleted_TABLE_NAME, RouteDeletedTableName);
            joinContext.JoinType(RDBJoinType.Inner);
            var joinCondition = joinContext.On();
            joinCondition.EqualsCondition(RouteDeletedTableName, COL_RSSC, RouteTableName, COL_RSSC);
            joinCondition.EqualsCondition(RouteDeletedTableName, COL_Code, RouteTableName, COL_Code);
            joinCondition.EqualsCondition(RouteDeletedTableName, COL_DNSet, RouteTableName, COL_DNSet);

            queryContext.AddDropTableQuery().TableName(routeDeleted_TABLE_NAME);

            queryContext.ExecuteNonQuery();
        }

        #endregion

        #endregion

        #region Second Step

        #region IBulkApplyDataManager

        public object InitialiazeStreamForDBApply()
        {
            TryRegisterTable(RouteTempTableName);
            var RouteTemp_TABLE_NAME = TableNames[RouteTempTableName];

            var queryContext = new RDBQueryContext(GetDataProvider());
            var streamForBulkInsert = queryContext.StartBulkInsert();
            streamForBulkInsert.IntoTable(RouteTemp_TABLE_NAME, '=', COL_DNSet, COL_RSSC, COL_Code, COL_RouteCaseId, COL_MINL, COL_MAXL, COL_CC);

            return streamForBulkInsert;
        }

        public void WriteRecordToStream(HuaweiConvertedRoute record, object dbApplyStream)
        {
            RDBBulkInsertQueryContext bulkInsertContext = dbApplyStream.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream");
            var recordContext = bulkInsertContext.WriteRecord();

            if (record.RSSC == default(int))
                throw new ArgumentNullException($"record.RSSC : {record.RSSC}");

            record.Code.ThrowIfNull("record.Code");

            if (record.RouteCaseId == default(long))
                throw new ArgumentNullException($"record.RouteCaseId : {record.RSSC}");

            recordContext.Value(record.DNSet);
            recordContext.Value(record.RSSC);
            recordContext.Value(record.Code);
            recordContext.Value(record.RouteCaseId);
            recordContext.Value(record.MinL);
            recordContext.Value(record.MaxL);
            recordContext.Value(record.CC);
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

        #endregion

        #region Third Step

        #region Public Methods

        public void CompareTables(IRouteCompareTablesContext context)
        {
            TryRegisterTable(RouteTableName);
            var Route_TABLE_NAME = TableNames.GetRecord(RouteTableName);

            TryRegisterTable(RouteTempTableName);
            var RouteTemp_TABLE_NAME = TableNames.GetRecord(RouteTempTableName);

            var checkIfTableExistsQueryContext = new RDBQueryContext(GetDataProvider());

            var checkIfTableExistsQuery = checkIfTableExistsQueryContext.AddCheckIfTableExistsQuery();
            checkIfTableExistsQuery.TableName(Route_TABLE_NAME);

            var queryContext = new RDBQueryContext(GetDataProvider());

            if (checkIfTableExistsQueryContext.ExecuteScalar().IntValue > 0)
            {
                var selectQuery = queryContext.AddSelectQuery();
                selectQuery.SelectColumns().Columns(COL_DNSet, COL_RSSC, COL_RouteCaseId, COL_Code, COL_MINL, COL_MAXL, COL_CC);
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
                groupByContext.Select().Columns(COL_RSSC, COL_Code, COL_RouteCaseId, COL_DNSet);
                groupByContext.Having().CompareCount(RDBCompareConditionOperator.Eq, 1);
            }
            else
            {
                var selectQuery = queryContext.AddSelectQuery();

                selectQuery.From(RouteTemp_TABLE_NAME, RouteTempTableName);
                selectQuery.SelectColumns().AllTableColumns(RouteTempTableName);
                selectQuery.SelectColumns().Expression("tableName").Value(RouteTempTableName);
            }

            var differences = new Dictionary<HuaweiConvertedRouteIdentifier, List<HuaweiConvertedRouteByCompare>>();

            queryContext.ExecuteReader(reader =>
            {
                while (reader.Read())
                {
                    var convertedRouteByCompare = new HuaweiConvertedRouteByCompare() { HuaweiConvertedRoute = HuaweiConvertedRouteMapper(reader), TableName = reader.GetString("tableName") };

                    var routeIdentifier = new HuaweiConvertedRouteIdentifier()
                    {
                        RSSC = convertedRouteByCompare.HuaweiConvertedRoute.RSSC,
                        Code = convertedRouteByCompare.HuaweiConvertedRoute.Code,
                        DNSet = convertedRouteByCompare.HuaweiConvertedRoute.DNSet,
                    };
                    List<HuaweiConvertedRouteByCompare> tempRouteDifferences = differences.GetOrCreateItem(routeIdentifier);
                    tempRouteDifferences.Add(convertedRouteByCompare);
                }

            });

            if (differences.Count == 0)
                return;

            var differencesByRSSC = new Dictionary<int, HuaweiConvertedRouteDifferences>();

            foreach (var differenceKvp in differences)
            {
                var routeDifferences = differenceKvp.Value;
                var difference = differencesByRSSC.GetOrCreateItem(differenceKvp.Key.RSSC);

                if (routeDifferences.Count == 1)
                {
                    var singleRouteDifference = routeDifferences.ElementAt(0);
                    if (string.Compare(singleRouteDifference.TableName ,RouteTableName) == 0)
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

            context.RouteDifferencesByRSSC = differencesByRSSC;
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
            if (routes == null || routes.Count() == 0)
                return;

            TryRegisterTable(RouteTempTableName);
            var RouteTempTABLE_NAME = TableNames.GetRecord(RouteTempTableName);

            var queryContext = new RDBQueryContext(GetDataProvider());

            var tempTableContext = CreateRouteTempTableContextForJoin(routes, RouteTempTABLE_NAME, queryContext);

            var deleteQuery = queryContext.AddDeleteQuery();
            deleteQuery.FromTable(RouteTempTABLE_NAME);

            var joinStatement = deleteQuery.Join("tempRoute");

            var joinCondition = joinStatement.Join(tempTableContext, "route").On();
            joinCondition.EqualsCondition("route", COL_RSSC, "tempRoute", COL_RSSC);
            joinCondition.EqualsCondition("route", COL_Code, "tempRoute", COL_Code);
            joinCondition.EqualsCondition("route", COL_RouteCaseId, "tempRoute", COL_RouteCaseId);
            joinCondition.EqualsCondition("route", COL_DNSet, "tempRoute", COL_DNSet);
            joinCondition.EqualsCondition("route", COL_MINL, "tempRoute", COL_MINL);
            joinCondition.EqualsCondition("route", COL_MAXL, "tempRoute", COL_MAXL);
            joinCondition.EqualsCondition("route", COL_CC, "tempRoute", COL_CC);

            queryContext.ExecuteNonQuery();
        }

        public void UpdateRoutesInTempTable(IEnumerable<HuaweiConvertedRoute> routes)
        {
            if (routes == null || routes.Count() == 0)
                return;

            TryRegisterTable(RouteTempTableName);
            var RouteTempTABLE_NAME = TableNames.GetRecord(RouteTempTableName);

            var queryContext = new RDBQueryContext(GetDataProvider());

            var tempTableContext = CreateRouteTempTableContextForJoin(routes, RouteTempTABLE_NAME, queryContext);

            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(RouteTempTABLE_NAME);
            updateQuery.Column(COL_RouteCaseId).Column("routesToUpdate", COL_RouteCaseId);
            updateQuery.Column(COL_MINL).Column("routesToUpdate", COL_MINL);
            updateQuery.Column(COL_MAXL).Column("routesToUpdate", COL_MAXL);
            updateQuery.Column(COL_CC).Column("routesToUpdate", COL_CC);

            var joinStatement = updateQuery.Join("tempRoutes");

            var joinCondition = joinStatement.Join(tempTableContext, "routesToUpdate").On();
            joinCondition.EqualsCondition("routesToUpdate", COL_RSSC, "tempRoutes", COL_RSSC);
            joinCondition.EqualsCondition("routesToUpdate", COL_Code, "tempRoutes", COL_Code);
            joinCondition.EqualsCondition("routesToUpdate", COL_DNSet, "tempRoutes", COL_DNSet);

            queryContext.ExecuteNonQuery();
        }

        public void Finalize(IRouteFinalizeContext context)
        {
            SwapTables(RouteTableName, RouteTempTableName);
            DeleteRouteTable(RouteAddedTableName);
            DeleteRouteTable(RouteUpdatedTableName);
            DeleteRouteTable(RouteDeletedTableName);
        }

        #endregion

        #region Private Methods
        private RDBTempTableQuery CreateRouteTempTableContextForJoin(IEnumerable<HuaweiConvertedRoute> routes, string RouteTempTABLE_NAME, RDBQueryContext queryContext)
        {
            var tempTableContext = queryContext.CreateTempTable();
            tempTableContext.AddColumnsFromTable(RouteTempTABLE_NAME);

            var insertMultipleRowsQuery = queryContext.AddInsertMultipleRowsQuery();
            insertMultipleRowsQuery.IntoTable(tempTableContext);

            foreach (var route in routes)
            {
                var rowContext = insertMultipleRowsQuery.AddRow();

                rowContext.Column(COL_RSSC).Value(route.RSSC);
                rowContext.Column(COL_Code).Value(route.Code);
                rowContext.Column(COL_RouteCaseId).Value(route.RouteCaseId);
                rowContext.Column(COL_DNSet).Value(route.DNSet);
                rowContext.Column(COL_MINL).Value(route.MinL);
                rowContext.Column(COL_MAXL).Value(route.MaxL);
                rowContext.Column(COL_CC).Value(route.CC);
            }

            return tempTableContext;
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

        #endregion

        #region Private Classes

        private class HuaweiConvertedRouteByCompare
        {
            public HuaweiConvertedRoute HuaweiConvertedRoute { get; set; }

            public string TableName { get; set; }
        }

        #endregion

        #endregion

        #region Common Private Methods

        private void DeleteRouteTable(string tableName)
        {
            TryRegisterTable(tableName);
            var TABLE_NAME = TableNames[tableName];

            var queryContext = new RDBQueryContext(GetDataProvider());
            queryContext.AddDropTableQuery().TableName(TABLE_NAME);

            queryContext.ExecuteNonQuery();
        }

        private void TryRegisterTable(string tableName)
        {
            var TABLE_NAME = $"{TABLE_Schema}.[{tableName}]";
            if (TableNames == null)
                TableNames = new ConcurrentDictionary<string, string>();

            TableNames.TryAdd(tableName, TABLE_NAME);

            RDBSchemaManager.Current.TryRegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = TABLE_Schema,
                DBTableName = tableName,
                Columns = RDBTableColumnDefinitionDictionary
            });
        }

        #endregion

        #region Mappers

        private HuaweiConvertedRoute HuaweiConvertedRouteMapper(IRDBDataReader reader)
        {
            return new HuaweiConvertedRoute()
            {
                DNSet = reader.GetInt(COL_DNSet),
                Code = reader.GetString(COL_Code),
                RouteCaseId = reader.GetLong(COL_RouteCaseId),
                RSSC = reader.GetInt(COL_RSSC),
                MinL = reader.GetInt(COL_MINL),
                MaxL = reader.GetInt(COL_MAXL),
                CC = reader.GetInt(COL_CC)
            };
        }

        #endregion
    }
}
