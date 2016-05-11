using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Data.SQL;
using Vanrise.Entities;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class CurrencyDataManager : BaseSQLDataManager
    {
        readonly string[] columns = { "Symbol", "Name", "SourceID" };
        string _tempTableName;
        public CurrencyDataManager(string tableName) :
            base(GetConnectionStringName("ConfigurationMigrationDBConnStringKey", "ConfigurationMigrationDBConnString"))
        {
            _tempTableName = tableName;
        }

        public void ApplyCurrenciesToDB(List<Currency> currencies)
        {
            string filePath = GetFilePathForBulkInsert();
            using (System.IO.StreamWriter wr = new System.IO.StreamWriter(filePath))
            {
                foreach (var c in currencies)
                {
                    wr.WriteLine(String.Format("{0}^{1}^{2}", c.Symbol, c.Name, c.SourceID));
                }
                wr.Close();
            }

            Object preparedCurrencies = new BulkInsertInfo
            {
                TableName = _tempTableName,
                DataFilePath = filePath,
                ColumnNames = columns,
                TabLock = true,
                KeepIdentity = true,
                FieldSeparator = '^',
            };

            InsertBulkToTable(preparedCurrencies as BaseBulkInsertInfo);
        }
    }
}
