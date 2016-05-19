using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Data.SQL.Common;
using Vanrise.Data.SQL;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class SaleCodeDBSyncDataManager : BaseSQLDataManager
    {
        readonly string[] columns = { "Code", "ZoneID", "CodeGroupID", "BED", "EED", "SourceID" };
        string _TableName = Vanrise.Common.Utilities.GetEnumDescription(DBTableName.SaleCode);
        string _Schema = "TOneWhS_BE";
        bool _UseTempTables;
        public SaleCodeDBSyncDataManager(bool useTempTables) :
            base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {
            _UseTempTables = useTempTables;
        }

        public void ApplySaleCodesToTemp(List<SaleCode> saleCodes)
        {
            string filePath = GetFilePathForBulkInsert();
            using (System.IO.StreamWriter wr = new System.IO.StreamWriter(filePath))
            {
                foreach (var c in saleCodes)
                {
                    wr.WriteLine(String.Format("{0}^{1}^{2}^{3}^{4}^{5}", c.Code, c.ZoneId, c.CodeGroupId, c.BED, c.EED, c.SourceId));
                }
                wr.Close();
            }

            Object preparedSaleCodes = new BulkInsertInfo
            {
                TableName = MigrationUtils.GetTableName(_Schema, _TableName, _UseTempTables),
                DataFilePath = filePath,
                ColumnNames = columns,
                TabLock = true,
                KeepIdentity = true,
                FieldSeparator = '^',
            };

            InsertBulkToTable(preparedSaleCodes as BaseBulkInsertInfo);
        }

        public Dictionary<string, SaleCode> GetSaleCodes()
        {
            return GetItemsText("SELECT Code, ZoneID, CodeGroupID, BED, EED, SourceID FROM"
                + MigrationUtils.GetTableName(_Schema, _TableName, _UseTempTables), SaleCodeMapper, cmd => { }).ToDictionary(x => x.SourceId, x => x);
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
