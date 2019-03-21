using System;
using System.Collections.Generic;
using Vanrise.Data.RDB;
using Vanrise.Entities;
using TOne.WhS.SMSBusinessEntity.Data;
using TOne.WhS.SMSBusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;

namespace TOne.WhS.SMSBusinessEntity.Data.RDB
{
    public class SupplierSMSPriceListDataManager : ISupplierSMSPriceListDataManager
    {
        static string TABLE_NAME = "TOneWhS_SMSBE_SupplierPriceList";
        static string TABLE_ALIAS = "SMSSupplierPriceList";
        const string COL_ID = "ID";
        internal const string COL_SupplierID = "SupplierID";
        const string COL_CurrencyID = "CurrencyID";
        const string COL_EffectiveOn = "EffectiveOn";
        const string COL_ProcessInstanceID = "ProcessInstanceID";
        const string COL_CreatedTime = "CreatedTime";
        const string COL_LastModifiedTime = "LastModifiedTime";
        const string COL_UserID = "UserID";

        static SupplierSMSPriceListDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_SupplierID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_CurrencyID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_EffectiveOn, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_ProcessInstanceID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_UserID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "TOneWhS_SMSBE",
                DBTableName = "SupplierPriceList",
                Columns = columns,
                IdColumnName = COL_ID,
                CreatedTimeColumnName = COL_CreatedTime,
                ModifiedTimeColumnName = COL_LastModifiedTime

            });
        }

        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("TOneWhS_SMSBuisenessEntity", "TOneWhS_SMSBE_DBConnStringKey", "TOneV2SMSDBConnString");
        }

        public List<SupplierSMSPriceList> GetSupplierSMSPriceLists()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS);
            selectQuery.SelectColumns().Columns(COL_ID, COL_SupplierID, COL_CurrencyID, COL_EffectiveOn, COL_ProcessInstanceID, COL_UserID);

            return queryContext.GetItems(SupplierSMSPriceListMapper);
        }

        public void JoinRateTableWithPriceListTable(RDBJoinContext joinContext, string supplierPriceListTableAlias, string otherTableAlias, string otherTableColumn)
        {
            joinContext.JoinOnEqualOtherTableColumn(TABLE_NAME, supplierPriceListTableAlias, COL_ID, otherTableAlias, otherTableColumn);
        }

        public bool AreSupplierSMSPriceListUpdated(ref object updateHandle)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
        }


        public void AddInsertPriceListQueryContext(RDBQueryContext queryContext, SupplierSMSPriceList supplierSMSPriceList)
        {
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            
            insertQuery.Column(COL_ID).Value(supplierSMSPriceList.ID);
            insertQuery.Column(COL_SupplierID).Value(supplierSMSPriceList.SupplierID);
            insertQuery.Column(COL_CurrencyID).Value(supplierSMSPriceList.CurrencyID);
            insertQuery.Column(COL_EffectiveOn).Value(supplierSMSPriceList.EffectiveOn);
            insertQuery.Column(COL_ProcessInstanceID).Value(supplierSMSPriceList.ProcessInstanceID);
            insertQuery.Column(COL_UserID).Value(supplierSMSPriceList.UserID);
        }


        private SupplierSMSPriceList SupplierSMSPriceListMapper(IRDBDataReader dataReader)
        {
            return new SupplierSMSPriceList()
            {
                ID = dataReader.GetLong(COL_ID),
                SupplierID = dataReader.GetInt(COL_SupplierID),
                CurrencyID = dataReader.GetInt(COL_CurrencyID),
                EffectiveOn = dataReader.GetDateTime(COL_EffectiveOn),
                ProcessInstanceID = dataReader.GetLong(COL_ProcessInstanceID),
                UserID = dataReader.GetInt(COL_UserID)
            };
        }
    }
}
