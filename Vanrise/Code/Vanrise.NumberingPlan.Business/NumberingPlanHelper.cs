using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.NumberingPlan.Entities;

namespace Vanrise.NumberingPlan.Business
{
    public static class NumberingPlanHelper
    {
        public static HashSet<string> GetExistingCodes(IEnumerable<ExistingZone> existingZones)
        {
            if (existingZones == null)
                throw new ArgumentNullException("existingZones");

            HashSet<string> distinctCodes = new HashSet<string>();
            foreach (ExistingZone zone in existingZones)
            {
                foreach (ExistingCode code in zone.ExistingCodes)
                {
                    distinctCodes.Add(code.CodeEntity.Code);
                }
            }

            return distinctCodes;
        }

        public static bool IsSaleZoneNational(long zoneId)
        {
            SaleZoneManager saleZoneManager = new SaleZoneManager();
            int countryId = saleZoneManager.GetSaleZoneCountryId(zoneId);

            Vanrise.Common.Business.ConfigManager commonConfigManager = new Common.Business.ConfigManager();
            return commonConfigManager.IsCountryNational(countryId);
        }
    }
}
