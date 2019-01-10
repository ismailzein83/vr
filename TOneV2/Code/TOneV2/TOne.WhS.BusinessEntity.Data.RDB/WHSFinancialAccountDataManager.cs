using System;
using Vanrise.Common;
using Vanrise.Data.RDB;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Entities;
namespace TOne.WhS.BusinessEntity.Data.RDB
{
    public class WHSFinancialAccountDataManager : IWHSFinancialAccountDataManager
    {
        #region RDB

        static string TABLE_ALIAS = "fa";
        static string TABLE_NAME = "TOneWhS_BE_FinancialAccount";
        const string COL_ID = "ID";
        const string COL_CarrierProfileId = "CarrierProfileId";
        const string COL_CarrierAccountId = "CarrierAccountId";
        const string COL_FinancialAccountDefinitionId = "FinancialAccountDefinitionId";
        const string COL_FinancialAccountSettings = "FinancialAccountSettings";
        const string COL_BED = "BED";
        const string COL_EED = "EED";
        const string COL_CreatedTime = "CreatedTime";
        const string COL_CreatedBy = "CreatedBy";
        const string COL_LastModifiedBy = "LastModifiedBy";
        const string COL_LastModifiedTime = "LastModifiedTime";


        static WHSFinancialAccountDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>
            {
                {COL_ID, new RDBTableColumnDefinition {DataType = RDBDataType.Int}},
                {COL_CarrierProfileId, new RDBTableColumnDefinition {DataType = RDBDataType.Int}},
                {COL_CarrierAccountId, new RDBTableColumnDefinition {DataType = RDBDataType.Int}},
                {COL_FinancialAccountDefinitionId,new RDBTableColumnDefinition {DataType = RDBDataType.UniqueIdentifier}},
                {COL_FinancialAccountSettings, new RDBTableColumnDefinition {DataType = RDBDataType.NVarchar}},
                {COL_BED, new RDBTableColumnDefinition {DataType = RDBDataType.DateTime}},
                {COL_EED, new RDBTableColumnDefinition {DataType = RDBDataType.DateTime}},
                {COL_CreatedTime, new RDBTableColumnDefinition {DataType = RDBDataType.DateTime}},
                {COL_CreatedBy, new RDBTableColumnDefinition {DataType = RDBDataType.Int}},
                {COL_LastModifiedBy, new RDBTableColumnDefinition {DataType = RDBDataType.Int}},
                {COL_LastModifiedTime, new RDBTableColumnDefinition {DataType = RDBDataType.DateTime}}
            };
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "TOneWhS_BE",
                DBTableName = "FinancialAccount",
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

        #region IWHSFinancialAccountDataManager Members

        public List<WHSFinancialAccount> GetFinancialAccounts()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
            return queryContext.GetItems(FinancialAccountMapper);
        }

        public bool Update(WHSFinancialAccountToEdit financialAccountToEdit)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);

            updateQuery.Column(COL_BED).Value(financialAccountToEdit.BED);

            if (financialAccountToEdit.EED.HasValue)
                updateQuery.Column(COL_EED).Value(financialAccountToEdit.EED.Value);

            if (financialAccountToEdit.LastModifiedBy.HasValue)
                updateQuery.Column(COL_LastModifiedBy).Value(financialAccountToEdit.LastModifiedBy.Value);

            //updateQuery.Column(COL_LastModifiedTime).DateNow();

            updateQuery.Where().EqualsCondition(COL_ID).Value(financialAccountToEdit.FinancialAccountId);

            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool Insert(WHSFinancialAccount financialAccount, out int insertedId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            insertQuery.AddSelectGeneratedId();

            if (financialAccount.CarrierProfileId.HasValue)
                insertQuery.Column(COL_CarrierProfileId).Value(financialAccount.CarrierProfileId.Value);

            if (financialAccount.CarrierAccountId.HasValue)
                insertQuery.Column(COL_CarrierAccountId).Value(financialAccount.CarrierAccountId.Value);

            insertQuery.Column(COL_FinancialAccountDefinitionId).Value(financialAccount.FinancialAccountDefinitionId);

            if (financialAccount.Settings != null)
                insertQuery.Column(COL_FinancialAccountSettings).Value(Serializer.Serialize(financialAccount.Settings));

            insertQuery.Column(COL_BED).Value(financialAccount.BED);

            if (financialAccount.EED.HasValue)
                insertQuery.Column(COL_EED).Value(financialAccount.EED.Value);

            if (financialAccount.CreatedBy.HasValue)
                insertQuery.Column(COL_CreatedBy).Value(financialAccount.CreatedBy.Value);

            if (financialAccount.LastModifiedBy.HasValue)
                insertQuery.Column(COL_LastModifiedBy).Value(financialAccount.LastModifiedBy.Value);

            var returnedValue = queryContext.ExecuteScalar().NullableIntValue;
            if (returnedValue.HasValue)
            {
                insertedId = returnedValue.Value;
                return true;
            }
            insertedId = 0;
            return false;
        }

        public bool AreFinancialAccountsUpdated(ref object updateHandle)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
        }
        #endregion

        #region Mappers

        WHSFinancialAccount FinancialAccountMapper(IRDBDataReader reader)
        {
            return new WHSFinancialAccount
            {
                FinancialAccountId = reader.GetInt(COL_ID),
                CarrierAccountId = reader.GetNullableInt(COL_CarrierAccountId),
                CarrierProfileId = reader.GetNullableInt(COL_CarrierProfileId),
                FinancialAccountDefinitionId = reader.GetGuid(COL_FinancialAccountDefinitionId),
                Settings = Serializer.Deserialize<WHSFinancialAccountSettings>(reader.GetString(COL_FinancialAccountSettings)),
                BED = reader.GetDateTime(COL_BED),
                EED = reader.GetNullableDateTime(COL_EED),
                CreatedTime = reader.GetDateTime(COL_CreatedTime),
                CreatedBy = reader.GetNullableInt(COL_CreatedBy),
                LastModifiedBy = reader.GetNullableInt(COL_LastModifiedBy),
                LastModifiedTime = reader.GetNullableDateTime(COL_LastModifiedTime)
            };
        }

        # endregion
    }
}
