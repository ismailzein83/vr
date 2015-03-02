using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TABS;
using System.Data;
using System.Data.SqlClient;
using TOne.Entities;
using TOne.Caching;
using TOne.CDR.Entities;

namespace TOne.Business
{
    public class ProtPricingGenerator : IDisposable
    {
        TOneCacheManager _cacheManager;

        public ProtPricingGenerator(TOneCacheManager cacheManager)
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

            if (TABS.CarrierAccount.All.TryGetValue(main.CustomerID, out customer)) return null;
            
            if (TABS.CarrierAccount.All.TryGetValue(main.SupplierID, out supplier)) return null;
            
            IList<Rate> rates;
            if (cost)
                rates = GetRates(CarrierAccount.SYSTEM.CarrierAccountID, zoneId, main.Attempt.AddMinutes(supplier.SupplierGMTTime));
            else
                rates = GetRates(main.CustomerID, zoneId, main.Attempt);

            // If a rate is defined for
            if (rates.Count > 0)
            {
                // Initialize Pricing
                pricing.BillingCDRMainID = main.ID;
                pricing.RateID = rates[0].ID;
                pricing.ZoneID = rates[0].Zone.ZoneID;
                pricing.RateValue = (double)rates[0].Value;
                pricing.Net = (double)(rates[0].Value * (main.DurationInSeconds / 60m));
                pricing.CurrencySymbol = rates[0].PriceList.Currency.Symbol;
                pricing.DurationInSeconds = main.DurationInSeconds;

                TABS.Rate rate = rates[0];
                TABS.Currency rateCurrency = rates[0].PriceList.Currency;

                // Usables...
                ToDConsideration tod = null;
                Tariff tariff = null;

                #region Get Usables

                // Effective and Active ToD for this Call?
                IList<ToDConsideration> tods = GetToDConsiderations(cost ? CarrierAccount.SYSTEM.CarrierAccountID : main.CustomerID, pricing.ZoneID, main.Attempt);

                // If ToD Considered, the rate applied should be changed
                foreach (ToDConsideration effective in tods) { if (effective.WasActive(main.Attempt)) { tod = effective; break; } }

                // Check for ToD first
                if (tod != null)
                {
                    //pricing.RateType =  tod.RateType;
                    pricing.RateValue = (double)tod.ActiveRateValue(rates[0]);
                    pricing.ToDConsiderationID = tod.ToDConsiderationID;
                }
                //else
                   //pricing.RateType = ToDRateType.Normal;

                var attemptDate = new DateTime(main.Attempt.Year, main.Attempt.Month, main.Attempt.Day);

                // Commissions or extra charges
                IList<Commission> commissionsAndExtraCharges = GetCommissions(cost ? CarrierAccount.SYSTEM.CarrierAccountID : main.CustomerID, pricing.ZoneID, main.Attempt);

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
                            pricing.ExtraChargeID = itemClone.CommissionID;

                            extraCharge = itemClone;
                        }

