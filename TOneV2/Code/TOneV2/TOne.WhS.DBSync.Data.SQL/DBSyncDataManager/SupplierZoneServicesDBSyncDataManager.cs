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

        public void ApplySupplierZoneServicesToTemp(List<SupplierZoneService> supplierZoneServices, long startingId)
        {
            DataTable dt = new DataTable();
            dt.TableName = MigrationUtils.GetTableName(_Schema, _TableName, _UseTempTables);
            dt.Columns.Add("ZoneID", typeof(long));
            dt.Columns.Add("ReceivedServicesFlag", typeof(string));
            dt.Columns.Add("EffectiveServiceFlag", typeof(string));
            dt.Columns.Add("BED", typeof(DateTime));
            dt.Columns.Add(new DataColumn { AllowDBNull = true, ColumnName = "EED", DataType = typeof(DateTime) });
            dt.Columns.Add("SourceID", typeof(string));
            dt.Columns.Add("ID", typeof(long));

            dt.BeginLoadData();
            foreach (var item in supplierZoneServices)
            {
                DataRow row = dt.NewRow();
                row["ZoneID"] = item.ZoneId;
                row["ReceivedServicesFlag"] = Vanrise.Common.Serializer.Serialize(item.ReceivedServices);
                row["EffectiveServiceFlag"] = Vanrise.Common.Serializer.Serialize(item.EffectiveServices);
                row["BED"] = item.BED;
                row["EED"] = item.EED.HasValue ? item.EED : (object)DBNull.Value;
                row["SourceID"] = item.SourceId;
                row["ID"] = startingId++;
                dt.Rows.Add(row);
            }
            dt.EndLoadData();
            WriteDataTableToDB(dt, System.Data.SqlClient.SqlBulkCopyOptions.KeepNulls);
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
