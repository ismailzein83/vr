using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class SwitchConnectivityDBSyncDataManager : BaseSQLDataManager, IDBSyncDataManager
    {
        string _TableName = Vanrise.Common.Utilities.GetEnumDescription(DBTableName.SwitchConnectivity);
        string _Schema = "TOneWhS_BE";
        bool _UseTempTables;
        public SwitchConnectivityDBSyncDataManager(bool useTempTables) :
            base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {
            _UseTempTables = useTempTables;
        }

        public void ApplySwitchesConnectivityToTemp(List<SwitchConnectivity> switchesConnectivity)
        {
            DataTable dt = new DataTable();
            dt.TableName = MigrationUtils.GetTableName(_Schema, _TableName, _UseTempTables);
            dt.Columns.Add("SourceID", typeof(string));
            dt.Columns.Add("CarrierAccountID", typeof(int));
            dt.Columns.Add("SwitchID", typeof(int));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("BED", typeof(DateTime));
            dt.Columns.Add(new DataColumn { AllowDBNull = true, ColumnName = "EED", DataType = typeof(DateTime) });
            dt.Columns.Add("Settings", typeof(string));
                  
            dt.BeginLoadData();
            foreach (var item in switchesConnectivity)
            {
                DataRow row = dt.NewRow();
                string serializedSettings = item.Settings != null ? Vanrise.Common.Serializer.Serialize(item.Settings) : null;
                row["SourceID"] = item.SourceId;
                row["CarrierAccountID"] = item.CarrierAccountId;
                row["SwitchID"] = item.SwitchId;
                row["Name"] = item.Name;
                row["BED"] = item.BED;
                row["EED"] = item.EED.HasValue ? (object)item.EED.Value : DBNull.Value;
                row["Settings"] = serializedSettings;

                dt.Rows.Add(row);
            }
            dt.EndLoadData();
            WriteDataTableToDB(dt, System.Data.SqlClient.SqlBulkCopyOptions.KeepNulls);
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
