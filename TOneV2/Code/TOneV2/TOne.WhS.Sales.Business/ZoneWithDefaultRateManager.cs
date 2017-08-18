using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;
using Vanrise.Common.Business;
using Vanrise.Common;
namespace TOne.WhS.Sales.Business
{
   public class ZoneWithDefaultRateManager
    {
       public Vanrise.Entities.IDataRetrievalResult<SaleZoneDetail> GetFilteredZones(Vanrise.Entities.DataRetrievalInput<ZoneWithDefaultRateQuery> input)
        {
            SaleZoneManager zoneManager = new SaleZoneManager();
            RatePlanDraftManager ratePlanDraftManager = new RatePlanDraftManager();
            IEnumerable<SaleZone> allZones = zoneManager.GetAllZones();
            List<long> zonesWithDefaultRates;
            if (ratePlanDraftManager.GetSellingZonesWithDefaultRatesTaskData(input.Query.OwnerType, input.Query.OwnerId).ZoneIdsWithDefaultRatesByCountryIds.TryGetValue(input.Query.CountryId, out zonesWithDefaultRates))
            {
                Func<SaleZone, bool> filterExpression = (prod) =>
            (zonesWithDefaultRates.Contains(prod.SaleZoneId));
                return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allZones.ToBigResult(input, filterExpression, SaleZoneDetailMapper), null);
            }
            return null;
        }


       private SaleZoneDetail SaleZoneDetailMapper(SaleZone saleZone)
       {
           SaleZoneDetail saleZoneDetail = new SaleZoneDetail();

           saleZoneDetail.Entity = saleZone;

           return saleZoneDetail;
       }
    }
}
