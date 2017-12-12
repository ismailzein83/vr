using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class SupplierZoneDBSyncDataManager : BaseSQLDataManager, IDBSyncDataManager
    {
        string _TableName = Vanrise.Common.Utilities.GetEnumDescription(DBTableName.SupplierZone);
        string _Schema = "TOneWhS_BE";
        bool _UseTempTables;
        public SupplierZoneDBSyncDataManager(bool useTempTables) :
            base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {
            _UseTempTables = useTempTables;
        }

        public void ApplySupplierZonesToTemp(List<SupplierZone> supplierZones, long startingId)
        {
            DataTable dt = new DataTable();
            dt.TableName = MigrationUtils.GetTableName(_Schema, _TableName, _UseTempTables);
            dt.Columns.Add("CountryID", typeof(int));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("SupplierID", typeof(int));
            dt.Columns.Add("BED", typeof(DateTime));
            dt.Columns.Add(new DataColumn { AllowDBNull = true, ColumnName = "EED", DataType = typeof(DateTime) });
            dt.Columns.Add("SourceID", typeof(string));
            dt.Columns.Add("ID", typeof(long));
            dt.BeginLoadData();
            foreach (var item in supplierZones)
            {
                DataRow row = dt.NewRow();
                int index = 0;
                row[index++] = item.CountryId;
                row[index++] = item.Name;
                row[index++] = item.SupplierId;
                row[index++] = item.BED;
                if (item.EED == null)
                    row[index++] = DBNull.Value;
                else
                    row[index++] = item.EED;
                row[index++] = item.SourceId;
                row[index++] = startingId++;

                dt.Rows.Add(row);
            }
            dt.EndLoadData();
            WriteDataTableToDB(dt, System.Data.SqlClient.SqlBulkCopyOptions.KeepNulls);
        }

        public Dictionary<string, SupplierZone> GetSupplierZones(bool useTempTables)
        {
            return GetItemsText(string.Format("SELECT ID,  CountryID, Name, SupplierID, BED, EED, SourceID FROM {0} where sourceid is not null"
                , MigrationUtils.GetTableName(_Schema, _TableName, useTempTables)), SupplierZoneMapper, cmd => { }).ToDictionary(x => x.SourceId, x => x);
        }

        public SupplierZone SupplierZoneMapper(IDataReader reader)
        {
            return new SupplierZone
            {
                SupplierId = (int)reader["SupplierID"],
                CountryId = (int)reader["CountryID"],
                SupplierZoneId = (long)reader["ID"],
                Name = reader["Name"] as string,
                BED = GetReaderValue<DateTime>(reader, "BED"),
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
