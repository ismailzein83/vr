using System;
using Vanrise.Common;
using Vanrise.Data.RDB;
using System.Collections.Generic;
using Vanrise.Entities;
namespace TOne.WhS.BusinessEntity.Data.RDB
{
    public class SalePriceLisNewDataManager
    {
        #region RDB
        static string TABLE_NAME = "TOneWhS_BE_SalePriceList_New";
        const string COL_ID = "ID";
        const string COL_OwnerType = "OwnerType";
        const string COL_OwnerID = "OwnerID";
        const string COL_CurrencyID = "CurrencyID";
        const string COL_EffectiveOn = "EffectiveOn";
        const string COL_PriceListType = "PriceListType";
        const string COL_SourceID = "SourceID";
        const string COL_ProcessInstanceID = "ProcessInstanceID";
        const string COL_FileID = "FileID";
        const string COL_CreatedTime = "CreatedTime";
        const string COL_IsSent = "IsSent";
        const string COL_UserID = "UserID";
        const string COL_Description = "Description";
        const string COL_PricelistStateBackupID = "PricelistStateBackupID";
        const string COL_LastModifiedTime = "LastModifiedTime";


        static SalePriceLisNewDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>
            {
                {COL_ID, new RDBTableColumnDefinition {DataType = RDBDataType.Int}},
                {COL_OwnerType, new RDBTableColumnDefinition {DataType = RDBDataType.Int}},
                {COL_OwnerID, new RDBTableColumnDefinition {DataType = RDBDataType.Int}},
                {COL_CurrencyID, new RDBTableColumnDefinition {DataType = RDBDataType.Int}},
                {COL_EffectiveOn, new RDBTableColumnDefinition {DataType = RDBDataType.DateTime}},
                {COL_PriceListType, new RDBTableColumnDefinition {DataType = RDBDataType.Int}},
                {COL_SourceID, new RDBTableColumnDefinition {DataType = RDBDataType.Varchar, Size = 50}},
                {COL_ProcessInstanceID, new RDBTableColumnDefinition {DataType = RDBDataType.BigInt}},
                {COL_FileID, new RDBTableColumnDefinition {DataType = RDBDataType.BigInt}},
                {COL_CreatedTime, new RDBTableColumnDefinition {DataType = RDBDataType.DateTime}},
                {COL_IsSent, new RDBTableColumnDefinition {DataType = RDBDataType.Boolean}},
                {COL_UserID, new RDBTableColumnDefinition {DataType = RDBDataType.Int}},
                {COL_Description, new RDBTableColumnDefinition {DataType = RDBDataType.NVarchar}},
                {COL_PricelistStateBackupID, new RDBTableColumnDefinition {DataType = RDBDataType.BigInt}},
                {COL_LastModifiedTime, new RDBTableColumnDefinition {DataType = RDBDataType.DateTime}}
            };
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "TOneWhS_BE",
                DBTableName = "SalePriceList_New",
                Columns = columns,
                CreatedTimeColumnName = COL_CreatedTime,
                ModifiedTimeColumnName = COL_LastModifiedTime

            });
        }
        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("TOneWhS_BE", "TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString");
        }


        #endregion

        #region Public Methods

        public void JoinSalePriceListNew(RDBJoinContext joinContext, string salePriceListTableAlias, string salePriceListTableCol, string originalTableAlias, string originalTableCol)
        {
            //TODO need to Add NoLock on Join Statement
            var joinStatement = joinContext.Join(TABLE_NAME, salePriceListTableAlias);
            joinStatement.JoinType(RDBJoinType.Inner);
            var joinCondition = joinStatement.On();
            joinCondition.EqualsCondition(originalTableAlias, originalTableCol, salePriceListTableAlias, salePriceListTableCol);
        }

        #endregion
    }
}
