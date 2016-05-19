using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using TOne.WhS.DBSync.Data.SQL.Common;
using Vanrise.Data.SQL;
using Vanrise.Entities;
using System.Linq;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class SaleRateDBSyncDataManager : BaseSQLDataManager
    {
        readonly string[] columns = { "PriceListID", "ZoneID", "CurrencyID", "Rate", "OtherRates", "BED", "EED", "SourceID" };
        string _TableName = Vanrise.Common.Utilities.GetEnumDescription(DBTableName.SaleRate);
        string _Schema = "TOneWhS_BE";
        bool _UseTempTables;
        public SaleRateDBSyncDataManager(bool useTempTables) :
            base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {
            _UseTempTables = useTempTables;
        }

        public void ApplySaleRatesToTemp(List<SaleRate> saleRates)
        {
            string filePath = GetFilePathForBulkInsert();
            using (System.IO.StreamWriter wr = new System.IO.StreamWriter(filePath))
            {
                foreach (var c in saleRates)
                {
                    wr.WriteLine(String.Format("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}", c.PriceListId, c.ZoneId, c.CurrencyId, c.NormalRate, c.OtherRates, c.BED, c.EED, c.SourceId));
                }
                wr.Close();
            }

            Object preparedSaleRates = new BulkInsertInfo
            {
                TableName = MigrationUtils.GetTableName(_Schema, _TableName, _UseTempTables),
                DataFilePath = filePath,
                ColumnNames = columns,
                TabLock = true,
                KeepIdentity = true,
                FieldSeparator = '^',
            };

            InsertBulkToTable(preparedSaleRates as BaseBulkInsertInfo);
        }

        public Dictionary<string, SaleRate> GetSaleRates()
        {
            return GetItemsText("SELECT PriceListID, ZoneID, CurrencyID, Rate, OtherRates, BED, EED, SourceID FROM"
                + MigrationUtils.GetTableName(_Schema, _TableName, _UseTempTables), SaleRateMapper, cmd => { }).ToDictionary(x => x.SourceId, x => x);
        }

        public SaleRate SaleRateMapper(IDataReader reader)
        {
            return new SaleRate
            {
                SaleRateId = (long)reader["ID"],
                ZoneId = (long)reader["ZoneID"],
                PriceListId = (int)reader["PriceListID"],
                NormalRate = (decimal)reader["Rate"],
                OtherRates = reader["OtherRates"] as string != null ? Vanrise.Common.Serializer.Deserialize<Dictionary<int, decimal>>(reader["OtherRates"] as string) : null,
                BED = (DateTime)reader["BED"],
                EED = GetReaderValue<DateTime?>(reader, "EED"),
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
