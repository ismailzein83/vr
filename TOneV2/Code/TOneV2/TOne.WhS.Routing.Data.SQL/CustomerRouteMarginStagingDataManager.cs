using System;
using System.Collections.Generic;
using TOne.WhS.Routing.Entities;
using Vanrise.Common;
using Vanrise.Data.RDB;
using Vanrise.Data.SQL;
using Vanrise.Entities;

namespace TOne.WhS.Routing.Data.SQL
{
    public class CustomerRouteMarginStagingDataManager : BaseSQLDataManager, ICustomerRouteMarginStagingDataManager
    {
        #region Fields/Ctor

        private string tableName;

        const string DBTableSchema = "TOneWhS_Routing";

        const string CurrentStaging_DBTableName = "CustomerRouteMarginStaging_Current";
        const string FutureStaging_DBTableName = "CustomerRouteMarginStaging_Future";

        public const string CurrentStaging_TableName = "TOneWhS_Routing_CustomerRouteMarginStaging_Current";
        public const string FutureStaging_TableName = "TOneWhS_Routing_CustomerRouteMarginStaging_Future";

        public const string COL_CustomerID = "CustomerID";
        public const string COL_SaleZoneID = "SaleZoneID";
        public const string COL_SaleRate = "SaleRate";
        public const string COL_SaleDealID = "SaleDealID";
        public const string COL_Codes = "Codes";

        public const string COL_SupplierZoneID = "SupplierZoneID";
        public const string COL_SupplierServiceIDs = "SupplierServiceIDs";
        public const string COL_SupplierRate = "SupplierRate";
        public const string COL_SupplierDealID = "SupplierDealID";

        public const string COL_OptimalSupplierZoneID = "OptimalSupplierZoneID";
        public const string COL_OptimalSupplierServiceIDs = "OptimalSupplierServiceIDs";
        public const string COL_OptimalSupplierRate = "OptimalSupplierRate";
        public const string COL_OptimalSupplierDealID = "OptimalSupplierDealID";

        static CustomerRouteMarginStagingDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();

            columns.Add(COL_CustomerID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_SaleZoneID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_SaleRate, new RDBTableColumnDefinition { DataType = RDBDataType.Decimal, Size = 20, Precision = 8 });
            columns.Add(COL_SaleDealID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_Codes, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });

