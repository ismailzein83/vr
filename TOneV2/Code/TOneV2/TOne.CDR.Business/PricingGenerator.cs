using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Business;
using TOne.BusinessEntity.Entities;
using TOne.Caching;
using TOne.CDR.Entities;
using TOne.Entities;

namespace TOne.CDR.Business
{
    public class PricingGenerator : IDisposable
    {

        TOneCacheManager _cacheManager;

        public PricingGenerator(TOneCacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }

        public T GetRepricing<T>(BillingCDRMain main) where T : BillingCDRPricingBase, new()
        {
            T pricing = new T();

            bool cost = pricing is BillingCDRCost;

            int zoneId = (cost) ? main.SupplierZoneID : main.OurZoneID;

            TABS.CarrierAccount supplier;
            TABS.CarrierAccount customer;

            if (!TABS.CarrierAccount.All.TryGetValue(main.CustomerID, out customer)) return null;

            if (!TABS.CarrierAccount.All.TryGetValue(main.SupplierID, out supplier)) return null;

            IList<Rate> rates;
            if (cost)
                rates = GetRates(TABS.CarrierAccount.SYSTEM.CarrierAccountID, zoneId, main.Attempt.AddMinutes(supplier.SupplierGMTTime));
            else
                rates = GetRates(main.CustomerID, zoneId, main.Attempt);

            // If a rate is defined for
            if (rates != null && rates.Count > 0)
            {
                // Initialize Pricing
                pricing.BillingCDRMainID = main.ID;
                pricing.RateID = rates[0].RateId;
                pricing.ZoneID = rates[0].ZoneId;
                pricing.RateValue = (double)rates[0].NormalRate;
                pricing.Net = (double)(rates[0].NormalRate * (main.DurationInSeconds / 60m));
                pricing.CurrencySymbol = rates[0].CurrencyID;
                pricing.DurationInSeconds = main.DurationInSeconds;

                Rate rate = rates[0];

                TABS.Currency rateCurrency = TABS.Currency.All.Keys.Contains(pricing.CurrencySymbol) ? TABS.Currency.All[pricing.CurrencySymbol] : new TABS.Currency();

                // Usables...
                ToDConsideration tod = null;
                Tariff tariff = null;

                #region Get Usables

                // Effective and Active ToD for this Call?
                IList<ToDConsideration> tods = GetToDConsiderations(cost ? TABS.CarrierAccount.SYSTEM.CarrierAccountID : main.CustomerID, pricing.ZoneID, main.Attempt);

                // If ToD Considered, the rate applied should be changed
                foreach (ToDConsideration effective in tods) { if (effective.WasActive(main.Attempt)) { tod = effective; break; } }

                // Check for ToD first
                if (tod != null)
                {
                    pricing.RateType = tod.RateType;
                    pricing.RateValue = (double)tod.ActiveRateValue(rates[0]);
                    pricing.ToDConsiderationID = tod.ToDConsiderationID;
                }
                else
                    pricing.RateType = ToDRateType.Normal;

                var attemptDate = new DateTime(main.Attempt.Year, main.Attempt.Month, main.Attempt.Day);
                pricing.Attempt = attemptDate;

                // Commissions or extra charges
                IList<Commission> commissionsAndExtraCharges = GetCommissions(cost ? TABS.CarrierAccount.SYSTEM.CarrierAccountID : main.CustomerID, pricing.ZoneID, main.Attempt);

                Commission commission = null;
                Commission extraCharge = null;

                foreach (Commission item in commissionsAndExtraCharges)
                {
                    var itemClone = (Commission)item.Clone();

                    var pricingValue = (float?)TABS.Rate.GetRate((decimal)pricing.RateValue, rateCurrency, cost ? supplier.CarrierProfile.Currency : customer.CarrierProfile.Currency, attemptDate);

                    if ((!item.FromRate.HasValue || item.FromRate <= pricingValue) && (!item.ToRate.HasValue || item.ToRate >= pricingValue))
                    {
                        if (item.IsExtraCharge && pricing.ExtraChargeID <= 0)
                        {
                            item.Amount = item.Amount != null ? item.Amount : (decimal)0.0; //in order if the commission value null apply the percentage and vise versa,also if the amount and perc found apply the two ammount then perc
                            //if (item.Amount != null && item.Amount != 0)
                            itemClone.Amount = (decimal?)TABS.Rate.GetRate(item.Amount.Value, cost ? supplier.CarrierProfile.Currency : customer.CarrierProfile.Currency, rateCurrency, attemptDate);
                            pricing.ExtraChargeID = itemClone.ID;

                            extraCharge = itemClone;
                        }

                        if (pricing.CommissionID <= 0)//(Removed from if Condition !item.IsExtraCharge && FOr bug 1958)
                        {
                            item.Amount = item.Amount != null ? item.Amount : (decimal)0.0;//in order if the commission value null apply the percentage and vise versa,also if the amount and perc found apply the two ammount then perc
                            //if (item.Amount != null && item.Amount != 0)
                            itemClone.Amount = (decimal?)TABS.Rate.GetRate(item.Amount.Value, cost ? supplier.CarrierProfile.Currency : customer.CarrierProfile.Currency, rateCurrency, attemptDate);
                            pricing.CommissionID = itemClone.ID;

                            commission = itemClone;
                        }
                    }
                }
                // Tariff Considered?
                IList<Tariff> tariffs = GetTariffs(cost ? TABS.CarrierAccount.SYSTEM.CarrierAccountID : main.CustomerID, zoneId, main.Attempt);

                Tariff tarrif = null;

                if (tariffs.Count > 0)
                {
                    tariff = tariffs[0];

                    var tariffClone = (Tariff)(tariff.Clone());

                    if (tariff.CallFee > 0)
                        tariffClone.CallFee = TABS.Rate.GetRate(tariff.CallFee, cost ? supplier.CarrierProfile.Currency : customer.CarrierProfile.Currency, rateCurrency, attemptDate);

                    if (tariff.FirstPeriodRate > 0)
                        tariffClone.FirstPeriodRate = TABS.Rate.GetRate(tariff.FirstPeriodRate, cost ? supplier.CarrierProfile.Currency : customer.CarrierProfile.Currency, rateCurrency, attemptDate);

                    pricing.TariffID = tariffClone.TariffID;
                    tarrif = tariffClone;
                }

                #endregion Get Usables

                // Calculate ...
                bool isCost = pricing is BillingCDRCost;
                pricing.Code = isCost ? main.SupplierCode : main.OurCode;
                CalculateAmounts(pricing, main.DurationInSeconds, customer.IsCustomerCeiling == "Y", supplier.IsSupplierCeiling == "Y", tarrif, tod, commission, rate, extraCharge);
            }
            else
            {
                // No suitable rate found for this Code / Supplier / Customer Combination
                pricing = null;
            }
            return pricing;
        }
        private double GetPricingNet(bool isCost, double rateValue, decimal durationInSeconds, bool isCustomerCeiling, bool isSupplierCeiling, out double accountedDuration)
        {
            accountedDuration = (double)durationInSeconds;
            double net = (rateValue * (double)durationInSeconds) / 60;

            //recalc in case of ceiling
            if (!isCost)
            {
                if (isCustomerCeiling)
                {
                    accountedDuration = (double)Math.Ceiling(durationInSeconds);
                    net = rateValue * (double)accountedDuration / 60.0;
                }

            }
            else
                if (isSupplierCeiling)
                {
                    accountedDuration = (double)Math.Ceiling(durationInSeconds);
                    net = rateValue * (double)accountedDuration / 60.0;
                }
            return net;
        }

