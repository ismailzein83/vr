using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;
using TOne.WhS.Sales.Entities.RateManagement;
using Vanrise.Common;

namespace TOne.WhS.Sales.Business
{
    public class PriceListRateManager
    {
        public void ProcessNewRate(NewRate newRate, IEnumerable<ExistingRate> existingZoneRates)
        {
            if(existingZoneRates != null)
            {
                foreach(var existingRate in existingZoneRates)
                {
                    if(existingRate.IsOverlapedWith(newRate))
                    {
                        DateTime existingRateEED = Utilities.Max(newRate.BED, existingRate.BED);
                        existingRate.ChangedRate = new ChangedRate
                        {
                            RateId = existingRate.RateEntity.SaleRateId,
                            EED = existingRateEED
                        };
                        newRate.ChangedExistingRates.Add(existingRate);
                    }
                }
            }
        }
    }
}
