using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.MainExtensions.CodeList
{
    public class SpecificCountryCodeListResolver : CodeListResolverSettings
    {
        public override Guid ConfigId { get { return new Guid("A3B95375-BB41-4843-A29D-532E402A2421"); } }
        public List<int> CountryIds { get; set; }
        public int SellingNumberPlanId { get; set; }
        public ExcludedDestinations ExcludedDestinations { get; set; }

        public override List<string> GetCodeList(ICodeListResolverContext context)
        {
            List<string> codes = new List<string>();
            List<long> zonneIds = new List<long>();
            SaleCodeManager saleCodeManager = new SaleCodeManager();
            SaleZoneManager saleZoneManager = new SaleZoneManager();
            IEnumerable<SaleZone>saleZones=saleZoneManager.GetEffectiveSaleZonesByCountryIds(SellingNumberPlanId, CountryIds, DateTime.Now, true);
           List<string> excludedCodes = (ExcludedDestinations!=null)? this.ExcludedDestinations.GetExcludedCodes(null):new List<string>();
            foreach (var item in saleZones)
                zonneIds.Add((long)item.SaleZoneId);

            List<SaleCode> saleCodes = saleCodeManager.GetSaleCodesByZoneIDs(zonneIds, DateTime.Now);
            foreach (var item in saleCodes)
                if (!excludedCodes.Contains(item.Code))
                  codes.Add(item.Code);
            return codes;
        }

    }
}
