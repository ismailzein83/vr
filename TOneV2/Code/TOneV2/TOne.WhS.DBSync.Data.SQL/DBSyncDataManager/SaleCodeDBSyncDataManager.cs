using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class SaleCodeDBSyncDataManager : BaseSQLDataManager, IDBSyncDataManager
    {
        string _TableName = Vanrise.Common.Utilities.GetEnumDescription(DBTableName.SaleCode);
        string _Schema = "TOneWhS_BE";
        bool _UseTempTables;

        public SaleCodeDBSyncDataManager(bool useTempTables) :
            base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {
            _UseTempTables = useTempTables;
        }

        public void ApplySaleCodesToTemp(List<SaleCode> saleCodes, long startingId)
        {
            DataTable dt = new DataTable();
            dt.TableName = MigrationUtils.GetTableName(_Schema, _TableName, _UseTempTables);
            dt.Columns.Add("Code", typeof(string));
            dt.Columns.Add("ZoneID", typeof(long));
            dt.Columns.Add("CodeGroupID", typeof(int));
            dt.Columns.Add("BED", typeof(DateTime));
            dt.Columns.Add(new DataColumn { AllowDBNull = true, ColumnName = "EED", DataType = typeof(DateTime) });
            dt.Columns.Add("SourceID", typeof(string));
            dt.Columns.Add("ID", typeof(long));

            dt.BeginLoadData();
            foreach (var item in saleCodes)
            {
                DataRow row = dt.NewRow();
                int index = 0;
                row[index++] = item.Code;
                row[index++] = item.ZoneId;
                row[index++] = item.CodeGroupId;
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

        public Dictionary<string, SaleCode> GetSaleCodes(bool useTempTables)
        {
            return GetItemsText(string.Format("SELECT [ID] ,[Code]  ,[ZoneID]  ,[BED], [EED], [SourceID] FROM {0} where sourceid is not null",
                        MigrationUtils.GetTableName(_Schema, _TableName, useTempTables)), SaleCodeMapper, cmd => { }).ToDictionary(x => x.SourceId, x => x);
        }

        public List<SaleCodeZone> GetDistinctSaleCodeZones()
        {
            return GetItemsText(string.Format("SELECT distinct [Code], [ZoneID] FROM {0} where [EED] is null",
                        MigrationUtils.GetTableName(_Schema, _TableName, _UseTempTables)), SaleCodeZoneMapper, cmd => { });
        }

        public SaleCode SaleCodeMapper(IDataReader reader)
        {
            return new SaleCode
            {
                SaleCodeId = (long)reader["ID"],
                Code = reader["Code"] as string,
                ZoneId = GetReaderValue<long>(reader, "ZoneID"),
                BED = GetReaderValue<DateTime>(reader, "BED"),
                EED = GetReaderValue<DateTime?>(reader, "EED"),
                SourceId = reader["SourceID"] as string,
            };
        }

        public SaleCodeZone SaleCodeZoneMapper(IDataReader reader)
        {
            return new SaleCodeZone
            {
                Code = reader["Code"] as string,
                ZoneId = GetReaderValue<long>(reader, "ZoneID"),
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
