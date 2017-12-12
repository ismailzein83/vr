using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TOne.WhS.DBSync.Entities;
using Vanrise.Data.SQL;
using Vanrise.Entities;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class VRTimeZoneDBSyncDataManager : BaseSQLDataManager, IDBSyncDataManager
    {
        string _TableName = Vanrise.Common.Utilities.GetEnumDescription(DBTableName.VRTimeZone);
        string _Schema = "Common";
        bool _UseTempTables;
        public VRTimeZoneDBSyncDataManager(bool useTempTables) :
            base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnString"))
        {
            _UseTempTables = useTempTables;
        }

        public void ApplyTimeZonesToTemp(List<VRTimeZone> timeZones, long startingId)
        {
            DataTable dt = new DataTable();
            dt.TableName = MigrationUtils.GetTableName(_Schema, _TableName, _UseTempTables);
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Settings", typeof(string));
            dt.Columns.Add("SourceID", typeof(string));
            dt.Columns.Add("ID", typeof(int));

            dt.BeginLoadData();
            foreach (var item in timeZones)
            {
                DataRow row = dt.NewRow();
                row["Name"] = item.Name;
                row["Settings"] = Vanrise.Common.Serializer.Serialize(item.Settings);
                row["SourceID"] = item.SourceId;
                row["ID"] = startingId++;
                dt.Rows.Add(row);
            }
            dt.EndLoadData();
            WriteDataTableToDB(dt, System.Data.SqlClient.SqlBulkCopyOptions.KeepNulls);
        }

        public Dictionary<string, VRTimeZone> GetVrTimeZones(bool useTempTables)
        {
            return GetItemsText(string.Format("SELECT [ID] ,[Name] ,[Settings] ,[SourceID] FROM {0} where sourceid is not null"
                , MigrationUtils.GetTableName(_Schema, _TableName, useTempTables)), VrTimeZoneMapper, cmd => { }).ToDictionary(x => x.SourceId, x => x);
        }

        public VRTimeZone VrTimeZoneMapper(IDataReader reader)
        {
            VRTimeZone country = new VRTimeZone
            {
                TimeZoneId = (int)reader["ID"],
                Name = reader["Name"] as string,
                Settings = Vanrise.Common.Serializer.Deserialize<VRTimeZoneSettings>(reader["Settings"] as string),
                SourceId = reader["SourceID"] as string,
            };

            return country;
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
