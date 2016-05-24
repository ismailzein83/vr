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
        readonly string[] columns = { "CountryID", "Name", "SupplierID", "BED", "EED", "SourceID", "ID" };
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
            string filePath = GetFilePathForBulkInsert();
            using (System.IO.StreamWriter wr = new System.IO.StreamWriter(filePath))
            {
                foreach (var c in supplierZones)
                {
                    wr.WriteLine(String.Format("{0}^{1}^{2}^{3}^{4}^{5}^{6}", c.CountryId, c.Name, c.SupplierId, c.BED, c.EED, c.SourceId, startingId++));
                }
                wr.Close();
            }

            Object preparedSupplierZones = new BulkInsertInfo
            {
                TableName = MigrationUtils.GetTableName(_Schema, _TableName, _UseTempTables),
                DataFilePath = filePath,
                ColumnNames = columns,
                TabLock = true,
                KeepIdentity = true,
                FieldSeparator = '^',
            };

            InsertBulkToTable(preparedSupplierZones as BaseBulkInsertInfo);
        }

        public Dictionary<string, SupplierZone> GetSupplierZones(bool useTempTables)
        {
            return GetItemsText("SELECT ID,  CountryID, Name, SupplierID, BED, EED, SourceID FROM"
                + MigrationUtils.GetTableName(_Schema, _TableName, useTempTables), SupplierZoneMapper, cmd => { }).ToDictionary(x => x.SourceId, x => x);
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
