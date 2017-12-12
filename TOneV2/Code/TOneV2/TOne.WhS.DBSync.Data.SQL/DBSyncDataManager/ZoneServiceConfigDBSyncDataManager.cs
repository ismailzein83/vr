using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Entities;
using Vanrise.Data.SQL;
using Vanrise.Entities;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class ZoneServiceConfigDBSyncDataManager : BaseSQLDataManager, IDBSyncDataManager
    {
        string _TableName = Vanrise.Common.Utilities.GetEnumDescription(DBTableName.ZoneServiceConfig);
        string _Schema = "TOneWhS_BE";
        bool _UseTempTables;
        public ZoneServiceConfigDBSyncDataManager(bool useTempTables) :
            base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {
            _UseTempTables = useTempTables;
        }

        public void ApplyZoneServicesConfigToTemp(List<ZoneServiceConfig> zoneServicesConfig)
        {
            DataTable dt = new DataTable();
            dt.TableName = MigrationUtils.GetTableName(_Schema, _TableName, _UseTempTables);
            dt.Columns.Add("Symbol", typeof(string));
            dt.Columns.Add("Settings", typeof(string));
            dt.Columns.Add("SourceID", typeof(string));
            dt.Columns.Add("ID", typeof(int));

            dt.BeginLoadData();
            foreach (var item in zoneServicesConfig)
            {
                DataRow row = dt.NewRow();
                row["Symbol"] = item.Symbol;
                row["Settings"] = Vanrise.Common.Serializer.Serialize(item.Settings);
                row["SourceID"] = item.SourceId;
                row["ID"] = item.ZoneServiceConfigId;
                dt.Rows.Add(row);
            }
            dt.EndLoadData();
            WriteDataTableToDB(dt, System.Data.SqlClient.SqlBulkCopyOptions.KeepNulls);
        }

        public Dictionary<string, ZoneServiceConfig> GetZoneServicesConfig(bool useTempTables)
        {
            return GetItemsText(string.Format("SELECT [ID] , [Symbol], [Settings], [SourceID] FROM {0} where sourceid is not null"
                , MigrationUtils.GetTableName(_Schema, _TableName, useTempTables)), ZoneServiceConfigMapper, cmd => { }).ToDictionary(x => x.SourceId, x => x);
        }

        public ZoneServiceConfig ZoneServiceConfigMapper(IDataReader reader)
        {
            return new ZoneServiceConfig
            {
                ZoneServiceConfigId = (int)reader["ID"],
                Settings = Vanrise.Common.Serializer.Deserialize<ServiceConfigSetting>(reader["Settings"] as string),
                SourceId = reader["SourceID"] as string,
                Symbol = reader["Symbol"] as string
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