                        if (pricing.CommissionID <= 0)//(Removed from if Condition !item.IsExtraCharge && FOr bug 1958)
                        {
                            item.Amount = item.Amount != null ? item.Amount : (decimal)0.0;//in order if the commission value null apply the percentage and vise versa,also if the amount and perc found apply the two ammount then perc
                            //if (item.Amount != null && item.Amount != 0)
                            itemClone.Amount = (decimal?)TABS.Rate.GetRate(item.Amount.Value, cost ? supplier.CarrierProfile.Currency : customer.CarrierProfile.Currency, rateCurrency, attemptDate);
                            pricing.CommissionID = itemClone.CommissionID;

                            commission = itemClone;
                        }
                    }
                }
                // Tariff Considered?
                IList<Tariff> tariffs = GetTariffs(cost ? CarrierAccount.SYSTEM.CarrierAccountID : main.CustomerID, zoneId, main.Attempt);

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
                CalculateAmounts(pricing, main, customer, supplier, tarrif, tod, commission, rate, extraCharge);
            }
            // No suitable rate found for this Code / Supplier / Customer Combination
            else
            {
                pricing = null;
            }
            return pricing;
        }

        void CalculateAmounts(BillingCDRPricingBase pricing, BillingCDRMain main, CarrierAccount customer, CarrierAccount supplier, TABS.Tariff tarrif,
            TABS.ToDConsideration ToDConsideration, TABS.Commission commission, TABS.Rate rate, TABS.Commission ExtraCharge)
        {
            bool isCost = pricing is BillingCDRCost;

            pricing.Code = isCost ? main.SupplierCode : main.OurCode;


            double accountedDuration = (double)main.DurationInSeconds;
            pricing.Net = (pricing.RateValue * (double)main.DurationInSeconds) / 60;

            //recalc in case of ceiling
            if (!isCost)
            {
                if (customer.IsCustomerCeiling == "Y")
                {

                    accountedDuration = (double)Math.Ceiling(main.DurationInSeconds);
                    pricing.Net = pricing.RateValue * (double)accountedDuration / 60.0;
                }

            }
            else
                if (supplier.IsSupplierCeiling == "Y")
                {
                    accountedDuration = (double)Math.Ceiling(main.DurationInSeconds);
                    pricing.Net = pricing.RateValue * (double)accountedDuration / 60.0;
                }


            // Tariff?
            if (tarrif != null)
            {
                pricing.Net = 0;

                // Calculate the amount for the firt period
                if (tarrif.FirstPeriod > 0)
                {
                    pricing.FirstPeriod = tarrif.FirstPeriod;
                    double firstPeriodRate = (tarrif.FirstPeriodRate > 0) ? (double)tarrif.FirstPeriodRate : pricing.RateValue;
                    if (tarrif.RepeatFirstPeriod)
                    {
                        //pricing.RepeatFirstperiod = pricing.RepeatFirstperiod;
                        //accountedDuration = Math.Ceiling(accountedDuration / tarrif.FirstPeriod) * tarrif.FirstPeriod;
                        //pricing.Net = (accountedDuration * firstPeriodRate) / 60;
                    }
                    else
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
            //else
            //{
            //    // Net
            //    pricing.Net = (pricing.RateValue * (double)pricing.Billing_CDR_Main.DurationInSeconds) / 60;
            //}

            // Discount (from TOD)



            if (ToDConsideration != null)
                pricing.Discount = ((double)rate.Value - ToDConsideration.ActiveRateValue(rate)) * (double)accountedDuration / 60;
            else
                pricing.Discount = 0;



            // Commission
            if (commission != null)
                pricing.CommissionValue = (pricing.RateValue - commission.DeductedRateValue(isCost, pricing.RateValue)) * (double)(main.DurationInSeconds) / 60;
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
            //if (tarrif != null)
            pricing.DurationInSeconds = (decimal)accountedDuration;
        }

        #region Cacheable Entities

        protected static Zone _AnyZone;
        protected static Zone AnyZone
        {
            get
            {
                lock (typeof(ProtPricingGenerator))
                {
                    if (_AnyZone == null)
                    {
                        _AnyZone = new Zone();
                        _AnyZone.Name = "**** Any Zone ****";
                        _AnyZone.Supplier = null;
                    }
                }
                return _AnyZone;
            }
        }

        protected class DateSensitiveEntityCache<T> where T : TABS.Interfaces.IZoneSupplied
        {
            Dictionary<int, List<T>> _supplierEntities = new Dictionary<int, List<T>>();
            Dictionary<int, Dictionary<String, List<T>>> _ourEntities = new Dictionary<int, Dictionary<String, List<T>>>();

            public DateSensitiveEntityCache(DateTime pricingStart, bool IsRepricing)
            {
                Load( null, 0, pricingStart, IsRepricing);
            }

            public DateSensitiveEntityCache(string customerId, int zoneId, DateTime pricingStart, bool IsRepricing)
            {
                Load( customerId, zoneId, pricingStart, IsRepricing);
            }

            protected void Load(string customerId, int zoneId, DateTime pricingStart, bool IsRepricing)
            {

                IList<T> all = ProtPricingGeneratorEntites<T>.Load(customerId, zoneId, pricingStart, IsRepricing);
                foreach (T entity in all)
                {
                    if (entity.Supplier == null) continue;
                    TABS.CarrierAccount entityCustomer = entity.Customer;
                    TABS.CarrierAccount entitySupplier = entity.Supplier;
                    TABS.Zone entityZone = (entity.Zone == null) ? AnyZone : entity.Zone;

                    // Our Entities
                    if (entitySupplier.Equals(TABS.CarrierAccount.SYSTEM))
                    {
                        Dictionary<String, List<T>> zoneEntities = null;
                        if (!_ourEntities.TryGetValue(entityZone.ZoneID, out zoneEntities))
                        {
                            zoneEntities = new Dictionary<String, List<T>>();
                            _ourEntities[entityZone.ZoneID] = zoneEntities;
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
                        if (!_supplierEntities.TryGetValue(entityZone.ZoneID, out zoneEntities))
                        {
                            zoneEntities = new List<T>();
                            _supplierEntities[entityZone.ZoneID] = zoneEntities;
                        }
                        zoneEntities.Add(entity);
                    }
                }
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
                                if (TABS.Components.DateTimeEffectiveEntity.GetIsEffective(entity, whenEffective))
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
                            if (TABS.Components.DateTimeEffectiveEntity.GetIsEffective(entity, whenEffective))
                                effective.Add(entity);
                    }
                }
                return effective;
            }
        }

        protected DateTime _PricingStart = new DateTime(2000, 1, 1);

        public List<T> GetEffectiveEntities<T>(String customerID, int zoneID, DateTime whenEffective) where T : TABS.Interfaces.IZoneSupplied
        {
            return _cacheManager.GetOrCreateObject(String.Format("GetEffectiveEntities_{0}_{1}_{2}_{3:ddMMMyy}", typeof(T).Name, customerID, zoneID, whenEffective.Date),
                CacheObjectType.Pricing,
                () =>
                {
                    DateSensitiveEntityCache<T> rates = null;
                    rates = new DateSensitiveEntityCache<T>(customerID, zoneID, whenEffective, true);
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


        public ProtPricingGenerator(TOneCacheManager cacheManager, NHibernate.ISession session)
        {
            _cacheManager = cacheManager;
        }

        public bool FixParentCodeSale(Billing_CDR_Main main, Zone originalZone, Code code, ProtCodeMap codeMap)
        {
            // If main (without fixing) has a sale, no need to fix. Return true if code is not null
            if (main.Billing_CDR_Sale != null)
                return code != null;

            Code found = null;
            // First time, get current code and call to check for parent 
            if (code == null)
            {
                found = codeMap.Find(main.CDPN, TABS.CarrierAccount.SYSTEM, main.Attempt);
                if (found != null)
                {
                    originalZone = main.OurZone;
                    return FixParentCodeSale(main, originalZone, found, codeMap);
                }
                else
                    return false;
            }
            // Recursive call, checking for parent
            else
            {
                StringBuilder codeValue = new StringBuilder(code.Value);
                codeValue.Length--;

                // While not found a parent code for the current code group
                while (found == null && codeValue.Length > 0)
                {
                    found = codeMap.Find(codeValue.ToString(), TABS.CarrierAccount.SYSTEM, main.Attempt);
                    if (found != null)
                    {
                        var rates = GetRates(main.Customer.CarrierAccountID, found.Zone.ZoneID, main.Attempt);
                        if (rates.Count > 0)
                            break;
                        else
                            found = null;
                    }
                    codeValue.Length--;
                }

                // No parent code found, return false.
                if (found == null)
                {
                    main.OurZone = originalZone; return false;
                }
                // Found a parent Sale Code.
                else
                {
                    main.OurZone = found.Zone;
                    main.Billing_CDR_Sale = Get<Billing_CDR_Sale>(main);
                    return FixParentCodeSale(main, originalZone, found, codeMap);
                }
            }
        }

        public T Get<T>(Billing_CDR_Main main) where T : Billing_CDR_Pricing_Base, new()
        {
            T pricing = new T();

            bool cost = pricing is Billing_CDR_Cost;

            Zone zone = (cost) ? main.SupplierZone : main.OurZone;
            if (zone == null) return null;

            // Get the proper Rate 
            IList<Rate> rates = GetRates(cost ? CarrierAccount.SYSTEM.CarrierAccountID : main.Customer.CarrierAccountID, zone.ZoneID, main.Attempt);

            // If a rate is defined for
            if (rates.Count > 0)
            {
                // Initialize Pricing
                pricing.Billing_CDR_Main = main;
                pricing.Rate = rates[0];
                pricing.Zone = rates[0].Zone;
                pricing.RateValue = (double)rates[0].Value;
                pricing.Net = (double)(rates[0].Value * (main.DurationInSeconds / 60m));
                pricing.Currency = rates[0].PriceList.Currency;
                pricing.DurationInSeconds = main.DurationInSeconds;

                rates = null;
                // Usables...
                ToDConsideration tod = null;
                Tariff tariff = null;

                #region Get Usables

                // Effective and Active ToD for this Call?
                IList<ToDConsideration> tods = GetToDConsiderations(cost ? CarrierAccount.SYSTEM.CarrierAccountID : main.Customer.CarrierAccountID, pricing.Zone.ZoneID, main.Attempt);

                // If ToD Considered, the rate applied should be changed
                foreach (ToDConsideration effective in tods) { if (effective.WasActive(main.Attempt)) { tod = effective; break; } }

                // Check for ToD first
                if (tod != null)
                {
                    pricing.RateType = tod.RateType;
                    pricing.RateValue = (double)tod.ActiveRateValue(pricing.Rate);
                    pricing.ToDConsideration = tod;
                }
                else
                    pricing.RateType = ToDRateType.Normal;

                var attemptDate = new DateTime(pricing.Billing_CDR_Main.Attempt.Year, pricing.Billing_CDR_Main.Attempt.Month, pricing.Billing_CDR_Main.Attempt.Day);

                // Commissions or extra charges
                IList<Commission> commissionsAndExtraCharges = GetCommissions(cost ? CarrierAccount.SYSTEM.CarrierAccountID : main.Customer.CarrierAccountID, pricing.Zone.ZoneID, main.Attempt);

                foreach (Commission item in commissionsAndExtraCharges)
                {

                    var itemClone = (Commission)item.Clone();

                    var pricingValue = (float?)TABS.Rate.GetRate((decimal)pricing.RateValue, pricing.Currency, cost ? main.Supplier.CarrierProfile.Currency : main.Customer.CarrierProfile.Currency, attemptDate);

                    if ((!item.FromRate.HasValue || item.FromRate <= pricingValue) && (!item.ToRate.HasValue || item.ToRate >= pricingValue))
                    {
                        if (item.IsExtraCharge && pricing.ExtraCharge == null)
                        {
                            if (item.Amount != null && item.Amount != 0)
                                itemClone.Amount = (decimal?)TABS.Rate.GetRate(item.Amount.Value, cost ? main.Supplier.CarrierProfile.Currency : main.Customer.CarrierProfile.Currency, pricing.Currency, attemptDate);

                            pricing.ExtraCharge = itemClone;

                        }

                        if (!item.IsExtraCharge && pricing.Commission == null)
                        {
                            if (item.Amount != null && item.Amount != 0)
                                itemClone.Amount = (decimal?)TABS.Rate.GetRate(item.Amount.Value, cost ? main.Supplier.CarrierProfile.Currency : main.Customer.CarrierProfile.Currency, pricing.Currency, attemptDate);

                            pricing.Commission = itemClone;
                        }
                    }
                }

                // Tariff Considered?
                IList<Tariff> tariffs = GetTariffs(cost ? CarrierAccount.SYSTEM.CarrierAccountID : main.Customer.CarrierAccountID, zone.ZoneID, main.Attempt);
                if (tariffs.Count > 0)
                {
                    tariff = tariffs[0];

                    var tariffClone = (Tariff)(tariff.Clone());

                    if (tariff.CallFee > 0)
                        tariffClone.CallFee = TABS.Rate.GetRate(tariff.CallFee, cost ? main.Supplier.CarrierProfile.Currency : main.Customer.CarrierProfile.Currency, pricing.Currency, attemptDate);

                    if (tariff.FirstPeriodRate > 0)
                        tariffClone.FirstPeriodRate = TABS.Rate.GetRate(tariff.FirstPeriodRate, cost ? main.Supplier.CarrierProfile.Currency : main.Customer.CarrierProfile.Currency, pricing.Currency, attemptDate);

                    pricing.Tariff = tariffClone;
                }



                #endregion Get Usables

                // Calculate ...
                CalculateAmounts(pricing);
            }
            // No suitable rate found for this Code / Supplier / Customer Combination
            else
            {
                pricing = null;
            }
            return pricing;
        }

        public T GetRepricing<T>(Billing_CDR_Main main) where T : Billing_CDR_Pricing_Base, new()
        {
            T pricing = new T();

            //

            return pricing;
        }

        /// <summary>
        /// Calculate the Amounts (Net, Discount, Commission, Extra Charge) and so on.
        /// </summary>
        /// <param name="pricing"></param>
        void CalculateAmountsold(Billing_CDR_Pricing_Base pricing)
        {
            bool isCost = pricing is Billing_CDR_Cost;

            pricing.Code = isCost ? pricing.Billing_CDR_Main.SupplierCode : pricing.Billing_CDR_Main.OurCode;
            double accountedDuration = (double)pricing.Billing_CDR_Main.DurationInSeconds;

            // Tariff?
            if (pricing.Tariff != null)
            {
                pricing.Net = 0;

                // Calculate the amount for the firt period
                if (pricing.Tariff.FirstPeriod > 0)
                {
                    pricing.FirstPeriod = pricing.Tariff.FirstPeriod;
                    double firstPeriodRate = (pricing.Tariff.FirstPeriodRate > 0) ? (double)pricing.Tariff.FirstPeriodRate : pricing.RateValue;
                    if (pricing.Tariff.RepeatFirstPeriod)
                    {
                        //pricing.RepeatFirstperiod = pricing.RepeatFirstperiod;
                        //accountedDuration = Math.Ceiling(accountedDuration / pricing.Tariff.FirstPeriod) * pricing.Tariff.FirstPeriod;
                        //pricing.Net = (accountedDuration * firstPeriodRate) / 60;
                    }
                    else
                    {
                        // Calculate first period amount then continue normally
                        pricing.Net = (pricing.Tariff.FirstPeriod * firstPeriodRate) / 60;
                        accountedDuration -= (double)pricing.Tariff.FirstPeriod;
                        accountedDuration = Math.Max(0, accountedDuration);
                    }
                }

                // if there is a fraction unit
                if (pricing.Tariff.FractionUnit > 0)
                {
                    pricing.FractionUnit = (byte)pricing.Tariff.FractionUnit;
                    accountedDuration = Math.Ceiling(accountedDuration / pricing.Tariff.FractionUnit) * pricing.Tariff.FractionUnit;
                }

                // Calculate the net amount
                pricing.Net += (accountedDuration * pricing.RateValue) / 60;

                // Calculate the Net from the Tariff
                pricing.Net += (double)pricing.Tariff.CallFee;
            }
            else
            {
                // Net
                pricing.Net = (pricing.RateValue * (double)pricing.Billing_CDR_Main.DurationInSeconds) / 60;
            }

            // Discount (from TOD)
            if (pricing.ToDConsideration != null)
                pricing.Discount = ((double)pricing.Rate.Value - pricing.ToDConsideration.ActiveRateValue(pricing.Rate)) * (double)accountedDuration / 60;
            else
                pricing.Discount = 0;

            // Commission
            if (pricing.Commission != null)
                pricing.CommissionValue = (pricing.RateValue - pricing.Commission.DeductedRateValue(isCost, pricing.RateValue)) * (double)accountedDuration / 60;
            else
                pricing.CommissionValue = 0;

            // Extra Charge
            if (pricing.ExtraCharge != null)
                pricing.ExtraChargeValue = (pricing.RateValue - pricing.ExtraCharge.DeductedRateValue(isCost, pricing.RateValue)) * (double)accountedDuration / 60;
            else
                pricing.ExtraChargeValue = 0;

            if (pricing.Tariff != null && pricing.Tariff.FirstPeriod > 0 && !pricing.Tariff.RepeatFirstPeriod)
                accountedDuration += (double)pricing.Tariff.FirstPeriod;

            // updating the billing duration (if tarrif included)
            if (pricing.Tariff != null)
                pricing.DurationInSeconds = (decimal)accountedDuration;

            //recalc in case of ceiling
            if (!isCost)
            {
                if (pricing.Billing_CDR_Main.Customer.IsCustomerCeiling == "Y")
                {

                    pricing.DurationInSeconds = Math.Ceiling(pricing.DurationInSeconds);
                    pricing.Net = pricing.RateValue * (double)pricing.DurationInSeconds / 60.0;
                }
            }
            else
                if (pricing.Billing_CDR_Main.Supplier.IsSupplierCeiling == "Y")
                {
                    pricing.DurationInSeconds = Math.Ceiling(pricing.DurationInSeconds);
                    pricing.Net = pricing.RateValue * (double)pricing.DurationInSeconds / 60.0;
                }


        }

        void CalculateAmounts(Billing_CDR_Pricing_Base pricing)
        {
            bool isCost = pricing is Billing_CDR_Cost;

            pricing.Code = isCost ? pricing.Billing_CDR_Main.SupplierCode : pricing.Billing_CDR_Main.OurCode;


            double accountedDuration = (double)pricing.Billing_CDR_Main.DurationInSeconds;
            pricing.Net = (pricing.RateValue * (double)pricing.Billing_CDR_Main.DurationInSeconds) / 60;

            //recalc in case of ceiling
            if (!isCost)
            {
                if (pricing.Billing_CDR_Main.Customer.IsCustomerCeiling == "Y")
                {

                    accountedDuration = (double)Math.Ceiling(pricing.Billing_CDR_Main.DurationInSeconds);
                    pricing.Net = pricing.RateValue * (double)accountedDuration / 60.0;
                }

            }
            else
                if (pricing.Billing_CDR_Main.Supplier.IsSupplierCeiling == "Y")
                {
                    accountedDuration = (double)Math.Ceiling(pricing.Billing_CDR_Main.DurationInSeconds);
                    pricing.Net = pricing.RateValue * (double)accountedDuration / 60.0;
                }




            // Tariff?
            if (pricing.Tariff != null)
            {
                pricing.Net = 0;

                // Calculate the amount for the firt period
                if (pricing.Tariff.FirstPeriod > 0)
                {
                    pricing.FirstPeriod = pricing.Tariff.FirstPeriod;
                    double firstPeriodRate = (pricing.Tariff.FirstPeriodRate > 0) ? (double)pricing.Tariff.FirstPeriodRate : pricing.RateValue;
                    if (pricing.Tariff.RepeatFirstPeriod)
                    {
                        //pricing.RepeatFirstperiod = pricing.RepeatFirstperiod;
                        //accountedDuration = Math.Ceiling(accountedDuration / pricing.Tariff.FirstPeriod) * pricing.Tariff.FirstPeriod;
                        //pricing.Net = (accountedDuration * firstPeriodRate) / 60;
                    }
                    else
                    {
                        // Calculate first period amount then continue normally
                        pricing.Net = firstPeriodRate;//(pricing.Tariff.FirstPeriod * firstPeriodRate) / 60
                        accountedDuration -= (double)pricing.Tariff.FirstPeriod;
                        accountedDuration = Math.Max(0, accountedDuration);
                    }
                }

                // if there is a fraction unit
                if (pricing.Tariff.FractionUnit > 0)
                {
                    pricing.FractionUnit = (byte)pricing.Tariff.FractionUnit;
                    accountedDuration = Math.Ceiling(accountedDuration / pricing.Tariff.FractionUnit) * pricing.Tariff.FractionUnit;
                    pricing.Net += Math.Ceiling(accountedDuration / pricing.Tariff.FractionUnit) * pricing.RateValue;// 60
                }
                else// Calculate the net amount
                    pricing.Net += (accountedDuration * pricing.RateValue) / 60;



                // Calculate the Net from the Tariff
                pricing.Net += (double)pricing.Tariff.CallFee;
            }
            //else
            //{
            //    // Net
            //    pricing.Net = (pricing.RateValue * (double)pricing.Billing_CDR_Main.DurationInSeconds) / 60;
            //}

            // Discount (from TOD)
            if (pricing.ToDConsideration != null)
                pricing.Discount = ((double)pricing.Rate.Value - pricing.ToDConsideration.ActiveRateValue(pricing.Rate)) * (double)accountedDuration / 60;
            else
                pricing.Discount = 0;

            // Commission
            if (pricing.Commission != null)
                pricing.CommissionValue = (pricing.RateValue - pricing.Commission.DeductedRateValue(isCost, pricing.RateValue)) * (double)(pricing.Billing_CDR_Main.DurationInSeconds) / 60;
            else
                pricing.CommissionValue = 0;

            // Extra Charge
            if (pricing.ExtraCharge != null)
                pricing.ExtraChargeValue = (pricing.RateValue - pricing.ExtraCharge.DeductedRateValue(isCost, pricing.RateValue)) * (double)accountedDuration / 60;
            else
                pricing.ExtraChargeValue = 0;

            if (pricing.Tariff != null && pricing.Tariff.FirstPeriod > 0 && !pricing.Tariff.RepeatFirstPeriod)
                accountedDuration += (double)pricing.Tariff.FirstPeriod;

            // updating the billing duration (if tarrif included)

            //if we have Tarrif accountedDuration Calculated with Ceiling value but when no Tarrif accountedDuration should also be assigned
            //if (pricing.Tariff != null)
            pricing.DurationInSeconds = (decimal)accountedDuration;
        }

        public void Dispose()
        {
            GC.Collect();
        }
    }

    public class ProtPricingGeneratorEntites<T> : IDisposable
    {
        public static List<T> Load(string customerId, int zoneId, DateTime when, bool IsRepricing)
        {
            if (typeof(T) == typeof(TABS.Rate))
                return GetRates(customerId, zoneId, when, IsRepricing) as List<T>;
            if (typeof(T) == typeof(TABS.Tariff))
                return GetTarrif(customerId, zoneId, when) as List<T>;
            if (typeof(T) == typeof(TABS.ToDConsideration))
                return GetTOD(customerId, zoneId, when) as List<T>;
            if (typeof(T) == typeof(TABS.Commission))
                return GetCommission(customerId, zoneId, when) as List<T>;

            return new List<T>();
        }
        public static List<Commission> GetCommission(string customerId, int zoneId, DateTime when)
        {
            List<Commission> Commissions = new List<Commission>();
            System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection();
            connection = connection = (SqlConnection)TABS.DataHelper.GetOpenConnection();
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat(@"SELECT [CommissionID],c.[SupplierID],[CustomerID],c.[ZoneID]
                                ,z.name,z.supplierid,z.codegroup,z.BeginEffectiveDate,z.EndEffectiveDate ,[FromRate]
                                ,[ToRate] ,[Percentage] ,[Amount] ,c.[BeginEffectiveDate],c.[EndEffectiveDate] ,[IsExtraCharge]
                                ,c.[IsEffective],c.[UserID],c.[timestamp]
                                FROM [Commission] c with (nolock) inner join Zone z with (nolock) on c.ZoneID=z.ZoneID 
                                where c.EndEffectiveDate is null or c.EndEffectiveDate>'{0}'
                                 and (z.EndEffectiveDate is null or (z.EndEffectiveDate > '{0}' and z.BeginEffectiveDate<z.EndEffectiveDate))
                                    and (c.CustomerID = '{1}')
                                    and (z.ZoneID={2})
                                order by z.Name ", when.ToString("yyyy-MM-dd"), customerId, zoneId);


            using (connection)
            {
                System.Data.IDbCommand command = connection.CreateCommand();
                command.CommandText = sql.ToString();
                command.Connection = connection;
                command.CommandTimeout = 120;
                System.Data.IDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection);
                int index = 0;
                while (reader.Read())
                {

                    index = 0;
                    Commission C = new Commission();
                    C.Zone = new Zone();
                    C.CommissionID = int.Parse(reader[index].ToString()); index++;
                    if (!TABS.CarrierAccount.All.ContainsKey(reader[index].ToString())) continue;
                    C.Supplier = TABS.CarrierAccount.All[reader[index].ToString()]; index++;
                    if (!TABS.CarrierAccount.All.ContainsKey(reader[index].ToString())) continue;
                    C.Customer = TABS.CarrierAccount.All[reader[index].ToString()]; index++;
                    C.Zone.ZoneID = !reader.IsDBNull(index) ? int.Parse(reader[index].ToString()) : 0; index++;
                    C.Zone.Name = !reader.IsDBNull(index) ? reader.GetString(index) : ""; index++;
                    string supplierid = !reader.IsDBNull(index) ? reader.GetString(index) : ""; index++;
                    C.Zone.Supplier = TABS.CarrierAccount.All.ContainsKey(supplierid) ? TABS.CarrierAccount.All[supplierid] : null;
                    string codegroup = !reader.IsDBNull(index) ? reader.GetString(index) : ""; index++;
                    C.Zone.CodeGroup = new TABS.CodeGroup();
                    C.Zone.CodeGroup.Code = codegroup;
                    C.Zone.BeginEffectiveDate = !reader.IsDBNull(index) ? (DateTime?)reader.GetDateTime(index) : null; index++;
                    C.Zone.EndEffectiveDate = !reader.IsDBNull(index) ? (DateTime?)reader.GetDateTime(index) : null;
                    index++; C.FromRate = float.Parse(reader[index].ToString());
                    index++; C.ToRate = float.Parse(reader[index].ToString());
                    index++; C.Percentage = (!reader.IsDBNull(index)) ? float.Parse(reader[index].ToString()) : 0;
                    index++; C.Amount = (!reader.IsDBNull(index)) ? (decimal?)decimal.Parse(reader[index].ToString()) : 0;
                    index++; C.BeginEffectiveDate = reader.GetDateTime(index);
                    index++;
                    if (!reader.IsDBNull(index))
                        C.EndEffectiveDate = reader.GetDateTime(index);
                    else
                        C.EndEffectiveDate = null;
                    index++;
                    C.IsExtraCharge = (reader[index].ToString() == "Y") ? true : false;
                    Commissions.Add(C);
                    index = -1;

                }
                return Commissions;
            }

        }
        public static List<TABS.ToDConsideration> GetTOD(string customerId, int zoneId, DateTime when)
        {
            List<TABS.ToDConsideration> Tarrif = new List<TABS.ToDConsideration>();
            System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection();
            connection = connection = (SqlConnection)TABS.DataHelper.GetOpenConnection();
            using (connection)
            {


                string sql = string.Format(@"SELECT [ToDConsiderationID],tod.[ZoneID] ,z.name,z.supplierid,z.codegroup,z.BeginEffectiveDate,z.EndEffectiveDate
                                          ,tod.[SupplierID],tod.[CustomerID]
                                          ,[BeginTime],[EndTime],[WeekDay],[HolidayDate],[HolidayName]
                                          ,[RateType],tod.[BeginEffectiveDate],tod.[EndEffectiveDate]
                                          ,tod.[IsEffective],[IsActive],tod.[UserID],tod.[timestamp]
                                           FROM [ToDConsideration] tod with (nolock) inner join Zone z  with (nolock) on tod.ZoneID=z.ZoneID
                                           Where  (tod.EndEffectiveDate is null or (tod.EndEffectiveDate > '{0}' and tod.BeginEffectiveDate<tod.EndEffectiveDate))
                                           and (z.EndEffectiveDate is null or (z.EndEffectiveDate > '{0}' and z.BeginEffectiveDate<z.EndEffectiveDate))
                                            and (tod.CustomerID = '{1}')
                                            and (z.ZoneID={2})
                                          order by z.Name", when.ToString("yyyy-MM-dd"), customerId, zoneId);

                System.Data.IDbCommand command = connection.CreateCommand();
                command.CommandText = sql;
                command.CommandTimeout = 120;
                command.Connection = connection;
                System.Data.IDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection);
                int index = 0;
                DateTime? zoneBED = null;
                while (reader.Read())
                {
                    index = 0;
                    TABS.ToDConsideration r = new ToDConsideration();
                    r.Zone = new Zone();
                    r.ToDConsiderationID = !reader.IsDBNull(index) ? int.Parse(reader[index].ToString()) : 0; index++;
                    int ZoneID = !reader.IsDBNull(index) ? int.Parse(reader[index].ToString()) : 0; index++;
                    string DBZoneName = !reader.IsDBNull(index) ? reader.GetString(index) : ""; index++;
                    string supplierid = !reader.IsDBNull(index) ? reader.GetString(index) : ""; index++;
                    string codegroup = !reader.IsDBNull(index) ? reader.GetString(index) : ""; index++;
                    DateTime? ZoneBED = !reader.IsDBNull(index) ? (DateTime?)reader.GetDateTime(index) : null; index++;
                    DateTime? ZoneEED = !reader.IsDBNull(index) ? (DateTime?)reader.GetDateTime(index) : null; index++;
                    r.Zone.Name = DBZoneName;
                    r.Zone.CodeGroup = new TABS.CodeGroup();
                    r.Zone.CodeGroup.Code = codegroup;
                    r.Zone.EffectiveCodes = new List<TABS.Code>();
                    r.Zone.Supplier = TABS.CarrierAccount.All.ContainsKey(supplierid) ? TABS.CarrierAccount.All[supplierid] : null;
                    r.Zone.BeginEffectiveDate = zoneBED;
                    r.Zone.EndEffectiveDate = ZoneEED;
                    r.Zone.ZoneID = ZoneID;

                    r.Supplier = (!reader.IsDBNull(index) && TABS.CarrierAccount.All.ContainsKey(reader[index].ToString())) ? TABS.CarrierAccount.All[reader[index].ToString()] : null; index++;
                    r.Customer = (!reader.IsDBNull(index) && TABS.CarrierAccount.All.ContainsKey(reader[index].ToString())) ? TABS.CarrierAccount.All[reader[index].ToString()] : null; index++;

                    r.BeginTime = (!reader.IsDBNull(index)) ? TimeSpan.Parse(reader[index].ToString()) : TimeSpan.Zero; index++;
                    r.EndTime = (!reader.IsDBNull(index)) ? TimeSpan.Parse(reader[index].ToString()) : TimeSpan.Zero; index++;
                    r.WeekDay = (!reader.IsDBNull(index)) ? (DayOfWeek)Enum.Parse(typeof(DayOfWeek), reader[index].ToString()) : 0; index++;
                    r.HolidayDate = !reader.IsDBNull(index) ? (DateTime?)reader.GetDateTime(index) : null; index++;
                    r.HolidayName = (!reader.IsDBNull(index)) ? reader[index].ToString() : ""; index++;
                    r.RateType = (!reader.IsDBNull(index)) ? (ToDRateType)Enum.Parse(typeof(ToDRateType), reader[index].ToString()) : ToDRateType.Normal; index++;//default normat ratetype to be checked
                    r.BeginEffectiveDate = !reader.IsDBNull(index) ? (DateTime?)reader.GetDateTime(index) : null; index++;
                    r.EndEffectiveDate = !reader.IsDBNull(index) ? (DateTime?)reader.GetDateTime(index) : null; index++;
                    if (r.Customer != null && r.Supplier != null)
                        Tarrif.Add(r);
                }
            }


            return Tarrif;
        }
        public static List<TABS.Tariff> GetTarrif(string customerId, int zoneId, DateTime when)
        {
            List<TABS.Tariff> Tarrif = new List<TABS.Tariff>();
            System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection();
            connection = connection = (SqlConnection)TABS.DataHelper.GetOpenConnection();
            using (connection)
            {


                string sql = string.Format(@"SELECT [TariffID],t.[ZoneID],z.name,z.supplierid,z.codegroup,z.BeginEffectiveDate,z.EndEffectiveDate
                                            ,t.[CustomerID],t.[SupplierID] ,t.[CallFee],t.[FirstPeriodRate],t.[FirstPeriod],t.[RepeatFirstPeriod],t.[FractionUnit]
                                            ,t.[BeginEffectiveDate],t.[EndEffectiveDate],t.[IsEffective],t.[UserID]
                                             FROM  [Tariff] t  with (nolock) inner join Zone z  with (nolock) on t.ZoneID=z.ZoneID 
                                            Where  (t.EndEffectiveDate is null or (t.EndEffectiveDate > '{0}' and t.BeginEffectiveDate<t.EndEffectiveDate))
                                              and (z.EndEffectiveDate is null or (z.EndEffectiveDate > '{0}' and z.BeginEffectiveDate<z.EndEffectiveDate))
                                            and (t.CustomerID = '{1}')
                                            and (z.ZoneID={2})
                                            order by z.Name", when.ToString("yyyy-MM-dd"), customerId, zoneId);

                System.Data.IDbCommand command = connection.CreateCommand();
                command.CommandTimeout = 120;
                command.CommandText = sql;
                command.Connection = connection;
                System.Data.IDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection);
                int index = 0;
                DateTime? zoneBED = null;
                while (reader.Read())
                {
                    index = 0;
                    TABS.Tariff r = new Tariff();
                    r.Zone = new Zone();
                    r.TariffID = !reader.IsDBNull(index) ? int.Parse(reader[index].ToString()) : 0; index++;
                    int ZoneID = !reader.IsDBNull(index) ? int.Parse(reader[index].ToString()) : 0; index++;
                    string DBZoneName = !reader.IsDBNull(index) ? reader.GetString(index) : ""; index++;
                    string supplierid = !reader.IsDBNull(index) ? reader.GetString(index) : ""; index++;
                    string codegroup = !reader.IsDBNull(index) ? reader.GetString(index) : ""; index++;
                    DateTime? ZoneBED = !reader.IsDBNull(index) ? (DateTime?)reader.GetDateTime(index) : null; index++;
                    DateTime? ZoneEED = !reader.IsDBNull(index) ? (DateTime?)reader.GetDateTime(index) : null; index++;

                    r.Zone.Name = DBZoneName;
                    r.Zone.CodeGroup = new TABS.CodeGroup();
                    r.Zone.CodeGroup.Code = codegroup;
                    r.Zone.EffectiveCodes = new List<TABS.Code>();
                    r.Zone.Supplier = TABS.CarrierAccount.All.ContainsKey(supplierid) ? TABS.CarrierAccount.All[supplierid] : null;
                    r.Zone.BeginEffectiveDate = zoneBED;
                    r.Zone.EndEffectiveDate = ZoneEED;
                    r.Zone.ZoneID = ZoneID;
                    r.Customer = (!reader.IsDBNull(index) && TABS.CarrierAccount.All.ContainsKey(reader[index].ToString())) ? TABS.CarrierAccount.All[reader[index].ToString()] : null; index++;
                    r.Supplier = (!reader.IsDBNull(index) && TABS.CarrierAccount.All.ContainsKey(reader[index].ToString())) ? TABS.CarrierAccount.All[reader[index].ToString()] : null; index++;
                    r.CallFee = (!reader.IsDBNull(index)) ? decimal.Parse(reader[index].ToString()) : 0; index++;
                    r.FirstPeriodRate = (!reader.IsDBNull(index)) ? decimal.Parse(reader[index].ToString()) : 0; index++;
                    r.FirstPeriod = (!reader.IsDBNull(index)) ? int.Parse(reader[index].ToString()) : 0; index++;
                    r.RepeatFirstPeriod = (!reader.IsDBNull(index) && reader[index].ToString() == "Y") ? true : false; index++;
                    r.FractionUnit = (!reader.IsDBNull(index)) ? int.Parse(reader[index].ToString()) : 0; index++;
                    r.BeginEffectiveDate = !reader.IsDBNull(index) ? (DateTime?)reader.GetDateTime(index) : null; index++;
                    r.EndEffectiveDate = !reader.IsDBNull(index) ? (DateTime?)reader.GetDateTime(index) : null; index++;
                    if (r.Customer != null && r.Supplier != null)
                        Tarrif.Add(r);
                }
            }

            //List<Tariff> tarrifes = Tarrif.Where(r => r.Customer.CarrierAccountID.ToLower() == "c651" && r.Supplier.CarrierAccountID.ToLower() == "d071" && r.Zone.ZoneID == 12905).ToList();
            return Tarrif;
        }
        public static List<TABS.Rate> GetRates(string customerId, int zoneId, DateTime when, bool IsRepricing)
        {
            List<TABS.Rate> Rates = new List<TABS.Rate>();
            System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection();
            connection = (SqlConnection)TABS.DataHelper.GetOpenConnection();
            using (connection)
            {



                string sql = string.Format(@"Select z.name,z.supplierid,z.codegroup,z.BeginEffectiveDate,z.EndEffectiveDate,r.BeginEffectiveDate,r.EndEffectiveDate,
                                    r.Zoneid,r.rate,r.OffPeakRate,r.WeekendRate,r.pricelistid,r.ServicesFlag,p.CurrencyID,C.lastrate,r.RateID,p.CustomerID,p.SupplierID
                                    from Rate r   With(nolock) inner join zone z With(nolock)  on r.zoneid=z.zoneid
                                    inner join pricelist P With(nolock) on P.PriceListId=r.PriceListId
                                    inner join Currency C With(nolock) on P.Currencyid=C.Currencyid
                                    Where P.PriceListId=r.PriceListId  and (r.EndEffectiveDate is null or (r.EndEffectiveDate > '{0}' and r.BeginEffectiveDate<r.EndEffectiveDate))
                                    and (z.EndEffectiveDate is null or (z.EndEffectiveDate > '{0}' and z.BeginEffectiveDate<z.EndEffectiveDate))
                                    and (P.CustomerID = '{1}')
                                    and (z.ZoneID={2})
                                    order by z.Name", when.ToString("yyyy-MM-dd"), customerId, zoneId);

                System.Data.IDbCommand command = connection.CreateCommand();
                command.CommandTimeout = 360;
                command.CommandText = sql;
                command.Connection = connection;
                System.Data.IDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection);
                int index = 0;
                DateTime? zoneBED = null;
                while (reader.Read())
                {
                    index = 0;
                    TABS.Rate r = new TABS.Rate();
                    TABS.PriceList pricelist = new TABS.PriceList();
                    TABS.Currency currency = new TABS.Currency();
                    r.PriceList = pricelist;
                    r.PriceList.Currency = currency;
                    string DBZoneName = !reader.IsDBNull(index) ? reader.GetString(index) : ""; index++;
                    string supplierid = !reader.IsDBNull(index) ? reader.GetString(index) : ""; index++;
                    string codegroup = !reader.IsDBNull(index) ? reader.GetString(index) : ""; index++;
                    DateTime? ZoneBED = !reader.IsDBNull(index) ? (DateTime?)reader.GetDateTime(index) : null; index++;
                    DateTime? ZoneEED = !reader.IsDBNull(index) ? (DateTime?)reader.GetDateTime(index) : null; index++;
                    if (!reader.IsDBNull(index))
                        r.BeginEffectiveDate = reader.GetDateTime(index);
                    else
                        r.BeginEffectiveDate = null;

                    index++;
                    if (!reader.IsDBNull(index))
                        r.EndEffectiveDate = reader.GetDateTime(index);
                    else
                        r.EndEffectiveDate = null;
                    index++;

                    r.Zone = new TABS.Zone();
                    r.Zone.Name = DBZoneName;
                    r.Zone.CodeGroup = new TABS.CodeGroup();
                    r.Zone.CodeGroup.Code = codegroup;
                    r.Zone.EffectiveCodes = new List<TABS.Code>();
                    r.Zone.Supplier = TABS.CarrierAccount.All.ContainsKey(supplierid) ? TABS.CarrierAccount.All[supplierid] : null;
                    r.Zone.BeginEffectiveDate = zoneBED;
                    r.Zone.EndEffectiveDate = ZoneEED;
                    r.Zone.ZoneID = !reader.IsDBNull(index) ? reader.GetInt32(index) : -1; index++;


                    r.Value = !reader.IsDBNull(index) ? reader.GetDecimal(index) : 0; index++;
                    r.OffPeakRate = !reader.IsDBNull(index) ? reader.GetDecimal(index) : 0; index++;
                    r.WeekendRate = !reader.IsDBNull(index) ? reader.GetDecimal(index) : 0; index++;

                    r.Change = TABS.Change.None;// reader.GetInt16(index); 
                    r.PriceList.ID = !reader.IsDBNull(index) ? reader.GetInt32(index) : -1; index++;
                    r.ServicesFlag = reader.GetInt16(index); index++;
                    string CurrencyID = !reader.IsDBNull(index) ? reader.GetString(index) : ""; index++;
                    r.PriceList.Currency = TABS.Currency.All.Keys.Contains(CurrencyID) ? TABS.Currency.All[CurrencyID] : r.PriceList.Currency; index++;
                    //r.PriceList.Currency.LastRate = !reader.IsDBNull(index) ? float.Parse(reader[index].ToString()) : -1; index++;
                    r.ID = reader.GetInt64(index); index++;

                    r.PriceList.Customer = (TABS.CarrierAccount.All.ContainsKey(reader[index].ToString())) ? TABS.CarrierAccount.All[reader[index].ToString()] : null; index++;
                    r.PriceList.Supplier = (TABS.CarrierAccount.All.ContainsKey(reader[index].ToString())) ? TABS.CarrierAccount.All[reader[index].ToString()] : null;
                    //if (IsRepricing == false)
                    //{

                    if (r.PriceList.Supplier != null && r.PriceList.Customer != null)
                        Rates.Add(r);
                    //}
                    //else
                    //    if (r.PriceList.Supplier != null)
                    //        Rates.Add(r);
                }
            }
            return Rates;

        }
        public static List<TABS.Billing_CDR_Main> Get_Cdr_Main(long lastPricedCdrID, int BatchSize)
        {

            List<TABS.Billing_CDR_Main> cdres = new List<Billing_CDR_Main>();
            string QueryString = string.Format(@"SELECT  top {0} [ID],[Attempt]
                                                  ,[Alert],[Connect],[Disconnect],[DurationInSeconds],[CustomerID],[OurZoneID],[OriginatingZoneID]
                                                   --,z.name,z.supplierid,z.codegroup,z.BeginEffectiveDate,z.EndEffectiveDate ,
                                                   ,[CDPN]
                                                  ,cdrm.[SupplierID],[SupplierZoneID] ,sz.name,sz.supplierid,sz.codegroup,sz.BeginEffectiveDate,sz.EndEffectiveDate
                                                  ,[CGPN],[ReleaseCode],[ReleaseSource],[SwitchID],[SwitchCdrID]
                                                  ,[Tag] ,[Extra_Fields],[Port_IN] ,[Port_OUT] ,[OurCode] 
                                                  ,[SupplierCode] ,[CDPNOut] ,[SubscriberID],[SIP]
                                                  FROM  [Billing_CDR_Main] cdrm  WITH (NOLOCK,index=PK_Billing_CDR_Main) 
                                                  --left join Zone z with(nolock) on cdrm.OriginatingZoneID=z.ZoneID
                                                  inner join Zone sz with(nolock) on cdrm.SupplierZoneID=sz.ZoneID
                                                   WHERE  (sz.EndEffectiveDate is null or (sz.EndEffectiveDate > cdrm.Attempt and sz.BeginEffectiveDate<sz.EndEffectiveDate))
                                                      and  ID > {1}  and ID <=  {1} + {0} ORDER BY ID,Attempt ", BatchSize, lastPricedCdrID);
            System.Data.SqlClient.SqlConnection conn = (SqlConnection)TABS.DataHelper.GetOpenConnection();
            SqlCommand comm = new SqlCommand(QueryString, conn);
            comm.Connection = conn;
            System.Data.IDataReader reader = comm.ExecuteReader(CommandBehavior.CloseConnection);
            int index = 0;
            while (reader.Read())
            {
                index = 0;
                TABS.Billing_CDR_Main cdr = new Billing_CDR_Main();
                cdr.OriginatingZone = new Zone();
                cdr.OriginatingZone.CodeGroup = new CodeGroup();
                cdr.SupplierZone = new Zone();
                cdr.SupplierZone.CodeGroup = new CodeGroup();
                cdr.ID = !reader.IsDBNull(index) ? reader.GetInt64(index) : -1; index++;
                if (!reader.IsDBNull(index)) cdr.Attempt = reader.GetDateTime(index); index++;//else cdr.Attempt = null;
                cdr.Alert = (!reader.IsDBNull(index)) ? (DateTime?)reader.GetDateTime(index) : null; index++;
                cdr.Connect = !reader.IsDBNull(index) ? (DateTime?)reader.GetDateTime(index) : null; index++;
                cdr.Disconnect = !reader.IsDBNull(index) ? (DateTime?)reader.GetDateTime(index) : null; index++;
                cdr.DurationInSeconds = !reader.IsDBNull(index) ? decimal.Parse(reader[index].ToString()) : 0; index++;
                cdr.Customer = (!reader.IsDBNull(index) && TABS.CarrierAccount.All.ContainsKey(reader[index].ToString())) ? TABS.CarrierAccount.All[reader.GetString(index)] : null; index++;
                cdr.OurZone = (!reader.IsDBNull(index) && TABS.Zone.OwnZones.ContainsKey(reader.GetInt32(index))) ? TABS.Zone.OwnZones[reader.GetInt32(index)] : null; index++;
                cdr.OriginatingZone.ZoneID = !reader.IsDBNull(index) ? int.Parse(reader[index].ToString()) : -1; index++;
                cdr.OriginatingZone = (cdr.OriginatingZone.ZoneID > 0 && TABS.Zone.OwnZones.ContainsKey(cdr.OriginatingZone.ZoneID)) ? TABS.Zone.OwnZones[cdr.OriginatingZone.ZoneID] : null;

                //cdr.OriginatingZone.Name = !reader.IsDBNull(index) ? reader.GetString(index) : ""; index++;
                //cdr.OriginatingZone.Supplier = (!reader.IsDBNull(index) && TABS.CarrierAccount.All.ContainsKey(reader[index].ToString())) ? TABS.CarrierAccount.All[reader[index].ToString()] : null; index++;
                //cdr.OriginatingZone.CodeGroup.Code = !reader.IsDBNull(index) ? reader.GetString(index) : ""; index++;
                //cdr.OriginatingZone.BeginEffectiveDate = !reader.IsDBNull(index) ? (DateTime?)reader.GetDateTime(index) : null; index++;
                //cdr.OriginatingZone.EndEffectiveDate = !reader.IsDBNull(index) ? (DateTime?)reader.GetDateTime(index) : null; index++;
                cdr.CDPN = (!reader.IsDBNull(index)) ? reader[index].ToString() : ""; index++;
                cdr.SupplierID = (!reader.IsDBNull(index)) ? reader.GetString(index) : "";
                cdr.Supplier = (!reader.IsDBNull(index) && !string.IsNullOrEmpty(cdr.SupplierID) && TABS.CarrierAccount.All.ContainsKey(cdr.SupplierID)) ? TABS.CarrierAccount.All[cdr.SupplierID] : null; index++;
                cdr.SupplierZone.ZoneID = !reader.IsDBNull(index) ? int.Parse(reader[index].ToString()) : 0; index++;
                cdr.SupplierZone.Name = !reader.IsDBNull(index) ? reader.GetString(index) : ""; index++;
                cdr.SupplierZone.Supplier = (!reader.IsDBNull(index) && TABS.CarrierAccount.All.ContainsKey(reader[index].ToString())) ? TABS.CarrierAccount.All[reader[index].ToString()] : null; index++;
                cdr.SupplierZone.CodeGroup.Code = !reader.IsDBNull(index) ? reader.GetString(index) : ""; index++;
                cdr.SupplierZone.BeginEffectiveDate = !reader.IsDBNull(index) ? (DateTime?)reader.GetDateTime(index) : null; index++;
                cdr.SupplierZone.EndEffectiveDate = !reader.IsDBNull(index) ? (DateTime?)reader.GetDateTime(index) : null; index++;
                cdr.CGPN = !reader.IsDBNull(index) ? reader[index].ToString() : ""; index++;
                cdr.ReleaseCode = !reader.IsDBNull(index) ? reader[index].ToString() : ""; index++;
                cdr.ReleaseSource = !reader.IsDBNull(index) ? reader[index].ToString() : ""; index++;
                cdr.Switch = (!reader.IsDBNull(index) && TABS.Switch.All.ContainsKey(int.Parse(reader[index].ToString()))) ? TABS.Switch.All[int.Parse(reader[index].ToString())] : null; index++;
                cdr.SwitchCdrID = !reader.IsDBNull(index) ? long.Parse(reader[index].ToString()) : 0; index++;
                cdr.Tag = !reader.IsDBNull(index) ? reader[index].ToString() : ""; index++;
                cdr.Extra_Fields = !reader.IsDBNull(index) ? reader[index].ToString() : ""; index++;
                cdr.Port_IN = !reader.IsDBNull(index) ? reader[index].ToString() : ""; index++;
                cdr.Port_OUT = !reader.IsDBNull(index) ? reader[index].ToString() : ""; index++;
                cdr.OurCode = !reader.IsDBNull(index) ? reader[index].ToString() : ""; index++;
                cdr.SupplierCode = !reader.IsDBNull(index) ? reader[index].ToString() : ""; index++;
                cdr.CDPNOut = !reader.IsDBNull(index) ? reader[index].ToString() : ""; index++;
                cdr.SubscriberID = !reader.IsDBNull(index) ? int.Parse(reader[index].ToString()) : 0; index++;
                cdr.SIP = !reader.IsDBNull(index) ? reader[index].ToString() : ""; index++;
                if (cdr.SupplierZone != null)
                    cdres.Add(cdr);
            }

            conn.Close();
            conn.Dispose();
            return cdres;

        }
        public void Dispose()
        {
        }
    }
}
