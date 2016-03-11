﻿using QM.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.BusinessEntity.MainExtensions.SourceZonesReaders
{
    public class ZoneTOneV1SQLReader : SourceZoneReader 
    {
        public string ConnectionString { get; set; }

        public override bool UseSourceItemId
        {
            get
            {
                return true;
            }
        }

        public override IEnumerable<SourceZone> GetChangedItems(ref object updatedHandle)
        {
            DataManager dataManager = new DataManager(this.ConnectionString);
            return dataManager.GetUpdatedZones(ref updatedHandle);
        }

        private class DataManager : Vanrise.Data.SQL.BaseSQLDataManager
        {
            public DataManager(string connectionString)
                : base(connectionString, false)
            {
            }

            public List<SourceZone> GetUpdatedZones(ref object updateHandle)
            {
                return GetItemsText(query_getUpdatedZones, SourceZonerMapper, null);
            }

            private SourceZone SourceZonerMapper(System.Data.IDataReader arg)
            {
                SourceZone sourceZone = new SourceZone()
                {
                    SourceId = arg["ZoneID"].ToString(),
                    SourceCountryId = arg["CodeGroup"].ToString(),
                    Name = arg["ZoneName"] as string,
                    CountryName = arg["CountryName"] as string,
                    BeginEffectiveDate = GetReaderValue<DateTime>(arg, "BeginEffectiveDate"),
                    EndEffectiveDate = GetReaderValue<DateTime>(arg, "EndEffectiveDate")

                };
                return sourceZone;
            }

            const string query_getUpdatedZones = @"SELECT [ZoneID]
                  ,[CodeGroup]
                  ,z.[Name] As ZoneName
                  ,c.[Name] as CountryName
                  ,[BeginEffectiveDate]
                  ,[EndEffectiveDate]
                FROM [dbo].[Zone] z join [dbo].[CodeGroup] c on z.CodeGroup = c.Code where ZoneID > 0 and z.SupplierID ='SYS' ";
        }

        public override IEnumerable<SourceZoneCode> GetAllCodes()
        {
            throw new NotImplementedException();
        }
    }
}
