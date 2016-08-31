using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class SaleRateDBSyncDataManager : BaseSQLDataManager, IDBSyncDataManager
    {
        string _TableName = Vanrise.Common.Utilities.GetEnumDescription(DBTableName.SaleRate);
        string _Schema = "TOneWhS_BE";
        bool _UseTempTables;
        public SaleRateDBSyncDataManager(bool useTempTables) :
            base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {
            _UseTempTables = useTempTables;
        }

        public void ApplySaleRatesToTemp(List<SaleRate> saleRates, long startingId)
        {
            DataTable dt = new DataTable();
            dt.TableName = MigrationUtils.GetTableName(_Schema, _TableName, _UseTempTables);
            dt.Columns.Add("PriceListID", typeof(int));
            dt.Columns.Add("ZoneID", typeof(string));
            dt.Columns.Add(new DataColumn { AllowDBNull = true, ColumnName = "CurrencyID", DataType = typeof(int) });
            dt.Columns.Add("Rate", typeof(decimal));
            dt.Columns.Add("OtherRates", typeof(string));
            dt.Columns.Add("BED", typeof(DateTime));
            dt.Columns.Add(new DataColumn { AllowDBNull = true, ColumnName = "EED", DataType = typeof(DateTime) });
            dt.Columns.Add("Change", typeof(Byte));
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add(new DataColumn { AllowDBNull = true, ColumnName = "RateTypeID", DataType = typeof(int) });
            dt.Columns.Add("SourceID", typeof(string));
           
            dt.BeginLoadData();
            foreach (var item in saleRates)
            {
                DataRow row = dt.NewRow();
                int index = 0;
                row[index++] = item.PriceListId;
                row[index++] = item.ZoneId;
                if (item.CurrencyId == null)
                    row[index++] = DBNull.Value;
                else
                    row[index++] = item.CurrencyId;
                row[index++] = item.NormalRate;
                row[index++] = Vanrise.Common.Serializer.Serialize(item.OtherRates);
                row[index++] = item.BED;
                if (item.EED == null)
                    row[index++] = DBNull.Value;
                else
                    row[index++] = item.EED;
                row[index++] = item.RateChange;
                row[index++] = startingId++;
                row[index++] = item.RateTypeId.HasValue ? item.RateTypeId : (object)DBNull.Value;
                row[index++] = item.SourceId;

                dt.Rows.Add(row);
            }
            dt.EndLoadData();
            WriteDataTableToDB(dt, System.Data.SqlClient.SqlBulkCopyOptions.KeepNulls);
        }

        public Dictionary<string, SaleRate> GetSaleRates(bool useTempTables)
        {
            return GetItemsText("SELECT [ID]  ,[ZoneID] ,[PriceListID],[Rate],[OtherRates],[BED], [EED],[Change], [SourceID] FROM "
                + MigrationUtils.GetTableName(_Schema, _TableName, useTempTables), SaleRateMapper, cmd => { }).ToDictionary(x => x.SourceId, x => x);
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
                RateChange = GetReaderValue<RateChangeType>(reader, "Change"),
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