        void CalculateAmounts(BillingCDRPricingBase pricing, decimal mainDurationInSeconds, bool isCustomerCeiling, bool isSupplierCeiling, Tariff tarrif,
            ToDConsideration ToDConsideration, Commission commission, Rate rate, Commission ExtraCharge)
        {
            bool isCost = pricing is BillingCDRCost;

            double accountedDuration = (double)mainDurationInSeconds;
            pricing.Net = GetPricingNet(isCost, pricing.RateValue, mainDurationInSeconds, isCustomerCeiling, isSupplierCeiling, out accountedDuration);

            // Tariff?
            if (tarrif != null)
            {
                pricing.Net = 0;

                // Calculate the amount for the firt period
                if (tarrif.FirstPeriod > 0)
                {
                    pricing.FirstPeriod = tarrif.FirstPeriod;
                    double firstPeriodRate = (tarrif.FirstPeriodRate > 0) ? (double)tarrif.FirstPeriodRate : pricing.RateValue;
                    if (!tarrif.RepeatFirstPeriod)
                    {
                        // Calculate first period amount then continue normally
                        pricing.Net = firstPeriodRate;//(tarrif.FirstPeriod * firstPeriodRate) / 60
                        accountedDuration -= (double)tarrif.FirstPeriod;
                        accountedDuration = Math.Max(0, accountedDuration);
                    }
                }

                // if there is a fraction unit
                if (tarrif.FractionUnit > 0)
                {
                    pricing.FractionUnit = (byte)tarrif.FractionUnit;
                    accountedDuration = Math.Ceiling(accountedDuration / tarrif.FractionUnit) * tarrif.FractionUnit;
                    pricing.Net += Math.Ceiling(accountedDuration / tarrif.FractionUnit) * pricing.RateValue;// 60
                }
                else// Calculate the net amount
                    pricing.Net += (accountedDuration * pricing.RateValue) / 60;

                // Calculate the Net from the Tariff
                pricing.Net += (double)tarrif.CallFee;
            }
            if (ToDConsideration != null)
                pricing.Discount = ((double)rate.NormalRate - ToDConsideration.ActiveRateValue(rate)) * (double)accountedDuration / 60;
            else
                pricing.Discount = 0;

            // Commission
            if (commission != null)
                pricing.CommissionValue = (pricing.RateValue - commission.DeductedRateValue(isCost, pricing.RateValue)) * (double)(mainDurationInSeconds) / 60;
            else
                pricing.CommissionValue = 0;

            // Extra Charge
            if (ExtraCharge != null)
                pricing.ExtraChargeValue = (pricing.RateValue - ExtraCharge.DeductedRateValue(isCost, pricing.RateValue)) * (double)accountedDuration / 60;
            else
                pricing.ExtraChargeValue = 0;

            if (tarrif != null && tarrif.FirstPeriod > 0 && !tarrif.RepeatFirstPeriod)
                accountedDuration += (double)tarrif.FirstPeriod;

            // updating the billing duration (if tarrif included)

            //if we have Tarrif accountedDuration Calculated with Ceiling value but when no Tarrif accountedDuration should also be assigned
            pricing.DurationInSeconds = (decimal)accountedDuration;
        }

