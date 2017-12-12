using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TOne.WhS.DBSync.Entities;
using Vanrise.Data.SQL;
using Vanrise.Entities;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class CurrencyExchangeRateDBSyncDataManager : BaseSQLDataManager, IDBSyncDataManager
    {
        string _TableName = Vanrise.Common.Utilities.GetEnumDescription(DBTableName.CurrencyExchangeRate);
        string _Schema = "Common";
        bool _UseTempTables;
        public CurrencyExchangeRateDBSyncDataManager(bool useTempTables) :
            base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnString"))
        {
            _UseTempTables = useTempTables;
        }

        public void ApplyCurrencyExchangeRatesToTemp(List<CurrencyExchangeRate> currencyExchangeRates)
        {
            DataTable dt = new DataTable();
            dt.TableName = MigrationUtils.GetTableName(_Schema, _TableName, _UseTempTables);
            dt.Columns.Add("CurrencyID", typeof(int));
            dt.Columns.Add("Rate", typeof(decimal));
            dt.Columns.Add("ExchangeDate", typeof(DateTime));
            dt.Columns.Add("SourceID", typeof(string));

            dt.BeginLoadData();
            foreach (var item in currencyExchangeRates)
            {
                DataRow row = dt.NewRow();
                int index = 0;
                row[index++] = item.CurrencyId;
                row[index++] = decimal.Round(item.Rate, 10, MidpointRounding.AwayFromZero);
                row[index++] = item.ExchangeDate;
                row[index++] = item.SourceId;
                dt.Rows.Add(row);
            }
            dt.EndLoadData();
            WriteDataTableToDB(dt, System.Data.SqlClient.SqlBulkCopyOptions.KeepNulls);
        }

        public Dictionary<string, CurrencyExchangeRate> GetCurrencyExchangeRates(bool useTempTables)
        {
            return GetItemsText(string.Format("SELECT [ID],[CurrencyID] ,[Rate]  ,[ExchangeDate]  ,[SourceID] FROM {0} where sourceid is not null ",
                MigrationUtils.GetTableName(_Schema, _TableName, useTempTables)), CurrencyExchangeRateMapper, cmd => { }).ToDictionary(x => x.SourceId, x => x);
        }

        public CurrencyExchangeRate CurrencyExchangeRateMapper(IDataReader reader)
        {
            return new CurrencyExchangeRate
            {
                CurrencyExchangeRateId = (long)reader["ID"],
                CurrencyId = (int)reader["CurrencyId"],
                Rate = GetReaderValue<decimal>(reader, "Rate"),
                ExchangeDate = GetReaderValue<DateTime>(reader, "ExchangeDate"),
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
