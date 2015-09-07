using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Business;
using TOne.BusinessEntity.Entities;
using TOne.Caching;
using TOne.CDR.Entities;

namespace TOne.CDR.Business
{
    public class CostPricing : BasePricing
    {
        public CostPricing(TOneCacheManager cacheManager)
            : base(cacheManager)
        {

        }

        public BillingCDRCost GetRepricing(BillingCDRMain main)
        { 
            BillingCDRCost billingCDRCost = new BillingCDRCost();
            int zoneId =  main.SupplierZoneID;
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            CarrierProfileManager carrierProfileManager = new CarrierProfileManager();
            CarrierAccount supplierAccount = carrierAccountManager.GetCarrierAccount(main.SupplierID);
            CarrierProfile supplierProfile = carrierProfileManager.GetCarrierProfile(supplierAccount.ProfileId);
            if (supplierAccount == null) return null;
            IList<Rate> rates = GetRates("SYS", zoneId, main.Attempt.AddMinutes(supplierAccount.SupplierGMTTime));


            if (rates != null && rates.Count > 0)
            {
                // Initialize Pricing
                billingCDRCost.BillingCDRMainID = main.ID;
                billingCDRCost.RateID = rates[0].RateId;
                billingCDRCost.ZoneID = rates[0].ZoneId;
                billingCDRCost.RateValue = (double)rates[0].NormalRate;
                billingCDRCost.Net = (double)(rates[0].NormalRate * (main.DurationInSeconds / 60m));
                billingCDRCost.CurrencySymbol = rates[0].CurrencyID;
                billingCDRCost.DurationInSeconds = main.DurationInSeconds;

                Rate rate = rates[0];
               
                Currency rateCurrency = Currencies.Find(c => c.CurrencyID == billingCDRCost.CurrencySymbol);

                // Usables...
                ToDConsideration tod = null;
                Tariff tariff = null;

                #region Get Usables

                // Effective and Active ToD for this Call?
                IList<ToDConsideration> tods = GetToDConsiderations("SYS", billingCDRCost.ZoneID, main.Attempt);

                // If ToD Considered, the rate applied should be changed
                foreach (ToDConsideration effective in tods) { if (effective.WasActive(main.Attempt)) { tod = effective; break; } }

                // Check for ToD first
                if (tod != null)
                {
                    billingCDRCost.RateType = tod.RateType;
                    billingCDRCost.RateValue = (double)tod.ActiveRateValue(rates[0]);
                    billingCDRCost.ToDConsiderationID = tod.ToDConsiderationID;
                }
                else
                    billingCDRCost.RateType = ToDRateType.Normal;

                var attemptDate = new DateTime(main.Attempt.Year, main.Attempt.Month, main.Attempt.Day);
                billingCDRCost.Attempt = attemptDate;

                // Commissions or extra charges
                List<Commission> commissionsAndExtraCharges = GetCommissions("SYS", billingCDRCost.ZoneID, main.Attempt);

                Commission commission = null;
                Commission extraCharge = null;
                Currency supplierCurrency = Currencies.Find(c => c.CurrencyID == supplierProfile.CurrencyID);
                foreach (Commission item in commissionsAndExtraCharges)
                {
                    var itemClone = (Commission)item.Clone();

                    var pricingValue = (float?)GetRate((decimal)billingCDRCost.RateValue, rateCurrency, supplierCurrency, attemptDate);

                    if ((!item.FromRate.HasValue || item.FromRate <= pricingValue) && (!item.ToRate.HasValue || item.ToRate >= pricingValue))
                    {
                        if (item.IsExtraCharge && billingCDRCost.ExtraChargeID <= 0)
                        {
                            item.Amount = item.Amount != null ? item.Amount : (decimal)0.0; //in order if the commission value null apply the percentage and vise versa,also if the amount and perc found apply the two ammount then perc
                            //if (item.Amount != null && item.Amount != 0)
                            itemClone.Amount = (decimal?)GetRate(item.Amount.Value, supplierCurrency, rateCurrency, attemptDate);
                            billingCDRCost.ExtraChargeID = itemClone.ID;

                            extraCharge = itemClone;
                        }

                        if (billingCDRCost.CommissionID <= 0)//(Removed from if Condition !item.IsExtraCharge && FOr bug 1958)
                        {
                            item.Amount = item.Amount != null ? item.Amount : (decimal)0.0;//in order if the commission value null apply the percentage and vise versa,also if the amount and perc found apply the two ammount then perc
                            //if (item.Amount != null && item.Amount != 0)
                            itemClone.Amount = (decimal?)GetRate(item.Amount.Value, supplierCurrency, rateCurrency, attemptDate);
                            billingCDRCost.CommissionID = itemClone.ID;

                            commission = itemClone;
                        }
                    }
                }
                // Tariff Considered?
                IList<Tariff> tariffs = GetTariffs("SYS", zoneId, main.Attempt);

                Tariff tarrif = null;

                if (tariffs.Count > 0)
                {
                    tariff = tariffs[0];

                    var tariffClone = (Tariff)(tariff.Clone());

                    if (tariff.CallFee > 0)
                        tariffClone.CallFee = GetRate(tariff.CallFee, supplierCurrency, rateCurrency, attemptDate);

                    if (tariff.FirstPeriodRate > 0)
                        tariffClone.FirstPeriodRate = GetRate(tariff.FirstPeriodRate, supplierCurrency, rateCurrency, attemptDate);

                    billingCDRCost.TariffID = tariffClone.TariffID;
                    tarrif = tariffClone;
                }

                #endregion Get Usables

                billingCDRCost.Code = main.SupplierCode;
                CalculateAmounts(billingCDRCost, main.DurationInSeconds, supplierAccount.IsSupplierCeiling == IsCeiling.Y, tarrif, tod, commission, rate, extraCharge);
            }
            else
            {
                // No suitable rate found for this Code / Supplier / Customer Combination
                billingCDRCost = null;
            }
            return billingCDRCost;
        }

