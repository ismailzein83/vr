using System;
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
            throw new NotImplementedException();
        }

        public bool Update(CarrierAccountToEdit carrierAccount, int carrierProfileId)
        {
            throw new NotImplementedException();
        }

        public bool AreCarrierAccountsUpdated(ref object updateHandle)
        {
            throw new NotImplementedException();
        }

        public bool UpdateExtendedSettings(int carrierAccountId, Dictionary<string, object> extendedSettings)
        {
            throw new NotImplementedException();
        }
        #endregion
        #region Mapper

        private CarrierAccount CarrierAccountMapper(IRDBDataReader reader)
        {
            return new CarrierAccount
            {
                CarrierAccountId = reader.GetInt("ID"),
                NameSuffix = reader.GetString("NameSuffix"),
                SupplierSettings = Serializer.Deserialize<CarrierAccountSupplierSettings>(reader.GetString("SupplierSettings")),
                CustomerSettings = Serializer.Deserialize<CarrierAccountCustomerSettings>(reader.GetString("CustomerSettings"))
            };
        }

        #endregion
    }
}
