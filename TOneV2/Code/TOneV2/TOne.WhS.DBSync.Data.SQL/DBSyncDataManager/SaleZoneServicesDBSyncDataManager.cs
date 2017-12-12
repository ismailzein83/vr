using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class SaleZoneServicesDBSyncDataManager : BaseSQLDataManager, IDBSyncDataManager
    {
        string _TableName = Vanrise.Common.Utilities.GetEnumDescription(DBTableName.SaleEntityService);
        string _Schema = "TOneWhS_BE";
        bool _UseTempTables;
        public SaleZoneServicesDBSyncDataManager(bool useTempTables) :
            base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {
            _UseTempTables = useTempTables;
        }

        static string[] s_columns = new string[] { "ID", "PriceListID", "ZoneID", "Services", "BED", "EED", "SourceID" };

        public void ApplySaleZoneServicesToTemp(List<SaleEntityZoneService> saleZoneServices, long startingId)
        {
            var stream = base.InitializeStreamForBulkInsert();
            foreach (var item in saleZoneServices)
            {
                stream.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}", startingId++, item.PriceListId, item.ZoneId, Vanrise.Common.Serializer.Serialize(item.Services, true), GetDateTimeForBCP(item.BED), item.EED.HasValue ? GetDateTimeForBCP(item.EED.Value) : "", item.SourceId);
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
            //dt.Columns.Add("ZoneID", typeof(long));
            //dt.Columns.Add("Services", typeof(string));
            //dt.Columns.Add("BED", typeof(DateTime));
            //dt.Columns.Add(new DataColumn { AllowDBNull = true, ColumnName = "EED", DataType = typeof(DateTime) });
            //dt.Columns.Add("SourceID", typeof(string));
            //dt.Columns.Add("ID", typeof(long));

            //dt.BeginLoadData();
            //foreach (var item in saleZoneServices)
            //{
            //    DataRow row = dt.NewRow();
            //    row["PriceListID"] = item.PriceListId;
            //    row["ZoneID"] = item.ZoneId;
            //    row["Services"] = Vanrise.Common.Serializer.Serialize(item.Services, true);
            //    row["BED"] = item.BED;
            //    row["EED"] = item.EED.HasValue ? item.EED : (object)DBNull.Value;
            //    row["SourceID"] = item.SourceId;
            //    row["ID"] = startingId++;
            //    dt.Rows.Add(row);
            //}
            //dt.EndLoadData();
            //WriteDataTableToDB(dt, System.Data.SqlClient.SqlBulkCopyOptions.KeepNulls);
        }

        public Dictionary<string, SaleEntityZoneService> GetSSaleZoneServices(bool useTempTables)
        {
            return GetItemsText(string.Format("SELECT [ID], [PriceListID], [ZoneID], [Services], [BED], [EED] FROM {0} where sourceid is not null"
                , MigrationUtils.GetTableName(_Schema, _TableName, useTempTables)), SaleZoneServiceMapper, cmd => { }).ToDictionary(x => x.SaleEntityServiceId.ToString(), x => x);
        }

        public SaleEntityZoneService SaleZoneServiceMapper(IDataReader reader)
        {
            return new SaleEntityZoneService
            {
                SaleEntityServiceId = (long)reader["ID"],
                ZoneId = GetReaderValue<long>(reader, "ZoneID"),
                Services = Vanrise.Common.Serializer.Deserialize<List<ZoneService>>(reader["Services"] as string),
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
