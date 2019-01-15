﻿using Vanrise.Data.RDB;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Entities;
namespace TOne.WhS.BusinessEntity.Data.RDB
{
    public class SalePriceListNewDataManager
    {
        #region RDB
        static string TABLE_NAME = "TOneWhS_BE_SalePriceList_New";
        static string TABLE_ALIAS = "splnew";

        const string COL_OwnerType = "OwnerType";
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

        internal const string COL_ID = "ID";
        internal const string COL_OwnerID = "OwnerID";

        static SalePriceListNewDataManager()
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

        public SalePriceListNew GetSalePriceListNew(long processInstanceId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, 1, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var whereContext = selectQuery.Where();
            whereContext.EqualsCondition(COL_ProcessInstanceID).Value(processInstanceId);

            return queryContext.GetItem(SalePriceListNewMapper);
        }


        public void JoinSalePriceListNew(RDBJoinContext joinContext, string salePriceListTableAlias, string salePriceListTableCol, string originalTableAlias, string originalTableCol)
        {
            //TODO need to Add NoLock on Join Statement
            var joinStatement = joinContext.Join(TABLE_NAME, salePriceListTableAlias);
            joinStatement.JoinType(RDBJoinType.Inner);
            var joinCondition = joinStatement.On();
            joinCondition.EqualsCondition(originalTableAlias, originalTableCol, salePriceListTableAlias, salePriceListTableCol);
        }
        public void BuildSelectQuery(RDBSelectQuery selectQuery, long processInstanceId)
        {
            selectQuery.From(TABLE_NAME, TABLE_ALIAS);
            selectQuery.SelectColumns().Columns(COL_ID, COL_OwnerType, COL_OwnerID, COL_CurrencyID, COL_EffectiveOn,
                COL_FileID, COL_PriceListType, COL_Description);
            selectQuery.Where().EqualsCondition(COL_ProcessInstanceID).Value(processInstanceId);
        }
        #endregion
        #region Mappers
        SalePriceListNew SalePriceListNewMapper(IRDBDataReader reader)
        {
            return new SalePriceListNew
            {
                PriceListId = reader.GetInt(COL_ID),
                OwnerId = reader.GetInt(COL_OwnerID),
                OwnerType = (SalePriceListOwnerType)reader.GetInt(COL_OwnerType),
                CurrencyId = reader.GetInt(COL_CurrencyID),
                EffectiveOn = reader.GetDateTimeWithNullHandling(COL_EffectiveOn),
                PriceListType = (SalePriceListType?)reader.GetIntWithNullHandling(COL_PriceListType),
                SourceId = reader.GetString(COL_SourceID),
                ProcessInstanceId = reader.GetLongWithNullHandling(COL_ProcessInstanceID),
                FileId = reader.GetLongWithNullHandling(COL_FileID),
                CreatedTime = reader.GetDateTimeWithNullHandling(COL_CreatedTime),
                UserId = reader.GetIntWithNullHandling(COL_UserID),
                Description = reader.GetString(COL_Description),
                PricelistStateBackupId = reader.GetNullableLong(COL_PricelistStateBackupID)
            };
        }

        #endregion

    }
}
