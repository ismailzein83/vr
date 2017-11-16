using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TOne.WhS.DBSync.Entities;
using Vanrise.Data.SQL;
using Vanrise.Entities;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class CurrencyDBSyncDataManager : BaseSQLDataManager, IDBSyncDataManager
    {
        string _TableName = Vanrise.Common.Utilities.GetEnumDescription(DBTableName.Currency);
        string _Schema = "Common";
        bool _UseTempTables;
        public CurrencyDBSyncDataManager(bool useTempTables) :
            base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnString"))
        {
            _UseTempTables = useTempTables;
        }

        public void ApplyCurrenciesToTemp(List<Currency> currencies)
        {
            DataTable dt = new DataTable();
            dt.TableName = MigrationUtils.GetTableName(_Schema, _TableName, _UseTempTables);
            dt.Columns.Add("Symbol", typeof(string));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("SourceID", typeof(string));

            dt.BeginLoadData();
            foreach (var item in currencies)
            {
                DataRow row = dt.NewRow();
                int index = 0;
                row[index++] = item.Symbol;
                row[index++] = item.Name;
                row[index++] = item.SourceId;
                dt.Rows.Add(row);
            }
            dt.EndLoadData();
            WriteDataTableToDB(dt, System.Data.SqlClient.SqlBulkCopyOptions.KeepNulls);
        }

        public Dictionary<string, Currency> GetCurrencies(bool useTempTables)
        {
            return GetItemsText(string.Format("SELECT [ID] ,[Symbol]  ,[Name] ,[SourceID] FROM {0} where SourceID is not null"
                , MigrationUtils.GetTableName(_Schema, _TableName, useTempTables)), CurrencyMapper, cmd => { }).ToDictionary(x => x.SourceId, x => x);
        }

        public Currency CurrencyMapper(IDataReader reader)
        {
            return new Currency
            {
                CurrencyId = (int)reader["ID"],
                Symbol = reader["Symbol"] as string,
                Name = reader["Name"] as string,
                SourceId = reader["SourceID"] as string,
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
