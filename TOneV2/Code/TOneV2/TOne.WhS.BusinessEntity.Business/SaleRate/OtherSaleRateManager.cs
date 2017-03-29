using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class OtherSaleRateManager
    {
        //public IEnumerable<SaleRateDetail> GetOtherSaleRates(SaleRateQuery query)
        //{
        //    IEnumerable<long> zoneIds = null;

        //    if (query.OwnerType == SalePriceListOwnerType.SellingProduct)
        //    {
        //        var sellingProductZoneRateHistoryLocator = new SellingProductZoneRateHistoryLocator(new SellingProductZoneRateHistoryReader(query.OwnerId, zoneIds, false, true));
        //        var rateTypeManager = new Vanrise.Common.Business.RateTypeManager().GetAllRateTypes();
        //    }
        //    else
        //    {

        //    }
        //}
    }

    public class OtherSaleRateQuery
    {
        public string ZoneName { get; set; }

        public SalePriceListOwnerType OwnerType { get; set; }

        public int OwnerId { get; set; }

        public DateTime EffectiveOn { get; set; }

        public int CurrencyId { get; set; }
    }
}
