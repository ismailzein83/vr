using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using TOne.WhS.DBSync.Data.SQL.Common;
using Vanrise.Data.SQL;
using Vanrise.Entities;
using System.Linq;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class SupplierCodeDBSyncDataManager : BaseSQLDataManager
    {
        readonly string[] columns = { "Code", "ZoneID", "CodeGroupID", "BED", "EED", "SourceID" };
        string _TableName = Vanrise.Common.Utilities.GetEnumDescription(DBTableName.SupplierCode);
        string _Schema = "TOneWhS_BE";
        bool _UseTempTables;
        public SupplierCodeDBSyncDataManager(bool useTempTables) :
            base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {
            _UseTempTables = useTempTables;
        }

        public void ApplySupplierCodesToTemp(List<SupplierCode> supplierCodes)
        {
            string filePath = GetFilePathForBulkInsert();
            using (System.IO.StreamWriter wr = new System.IO.StreamWriter(filePath))
            {
                foreach (var c in supplierCodes)
                {
                    wr.WriteLine(String.Format("{0}^{1}^{2}", c.Code, c.ZoneId, c.CodeGroupId, c.BED, c.EED, c.SourceId));
                }
                wr.Close();
            }

            Object preparedSupplierCodes = new BulkInsertInfo
            {
                TableName = MigrationUtils.GetTableName(_Schema, _TableName, _UseTempTables),
                DataFilePath = filePath,
                ColumnNames = columns,
                TabLock = true,
                KeepIdentity = true,
                FieldSeparator = '^',
            };

            InsertBulkToTable(preparedSupplierCodes as BaseBulkInsertInfo);
        }

        public Dictionary<string, SupplierCode> GetSupplierCodes()
        {
            return GetItemsText("SELECT ID,  Code, ZoneID, CodeGroupID, BED, EED, SourceID FROM"
                + MigrationUtils.GetTableName(_Schema, _TableName, _UseTempTables), SupplierCodeMapper, cmd => { }).ToDictionary(x => x.SourceId, x => x);
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
