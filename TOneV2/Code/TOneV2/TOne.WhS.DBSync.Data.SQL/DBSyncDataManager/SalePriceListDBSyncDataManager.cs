﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class SalePriceListDBSyncDataManager : BaseSQLDataManager, IDBSyncDataManager
    {
        string _TableName = Vanrise.Common.Utilities.GetEnumDescription(DBTableName.SalePriceList);
        string _Schema = "TOneWhS_BE";
        bool _UseTempTables;
        public SalePriceListDBSyncDataManager(bool useTempTables) :
            base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {
            _UseTempTables = useTempTables;
        }

        public void ApplySalePriceListsToTemp(List<SalePriceList> salePriceLists)
        {
            DataTable dt = new DataTable();
            dt.TableName = MigrationUtils.GetTableName(_Schema, _TableName, _UseTempTables);
            dt.Columns.Add("OwnerType", typeof(int));
            dt.Columns.Add("OwnerID", typeof(int));
            dt.Columns.Add("CurrencyID", typeof(int));
            dt.Columns.Add("SourceID", typeof(string));
            dt.Columns.Add("EffectiveOn", typeof(DateTime));
            dt.BeginLoadData();
            foreach (var item in salePriceLists)
            {
                DataRow row = dt.NewRow();
                int index = 0;
                row[index++] = (int)item.OwnerType;
                row[index++] = item.OwnerId;
                row[index++] = item.CurrencyId;
                row[index++] = item.SourceId;
                row[index++] = item.EffectiveOn;
                dt.Rows.Add(row);
            }
            dt.EndLoadData();
            WriteDataTableToDB(dt, System.Data.SqlClient.SqlBulkCopyOptions.KeepNulls);
        }

        public Dictionary<string, SalePriceList> GetSalePriceLists(bool useTempTables)
        {
            return GetItemsText("SELECT ID,  OwnerType, OwnerID, CurrencyID, EffectiveOn, SourceID FROM"
                + MigrationUtils.GetTableName(_Schema, _TableName, useTempTables), SalePriceListMapper, cmd => { }).ToDictionary(x => x.SourceId, x => x);
        }

        public SalePriceList SalePriceListMapper(IDataReader reader)
        {
            return new SalePriceList
            {
                OwnerId = (int)reader["OwnerID"],
                CurrencyId = (int)reader["CurrencyID"],
                PriceListId = (int)reader["ID"],
                OwnerType = (SalePriceListOwnerType)GetReaderValue<int>(reader, "OwnerType"),
                SourceId = reader["SourceID"] as string,
                EffectiveOn = GetReaderValue<DateTime>(reader, "EffectiveOn")
            };
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
