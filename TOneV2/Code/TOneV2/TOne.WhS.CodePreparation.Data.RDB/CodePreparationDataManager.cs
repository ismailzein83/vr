using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Entities;
using Vanrise.Data.RDB;
using Vanrise.Entities;
using Vanrise.Common;
using TOne.WhS.BusinessEntity.Data.RDB;

namespace TOne.WhS.CodePreparation.Data.RDB
{
    public class CodePreparationDataManager : ICodePreparationDataManager
    {
        #region RDB
        static string TABLE_NAME = "TOneWhs_CP_CodePreparation";
        static string TABLE_ALIAS = "cp";
        const string COL_ID = "ID";
        const string COL_SellingNumberPlanId = "SellingNumberPlanId";
        const string COL_Changes = "Changes";
        const string COL_Status = "Status";
        const string COL_CreatedTime = "CreatedTime";


        static CodePreparationDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_SellingNumberPlanId, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_Changes, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_Status, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "TOneWhs_CP",
                DBTableName = "CodePreparation",
                Columns = columns,
                IdColumnName = COL_ID,
                CreatedTimeColumnName = COL_CreatedTime
            });
        }
        #endregion

        #region Private Methods
        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("WhS_CodePrep", "TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString");
        }
        #endregion

        #region Mappers
        private Changes ChangesMapper(IRDBDataReader reader)
        {
            return Serializer.Deserialize<Changes>(reader.GetString(COL_Changes));
        }
        #endregion
        #region ICodePreparationDataManager
        public bool AddPriceListAndSyncImportedDataWithDB(long processInstanceID, int sellingNumberPlanId, long stateBackupId)//there are sale pricelist tables missing
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var salePriceListDataManager = new SalePriceListDataManager();
            var salePriceListNewDataManager = new SalePriceListNewDataManager();
            var insertSalePricelist = queryContext.AddInsertQuery();
            salePriceListDataManager.BuildInsertQuery(insertSalePricelist, processInstanceID, stateBackupId);
            salePriceListNewDataManager.BuildSelectQuery(insertSalePricelist.FromSelect(), processInstanceID);


            var saleZoneDataManager = new SaleZoneDataManager();
            var newSaleZoneDataManager = new NewSaleZoneDataManager();
            var insertSaleZone = queryContext.AddInsertQuery();
            saleZoneDataManager.BuildInsertQuery(insertSaleZone, processInstanceID);
            newSaleZoneDataManager.BuildSelectQuery(insertSaleZone.FromSelect(), processInstanceID, sellingNumberPlanId);

            var saleCodeDataManager = new SaleCodeDataManager();
            var newSaleCodeDataManager = new NewSaleCodeDataManager();
            var insertSaleCodeDataManager = queryContext.AddInsertQuery();
            saleCodeDataManager.BuildInsertQuery(insertSaleCodeDataManager, processInstanceID);
            newSaleCodeDataManager.BuildSelectQuery(insertSaleCodeDataManager.FromSelect(), processInstanceID);

            var saleRateDataManager = new SaleRateDataManager();
            var newSaleRateDataManager = new NewSaleRateDataManager();
            var insertSaleRate = queryContext.AddInsertQuery();
            saleRateDataManager.BuildInsertQuery(insertSaleRate);
            newSaleRateDataManager.BuildSelectQuery(insertSaleRate.FromSelect(), processInstanceID);

            var saleEntityRoutingProductDataManager = new SaleEntityRoutingProductDataManager();
            var newSaleZoneRPDataManager = new NewSaleZoneRPDataManager();
            var insertsaleEntityRoutingProduct = queryContext.AddInsertQuery();
            saleEntityRoutingProductDataManager.BuildInsertQuery(insertsaleEntityRoutingProduct);
            newSaleZoneRPDataManager.BuildSelectQuery(insertsaleEntityRoutingProduct.FromSelect(), processInstanceID);

            var salePricelistCodeChangeDataManager = new SalePricelistCodeChangeDataManager();
            var salePricelistCodeChangeNewDataManager = new SalePricelistCodeChangeNewDataManager();
            var insertSalePricelistCodeChange = queryContext.AddInsertQuery();
            salePricelistCodeChangeDataManager.BuildInsertQuery(insertSalePricelistCodeChange);
            salePricelistCodeChangeNewDataManager.BuildSelectQuery(insertSalePricelistCodeChange.FromSelect(), processInstanceID);

            var salePricelistCustomerChangeDataManager = new SalePricelistCustomerChangeDataManager();
            var salePricelistCustomerChangeNewDataManager = new SalePricelistCustomerChangeNewDataManager();
            var insertSalePricelistCustomerChange = queryContext.AddInsertQuery();
            salePricelistCustomerChangeDataManager.BuildInsertQuery(insertSalePricelistCustomerChange);
            salePricelistCustomerChangeNewDataManager.BuildSelectQuery(insertSalePricelistCustomerChange.FromSelect(), processInstanceID);

            var salePricelistRateChangeDataManager = new SalePricelistRateChangeDataManager();
            var salePriceListRateChangeNewDataManager = new SalePricelistRateChangeNewDataManager();
            var insertSalePricelistRateChange = queryContext.AddInsertQuery();
            salePricelistRateChangeDataManager.BuildInsertQuery(insertSalePricelistRateChange);
            salePriceListRateChangeNewDataManager.BuildSelectQuery(insertSalePricelistRateChange.FromSelect(), processInstanceID);

            var salePricelistRPChangeDataManager = new SalePricelistRPChangeDataManager();
            var salePricelistRPChangeNewDataManager = new SalePricelistRPChangeNewDataManager();
            var insertSalePricelistRPChange = queryContext.AddInsertQuery();
            salePricelistRPChangeDataManager.BuildInsertQuery(insertSalePricelistRPChange);
            salePricelistRPChangeNewDataManager.BuildSelectQuery(insertSalePricelistRPChange.FromSelect(), processInstanceID);

            string saleZoneChangedAlias = "szchanged";
            string saleZoneAlias = "sz";
            var updateSaleZone = queryContext.AddUpdateQuery();
            saleZoneDataManager.BuildUpdateQuery(updateSaleZone, processInstanceID, saleZoneChangedAlias, ChangedSaleZoneDataManager.COL_ProcessInstanceID);
            var changedSaleZoneDataManager = new ChangedSaleZoneDataManager();
            changedSaleZoneDataManager.SetJoinContext(updateSaleZone.Join(saleZoneAlias), saleZoneChangedAlias, saleZoneAlias, SaleZoneDataManager.COL_ID);

            string saleCodeChangedAlias = "scchanged";
            string saleCodeAlias = "sc";
            var updateSaleCode = queryContext.AddUpdateQuery();
            saleCodeDataManager.BuildUpdateQuery(updateSaleCode, processInstanceID, saleCodeChangedAlias, ChangedSaleCodeDataManager.COL_ProcessInstanceID);
            var changedSaleCodeDataManager = new ChangedSaleCodeDataManager();
            changedSaleCodeDataManager.SetJoinContext(updateSaleCode.Join(saleCodeAlias), saleCodeChangedAlias, saleCodeAlias ,SaleCodeDataManager.COL_ID);

            string customerCountryChangedAlias = "ccchanged";
            string customerCountryAlias = "cc";
            var customerCountryDataManager = new CustomerCountryDataManager();
            var updateCustomerCountry = queryContext.AddUpdateQuery();
            customerCountryDataManager.BuildUpdateQuery(updateCustomerCountry, processInstanceID, customerCountryChangedAlias, ChangedCustomerCountryDataManager.COL_ProcessInstanceID);
            var changedCustomerCountryDataManager = new ChangedCustomerCountryDataManager();
            changedCustomerCountryDataManager.SetJoinContext(updateCustomerCountry.Join(customerCountryAlias), customerCountryChangedAlias, customerCountryAlias, CustomerCountryDataManager.COL_ID);

            var saleRateChangedAlias = "srchanged";
            string saleRateAlias = "sr";
            var updateSaleRate = queryContext.AddUpdateQuery();
            saleRateDataManager.BuildUpdateQuery(updateSaleRate, processInstanceID, saleRateChangedAlias, ChangedSaleRateDataManager.COL_ProcessInstanceID);
            var changedSaleRateDataManager = new ChangedSaleRateDataManager();
            changedSaleRateDataManager.SetJoinContext(updateSaleRate.Join(saleRateAlias), saleRateChangedAlias, saleRateAlias, SaleRateDataManager.COL_ID);

            var saleEntityServicesChanged = "seschanged";
            var saleZoneServicesAlias = "szs";
            var saleEntityServiceDataManager = new SaleEntityServiceDataManager();
            var updateSaleEntityService = queryContext.AddUpdateQuery();
            saleEntityServiceDataManager.BuildUpdateQuery(updateSaleEntityService, processInstanceID, saleEntityServicesChanged, ChangedSaleZoneServicesDataManager.COL_ProcessInstanceID);
            var changedSaleZoneServicesDataManager = new ChangedSaleZoneServicesDataManager();
            changedSaleZoneServicesDataManager.SetJoinContext(updateSaleEntityService.Join(saleZoneServicesAlias), saleEntityServicesChanged, saleZoneServicesAlias, SaleEntityServiceDataManager.COL_ID);

            var saleEntityRPChangedAlias = "rpchanged";
            var saleEntityRPAlias = "rp";
            var updateSaleEntityRP = queryContext.AddUpdateQuery();
            saleEntityRoutingProductDataManager.BuildUpdateQuery(updateSaleEntityRP, processInstanceID, saleEntityRPChangedAlias, ChangedSaleZoneRoutingProductsDataManager.COL_ProcessInstanceID);
            var changedSaleZoneRoutingProductsDataManager = new ChangedSaleZoneRoutingProductsDataManager();
            changedSaleZoneRoutingProductsDataManager.SetJoinContext(updateSaleEntityRP.Join(saleEntityRPAlias), saleEntityRPChangedAlias,saleEntityRPAlias, SaleEntityRoutingProductDataManager.COL_ID);

            return queryContext.ExecuteNonQuery(true) > 0;
        }

        public bool CheckCodePreparationState(int sellingNumberPlanId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectAggregates().Count("ColumnCount");
            var where = selectQuery.Where();
            where.EqualsCondition(COL_SellingNumberPlanId).Value(sellingNumberPlanId);
            where.EqualsCondition(COL_Status).Value((int)CodePreparationStatus.Draft);
            return queryContext.ExecuteScalar().IntValue > 0;
        }

        public bool CleanTemporaryTables(long processInstanceId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var changedCustomerCountryDataManager = new ChangedCustomerCountryDataManager();
            changedCustomerCountryDataManager.DeleteRecords(queryContext.AddDeleteQuery(), processInstanceId);

            var changedSaleCodeDataManager = new ChangedSaleCodeDataManager();
            changedSaleCodeDataManager.DeleteRecords(queryContext.AddDeleteQuery(), processInstanceId);

            var newSaleCodeDataManager = new NewSaleCodeDataManager();
            newSaleCodeDataManager.DeleteRecords(queryContext.AddDeleteQuery(), processInstanceId);

            var changedSaleRateDataManager = new ChangedSaleRateDataManager();
            changedSaleRateDataManager.DeleteRecords(queryContext.AddDeleteQuery(), processInstanceId);

            var newSaleRateDataManager = new NewSaleRateDataManager();
            newSaleRateDataManager.DeleteRecords(queryContext.AddDeleteQuery(), processInstanceId);

            var changedSaleZoneDataManager = new ChangedSaleZoneDataManager();
            changedSaleZoneDataManager.DeleteRecords(queryContext.AddDeleteQuery(), processInstanceId);

            var newSaleZoneDataManager = new NewSaleZoneDataManager();
            newSaleZoneDataManager.DeleteRecords(queryContext.AddDeleteQuery(), processInstanceId);

            var newSaleZoneRoutingProductDataManager = new NewSaleZoneRPDataManager();
            newSaleZoneRoutingProductDataManager.DeleteRecords(queryContext.AddDeleteQuery(), processInstanceId);

            var changedSaleZoneRoutingProductsDataManager = new ChangedSaleZoneRoutingProductsDataManager();
            changedSaleZoneRoutingProductsDataManager.DeleteRecords(queryContext.AddDeleteQuery(), processInstanceId);

            var changedSaleZoneServicesDataManager = new ChangedSaleZoneServicesDataManager();
            changedSaleZoneServicesDataManager.DeleteRecords(queryContext.AddDeleteQuery(), processInstanceId);

            var saleCodePreviewDataManager = new SaleCodePreviewDataManager();
            saleCodePreviewDataManager.DeleteRecords(queryContext.AddDeleteQuery(), processInstanceId);

            var saleRatePreviewDataManager = new SaleRatePreviewDataManager();
            saleRatePreviewDataManager.DeleteRecords(queryContext.AddDeleteQuery(), processInstanceId);

            var saleZonePreviewDataManager = new SaleZonePreviewDataManager();
            saleZonePreviewDataManager.DeleteRecords(queryContext.AddDeleteQuery(), processInstanceId);

            var saleZoneRPPreviewDataManager = new SaleZoneRPPreviewDataManager();
            saleZoneRPPreviewDataManager.DeleteRecords(queryContext.AddDeleteQuery(), processInstanceId);

            var salePriceListNewDataManager = new SalePriceListNewDataManager();
            salePriceListNewDataManager.DeleteRecords(queryContext.AddDeleteQuery(), processInstanceId);

            var salePricelistCodeChangeNewDataManager = new SalePricelistCodeChangeNewDataManager();
            salePricelistCodeChangeNewDataManager.DeleteRecords(queryContext.AddDeleteQuery(), processInstanceId);

            var salePricelistCustomerChangeNewDataManager = new SalePricelistCustomerChangeNewDataManager();
            salePricelistCustomerChangeNewDataManager.DeleteRecords(queryContext.AddDeleteQuery(), processInstanceId);

            var salePricelistRateChangeNewDataManager = new SalePricelistRateChangeNewDataManager();
            salePricelistRateChangeNewDataManager.DeleteRecords(queryContext.AddDeleteQuery(), processInstanceId);

            var salePricelistRPChangeNewDataManager = new SalePricelistRPChangeNewDataManager();
            salePricelistRPChangeNewDataManager.DeleteRecords(queryContext.AddDeleteQuery(), processInstanceId);

            return queryContext.ExecuteNonQuery() > 0;
        }

        public Changes GetChanges(int sellingNumberPlanId, CodePreparationStatus status)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().Column(COL_Changes);
            var where = selectQuery.Where();
            where.EqualsCondition(COL_SellingNumberPlanId).Value(sellingNumberPlanId);
            where.EqualsCondition(COL_Status).Value((int)status);
            return queryContext.GetItem(ChangesMapper);
        }

        public bool InsertOrUpdateChanges(int sellingNumberPlanId, Changes changes, CodePreparationStatus status)
        {
            var queryContext1 = new RDBQueryContext(GetDataProvider());
            var insertQuery = queryContext1.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            var ifNotExists = insertQuery.IfNotExists(TABLE_ALIAS);
            ifNotExists.EqualsCondition(COL_SellingNumberPlanId).Value(sellingNumberPlanId);
            ifNotExists.EqualsCondition(COL_Status).Value((int)status);
            insertQuery.Column(COL_SellingNumberPlanId).Value(sellingNumberPlanId);
            if (changes != null)
                insertQuery.Column(COL_Changes).Value(Serializer.Serialize(changes));
            insertQuery.Column(COL_Status).Value((int)status);

            var updateQuery = queryContext1.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            if (changes != null)
                updateQuery.Column(COL_Changes).Value(Serializer.Serialize(changes));
            else
                updateQuery.Column(COL_Changes).Null();
            var where = updateQuery.Where();
            where.EqualsCondition(COL_SellingNumberPlanId).Value(sellingNumberPlanId);
            where.EqualsCondition(COL_Status).Value((int)status);
            return queryContext1.ExecuteNonQuery() > 0;
        
        }

        public bool UpdateCodePreparationStatus(int sellingNumberPlanId, CodePreparationStatus status)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            updateQuery.Column(COL_Status).Value((int)status);
            var where = updateQuery.Where();
            where.EqualsCondition(COL_SellingNumberPlanId).Value(sellingNumberPlanId);
            where.EqualsCondition(COL_Status).Value((int)CodePreparationStatus.Draft);
            return queryContext.ExecuteNonQuery() > 0;
        }
        #endregion
    }
}
