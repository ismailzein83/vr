using QM.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.BusinessEntity.MainExtensions.SourceZonesReaders
{
    public class ZoneTOneV1SQLReader : SourceZoneReader 
    {
        public override Guid ConfigId { get { return new Guid("3b68fae1-c18f-4d15-8c56-74e12a56fd47"); } }

        public string ConnectionString { get; set; }
        public string ZoneNames { get; set; }

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
            List<SourceZone> listSourceZones = dataManager.GetUpdatedZones(ref updatedHandle);
            List<SourceZone> listZones = new List<SourceZone>();
            List<string> mobileZones = (ZoneNames).ToLower().Split(',').ToList();

            for (int i = 0; i < listSourceZones.Count; i++)
            {
                if (listSourceZones[i].IsMobile)
                    listZones.Add(listSourceZones[i]);
                else
                    if (mobileZones.Any(w => listSourceZones[i].Name.ToLower().Contains(w)))
                        listZones.Add(listSourceZones[i]);
            }
                
            return listZones;
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

            public IEnumerable<SourceZoneCode> GetAllCodes()
            {
                return GetItemsText(query_getAllZones, SourceZonerCodeMapper, null);
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
                    EndEffectiveDate = GetReaderValue<DateTime>(arg, "EndEffectiveDate"),
                    IsMobile = (arg["IsMobile"] as string == "Y")
                };
                return sourceZone;
            }

            private SourceZoneCode SourceZonerCodeMapper(System.Data.IDataReader arg)
            {
                SourceZoneCode sourceZoneCode = new SourceZoneCode()
                {
                    SourceZoneId = arg["ZoneID"].ToString(),
                    Code = arg["Code"].ToString(),
                };
                return sourceZoneCode;
            }


            const string query_getUpdatedZones = @"SELECT [ZoneID]
                  ,[CodeGroup]
                  ,z.[Name] As ZoneName
                  ,c.[Name] as CountryName
                  ,[BeginEffectiveDate]
                  ,[EndEffectiveDate]
                  ,[IsMobile]
                FROM [dbo].[Zone] z join [dbo].[CodeGroup] c on z.CodeGroup = c.Code where ZoneID > 0 and z.SupplierID ='SYS' and IsEffective = 'Y' ";

            private const string query_getAllZones = @"SELECT [Code] ,[ZoneID] FROM [dbo].[Code] where ZoneID > 0 and IsEffective = 'Y'";
        }

        public override IEnumerable<SourceZoneCode> GetAllCodes()
        {
            DataManager dataManager = new DataManager(this.ConnectionString);
            return dataManager.GetAllCodes();
        }
    }
}
