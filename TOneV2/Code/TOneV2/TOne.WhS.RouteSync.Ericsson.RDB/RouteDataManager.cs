using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.RouteSync.Ericsson.Data;
using Vanrise.Common;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace TOne.WhS.RouteSync.Ericsson.RDB
{
    class RouteDataManager : IRouteDataManager
    {
        #region Properties/Ctor

        string TABLE_Schema;

        private ConcurrentDictionary<string, string> TableNames;

        const string RouteTableName = "Route";
        const string RouteTempTableName = "Route_temp";
        const string RouteAddedTableName = "Route_Added";
        const string RouteUpdatedTableName = "Route_Updated";
        const string RouteDeletedTableName = "Route_Deleted";

        const string COL_OBA = "OBA";
        const string COL_Code = "Code";
        const string COL_TRD = "RCNumber";
        const string COL_RCNumber = "RCNumber";
        const string COL_RouteType = "RouteType";

        public string SwitchId { get; set; }

        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("RouteSync", "TOneWhS_RouteSync_DBConnStringKey", "RouteSyncDBConnString");
        }

        #endregion

        #region Public Methods

        public void Initialize(IRouteInitializeContext context)
        {
            Guid guid = Guid.NewGuid();

            CreateRouteTempTableQuery(guid);

            CreateRouteTableQuery(guid);

            #region Deleted

            SyncWithRouteDeletedTable();
            CreateCommonRouteTableQuery(guid, RouteDeletedTableName);

            #endregion

            #region Updated

            SyncWithRouteUpdatedTableQuery();
            CreateCommonRouteTableQuery(guid, RouteUpdatedTableName);

            #endregion

            #region Added

            SyncWithRouteAddedTable();
            CreateCommonRouteTableQuery(guid, RouteAddedTableName);

            #endregion
        }

        public new void Finalize(IRouteFinalizeContext context)
        {
            SwapTables(RouteTableName, RouteTempTableName);

            DeleteRouteTable(RouteAddedTableName);
            DeleteRouteTable(RouteUpdatedTableName);
            DeleteRouteTable(RouteDeletedTableName);
        }

        public void CompareTables(IRouteCompareTablesContext context)
        {
            var differencesByOBA = new Dictionary<int, EricssonConvertedRouteDifferences>();
            var differences = new Dictionary<EricssonConvertedRouteIdentifier, List<EricssonConvertedRouteByCompare>>();


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
                selectQuery.SelectColumns().Columns(COL_OBA, COL_Code, COL_RCNumber, COL_TRD, COL_RouteType);
                selectQuery.SelectAggregates().Aggregate(RDBNonCountAggregateType.MAX, "tableName");

                var unionSelect = selectQuery.FromSelectUnion("v");

                var firstSelect = unionSelect.AddSelect();
                firstSelect.From(Route_TABLE_NAME, RouteTableName);
                firstSelect.SelectColumns().Columns(COL_OBA, COL_Code, COL_RCNumber, COL_TRD, COL_RouteType);
                firstSelect.SelectColumns().Expression("tableName").Value(RouteTableName);

                var secondSelect = unionSelect.AddSelect();
                secondSelect.From(RouteTemp_TABLE_NAME, RouteTempTableName);
                secondSelect.SelectColumns().AllTableColumns(RouteTempTableName);
                secondSelect.SelectColumns().Expression("tableName").Value(RouteTempTableName);


                var groupByContext = selectQuery.GroupBy();
                groupByContext.Select().Columns(COL_OBA, COL_Code, COL_RCNumber, COL_TRD, COL_RouteType);
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
                    var convertedRouteByCompare = new EricssonConvertedRouteByCompare() { EricssonConvertedRoute = EricssonConvertedRouteMapper(reader), TableName = reader.GetString("tableName") };
                    var routeIdentifier = new EricssonConvertedRouteIdentifier() { OBA = convertedRouteByCompare.EricssonConvertedRoute.OBA, Code = convertedRouteByCompare.EricssonConvertedRoute.Code, RouteType = convertedRouteByCompare.EricssonConvertedRoute.RouteType, TRD = convertedRouteByCompare.EricssonConvertedRoute.TRD };
                    List<EricssonConvertedRouteByCompare> tempRouteDifferences = differences.GetOrCreateItem(routeIdentifier);
                    tempRouteDifferences.Add(convertedRouteByCompare);
                }
            });

            if (differences.Count > 0)
            {
                foreach (var differenceKvp in differences)
                {
                    var routeDifferences = differenceKvp.Value;
                    var difference = differencesByOBA.GetOrCreateItem(differenceKvp.Key.OBA);
                    if (routeDifferences.Count == 1)
                    {
                        var singleRouteDifference = differenceKvp.Value[0];
                        if (singleRouteDifference.TableName == RouteTableName)
                            difference.RoutesToDelete.Add(new EricssonConvertedRouteCompareResult() { Route = singleRouteDifference.EricssonConvertedRoute });

                        else
                            difference.RoutesToAdd.Add(new EricssonConvertedRouteCompareResult() { Route = singleRouteDifference.EricssonConvertedRoute });
                    }
                    else //routeDifferences.Count = 2
                    {
                        var route = routeDifferences.FindRecord(item => (string.Compare(item.TableName, RouteTempTableName, true) == 0));
                        var routeOldValue = routeDifferences.FindRecord(item => (string.Compare(item.TableName, RouteTableName, true) == 0));

                        if (route != null)
                        {
                            difference.RoutesToUpdate.Add(new EricssonConvertedRouteCompareResult()
                            {
                                Route = new EricssonConvertedRoute()
                                {
                                    OBA = route.EricssonConvertedRoute.OBA,
                                    Code = route.EricssonConvertedRoute.Code,
                                    RCNumber = route.EricssonConvertedRoute.RCNumber,
                                    TRD = route.EricssonConvertedRoute.TRD,
                                    RouteType = route.EricssonConvertedRoute.RouteType
                                },
                                OriginalValue = (routeOldValue != null) ? new EricssonConvertedRoute()
                                {
                                    OBA = routeOldValue.EricssonConvertedRoute.OBA,
                                    Code = routeOldValue.EricssonConvertedRoute.Code,
                                    RCNumber = routeOldValue.EricssonConvertedRoute.RCNumber,
                                    TRD = route.EricssonConvertedRoute.TRD,
                                    RouteType = routeOldValue.EricssonConvertedRoute.RouteType
                                } : null
                            });
                        }
                    }
                }
                context.RouteDifferencesByOBA = differencesByOBA;
            }
        }

        public void CopyCustomerRoutesToTempTable(IEnumerable<int> customerOBAs)
        {
            if (customerOBAs == null || !customerOBAs.Any())
                return;

            TryRegisterTable(RouteTableName);
            var Route_TABLE_NAME = TableNames[RouteTableName];

            TryRegisterTable(RouteTempTableName);
            var RouteTemp_TABLE_NAME = TableNames[RouteTempTableName];

            var queryContext = new RDBQueryContext(GetDataProvider());
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(RouteTemp_TABLE_NAME);

            var fromSelectQuery = insertQuery.FromSelect();
            fromSelectQuery.From(Route_TABLE_NAME, RouteTableName);
            fromSelectQuery.SelectColumns().AllTableColumns(RouteTableName);

            var whereContext = fromSelectQuery.Where();
            whereContext.ListCondition(COL_OBA, RDBListConditionOperator.IN, customerOBAs);

            queryContext.ExecuteNonQuery();
        }

        public Dictionary<int, List<EricssonConvertedRoute>> GetFilteredConvertedRouteByOBA(IEnumerable<int> customerOBAs)
        {
            TryRegisterTable(RouteTempTableName);
            var RouteTemp_TABLE_NAME = TableNames[RouteTempTableName];

            var convertedRoutesByOBA = new Dictionary<int, List<EricssonConvertedRoute>>();

            var queryContext = new RDBQueryContext(GetDataProvider());

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(RouteTemp_TABLE_NAME, RouteTempTableName);
            selectQuery.SelectColumns().AllTableColumns(RouteTempTableName);

            var whereContext = selectQuery.Where();

            if (customerOBAs != null && customerOBAs.Any())
                whereContext.ListCondition(COL_OBA, RDBListConditionOperator.IN, customerOBAs);

            queryContext.ExecuteReader(reader =>
           {
               while (reader.Read())
               {
                   var convertedRoute = EricssonConvertedRouteMapper(reader);
                   List<EricssonConvertedRoute> convertedRoutes = convertedRoutesByOBA.GetOrCreateItem(convertedRoute.OBA);
                   convertedRoutes.Add(convertedRoute);
               }
           });

            return convertedRoutesByOBA;
        }

        public void InsertRoutesToTempTable(IEnumerable<EricssonConvertedRoute> routes)
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

        public void RemoveRoutesFromTempTable(IEnumerable<EricssonConvertedRoute> routes)
        {
            if (routes == null || !routes.Any())
                return;

            TryRegisterTable(RouteTempTableName);
            var RouteTempTABLE_NAME = TableNames[RouteTempTableName];
            string routeTableAlias = "route";
            string tempTableAlias = "tempRoute";


            var queryContext = new RDBQueryContext(GetDataProvider());

            var tempTableContext = CreateRouteTempTableForJoin(routes, RouteTempTABLE_NAME, queryContext);

            var deleteQuery = queryContext.AddDeleteQuery();
            deleteQuery.FromTable(RouteTempTABLE_NAME);

            var joinStatement = deleteQuery.Join("tempRoute");

            var joinCondition = joinStatement.Join(tempTableContext, "route").On();
            joinCondition.EqualsCondition(routeTableAlias, COL_OBA, tempTableAlias, COL_OBA);
            joinCondition.EqualsCondition(routeTableAlias, COL_Code, tempTableAlias, COL_Code);
            joinCondition.EqualsCondition(routeTableAlias, COL_RCNumber, tempTableAlias, COL_RCNumber);
            joinCondition.EqualsCondition(routeTableAlias, COL_TRD, tempTableAlias, COL_TRD);
            joinCondition.EqualsCondition(routeTableAlias, COL_RouteType, tempTableAlias, COL_RouteType);

            queryContext.ExecuteNonQuery();
        }

        public void UpdateRoutesInTempTable(IEnumerable<EricssonConvertedRoute> routes)
        {
            if (routes == null || !routes.Any())
                return;

            TryRegisterTable(RouteTempTableName);
            var RouteTempTABLE_NAME = TableNames[RouteTempTableName];

            string tempRouteTableAlias = "tempRoutes";
            string tempTableAlias = "routesToUpdate";

            var queryContext = new RDBQueryContext(GetDataProvider());

            var tempTableContext = CreateRouteTempTableForJoin(routes, RouteTempTABLE_NAME, queryContext);

            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(RouteTempTABLE_NAME);
            updateQuery.Column(COL_RCNumber).Column(tempTableAlias, COL_RCNumber);

            var joinStatement = updateQuery.Join(tempRouteTableAlias);

            var joinCondition = joinStatement.Join(tempTableContext, tempTableAlias).On();
            joinCondition.EqualsCondition(tempTableAlias, COL_OBA, tempRouteTableAlias, COL_OBA);
            joinCondition.EqualsCondition(tempTableAlias, COL_Code, tempRouteTableAlias, COL_Code);
            joinCondition.EqualsCondition(tempTableAlias, COL_RouteType, tempRouteTableAlias, COL_RouteType);

            queryContext.ExecuteNonQuery();
        }

        #region IBulkApplyDataManager

        public object InitialiazeStreamForDBApply()
        {
            TryRegisterTable(RouteTempTableName);
            var RouteTemp_TABLE_NAME = TableNames[RouteTempTableName];

            var queryContext = new RDBQueryContext(GetDataProvider());
            var streamForBulkInsert = queryContext.StartBulkInsert();
            streamForBulkInsert.IntoTable(RouteTemp_TABLE_NAME, '^', COL_OBA, COL_Code, COL_RCNumber, COL_TRD, COL_RouteType);

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

            recordContext.Value(record.TRD);

            recordContext.Value(record.RCNumber);
            recordContext.Value((int)record.RouteType);
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

        #region Private Methods

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
            fromSelectQuery.SelectColumns().Columns(COL_OBA, COL_Code, COL_RCNumber, COL_TRD, COL_RouteType);

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

            string deletedTableAlias = "deletedRoutes";

            var queryContext = new RDBQueryContext(GetDataProvider());

            var deleteQuery = queryContext.AddDeleteQuery();
            deleteQuery.FromTable(TableNames[RouteTableName]);

            var joinStatement = deleteQuery.Join(RouteTableName);
            var joinContext = joinStatement.Join(RouteDeleted_TABLE_NAME, deletedTableAlias);
            joinContext.JoinType(RDBJoinType.Inner);
            var joinCondition = joinContext.On();
            joinCondition.EqualsCondition(deletedTableAlias, COL_OBA, RouteTableName, COL_OBA);
            joinCondition.EqualsCondition(deletedTableAlias, COL_Code, RouteTableName, COL_Code);
            joinCondition.EqualsCondition(deletedTableAlias, COL_RouteType, RouteTableName, COL_RouteType);

            queryContext.AddDropTableQuery().TableName(RouteDeleted_TABLE_NAME);

            queryContext.ExecuteNonQuery();
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

            string updatedTableAlias = "updatedRoutes";
            string routeTableAlias = "routes";

            TryRegisterTable(RouteTableName);
            var Route_TABLE_NAME = TableNames[RouteTableName];

            var queryContext = new RDBQueryContext(GetDataProvider());

            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TableNames[RouteTableName]);
            updateQuery.Column(COL_RCNumber).Column(updatedTableAlias, COL_RCNumber);

            var joinStatement = updateQuery.Join(routeTableAlias);
            var joinContext = joinStatement.Join(RouteUpdated_TABLE_NAME, updatedTableAlias);
            joinContext.JoinType(RDBJoinType.Inner);
            var joinCondition = joinContext.On();
            joinCondition.EqualsCondition(updatedTableAlias, COL_OBA, routeTableAlias, COL_OBA);
            joinCondition.EqualsCondition(updatedTableAlias, COL_Code, routeTableAlias, COL_Code);
            joinCondition.EqualsCondition(updatedTableAlias, COL_RouteType, routeTableAlias, COL_RouteType);

            queryContext.AddDropTableQuery().TableName(RouteUpdated_TABLE_NAME);

            queryContext.ExecuteNonQuery();
        }

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

            createTableQuery.AddColumn(COL_OBA, RDBDataType.NVarchar, 255, null, true);
            createTableQuery.AddColumn(COL_Code, RDBDataType.Varchar, 20, null, true);
            createTableQuery.AddColumn(COL_RCNumber, RDBDataType.Int, true);
            createTableQuery.AddColumn(COL_TRD, RDBDataType.Int, true);
            createTableQuery.AddColumn(COL_RouteType, RDBDataType.Int, true);


            var createIndexQuery = createTableQueryContext.AddCreateIndexQuery();
            createIndexQuery.DBTableName(TABLE_Schema, TABLE_NAME);
            createIndexQuery.IndexName($"[PK_{TABLE_Schema}{SwitchId}.{TABLE_NAME}{guid}]");
            createIndexQuery.IndexType(RDBCreateIndexType.UniqueClustered);
            createIndexQuery.AddColumn(COL_OBA);
            createIndexQuery.AddColumn(COL_Code);
            createIndexQuery.AddColumn(COL_RouteType);

            createTableQueryContext.ExecuteNonQuery();
        }

        private static RDBTempTableQuery CreateRouteTempTableForJoin(IEnumerable<EricssonConvertedRoute> routes, string RouteTempTABLE_NAME, RDBQueryContext queryContext)
        {
            var tempTableContext = queryContext.CreateTempTable();
            tempTableContext.AddColumnsFromTable(RouteTempTABLE_NAME);

            var insertMultipleRowsQuery = queryContext.AddInsertMultipleRowsQuery();
            insertMultipleRowsQuery.IntoTable(tempTableContext);

            foreach (var route in routes)
            {
                var rowContext = insertMultipleRowsQuery.AddRow();

                rowContext.Column(COL_OBA).Value(route.OBA);
                rowContext.Column(COL_Code).Value(route.Code);
                rowContext.Column(COL_RCNumber).Value(route.RCNumber);
                rowContext.Column(COL_RouteType).Value((int)route.RouteType);
            }

            return tempTableContext;
        }

        private void TryRegisterTable(string TableName)
        {

            TABLE_Schema = ($"WhS_RouteSync_Ericsson_{SwitchId}");
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
            columns.Add(COL_OBA, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
            columns.Add(COL_Code, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 20 });
            columns.Add(COL_RCNumber, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_RouteType, new RDBTableColumnDefinition { DataType = RDBDataType.Int });

            return columns;
        }

        #endregion

        #region Private Classes
        private class EricssonConvertedRouteByCompare
        {
            public EricssonConvertedRoute EricssonConvertedRoute { get; set; }
            public string TableName { get; set; }
        }

        #endregion

        #region Mappers

        EricssonConvertedRoute EricssonConvertedRouteMapper(IRDBDataReader reader)
        {
            return new EricssonConvertedRoute()
            {
                OBA = reader.GetInt(COL_OBA),
                Code = reader.GetString(COL_Code),
                RCNumber = reader.GetInt(COL_RCNumber),
                TRD = reader.GetInt(COL_TRD),
                RouteType = (EricssonRouteType)reader.GetInt(COL_RouteType)
            };
        }

        #endregion
    }
}
