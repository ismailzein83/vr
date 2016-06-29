﻿using System;
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
            DataTable dt = new DataTable();
            dt.TableName = MigrationUtils.GetTableName(_Schema, _TableName, _UseTempTables);
            dt.Columns.Add("NameSuffix", typeof(string));
            dt.Columns.Add("CarrierProfileID", typeof(int));
            dt.Columns.Add("AccountType", typeof(int));
            dt.Columns.Add("SupplierSettings", typeof(string));
            dt.Columns.Add("CustomerSettings", typeof(string));
            dt.Columns.Add("CarrierAccountSettings", typeof(string));
            dt.Columns.Add("SellingNumberPlanID", typeof(int));
            dt.Columns.Add("SourceID", typeof(string));

            dt.BeginLoadData();
            foreach (var item in carrierAccounts)
            {
                DataRow row = dt.NewRow();
                int index = 0;
                row[index++] = item.NameSuffix;
                row[index++] = item.CarrierProfileId;
                row[index++] = (int)item.AccountType;
                row[index++] = Vanrise.Common.Serializer.Serialize(item.SupplierSettings);
                row[index++] = Vanrise.Common.Serializer.Serialize(item.CustomerSettings);
                row[index++] = Vanrise.Common.Serializer.Serialize(item.CarrierAccountSettings);
                row[index++] = item.SellingNumberPlanId.HasValue ? (object)item.SellingNumberPlanId.Value : DBNull.Value;
                row[index++] = item.SourceId;
                dt.Rows.Add(row);
            }
            dt.EndLoadData();
            WriteDataTableToDB(dt, System.Data.SqlClient.SqlBulkCopyOptions.KeepNulls);
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
