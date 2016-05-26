using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class SwitchDBSyncDataManager : BaseSQLDataManager, IDBSyncDataManager
    {
        string _TableName = Vanrise.Common.Utilities.GetEnumDescription(DBTableName.Switch);
        string _Schema = "TOneWhS_BE";
        bool _UseTempTables;
        public SwitchDBSyncDataManager(bool useTempTables) :
            base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {
            _UseTempTables = useTempTables;
        }

        public void ApplySwitchesToTemp(List<Switch> switches)
        {
            DataTable dt = new DataTable();
            dt.TableName = MigrationUtils.GetTableName(_Schema, _TableName, _UseTempTables);
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("SourceID", typeof(string));
            dt.BeginLoadData();
            foreach (var item in switches)
            {
                DataRow row = dt.NewRow();
                int index = 0;
                row[index++] = item.Name;
                row[index++] = item.SourceId;

                dt.Rows.Add(row);
            }
            dt.EndLoadData();
            WriteDataTableToDB(dt, System.Data.SqlClient.SqlBulkCopyOptions.KeepNulls);
        }

        public Dictionary<string, Switch> GetSwitches(bool useTempTables)
        {
            return GetItemsText("SELECT [ID]  ,[Name], [SourceID] FROM "
                + MigrationUtils.GetTableName(_Schema, _TableName, useTempTables), SwitchMapper, cmd => { }).ToDictionary(x => x.SourceId, x => x);
        }

        public Switch SwitchMapper(IDataReader reader)
        {
            return new Switch
            {
                SwitchId = (int)reader["ID"],
                Name = reader["Name"] as string,
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
