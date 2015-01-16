using System;
using System.Collections.Generic;

namespace TABS.Components.CustomerPool
{
    public class CustomerRatePool
    {
        public int OurZoneID { get; set; }
        public Zone Zone { get { return TABS.Zone.OwnZones.ContainsKey(OurZoneID) ? TABS.Zone.OwnZones[OurZoneID] : ObjectAssembler.Get<Zone>(OurZoneID); } }
        public decimal OurActiveRate { get; set; }
        public decimal OurNormalRate { get; set; }
        public decimal? OurOffPeakRate { get; set; }
        public decimal? OurWeekendRate { get; set; }
        public short OurServicesFlag { get; set; }
        public string CurrencyID { get; set; }
        public DateTime BeginEffectiveDate { get; set; }
        public DateTime? EndEffectiveDate { get; set; }
        public Dictionary<string, HashSet<Rate>> SupplyRatesByCode { get; set; }


        public static HashSet<CustomerRatePool> GetRates(CarrierAccount customer, DateTime when)
        {
            HashSet<CustomerRatePool> customerRates = new HashSet<CustomerRatePool>();

            var SQL = string.Format(@";WITH pricelistCTE AS (
	                                        SELECT pl.PriceListID, pl.CurrencyID
	                                          FROM PriceList pl WITH(NOLOCK) WHERE pl.CustomerID='{0}'
	                                          )
                                        SELECT r.ZoneID,
                                               r.Rate,
                                               pl.CurrencyID ,
                                               r.OffPeakRate,
                                               r.WeekendRate,
                                               r.ServicesFlag,
                                               r.BeginEffectiveDate,
                                               r.EndEffectiveDate
                                        FROM   Rate r WITH(NOLOCK,INDEX(IX_Rate_Pricelist)),
                                               pricelistCTE pl WITH(NOLOCK)
                                        WHERE  pl.PriceListID = r.PriceListID
                                               AND r.BeginEffectiveDate <= '{1:yyyy-MM-dd}'
                                               AND (
                                                       r.EndEffectiveDate IS NULL
                                                       OR r.EndEffectiveDate >= '{1:yyyy-MM-dd}'
                                                   ) 

                                        "
                                        , customer.CarrierAccountID
                                        , when);
            var ExtrachargeCommissions = CommissionCalculator.GetCommissions(true, true);
            var AmountCommissions = CommissionCalculator.GetCommissions(false, true);

            using (var reader = TABS.DataHelper.ExecuteReader(SQL))
            {
                while (reader.Read())
                {
                    int index = 0;
                    CustomerRatePool customerRatePool = new CustomerRatePool();

                    customerRatePool.OurZoneID = reader.GetInt32(index);
                    index++; customerRatePool.OurNormalRate = reader.GetDecimal(index);
                    index++; customerRatePool.CurrencyID = reader.GetString(index);
                    index++; customerRatePool.OurOffPeakRate = reader.IsDBNull(index) ? 0 : reader.GetDecimal(index);
                    index++; customerRatePool.OurWeekendRate = reader.IsDBNull(index) ? 0 : reader.GetDecimal(index);
                    index++; customerRatePool.OurServicesFlag = reader.GetInt16(index);
                    index++; customerRatePool.BeginEffectiveDate = reader.GetDateTime(index);
                    index++; customerRatePool.EndEffectiveDate = reader.IsDBNull(index) ? (DateTime?)null : reader.GetDateTime(index);

                    customerRatePool.SupplyRatesByCode = new Dictionary<string, HashSet<Rate>>();


                    var key = string.Concat("SYS", customer.CarrierAccountID, customerRatePool.OurZoneID);


                    var currency = TABS.Currency.All[customerRatePool.CurrencyID];
                    var factor = (decimal)(TABS.Currency.Main.LastRate / currency.LastRate);
                    var rateValue = customerRatePool.OurNormalRate;

                    decimal amountCommission = 0;
                    decimal extrachargeCommission = 0;

                    //calculate commissions 
                    if (AmountCommissions.ContainsKey(key))
                    {
                        var commission = AmountCommissions[key];
                        if (commission.Amount.HasValue && rateValue >= (decimal)commission.FromRate.Value && rateValue <= (decimal)commission.ToRate.Value)
                            amountCommission = commission.Amount.Value / factor;

                        if (commission.Percentage.HasValue && rateValue >= (decimal)commission.FromRate.Value && rateValue <= (decimal)commission.ToRate.Value)
                            amountCommission = rateValue * (decimal)commission.Percentage.Value / 100;


                    }
                    if (ExtrachargeCommissions.ContainsKey(key))
                    {
                        var commission = ExtrachargeCommissions[key];

                        if (commission.Amount.HasValue && rateValue >= (decimal)commission.FromRate.Value && rateValue <= (decimal)commission.ToRate.Value)
                            extrachargeCommission = commission.Amount.Value / factor;

                        if (commission.Percentage.HasValue && rateValue >= (decimal)commission.FromRate.Value && rateValue <= (decimal)commission.ToRate.Value)
                            extrachargeCommission = rateValue * (decimal)commission.Percentage.Value / 100;
                    }
                    customerRatePool.OurNormalRate += amountCommission + extrachargeCommission;

                    customerRates.Add(customerRatePool);

                }
            }


            return customerRates;
        }

    }
}
