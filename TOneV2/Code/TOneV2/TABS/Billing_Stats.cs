using System;
using System.Collections.Generic;

namespace TABS
{
    /// <summary>
    /// Billing Stats. 
    /// </summary>
    public class Billing_Stats
    {
        public virtual DateTime CallDate { get; set; }
        public virtual CarrierAccount Customer { get; set; }
        public virtual CarrierAccount Supplier { get; set; }
        public virtual Zone CostZone { get; set; }
        public virtual Zone SaleZone { get; set; }
        public virtual Currency Cost_Currency { get; set; }
        public virtual Currency Sale_Currency { get; set; }
        public virtual decimal SaleDuration { get; set; }
        public virtual decimal CostDuration { get; set; }
        public virtual int NumberOfCalls { get; set; }
        public virtual TimeSpan FirstCallTime { get; set; }
        public virtual TimeSpan LastCallTime { get; set; }
        public virtual decimal MinDuration { get; set; }
        public virtual decimal MaxDuration { get; set; }
        public virtual decimal AvgDuration { get; set; }
        public virtual decimal Cost_Nets { get; set; }
        public virtual decimal Cost_Discounts { get; set; }
        public virtual decimal Cost_Commissions { get; set; }
        public virtual decimal Cost_ExtraCharges { get; set; }
        public virtual decimal Sale_Nets { get; set; }
        public virtual decimal Sale_Discounts { get; set; }
        public virtual decimal Sale_Commissions { get; set; }
        public virtual decimal Sale_ExtraCharges { get; set; }
        public virtual decimal Sale_Rate { get; set; }
        public virtual decimal Cost_Rate { get; set; }

        /// <summary>
        /// Group the given cdrs into the grouping
        /// </summary>
        /// <param name="grouping"></param>
        /// <param name="cdrs"></param>
        public static void Group(Dictionary<string, Billing_Stats> grouping, IEnumerable<Billing_CDR_Base> cdrs)
        {
            foreach (var cdrBase in cdrs)
            {
                if (cdrBase.IsValid)
                {
                    Billing_CDR_Main cdr = (Billing_CDR_Main)cdrBase;
                    Billing_Stats group = null;
                    string key = GetGroupingKey(cdr);

                    // Find or create group
                    if (!grouping.TryGetValue(key, out group))
                    {
                        group = new Billing_Stats();
                        group.CallDate = cdr.Attempt.Date;
                        group.Customer = cdr.Customer;
                        group.SaleZone = cdr.OurZone;
                        group.Sale_Currency = cdr.Billing_CDR_Sale == null ? null : cdr.Billing_CDR_Sale.Currency;
                        group.Supplier = cdr.Supplier;
                        group.CostZone = cdr.SupplierZone;
                        group.Cost_Currency = cdr.Billing_CDR_Cost == null ? null : cdr.Billing_CDR_Cost.Currency;
                        grouping[key] = group;
                    }

                    // General
                    group.SaleDuration += cdr.Billing_CDR_Sale.DurationInSeconds;
                    group.CostDuration += cdr.Billing_CDR_Cost.DurationInSeconds;
                    group.NumberOfCalls++;
                    if (cdr.Attempt.TimeOfDay < group.FirstCallTime) group.FirstCallTime = cdr.Attempt.TimeOfDay;
                    if (cdr.Attempt.TimeOfDay > group.LastCallTime) group.LastCallTime = cdr.Attempt.TimeOfDay;
                    if (cdr.DurationInSeconds < group.MinDuration) group.MinDuration = cdr.DurationInSeconds;
                    if (cdr.DurationInSeconds > group.MaxDuration) group.MaxDuration = cdr.DurationInSeconds;
                    group.AvgDuration = (group.AvgDuration * (group.NumberOfCalls - 1) + cdr.DurationInSeconds) / (decimal)group.NumberOfCalls;

                    // Sale
                    if (cdr.Billing_CDR_Sale != null)
                    {
                        group.Sale_Nets += (decimal)cdr.Billing_CDR_Sale.Net;
                        group.Sale_Discounts += cdr.Billing_CDR_Sale.Discount.HasValue ? (decimal)cdr.Billing_CDR_Sale.Discount.Value : 0;
                        group.Sale_Commissions += (decimal)cdr.Billing_CDR_Sale.CommissionValue;
                        group.Sale_ExtraCharges += (decimal)cdr.Billing_CDR_Sale.ExtraChargeValue;
                        group.Sale_Rate = (group.Sale_Rate * (group.NumberOfCalls - 1) + (decimal)cdr.Billing_CDR_Sale.RateValue) / (decimal)group.NumberOfCalls;
                    }

                    // Cost
                    if (cdr.Billing_CDR_Cost != null)
                    {
                        group.Cost_Nets += (decimal)cdr.Billing_CDR_Cost.Net;
                        group.Cost_Discounts += cdr.Billing_CDR_Cost.Discount.HasValue ? (decimal)cdr.Billing_CDR_Cost.Discount.Value : 0;
                        group.Cost_Commissions += (decimal)cdr.Billing_CDR_Cost.CommissionValue;
                        group.Cost_ExtraCharges += (decimal)cdr.Billing_CDR_Cost.ExtraChargeValue;
                        group.Cost_Rate = (group.Cost_Rate * (group.NumberOfCalls - 1) + (decimal)cdr.Billing_CDR_Cost.RateValue) / (decimal)group.NumberOfCalls;
                    }
                }
            }
        }

        /// <summary>
        /// Create a grouping key for billing statistics
        /// Important Note:
        /// ---------------
        /// When grouping changes in the Stored Procedure bp_BuildBillingStats it is vital that
        /// grouping here changes
        /// </summary>
        /// <param name="cdr"></param>
        /// <returns></returns>
        public static string GetGroupingKey(Billing_CDR_Main cdr)
        {
            return string.Concat(
                    cdr.Attempt.ToString("yyMMdd"),
                    cdr.CustomerID,
                    cdr.OurZone.ZoneID.ToString(),
                    cdr.Billing_CDR_Sale == null ? string.Empty : cdr.Billing_CDR_Sale.Currency.Symbol,
                    cdr.SupplierID,
                    cdr.SupplierZone.ZoneID.ToString(),
                    cdr.Billing_CDR_Cost == null ? string.Empty : cdr.Billing_CDR_Cost.Currency.Symbol
                );
        }
    }
}
