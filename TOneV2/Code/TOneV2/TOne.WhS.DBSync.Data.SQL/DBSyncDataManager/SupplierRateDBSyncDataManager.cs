using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Data.SQL.Common;
using Vanrise.Data.SQL;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class SupplierRateDBSyncDataManager : BaseSQLDataManager
    {
        readonly string[] columns = { "PriceListID", "ZoneID", "CurrencyID", "NormalRate", "OtherRates", "BED", "EED", "SourceID", "ID" };
        string _TableName = Vanrise.Common.Utilities.GetEnumDescription(DBTableName.SupplierRate);
        string _Schema = "TOneWhS_BE";
        bool _UseTempTables;
        public SupplierRateDBSyncDataManager(bool useTempTables) :
            base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {
            _UseTempTables = useTempTables;
        }

        public void ApplySupplierRatesToTemp(List<SupplierRate> supplierRates, long startingId)
        {
            string filePath = GetFilePathForBulkInsert();
            using (System.IO.StreamWriter wr = new System.IO.StreamWriter(filePath))
            {
                foreach (var c in supplierRates)
                {
                    wr.WriteLine(String.Format("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^{8}", c.PriceListId, c.ZoneId, c.CurrencyId, c.NormalRate, Vanrise.Common.Serializer.Serialize(c.OtherRates), c.BED, c.EED, c.SourceId, startingId++));
                }
                wr.Close();
            }

            Object preparedSupplierRates = new BulkInsertInfo
            {
                TableName = MigrationUtils.GetTableName(_Schema, _TableName, _UseTempTables),
                DataFilePath = filePath,
                ColumnNames = columns,
                TabLock = true,
                KeepIdentity = true,
                FieldSeparator = '^',
            };

            InsertBulkToTable(preparedSupplierRates as BaseBulkInsertInfo);
        }

        public Dictionary<string, SupplierRate> GetSupplierRates()
        {
            return GetItemsText("SELECT ID,  PriceListID, ZoneID, CurrencyID, NormalRate, OtherRates, BED, EED, SourceID FROM"
                + MigrationUtils.GetTableName(_Schema, _TableName, _UseTempTables), SupplierRateMapper, cmd => { }).ToDictionary(x => x.SourceId, x => x);
        }

        public SupplierRate SupplierRateMapper(IDataReader reader)
        {
            return new SupplierRate
            {
                NormalRate = GetReaderValue<decimal>(reader, "NormalRate"),
                OtherRates = reader["OtherRates"] as string != null ? Vanrise.Common.Serializer.Deserialize<Dictionary<int, decimal>>(reader["OtherRates"] as string) : null,
                SupplierRateId = (long)reader["ID"],
                ZoneId = (long)reader["ZoneID"],
                PriceListId = GetReaderValue<int>(reader, "PriceListID"),
                BED = GetReaderValue<DateTime>(reader, "BED"),
                EED = GetReaderValue<DateTime?>(reader, "EED"),
                CurrencyId = GetReaderValue<int?>(reader, "CurrencyId"),
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
