using System;
using System.Collections.Generic;
using System.Linq;

namespace TABS.Addons.PriceListExport
{
    public class PricelistConstructor
    {
        static log4net.ILog log = log4net.LogManager.GetLogger(typeof(PricelistConstructor));

        public static event Action<TABS.PriceList, List<RateCode>> PricelistRateCodesGenerated;

        private static readonly object incrementLocker = new object();
        private static readonly object rateChangeLocker = new object();

        TABS.PriceList Pricelist { get; set; }


        public PricelistConstructor(TABS.PriceList _pricelist)
        {
            Pricelist = _pricelist;
        }

        /// <summary>
        /// get the rates of the current pricelist 
        /// </summary>
        /// <returns></returns>
        public List<RateCode> GetRateCodes()
        {
            _rates = null;
            //if (Pricelist.Rates.Values.Count >= 100)
            //{ return GetThreadedRateCodes(); }
            //else
            //{
                List<RateCode> results = new List<RateCode>();


                //sw.Start();
                foreach (TABS.RateBase rate in Pricelist.Rates.Values)
                {
                    string ratechange = GetRateChange(rate);
                    string incremrnt = GetIncrement(rate);
                    bool isratepending = IsRatePending(rate);

                    if(!ratechange.Equals("R"))
                    {
                        foreach (Code code in rate.EffectiveCodes)
                        {
                            RateCode ratecode = new RateCode();
                            //ratecode.CurrentRate = (TABS.Zone.OwnZones.Keys.Contains(rate.ZoneID) && TABS.Zone.OwnZones[rate.ZoneID].IsEffective) ? TABS.Zone.OwnZones[rate.ZoneID].EffectiveRate : null;
                            ratecode.Rate = rate;
                            ratecode.Zone = rate.Zone;
                            ratecode.Code = code;
                            ratecode.CodeChange = GetCodeChange(code);
                            ratecode.RateChange = ratechange;
                            ratecode.Increment = incremrnt;
                            ratecode.IsCodePending = IsCodePending(code);
                            ratecode.IsRatePending = isratepending;
                            ratecode.ServiceSymbol = rate.Zone.Services; //TABS.FlaggedService.GetBySymbol(ratecode.Zone.ServicesFlag.ToString()).Symbol;
                            if (!ratecode.CodeChange.Equals("R"))
                            {
                                results.Add(ratecode);
                            }
                        }
                    }
                }
                //sw.Stop();
                // 
                // Check if Pricelist Rate Codes generation is hooked
                //

                if (PricelistRateCodesGenerated != null)
                {
                    try
                    {
                        PricelistRateCodesGenerated(Pricelist, results);
                    }
                    catch (Exception ex)
                    {
                        log.Error("Error Raising Pricelist Rate Code Generation event", ex);

                        // restore results?
                    }
                }


                return results;
            //}
        }




        protected DateTime PEDPlusNotification { get { return Pricelist.BeginEffectiveDate.Value.AddDays(RateNotification); } }
        protected DateTime PED { get { return Pricelist.BeginEffectiveDate.Value; } }
        protected CodeView codeView { get { return (CodeView)((byte)TABS.SystemParameter.CodeView.NumericValue); } }

        protected double SystemRateNotification { get { return (double)TABS.SystemConfiguration.KnownParameters[TABS.KnownSystemParameter.sys_BeginEffectiveRateDays].NumericValue.Value; } }
        protected Dictionary<RateBase, string> rateChanges = new Dictionary<RateBase, string>();
        protected Dictionary<RateBase, string> rateIncrements = new Dictionary<RateBase, string>();
        /// <summary>
        /// Rate notification days 
        /// </summary>
        protected double RateNotification
        {
            get
            {
                if (Pricelist.Supplier == TABS.CarrierAccount.SYSTEM)
                    return Pricelist.Customer.RateIncreaseDays.HasValue ? (double)Pricelist.Customer.RateIncreaseDays.Value : SystemRateNotification;
                else
                    return Pricelist.Supplier.RateIncreaseDays.HasValue ? (double)Pricelist.Supplier.RateIncreaseDays.Value : SystemRateNotification;
            }
        }

        /// <summary>
        /// Get rate change Display value
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        protected string GetRateChange(TABS.RateBase r)
        {
            if (Pricelist.ID > 0)
            {
                string changes;
                if (r.EndEffectiveDate.HasValue) return "R";

                if (r.Change == TABS.Change.None)
                    changes = "S";
                else if (r.Change == TABS.Change.Increase)
                    changes = "I";
                else if (r.Change == TABS.Change.Decrease)
                    changes = "D";
                else
                    changes = "N";

                return changes;
            }
            else
            {
                string changes;

                if (r.EndEffectiveDate.HasValue) return "R";

                Dictionary<string, TABS.Rate> ratesTocompare = new Dictionary<string, Rate>();
                foreach (TABS.Rate rate in rates.Values)
                {
                    ratesTocompare[rate.Zone.Name] = rate;
                }

                if (ratesTocompare.ContainsKey(r.Zone.Name))
                {
                    if (r.Change == TABS.Change.None)
                        changes = "S";
                    else if (r.Change == TABS.Change.Increase)
                        changes = "I";
                    else if (r.Change == TABS.Change.Decrease)
                        changes = "D";
                    else
                        changes = "N";
                }
                else
                    changes = "N";

                if (r.EndEffectiveDate != null && r.EndEffectiveDate >= PED)   //&& r.EndEffectiveDate <= PEDPlusNotification)
                    changes = "R";

                return changes;
            }

        }

