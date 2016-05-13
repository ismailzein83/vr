using System;
using System.Collections.Generic;
using System.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class CarrierProfileDBSyncDataManager : BaseSQLDataManager
    {
        readonly string[] columns = { "Settings", "Name", "SourceID" };
        bool _UseTempTables;
        public CarrierProfileDBSyncDataManager(bool useTempTables) :
            base(GetConnectionStringName("TOneWhS_BE_MigrationDBConnStringKey", "TOneV2MigrationDBConnString"))
        {
            _UseTempTables = useTempTables;
        }

        public void ApplyCarrierProfilesToTemp(List<CarrierProfile> carrierProfiles)
        {
            string filePath = GetFilePathForBulkInsert();
            using (System.IO.StreamWriter wr = new System.IO.StreamWriter(filePath))
            {
                foreach (var c in carrierProfiles)
                {
                    wr.WriteLine(String.Format("{0}^{1}^{2}", c.Settings, c.Name, c.SourceId));
                }
                wr.Close();
            }

            Object preparedCarrierProfiles = new BulkInsertInfo
            {
                TableName = "[TOneWhS_BE].[CarrierProfile" + (_UseTempTables ? Constants._Temp : "") + "]",
                DataFilePath = filePath,
                ColumnNames = columns,
                TabLock = true,
                KeepIdentity = true,
                FieldSeparator = '^',
            };

            InsertBulkToTable(preparedCarrierProfiles as BaseBulkInsertInfo);
        }

        public List<CarrierProfile> GetCarrierProfiles()
        {
            return GetItemsText("SELECT [ID] ,[Settings]  ,[Name] ,[SourceID] FROM [TOneWhS_BE].[CarrierProfile" + (_UseTempTables ? Constants._Temp : "") + "] ", CarrierProfileMapper, cmd => { });
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

    }
}
