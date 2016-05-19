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
    public class CodeGroupDBSyncDataManager : BaseSQLDataManager
    {
        readonly string[] columns = { "Code", "SourceID", "CountryID" };
        string _TableName = Vanrise.Common.Utilities.GetEnumDescription(DBTableName.CodeGroup);
        string _Schema = "TOneWhS_BE";
        bool _UseTempTables;
        public CodeGroupDBSyncDataManager(bool useTempTables) :
            base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {
            _UseTempTables = useTempTables;
        }

        public void ApplyCodeGroupsToTemp(List<CodeGroup> codeGroups)
        {
            string filePath = GetFilePathForBulkInsert();
            using (System.IO.StreamWriter wr = new System.IO.StreamWriter(filePath))
            {
                foreach (var c in codeGroups)
                {
                    wr.WriteLine(String.Format("{0}^{1}^{2}", c.Code, c.SourceId, c.CountryId));
                }
                wr.Close();
            }

            Object preparedCodeGroups = new BulkInsertInfo
            {
                TableName = MigrationUtils.GetTableName(_Schema, _TableName, _UseTempTables),
                DataFilePath = filePath,
                ColumnNames = columns,
                TabLock = true,
                KeepIdentity = true,
                FieldSeparator = '^',
            };

            InsertBulkToTable(preparedCodeGroups as BaseBulkInsertInfo);
        }

        public Dictionary<string, CodeGroup> GetCodeGroups()
        {
            return GetItemsText("SELECT [ID], [CountryID], [Code], [SourceID] FROM"
                + MigrationUtils.GetTableName(_Schema, _TableName, _UseTempTables), CodeGroupMapper, cmd => { }).ToDictionary(x => x.SourceId, x => x);
        }

        public CodeGroup CodeGroupMapper(IDataReader reader)
        {
            CodeGroup codeGroup = new CodeGroup
            {
                CountryId = (int)reader["CountryId"],
                Code = reader["Code"] as string,
                SourceId = reader["SourceID"] as string,
            };

            return codeGroup;
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
