using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class CarrierAccountDBSyncDataManager : BaseSQLDataManager, IDBSyncDataManager
    {
        readonly string[] _Columns = { "NameSuffix", "CarrierProfileID", "AccountType", "SupplierSettings", "CustomerSettings", "CarrierAccountSettings", "SellingNumberPlanID", "SourceID" };
        string _TableName = Vanrise.Common.Utilities.GetEnumDescription(DBTableName.CarrierAccount);
        string _Schema = "TOneWhS_BE";
        bool _UseTempTables;
        public CarrierAccountDBSyncDataManager(bool useTempTables) :
            base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
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
                    wr.WriteLine(String.Format("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}",
                        c.NameSuffix,
                        c.CarrierProfileId,
                        (int)c.AccountType,
                        Vanrise.Common.Serializer.Serialize(c.SupplierSettings),
                        Vanrise.Common.Serializer.Serialize(c.CustomerSettings),
                        Vanrise.Common.Serializer.Serialize(c.CarrierAccountSettings),
                        c.SellingNumberPlanId,
                        c.SourceId
                        ));
                }
                wr.Close();
            }

            Object preparedCarrierAccounts = new BulkInsertInfo
            {
                TableName = MigrationUtils.GetTableName(_Schema, _TableName, _UseTempTables),
                DataFilePath = filePath,
                ColumnNames = _Columns,
                TabLock = true,
                KeepIdentity = true,
                FieldSeparator = '^',
            };

            InsertBulkToTable(preparedCarrierAccounts as BaseBulkInsertInfo);
        }

        public Dictionary<string, CarrierAccount> GetCarrierAccounts(bool useTempTables)
        {
            return GetItemsText("SELECT [ID] ,[NameSuffix]  ,[AccountType] ,[SellingNumberPlanID],[CarrierProfileId],[SourceID] FROM"
                + MigrationUtils.GetTableName(_Schema, _TableName, useTempTables), CarrierAccountMapper, cmd => { }).ToDictionary(x => x.SourceId, x => x);
        }

        private CarrierAccount CarrierAccountMapper(IDataReader reader)
        {
            CarrierAccount carrierAccount = new CarrierAccount
            {
                CarrierAccountId = (int)reader["ID"],
                NameSuffix = reader["NameSuffix"] as string,
                AccountType = (CarrierAccountType)GetReaderValue<int>(reader, "AccountType"),
                SellingNumberPlanId = GetReaderValue<int?>(reader, "SellingNumberPlanID"),
                CarrierProfileId = (int)reader["CarrierProfileId"],
                SourceId = reader["SourceID"] as string,
            };
            return carrierAccount;
        }

        public string GetConnection()
        {
            return base.GetConnectionString();
        }

        public string GetTableName()
        {
            return _TableName;
        }

        public string GetSchema()
        {
            return _Schema;
        }
    }
}
