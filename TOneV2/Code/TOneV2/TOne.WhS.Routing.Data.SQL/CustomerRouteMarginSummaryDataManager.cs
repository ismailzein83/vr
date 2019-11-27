using System;
using System.Collections.Generic;
using TOne.WhS.Routing.Entities;
using Vanrise.Common;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace TOne.WhS.Routing.Data.SQL
{
    public class CustomerRouteMarginSummaryDataManager : ICustomerRouteMarginSummaryDataManager
    {
        #region Fields/Ctor

        private string tableName;

        const string DBTableSchema = "TOneWhS_Routing";

        const string Current_DBTableName = "CustomerRouteMarginSummary_Current";
        const string CurrentTemp_DBTableName = "CustomerRouteMarginSummary_Current_Temp";
        const string Future_DBTableName = "CustomerRouteMarginSummary_Future";
        const string FutureTemp_DBTableName = "CustomerRouteMarginSummary_Future_Temp";

        public const string Current_TableName = "TOneWhS_Routing_CustomerRouteMarginSummary_Current";
        public const string CurrentTemp_TableName = "TOneWhS_Routing_CustomerRouteMarginSummary_Current_Temp";
        public const string Future_TableName = "TOneWhS_Routing_CustomerRouteMarginSummary_Future";
        public const string FutureTemp_TableName = "TOneWhS_Routing_CustomerRouteMarginSummary_Future_Temp";

        public const string COL_ID = "ID";
        public const string COL_CustomerID = "CustomerID";
        public const string COL_SaleZoneID = "SaleZoneID";
        public const string COL_SaleRate = "SaleRate";
        public const string COL_SaleDealID = "SaleDealID";

        public const string COL_MinSupplierRate = "MinSupplierRate";
        public const string COL_MaxMargin = "MaxMargin";
        public const string COL_MaxMarginCategoryID = "MaxMarginCategoryID";

        public const string COL_MaxSupplierRate = "MaxSupplierRate";
        public const string COL_MinMargin = "MinMargin";
        public const string COL_MinMarginCategoryID = "MinMarginCategoryID";

        public const string COL_AvgSupplierRate = "AvgSupplierRate";
        public const string COL_AvgMargin = "AvgMargin";
        public const string COL_AvgMarginCategoryID = "AvgMarginCategoryID";

        public const string COL_CreatedTime = "CreatedTime";

        static CustomerRouteMarginSummaryDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();

            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_CustomerID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_SaleZoneID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_SaleRate, new RDBTableColumnDefinition { DataType = RDBDataType.Decimal, Size = 20, Precision = 8 });
            columns.Add(COL_SaleDealID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });

            columns.Add(COL_MinSupplierRate, new RDBTableColumnDefinition { DataType = RDBDataType.Decimal, Size = 20, Precision = 8 });
            columns.Add(COL_MaxMargin, new RDBTableColumnDefinition { DataType = RDBDataType.Decimal, Size = 20, Precision = 8 });
            columns.Add(COL_MaxMarginCategoryID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });

            columns.Add(COL_MaxSupplierRate, new RDBTableColumnDefinition { DataType = RDBDataType.Decimal, Size = 20, Precision = 8 });
            columns.Add(COL_MinMargin, new RDBTableColumnDefinition { DataType = RDBDataType.Decimal, Size = 20, Precision = 8 });
            columns.Add(COL_MinMarginCategoryID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });

            columns.Add(COL_AvgSupplierRate, new RDBTableColumnDefinition { DataType = RDBDataType.Decimal, Size = 20, Precision = 8 });
            columns.Add(COL_AvgMargin, new RDBTableColumnDefinition { DataType = RDBDataType.Decimal, Size = 20, Precision = 8 });
            columns.Add(COL_AvgMarginCategoryID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });

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

        public void CreateCustomerRouteMarginSummaryTempTable(RoutingDatabaseType routingDatabaseType, Action<string> trackStep)
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

            createTableQuery.AddColumn(COL_MinSupplierRate, RDBDataType.Decimal, 20, 8, true);
            createTableQuery.AddColumn(COL_MaxMargin, RDBDataType.Decimal, 20, 8, true);
            createTableQuery.AddColumn(COL_MaxMarginCategoryID, RDBDataType.UniqueIdentifier, false);

            createTableQuery.AddColumn(COL_MaxSupplierRate, RDBDataType.Decimal, 20, 8, true);
            createTableQuery.AddColumn(COL_MinMargin, RDBDataType.Decimal, 20, 8, true);
            createTableQuery.AddColumn(COL_MinMarginCategoryID, RDBDataType.UniqueIdentifier, false);

            createTableQuery.AddColumn(COL_AvgSupplierRate, RDBDataType.Decimal, 20, 8, true);
            createTableQuery.AddColumn(COL_AvgMargin, RDBDataType.Decimal, 20, 8, true);
            createTableQuery.AddColumn(COL_AvgMarginCategoryID, RDBDataType.UniqueIdentifier, false);

            createTableQuery.AddColumn(COL_CreatedTime, RDBDataType.DateTime, true);

            queryContext.ExecuteNonQuery();

            trackStep("Creating CustomerRouteMarginSummaryTemp table is done");
        }

        public void InsertCustomerRouteMarginSummariesToDB(RoutingDatabaseType routingDatabaseType, List<CustomerRouteMarginSummary> customerRouteMarginSummaryList, Action<string> trackStep)
        {
            if (customerRouteMarginSummaryList == null || customerRouteMarginSummaryList.Count == 0)
                return;

            trackStep("Starting save CustomerRouteMarginSummary Data");

            tableName = GetTableName(routingDatabaseType, true);

            Object dbApplyAddStream = InitialiazeStreamForDBApply();
            foreach (var customerRouteMarginSummary in customerRouteMarginSummaryList)
            {
                WriteRecordToStream(customerRouteMarginSummary, dbApplyAddStream);
            }
            Object preparedAddRoutes = FinishDBApplyStream(dbApplyAddStream);
            ApplyCustomerRouteMarginForDB(preparedAddRoutes);

            trackStep($"Finished save CustomerRouteMarginSummary Data. Events count: {customerRouteMarginSummaryList.Count}");
        }

        public void CreateIndexes(RoutingDatabaseType routingDatabaseType, Action<string> trackStep)
        {
            string dummyGuid = Guid.NewGuid().ToString("N");
            string dbTableName = GetDBTableName(routingDatabaseType, true);

            var queryContext = new RDBQueryContext(this.GetDataProvider());

            trackStep("Starting create Indexes on CustomerRouteMarginSummaryTemp table.");

            var createPrimaryKeyIndexQuery = queryContext.AddCreateIndexQuery();
            createPrimaryKeyIndexQuery.DBTableName(DBTableSchema, dbTableName);
            createPrimaryKeyIndexQuery.IndexName($"PK_CustomerRouteMarginSummary_ID_{dummyGuid}");
            createPrimaryKeyIndexQuery.IndexType(RDBCreateIndexType.UniqueClustered);
            createPrimaryKeyIndexQuery.AddColumn(COL_ID);

            var createCustomerSaleZoneNonClusteredIndexQuery = queryContext.AddCreateIndexQuery();
            createCustomerSaleZoneNonClusteredIndexQuery.DBTableName(DBTableSchema, dbTableName);
            createCustomerSaleZoneNonClusteredIndexQuery.IndexName($"IX_CustomerRouteMarginSummary_CustomerID_SaleZoneID_{dummyGuid}");
            createCustomerSaleZoneNonClusteredIndexQuery.IndexType(RDBCreateIndexType.NonClustered);
            createCustomerSaleZoneNonClusteredIndexQuery.AddColumn(COL_CustomerID);
            createCustomerSaleZoneNonClusteredIndexQuery.AddColumn(COL_SaleZoneID);

            var createSaleZoneNonClusteredIndexQuery = queryContext.AddCreateIndexQuery();
            createSaleZoneNonClusteredIndexQuery.DBTableName(DBTableSchema, dbTableName);
            createSaleZoneNonClusteredIndexQuery.IndexName($"IX_CustomerRouteMarginSummary_SaleZoneID_{dummyGuid}");
            createSaleZoneNonClusteredIndexQuery.IndexType(RDBCreateIndexType.NonClustered);
            createSaleZoneNonClusteredIndexQuery.AddColumn(COL_SaleZoneID);

            queryContext.ExecuteNonQuery();

            trackStep("Finished create Indexes on CustomerRouteMarginSummaryTemp table.");
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

            bulkInsertQueryContext.IntoTable(tableName, '^', COL_ID, COL_CustomerID, COL_SaleZoneID, COL_SaleRate, COL_SaleDealID,
                COL_MinSupplierRate, COL_MaxMargin, COL_MaxMarginCategoryID,
                COL_MaxSupplierRate, COL_MinMargin, COL_MinMarginCategoryID,
                COL_AvgSupplierRate, COL_AvgMargin, COL_AvgMarginCategoryID,
                COL_CreatedTime);

            return bulkInsertQueryContext;
        }

        public void WriteRecordToStream(CustomerRouteMarginSummary record, object dbApplyStream)
        {
            RDBBulkInsertQueryContext bulkInsertQueryContext = dbApplyStream.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream");

            var recordContext = bulkInsertQueryContext.WriteRecord();

            recordContext.Value(record.CustomerRouteMarginSummaryID);
            recordContext.Value(record.CustomerID);
            recordContext.Value(record.SaleZoneID);
            recordContext.Value(record.SaleRate);

            if (record.SaleDealID.HasValue)
                recordContext.Value(record.SaleDealID.Value);
            else
                recordContext.Value(string.Empty);

            //Min Supplier Rate
            recordContext.Value(record.MinSupplierRate);
            recordContext.Value(record.MaxMargin);

            if (record.MaxMarginCategoryID.HasValue)
                recordContext.Value(record.MaxMarginCategoryID.Value);
            else
                recordContext.Value(string.Empty);

            //Max Supplier Rate
            recordContext.Value(record.MaxSupplierRate);
            recordContext.Value(record.MinMargin);

            if (record.MinMarginCategoryID.HasValue)
                recordContext.Value(record.MinMarginCategoryID.Value);
            else
                recordContext.Value(string.Empty);

            //Avg Supplier Rate
            recordContext.Value(record.AvgSupplierRate);
            recordContext.Value(record.AvgMargin);

            if (record.AvgMarginCategoryID.HasValue)
                recordContext.Value(record.AvgMarginCategoryID.Value);
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