using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class SupplierPriceListDBSyncDataManager : BaseSQLDataManager, IDBSyncDataManager
    {
        readonly string[] columns = { "SupplierID", "CurrencyID", "FileID", "SourceID", "ID" };
        string _TableName = Vanrise.Common.Utilities.GetEnumDescription(DBTableName.SupplierPriceList);
        string _Schema = "TOneWhS_BE";
        bool _UseTempTables;
        public SupplierPriceListDBSyncDataManager(bool useTempTables) :
            base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {
            _UseTempTables = useTempTables;
        }

        public void ApplySupplierPriceListsToTemp(List<SupplierPriceList> supplierPriceList, long startingId)
        {
            string filePath = GetFilePathForBulkInsert();
            using (System.IO.StreamWriter wr = new System.IO.StreamWriter(filePath))
            {
                foreach (var c in supplierPriceList)
                {
                    wr.WriteLine(String.Format("{0}^{1}^{2}^{3}^{4}", c.SupplierId, c.CurrencyId, c.FileId, c.SourceId, startingId++));
                }
                wr.Close();
            }

            Object preparedSupplierPriceLists = new BulkInsertInfo
            {
                TableName = MigrationUtils.GetTableName(_Schema, _TableName, _UseTempTables),
                DataFilePath = filePath,
                ColumnNames = columns,
                TabLock = true,
                KeepIdentity = true,
                FieldSeparator = '^',
            };

            InsertBulkToTable(preparedSupplierPriceLists as BaseBulkInsertInfo);
        }

        public Dictionary<string, SupplierPriceList> GetSupplierPriceLists(bool useTempTables)
        {
            return GetItemsText("SELECT ID,  SupplierID, CurrencyID, FileID, CreatedTime, SourceID FROM"
                + MigrationUtils.GetTableName(_Schema, _TableName, useTempTables), SupplierPriceListMapper, cmd => { }).ToDictionary(x => x.SourceId, x => x);
        }

        public SupplierPriceList SupplierPriceListMapper(IDataReader reader)
        {
            return new SupplierPriceList
            {
                SupplierId = (int)reader["SupplierID"],
                CurrencyId = (int)reader["CurrencyID"],
                PriceListId = (int)reader["ID"],
                FileId = (long)reader["FileID"],
                CreateTime = GetReaderValue<DateTime>(reader, "CreatedTime"),
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
