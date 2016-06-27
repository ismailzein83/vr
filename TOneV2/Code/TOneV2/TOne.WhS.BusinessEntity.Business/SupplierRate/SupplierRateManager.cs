﻿using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.Common.Business;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SupplierRateManager
    {

        #region Public Methods
        public Vanrise.Entities.IDataRetrievalResult<SupplierRateDetail> GetFilteredSupplierRates(Vanrise.Entities.DataRetrievalInput<SupplierRateQuery> input)
        {
            return BigDataManager.Instance.RetrieveData(input, new SupplierRateRequestHandler());
        }
        public List<SupplierRate> GetSupplierRatesEffectiveAfter(int supplierId, DateTime minimumDate)
        {
            ISupplierRateDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierRateDataManager>();
            return dataManager.GetSupplierRates(supplierId, minimumDate);
        }
        public List<SupplierRate> GetRates(DateTime? effectiveOn, bool isEffectiveInFuture, IEnumerable<RoutingSupplierInfo> supplierInfos)
        {
            ISupplierRateDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierRateDataManager>();
            return dataManager.GetEffectiveSupplierRatesBySuppliers(effectiveOn, isEffectiveInFuture, supplierInfos);
        }

        public SupplierZoneRate GetCachedSupplierZoneRate(int supplierId, long supplierZoneId, DateTime effectiveOn)
        {
            SupplierZoneRateLocator supplierZoneRateLocator = new SupplierZoneRateLocator(new SupplierRateReadWithCache(effectiveOn));
            return supplierZoneRateLocator.GetSupplierZoneRate(supplierId, supplierZoneId);
        }

        public CallCost GetCallCost(int supplierId, long supplierZoneId, int durationInSeconds, DateTime effectiveOn)
        {
            CallCost callSale = null;
            SupplierZoneRateLocator supplierZoneRateLocator = new SupplierZoneRateLocator(new SupplierRateReadWithCache(effectiveOn));

            SupplierZoneRate supplierZoneRate = supplierZoneRateLocator.GetSupplierZoneRate(supplierId, supplierZoneId);
            if (supplierZoneRate != null)
            {
                int currencyId = supplierZoneRate.Rate.CurrencyId.HasValue ? supplierZoneRate.Rate.CurrencyId.Value : supplierZoneRate.PriceList.CurrencyId;
                PurchasePricingRuleManager purchasePricingRuleManager = new PurchasePricingRuleManager();
                PurchasePricingRulesInput purchasePricingRulesInput = new PurchasePricingRulesInput
                {
                    SupplierId = supplierId,
                    SupplierZoneId = supplierZoneId,
                    Rate = supplierZoneRate.Rate,
                    DurationInSeconds = durationInSeconds,
                    EffectiveOn = effectiveOn
                };
                var pricingRulesResult = purchasePricingRuleManager.ApplyPricingRules(purchasePricingRulesInput);
                callSale = new CallCost
                {
                    RateValue = pricingRulesResult.Rate,
                    TotalNet = pricingRulesResult.TotalAmount,
                    CurrencyId = currencyId,
                    EffectiveDurationInSeconds = pricingRulesResult.EffectiveDurationInSeconds,
                    ExtraChargeValue = pricingRulesResult.ExtraChargeValue,
                    RateType = pricingRulesResult.RateType

                };
            }
            return callSale;
        }
        public long ReserveIDRange(int numberOfIDs)
        {
            long startingId;
            IDManager.Instance.ReserveIDRange(this.GetType(), numberOfIDs, out startingId);
            return startingId;
        }

        public int GetSupplierRateTypeId()
        {
            return Vanrise.Common.Business.TypeManager.Instance.GetTypeId(this.GetSupplierRateType());
        }

        public Type GetSupplierRateType()
        {
            return this.GetType();
        }
       
        #endregion

        #region Private Members
       
        private string GetCurrencyName(int? currencyId)
        {
            if (currencyId != null)
            {
                CurrencyManager manager = new CurrencyManager();
                Currency currency = manager.GetCurrency(currencyId.Value);

                if (currency != null)
                    return currency.Name;
            }

            return "Currency Not Found";
        }
        private string GetSupplierZoneName(long zoneId)
        {
            SupplierZoneManager manager = new SupplierZoneManager();
            SupplierZone suplierZone = manager.GetSupplierZone(zoneId);

            if (suplierZone != null)
                return suplierZone.Name;

            return "Zone Not Found";
        }
        #endregion

        #region Mappers
        private SupplierRateDetail SupplierRateDetailMapper(SupplierRate supplierRate)
        {
            CurrencyManager currencyManager = new CurrencyManager();
            int currencyId;

            if (supplierRate.CurrencyId.HasValue)
                currencyId = supplierRate.CurrencyId.Value;
            else
            {
                SupplierPriceListManager manager = new SupplierPriceListManager();
                SupplierPriceList priceList = manager.GetPriceList(supplierRate.PriceListId);
                currencyId= priceList.CurrencyId;
            }

            return new SupplierRateDetail()
            {
                Entity = supplierRate,
                CurrencyName = currencyManager.GetCurrencySymbol(currencyId),
                SupplierZoneName = this.GetSupplierZoneName(supplierRate.ZoneId),
            };
        }
        #endregion

        #region Private Classes

        private class SupplierRateRequestHandler : BigDataRequestHandler<SupplierRateQuery, SupplierRate, SupplierRateDetail>
        {
            public override SupplierRateDetail EntityDetailMapper(SupplierRate entity)
            {
                SupplierRateManager manager = new SupplierRateManager();
                return manager.SupplierRateDetailMapper(entity);
            }

            public override IEnumerable<SupplierRate> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<SupplierRateQuery> input)
            {
                ISupplierRateDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierRateDataManager>();
                return dataManager.GetFilteredSupplierRates(input.Query);
            }
        }

        #endregion
    }
}
