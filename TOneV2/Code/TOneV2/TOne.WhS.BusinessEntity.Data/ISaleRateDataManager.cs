﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.BusinessEntity.Data
{
    public interface ISaleRateDataManager : IDataManager
    {
        Vanrise.Entities.BigResult<Entities.SaleRate> GetSaleRateFilteredFromTemp(Vanrise.Entities.DataRetrievalInput<Entities.SaleRateQuery> input);
        List<SaleRate> GetEffectiveSaleRates(SalePriceListOwnerType ownerType, int ownerId, DateTime effectiveOn);
        List<SaleRate> GetEffectiveSaleRateByCustomers(IEnumerable<RoutingCustomerInfo> customerInfos, DateTime? effectiveOn, bool isEffectiveInFuture);
        bool AreSaleRatesUpdated(ref object updateHandle);
        bool CloseRates(IEnumerable<RateChange> rateChanges);
        bool InsertRates(IEnumerable<NewRate> newRates, int priceListId);
        IEnumerable<SaleRate> GetExistingRatesByZoneIds(SalePriceListOwnerType ownerType, int ownerId, IEnumerable<long> zoneIds, DateTime minEED);
    }
}
