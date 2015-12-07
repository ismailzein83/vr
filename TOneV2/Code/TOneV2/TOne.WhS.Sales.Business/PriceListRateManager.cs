using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities.RateManagement;
using Vanrise.Common;

namespace TOne.WhS.Sales.Business
{
    public class PriceListRateManager
    {
        public void ProcessNewRate(NewRate newRate, List<ExistingRate> existingZoneRates)
        {
            if(existingZoneRates != null)
            {
                foreach(var existingRate in existingZoneRates)
                {
                    if(existingRate.EED.VRGreaterThan(newRate.BED) && newRate.EED.VRGreaterThan(existingRate.RateEntity.BED))
                    {
                        DateTime existingRateEED = newRate.BED > existingRate.RateEntity.BED ? newRate.BED : existingRate.RateEntity.BED;
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
