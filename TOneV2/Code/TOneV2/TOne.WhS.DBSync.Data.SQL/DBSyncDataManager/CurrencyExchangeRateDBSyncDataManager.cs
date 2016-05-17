using System;
using System.Collections.Generic;
using TOne.WhS.DBSync.Data.SQL.Common;
using Vanrise.Data.SQL;
using Vanrise.Entities;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class CurrencyExchangeRateDBSyncDataManager : BaseSQLDataManager
    {
        readonly string[] columns = { "CurrencyID", "Rate", "ExchangeDate", "SourceID" };
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
            string filePath = GetFilePathForBulkInsert();
            using (System.IO.StreamWriter wr = new System.IO.StreamWriter(filePath))
            {
                foreach (var c in currencyExchangeRates)
                {
                    wr.WriteLine(String.Format("{0}^{1}^{2}^{3}", c.CurrencyId, c.Rate, c.ExchangeDate, c.SourceId));
                }
                wr.Close();
            }

            Object preparedCurrencyExchangeRates = new BulkInsertInfo
            {
                TableName = MigrationUtils.GetTableName(_Schema, _TableName, _UseTempTables),
                DataFilePath = filePath,
                ColumnNames = columns,
                TabLock = true,
                KeepIdentity = true,
                FieldSeparator = '^',
            };

            InsertBulkToTable(preparedCurrencyExchangeRates as BaseBulkInsertInfo);
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
