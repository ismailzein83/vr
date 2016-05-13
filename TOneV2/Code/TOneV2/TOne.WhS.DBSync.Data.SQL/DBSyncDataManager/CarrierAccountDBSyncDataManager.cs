using System;
using System.Collections.Generic;
using System.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class CarrierAccountDBSyncDataManager : BaseSQLDataManager
    {
        readonly string[] columns = { "NameSuffix", "CarrierProfileId", "SellingNumberPlanId", "AccountType", "CarrierAccountSettings", "SupplierSettings", "CustomerSettings" };

        bool _UseTempTables;
        public CarrierAccountDBSyncDataManager(bool useTempTables) :
            base(GetConnectionStringName("TOneWhS_BE_MigrationDBConnStringKey", "TOneV2MigrationDBConnString"))
        {
            _UseTempTables = useTempTables;
        }

        public void ApplyCarrierAccountsToTemp(List<CarrierAccount> carrierAccounts)
        {
            string filePath = GetFilePathForBulkInsert();
            using (System.IO.StreamWriter wr = new System.IO.StreamWriter(filePath))
            {
                foreach (var c in carrierAccounts)
                {
                    wr.WriteLine(String.Format("{0}^{1}^{2}^{3}^{4}^{5}", c.CarrierProfileId, c.SellingNumberPlanId, c.AccountType, c.CarrierAccountSettings, c.SupplierSettings, c.CustomerSettings));
                }
                wr.Close();
            }

            Object preparedCarrierAccounts = new BulkInsertInfo
            {
                TableName = "[TOneWhS_BE].[CarrierAccount" + (_UseTempTables ? Constants._Temp : "") + "]",
                DataFilePath = filePath,
                ColumnNames = columns,
                TabLock = true,
                KeepIdentity = true,
                FieldSeparator = '^',
            };

            InsertBulkToTable(preparedCarrierAccounts as BaseBulkInsertInfo);
        }

        public List<CarrierAccount> GetCarrierAccounts()
        {
            return GetItemsText("SELECT [ID] ,[Settings]  ,[Name] ,[SourceID] FROM [TOneWhS_BE].[CarrierAccount" + (_UseTempTables ? Constants._Temp : "") + "] ", CarrierAccountMapper, cmd => { });
        }

        private CarrierAccount CarrierAccountMapper(IDataReader reader)
        {
            CarrierAccount carrierAccount = new CarrierAccount
            {
                CarrierAccountId = (int)reader["ID"],
                NameSuffix = reader["NameSuffix"] as string,
                AccountType = (CarrierAccountType)GetReaderValue<int>(reader, "AccountType"),
                SupplierSettings = Vanrise.Common.Serializer.Deserialize<CarrierAccountSupplierSettings>(reader["SupplierSettings"] as string),
                CustomerSettings = Vanrise.Common.Serializer.Deserialize<CarrierAccountCustomerSettings>(reader["CustomerSettings"] as string),
                SellingNumberPlanId = GetReaderValue<int?>(reader, "SellingNumberPlanID"),
                CarrierProfileId = (int)reader["CarrierProfileId"],
                CarrierAccountSettings = Vanrise.Common.Serializer.Deserialize<CarrierAccountSettings>(reader["CarrierAccountSettings"] as string),

            };
            return carrierAccount;
        }

    }
}