        #region Cacheable Entities

        protected static TABS.Zone _AnyZone;
        protected static TABS.Zone AnyZone
        {
            get
            {
                lock (typeof(PricingGenerator))
                {
                    if (_AnyZone == null)
                    {
                        _AnyZone = new TABS.Zone();
                        _AnyZone.Name = "**** Any Zone ****";
                        _AnyZone.Supplier = null;
                    }
                }
                return _AnyZone;
            }
        }

        protected class DateSensitiveEntityCache<T> where T : IZoneSupplied
        {
            Dictionary<int, List<T>> _supplierEntities = new Dictionary<int, List<T>>();
            Dictionary<int, Dictionary<String, List<T>>> _ourEntities = new Dictionary<int, Dictionary<String, List<T>>>();

            public DateSensitiveEntityCache(DateTime pricingStart, bool IsRepricing)
            {
                Load(null, 0, pricingStart, IsRepricing);
            }

            public DateSensitiveEntityCache(string customerId, int zoneId, DateTime pricingStart, bool IsRepricing)
            {
                Load(customerId, zoneId, pricingStart, IsRepricing);
            }

            protected void Load(string customerId, int zoneId, DateTime pricingStart, bool IsRepricing)
            {
                IList<T> all = TOne.CDR.Business.PricingGeneratorEntities<T>.Load(customerId, zoneId, pricingStart, IsRepricing);
                foreach (T entity in all)
                {
                    TABS.CarrierAccount entityCustomer;
                    TABS.CarrierAccount entitySupplier;

                    if (!TABS.CarrierAccount.All.TryGetValue(entity.CustomerId, out entityCustomer)) continue;
                    if (!TABS.CarrierAccount.All.TryGetValue(entity.SupplierId, out entitySupplier)) continue;

                    int entityZoneID = (entity.ZoneId == null) ? AnyZone.ZoneID : entity.ZoneId;

                    // Our Entities
                    if (entitySupplier.Equals(TABS.CarrierAccount.SYSTEM))
                    {
                        Dictionary<String, List<T>> zoneEntities = null;
                        if (!_ourEntities.TryGetValue(entityZoneID, out zoneEntities))
                        {
                            zoneEntities = new Dictionary<String, List<T>>();
                            _ourEntities[entityZoneID] = zoneEntities;
                        }
                        List<T> zoneCustomerEntities = null;
                        if (!zoneEntities.TryGetValue(entityCustomer.CarrierAccountID, out zoneCustomerEntities))
                        {
                            zoneCustomerEntities = new List<T>();
                            zoneEntities[entityCustomer.CarrierAccountID] = zoneCustomerEntities;
                        }
                        zoneEntities[entityCustomer.CarrierAccountID].Add(entity);
                    }
                    // Supplier Entities
                    else
                    {
                        List<T> zoneEntities = null;
                        if (!_supplierEntities.TryGetValue(entityZoneID, out zoneEntities))
                        {
                            zoneEntities = new List<T>();
                            _supplierEntities[entityZoneID] = zoneEntities;
                        }
                        zoneEntities.Add(entity);
                    }
                }
            }

