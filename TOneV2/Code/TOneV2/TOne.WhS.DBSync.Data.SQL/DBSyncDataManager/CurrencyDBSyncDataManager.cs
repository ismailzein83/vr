using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Vanrise.Data.SQL;
using Vanrise.Entities;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class CurrencyDBSyncDataManager : BaseSQLDataManager
    {
        readonly string[] columns = { "Symbol", "Name", "SourceID" };
        bool _UseTempTables;
        public CurrencyDBSyncDataManager(bool useTempTables) :
            base(GetConnectionStringName("ConfigurationMigrationDBConnStringKey", "ConfigurationMigrationDBConnString"))
        {
            _UseTempTables = useTempTables;
        }

        public void ApplyCurrenciesToTemp(List<Currency> currencies)
        {
            string filePath = GetFilePathForBulkInsert();
            using (System.IO.StreamWriter wr = new System.IO.StreamWriter(filePath))
            {
                foreach (var c in currencies)
                {
                    wr.WriteLine(String.Format("{0}^{1}^{2}", c.Symbol, c.Name, c.SourceId));
                }
                wr.Close();
            }

            Object preparedCurrencies = new BulkInsertInfo
            {
                TableName = "[common].[Currency" + (_UseTempTables ? Constants._Temp : "") + "]",
                DataFilePath = filePath,
                ColumnNames = columns,
                TabLock = true,
                KeepIdentity = true,
                FieldSeparator = '^',
            };

            InsertBulkToTable(preparedCurrencies as BaseBulkInsertInfo);
        }

        public List<Currency> GetCurrencies()
        {
            return GetItemsText("SELECT [ID] ,[Symbol]  ,[Name] ,[SourceID] FROM [common].[Currency" + (_UseTempTables ? Constants._Temp : "") + "] ", CurrencyMapper, cmd => { });
        }

        public Currency CurrencyMapper(IDataReader reader)
        {
            Currency currency = new Currency
            {
                CurrencyId = (int)reader["ID"],
                Symbol = reader["Symbol"] as string,
                Name = reader["Name"] as string,
                SourceId = reader["SourceID"] as string,
            };

            return currency;
        }

    }
}