            columns.Add(COL_SupplierZoneID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_SupplierServiceIDs, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_SupplierRate, new RDBTableColumnDefinition { DataType = RDBDataType.Decimal, Size = 20, Precision = 8 });
            columns.Add(COL_SupplierDealID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });

            columns.Add(COL_OptimalSupplierZoneID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_OptimalSupplierServiceIDs, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_OptimalSupplierRate, new RDBTableColumnDefinition { DataType = RDBDataType.Decimal, Size = 20, Precision = 8 });
            columns.Add(COL_OptimalSupplierDealID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });

            RDBSchemaManager.Current.RegisterDefaultTableDefinition(CurrentStaging_TableName, new RDBTableDefinition
            {
                DBSchemaName = DBTableSchema,
                DBTableName = CurrentStaging_DBTableName,
                Columns = columns
            });

            RDBSchemaManager.Current.RegisterDefaultTableDefinition(FutureStaging_TableName, new RDBTableDefinition
            {
                DBSchemaName = DBTableSchema,
                DBTableName = FutureStaging_DBTableName,
                Columns = columns
            });
        }

        #endregion

        #region Public Methods

        public void CreateCustomerRouteMarginStagingTable(RoutingDatabaseType routingDatabaseType)
        {
            tableName = GetTableName(routingDatabaseType);
            string dbTableName = GetDBTableName(routingDatabaseType, true);

            var queryContext = new RDBQueryContext(this.GetDataProvider());

            var dropTableQuery = queryContext.AddDropTableQuery();
            dropTableQuery.TableName(tableName);

            var createTableQuery = queryContext.AddCreateTableQuery();
            createTableQuery.DBTableName(DBTableSchema, dbTableName);

            createTableQuery.AddColumn(COL_CustomerID, RDBDataType.Int, true);
            createTableQuery.AddColumn(COL_SaleZoneID, RDBDataType.BigInt, true);
            createTableQuery.AddColumn(COL_SaleRate, RDBDataType.Decimal, 20, 8, true);
            createTableQuery.AddColumn(COL_SaleDealID, RDBDataType.Int);
            createTableQuery.AddColumn(COL_Codes, RDBDataType.NVarchar, true);

            createTableQuery.AddColumn(COL_SupplierZoneID, RDBDataType.BigInt, true);
            createTableQuery.AddColumn(COL_SupplierServiceIDs, RDBDataType.NVarchar, true);
            createTableQuery.AddColumn(COL_SupplierRate, RDBDataType.Decimal, 20, 8, true);
            createTableQuery.AddColumn(COL_SupplierDealID, RDBDataType.Int);

            createTableQuery.AddColumn(COL_OptimalSupplierZoneID, RDBDataType.BigInt, true);
            createTableQuery.AddColumn(COL_OptimalSupplierServiceIDs, RDBDataType.NVarchar, true);
            createTableQuery.AddColumn(COL_OptimalSupplierRate, RDBDataType.Decimal, 20, 8, true);
            createTableQuery.AddColumn(COL_OptimalSupplierDealID, RDBDataType.Int);

            queryContext.ExecuteNonQuery();
        }

        public void InsertCustomerRouteMarginStagingListToDB(RoutingDatabaseType routingDatabaseType, List<CustomerRouteMarginStaging> customerRouteMarginStagingList)
        {
            if (customerRouteMarginStagingList == null || customerRouteMarginStagingList.Count == 0)
                return;

            tableName = GetTableName(routingDatabaseType);

            Object dbApplyAddStream = InitialiazeStreamForDBApply();
            foreach (var customerRouteMarginStaging in customerRouteMarginStagingList)
            {
                WriteRecordToStream(customerRouteMarginStaging, dbApplyAddStream);
            }
            Object preparedAddRoutes = FinishDBApplyStream(dbApplyAddStream);
            ApplyCustomerRouteMarginForDB(preparedAddRoutes);
        }

        public List<CustomerRouteMarginStaging> GetCustomerRouteMarginStagingList(RoutingDatabaseType routingDatabaseType)
        {
            tableName = GetTableName(routingDatabaseType);
            string tableAlias = "crms";

            var queryContext = new RDBQueryContext(GetDataProvider());

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(tableName, tableAlias, null, true);
            selectQuery.SelectColumns().AllTableColumns(tableAlias);
            return queryContext.GetItems<CustomerRouteMarginStaging>(CustomerRouteMarginStagingMapper);
        }

        public void DropTable(RoutingDatabaseType routingDatabaseType)
        {
            tableName = GetTableName(routingDatabaseType);

            var queryContext = new RDBQueryContext(this.GetDataProvider());
            var dropTableQuery = queryContext.AddDropTableQuery();
            dropTableQuery.TableName(tableName);

            queryContext.ExecuteNonQuery();
        }

        #endregion

        #region IBulkApplyDataManager

        public object InitialiazeStreamForDBApply()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var bulkInsertQueryContext = queryContext.StartBulkInsert();

            bulkInsertQueryContext.IntoTable(tableName, '^', COL_CustomerID, COL_SaleZoneID, COL_SaleRate, COL_SaleDealID, COL_Codes,
                COL_SupplierZoneID, COL_SupplierServiceIDs, COL_SupplierRate, COL_SupplierDealID,
                COL_OptimalSupplierZoneID, COL_OptimalSupplierServiceIDs, COL_OptimalSupplierRate, COL_OptimalSupplierDealID);

            return bulkInsertQueryContext;
        }

        public void WriteRecordToStream(CustomerRouteMarginStaging record, object dbApplyStream)
        {
            RDBBulkInsertQueryContext bulkInsertQueryContext = dbApplyStream.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream");

            string codes = string.Join(",", record.Codes);
            string serializedSupplierServiceIds = Vanrise.Common.Serializer.Serialize(record.CustomerRouteOptionMarginStaging.SupplierServiceIDs, true);
            string serializedOptimalSupplierServiceIds = Vanrise.Common.Serializer.Serialize(record.OptimalCustomerRouteOptionMarginStaging.SupplierServiceIDs, true);

            var recordContext = bulkInsertQueryContext.WriteRecord();
            recordContext.Value(record.CustomerID);
            recordContext.Value(record.SaleZoneID);
            recordContext.Value(record.SaleRate);

            if(record.SaleDealID.HasValue)
                recordContext.Value(record.SaleDealID.Value);
            else
                recordContext.Value(string.Empty);

            recordContext.Value(codes);

            recordContext.Value(record.CustomerRouteOptionMarginStaging.SupplierZoneID);
            recordContext.Value(serializedSupplierServiceIds);
            recordContext.Value(record.CustomerRouteOptionMarginStaging.SupplierRate);

            if (record.CustomerRouteOptionMarginStaging.SupplierDealID.HasValue)
                recordContext.Value(record.CustomerRouteOptionMarginStaging.SupplierDealID.Value);
            else
                recordContext.Value(string.Empty);

            recordContext.Value(record.OptimalCustomerRouteOptionMarginStaging.SupplierZoneID);
            recordContext.Value(serializedOptimalSupplierServiceIds);
            recordContext.Value(record.OptimalCustomerRouteOptionMarginStaging.SupplierRate);

            if (record.OptimalCustomerRouteOptionMarginStaging.SupplierDealID.HasValue)
                recordContext.Value(record.OptimalCustomerRouteOptionMarginStaging.SupplierDealID.Value);
            else
                recordContext.Value(string.Empty);
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            RDBBulkInsertQueryContext bulkInsertQueryContext = dbApplyStream.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream");
            bulkInsertQueryContext.CloseStream();
            return bulkInsertQueryContext;
        }

        private void ApplyCustomerRouteMarginForDB(object preparedCustomerRouteMargin)
        {
            preparedCustomerRouteMargin.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream").Apply();
        }

        #endregion

        #region Private Methods

        private BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("TOneWhS_Routing", "TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString");
        }

        private string GetTableName(RoutingDatabaseType routingDatabaseType)
        {
            switch (routingDatabaseType)
            {
                case RoutingDatabaseType.Current: return CurrentStaging_TableName;
                case RoutingDatabaseType.Future: return FutureStaging_TableName;
                default: throw new NotSupportedException($"RoutingDatabaseType {routingDatabaseType} not supported.");
            }
        }

        private string GetDBTableName(RoutingDatabaseType routingDatabaseType, bool isTempTable)
        {
            switch (routingDatabaseType)
            {
                case RoutingDatabaseType.Current: return CurrentStaging_DBTableName;
                case RoutingDatabaseType.Future: return FutureStaging_DBTableName;
                default: throw new NotSupportedException($"RoutingDatabaseType {routingDatabaseType} not supported.");
            }
        }

        private CustomerRouteMarginStaging CustomerRouteMarginStagingMapper(IRDBDataReader reader)
        {
            string codesAsString = reader.GetString(COL_Codes);
            HashSet<string> codes = new HashSet<string>(codesAsString.Split(','));

            string supplierServiceIDsAsString = reader.GetString(COL_SupplierServiceIDs);
            HashSet<int> supplierServiceIDs = Vanrise.Common.Serializer.Deserialize<HashSet<int>>(supplierServiceIDsAsString);

            string optimalSupplierServiceIDsAsString = reader.GetString(COL_OptimalSupplierServiceIDs);
            HashSet<int> optimalSupplierServiceIDs = Vanrise.Common.Serializer.Deserialize<HashSet<int>>(optimalSupplierServiceIDsAsString);

            CustomerRouteMarginStaging customerRouteMarginStaging = new CustomerRouteMarginStaging
            {
                CustomerID = reader.GetInt(COL_CustomerID),
                SaleZoneID = reader.GetLong(COL_SaleZoneID),
                SaleRate = reader.GetDecimal(COL_SaleRate),
                SaleDealID = reader.GetNullableInt(COL_SaleDealID),
                Codes = codes,
                CustomerRouteOptionMarginStaging = new CustomerRouteOptionMarginStaging()
                {
                    SupplierZoneID = reader.GetLong(COL_SupplierZoneID),
                    SupplierServiceIDs = supplierServiceIDs,
                    SupplierRate = reader.GetDecimal(COL_SupplierRate),
                    SupplierDealID = reader.GetNullableInt(COL_SupplierDealID)
                },
                OptimalCustomerRouteOptionMarginStaging = new CustomerRouteOptionMarginStaging()
                {
                    SupplierZoneID = reader.GetLong(COL_OptimalSupplierZoneID),
                    SupplierServiceIDs = optimalSupplierServiceIDs,
                    SupplierRate = reader.GetDecimal(COL_OptimalSupplierRate),
                    SupplierDealID = reader.GetNullableInt(COL_OptimalSupplierDealID)
                }
            };

            return customerRouteMarginStaging;
        }

        #endregion
    }
}