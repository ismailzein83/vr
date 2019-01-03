using System;
using System.Data;
using Vanrise.Common;
using Vanrise.Data.RDB;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data.RDB
{
    public class CarrierAccountDataManager : ICarrierAccountDataManager
    {
        static string TABLE_ALIAS = "ca";
        static string TABLE_NAME = "TOneWhS_BE_CarrierAccount";
        const string COL_ID = "ID";
        const string COL_NameSuffix = "NameSuffix";
        const string COL_CarrierProfileID = "CarrierProfileID";
        const string COL_AccountType = "AccountType";
        const string COL_SupplierSettings = "SupplierSettings";
        const string COL_CustomerSettings = "CustomerSettings";
        const string COL_CarrierAccountSettings = "CarrierAccountSettings";
        const string COL_ExtendedSettings = "ExtendedSettings";
        const string COL_SellingNumberPlanID = "SellingNumberPlanID";
        const string COL_SellingProductID = "SellingProductID";
        const string COL_SourceID = "SourceID";
        const string COL_IsDeleted = "IsDeleted";
        const string COL_CreatedTime = "CreatedTime";
        const string COL_CreatedBy = "CreatedBy";
        const string COL_LastModifiedBy = "LastModifiedBy";
        const string COL_LastModifiedTime = "LastModifiedTime";


        static CarrierAccountDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>
            {
                {COL_ID, new RDBTableColumnDefinition {DataType = RDBDataType.Int}},
                {COL_NameSuffix, new RDBTableColumnDefinition {DataType = RDBDataType.NVarchar, Size = 255}},
                {COL_CarrierProfileID, new RDBTableColumnDefinition {DataType = RDBDataType.Int}},
                {COL_AccountType, new RDBTableColumnDefinition {DataType = RDBDataType.Int}},
                {COL_SupplierSettings, new RDBTableColumnDefinition {DataType = RDBDataType.NVarchar}},
                {COL_CustomerSettings, new RDBTableColumnDefinition {DataType = RDBDataType.NVarchar}},
                {COL_CarrierAccountSettings, new RDBTableColumnDefinition {DataType = RDBDataType.NVarchar}},
                {COL_ExtendedSettings, new RDBTableColumnDefinition {DataType = RDBDataType.NVarchar}},
                {COL_SellingNumberPlanID, new RDBTableColumnDefinition {DataType = RDBDataType.Int}},
                {COL_SellingProductID, new RDBTableColumnDefinition {DataType = RDBDataType.Int}},
                {COL_SourceID, new RDBTableColumnDefinition {DataType = RDBDataType.Varchar, Size = 50}},
                {COL_IsDeleted, new RDBTableColumnDefinition {DataType = RDBDataType.Boolean}},
                {COL_CreatedTime, new RDBTableColumnDefinition {DataType = RDBDataType.DateTime}},
                {COL_CreatedBy, new RDBTableColumnDefinition {DataType = RDBDataType.Int}},
                {COL_LastModifiedBy, new RDBTableColumnDefinition {DataType = RDBDataType.Int}},
                {COL_LastModifiedTime, new RDBTableColumnDefinition {DataType = RDBDataType.DateTime}}
            };

            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "TOneWhS_BE",
                DBTableName = "CarrierAccount",
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

        #region ICarrierAccountDataManager Members
        public List<CarrierAccount> GetCarrierAccounts()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
            return queryContext.GetItems(CarrierAccountMapper);
        }

        public bool Insert(CarrierAccount carrierAccount, out int carrierAccountId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            insertQuery.AddSelectGeneratedId();

            var notExistsCondition = insertQuery.IfNotExists(TABLE_ALIAS);
            notExistsCondition.EqualsCondition(COL_NameSuffix).Value(carrierAccount.NameSuffix);
            notExistsCondition.EqualsCondition(COL_CarrierProfileID).Value(carrierAccount.CarrierProfileId);
            notExistsCondition.EqualsCondition(COL_IsDeleted).Value(0);

            insertQuery.Column(COL_NameSuffix).Value(carrierAccount.NameSuffix);
            insertQuery.Column(COL_CarrierProfileID).Value(carrierAccount.CarrierProfileId);
            insertQuery.Column(COL_AccountType).Value((int)carrierAccount.AccountType);

            if (carrierAccount.SupplierSettings != null)
                insertQuery.Column(COL_SupplierSettings).Value(Serializer.Serialize(carrierAccount.SupplierSettings));

            if (carrierAccount.CustomerSettings != null)
                insertQuery.Column(COL_CustomerSettings).Value(Serializer.Serialize(carrierAccount.CustomerSettings));

            if (carrierAccount.CarrierAccountSettings != null)
                insertQuery.Column(COL_CarrierAccountSettings).Value(Serializer.Serialize(carrierAccount.CarrierAccountSettings));

            if (carrierAccount.SellingNumberPlanId.HasValue)
                insertQuery.Column(COL_SellingNumberPlanID).Value(carrierAccount.SellingNumberPlanId.Value);

            if (carrierAccount.SellingProductId.HasValue)
                insertQuery.Column(COL_SellingProductID).Value(carrierAccount.SellingProductId.Value);

            if (carrierAccount.CreatedBy.HasValue)
                insertQuery.Column(COL_CreatedBy).Value(carrierAccount.CreatedBy.Value);

            if (carrierAccount.LastModifiedBy.HasValue)
                insertQuery.Column(COL_LastModifiedBy).Value(carrierAccount.LastModifiedBy.Value);

            var returnedValue = queryContext.ExecuteScalar().NullableIntValue;
            if (returnedValue.HasValue)
            {
                carrierAccountId = returnedValue.Value;
                return true;
            }
            carrierAccountId = 0;
            return false;
        }

        public bool Update(CarrierAccountToEdit carrierAccount, int carrierProfileId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);

            var notExistsCondition = updateQuery.IfNotExists(TABLE_ALIAS);
            notExistsCondition.NotEqualsCondition(COL_ID).Value(carrierAccount.CarrierAccountId);
            notExistsCondition.EqualsCondition(COL_NameSuffix).Value(carrierAccount.NameSuffix);
            notExistsCondition.EqualsCondition(COL_CarrierProfileID).Value(carrierProfileId);
            notExistsCondition.EqualsCondition(COL_IsDeleted).Value(false);

            updateQuery.Column(COL_NameSuffix).Value(carrierAccount.NameSuffix);

            if (carrierAccount.SellingProductId.HasValue)
                updateQuery.Column(COL_SellingProductID).Value(carrierAccount.SellingProductId.Value);

            if (carrierAccount.SupplierSettings != null)
                updateQuery.Column(COL_SupplierSettings).Value(Serializer.Serialize(carrierAccount.SupplierSettings));

            if (carrierAccount.CustomerSettings != null)
                updateQuery.Column(COL_CustomerSettings).Value(Serializer.Serialize(carrierAccount.CustomerSettings));

            if (carrierAccount.CarrierAccountSettings != null)
                updateQuery.Column(COL_CarrierAccountSettings).Value(Serializer.Serialize(carrierAccount.CarrierAccountSettings));

            if (carrierAccount.LastModifiedBy.HasValue)
                updateQuery.Column(COL_LastModifiedBy).Value(carrierAccount.LastModifiedBy.Value);

            updateQuery.Column(COL_LastModifiedTime).DateNow();

            updateQuery.Where().EqualsCondition(COL_ID).Value(carrierAccount.CarrierAccountId);

            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool AreCarrierAccountsUpdated(ref object updateHandle)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
        }

        public bool UpdateExtendedSettings(int carrierAccountId, Dictionary<string, object> extendedSettings)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);

            if (extendedSettings != null && extendedSettings.Count > 0)
                updateQuery.Column(COL_ExtendedSettings).Value(Serializer.Serialize(extendedSettings));
            else
                updateQuery.Column(COL_ExtendedSettings).Null();

            updateQuery.Where().EqualsCondition(COL_ID).Value(carrierAccountId);
            return queryContext.ExecuteNonQuery() > 0;
        }
        #endregion

        #region Mapper

        private CarrierAccount CarrierAccountMapper(IRDBDataReader reader)
        {
            return new CarrierAccount
            {
                CarrierAccountId = reader.GetInt("ID"),
                CarrierProfileId = reader.GetInt("CarrierProfileId"),
                NameSuffix = reader.GetString("NameSuffix"),
                SourceId = reader.GetString("SourceID"),
                AccountType = (CarrierAccountType)reader.GetInt("AccountType"),
                SellingNumberPlanId = reader.GetNullableInt("SellingNumberPlanID"),
                SellingProductId = reader.GetNullableInt("SellingProductId"),
                CreatedBy = reader.GetNullableInt("CreatedBy"),
                LastModifiedBy = reader.GetNullableInt("LastModifiedBy"),
                LastModifiedTime = reader.GetNullableDateTime("LastModifiedTime"),
                IsDeleted = reader.GetBooleanWithNullHandling("IsDeleted"),
                CreatedTime = reader.GetDateTimeWithNullHandling("CreatedTime"),
                SupplierSettings = Serializer.Deserialize<CarrierAccountSupplierSettings>(reader.GetString("SupplierSettings")),
                CustomerSettings = Serializer.Deserialize<CarrierAccountCustomerSettings>(reader.GetString("CustomerSettings")),
                CarrierAccountSettings = Serializer.Deserialize<CarrierAccountSettings>(reader.GetString("CarrierAccountSettings")),
                ExtendedSettings = Serializer.Deserialize<Dictionary<string, Object>>(reader.GetString("ExtendedSettings"))
            };
        }

        #endregion
    }
}
