using System;
using System.Collections.Generic;
using TOne.WhS.Routing.Entities;
using Vanrise.Common;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace TOne.WhS.Routing.Data.SQL
{
    public class RiskyMarginCodeDataManager : IRiskyMarginCodeDataManager
    {
        #region Fields/Ctor

        private string tableName;

        const string DBTableSchema = "TOneWhS_Routing";

        const string Current_DBTableName = "RiskyMarginCode_Current";
        const string CurrentTemp_DBTableName = "RiskyMarginCode_Current_Temp";
        const string Future_DBTableName = "RiskyMarginCode_Future";
        const string FutureTemp_DBTableName = "RiskyMarginCode_Future_Temp";

        public const string Current_TableName = "TOneWhS_Routing_RiskyMarginCode_Current";
        public const string CurrentTemp_TableName = "TOneWhS_Routing_RiskyMarginCode_Current_Temp";
        public const string Future_TableName = "TOneWhS_Routing_RiskyMarginCode_Future";
        public const string FutureTemp_TableName = "TOneWhS_Routing_RiskyMarginCode_Future_Temp";

        public const string COL_Code = "Code";
        public const string COL_CustomerRouteMarginID = "CustomerRouteMarginID";
        public const string COL_CreatedTime = "CreatedTime";
         
        static RiskyMarginCodeDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_Code, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 20 });
            columns.Add(COL_CustomerRouteMarginID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
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

        public void CreateRiskyMarginCodeTempTable(RoutingDatabaseType routingDatabaseType, Action<string> trackStep)
        {
            tableName = GetTableName(routingDatabaseType, true);
            string dbTableName = GetDBTableName(routingDatabaseType, true);

            var queryContext = new RDBQueryContext(this.GetDataProvider());

            var dropTableQuery = queryContext.AddDropTableQuery();
            dropTableQuery.TableName(tableName);

            var createTableQuery = queryContext.AddCreateTableQuery();
            createTableQuery.DBTableName(DBTableSchema, dbTableName);
            createTableQuery.AddColumn(COL_Code, RDBDataType.Varchar, 20, 0, true);
            createTableQuery.AddColumn(COL_CustomerRouteMarginID, RDBDataType.BigInt, true);
            createTableQuery.AddColumn(COL_CreatedTime, RDBDataType.DateTime, true);

            queryContext.ExecuteNonQuery();

            trackStep("Creating RiskyMarginCodeTemp table is done");
        }

        public void InsertRiskyMarginCodesToDB(RoutingDatabaseType routingDatabaseType, List<RiskyMarginCode> riskyMarginCodes, Action<string> trackStep)
        {
            if (riskyMarginCodes == null || riskyMarginCodes.Count == 0)
                return;

            trackStep("Starting save RiskyMarginCode Data");

            tableName = GetTableName(routingDatabaseType, true);

            int riskyMarginCodeNB = 0;

            Object dbApplyAddStream = InitialiazeStreamForDBApply();
            foreach (var riskyMarginCode in riskyMarginCodes)
            {
                WriteRecordToStream(riskyMarginCode, dbApplyAddStream);
                riskyMarginCodeNB++;

                if (riskyMarginCodeNB >= 100000)
                {
                    Object preparedAddRoutes = FinishDBApplyStream(dbApplyAddStream);
                    ApplyCustomerRouteMarginForDB(preparedAddRoutes);

                    riskyMarginCodeNB = 0;
                    dbApplyAddStream = InitialiazeStreamForDBApply();
                }
            }

            trackStep($"Finished save RiskyMarginCode Data. Events count: {riskyMarginCodes.Count}");
        }

        public void CreateIndexes(RoutingDatabaseType routingDatabaseType, Action<string> trackStep)
        {
            string dummyGuid = Guid.NewGuid().ToString("N");
            string dbTableName = GetDBTableName(routingDatabaseType, true);

            var queryContext = new RDBQueryContext(this.GetDataProvider());

            trackStep("Starting create Indexes on RiskyMarginCodeTemp table.");

            var createCustomerSaleZoneNonClusteredIndexQuery = queryContext.AddCreateIndexQuery();
            createCustomerSaleZoneNonClusteredIndexQuery.DBTableName(DBTableSchema, dbTableName);
            createCustomerSaleZoneNonClusteredIndexQuery.IndexName($"IX_RiskyMarginCode_CustomerRouteMarginID_{dummyGuid}");
            createCustomerSaleZoneNonClusteredIndexQuery.IndexType(RDBCreateIndexType.NonClustered);
            createCustomerSaleZoneNonClusteredIndexQuery.AddColumn(COL_CustomerRouteMarginID);
            
            queryContext.ExecuteNonQuery();

            trackStep("Finished create Indexes on RiskyMarginCodeTemp table.");
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

            bulkInsertQueryContext.IntoTable(tableName, '^', COL_Code, COL_CustomerRouteMarginID, COL_CreatedTime);

            return bulkInsertQueryContext;
        }

        public void WriteRecordToStream(RiskyMarginCode record, object dbApplyStream)
        {
            RDBBulkInsertQueryContext bulkInsertQueryContext = dbApplyStream.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream");

            var recordContext = bulkInsertQueryContext.WriteRecord();
            recordContext.Value(record.Code);
            recordContext.Value(record.CustomerRouteMarginID);
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