        /// <summary>
        /// Get code change Display value
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        protected string GetCodeChange(TABS.Code c)
        {
            string changes = "S";

            if (c.BeginEffectiveDate >= PED && c.BeginEffectiveDate <= PEDPlusNotification)
                changes = "N";

            if (c.EndEffectiveDate != null && c.EndEffectiveDate >= PED) //  && c.EndEffectiveDate <= PEDPlusNotification)
                changes = "R";

            return changes;
        }


        /// <summary>
        /// determining if the code is pending according to the pricelist 
        /// </summary>
        /// <param name="Entity"></param>
        /// <returns></returns>
        protected bool IsCodePending(TABS.Code Entity)
        {
            if (Entity.BeginEffectiveDate >= PED && Entity.BeginEffectiveDate <= PEDPlusNotification)
                return true;

            return false;
        }


        /// <summary>
        /// determinig if the rate is pending according to the pricelist
        /// </summary>
        /// <param name="Entity"></param>
        /// <returns></returns>
        protected bool IsRatePending(TABS.RateBase Entity)
        {
            if (Entity.BeginEffectiveDate >= PED && Entity.BeginEffectiveDate <= PEDPlusNotification)
                return true;

            return false;
        }

        /// <summary>
        /// Get increment display : Tariffs
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        protected string GetIncrement(TABS.RateBase r)
        {
            //lock (incrementLocker)
            //{
                string increment;
                //if (!rateIncrements.TryGetValue(r, out increment))
                //{
                    List<TABS.Tariff> tariffs = SaleTariffs.Where(t => t.Zone.Name.Equals(r.Zone.Name)).ToList();

                    TABS.Tariff tariff = null;

                    if (tariffs != null && tariffs.Count > 0)
                    {
                        tariff = tariffs.First();
                    }


                    increment = tariff != null ? tariff.IncrementDisplay : "1/1";

                    //rateIncrements.Add(r, increment);

                    return increment;
            //    }
            //    else { return increment; }
            //}
        }

        protected internal Dictionary<Zone, Rate> _rates;
        /// <summary>
        /// get the effective rates for the current customer 
        /// </summary>
        protected Dictionary<Zone, Rate> rates
        {
            get
            {
                if (_rates == null)
                {
                    if (Pricelist.ID > 0)
                        _rates = TABS.ObjectAssembler.GetLastEffectiveRatesBefore(Pricelist);
                    else
                        _rates = GetSaleRates();
                }
                return _rates;
            }
            set { _rates = value; }
        }


        protected IList<Tariff> _SaleTariffs;
        protected IList<Tariff> SaleTariffs
        {
            get
            {
                if (_SaleTariffs == null)
                {
                   
                        _SaleTariffs = Pricelist.Rates.Keys.SelectMany(key => key.EffectiveTariffs).ToList();
                    //}
                    //else
                    //    _SaleTariffs = TABS.ObjectAssembler.GetTariffs(this.Pricelist.Supplier, this.Pricelist.Customer, DateTime.Now);
                }
                return _SaleTariffs;
            }
            set
            {
                _SaleTariffs = value;
            }

        }
        protected Dictionary<Zone, Rate> GetSaleRates()
        {
            SaleTariffs = null;
            Dictionary<Zone, Rate> saleRates = new Dictionary<Zone, Rate>();

            IList<TABS.Rate> resutls = DataConfiguration.CurrentSession
                       .CreateQuery(@"FROM Rate R 
                          WHERE     
                                R.PriceList.Supplier = :Supplier 
                            AND R.PriceList.Customer = :Customer
                            AND ((R.BeginEffectiveDate < :when AND (R.EndEffectiveDate IS NULL OR R.EndEffectiveDate > :when)) OR R.BeginEffectiveDate > :when)")
                        .SetParameter("Supplier", TABS.CarrierAccount.SYSTEM)
                        .SetParameter("Customer", Pricelist.Customer)
                        .SetParameter("when", DateTime.Now)
                        .List<TABS.Rate>();

            foreach (TABS.Rate rate in resutls)
                saleRates[rate.Zone] = rate;

            return saleRates;
        }

        /// <summary>
        /// Contains all information of the current pricelist
        /// </summary>
        public class RateCode
        {
            public TABS.Zone Zone { get; set; }
            public TABS.Code Code { get; set; }
            public string CodeChange { get; set; }
            public TABS.RateBase Rate { get; set; }
            public TABS.RateBase CurrentRate { get; set; }
            public string Route { get; set; }
            public string Increment { get; set; }
            public string RateChange { get; set; }
            public bool IsRatePending { get; set; }
            public bool IsCodePending { get; set; }
            public string ServiceSymbol { get; set; }
            public DateTime? EED
            {
                get { return this.Code.EndEffectiveDate != null ? this.Code.EndEffectiveDate : this.Rate.EndEffectiveDate; }
            }

         

            public DateTime? HighestPendingDate
            {
                get
                {

                    if (IsRatePending || IsCodePending)
                        return Rate.BeginEffectiveDate > Code.BeginEffectiveDate ? Rate.BeginEffectiveDate : Code.BeginEffectiveDate;
                    else
                        return Rate.BeginEffectiveDate;
                }
            }
        }
    }
}
