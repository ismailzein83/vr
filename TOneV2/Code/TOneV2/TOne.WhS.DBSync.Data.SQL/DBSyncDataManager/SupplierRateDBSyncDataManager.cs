using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class SupplierRateDBSyncDataManager : BaseSQLDataManager, IDBSyncDataManager
    {
        string _TableName = Vanrise.Common.Utilities.GetEnumDescription(DBTableName.SupplierRate);
        string _Schema = "TOneWhS_BE";
        bool _UseTempTables;
        public SupplierRateDBSyncDataManager(bool useTempTables) :
            base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {
            _UseTempTables = useTempTables;
        }

        static string[] s_columns = new string[] { "ID", "PriceListID", "ZoneID", "Rate", "BED", "EED", "Change", "RateTypeID", "SourceID" };

        public void ApplySupplierRatesToTemp(List<SupplierRate> supplierRates, long startingId)
        {
            var stream = base.InitializeStreamForBulkInsert();
            foreach (var item in supplierRates)
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
            //dt.Columns.Add("NormalRate", typeof(decimal));
            //dt.Columns.Add("OtherRates", typeof(string));
            //dt.Columns.Add("BED", typeof(DateTime));
            //dt.Columns.Add(new DataColumn { AllowDBNull = true, ColumnName = "EED", DataType = typeof(DateTime) });
            //dt.Columns.Add("Change", typeof(Byte));
            //dt.Columns.Add("SourceID", typeof(string));
            //dt.Columns.Add(new DataColumn { AllowDBNull = true, ColumnName = "RateTypeID", DataType = typeof(int) });
            //dt.Columns.Add("ID", typeof(long));

            //dt.BeginLoadData();
            //foreach (var item in supplierRates)
            //{
            //    DataRow row = dt.NewRow();
            //    row["PriceListID"] = item.PriceListId;
            //    row["ZoneID"] = item.ZoneId;
            //    row["NormalRate"] = item.NormalRate;
            //    row["OtherRates"] = Vanrise.Common.Serializer.Serialize(item.OtherRates);
            //    row["BED"] = item.BED;
            //    row["EED"] = item.EED.HasValue ? item.EED : (object)DBNull.Value;
            //    row["Change"] = item.RateChange;
            //    row["SourceID"] = item.SourceId;
            //    row["RateTypeID"] = item.RateTypeId.HasValue ? item.RateTypeId : (object)DBNull.Value;
            //    row["ID"] = startingId++;
            //    dt.Rows.Add(row);



            //}
            //dt.EndLoadData();
            //WriteDataTableToDB(dt, System.Data.SqlClient.SqlBulkCopyOptions.KeepNulls);
        }

        public Dictionary<string, SupplierRate> GetSupplierRates(bool useTempTables)
        {
            return GetItemsText(string.Format("SELECT [ID]  ,[ZoneID] ,[PriceListID],[Rate], [RateTypeID],[BED], [EED], [Change], [SourceID] FROM {0} where sourceid is not null"
                , MigrationUtils.GetTableName(_Schema, _TableName, useTempTables)), SupplierRateMapper, cmd => { }).ToDictionary(x => x.SourceId, x => x);
        }

        public SupplierRate SupplierRateMapper(IDataReader reader)
        {
            return new SupplierRate
            {
                SupplierRateId = (long)reader["ID"],
                ZoneId = (long)reader["ZoneID"],
                PriceListId = GetReaderValue<int>(reader, "PriceListID"),
                Rate = GetReaderValue<decimal>(reader, "Rate"),
                BED = GetReaderValue<DateTime>(reader, "BED"),
                EED = GetReaderValue<DateTime?>(reader, "EED"),
                RateTypeId = GetReaderValue<int?>(reader, "RateTypeID"),
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
