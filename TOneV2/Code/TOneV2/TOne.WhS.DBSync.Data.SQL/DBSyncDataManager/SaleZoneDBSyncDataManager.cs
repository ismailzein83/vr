using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class SaleZoneDBSyncDataManager : BaseSQLDataManager, IDBSyncDataManager
    {
        readonly string[] columns = { "SellingNumberPlanID", "CountryID", "Name", "BED", "EED", "SourceID", "ID" };
        string _TableName = Vanrise.Common.Utilities.GetEnumDescription(DBTableName.SaleZone);
        string _Schema = "TOneWhS_BE";
        bool _UseTempTables;
        public SaleZoneDBSyncDataManager(bool useTempTables) :
            base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {
            _UseTempTables = useTempTables;
        }

        public void ApplySaleZonesToTemp(List<SaleZone> saleZones, long startingId)
        {
            string filePath = GetFilePathForBulkInsert();
            using (System.IO.StreamWriter wr = new System.IO.StreamWriter(filePath))
            {
                foreach (var c in saleZones)
                {
                    wr.WriteLine(String.Format("{0}^{1}^{2}^{3}^{4}^{5}^{6}", c.SellingNumberPlanId, c.CountryId, c.Name, c.BED, c.EED, c.SourceId, startingId++));
                }
                wr.Close();
            }

            Object preparedSaleZones = new BulkInsertInfo
            {
                TableName = MigrationUtils.GetTableName(_Schema, _TableName, _UseTempTables),
                DataFilePath = filePath,
                ColumnNames = columns,
                TabLock = true,
                KeepIdentity = true,
                FieldSeparator = '^',
            };

            InsertBulkToTable(preparedSaleZones as BaseBulkInsertInfo);
        }

        public Dictionary<string, SaleZone> GetSaleZones(bool useTempTables)
        {
            return GetItemsText("SELECT ID, SellingNumberPlanID, CountryID, Name, BED, EED, SourceID FROM"
                + MigrationUtils.GetTableName(_Schema, _TableName, useTempTables), SaleZoneMapper, cmd => { }).ToDictionary(x => x.SourceId, x => x);
        }

        public SaleZone SaleZoneMapper(IDataReader reader)
        {
            return new SaleZone
            {
                SaleZoneId = (long)reader["ID"],
                SellingNumberPlanId = (int)reader["SellingNumberPlanID"],
                CountryId = GetReaderValue<int>(reader, "CountryID"),
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
