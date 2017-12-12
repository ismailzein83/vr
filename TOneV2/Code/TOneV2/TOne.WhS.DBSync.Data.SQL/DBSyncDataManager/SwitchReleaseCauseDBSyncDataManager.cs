using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class SwitchReleaseCauseDBSyncDataManager : BaseSQLDataManager, IDBSyncDataManager
    {

        string _TableName = Vanrise.Common.Utilities.GetEnumDescription(DBTableName.SwitchReleaseCause);
        string _Schema = "TOneWhS_BE";
        bool _UseTempTables;
        public SwitchReleaseCauseDBSyncDataManager(bool useTempTables) :
            base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {
            _UseTempTables = useTempTables;
        }

        public void ApplySwitchReleaseCausesToTemp(List<SwitchReleaseCause> switchReleaseCauses, int startingId)
        {
            DataTable dt = new DataTable();
            dt.TableName = MigrationUtils.GetTableName(_Schema, _TableName, _UseTempTables);
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("SwitchID", typeof(int));
            dt.Columns.Add("ReleaseCode", typeof(string));
            dt.Columns.Add("Settings", typeof(string));
            dt.Columns.Add("SourceID", typeof(string));

            dt.BeginLoadData();
            foreach (var item in switchReleaseCauses)
            {
                DataRow row = dt.NewRow();
                int index = 0;
                row[index++] = startingId++;
                row[index++] = item.SwitchId;
                row[index++] = item.ReleaseCode;
                row[index++] = Vanrise.Common.Serializer.Serialize(item.Settings);
                row[index++] = item.SourceId;
                dt.Rows.Add(row);
            }
            dt.EndLoadData();
            WriteDataTableToDB(dt, System.Data.SqlClient.SqlBulkCopyOptions.KeepNulls);
        }

        public Dictionary<string, SwitchReleaseCause> GetSwitchReleaseCauses(bool useTempTables)
        {
            return GetItemsText(string.Format("SELECT [ID],[SwitchID],[ReleaseCode],[Settings], [SourceID] FROM {0} where sourceid is not null"
                , MigrationUtils.GetTableName(_Schema, _TableName, useTempTables)), SwitchReleaseCauseMapper, cmd => { }).ToDictionary(x => x.SourceId, x => x);
        }

        private SwitchReleaseCause SwitchReleaseCauseMapper(IDataReader reader)
        {
            string settings = reader["Settings"] as string;
            return new SwitchReleaseCause
            {
                SourceId = reader["SourceID"] as string,
                ReleaseCode = reader["ReleaseCode"] as string,
                SwitchId = (int)reader["SwitchID"],
                Settings = string.IsNullOrEmpty(settings) ? null : Vanrise.Common.Serializer.Deserialize<SwitchReleaseCauseSetting>(settings),
                SwitchReleaseCauseId = (int)reader["ID"]
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
