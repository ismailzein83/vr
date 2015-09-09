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
    public class SalePricingManager : BasePricingManager
    {
        TOneCacheManager _cacheManager;
        public SalePricingManager(TOneCacheManager cacheManager)
            : base(cacheManager)
        {
            _cacheManager = cacheManager;
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
              Rate rateValue = GetSaleRates(main.CustomerID, zoneId, main.Attempt);
              if (rateValue != null)
            {
                billingCDRSale.BillingCDRMainID = main.ID;
                billingCDRSale.RateID = rateValue.RateId;
                billingCDRSale.ZoneID = rateValue.ZoneId;
                billingCDRSale.RateValue = (double)rateValue.NormalRate;
                billingCDRSale.Net = (double)(rateValue.NormalRate * (main.DurationInSeconds / 60m));
                billingCDRSale.CurrencySymbol = rateValue.CurrencyID;
                billingCDRSale.DurationInSeconds = main.DurationInSeconds;
                Rate rate = rateValue;
                Currency rateCurrency = Currencies.Find(c => c.CurrencyID == billingCDRSale.CurrencySymbol);
                #region ToDConsideration
                // Effective and Active ToD for this Call?
                CustomerTODConsiderationInfo tod = GetCustomerToDConsiderations(main.CustomerID, billingCDRSale.ZoneID, main.Attempt);
                if (tod != null && tod.WasActive(main.Attempt) )
                {
                        billingCDRSale.RateType = tod.RateType;
                        billingCDRSale.RateValue = (double)tod.ActiveRateValue(rateValue);
                        billingCDRSale.ToDConsiderationID =(int) tod.ToDConsiderationID;
                }
                else
                    billingCDRSale.RateType = ToDRateType.Normal;
                var attemptDate = new DateTime(main.Attempt.Year, main.Attempt.Month, main.Attempt.Day);
                billingCDRSale.Attempt = attemptDate;
                Currency customerCurrency = Currencies.Find(c => c.CurrencyID == customerProfile.CurrencyID);

                #endregion ToDConsideration
                #region Commission
                CustomerCommission commissionsAndExtraCharges = GetCustomerCommissions(main.CustomerID, billingCDRSale.ZoneID, main.Attempt);
                 
                CustomerCommission commission = null;
                CustomerCommission extraCharge = null;
                if (commissionsAndExtraCharges != null)
                {
                  
                    var itemClone = (CustomerCommission)commissionsAndExtraCharges.Clone();
                    var pricingValue = (float?)GetRate((decimal)billingCDRSale.RateValue, rateCurrency, customerCurrency, attemptDate);

                    if ((!commissionsAndExtraCharges.FromRate.HasValue || commissionsAndExtraCharges.FromRate <= pricingValue) && (!commissionsAndExtraCharges.ToRate.HasValue || commissionsAndExtraCharges.ToRate >= pricingValue))
                    {
                        if (commissionsAndExtraCharges.IsExtraCharge && billingCDRSale.ExtraChargeID <= 0)
                        {
                            commissionsAndExtraCharges.Amount = commissionsAndExtraCharges.Amount != null ? commissionsAndExtraCharges.Amount : (decimal)0.0; //in order if the commission value null apply the percentage and vise versa,also if the amount and perc found apply the two ammount then perc
                            itemClone.Amount = (decimal?)GetRate(commissionsAndExtraCharges.Amount.Value, customerCurrency, rateCurrency, attemptDate);
                            billingCDRSale.ExtraChargeID = itemClone.ID;
                            extraCharge = itemClone;
                        }
                        if (billingCDRSale.CommissionID <= 0)//(Removed from if Condition !item.IsExtraCharge && FOr bug 1958)
                        {
                            commissionsAndExtraCharges.Amount = commissionsAndExtraCharges.Amount != null ? commissionsAndExtraCharges.Amount : (decimal)0.0;//in order if the commission value null apply the percentage and vise versa,also if the amount and perc found apply the two ammount then perc
                            itemClone.Amount = (decimal?)GetRate(commissionsAndExtraCharges.Amount.Value, customerCurrency, rateCurrency, attemptDate);
                            billingCDRSale.CommissionID = itemClone.ID;
                            commission = itemClone;
                        }
                    }
                }
                
                
                #endregion Commission
                 #region Tariff
                 // Tariff Considered?
                 CustomerTariff tariffValue = GetSalesTariff(main.CustomerID, zoneId, main.Attempt);

                if (tariffValue!=null)
                {


                    var tariffClone = (CustomerTariff)(tariffValue.Clone());

                    if (tariffValue.CallFee > 0)
                        tariffClone.CallFee = GetRate(tariffValue.CallFee, customerCurrency, rateCurrency, attemptDate);

                    if (tariffValue.FirstPeriodRate > 0)
                        tariffClone.FirstPeriodRate = GetRate(tariffValue.FirstPeriodRate, customerCurrency, rateCurrency, attemptDate);

                    billingCDRSale.TariffID = (int)tariffClone.TariffID;
                    tariffValue = tariffClone;
                }
                 #endregion Tariff
                billingCDRSale.Code = main.OurCode;
                CalculateAmounts(billingCDRSale, main.DurationInSeconds, customerAccount.IsCustomerCeiling == IsCeiling.Y, tariffValue, tod, commission, rate, extraCharge);

            }
             else
            {
                // No suitable rate found for this Code / Supplier / Customer Combination
                billingCDRSale = null;
            }
            return billingCDRSale;
        }

        void CalculateAmounts(BillingCDRPricingBase billingCDRSale, decimal mainDurationInSeconds, bool isCustomerCeiling, CustomerTariff tarrif,
             CustomerTODConsiderationInfo ToDConsideration, CustomerCommission commission, Rate rate, CustomerCommission ExtraCharge)
        {
            double accountedDuration = (double)mainDurationInSeconds;
            billingCDRSale.Net = GetPricingNet(billingCDRSale.RateValue, mainDurationInSeconds, isCustomerCeiling, out accountedDuration);
            // Tariff?
            if (tarrif != null)
                accountedDuration = CalculateAndUpdateTarrif(billingCDRSale, tarrif, accountedDuration);
            //TOD
            CalculateAndUpdateToDConsideration(billingCDRSale, accountedDuration, rate, ToDConsideration);
            // Commission
            CalculateAndUpdateCommission(billingCDRSale, mainDurationInSeconds, rate, commission);
            // Extra Charge
            CalculateAndUpdateExtraCharge(billingCDRSale, accountedDuration, ExtraCharge);
            if (tarrif != null && tarrif.FirstPeriod > 0 && !tarrif.RepeatFirstPeriod)
                accountedDuration += (double)tarrif.FirstPeriod;
            // updating the billing duration (if tarrif included)
            //if we have Tarrif accountedDuration Calculated with Ceiling value but when no Tarrif accountedDuration should also be assigned
            billingCDRSale.DurationInSeconds = (decimal)accountedDuration;
        }
        public void CalculateAndUpdateExtraCharge(BillingCDRPricingBase billingCDRSale, double accountedDuration, CustomerCommission ExtraCharge)
        {
            if (ExtraCharge != null)
                billingCDRSale.ExtraChargeValue = (billingCDRSale.RateValue - ExtraCharge.DeductedRateValue(billingCDRSale.RateValue)) * (double)accountedDuration / 60;
            else
                billingCDRSale.ExtraChargeValue = 0;
        }
        public void CalculateAndUpdateCommission(BillingCDRPricingBase billingCDRSale, decimal mainDurationInSeconds, Rate rate, CustomerCommission commission)
        {
            if (commission != null)
                billingCDRSale.CommissionValue = (billingCDRSale.RateValue - commission.DeductedRateValue(billingCDRSale.RateValue)) * (double)(mainDurationInSeconds) / 60;
            else
                billingCDRSale.CommissionValue = 0;
        }
        public void CalculateAndUpdateToDConsideration(BillingCDRPricingBase billingCDRSale, double accountedDuration, Rate rate, CustomerTODConsiderationInfo ToDConsideration)
        {
            if (ToDConsideration != null)
                billingCDRSale.Discount = ((double)rate.NormalRate - ToDConsideration.ActiveRateValue(rate)) * (double)accountedDuration / 60;
            else
                billingCDRSale.Discount = 0;
        }
        public double CalculateAndUpdateTarrif(BillingCDRPricingBase billingCDRSale, CustomerTariff tarrif, double accountedDuration)
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
                return accountedDuration;
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
        public Rate GetSaleRates(String customerID, int zoneID, DateTime whenEffective)
        {
            SaleRateManager saleRateManager = new SaleRateManager(_cacheManager);
            return saleRateManager.GetSaleRates(customerID, zoneID, whenEffective); 
        }
        public CustomerTariff GetSalesTariff(String customerID, int zoneID, DateTime whenEffective)
        {
            CustomerTariffManager customerTariffManager = new CustomerTariffManager(_cacheManager);
            return customerTariffManager.GetCustomerTariff(customerID, zoneID, whenEffective);
        }
        public CustomerCommission GetCustomerCommissions(String customerID, int zoneID, DateTime whenEffective)
        {
            CustomerCommissionManager customerCommissionManager=new CustomerCommissionManager(_cacheManager);
            return customerCommissionManager.GetCustomerCommissions(customerID, zoneID, whenEffective);
        }
        public CustomerTODConsiderationInfo GetCustomerToDConsiderations(String customerID, int zoneID, DateTime whenEffective)
        {
            CustomerTODManager customerTODManager = new CustomerTODManager(_cacheManager);
            return customerTODManager.GetCustomerToDConsideration(customerID, zoneID, whenEffective);
        }
    }
}
