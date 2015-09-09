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
    public class CostPricingManager : BasePricingManager
    {
        TOneCacheManager _cacheManager;
        public CostPricingManager(TOneCacheManager cacheManager)
            : base(cacheManager)
        {
            _cacheManager = cacheManager;
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
            Rate rateValue = GetSupplierRates(main.SupplierID, zoneId, main.Attempt.AddMinutes(supplierAccount.SupplierGMTTime));
            if (rateValue != null )
            {
                billingCDRCost.BillingCDRMainID = main.ID;
                billingCDRCost.RateID = rateValue.RateId;
                billingCDRCost.ZoneID = rateValue.ZoneId;
                billingCDRCost.RateValue = (double)rateValue.NormalRate;
                billingCDRCost.Net = (double)(rateValue.NormalRate * (main.DurationInSeconds / 60m));
                billingCDRCost.CurrencySymbol = rateValue.CurrencyID;
                billingCDRCost.DurationInSeconds = main.DurationInSeconds;
                Currency rateCurrency = Currencies.Find(c => c.CurrencyID == billingCDRCost.CurrencySymbol);

                #region Get Usables
                #region ToDConsideration
                // Effective and Active ToD for this Call?
                SupplierTODConsiderationInfo tod = GetSupplierToDConsiderations(main.SupplierID, billingCDRCost.ZoneID, main.Attempt);
                if ( tod!= null && tod.WasActive(main.Attempt))
                {
                    billingCDRCost.RateType = tod.RateType;
                    billingCDRCost.RateValue = (double)tod.ActiveRateValue(rateValue);
                    billingCDRCost.ToDConsiderationID =(int) tod.ToDConsiderationID;
                } 
                else
                    billingCDRCost.RateType = ToDRateType.Normal;

                var attemptDate = new DateTime(main.Attempt.Year, main.Attempt.Month, main.Attempt.Day);
                billingCDRCost.Attempt = attemptDate;
                Currency supplierCurrency = Currencies.Find(c => c.CurrencyID == supplierProfile.CurrencyID);
                #endregion ToDConsideration
                // Commissions or extra charges
                #region Commission
                SupplierCommission commissionsAndExtraCharges = GetCustomerCommissions(main.SupplierID, billingCDRCost.ZoneID, main.Attempt);
                SupplierCommission commission = null;
                SupplierCommission extraCharge = null;
                if (commissionsAndExtraCharges != null)
                {
                    var itemClone = (SupplierCommission)commissionsAndExtraCharges.Clone();
                    var pricingValue = (float?)GetRate((decimal)billingCDRCost.RateValue, rateCurrency, supplierCurrency, attemptDate);
                    if ((!commissionsAndExtraCharges.FromRate.HasValue || commissionsAndExtraCharges.FromRate <= pricingValue) && (!commissionsAndExtraCharges.ToRate.HasValue || commissionsAndExtraCharges.ToRate >= pricingValue))
                    {
                        if (commissionsAndExtraCharges.IsExtraCharge && billingCDRCost.ExtraChargeID <= 0)
                        {
                            commissionsAndExtraCharges.Amount = commissionsAndExtraCharges.Amount != null ? commissionsAndExtraCharges.Amount : (decimal)0.0; //in order if the commission value null apply the percentage and vise versa,also if the amount and perc found apply the two ammount then perc
                            itemClone.Amount = (decimal?)GetRate(commissionsAndExtraCharges.Amount.Value, supplierCurrency, rateCurrency, attemptDate);
                            billingCDRCost.ExtraChargeID = itemClone.ID;
                            extraCharge = itemClone;
                        }
                        if (billingCDRCost.CommissionID <= 0)//(Removed from if Condition !item.IsExtraCharge && FOr bug 1958)
                        {
                            commissionsAndExtraCharges.Amount = commissionsAndExtraCharges.Amount != null ? commissionsAndExtraCharges.Amount : (decimal)0.0;//in order if the commission value null apply the percentage and vise versa,also if the amount and perc found apply the two ammount then perc
                            itemClone.Amount = (decimal?)GetRate(commissionsAndExtraCharges.Amount.Value, supplierCurrency, rateCurrency, attemptDate);
                            billingCDRCost.CommissionID = itemClone.ID;
                            commission = itemClone;
                        }
                    }
                }
                
                #endregion Commission
                #region Tariff
                SupplierTariff tariffValue = GetSupplierTariff(main.SupplierID, zoneId, main.Attempt);
                if (tariffValue!=null)
                {
                    var tariffClone = (SupplierTariff)(tariffValue.Clone());
                    if (tariffValue.CallFee > 0)
                        tariffClone.CallFee = GetRate(tariffValue.CallFee, supplierCurrency, rateCurrency, attemptDate);
                    if (tariffValue.FirstPeriodRate > 0)
                        tariffClone.FirstPeriodRate = GetRate(tariffValue.FirstPeriodRate, supplierCurrency, rateCurrency, attemptDate);
                    billingCDRCost.TariffID = (int)tariffClone.TariffID;
                    tariffValue = tariffClone;
                }
                #endregion Tariff

                #endregion Get Usables

                billingCDRCost.Code = main.SupplierCode;
                CalculateAmounts(billingCDRCost, main.DurationInSeconds, supplierAccount.IsSupplierCeiling == IsCeiling.Y, tariffValue, tod, commission, rateValue, extraCharge);
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

        void CalculateAmounts(BillingCDRPricingBase billingCDRCost, decimal mainDurationInSeconds, bool isSupplierCeiling, SupplierTariff tarrif,
            SupplierTODConsiderationInfo ToDConsideration, SupplierCommission commission, Rate rate, SupplierCommission ExtraCharge)
        {
            double accountedDuration = (double)mainDurationInSeconds;
            billingCDRCost.Net = GetPricingNet(billingCDRCost.RateValue, mainDurationInSeconds, isSupplierCeiling, out accountedDuration);
            if (tarrif != null)
                accountedDuration = CalculateAndUpdateTarrif(billingCDRCost, tarrif, accountedDuration); // CalculateAndUpdateTarrif
            CalculateAndUpdateToDConsideration(billingCDRCost, accountedDuration, rate, ToDConsideration); // CalculateAndUpdateToDConsideration
            CalculateAndUpdateCommission(billingCDRCost, mainDurationInSeconds, rate, commission);// CalculateAndUpdateCommission
            CalculateAndUpdateExtraCharge(billingCDRCost, accountedDuration, ExtraCharge);// CalculateAndUpdateExtraCharge
            if (tarrif != null && tarrif.FirstPeriod > 0 && !tarrif.RepeatFirstPeriod)
                accountedDuration += (double)tarrif.FirstPeriod;
            // updating the billing duration (if tarrif included)
            //if we have Tarrif accountedDuration Calculated with Ceiling value but when no Tarrif accountedDuration should also be assigned
            billingCDRCost.DurationInSeconds = (decimal)accountedDuration;
        }
        private void CalculateAndUpdateExtraCharge(BillingCDRPricingBase billingCDRCost, double accountedDuration, SupplierCommission ExtraCharge)
        {
            if (ExtraCharge != null)
                billingCDRCost.ExtraChargeValue = (billingCDRCost.RateValue - ExtraCharge.DeductedRateValue(billingCDRCost.RateValue)) * (double)accountedDuration / 60;
            else
                billingCDRCost.ExtraChargeValue = 0;
        }
        private void CalculateAndUpdateCommission(BillingCDRPricingBase billingCDRCost, decimal mainDurationInSeconds, Rate rate, SupplierCommission commission)
        {
            if (commission != null)
                billingCDRCost.CommissionValue = (billingCDRCost.RateValue - commission.DeductedRateValue(billingCDRCost.RateValue)) * (double)(mainDurationInSeconds) / 60;
            else
                billingCDRCost.CommissionValue = 0;

        }
        private void CalculateAndUpdateToDConsideration(BillingCDRPricingBase billingCDRCost, double accountedDuration, Rate rate, SupplierTODConsiderationInfo ToDConsideration)
        {
            if (ToDConsideration != null)
                billingCDRCost.Discount = ((double)rate.NormalRate - ToDConsideration.ActiveRateValue(rate)) * (double)accountedDuration / 60;
            else
                billingCDRCost.Discount = 0;
        }
        private double CalculateAndUpdateTarrif(BillingCDRPricingBase billingCDRCost, SupplierTariff tarrif, double accountedDuration)
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
            return accountedDuration;
        }
        private Rate GetSupplierRates(String supplierID, int zoneID, DateTime whenEffective)
        {
            SupplierRateManager supplierRateManager = new SupplierRateManager(_cacheManager);
            return supplierRateManager.GetSupplierRates(supplierID, zoneID, whenEffective, true);
        }

        private SupplierTariff GetSupplierTariff(String supplierID, int zoneID, DateTime whenEffective)
        {
            SupplierTariffManager supplierTariffManager = new SupplierTariffManager(_cacheManager);
            return supplierTariffManager.GetSupplierTariff(supplierID, zoneID, whenEffective);
        }
        private SupplierCommission GetCustomerCommissions(String supplierID, int zoneID, DateTime whenEffective)
        {
            SupplierCommissionManager supplierCommissionManager = new SupplierCommissionManager(_cacheManager);
            return supplierCommissionManager.GetSupplierCommissions(supplierID, zoneID, whenEffective);
        }
        private SupplierTODConsiderationInfo GetSupplierToDConsiderations(String customerID, int zoneID, DateTime whenEffective)
        {
            SupplierTODManager customerTODManager = new SupplierTODManager(_cacheManager);
            return customerTODManager.GetSuppliersToDConsideration(customerID, zoneID, whenEffective);
        }
    }
}