        private double GetPricingNet( double rateValue, decimal durationInSeconds, bool isSupplierCeiling, out double accountedDuration)
        {
            accountedDuration = (double)durationInSeconds;
            double net = (rateValue * (double)durationInSeconds) / 60;
                if (isSupplierCeiling)
                {
                    accountedDuration = (double)Math.Ceiling(durationInSeconds);
                    net = rateValue * (double)accountedDuration / 60.0;
                }
            return net;
        }

        void CalculateAmounts(BillingCDRPricingBase billingCDRCost, decimal mainDurationInSeconds, bool isSupplierCeiling, Tariff tarrif,
            ToDConsideration ToDConsideration, Commission commission, Rate rate, Commission ExtraCharge)
        {
            double accountedDuration = (double)mainDurationInSeconds;
            billingCDRCost.Net = GetPricingNet(billingCDRCost.RateValue, mainDurationInSeconds, isSupplierCeiling, out accountedDuration);

            // Tariff?
            if (tarrif != null)
            {
                billingCDRCost.Net = 0;

                // Calculate the amount for the firt period
                if (tarrif.FirstPeriod > 0)
                {
                    billingCDRCost.FirstPeriod = tarrif.FirstPeriod;
                    double firstPeriodRate = (tarrif.FirstPeriodRate > 0) ? (double)tarrif.FirstPeriodRate : billingCDRCost.RateValue;
                    if (!tarrif.RepeatFirstPeriod)
                    {
                        // Calculate first period amount then continue normally
                        billingCDRCost.Net = firstPeriodRate;//(tarrif.FirstPeriod * firstPeriodRate) / 60
                        accountedDuration -= (double)tarrif.FirstPeriod;
                        accountedDuration = Math.Max(0, accountedDuration);
                    }
                }

                // if there is a fraction unit
                if (tarrif.FractionUnit > 0)
                {
                    billingCDRCost.FractionUnit = (byte)tarrif.FractionUnit;
                    accountedDuration = Math.Ceiling(accountedDuration / tarrif.FractionUnit) * tarrif.FractionUnit;
                    billingCDRCost.Net += Math.Ceiling(accountedDuration / tarrif.FractionUnit) * billingCDRCost.RateValue;// 60
                }
                else// Calculate the net amount
                    billingCDRCost.Net += (accountedDuration * billingCDRCost.RateValue) / 60;

                // Calculate the Net from the Tariff
                billingCDRCost.Net += (double)tarrif.CallFee;
            }
            if (ToDConsideration != null)
                billingCDRCost.Discount = ((double)rate.NormalRate - ToDConsideration.ActiveRateValue(rate)) * (double)accountedDuration / 60;
            else
                billingCDRCost.Discount = 0;

            // Commission
            if (commission != null)
                billingCDRCost.CommissionValue = (billingCDRCost.RateValue - commission.DeductedRateValue(true, billingCDRCost.RateValue)) * (double)(mainDurationInSeconds) / 60;
            else
                billingCDRCost.CommissionValue = 0;

            // Extra Charge
            if (ExtraCharge != null)
                billingCDRCost.ExtraChargeValue = (billingCDRCost.RateValue - ExtraCharge.DeductedRateValue(true, billingCDRCost.RateValue)) * (double)accountedDuration / 60;
            else
                billingCDRCost.ExtraChargeValue = 0;

            if (tarrif != null && tarrif.FirstPeriod > 0 && !tarrif.RepeatFirstPeriod)
                accountedDuration += (double)tarrif.FirstPeriod;

            // updating the billing duration (if tarrif included)

            //if we have Tarrif accountedDuration Calculated with Ceiling value but when no Tarrif accountedDuration should also be assigned
            billingCDRCost.DurationInSeconds = (decimal)accountedDuration;
        }
    }
}
