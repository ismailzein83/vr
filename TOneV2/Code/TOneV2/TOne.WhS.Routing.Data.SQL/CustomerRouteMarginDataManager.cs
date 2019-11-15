using System;
using System.Collections.Generic;
using TOne.WhS.Routing.Entities;
using Vanrise.Common;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace TOne.WhS.Routing.Data.SQL
{
    public class CustomerRouteMarginDataManager : ICustomerRouteMarginDataManager
    {
        #region Fields/Ctor

        private string tableName;

        const string DBTableSchema = "TOneWhS_Routing";

        const string Current_DBTableName = "CustomerRouteMargin_Current";
        const string CurrentTemp_DBTableName = "CustomerRouteMargin_Current_Temp";
        const string Future_DBTableName = "CustomerRouteMargin_Future";
        const string FutureTemp_DBTableName = "CustomerRouteMargin_Future_Temp";

        public const string Current_TableName = "TOneWhS_Routing_CustomerRouteMargin_Current";
        public const string CurrentTemp_TableName = "TOneWhS_Routing_CustomerRouteMargin_Current_Temp";
        public const string Future_TableName = "TOneWhS_Routing_CustomerRouteMargin_Future";
        public const string FutureTemp_TableName = "TOneWhS_Routing_CustomerRouteMargin_Future_Temp";

        public const string COL_ID = "ID";
        public const string COL_CustomerID = "CustomerID";
        public const string COL_SaleZoneID = "SaleZoneID";
        public const string COL_SaleRate = "SaleRate";
        public const string COL_SaleDealID = "SaleDealID";
        public const string COL_IsRisky = "IsRisky";

        public const string COL_SupplierZoneID = "SupplierZoneID";
        public const string COL_SupplierServiceIDs = "SupplierServiceIDs";
        public const string COL_SupplierRate = "SupplierRate";
        public const string COL_SupplierDealID = "SupplierDealID";
        public const string COL_Margin = "Margin";
        public const string COL_MarginCategoryID = "MarginCategoryID";

        public const string COL_OptimalSupplierZoneID = "OptimalSupplierZoneID";
        public const string COL_OptimalSupplierServiceIDs = "OptimalSupplierServiceIDs";
        public const string COL_OptimalSupplierRate = "OptimalSupplierRate";
        public const string COL_OptimalSupplierDealID = "OptimalSupplierDealID";
        public const string COL_OptimalMargin = "OptimalMargin";
        public const string COL_OptimalMarginCategoryID = "OptimalMarginCategoryID";

        public const string COL_CreatedTime = "CreatedTime";

        static CustomerRouteMarginDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();

            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_CustomerID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_SaleZoneID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_SaleRate, new RDBTableColumnDefinition { DataType = RDBDataType.Decimal, Size = 20, Precision = 8 });
            columns.Add(COL_SaleDealID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_IsRisky, new RDBTableColumnDefinition { DataType = RDBDataType.Boolean });

            columns.Add(COL_SupplierZoneID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_SupplierServiceIDs, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_SupplierRate, new RDBTableColumnDefinition { DataType = RDBDataType.Decimal, Size = 20, Precision = 8 });
            columns.Add(COL_SupplierDealID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_Margin, new RDBTableColumnDefinition { DataType = RDBDataType.Decimal, Size = 20, Precision = 8 });
            columns.Add(COL_MarginCategoryID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });

            columns.Add(COL_OptimalSupplierZoneID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_OptimalSupplierServiceIDs, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_OptimalSupplierRate, new RDBTableColumnDefinition { DataType = RDBDataType.Decimal, Size = 20, Precision = 8 });
            columns.Add(COL_OptimalSupplierDealID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_OptimalMargin, new RDBTableColumnDefinition { DataType = RDBDataType.Decimal, Size = 20, Precision = 8 });
            columns.Add(COL_OptimalMarginCategoryID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });

            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });

            RDBSchemaManager.Current.RegisterDefaultTableDefinition(Current_TableName, new RDBTableDefinition
            {
                DBSchemaName = DBTableSchema,
                DBTableName = Current_DBTableName,
                Columns = columns
            });

            RDBSchemaManager.Current.RegisterDefaultTableDefinition(CurrentTemp_TableName, new RDBTableDefinition
            {
                DBSchemaName = DBTableSchema,
                DBTableName = CurrentTemp_DBTableName,
                Columns = columns
            });

            RDBSchemaManager.Current.RegisterDefaultTableDefinition(Future_TableName, new RDBTableDefinition
            {
                DBSchemaName = DBTableSchema,
                DBTableName = Future_DBTableName,
                Columns = columns
            });

            RDBSchemaManager.Current.RegisterDefaultTableDefinition(FutureTemp_TableName, new RDBTableDefinition
            {
                DBSchemaName = DBTableSchema,
                DBTableName = FutureTemp_DBTableName,
                Columns = columns
            });
        }

        #endregion

        #region Public Methods

        public void CreateCustomerRouteMarginTempTable(RoutingDatabaseType routingDatabaseType)
        {
            tableName = GetTableName(routingDatabaseType, true);
            string dbTableName = GetDBTableName(routingDatabaseType, true);

            var queryContext = new RDBQueryContext(this.GetDataProvider());

            var dropTableQuery = queryContext.AddDropTableQuery();
            dropTableQuery.TableName(tableName);

            var createTableQuery = queryContext.AddCreateTableQuery();
            createTableQuery.DBTableName(DBTableSchema, dbTableName);

            createTableQuery.AddColumn(COL_ID, RDBDataType.BigInt, true);
            createTableQuery.AddColumn(COL_CustomerID, RDBDataType.Int, true);
            createTableQuery.AddColumn(COL_SaleZoneID, RDBDataType.BigInt, true);
            createTableQuery.AddColumn(COL_SaleRate, RDBDataType.Decimal, 20, 8, true);
            createTableQuery.AddColumn(COL_SaleDealID, RDBDataType.Int);
            createTableQuery.AddColumn(COL_IsRisky, RDBDataType.Boolean, true);

            createTableQuery.AddColumn(COL_SupplierZoneID, RDBDataType.BigInt);
            createTableQuery.AddColumn(COL_SupplierServiceIDs, RDBDataType.NVarchar, true);
            createTableQuery.AddColumn(COL_SupplierRate, RDBDataType.Decimal, 20, 8, true);
            createTableQuery.AddColumn(COL_SupplierDealID, RDBDataType.Int);
            createTableQuery.AddColumn(COL_Margin, RDBDataType.Decimal, 20, 8, true);
            createTableQuery.AddColumn(COL_MarginCategoryID, RDBDataType.UniqueIdentifier, false);

            createTableQuery.AddColumn(COL_OptimalSupplierZoneID, RDBDataType.BigInt);
            createTableQuery.AddColumn(COL_OptimalSupplierServiceIDs, RDBDataType.NVarchar, true);
            createTableQuery.AddColumn(COL_OptimalSupplierRate, RDBDataType.Decimal, 20, 8, true);
            createTableQuery.AddColumn(COL_OptimalSupplierDealID, RDBDataType.Int);
            createTableQuery.AddColumn(COL_OptimalMargin, RDBDataType.Decimal, 20, 8, true);
            createTableQuery.AddColumn(COL_OptimalMarginCategoryID, RDBDataType.UniqueIdentifier, false);

            createTableQuery.AddColumn(COL_CreatedTime, RDBDataType.DateTime, true);

            queryContext.ExecuteNonQuery();
        }

        public void InsertCustomerRouteMarginsToDB(RoutingDatabaseType routingDatabaseType, List<CustomerRouteMargin> customerRouteMargins)
        {
            if (customerRouteMargins == null || customerRouteMargins.Count == 0)
                return;

            tableName = GetTableName(routingDatabaseType, true);

            Object dbApplyAddStream = InitialiazeStreamForDBApply();
            foreach (var customerRouteMargin in customerRouteMargins)
            {
                WriteRecordToStream(customerRouteMargin, dbApplyAddStream);
            }
            Object preparedAddRoutes = FinishDBApplyStream(dbApplyAddStream);
            ApplyCustomerRouteMarginForDB(preparedAddRoutes);
        }

        public void CreateIndexes(RoutingDatabaseType routingDatabaseType, Action<string> trackStep)
        {
            string dummyGuid = Guid.NewGuid().ToString().Replace("-", "");
            string dbTableName = GetDBTableName(routingDatabaseType, true);

            var queryContext = new RDBQueryContext(this.GetDataProvider());

            trackStep("Starting create Indexes on CustomerRouteMargin table.");

            var createPrimaryKeyIndexQuery = queryContext.AddCreateIndexQuery();
            createPrimaryKeyIndexQuery.DBTableName(DBTableSchema, dbTableName);
            createPrimaryKeyIndexQuery.IndexName($"PK_CustomerRouteMargin_ID_{dummyGuid}");
            createPrimaryKeyIndexQuery.IndexType(RDBCreateIndexType.UniqueClustered);
            createPrimaryKeyIndexQuery.AddColumn(COL_ID);

            var createCustomerSaleZoneNonClusteredIndexQuery = queryContext.AddCreateIndexQuery();
            createCustomerSaleZoneNonClusteredIndexQuery.DBTableName(DBTableSchema, dbTableName);
            createCustomerSaleZoneNonClusteredIndexQuery.IndexName($"IX_CustomerRouteMargin_CustomerID_SaleZoneID_{dummyGuid}");
            createCustomerSaleZoneNonClusteredIndexQuery.IndexType(RDBCreateIndexType.NonClustered);
            createCustomerSaleZoneNonClusteredIndexQuery.AddColumn(COL_CustomerID);
            createCustomerSaleZoneNonClusteredIndexQuery.AddColumn(COL_SaleZoneID);

            var createSaleZoneNonClusteredIndexQuery = queryContext.AddCreateIndexQuery();
            createSaleZoneNonClusteredIndexQuery.DBTableName(DBTableSchema, dbTableName);
            createSaleZoneNonClusteredIndexQuery.IndexName($"IX_CustomerRouteMargin_SaleZoneID_{dummyGuid}");
            createSaleZoneNonClusteredIndexQuery.IndexType(RDBCreateIndexType.NonClustered);
            createSaleZoneNonClusteredIndexQuery.AddColumn(COL_SaleZoneID);

            var createSupplierZoneNonClusteredIndexQuery = queryContext.AddCreateIndexQuery();
            createSupplierZoneNonClusteredIndexQuery.DBTableName(DBTableSchema, dbTableName);
            createSupplierZoneNonClusteredIndexQuery.IndexName($"IX_CustomerRouteMargin_SupplierZoneID_{dummyGuid}");
            createSupplierZoneNonClusteredIndexQuery.IndexType(RDBCreateIndexType.NonClustered);
            createSupplierZoneNonClusteredIndexQuery.AddColumn(COL_SupplierZoneID);

            trackStep("Finished create Indexes on CustomerRouteMargin table.");

            queryContext.ExecuteNonQuery();
        }

        public void SwapTables(RoutingDatabaseType routingDatabaseType)
        {
            string tempTableName = GetTableName(routingDatabaseType, true);
            string existingTableName = GetTableName(routingDatabaseType, false);

            var queryContext = new RDBQueryContext(GetDataProvider());
            var swapTablesQuery = queryContext.AddSwapTablesQuery();
            swapTablesQuery.TableNames(existingTableName, tempTableName, false);

            queryContext.ExecuteNonQuery();
        }

        #endregion

        #region IBulkApplyDataManager

        public object InitialiazeStreamForDBApply()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var bulkInsertQueryContext = queryContext.StartBulkInsert();

            bulkInsertQueryContext.IntoTable(tableName, '^', COL_ID, COL_CustomerID, COL_SaleZoneID, COL_SaleRate, COL_SaleDealID, COL_IsRisky,
                COL_SupplierZoneID, COL_SupplierServiceIDs, COL_SupplierRate, COL_SupplierDealID, COL_Margin, COL_MarginCategoryID,
                COL_OptimalSupplierZoneID, COL_OptimalSupplierServiceIDs, COL_OptimalSupplierRate, COL_OptimalSupplierDealID, COL_OptimalMargin, COL_OptimalMarginCategoryID,
                COL_CreatedTime);

            return bulkInsertQueryContext;
        }

        public void WriteRecordToStream(CustomerRouteMargin record, object dbApplyStream)
        {
            RDBBulkInsertQueryContext bulkInsertQueryContext = dbApplyStream.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream");

            string serializedSupplierServiceIds = Vanrise.Common.Serializer.Serialize(record.CustomerRouteOptionMargin.SupplierServiceIDs, true);
            string serializedOptimalSupplierServiceIds = Vanrise.Common.Serializer.Serialize(record.OptimalCustomerRouteOptionMargin.SupplierServiceIDs, true);

            var recordContext = bulkInsertQueryContext.WriteRecord();
            recordContext.Value(record.CustomerRouteMarginID);
            recordContext.Value(record.CustomerID);
            recordContext.Value(record.SaleZoneID);
            recordContext.Value(record.SaleRate);

            if (record.SaleDealID.HasValue)
                recordContext.Value(record.SaleDealID.Value);
            else
                recordContext.Value(string.Empty);

            recordContext.Value(record.IsRisky);


            recordContext.Value(record.CustomerRouteOptionMargin.SupplierZoneID);
            recordContext.Value(serializedSupplierServiceIds);
            recordContext.Value(record.CustomerRouteOptionMargin.SupplierRate);

            if (record.CustomerRouteOptionMargin.SupplierDealID.HasValue)
                recordContext.Value(record.CustomerRouteOptionMargin.SupplierDealID.Value);
            else
                recordContext.Value(string.Empty);

            recordContext.Value(record.CustomerRouteOptionMargin.Margin);

            if (record.CustomerRouteOptionMargin.MarginCategoryID.HasValue)
                recordContext.Value(record.CustomerRouteOptionMargin.MarginCategoryID.Value);
            else
                recordContext.Value(string.Empty);


            recordContext.Value(record.OptimalCustomerRouteOptionMargin.SupplierZoneID);
            recordContext.Value(serializedOptimalSupplierServiceIds);
            recordContext.Value(record.OptimalCustomerRouteOptionMargin.SupplierRate);

            if (record.OptimalCustomerRouteOptionMargin.SupplierDealID.HasValue)
                recordContext.Value(record.OptimalCustomerRouteOptionMargin.SupplierDealID.Value);
            else
                recordContext.Value(string.Empty);

            recordContext.Value(record.OptimalCustomerRouteOptionMargin.Margin);

            if (record.OptimalCustomerRouteOptionMargin.MarginCategoryID.HasValue)
                recordContext.Value(record.OptimalCustomerRouteOptionMargin.MarginCategoryID.Value);
            else
                recordContext.Value(string.Empty);

            recordContext.Value(DateTime.Now);
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

        private string GetTableName(RoutingDatabaseType routingDatabaseType, bool isTempTable)
        {
            switch (routingDatabaseType)
            {
                case RoutingDatabaseType.Current: return isTempTable ? CurrentTemp_TableName : Current_TableName;
                case RoutingDatabaseType.Future: return isTempTable ? FutureTemp_TableName : Future_TableName;
                default: throw new NotSupportedException($"RoutingDatabaseType {routingDatabaseType} not supported.");
            }
        }

        private string GetDBTableName(RoutingDatabaseType routingDatabaseType, bool isTempTable)
        {
            switch (routingDatabaseType)
            {
                case RoutingDatabaseType.Current: return isTempTable ? CurrentTemp_DBTableName : Current_DBTableName;
                case RoutingDatabaseType.Future: return isTempTable ? FutureTemp_DBTableName : Future_DBTableName;
                default: throw new NotSupportedException($"RoutingDatabaseType {routingDatabaseType} not supported.");
            }
        }

        #endregion
    }
}