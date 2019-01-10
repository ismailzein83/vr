using System;
using Vanrise.Data.RDB;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Entities;
namespace TOne.WhS.BusinessEntity.Data.RDB
{
    public class SupplierPriceListDataManager : ISupplierPriceListDataManager
    {
        #region RDB

        static string TABLE_ALIAS = "spr";
        static string TABLE_NAME = "TOneWhS_BE_SupplierPriceList";
        const string COL_ID = "ID";
        const string COL_SupplierID = "SupplierID";
        const string COL_CurrencyID = "CurrencyID";
        const string COL_FileID = "FileID";
        const string COL_EffectiveOn = "EffectiveOn";
        const string COL_PricelistType = "PricelistType";
        const string COL_CreatedTime = "CreatedTime";
        const string COL_SourceID = "SourceID";
        const string COL_ProcessInstanceID = "ProcessInstanceID";
        const string COL_SPLStateBackupID = "SPLStateBackupID";
        const string COL_UserID = "UserID";
        const string COL_LastModifiedTime = "LastModifiedTime";


        static SupplierPriceListDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>
            {
                {COL_ID, new RDBTableColumnDefinition {DataType = RDBDataType.Int}},
                {COL_SupplierID, new RDBTableColumnDefinition {DataType = RDBDataType.Int}},
                {COL_CurrencyID, new RDBTableColumnDefinition {DataType = RDBDataType.Int}},
                {COL_FileID, new RDBTableColumnDefinition {DataType = RDBDataType.BigInt}},
                {COL_EffectiveOn, new RDBTableColumnDefinition {DataType = RDBDataType.DateTime}},
                {COL_PricelistType, new RDBTableColumnDefinition {DataType = RDBDataType.Int}},
                {COL_CreatedTime, new RDBTableColumnDefinition {DataType = RDBDataType.DateTime}},
                {COL_SourceID, new RDBTableColumnDefinition {DataType = RDBDataType.Varchar, Size = 50}},
                {COL_ProcessInstanceID, new RDBTableColumnDefinition {DataType = RDBDataType.BigInt}},
                {COL_SPLStateBackupID, new RDBTableColumnDefinition {DataType = RDBDataType.BigInt}},
                {COL_UserID, new RDBTableColumnDefinition {DataType = RDBDataType.Int}},
                {COL_LastModifiedTime, new RDBTableColumnDefinition {DataType = RDBDataType.DateTime}}
            };
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "TOneWhS_BE",
                DBTableName = "SupplierPriceList",
                Columns = columns,
                IdColumnName = COL_ID,
                CreatedTimeColumnName = COL_CreatedTime,
                ModifiedTimeColumnName = COL_LastModifiedTime

            });
        }

        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("TOneWhS_BE", "TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString");
        }

        #endregion

        #region ISupplierPriceListDataManager Members

        public SupplierPriceList GetPriceList(int priceListId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var whereQuery = selectQuery.Where();
            whereQuery.EqualsCondition(COL_ID).Value(priceListId);
            return queryContext.GetItem(SupplierPriceListMapper);
        }

        public List<SupplierPriceList> GetPriceLists()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
            return queryContext.GetItems(SupplierPriceListMapper);
        }

        public bool ArGetPriceListsUpdated(ref object updateHandle)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
        }

        #endregion

        #region StateBackup

        //TODO Not yet implemented

        #endregion

        #region Mappers
        SupplierPriceList SupplierPriceListMapper(IRDBDataReader reader)
        {
            return new SupplierPriceList
            {
                SupplierId = reader.GetInt(COL_SupplierID),
                CurrencyId = reader.GetInt(COL_CurrencyID),
                PriceListId = reader.GetInt(COL_ID),
                FileId = reader.GetNullableLong(COL_FileID),
                CreateTime = reader.GetDateTimeWithNullHandling(COL_CreatedTime),
                ProcessInstanceId = reader.GetNullableLong(COL_ProcessInstanceID),
                SPLStateBackupId = reader.GetNullableLong(COL_SPLStateBackupID),
                UserId = reader.GetInt(COL_UserID),
                PricelistType = (SupplierPricelistType?)reader.GetNullableInt(COL_PricelistType),
                EffectiveOn = reader.GetDateTimeWithNullHandling(COL_EffectiveOn)
            };
        }

        #endregion

        #region

        public void JoinSupplierPriceList(RDBJoinContext joinContext, string priceListTableAlias, string originalTableAlias, string originalTablePriceListIdCol)
        {
            var joinStatement = joinContext.Join(TABLE_NAME, priceListTableAlias);
            joinStatement.JoinType(RDBJoinType.Inner);
            var joinCondition = joinStatement.On();
            joinCondition.EqualsCondition(originalTableAlias, originalTablePriceListIdCol, priceListTableAlias, COL_ID);
        }
        #endregion
    }
}
