using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class CarrierProfileDBSyncDataManager : BaseSQLDataManager, IDBSyncDataManager
    {
        string _TableName = Vanrise.Common.Utilities.GetEnumDescription(DBTableName.CarrierProfile);
        string _Schema = "TOneWhS_BE";
        bool _UseTempTables;
        public CarrierProfileDBSyncDataManager(bool useTempTables) :
            base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {
            _UseTempTables = useTempTables;
        }


        public void ApplyCarrierProfilesToTemp(List<CarrierProfile> carrierProfiles)
        {
            DataTable dt = new DataTable();
            dt.TableName = MigrationUtils.GetTableName(_Schema, _TableName, _UseTempTables);
            dt.Columns.Add("Settings", typeof(string));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("SourceID", typeof(string));
            dt.Columns.Add("IsDeleted", typeof(bool));
            dt.BeginLoadData();
            foreach (var item in carrierProfiles)
            {
                DataRow row = dt.NewRow();
                int index = 0;
                row[index++] = Vanrise.Common.Serializer.Serialize(item.Settings);
                row[index++] = item.Name;
                row[index++] = item.SourceId;
                row[index++] = item.IsDeleted;
                dt.Rows.Add(row);
            }
            dt.EndLoadData();
            WriteDataTableToDB(dt, System.Data.SqlClient.SqlBulkCopyOptions.KeepNulls);
        }

        public Dictionary<string, CarrierProfile> GetCarrierProfiles(bool useTempTables)
        {
            return GetItemsText(string.Format("SELECT [ID] ,[Settings]  ,[Name] ,[SourceID] FROM {0} where sourceId is not null",
                 MigrationUtils.GetTableName(_Schema, _TableName, useTempTables)), CarrierProfileMapper, cmd => { }).ToDictionary(x => x.SourceId, x => x);
        }

        private CarrierProfile CarrierProfileMapper(IDataReader reader)
        {
            CarrierProfile carrierProfile = new CarrierProfile
            {
                CarrierProfileId = (int)reader["ID"],
                Name = reader["Name"] as string,
                Settings = Vanrise.Common.Serializer.Deserialize<CarrierProfileSettings>(reader["Settings"] as string),
                SourceId = reader["SourceID"] as string,
            };
            return carrierProfile;
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
