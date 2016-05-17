﻿using System;
using System.Collections.Generic;
using System.Data;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Data.SQL.Common;
using Vanrise.Data.SQL;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class CarrierProfileDBSyncDataManager : BaseSQLDataManager
    {
        readonly string[] columns = { "Settings", "Name", "SourceID" };
        string _TableName = Vanrise.Common.Utilities.GetEnumDescription(DBTableName.CarrierProfile);
        string _Schema = "TOneWhS_BE";
        bool _UseTempTables;
        public CarrierProfileDBSyncDataManager(bool useTempTables) :
            base(GetConnectionStringName("TOneWhS_BE_MigrationDBConnStringKey", "TOneV2DBConnString"))
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
                    wr.WriteLine(String.Format("{0}^{1}^{2}", Vanrise.Common.Serializer.Serialize(c.Settings), c.Name, c.SourceId));
                }
                wr.Close();
            }

            Object preparedCarrierProfiles = new BulkInsertInfo
            {
                TableName = MigrationUtils.GetTableName(_Schema, _TableName, _UseTempTables),
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
            return GetItemsText("SELECT [ID] ,[Settings]  ,[Name] ,[SourceID] FROM" + MigrationUtils.GetTableName(_Schema, _TableName, _UseTempTables), CarrierProfileMapper, cmd => { });
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
