using QM.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.BusinessEntity.Business
{
    public class ZoneManager
    {
        public IEnumerable<ZoneInfo> GetCountryZones(int supplierId, string countryId)
        {
            var countryReader = ReaderFactory.GetCountryReader();
            var country = countryReader.GetCountry(countryId);
            if (country == null)
                return null;
            var zoneReader = ReaderFactory.GetZoneReader();
            var zones = zoneReader.GetZones(supplierId, country);
            if (zones == null)
                return null;
            else
                return zones.Select(z =>
                    new ZoneInfo
                    {
                       // ZoneId = z.ZoneId,
                        Name = z.Name
                    });
        }

        internal static void ReserveIDRange(int nbOfIds, out long startingId)
        {
            Vanrise.Common.Business.IDManager.Instance.ReserveIDRange(typeof(ZoneManager), nbOfIds, out startingId);
        }
    }
}
