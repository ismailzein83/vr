using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data
{
    public interface ISaleRateDataManager : IDataManager
    {
        List<SaleRate> GetEffectiveSaleRates(DateTime effectiveOn);
        List<SaleRate> GetSaleRatesEffectiveAfter(int sellingNumberPlanId, DateTime minimumDate);
        IEnumerable<SaleRate> GetSaleRatesEffectiveAfter(SalePriceListOwnerType ownerType, int ownerId, DateTime minimumDate);
        List<SaleRate> GetSaleRatesInBetweenPeriod(DateTime fromTime, DateTime tillTime);
        List<SaleRate> GetEffectiveSaleRateByOwner(IEnumerable<RoutingCustomerInfoDetails> customerInfos, DateTime? effectiveOn, bool isEffectiveInFuture);
        List<SaleRate> GetEffectiveAfterByMultipleOwners(IEnumerable<RoutingCustomerInfoDetails> customerInfos, DateTime effectiveAfter);
        bool AreSaleRatesUpdated(ref object updateHandle);
        IEnumerable<SaleRate> GetExistingRatesByZoneIds(SalePriceListOwnerType ownerType, int ownerId, IEnumerable<long> zoneIds, DateTime minEED);
        IEnumerable<SaleRate> GetFutureSaleRates(SalePriceListOwnerType ownerType, int ownerId);
        SaleRate GetSaleRateById(long rateId);
        IEnumerable<SaleRate> GetSaleRatesEffectiveAfterByOwnerAndZones(SalePriceListOwnerType ownerType, int ownerId, IEnumerable<long> zoneIds, DateTime effectiveOn);
        IEnumerable<SaleRate> GetZoneRatesBySellingProduct(int sellingProductId, IEnumerable<long> saleZoneIds);
        IEnumerable<SaleRate> GetZoneRatesBySellingProducts(IEnumerable<int> sellingProductIds, IEnumerable<long> saleZoneIds);
        IEnumerable<SaleRate> GetAllSaleRatesByOwner(SalePriceListOwnerType ownerType, int ownerId, IEnumerable<long> saleZoneIds, bool getNormalRates, bool getOtherRates);
        IEnumerable<SaleRate> GetAllSaleRatesBySellingProductAndCustomer(IEnumerable<long> saleZoneIds, int sellingProductId, int customerId, bool getNormalRates, bool getOtherRates);
        IEnumerable<SaleRate> GetSaleRatesEffectiveAfterByOwnersAndZones(SalePriceListOwnerType ownerType, IEnumerable<int> ownerIds, IEnumerable<long> zoneIds, DateTime minimumDate);
        DateTime? GetNextOpenOrCloseTime(DateTime effectiveDate);
        object GetMaximumTimeStamp();
        IEnumerable<SaleRate> GetAllSaleRatesByOwners(IEnumerable<int> sellingProductIds, IEnumerable<int> customerIds, IEnumerable<long> zoneIds, bool getNormalRates, bool getOtherRates);
        IEnumerable<SaleRate> GetAllSaleRatesByOwnerType(SalePriceListOwnerType ownerType, IEnumerable<int> ownerIds, IEnumerable<long> zoneIds, bool getNormalRates, bool getOtherRates);
    }
}
