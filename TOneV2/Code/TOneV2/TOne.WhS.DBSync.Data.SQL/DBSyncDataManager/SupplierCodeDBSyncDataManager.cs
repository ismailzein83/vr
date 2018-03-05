using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class SupplierCodeDBSyncDataManager : BaseSQLDataManager, IDBSyncDataManager
    {
        static string[] s_columns = new string[] { "Code", "ZoneID", "CodeGroupID", "BED", "EED", "SourceID", "ID" };

        string _TableName = Vanrise.Common.Utilities.GetEnumDescription(DBTableName.SupplierCode);
        string _Schema = "TOneWhS_BE";
        bool _UseTempTables;

        public SupplierCodeDBSyncDataManager(bool useTempTables) :
            base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {
            _UseTempTables = useTempTables;
        }

        public void ApplySupplierCodesToTemp(List<SupplierCode> supplierCodes, long startingId)
        {
            //DataTable dt = new DataTable();
            //dt.TableName = MigrationUtils.GetTableName(_Schema, _TableName, _UseTempTables);

            var stream = base.InitializeStreamForBulkInsert();
            foreach (var item in supplierCodes)
            {
                stream.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}", item.Code, item.ZoneId, item.CodeGroupId, GetDateTimeForBCP(item.BED), item.EED.HasValue ? GetDateTimeForBCP(item.EED.Value) : "", item.SourceId, startingId++);
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

            //dt.Columns.Add("Code", typeof(string));
            //dt.Columns.Add("ZoneID", typeof(long));
            //dt.Columns.Add(new DataColumn { AllowDBNull = true, ColumnName = "CodeGroupID", DataType = typeof(int) });
            //dt.Columns.Add("BED", typeof(DateTime));
            //dt.Columns.Add(new DataColumn { AllowDBNull = true, ColumnName = "EED", DataType = typeof(DateTime) });
            //dt.Columns.Add("SourceID", typeof(string));
            //dt.Columns.Add("ID", typeof(long));

            //dt.BeginLoadData();
            //foreach (var item in supplierCodes)
            //{
            //    DataRow row = dt.NewRow();
            //    int index = 0;
            //    row[index++] = item.Code;
            //    row[index++] = item.ZoneId;
            //    if (item.CodeGroupId == null)
            //        row[index++] = DBNull.Value;
            //    else
            //        row[index++] = item.CodeGroupId;
            //    row[index++] = item.BED;
            //    if (item.EED == null)
            //        row[index++] = DBNull.Value;
            //    else
            //        row[index++] = item.EED; 
            //    row[index++] = item.SourceId;
            //    row[index++] = startingId++;

            //    dt.Rows.Add(row);
            //}
            //dt.EndLoadData();
            //WriteDataTableToDB(dt, System.Data.SqlClient.SqlBulkCopyOptions.KeepNulls);
        }

        public Dictionary<string, SupplierCode> GetSupplierCodes(bool useTempTables)
        {
            return GetItemsText(string.Format("SELECT [ID] ,[Code]  ,[ZoneID] ,[BED], [EED], [SourceID] FROM {0} where sourceid is not null",
                        MigrationUtils.GetTableName(_Schema, _TableName, useTempTables)), SupplierCodeMapper, cmd => { }).ToDictionary(x => x.SourceId, x => x);
        }

        public List<SupplierCodeZone> GetDictinctSupplierCodeZones()
        {
            return GetItemsText(string.Format("SELECT distinct [Code], [ZoneID] FROM {0} where [EED] is not null",
                        MigrationUtils.GetTableName(_Schema, _TableName, _UseTempTables)), SupplierCodeZoneMapper, cmd => { });
        }

        public SupplierCode SupplierCodeMapper(IDataReader reader)
        {
            return new SupplierCode
            {
                Code = GetReaderValue<string>(reader, "Code"),
                SupplierCodeId = GetReaderValue<long>(reader, "ID"),
                ZoneId = (long)reader["ZoneID"],
                BED = GetReaderValue<DateTime>(reader, "BED"),
                EED = GetReaderValue<DateTime?>(reader, "EED"),
                SourceId = reader["SourceID"] as string,
            };
        }

        public SupplierCodeZone SupplierCodeZoneMapper(IDataReader reader)
        {
            return new SupplierCodeZone
            {
                Code = GetReaderValue<string>(reader, "Code"),
                ZoneId = (long)reader["ZoneID"],
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