            public bool GetIsEffective(DateTime? BeginEffectiveDate, DateTime? EndEffectiveDate, DateTime when)
            {
                bool isEffective = BeginEffectiveDate.HasValue ? BeginEffectiveDate.Value <= when : true;
                if (isEffective)
                    isEffective = EndEffectiveDate.HasValue ? EndEffectiveDate.Value >= when : true;
                return isEffective;
            }

            public List<T> GetEffectiveEntities(String customerID, int zoneID, DateTime whenEffective)
            {
                List<T> effective = new List<T>();
                // Our Entities?
                if (!customerID.Equals(TABS.CarrierAccount.SYSTEM.CarrierAccountID))
                {
                    if (!_ourEntities.ContainsKey(zoneID)) zoneID = AnyZone.ZoneID;
                    if (_ourEntities.ContainsKey(zoneID))
                    {
                        if (_ourEntities[zoneID].ContainsKey(customerID))
                        {
                            List<T> entities = _ourEntities[zoneID][customerID];
                            foreach (T entity in entities)
                                if (GetIsEffective(entity.BeginEffectiveDate, entity.EndEffectiveDate, whenEffective))
                                    effective.Add(entity);
                        }
                    }
                }
                // Supplier Entities
                else
                {
                    if (!_supplierEntities.ContainsKey(zoneID)) zoneID = AnyZone.ZoneID;
                    if (_supplierEntities.ContainsKey(zoneID))
                    {
                        List<T> entities = _supplierEntities[zoneID];
                        foreach (T entity in entities)
                            if (GetIsEffective(entity.BeginEffectiveDate, entity.EndEffectiveDate, whenEffective))
                                effective.Add(entity);
                    }
                }
                return effective;
            }
        }

        protected DateTime _PricingStart = new DateTime(2000, 1, 1);

        public List<T> GetEffectiveEntities<T>(String customerID, int zoneID, DateTime whenEffective) where T : IZoneSupplied
        {
            return _cacheManager.GetOrCreateObject(String.Format("GetEffectiveEntities_{0}_{1}_{2:ddMMMyy}", typeof(T).Name, customerID, whenEffective.Date),
                CacheObjectType.Pricing,
                () =>
                {
                    DateSensitiveEntityCache<T> rates = null;
                    rates = new DateSensitiveEntityCache<T>(customerID, 0, whenEffective, true);
                    return rates;
                }).GetEffectiveEntities(customerID, zoneID, whenEffective);
        }

        public List<Rate> GetRates(String customerID, int zoneID, DateTime whenEffective)
        {
            return GetEffectiveEntities<Rate>(customerID, zoneID, whenEffective);
        }

        public List<ToDConsideration> GetToDConsiderations(String customerID, int zoneID, DateTime whenEffective)
        {
            return GetEffectiveEntities<ToDConsideration>(customerID, zoneID, whenEffective);
        }

        public List<Commission> GetCommissions(String customerID, int zoneID, DateTime whenEffective)
        {
            return GetEffectiveEntities<Commission>(customerID, zoneID, whenEffective);
        }

        public List<Tariff> GetTariffs(String customerID, int zoneID, DateTime whenEffective)
        {
            return GetEffectiveEntities<Tariff>(customerID, zoneID, whenEffective);
        }

        #endregion Cacheable Entities

        public void Dispose()
        {
            GC.Collect();
        }
    }
}
