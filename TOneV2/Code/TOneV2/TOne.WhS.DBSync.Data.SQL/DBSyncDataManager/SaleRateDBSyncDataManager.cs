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
        static string[] s_columns = new string[] { "ID", "PriceListID", "ZoneID", "Rate", "BED", "EED", "Change", "RateTypeID", "SourceID" };

        public void ApplySaleRatesToTemp(List<SaleRate> saleRates, long startingId)
        {
            var stream = base.InitializeStreamForBulkInsert();
            foreach (var item in saleRates)
            {
                stream.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^{8}", startingId++, item.PriceListId, item.ZoneId, item.Rate, GetDateTimeForBCP(item.BED), item.EED.HasValue ? GetDateTimeForBCP(item.EED.Value) : "", (int)item.RateChange, item.RateTypeId, item.SourceId);
            }
            stream.Close();
            StreamBulkInsertInfo streamBulkInsert = new StreamBulkInsertInfo
            {
                Stream = stream,
                ColumnNames = s_columns,
                FieldSeparator = '^',
                TableName = MigrationUtils.GetTableName(_Schema, _TableName, _UseTempTables),
                TabLock = true
            };
            base.InsertBulkToTable(streamBulkInsert);

            //DataTable dt = new DataTable();
            //dt.TableName = MigrationUtils.GetTableName(_Schema, _TableName, _UseTempTables);
            //dt.Columns.Add("PriceListID", typeof(int));
            //dt.Columns.Add("ZoneID", typeof(string));
            //dt.Columns.Add("Rate", typeof(decimal));
            //dt.Columns.Add("OtherRates", typeof(string));
            //dt.Columns.Add("BED", typeof(DateTime));
            //dt.Columns.Add(new DataColumn { AllowDBNull = true, ColumnName = "EED", DataType = typeof(DateTime) });
            //dt.Columns.Add("Change", typeof(Byte));
            //dt.Columns.Add("ID", typeof(int));
            //dt.Columns.Add(new DataColumn { AllowDBNull = true, ColumnName = "RateTypeID", DataType = typeof(int) });
            //dt.Columns.Add("SourceID", typeof(string));

            //dt.BeginLoadData();
            //foreach (var item in saleRates)
            //{
            //    DataRow row = dt.NewRow();
            //    row["PriceListID"] = item.PriceListId;
            //    row["ZoneID"] = item.ZoneId;
            //    row["Rate"] = item.NormalRate;
            //    row["OtherRates"] = Vanrise.Common.Serializer.Serialize(item.OtherRates);
            //    row["BED"] = item.BED;
            //    row["EED"] = item.EED.HasValue ? item.EED : (object)DBNull.Value;
            //    row["Change"] = item.RateChange;
            //    row["ID"] = startingId++;
            //    row["RateTypeID"] = item.RateTypeId.HasValue ? item.RateTypeId : (object)DBNull.Value;
            //    row["SourceID"] = item.SourceId;

            //    dt.Rows.Add(row);
            //}
            //dt.EndLoadData();
            //WriteDataTableToDB(dt, System.Data.SqlClient.SqlBulkCopyOptions.KeepNulls);
        }

        public Dictionary<string, SaleRate> GetSaleRates(bool useTempTables)
        {
            return GetItemsText(string.Format("SELECT [ID]  ,[ZoneID] ,[PriceListID],[Rate],[BED], [EED],[Change], [SourceID] FROM {0} where sourceid is not null"
                , MigrationUtils.GetTableName(_Schema, _TableName, useTempTables)), SaleRateMapper, cmd => { }).ToDictionary(x => x.SourceId, x => x);
        }

        public SaleRate SaleRateMapper(IDataReader reader)
        {
            return new SaleRate
            {
                SaleRateId = (long)reader["ID"],
                ZoneId = (long)reader["ZoneID"],
                PriceListId = (int)reader["PriceListID"],
                Rate = (decimal)reader["Rate"],
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
