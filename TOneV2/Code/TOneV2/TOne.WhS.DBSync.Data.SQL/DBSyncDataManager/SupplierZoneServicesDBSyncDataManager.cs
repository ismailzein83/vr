using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class SupplierZoneServicesDBSyncDataManager : BaseSQLDataManager, IDBSyncDataManager
    {
        string _TableName = Vanrise.Common.Utilities.GetEnumDescription(DBTableName.SupplierZoneService);
        string _Schema = "TOneWhS_BE";
        bool _UseTempTables;
        public SupplierZoneServicesDBSyncDataManager(bool useTempTables) :
            base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {
            _UseTempTables = useTempTables;
        }

        static string[] s_columns = new string[] { "ID", "PriceListID", "ZoneID", "ReceivedServicesFlag", "EffectiveServiceFlag", "BED", "EED", "SourceID" };
        public void ApplySupplierZoneServicesToTemp(List<SupplierZoneService> supplierZoneServices, long startingId)
        {
            var stream = base.InitializeStreamForBulkInsert();
            foreach (var item in supplierZoneServices)
            {
                stream.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}", startingId++, item.PriceListId, item.ZoneId, Vanrise.Common.Serializer.Serialize(item.ReceivedServices, true), Vanrise.Common.Serializer.Serialize(item.EffectiveServices, true), GetDateTimeForBCP(item.BED), item.EED.HasValue ? GetDateTimeForBCP(item.EED.Value) : "", item.SourceId);
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
            //dt.Columns.Add("ZoneID", typeof(long));
            //dt.Columns.Add("PriceListID", typeof(int));
            //dt.Columns.Add("ReceivedServicesFlag", typeof(string));
            //dt.Columns.Add("EffectiveServiceFlag", typeof(string));
            //dt.Columns.Add("BED", typeof(DateTime));
            //dt.Columns.Add(new DataColumn { AllowDBNull = true, ColumnName = "EED", DataType = typeof(DateTime) });
            //dt.Columns.Add("SourceID", typeof(string));
            //dt.Columns.Add("ID", typeof(long));

            //dt.BeginLoadData();
            //foreach (var item in supplierZoneServices)
            //{
            //    DataRow row = dt.NewRow();
            //    row["ZoneID"] = item.ZoneId;
            //    row["PriceListID"] = item.PriceListId;
            //    row["ReceivedServicesFlag"] = Vanrise.Common.Serializer.Serialize(item.ReceivedServices, true);
            //    row["EffectiveServiceFlag"] = Vanrise.Common.Serializer.Serialize(item.EffectiveServices, true);
            //    row["BED"] = item.BED;
            //    row["EED"] = item.EED.HasValue ? item.EED : (object)DBNull.Value;
            //    row["SourceID"] = item.SourceId;
            //    row["ID"] = startingId++;
            //    dt.Rows.Add(row);
            //}
            //dt.EndLoadData();
            //WriteDataTableToDB(dt, System.Data.SqlClient.SqlBulkCopyOptions.KeepNulls);
        }

        public Dictionary<string, SupplierZoneService> GetSupplierZoneServices(bool useTempTables)
        {
            return GetItemsText("SELECT [ID], [ZoneID], [ReceivedServicesFlag], [EffectiveServiceFlag], [BED], [EED] FROM "
                + MigrationUtils.GetTableName(_Schema, _TableName, useTempTables), SupplierZoneServiceMapper, cmd => { }).ToDictionary(x => x.SupplierZoneServiceId.ToString(), x => x);
        }

        public SupplierZoneService SupplierZoneServiceMapper(IDataReader reader)
        {
            return new SupplierZoneService
            {
                SupplierZoneServiceId = (long)reader["ID"],
                ZoneId = (long)reader["ZoneID"],
                ReceivedServices = Vanrise.Common.Serializer.Deserialize<List<ZoneService>>(reader["ReceivedServicesFlag"] as string),
                EffectiveServices = Vanrise.Common.Serializer.Deserialize<List<ZoneService>>(reader["EffectiveServiceFlag"] as string),
                BED = GetReaderValue<DateTime>(reader, "BED"),
                EED = GetReaderValue<DateTime?>(reader, "EED")
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
