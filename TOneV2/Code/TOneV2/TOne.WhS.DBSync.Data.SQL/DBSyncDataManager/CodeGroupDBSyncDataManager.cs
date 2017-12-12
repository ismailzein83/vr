using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Entities;
using Vanrise.Data.SQL;


namespace TOne.WhS.DBSync.Data.SQL
{
    public class CodeGroupDBSyncDataManager : BaseSQLDataManager, IDBSyncDataManager
    {
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
            DataTable dt = new DataTable();
            dt.TableName = MigrationUtils.GetTableName(_Schema, _TableName, _UseTempTables);
            dt.Columns.Add("Code", typeof(string));
            dt.Columns.Add("SourceID", typeof(string));
            dt.Columns.Add("CountryID", typeof(int));
            dt.Columns.Add("Name", typeof(string));

            dt.BeginLoadData();
            foreach (var item in codeGroups)
            {
                DataRow row = dt.NewRow();
                int index = 0;
                row[index++] = item.Code;
                row[index++] = item.SourceId;
                row[index++] = item.CountryId;
                row[index++] = item.Name;
                dt.Rows.Add(row);
            }
            dt.EndLoadData();
            WriteDataTableToDB(dt, System.Data.SqlClient.SqlBulkCopyOptions.KeepNulls);
        }

        public Dictionary<string, CodeGroup> GetCodeGroups(bool useTempTables)
        {
            return GetItemsText(string.Format("SELECT [ID], [CountryID], [Code],[Name], [SourceID] FROM {0} where sourceid is not null",
                 MigrationUtils.GetTableName(_Schema, _TableName, useTempTables)), CodeGroupMapper, cmd => { }).ToDictionary(x => x.SourceId, x => x);
        }

        public CodeGroup CodeGroupMapper(IDataReader reader)
        {
            CodeGroup codeGroup = new CodeGroup
            {
                CodeGroupId = (int)reader["ID"],
                CountryId = (int)reader["CountryId"],
                Code = reader["Code"] as string,
                Name = reader["Name"] as string,
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
