using QM.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace QM.CLITester.iTestIntegration
{
    public class ZoneITestReader : SourceZoneReader
    {
        public override bool UseSourceItemId
        {
            get { return false; }
        }

        public override IEnumerable<SourceZone> GetChangedItems(ref object updatedHandle)
        {
            ITestZoneManager zoneManager = new ITestZoneManager();
            var itestZones = zoneManager.GetAllZones();
            if (itestZones != null)
            {
                List<SourceZone> zones = new List<SourceZone>();
                foreach (var z in itestZones.Values)
                {
                    var zone = new SourceZone
                    {
                        IsFromTestingConnectorZone = true,
                        SourceId = z.ZoneId,
                        Name = z.ZoneName,
                        SourceCountryId = z.CountryId,
                        CountryName = z.CountryName
                    };
                    zones.Add(zone);
                }
                return zones;
            }
            else
                return null;
        }

        public override IEnumerable<SourceZoneCode> GetAllCodes()
        {
            return null;
        }
    }
}
