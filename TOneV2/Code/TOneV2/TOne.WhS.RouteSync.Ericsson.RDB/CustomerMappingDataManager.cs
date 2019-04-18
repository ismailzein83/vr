using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.RouteSync.Ericsson;
using TOne.WhS.RouteSync.Ericsson.Data;
using Vanrise.Common;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace TOne.WhS.RouteSync.Ericsson.RDB
{
    public class CustomerMappingDataManager : ICustomerMappingDataManager
    {

        #region Properties/Ctor

        string TABLE_Schema;

        private ConcurrentDictionary<string, string> TableNames;

        const string CustomerMappingTableName = "CustomerMapping";
        const string CustomerMappingTempTableName = "CustomerMapping_temp";
        const string CustomerMappingSucceededTableName = "CustomerMapping_Succeeded";

        const string COL_BO = "BO";
        const string COL_CustomerMapping = "CustomerMapping";
        const string COL_Action = "Action";

        public string SwitchId { get; set; }

        #endregion

        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("RouteSync", "TOneWhS_RouteSync_DBConnStringKey", "RouteSyncDBConnString");
        }

        public void Initialize(ICustomerMappingInitializeContext context)
        {
            Guid guid = Guid.NewGuid();

            CreateCustomerMappingTempTableQuery(guid);

            CreateCustomerMappingTableQuery(guid);

            SyncWithCustomerMappingSucceededTable();

            CreateSucceededCustomerMappingTableQuery(guid);
        }

        public new void Finalize(ICustomerMappingFinalizeContext context)
        {
            TryRegisterCommonTable(CustomerMappingTableName);
            var CustomerMapping_TABLE_NAME = TableNames[CustomerMappingTableName];

            TryRegisterCommonTable(CustomerMappingTempTableName);
            var CustomerMappingTemp_TABLE_NAME = TableNames[CustomerMappingTempTableName];

            #region Swap Tables

            var swapTablesQueryContext = new RDBQueryContext(GetDataProvider());
            swapTablesQueryContext.AddSwapTablesQuery().TableNames(CustomerMapping_TABLE_NAME, CustomerMappingTemp_TABLE_NAME, true);
            swapTablesQueryContext.ExecuteNonQuery();

            #endregion

            #region Drop Customer Mapping Table

            TryRegisterCommonTable(CustomerMappingSucceededTableName);
            DropTable(TableNames[CustomerMappingSucceededTableName]);

            #endregion
        }

        public void CompareTables(ICustomerMappingTablesContext context)
        {
            var differences = new Dictionary<string, List<CustomerMappingByCompare>>();

            TryRegisterCommonTable(CustomerMappingTableName);
            var CustomerMapping_TABLE_NAME = TableNames[CustomerMappingTableName];

            TryRegisterCommonTable(CustomerMappingTempTableName);
            var CustomerMappingTemp_TABLE_NAME = TableNames[CustomerMappingTempTableName];

            var checkIfTableExistsQueryContext = new RDBQueryContext(GetDataProvider());

            var checkIfTableExistsQuery = checkIfTableExistsQueryContext.AddCheckIfTableExistsQuery();
            checkIfTableExistsQuery.TableName(CustomerMapping_TABLE_NAME);


            var queryContext = new RDBQueryContext(GetDataProvider());
            var tableNameColumn = "tableName";
            AddCompareTablesQuery(CustomerMapping_TABLE_NAME, CustomerMappingTemp_TABLE_NAME, checkIfTableExistsQueryContext, queryContext, tableNameColumn);

            queryContext.ExecuteReader(reader =>
            {
                while (reader.Read())
                {
                    var customerMappingByCompare = new CustomerMappingByCompare() { CustomerMapping = CustomerMappingMapper(reader), TableName = reader.GetString(tableNameColumn) };
                    List<CustomerMappingByCompare> tempCustomerMappingDifferences = differences.GetOrCreateItem(customerMappingByCompare.CustomerMapping.BO);
                    tempCustomerMappingDifferences.Add(customerMappingByCompare);
                }
            });

            if (differences.Count > 0)
            {
                List<CustomerMappingSerialized> customerMappingsToAdd = new List<CustomerMappingSerialized>();
                List<CustomerMappingSerialized> customerMappingsToUpdate = new List<CustomerMappingSerialized>();
                List<CustomerMappingSerialized> customerMappingsToDelete = new List<CustomerMappingSerialized>();
                foreach (var differenceKvp in differences)
                {
                    var customerMappingDifferences = differenceKvp.Value;
                    if (customerMappingDifferences.Count == 1)
                    {
                        var singleCustomerMappingDifference = differenceKvp.Value[0];
                        if (singleCustomerMappingDifference.TableName == CustomerMappingTableName)
                            customerMappingsToDelete.Add(singleCustomerMappingDifference.CustomerMapping);
                        else
                            customerMappingsToAdd.Add(singleCustomerMappingDifference.CustomerMapping);
                    }
                    else
                    {
                        var customerMapping = customerMappingDifferences.FindRecord(item => (string.Compare(item.TableName, CustomerMappingTempTableName, true) == 0));
                        var customerMappingOldValue = customerMappingDifferences.FindRecord(item => (string.Compare(item.TableName, CustomerMappingTableName, true) == 0));
                        var customerMappingToUpdate = new CustomerMappingSerialized();
                        if (customerMapping != null)
                        {
                            customerMappingToUpdate.BO = customerMapping.CustomerMapping.BO;
                            customerMappingToUpdate.CustomerMappingAsString = customerMapping.CustomerMapping.CustomerMappingAsString;
                            if (customerMappingOldValue != null)
                                customerMappingToUpdate.CustomerMappingOldValueAsString = customerMappingOldValue.CustomerMapping.CustomerMappingAsString;

                            customerMappingsToUpdate.Add(customerMappingToUpdate);
                        }
                    }
                }

                if (customerMappingsToAdd.Count > 0)
                    context.CustomerMappingsToAdd = customerMappingsToAdd;

                if (customerMappingsToUpdate.Count > 0)
                    context.CustomerMappingsToUpdate = customerMappingsToUpdate;

                if (customerMappingsToDelete.Count > 0)
                    context.CustomerMappingsToDelete = customerMappingsToDelete;
            }
        }

        private static void AddCompareTablesQuery(string CustomerMapping_TABLE_NAME, string CustomerMappingTemp_TABLE_NAME, RDBQueryContext checkIfTableExistsQueryContext, RDBQueryContext queryContext, string tableNameColumn)
        {
            if (checkIfTableExistsQueryContext.ExecuteScalar().IntValue > 0)
            {
                var selectQuery = queryContext.AddSelectQuery();
                selectQuery.SelectColumns().Columns(COL_BO, COL_CustomerMapping);
                selectQuery.SelectAggregates().Aggregate(RDBNonCountAggregateType.MAX, tableNameColumn);

                var unionSelect = selectQuery.FromSelectUnion("v");

                var firstSelect = unionSelect.AddSelect();
                firstSelect.From(CustomerMapping_TABLE_NAME, CustomerMappingTableName);
                firstSelect.SelectColumns().AllTableColumns(CustomerMappingTableName);
                firstSelect.SelectColumns().Expression(tableNameColumn).Value(CustomerMappingTableName);

                var secondSelect = unionSelect.AddSelect();
                secondSelect.From(CustomerMappingTemp_TABLE_NAME, CustomerMappingTempTableName);
                secondSelect.SelectColumns().AllTableColumns(CustomerMappingTempTableName);
                secondSelect.SelectColumns().Expression(tableNameColumn).Value(CustomerMappingTempTableName);

                var groupByContext = selectQuery.GroupBy();
                groupByContext.Select().Columns(COL_BO, COL_CustomerMapping);
                groupByContext.Having().CompareCount(RDBCompareConditionOperator.Eq, 1);
            }
            else
            {
                var selectQuery = queryContext.AddSelectQuery();

                selectQuery.From(CustomerMappingTemp_TABLE_NAME, CustomerMappingTempTableName);
                selectQuery.SelectColumns().AllTableColumns(CustomerMappingTempTableName);
                selectQuery.SelectColumns().Expression(tableNameColumn).Value(CustomerMappingTempTableName);
            }
        }

        #region handleFailedRecords

        public void RemoveCutomerMappingsFromTempTable(IEnumerable<string> failedRecordsBO)
        {
            if (failedRecordsBO == null || !failedRecordsBO.Any())
                return;

            RemoveFailedRecords(failedRecordsBO, CustomerMappingTableName);
        }

        public void UpdateCustomerMappingsInTempTable(IEnumerable<CustomerMapping> customerMappingsToUpdate)
        {
            if (customerMappingsToUpdate == null || !customerMappingsToUpdate.Any())
                return;

            TryRegisterCommonTable(CustomerMappingTableName);
            var CustomerMapping_TABLE_NAME = TableNames[CustomerMappingTableName];

            var queryContext = new RDBQueryContext(GetDataProvider());

            var tempTableContext = CreateCustomerMappingTempTableForJoin(customerMappingsToUpdate, CustomerMapping_TABLE_NAME, queryContext);

            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(CustomerMapping_TABLE_NAME);
            updateQuery.Column(COL_BO).Column("updatedCustomerMappings", COL_BO);
            updateQuery.Column(COL_CustomerMapping).Column("updatedCustomerMappings", COL_CustomerMapping);

            var joinStatement = updateQuery.Join(CustomerMappingTableName);

            var joinCondtition = joinStatement.Join(tempTableContext, "updatedCustomerMappings").On();
            joinCondtition.EqualsCondition("updatedCustomerMappings", COL_BO, CustomerMappingTableName, COL_BO);

            queryContext.ExecuteNonQuery();
        }

        #endregion

        #region BCP
        public void InsertCutomerMappingsToTempTable(IEnumerable<CustomerMapping> customerMappings)
        {
            if (customerMappings != null && customerMappings.Any())
            {
                object dbApplyStream = InitialiazeStreamForDBApply();
                foreach (var customerMapping in customerMappings)
                {
                    CustomerMappingSerialized customerMappingSerialized = new CustomerMappingSerialized() { BO = customerMapping.BO, CustomerMappingAsString = Helper.SerializeCustomerMapping(customerMapping) };
                    WriteRecordToStream(customerMappingSerialized, dbApplyStream);
                }
                object obj = FinishDBApplyStream(dbApplyStream);
                ApplyCustomerMappingForDB(obj);
            }
        }

        public object InitialiazeStreamForDBApply()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            TryRegisterCommonTable(CustomerMappingTempTableName);
           var  CustomerMappingTempTable_TABLE_NAME = TableNames[CustomerMappingTempTableName];
            var streamForBulkInsert = queryContext.StartBulkInsert();
            streamForBulkInsert.IntoTable(CustomerMappingTempTable_TABLE_NAME, '^', COL_BO, COL_CustomerMapping);

            return streamForBulkInsert;
        }
        public void WriteRecordToStream(CustomerMappingSerialized record, object dbApplyStream)
        {
            RDBBulkInsertQueryContext bulkInsertContext = dbApplyStream.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream");
            var recordContext = bulkInsertContext.WriteRecord();

            if (record.BO != null)
                recordContext.Value(record.BO);
            else
                recordContext.Value(string.Empty);

            recordContext.Value(record.CustomerMappingAsString);
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            RDBBulkInsertQueryContext bulkInsertContext = dbApplyStream.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream");
            bulkInsertContext.CloseStream();
            return bulkInsertContext;
        }

        public void ApplyCustomerMappingForDB(object preparedCustomerMapping)
        {
            preparedCustomerMapping.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream").Apply();
        }

        #endregion

        #region Private Methods

        private void RemoveFailedRecords(IEnumerable<string> failedRecordsBO, string CustomerMappingTableName)
        {
            TryRegisterCommonTable(CustomerMappingTableName);
            var CustomerMapping_TABLE_NAME = TableNames[CustomerMappingTableName];

            var queryContext = new RDBQueryContext(GetDataProvider());

            var deleteQuery = queryContext.AddDeleteQuery();
            deleteQuery.FromTable(CustomerMappingTableName);
            deleteQuery.Where().ListCondition(COL_BO, RDBListConditionOperator.IN, failedRecordsBO);

            queryContext.ExecuteNonQuery();

        }
        private void CreateCustomerMappingTableQuery(Guid guid)
        {
            TryRegisterCommonTable(CustomerMappingTableName);
            var TABLE_NAME = TableNames[CustomerMappingTableName];

            var checkIfTableExistsQueryContext = new RDBQueryContext(GetDataProvider());

            var checkIfTableExistsQuery = checkIfTableExistsQueryContext.AddCheckIfTableExistsQuery();
            checkIfTableExistsQuery.TableName(TABLE_NAME);

            if (checkIfTableExistsQueryContext.ExecuteScalar().IntValue > 0)
                return;
            CreateCommonCustomerMappingQuery(guid, CustomerMappingTableName);
        }

        private void CreateCustomerMappingTempTableQuery(Guid guid)
        {
            TryRegisterCommonTable(CustomerMappingTempTableName);
            var TABLE_NAME = TableNames[CustomerMappingTempTableName];

            var dropTableQueryContext = new RDBQueryContext(GetDataProvider());

            var dropTableQuery = dropTableQueryContext.AddDropTableQuery();
            dropTableQuery.TableName(TABLE_NAME);

            dropTableQueryContext.ExecuteNonQuery();

            CreateCommonCustomerMappingQuery(guid, CustomerMappingTempTableName);
        }

        private void CreateCommonCustomerMappingQuery(Guid guid, string TABLE_NAME)
        {
            var createTableQueryContext = new RDBQueryContext(GetDataProvider());

            var createTableQuery = createTableQueryContext.AddCreateTableQuery();

            createTableQuery.DBTableName(TABLE_Schema, TABLE_NAME);

            createTableQuery.AddColumn(COL_BO, RDBDataType.NVarchar, 255, null, true);
            createTableQuery.AddColumn(COL_CustomerMapping, RDBDataType.Varchar, null, null, true);

            var createIndexQuery = createTableQueryContext.AddCreateIndexQuery();
            createIndexQuery.DBTableName(TABLE_Schema, TABLE_NAME);
            createIndexQuery.IndexName($"[PK_{TABLE_Schema}{SwitchId}.{TABLE_NAME}{guid}]");
            createIndexQuery.IndexType(RDBCreateIndexType.UniqueClustered);
            createIndexQuery.AddColumn(COL_BO);

            createTableQueryContext.ExecuteNonQuery();
        }

        private void CreateSucceededCustomerMappingTableQuery(Guid guid)
        {
            var createTableQueryContext = new RDBQueryContext(GetDataProvider());

            var createTableQuery = createTableQueryContext.AddCreateTableQuery();

            createTableQuery.DBTableName(TABLE_Schema, CustomerMappingSucceededTableName);

            createTableQuery.AddColumn(COL_BO, RDBDataType.NVarchar, 255, null, true);
            createTableQuery.AddColumn(COL_CustomerMapping, RDBDataType.Varchar, null, null, true);
            createTableQuery.AddColumn(COL_Action, RDBDataType.Int, null, null, true);

            var createIndexQuery = createTableQueryContext.AddCreateIndexQuery();
            createIndexQuery.DBTableName(TABLE_Schema, CustomerMappingSucceededTableName);
            createIndexQuery.IndexName($"[PK_{TABLE_Schema}{SwitchId}.{CustomerMappingSucceededTableName}{guid}]");
            createIndexQuery.IndexType(RDBCreateIndexType.UniqueClustered);
            createIndexQuery.AddColumn(COL_BO);

            createTableQueryContext.ExecuteNonQuery();
        }

        private void DropTable(string TableName)
        {
            var dropTableQueryContext = new RDBQueryContext(GetDataProvider());

            var dropTableQuery = dropTableQueryContext.AddDropTableQuery();
            dropTableQuery.TableName(TableName);

            dropTableQueryContext.ExecuteNonQuery();
        }

        private void SyncWithCustomerMappingSucceededTable()
        {
            TryRegisterSucceededTable(CustomerMappingSucceededTableName);
            var Succeeded_TABLE_NAME = TableNames[CustomerMappingSucceededTableName];

            var checkIfTableExistsQueryContext = new RDBQueryContext(GetDataProvider());

            var checkIfTableExistsQuery = checkIfTableExistsQueryContext.AddCheckIfTableExistsQuery();
            checkIfTableExistsQuery.TableName(Succeeded_TABLE_NAME);

            if (checkIfTableExistsQueryContext.ExecuteScalar().IntValue <= 0)
                return;

            TryRegisterCommonTable(CustomerMappingTableName);
            var CustomerMapping_TABLE_NAME = TableNames[CustomerMappingTableName];
            var CustomerMapping_TABLE_ALIAS = "cm";
            var Succeeded_TABLE_ALIAS = "cms";

            var queryContext = new RDBQueryContext(GetDataProvider());

            #region  DeleteQuery

            var deleteQuery = queryContext.AddDeleteQuery();
            deleteQuery.FromTable(CustomerMapping_TABLE_NAME);

            var deleteJoinStatement = deleteQuery.Join(CustomerMapping_TABLE_ALIAS);
            var deleteJoinContext = deleteJoinStatement.Join(Succeeded_TABLE_NAME, Succeeded_TABLE_ALIAS);
            deleteJoinContext.JoinType(RDBJoinType.Inner);
            var deleteJoinCondition = deleteJoinContext.On();
            deleteJoinCondition.EqualsCondition(Succeeded_TABLE_ALIAS, COL_BO, CustomerMapping_TABLE_ALIAS, COL_BO);

            deleteQuery.Where().EqualsCondition(Succeeded_TABLE_ALIAS, COL_Action).Value((int)CustomerMappingActionType.Delete);

            #endregion

            #region Update Query

            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(CustomerMapping_TABLE_NAME);
            updateQuery.Column(COL_CustomerMapping).Column(Succeeded_TABLE_ALIAS, COL_CustomerMapping);

            var updateJoinStatement = updateQuery.Join(CustomerMapping_TABLE_ALIAS);
            var updateJoinContext = updateJoinStatement.Join(Succeeded_TABLE_NAME, Succeeded_TABLE_ALIAS);
            updateJoinContext.JoinType(RDBJoinType.Inner);
            var updateJoinCondition = updateJoinContext.On();
            updateJoinCondition.EqualsCondition(Succeeded_TABLE_ALIAS, COL_BO, CustomerMapping_TABLE_ALIAS, COL_BO);
            updateJoinCondition.EqualsCondition(Succeeded_TABLE_ALIAS, COL_Action).Value((int)CustomerMappingActionType.Update);

            #endregion

            #region Insert Table Query

            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(CustomerMapping_TABLE_NAME);

            var fromSelect = insertQuery.FromSelect();
            fromSelect.From(Succeeded_TABLE_NAME, Succeeded_TABLE_ALIAS);
            fromSelect.SelectColumns().Columns(COL_BO, COL_CustomerMapping);
            fromSelect.Where().EqualsCondition(Succeeded_TABLE_ALIAS, COL_Action).Value((int)CustomerMappingActionType.Add);

            #endregion

            #region Drop Table Query

            var dropTableQuery = queryContext.AddDropTableQuery();
            dropTableQuery.TableName(Succeeded_TABLE_NAME);

            #endregion

            queryContext.ExecuteNonQuery(true);
        }

        private void TryRegisterCommonTable(string TableName)
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
                Columns = GetCommonTableColumnDefinitionDictionary()
            });
        }

        private Dictionary<string, RDBTableColumnDefinition> GetCommonTableColumnDefinitionDictionary()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_BO, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
            columns.Add(COL_CustomerMapping, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar });
            return columns;
        }

        private void TryRegisterSucceededTable(string TableName)
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
                Columns = GetSucceededTableColumnDefinitionDictionary()
            });
        }

        private Dictionary<string, RDBTableColumnDefinition> GetSucceededTableColumnDefinitionDictionary()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_BO, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
            columns.Add(COL_CustomerMapping, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar });
            columns.Add(COL_Action, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            return columns;
        }

        private static RDBTempTableQuery CreateCustomerMappingTempTableForJoin(IEnumerable<CustomerMapping> customerMapping, string tableNameToGetColumnsFrom, RDBQueryContext queryContext)
        {
            var tempTableContext = queryContext.CreateTempTable();
            tempTableContext.AddColumnsFromTable(tableNameToGetColumnsFrom);

            var insertMultipleRowsQuery = queryContext.AddInsertMultipleRowsQuery();
            insertMultipleRowsQuery.IntoTable(tempTableContext);

            foreach (var cm in customerMapping)
            {
                var rowContext = insertMultipleRowsQuery.AddRow();

                rowContext.Column(COL_BO).Value(cm.BO);
                rowContext.Column(COL_CustomerMapping).Value(Helper.SerializeCustomerMapping(cm));
            }

            return tempTableContext;
        }

        #endregion

        #region Mappers
        CustomerMappingSerialized CustomerMappingMapper(IRDBDataReader reader)
        {
            return new CustomerMappingSerialized()
            {
                BO = reader.GetString(COL_BO),
                CustomerMappingAsString = reader.GetString(COL_CustomerMapping)
            };
        }
        #endregion

    }
}
