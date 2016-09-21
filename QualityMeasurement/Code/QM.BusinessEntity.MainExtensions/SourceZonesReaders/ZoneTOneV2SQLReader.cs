using QM.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.BusinessEntity.MainExtensions.SourceZonesReaders
{
    public class ZoneTOneV2SQLReader : SourceZoneReader 
    {
        public override Guid ConfigId { get { return new Guid("71faa2f0-e8b5-4e32-9edf-53c31db8ab6a"); } }
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
                    SourceId = arg["ID"].ToString(),
                    SourceCountryId = arg["CountryID"].ToString(),
                    Name = arg["Name"] as string,
                    CountryName = " ",
                    BeginEffectiveDate = GetReaderValue<DateTime>(arg, "BED"),
                    EndEffectiveDate = GetReaderValue<DateTime>(arg, "EED")

                };
                return sourceZone;
            }

            const string query_getUpdatedZones = @"SELECT [ID]
              ,[CountryID]
              ,[Name]
              ,[BED]
              ,[EED]
               FROM [TOneWhS_BE].[SaleZone]
               where [CountryID] > 0";
        }

        public override IEnumerable<SourceZoneCode> GetAllCodes()
        {
            throw new NotImplementedException();
        }
    }
}
