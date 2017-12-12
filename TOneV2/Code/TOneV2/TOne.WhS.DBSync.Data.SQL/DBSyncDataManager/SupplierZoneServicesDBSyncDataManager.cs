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

        static string[] s_columns = new string[] { "ID", "PriceListID", "ZoneID", "SupplierID", "ReceivedServicesFlag", "EffectiveServiceFlag", "BED", "EED", "SourceID" };
        public void ApplySupplierZoneServicesToTemp(List<SupplierZoneService> supplierZoneServices, long startingId)
        {
            var stream = base.InitializeStreamForBulkInsert();
            foreach (var item in supplierZoneServices)
            {
                stream.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^{8}", startingId++, item.PriceListId, item.ZoneId, item.SupplierId, Vanrise.Common.Serializer.Serialize(item.ReceivedServices, true), Vanrise.Common.Serializer.Serialize(item.EffectiveServices, true), GetDateTimeForBCP(item.BED), item.EED.HasValue ? GetDateTimeForBCP(item.EED.Value) : "", item.SourceId);
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
        }


        public void ApplySupplierDefaultServicesToTemp(List<SupplierDefaultService> supplierDefaultServices, long startingId)
        {
            var stream = base.InitializeStreamForBulkInsert();
            foreach (var item in supplierDefaultServices)
            {
                stream.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^{8}", startingId++, string.Empty, string.Empty, item.SupplierId, Vanrise.Common.Serializer.Serialize(item.ReceivedServices, true), Vanrise.Common.Serializer.Serialize(item.EffectiveServices, true), GetDateTimeForBCP(item.BED), item.EED.HasValue ? GetDateTimeForBCP(item.EED.Value) : "", item.SourceId);
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
        }


        public Dictionary<string, SupplierZoneService> GetSupplierZoneServices(bool useTempTables)
        {
            return GetItemsText(string.Format("SELECT [ID], [ZoneID], [SupplierID], [ReceivedServicesFlag], [EffectiveServiceFlag], [BED], [EED] FROM {0} where sourceid is not null"
                , MigrationUtils.GetTableName(_Schema, _TableName, useTempTables)), SupplierZoneServiceMapper, cmd => { }).ToDictionary(x => x.SupplierZoneServiceId.ToString(), x => x);
        }

        public SupplierZoneService SupplierZoneServiceMapper(IDataReader reader)
        {
            return new SupplierZoneService
            {
                SupplierZoneServiceId = (long)reader["ID"],
                ZoneId = GetReaderValue<long>(reader, "ZoneID"),
                SupplierId = GetReaderValue<int>(reader, "SupplierID"),
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
