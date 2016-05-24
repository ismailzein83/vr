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
        readonly string[] columns = { "Symbol", "Name", "SourceID" };
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
                TableName = MigrationUtils.GetTableName(_Schema, _TableName, _UseTempTables),
                DataFilePath = filePath,
                ColumnNames = columns,
                TabLock = true,
                KeepIdentity = true,
                FieldSeparator = '^',
            };

            InsertBulkToTable(preparedCurrencies as BaseBulkInsertInfo);
        }

        public Dictionary<string, Currency> GetCurrencies(bool useTempTables)
        {
            return GetItemsText("SELECT [ID] ,[Symbol]  ,[Name] ,[SourceID] FROM"
                + MigrationUtils.GetTableName(_Schema, _TableName, useTempTables), CurrencyMapper, cmd => { }).ToDictionary(x => x.SourceId, x => x);
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
