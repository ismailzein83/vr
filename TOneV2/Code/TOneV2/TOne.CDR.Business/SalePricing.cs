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
    public class SalePricing : BasePricing
    {
        public SalePricing(TOneCacheManager cacheManager)
            : base(cacheManager)
        {

        }

        public BillingCDRSale GetRepricing(BillingCDRMain main)
        {
            BillingCDRSale billingCDRSale = new BillingCDRSale();
            int zoneId =  main.OurZoneID;
           
            CarrierAccountManager carrierAccountManager=new CarrierAccountManager();
            CarrierProfileManager carrierProfileManager = new CarrierProfileManager();

              CarrierAccount customerAccount = carrierAccountManager.GetCarrierAccount(main.CustomerID);
              CarrierProfile customerProfile = carrierProfileManager.GetCarrierProfile(customerAccount.ProfileId);
              if (customerAccount == null) return null;
              IList<Rate> rates = GetRates(main.CustomerID, zoneId, main.Attempt);
            if (rates != null && rates.Count > 0)
            {
                // Initialize Pricing
                billingCDRSale.BillingCDRMainID = main.ID;
                billingCDRSale.RateID = rates[0].RateId;
                billingCDRSale.ZoneID = rates[0].ZoneId;
                billingCDRSale.RateValue = (double)rates[0].NormalRate;
                billingCDRSale.Net = (double)(rates[0].NormalRate * (main.DurationInSeconds / 60m));
                billingCDRSale.CurrencySymbol = rates[0].CurrencyID;
                billingCDRSale.DurationInSeconds = main.DurationInSeconds;

                Rate rate = rates[0];
               
                Currency rateCurrency = Currencies.Find(c => c.CurrencyID == billingCDRSale.CurrencySymbol);
                //if (rateCurrency==null)
                //   rateCurrency= new Currency();
                ToDConsideration tod = null;
                Tariff tariff = null;

                // Effective and Active ToD for this Call?
                IList<ToDConsideration> tods = GetToDConsiderations(main.CustomerID, billingCDRSale.ZoneID, main.Attempt);
                foreach (ToDConsideration effective in tods) { if (effective.WasActive(main.Attempt)) { tod = effective; break; } }

                 // Check for ToD first
                if (tod != null)
                {
                    billingCDRSale.RateType = tod.RateType;
                    billingCDRSale.RateValue = (double)tod.ActiveRateValue(rates[0]);
                    billingCDRSale.ToDConsiderationID = tod.ToDConsiderationID;
                }
                else
                    billingCDRSale.RateType = ToDRateType.Normal;

                 var attemptDate = new DateTime(main.Attempt.Year, main.Attempt.Month, main.Attempt.Day);
                billingCDRSale.Attempt = attemptDate;

                // Commissions or extra charges
                List<Commission> commissionsAndExtraCharges = GetCommissions(main.CustomerID, billingCDRSale.ZoneID, main.Attempt);

                Commission commission = null;
                Commission extraCharge = null;
                Currency customerCurrency = Currencies.Find(c => c.CurrencyID == customerProfile.CurrencyID);
                 foreach (Commission item in commissionsAndExtraCharges)
                {
                    var itemClone = (Commission)item.Clone();
                    
                    var pricingValue = (float?)GetRate((decimal)billingCDRSale.RateValue, rateCurrency, customerCurrency, attemptDate);

                    if ((!item.FromRate.HasValue || item.FromRate <= pricingValue) && (!item.ToRate.HasValue || item.ToRate >= pricingValue))
                    {
                        if (item.IsExtraCharge && billingCDRSale.ExtraChargeID <= 0)
                        {
                            item.Amount = item.Amount != null ? item.Amount : (decimal)0.0; //in order if the commission value null apply the percentage and vise versa,also if the amount and perc found apply the two ammount then perc
                            //if (item.Amount != null && item.Amount != 0)
                            itemClone.Amount = (decimal?)GetRate(item.Amount.Value, customerCurrency, rateCurrency, attemptDate);
                            billingCDRSale.ExtraChargeID = itemClone.ID;

                            extraCharge = itemClone;
                        }

                        if (billingCDRSale.CommissionID <= 0)//(Removed from if Condition !item.IsExtraCharge && FOr bug 1958)
                        {
                            item.Amount = item.Amount != null ? item.Amount : (decimal)0.0;//in order if the commission value null apply the percentage and vise versa,also if the amount and perc found apply the two ammount then perc
                            //if (item.Amount != null && item.Amount != 0)
                            itemClone.Amount = (decimal?)GetRate(item.Amount.Value, customerCurrency, rateCurrency, attemptDate);
                            billingCDRSale.CommissionID = itemClone.ID;

                            commission = itemClone;
                        }
                    }
                }


                 // Tariff Considered?
                IList<Tariff> tariffs = GetTariffs(main.CustomerID, zoneId, main.Attempt);

                Tariff tarrif = null;

                if (tariffs.Count > 0)
                {
                    tariff = tariffs[0];

                    var tariffClone = (Tariff)(tariff.Clone());

                    if (tariff.CallFee > 0)
                        tariffClone.CallFee = GetRate(tariff.CallFee, customerCurrency, rateCurrency, attemptDate);

                    if (tariff.FirstPeriodRate > 0)
                        tariffClone.FirstPeriodRate = GetRate(tariff.FirstPeriodRate, customerCurrency, rateCurrency, attemptDate);

                    billingCDRSale.TariffID = tariffClone.TariffID;
                    tarrif = tariffClone;
                }

                billingCDRSale.Code = main.OurCode;
                CalculateAmounts(billingCDRSale, main.DurationInSeconds, customerAccount.IsCustomerCeiling == IsCeiling.Y, tarrif, tod, commission, rate, extraCharge);

            }
             else
            {
                // No suitable rate found for this Code / Supplier / Customer Combination
                billingCDRSale = null;
            }
            return billingCDRSale;
        }

        void CalculateAmounts(BillingCDRPricingBase billingCDRSale, decimal mainDurationInSeconds, bool isCustomerCeiling, Tariff tarrif,
       ToDConsideration ToDConsideration, Commission commission, Rate rate, Commission ExtraCharge)
        {
            double accountedDuration = (double)mainDurationInSeconds;
            billingCDRSale.Net = GetPricingNet(billingCDRSale.RateValue, mainDurationInSeconds, isCustomerCeiling, out accountedDuration);

            // Tariff?
            if (tarrif != null)
            {
                billingCDRSale.Net = 0;

                // Calculate the amount for the firt period
                if (tarrif.FirstPeriod > 0)
                {
                    billingCDRSale.FirstPeriod = tarrif.FirstPeriod;
                    double firstPeriodRate = (tarrif.FirstPeriodRate > 0) ? (double)tarrif.FirstPeriodRate : billingCDRSale.RateValue;
                    if (!tarrif.RepeatFirstPeriod)
                    {
                        // Calculate first period amount then continue normally
                        billingCDRSale.Net = firstPeriodRate;//(tarrif.FirstPeriod * firstPeriodRate) / 60
                        accountedDuration -= (double)tarrif.FirstPeriod;
                        accountedDuration = Math.Max(0, accountedDuration);
                    }
                }

                // if there is a fraction unit
                if (tarrif.FractionUnit > 0)
                {
                    billingCDRSale.FractionUnit = (byte)tarrif.FractionUnit;
                    accountedDuration = Math.Ceiling(accountedDuration / tarrif.FractionUnit) * tarrif.FractionUnit;
                    billingCDRSale.Net += Math.Ceiling(accountedDuration / tarrif.FractionUnit) * billingCDRSale.RateValue;// 60
                }
                else// Calculate the net amount
                    billingCDRSale.Net += (accountedDuration * billingCDRSale.RateValue) / 60;

                // Calculate the Net from the Tariff
                billingCDRSale.Net += (double)tarrif.CallFee;
            }
            if (ToDConsideration != null)
                billingCDRSale.Discount = ((double)rate.NormalRate - ToDConsideration.ActiveRateValue(rate)) * (double)accountedDuration / 60;
            else
                billingCDRSale.Discount = 0;

            // Commission
            if (commission != null)
                billingCDRSale.CommissionValue = (billingCDRSale.RateValue - commission.DeductedRateValue(false, billingCDRSale.RateValue)) * (double)(mainDurationInSeconds) / 60;
            else
                billingCDRSale.CommissionValue = 0;

            // Extra Charge
            if (ExtraCharge != null)
                billingCDRSale.ExtraChargeValue = (billingCDRSale.RateValue - ExtraCharge.DeductedRateValue(false, billingCDRSale.RateValue)) * (double)accountedDuration / 60;
            else
                billingCDRSale.ExtraChargeValue = 0;

            if (tarrif != null && tarrif.FirstPeriod > 0 && !tarrif.RepeatFirstPeriod)
                accountedDuration += (double)tarrif.FirstPeriod;

            // updating the billing duration (if tarrif included)

            //if we have Tarrif accountedDuration Calculated with Ceiling value but when no Tarrif accountedDuration should also be assigned
            billingCDRSale.DurationInSeconds = (decimal)accountedDuration;
        }
        private double GetPricingNet( double rateValue, decimal durationInSeconds, bool isCustomerCeiling, out double accountedDuration)
        {
            accountedDuration = (double)durationInSeconds;
            double net = (rateValue * (double)durationInSeconds) / 60;

                if (isCustomerCeiling)
                {
                    accountedDuration = (double)Math.Ceiling(durationInSeconds);
                    net = rateValue * (double)accountedDuration / 60.0;
                }
            return net;
        }



       
    }
}
