using System;
using System.Collections.Generic;
using System.Linq;
namespace TABS.DTO
{
    public class DTO_EditablePlanningRate : TABS.PlaningRate, IComparable<DTO_EditablePlanningRate>,IDisposable
    {
        public override string Identifier { get { return this.Zone.Identifier; } }

        public virtual TABS.Rate PendingRate { get; set; }

        public virtual List<TABS.Rate> ZoneRates { get; set; }
        private int _overrideIncludeZoneBlock =-1;//depand on System parameter
        public virtual int OverrideIncludeZoneBlocks { get { return _overrideIncludeZoneBlock; } set { _overrideIncludeZoneBlock = value; } }
        public decimal? PendingRateValue
        {
            get
            {
                if (PendingRate != null && PendingRate.Value.HasValue)
                {
                    var factor = (decimal)(this.Curreny.LastRate / PendingRate.PriceListBase.Currency.LastRate);
                    return PendingRate.Value.Value * factor;
                }
                return null;
            }
        }

        public virtual string changeIndicator
        {
            get
            {
                return this.Change.ToString();
            }
        }

        public virtual string ZoneName { get { return Zone.Name; } }
        public virtual bool Checked { get; set; }
        public virtual short OriginalServicesFlag { get; protected set; }
        public virtual DateTime? OriginalBeginEffectiveDate { get; protected set; }
        public virtual DateTime? OriginalEndEffectiveDate { get; protected set; }
        public virtual TABS.Currency Curreny { get; set; }

        //public virtual TABS.Components.RoutePool RoutePool { get; set; }

        public TABS.Rate OldRateBase { get; set; }

        protected List<TABS.DTO.DTO_SupplyRate> _ValidSupplyRates;
        protected List<TABS.DTO.DTO_SupplyRate> _AllSupplyRates;

        protected int? _AffectingOverridesCount;

        public List<TABS.DTO.DTO_SupplyRate> AllSupplyRates
        {
            get { return _AllSupplyRates; }
            set { _AllSupplyRates = null; _AllSupplyRates = value;
            if (ValidSupplyRates == null)
                ValidSupplyRates = new List<DTO_SupplyRate>();
                //ResetValidSupplyRates();
            }
        }
        
        //  public static List<DTO.DTO_SupplyRate> GetRatesAfterZoneBlock(
        //List<DTO.DTO_SupplyRate> supplyRates
        //   , TABS.CarrierAccount customer)
        //  {
        //      return supplyRates;
        //      var effectiveBlocks = TABS.RouteBlock.SupplierZoneBlocks
        //          .Where(r =>
        //              r.Customer == null
        //              || r.Customer.Equals(customer));


        //      List<DTO.DTO_SupplyRate> tobeExcluded = new List<DTO.DTO_SupplyRate>();
        //      // group by supplier and zone
        //      var groupedRates = supplyRates.GroupBy(r => new { r.Zone.CodeGroup, r.Supplier });

        //      foreach (var group in groupedRates)
        //      {
        //          if (group.Any(g => effectiveBlocks.Any(r => r.Supplier.Equals(g.Supplier) && r.Zone.Equals(g.Zone))))
        //              tobeExcluded.AddRange(group);
        //      }


        //      return supplyRates.Except(tobeExcluded).ToList();
        //  }

        public List<TABS.DTO.DTO_SupplyRate> ValidSupplyRates
        {
            get { return _ValidSupplyRates; }
           // protected set
             set {
                _ValidSupplyRates = null;
                _ValidSupplyRates = value;
            }
        }

        public int AffectingOverridesCount
        {
            get
            {
                if (_AffectingOverridesCount == null)
                {
                    _AffectingOverridesCount = 0;
                    List<RouteOverride> customerOverrides = null;
                    if (TABS.RouteOverride.All.TryGetValue(this.Customer, out customerOverrides))
                        _AffectingOverridesCount = TABS.RouteOverride.All[this.Customer].Where(o => o.ZoneName != null && o.ZoneName.Equals(this.ZoneName)).Count();
                }
                return _AffectingOverridesCount.Value;
            }
        }

        public bool HasIncreased { get { return ((decimal)this.NewRate > this.Value || this.NewOffPeakRate > this.OffPeakRate || this.NewWeekEndRate > this.WeekendRate); } }
        public bool HasTOD { get; set; }
        private bool _ISFutureRatePlan=false;
        public bool ISFutureRatePlan { get { return _ISFutureRatePlan; } set { _ISFutureRatePlan = value; } }
        public void ReloadSupplyRates()
        {
            //    var available = TABS.DataHelper.GetPlaningRatesWithSupply(this.Customer
            //        , this.Curreny
            //        , false
            //        , 0
            //        , DateTime.Today
            //        , DateTime.Today
            //        , this.Policy
            //        , this.
            //        );

            //    //Dictionary<int, DTO_ZoneRate> available = TABS.DataHelper.GetPlaningRatesWithSupply
            //    //   (this.Customer.CarrierAccountID, this.Curreny, this.ZoneID, 1000, this.ServicesFlag, false, 0, DateTime.Today, DateTime.Today, Policy);
            //    if (available.Count > 0 && available.ContainsKey(this.ZoneID))
            //        AllSupplyRates = available[this.ZoneID].SupplierRates;
            //    else
            //        AllSupplyRates = new List<DTO_SupplyRate>();
        }
        private short _ServicesFlagSelection = 0;
        public bool IncludeLossesExactMatch = false;
        public static double GetCurrencyExchangeFactor(Currency from, Currency to)
        {
            if (from.Equals(to)) return 1;
            else return to.LastRate / from.LastRate;
        }
        public List<string> Warnings { get; set; }
        //private bool InsureServices(Dictionary<TABS.Zone, TABS.Rate> rates, TABS.Zone zone, short serviceFlag)
        //{

        //    TABS.Rate saleRate;
        //    short minServiceFlag = FlaggedServicesSelection.ServicesFlag;
        //    //minServiceFlag = (minServiceFlag != 0) ? serviceFlag : minServiceFlag;
        //    if (rates.TryGetValue(zone, out saleRate))
        //    // minServiceFlag |= saleRate.ServicesFlag;
        //    {
        //        minServiceFlag = (minServiceFlag != 0) ? minServiceFlag : saleRate.ServicesFlag;
        //    }

        //    return (serviceFlag & minServiceFlag) == minServiceFlag;
        //}
        public void ResetValidSupplyRates()
        {
            var OriginalRates = AllSupplyRates;

            // filter Market Price Rates  (to be updated )
            OriginalRates =
                (this.Policy == SupplierRatePolicy.Highest_Rate)
                ? TABS.DataHelper.GetMarketPriceRates(this.Zone, this.ServicesFlag, this.Curreny, AllSupplyRates).ToList() : AllSupplyRates;


            //if (this.Policy == SupplierRatePolicy.Highest_Rate)
            //    TABS.DataHelper.GetMarketPriceRates(this.Zone, this.ServicesFlag, this.Curreny, AllSupplyRates, out OriginalRates);

            //var supplierZoneBlocks = TABS.RouteBlock.SupplierZoneBlocksList(ISFutureRatePlan).Where(s => s.Customer == null || s.Customer.Equals(this.Customer)).ToArray();
            var supplierZoneBlocks = _SupplierZoneBlocks;
            HashSet<DTO_SupplyRate> rates = new HashSet<DTO_SupplyRate>();

            foreach (var item in OriginalRates)
                rates.Add(item);
            //HashSet<DTO_SupplyRate> _Rates = new HashSet<DTO_SupplyRate>(rates.Where(
            //  r => ((r.ServicesFlag & this.ServicesFlag) == this.ServicesFlag)));

            var FavorateRates = (this.Policy == SupplierRatePolicy.Highest_Rate) ?
               TABS.DataHelper.GetFavorateRates(this.Customer
                , supplierZoneBlocks.ToArray()
                , this.Zone
                , rates
                , this.RoutePoolDate == DateTime.Today, this.Warnings)
                : rates.ToList();
            if (FavorateRates == null) FavorateRates = rates.ToList();
            //var FavorateRates = rates.ToList();
            //if (this.Policy == SupplierRatePolicy.Highest_Rate)
            //    TABS.DataHelper.GetFavorateRates(this.Customer
            //     , supplierZoneBlocks
            //     , this.Zone
            //     , rates.ToList()
            //     , this.RoutePoolDate == DateTime.Today, this.Warnings, out FavorateRates);


            // service comes first
            //short minServiceFlag = this.Zone.FlaggedServices.ToList().OrderBy(S=>S.FlaggedServiceID).Last().FlaggedServiceID;// _ServicesFlagSelection;
            short minServiceFlag = this.ServicesFlag;//(minServiceFlag != 0) ? minServiceFlag : 
            var all = FavorateRates.Where(
              r => ((r.ServicesFlag & minServiceFlag) == minServiceFlag)
                // && ((decimal?)r.Normal < ((decimal?)this.NewRate == 0 ? this.Value : (decimal?)this.NewRate))

                  )
                  .ToList();
            FavorateRates = null;
            if (_overrideIncludeZoneBlock < 0)
            {
                if (!TABS.SystemParameter.Include_Blocked_Zones_In_ZoneRates.BooleanValue.Value)
                    all = all.Where(r => !r.IsBlockAffected).ToList();
            }
            else
            {
                if (_overrideIncludeZoneBlock!=1)//true ==1
                    all = all.Where(r => !r.IsBlockAffected).ToList();
            }
         

            //var validRates = new List<DTO_SupplyRate>();
            ValidSupplyRates = new List<DTO_SupplyRate>(); 
            switch (this.Policy)
            {
                case SupplierRatePolicy.None:
                    ValidSupplyRates = all;
                    break;
                case SupplierRatePolicy.Highest_Rate:
                    foreach (var item in all.GroupBy(r => r.Supplier))
                    {
                        var max = item.Max(i => i.Normal.Value);
                        ValidSupplyRates.Add(item.FirstOrDefault(i => i.Normal.Value.Equals(max)));
                    }

                    break;
                case SupplierRatePolicy.Lowest_Rate:
                    foreach (var item in all.GroupBy(r => r.Supplier))
                    {
                        var min = item.Min(i => i.Normal.Value);
                        ValidSupplyRates.Add(item.FirstOrDefault(i => i.Normal.Value.Equals(min)));
                    }
                    break;
                default:
                    break;
            }
            all = null;
            rates = null;
            ValidSupplyRates.OrderBy(r => r.Normal.Value).ToList();
        }
    
        private bool _IsGroupSelected;
        private int _RateCount;
        private double _NewRate;

        private decimal? _NewOffPeakRate;
        private decimal? _NewWeekEndRate;

        public SupplierRatePolicy Policy { get; set; }

        private int? _ZoneStatsDays;
        private DateTime? _StatsFrom;
        private DateTime? _StatsTo;
        public int? ZoneStatsDays
        {
            get { return _ZoneStatsDays; }
            set { _ZoneStatsDays = value; }
        }
        public DateTime? StatsFrom
        {
            get { return _StatsFrom; }
            set { _StatsFrom = value; }
        }
        public DateTime? StatsTill
        {
            get { return _StatsTo; }
            set { _StatsTo = value; }
        }
        public bool IsGroupSelected
        {
            get { return _IsGroupSelected; }
            set { _IsGroupSelected = value; }
        }

        public decimal? NewOffPeakRate
        {
            get { return _NewOffPeakRate; }
            set { _NewOffPeakRate = value; }
        }

        public decimal? NewWeekEndRate
        {
            get { return _NewWeekEndRate; }
            set { _NewWeekEndRate = value; }
        }

        public int RateCount
        {
            get { return _RateCount; }
            set { _RateCount = value; }
        }

        public decimal ChangeValue
        {
            get
            {
                if (NewRate == 0 || Value.Value == 0)
                    return 0;
                else
                    return (decimal)NewRate - Value.Value;
            }
        }

        public Change RateChange
        {
            get
            {
                if (NewRate > 0 && Value.HasValue && Value.Value == 0)
                    return Change.New;

                if (ChangeValue > 0)
                    return Change.Increase;
                else if (ChangeValue < 0)
                    return Change.Decrease;
                else
                    return Change.None;
            }
        }

        public double NewRate
        {
            get
            {
                return _NewRate;
            }
            set { _NewRate = value; }
        }

        public double Margin
        {
            get
            {
                double toEval = 0;
                if (EvaluatedValue != 0) toEval = EvaluatedValue; else toEval = (double)Value;

                if (ValidSupplyRates.Count > 0 && ValidSupplyRates[0].Normal != null)
                    return (double)toEval - (double)ValidSupplyRates[0].Normal;
                else
                    return 0;
            }
        }

        public double PercMargin
        {

            get
            {

                List<TABS.DTO.DTO_SupplyRate> supplyRates = ValidSupplyRates;
                if (supplyRates.Count > 0 && supplyRates[0].Normal != null)
                    return (Margin / (double)supplyRates[0].Normal) * 100;
                else
                    return 0;
            }
            set { }
        }

        public double PercMarginfilterValue
        {
            get { return Math.Round(PercMargin, 2); }
        }

        public double PercMarginLCRFilterValue
        {
            get { return Math.Round(PercLCR1WithNewRate, 2); }
        }

        public double? AVG
        {
            get
            {
                double? sum = 0;
                int i = 0;
                if (ValidSupplyRates != null)
                {
                    foreach (DTO.DTO_SupplyRate supplyrate in ValidSupplyRates)
                        if (i <= RateCount - 1)
                        {
                            sum += supplyrate.Normal;
                            i++;
                        }
                }
                if (i == 0) return null;
                else
                    return sum / i;
            }
        }

        public double? WeightedAVG
        {
            get
            {
                double? rateWeightSum = 0;
                double? weightSum = 0;
                //int i = 0;
                foreach (DTO.DTO_SupplyRate supplyrate in ValidSupplyRates.Where(v => v.Duration.HasValue && v.Duration.Value > 0))
                //if (i < RateCount)
                {
                    //var duration = supplyrate.Duration < 1 ? 1.0 : supplyrate.Duration;
                    var duration = supplyrate.Duration;
                    rateWeightSum += (supplyrate.Normal * duration);
                    weightSum += duration;
                    //i++;
                }
                //Modified By Sari 2013-09-17 , WAV should be same as AVG if there is no traffic stats
                if (!weightSum.HasValue || weightSum.Value == 0) return AVG;
                else return rateWeightSum / weightSum;
            }
        }

        public double PercAVG
        {
            get { return AVG == null ? 0 : AVG.Value; }
        }

        public double PercLCR1
        {
            get
            {
                if (ValidSupplyRates.Count > 0)
                    return ValidSupplyRates[0].Normal.Value;
                else
                    return 0;
            }
        }

        public double PercLCR1WithNewRate
        {
            get
            {
                double result = 0;
                if (ValidSupplyRates.Count > 0)
                    result = DiffLCR1 != 0 ? (DiffLCR1 / ValidSupplyRates[0].Normal.Value) * 100 : 0;
                return result;
            }
        }


        public double DiffAVG
        {
            get
            {
                double toEval = 0;
                if (EvaluatedValue != 0) toEval = EvaluatedValue; else toEval = NewRate;

                if (toEval == 0) return 0;
                else return toEval - (AVG.HasValue ? AVG.Value : 0);
            }
        }

        public double DiffLCR1
        {
            get
            {
                double toEval = 0;
                if (EvaluatedValue != 0) toEval = EvaluatedValue; else toEval = NewRate;

                if (toEval != 0)
                {
                    if (ValidSupplyRates.Count > 0 && ValidSupplyRates[0].Normal != null)
                        return toEval - (double)ValidSupplyRates[0].Normal;
                    else
                        return 0;
                }
                else
                    return 0;
            }
        }

        public double DiffPrevRate
        {
            get
            {
                double toEval = 0;
                if (EvaluatedValue != 0) toEval = EvaluatedValue; else toEval = NewRate;

                if (toEval != 0 && Value.HasValue)
                    return toEval - (double)Value.Value;
                else
                    return 0;
            }
        }
        private TABS.MarginStatus _Valid = TABS.MarginStatus.InValid;
        private int _MarginStatus = 0;
        public int SetMarginStatus
        {
           set{ _MarginStatus=value;}
        }
        public TABS.MarginStatus Valid { get { return _Valid; } set { _Valid = value; } }
        public MarginStatus MarginStatus
        {
            
            get{
                return _Valid;
                if (Enum.IsDefined(typeof(MarginStatus), _MarginStatus))
                    return (MarginStatus)Enum.Parse(typeof(MarginStatus), _MarginStatus.ToString());
                else
                    return MarginStatus.None;
            }
            
            
        }
        public double EvaluatedValue { get; set; }
        public DateTime RoutePoolDate { get; set; }
        public DTO_EditablePlanningRate(TABS.PlaningRate copy, TABS.Currency currency, List<TABS.Rate> zoneRates, DateTime routePoolDate)
        {
            Warnings = new List<string>();
            this.RoutePoolDate = routePoolDate;
            this.ID = copy.ID;
            this.ServicesFlag = copy.ServicesFlag;
            this.BeginEffectiveDate = copy.BeginEffectiveDate;
            this.EndEffectiveDate = copy.EndEffectiveDate;
            this.OriginalBeginEffectiveDate = copy.BeginEffectiveDate;
            this.OriginalEndEffectiveDate = copy.EndEffectiveDate;
            this.Curreny = currency;
            this.Notes = copy.Notes;
            this.RatePlan = copy.RatePlan;

            this.ServicesFlag = copy.ServicesFlag;
            this.OriginalServicesFlag = copy.ServicesFlag;
            this.IsGroupSelected = false;
            this.Zone = copy.Zone;
            this.NewRate = copy.Value.HasValue ? (double)copy.Value.Value : 0;
            this.NewOffPeakRate = copy.OffPeakRate;
            this.NewWeekEndRate = copy.WeekendRate;


            var factor = 1m;

            if (zoneRates.Count > 0)
            {
                var sortedRates = zoneRates.OrderBy(r => r.BeginEffectiveDate);
                var effRate = routePoolDate.Date > DateTime.Today ? sortedRates.Last() : sortedRates.First();

                factor = (decimal)(this.Curreny.LastRate / effRate.PriceListBase.Currency.LastRate);

                this.Value = effRate.Value;
                this.OffPeakRate = effRate.OffPeakRate;
                this.WeekendRate = effRate.WeekendRate;
                if (this.NewRate == 0)
                {
                    this.BeginEffectiveDate = effRate.BeginEffectiveDate;
                    this.EndEffectiveDate = effRate.EndEffectiveDate;
                }
                else
                {
                    double DecreaseDays = double.Parse(TABS.SystemConfiguration.KnownParameters[TABS.KnownSystemParameter.sys_BeginEffectiveDecreaseRateDays].Value.ToString());
                    double IncreaseDays = double.Parse(TABS.SystemConfiguration.KnownParameters[TABS.KnownSystemParameter.sys_BeginEffectiveRateDays].Value.ToString());
                    //if (this.NewRate > double.Parse(effRate.Value.ToString()))
                    //    this.BeginEffectiveDate = routePoolDate.Date;
                    //else
                    //    this.BeginEffectiveDate = routePoolDate.Date.AddDays(-(IncreaseDays-DecreaseDays));
                      
                    
                }

            }
            else
            {
                this.Value = 0;
                this.OffPeakRate = 0;
                this.WeekendRate = 0;
            }

            var newfactor = copy.PriceListBase != null ? (decimal)(this.Curreny.LastRate / copy.PriceListBase.Currency.LastRate) : 1;

            this.Value = decimal.Parse((this.Value.Value * factor).ToString("0.00000"));
            this.OffPeakRate = decimal.Parse(((this.OffPeakRate ?? 0) * factor).ToString("0.00000"));
            this.WeekendRate = decimal.Parse(((this.WeekendRate ?? 0) * factor).ToString("0.00000"));
            this.NewRate = (double)decimal.Parse(((decimal)this.NewRate * newfactor).ToString("0.00000"));
            this.NewOffPeakRate = decimal.Parse(((this.NewOffPeakRate ?? 0) * newfactor).ToString("0.00000"));
            this.NewWeekEndRate = decimal.Parse(((this.NewWeekEndRate ?? 0) * newfactor).ToString("0.00000"));

            // handling rate chnage flag 

            ZoneRates = zoneRates;
        }
        public DTO_EditablePlanningRate(TABS.PlaningRate copy, TABS.Currency currency, List<TABS.Rate> zoneRates, DateTime routePoolDate, DateTime IssueDate, bool IsImportedRatePlan, bool IsFutureRatePaln, short ServicesFlagSelection)
        {
            ISFutureRatePlan = IsFutureRatePaln;
            Warnings = new List<string>();
            this.RoutePoolDate = routePoolDate;
            this.ID = copy.ID;
            this.ServicesFlag = copy.ServicesFlag;
            this.BeginEffectiveDate = copy.BeginEffectiveDate;
            this.EndEffectiveDate = copy.EndEffectiveDate;
            this.OriginalBeginEffectiveDate = copy.BeginEffectiveDate;
            this.OriginalEndEffectiveDate = copy.EndEffectiveDate;
            this.Curreny = currency;
            this.Notes = copy.Notes;
            this.RatePlan = copy.RatePlan;

            this.ServicesFlag = copy.ServicesFlag;
            this.OriginalServicesFlag = copy.ServicesFlag;
            this.IsGroupSelected = false;
            this.Zone = copy.Zone;
            this.NewRate = copy.Value.HasValue ? (double)copy.Value.Value : 0;
            this.NewOffPeakRate = copy.OffPeakRate;
            this.NewWeekEndRate = copy.WeekendRate;
            this._ServicesFlagSelection = ServicesFlagSelection;

            var factor = 1m;

            if (zoneRates.Count > 0)
            {
                var sortedRates = zoneRates.OrderBy(r => r.BeginEffectiveDate);
                var effRate = routePoolDate.Date > IssueDate ? sortedRates.Last() : sortedRates.First();

                factor = (decimal)(this.Curreny.LastRate / effRate.PriceListBase.Currency.LastRate);

                this.Value = effRate.Value;
                //if (!this.HasTOD)
                //{
                    List<TABS.ToDConsideration> tod = DataHelper.GetEffectiveTodConsideration(copy.Customer, this.Zone, IssueDate).ToList();//.OrderBy(t => t.BeginEffectiveDate).Last();
                    if (tod == null || tod.Count==0)
                    {//no tod found
                        this.OffPeakRate = effRate.Value;
                        this.WeekendRate = effRate.Value;
                    }
                    else
                    {
                        //TABS.ToDConsideration lasttod= tod.OrderBy(t => t.BeginEffectiveDate).Last();
                        this.OffPeakRate = 0;
                        this.WeekendRate = 0;
                    }
                //}
                //this.OffPeakRate = effRate.OffPeakRate;
                //this.WeekendRate = effRate.WeekendRate;
                if (this.NewRate == 0)
                {
                    this.BeginEffectiveDate = effRate.BeginEffectiveDate;
                    this.EndEffectiveDate = effRate.EndEffectiveDate;
                }
                else
                {
                    if (IsImportedRatePlan == false)
                    {//not imported rate plan
                        double DecreaseDays = double.Parse(TABS.SystemConfiguration.KnownParameters[TABS.KnownSystemParameter.sys_BeginEffectiveDecreaseRateDays].Value.ToString());
                        double IncreaseDays = double.Parse(TABS.SystemConfiguration.KnownParameters[TABS.KnownSystemParameter.sys_BeginEffectiveRateDays].Value.ToString());
                        if (this.NewRate > double.Parse(effRate.Value.ToString()))
                            this.BeginEffectiveDate = IssueDate.Date.AddDays(IncreaseDays);
                        else
                            this.BeginEffectiveDate = routePoolDate.Date.AddDays(DecreaseDays);
                    }


                }

            }
            else
            {
                this.Value = 0;
                this.OffPeakRate = 0;
                this.WeekendRate = 0;
            }

            var newfactor = copy.PriceListBase != null ? (decimal)(this.Curreny.LastRate / copy.PriceListBase.Currency.LastRate) : 1;

            this.Value = decimal.Parse((this.Value.Value * factor).ToString("0.00000"));
            this.OffPeakRate = decimal.Parse(((this.OffPeakRate ?? 0) * factor).ToString("0.00000"));
            this.WeekendRate = decimal.Parse(((this.WeekendRate ?? 0) * factor).ToString("0.00000"));
            this.NewRate = (double)decimal.Parse(((decimal)this.NewRate * newfactor).ToString("0.00000"));
            this.NewOffPeakRate = decimal.Parse(((this.NewOffPeakRate ?? 0) * newfactor).ToString("0.00000"));
            this.NewWeekEndRate = decimal.Parse(((this.NewWeekEndRate ?? 0) * newfactor).ToString("0.00000"));

            // handling rate chnage flag 

            ZoneRates = zoneRates;
        }
        private List<TABS.RouteBlock> _SupplierZoneBlocks = null;
        private int TopSuppierRates { get; set; }
        public DTO_EditablePlanningRate(TABS.PlaningRate copy, TABS.Currency currency, List<TABS.Rate> zoneRates, DateTime routePoolDate, DateTime IssueDate, bool IsImportedRatePlan, bool IsFutureRatePaln, short ServicesFlagSelection, bool IncludeExactMatchLosses, TABS.RouteBlock[] SupplierZoneBlocks,int topSupplierRates,bool hasTOD)
        {
            ISFutureRatePlan = IsFutureRatePaln;
            Warnings = new List<string>();
            _SupplierZoneBlocks = SupplierZoneBlocks.ToList();
            this.RoutePoolDate = routePoolDate;
            this.ID = copy.ID;
            this.ServicesFlag = copy.ServicesFlag;
            this.BeginEffectiveDate = (copy.BeginEffectiveDate < copy.Zone.BeginEffectiveDate) ?copy.Zone.BeginEffectiveDate: copy.BeginEffectiveDate;//copy.BeginEffectiveDate
            this.EndEffectiveDate = copy.EndEffectiveDate;
            // moved down by Sari {30-07-2013}, Original effective rate should be taken all the time from the previous effective rate information if any exists
            //this.OriginalBeginEffectiveDate = copy.BeginEffectiveDate;
            //this.OriginalEndEffectiveDate = copy.EndEffectiveDate;
            //this.OriginalServicesFlag = copy.ServicesFlag;
            this.Curreny = currency;
            this.Notes = copy.Notes;
            this.RatePlan = copy.RatePlan;

            this.IsGroupSelected = false;
            this.Zone = copy.Zone;
            this.NewRate = copy.Value.HasValue ? (double)copy.Value.Value : 0;
            this.NewOffPeakRate = copy.OffPeakRate;
            this.NewWeekEndRate = copy.WeekendRate;
            this._ServicesFlagSelection = ServicesFlagSelection;
            this.IncludeLossesExactMatch = IncludeExactMatchLosses;
            var factor = 1m;
            
            if (zoneRates.Count > 0)
            {
                var sortedRates = zoneRates.OrderBy(r => r.BeginEffectiveDate);
                // Removed by Sari {29-07-2013} to force selecting our current selleing even when swapping between Current and Future LCR
                //var effRate = routePoolDate.Date > IssueDate ? sortedRates.Last() : sortedRates.First();
                var effRate = sortedRates.First();

                factor = (decimal)(this.Curreny.LastRate / effRate.PriceListBase.Currency.LastRate);

                this.Value = effRate.Value;
                this.OriginalBeginEffectiveDate = (effRate.BeginEffectiveDate < copy.Zone.BeginEffectiveDate) ? copy.Zone.BeginEffectiveDate:effRate.BeginEffectiveDate; //effRate.BeginEffectiveDate;
                this.OriginalEndEffectiveDate = effRate.EndEffectiveDate;
                this.OriginalServicesFlag = effRate.ServicesFlag;
                //if (!this.HasTOD)
                //{
                List<TABS.ToDConsideration> tod = new List<ToDConsideration>();
                //to be loaded when we populate OurRates by the SQL Query 24-7-2013
                // DataHelper.GetEffectiveTodConsideration(copy.Customer, this.Zone, IssueDate).ToList();//.OrderBy(t => t.BeginEffectiveDate).Last();
                //if (tod == null || tod.Count == 0)
                if (hasTOD==false)
                {//no tod found
                    this.OffPeakRate = effRate.Value;
                    this.WeekendRate = effRate.Value;
                }
                else
                {
                    //TABS.ToDConsideration lasttod= tod.OrderBy(t => t.BeginEffectiveDate).Last();
                    this.OffPeakRate = effRate.OffPeakRate;
                    this.WeekendRate = effRate.WeekendRate;
                }
                //}
                //this.OffPeakRate = effRate.OffPeakRate;
                //this.WeekendRate = effRate.WeekendRate;
                if (this.NewRate == 0)
                {
                    this.BeginEffectiveDate = (effRate.BeginEffectiveDate < copy.Zone.BeginEffectiveDate) ? copy.Zone.BeginEffectiveDate:effRate.BeginEffectiveDate ;// effRate.BeginEffectiveDate;
                    this.EndEffectiveDate = effRate.EndEffectiveDate;
                }
                // Removed by Sari {29-07-2013} to solve effective date issue causesd by swapping beteen Current and Future LCR
                //else
                //{
                    //if (IsImportedRatePlan == false)
                    //{
                //not imported rate plan
                //        double DecreaseDays = double.Parse(TABS.SystemConfiguration.KnownParameters[TABS.KnownSystemParameter.sys_BeginEffectiveDecreaseRateDays].Value.ToString());
                //        double IncreaseDays = double.Parse(TABS.SystemConfiguration.KnownParameters[TABS.KnownSystemParameter.sys_BeginEffectiveRateDays].Value.ToString());
                //        if (this.NewRate > double.Parse(effRate.Value.ToString()))
                //            this.BeginEffectiveDate = IssueDate.Date.AddDays(IncreaseDays);
                //        else
                //            this.BeginEffectiveDate = routePoolDate.Date.AddDays(DecreaseDays);
                    //    if (this.NewRate == double.Parse(effRate.Value.ToString()))
                    //    {
                    //        this.BeginEffectiveDate = effRate.BeginEffectiveDate;
                    //        this.EndEffectiveDate = effRate.EndEffectiveDate;
                    //    }
                    //}



                //}

            }
            else
            {
                this.Value = 0;
                this.OffPeakRate = 0;
                this.WeekendRate = 0;

                // Added by Sari {31-07-2013} in case no previous effective rate exists, case of Newly sold zone take the effective date information from the copy
                //bug 2802 if zone start after rate the bed is the zone bed 
                this.OriginalBeginEffectiveDate = (copy.BeginEffectiveDate < copy.Zone.BeginEffectiveDate) ? copy.Zone.BeginEffectiveDate:copy.BeginEffectiveDate ;// copy.BeginEffectiveDate;
                //this.OriginalEndEffectiveDate = copy.EndEffectiveDate;
                this.OriginalServicesFlag = copy.ServicesFlag;
            }

            var newfactor = copy.PriceListBase != null ? (decimal)(this.Curreny.LastRate / copy.PriceListBase.Currency.LastRate) : 1;

            this.Value = decimal.Parse((this.Value.Value * factor).ToString("0.00000"));
            this.OffPeakRate = decimal.Parse(((this.OffPeakRate ?? 0) * factor).ToString("0.00000"));
            this.WeekendRate = decimal.Parse(((this.WeekendRate ?? 0) * factor).ToString("0.00000"));
            this.NewRate = (double)decimal.Parse(((decimal)this.NewRate * newfactor).ToString("0.00000"));
            this.NewOffPeakRate = decimal.Parse(((this.NewOffPeakRate ?? 0) * newfactor).ToString("0.00000"));
            this.NewWeekEndRate = decimal.Parse(((this.NewWeekEndRate ?? 0) * newfactor).ToString("0.00000"));

            this.HasTOD = hasTOD;
            this.Notes = copy.Notes;

            // handling rate chnage flag 

            ZoneRates = zoneRates;
            TopSuppierRates = topSupplierRates;
        }
        public DTO_EditablePlanningRate(TABS.PlaningRate copy, TABS.Currency currency, List<TABS.Rate> zoneRates, DateTime routePoolDate, DateTime IssueDate, bool IsImportedRatePlan, bool IsFutureRatePaln, short ServicesFlagSelection, bool IncludeExactMatchLosses, TABS.RouteBlock[] SupplierZoneBlocks, int topSupplierRates, bool hasTOD,DateTime? StateFrom,DateTime? StateTo)
        {
            ISFutureRatePlan = IsFutureRatePaln;
            Warnings = new List<string>();
            _SupplierZoneBlocks = SupplierZoneBlocks.ToList();
            this.RoutePoolDate = routePoolDate;
            this.ID = copy.ID;
            this.ServicesFlag = copy.ServicesFlag;
            this.BeginEffectiveDate = (copy.BeginEffectiveDate < copy.Zone.BeginEffectiveDate) ? copy.Zone.BeginEffectiveDate : copy.BeginEffectiveDate;//copy.BeginEffectiveDate
            this.EndEffectiveDate = copy.EndEffectiveDate;
            // moved down by Sari {30-07-2013}, Original effective rate should be taken all the time from the previous effective rate information if any exists
            //this.OriginalBeginEffectiveDate = copy.BeginEffectiveDate;
            //this.OriginalEndEffectiveDate = copy.EndEffectiveDate;
            //this.OriginalServicesFlag = copy.ServicesFlag;
            this.Curreny = currency;
            this.Notes = copy.Notes;
            this.RatePlan = copy.RatePlan;

            this.IsGroupSelected = false;
            this.Zone = copy.Zone;
            this.NewRate = copy.Value.HasValue ? (double)copy.Value.Value : 0;
            this.NewOffPeakRate = copy.OffPeakRate;
            this.NewWeekEndRate = copy.WeekendRate;
            this._ServicesFlagSelection = ServicesFlagSelection;
            this.IncludeLossesExactMatch = IncludeExactMatchLosses;
            var factor = 1m;

            if (zoneRates.Count > 0)
            {
                var sortedRates = zoneRates.OrderBy(r => r.BeginEffectiveDate);
                // Removed by Sari {29-07-2013} to force selecting our current selleing even when swapping between Current and Future LCR
                //var effRate = routePoolDate.Date > IssueDate ? sortedRates.Last() : sortedRates.First();
                var effRate = sortedRates.First();

                factor = (decimal)(this.Curreny.LastRate / effRate.PriceListBase.Currency.LastRate);

                this.Value = effRate.Value;
                this.OriginalBeginEffectiveDate = (effRate.BeginEffectiveDate < copy.Zone.BeginEffectiveDate) ? copy.Zone.BeginEffectiveDate : effRate.BeginEffectiveDate; //effRate.BeginEffectiveDate;
                this.OriginalEndEffectiveDate = effRate.EndEffectiveDate;
                this.OriginalServicesFlag = effRate.ServicesFlag;
                //if (!this.HasTOD)
                //{
                List<TABS.ToDConsideration> tod = new List<ToDConsideration>();
                //to be loaded when we populate OurRates by the SQL Query 24-7-2013
                // DataHelper.GetEffectiveTodConsideration(copy.Customer, this.Zone, IssueDate).ToList();//.OrderBy(t => t.BeginEffectiveDate).Last();
                //if (tod == null || tod.Count == 0)
                if (hasTOD == false)
                {//no tod found
                    this.OffPeakRate = effRate.Value;
                    this.WeekendRate = effRate.Value;
                }
                else
                {
                    //TABS.ToDConsideration lasttod= tod.OrderBy(t => t.BeginEffectiveDate).Last();
                    this.OffPeakRate = effRate.OffPeakRate;
                    this.WeekendRate = effRate.WeekendRate;
                }
                //}
                //this.OffPeakRate = effRate.OffPeakRate;
                //this.WeekendRate = effRate.WeekendRate;
                if (this.NewRate == 0)
                {
                    this.BeginEffectiveDate = (effRate.BeginEffectiveDate < copy.Zone.BeginEffectiveDate) ? copy.Zone.BeginEffectiveDate : effRate.BeginEffectiveDate;// effRate.BeginEffectiveDate;
                    this.EndEffectiveDate = effRate.EndEffectiveDate;
                }
                // Removed by Sari {29-07-2013} to solve effective date issue causesd by swapping beteen Current and Future LCR
                //else
                //{
                //if (IsImportedRatePlan == false)
                //{
                //not imported rate plan
                //        double DecreaseDays = double.Parse(TABS.SystemConfiguration.KnownParameters[TABS.KnownSystemParameter.sys_BeginEffectiveDecreaseRateDays].Value.ToString());
                //        double IncreaseDays = double.Parse(TABS.SystemConfiguration.KnownParameters[TABS.KnownSystemParameter.sys_BeginEffectiveRateDays].Value.ToString());
                //        if (this.NewRate > double.Parse(effRate.Value.ToString()))
                //            this.BeginEffectiveDate = IssueDate.Date.AddDays(IncreaseDays);
                //        else
                //            this.BeginEffectiveDate = routePoolDate.Date.AddDays(DecreaseDays);
                //    if (this.NewRate == double.Parse(effRate.Value.ToString()))
                //    {
                //        this.BeginEffectiveDate = effRate.BeginEffectiveDate;
                //        this.EndEffectiveDate = effRate.EndEffectiveDate;
                //    }
                //}



                //}

            }
            else
            {
                this.Value = 0;
                this.OffPeakRate = 0;
                this.WeekendRate = 0;

                // Added by Sari {31-07-2013} in case no previous effective rate exists, case of Newly sold zone take the effective date information from the copy
                //bug 2802 if zone start after rate the bed is the zone bed 
                this.OriginalBeginEffectiveDate = (copy.BeginEffectiveDate < copy.Zone.BeginEffectiveDate) ? copy.Zone.BeginEffectiveDate : copy.BeginEffectiveDate;// copy.BeginEffectiveDate;
                //this.OriginalEndEffectiveDate = copy.EndEffectiveDate;
                this.OriginalServicesFlag = copy.ServicesFlag;
            }

            var newfactor = copy.PriceListBase != null ? (decimal)(this.Curreny.LastRate / copy.PriceListBase.Currency.LastRate) : 1;

            this.Value = decimal.Parse((this.Value.Value * factor).ToString("0.00000"));
            this.OffPeakRate = decimal.Parse(((this.OffPeakRate ?? 0) * factor).ToString("0.00000"));
            this.WeekendRate = decimal.Parse(((this.WeekendRate ?? 0) * factor).ToString("0.00000"));
            this.NewRate = (double)decimal.Parse(((decimal)this.NewRate * newfactor).ToString("0.00000"));
            this.NewOffPeakRate = decimal.Parse(((this.NewOffPeakRate ?? 0) * newfactor).ToString("0.00000"));
            this.NewWeekEndRate = decimal.Parse(((this.NewWeekEndRate ?? 0) * newfactor).ToString("0.00000"));

            this.HasTOD = hasTOD;
            this.Notes = copy.Notes;
            this.StatsFrom = StateFrom;
            this.StatsTill = StateTo;
            // handling rate chnage flag 

            ZoneRates = zoneRates;
            TopSuppierRates = topSupplierRates;
        }

        private TABS.Components.RoutePool GetRoutePool(bool IsCurrent, bool IncludeLosses)
        {
            TABS.Components.RoutePool ratePool = null;
            bool isexactmatch = (bool)TABS.SystemConfiguration.KnownParameters[TABS.KnownSystemParameter.sys_ExactMatchProcessing].Value;
            if (isexactmatch == false)
            {
                if (IsCurrent == true)
                    ratePool = TABS.Components.RoutePool.Current;
                else
                    ratePool = TABS.Components.RoutePool.Future;
            }
            else
            {

                if (IsCurrent == true)
                    ratePool = TABS.Components.RoutePool.CurrentPool(Customer, IncludeLosses);
                if (IsCurrent == false)
                    ratePool = TABS.Components.RoutePool.FuturePool(Customer, IncludeLosses);
            }



            return ratePool;
        }
        public void GetAllSupplyRatesbyZones()
        {
            HashSet<TABS.Rate> supplyRates = null;

            //var ratePool = (this.ISFutureRatePlan == false) ? TABS.Components.RoutePool.Current : TABS.Components.RoutePool.Future;
           var ratePool = GetRoutePool(this.ISFutureRatePlan == false, IncludeLossesExactMatch);
            var supplierZoneBlocks = TABS.RouteBlock.SupplierZoneBlocksList(ratePool.BaseDate == DateTime.Now).Where(s => s.Customer == null || s.Customer.Equals(this.Customer)).ToArray();

            var blokcsByZoneSupplier = supplierZoneBlocks.Distinct(new TABS.DataHelper.ISupplierBlockComparer()).ToDictionary(s => string.Concat(s.Zone.ZoneID, s.Supplier.CarrierAccountID));
            if (ratePool.SupplyRatesBySaleZone.TryGetValue(this.Zone, out supplyRates))
            {

                List<TABS.Rate> sortedRates = new List<TABS.Rate>();
                List<TABS.Rate> items = new List<TABS.Rate>();
                short minServiceFlag = this.ServicesFlag;// ServicesFlag;
                List<TABS.Rate> all = null;
                switch (this.Policy)
                {
                    case TABS.SupplierRatePolicy.None:
                        all = supplyRates.Where(r => !r.Supplier.CarrierProfile.Equals(this.Customer.CarrierProfile) && ((r.ServicesFlag & minServiceFlag) == minServiceFlag)).ToList();
                        if (this._overrideIncludeZoneBlock == 0)
                            all = all.Where(r => !blokcsByZoneSupplier.Keys.Contains(string.Concat(r.Zone.ZoneID, r.Supplier.CarrierAccountID))).ToList();
                        items = all.OrderBy(r => r.Value.Value * decimal.Parse(TABS.DataHelper.GetCurrencyExchangeFactor(r.PriceList.Currency, this.Curreny).ToString())).Take(this.TopSuppierRates).ToList();
                        sortedRates.AddRange(items);

                        break;
                    case TABS.SupplierRatePolicy.Highest_Rate:

                        TABS.DataHelper.GetMarketPriceRates(this.Zone, this.ServicesFlag, this.Curreny, supplyRates.Where(i => !i.Supplier.CarrierProfile.Equals(this.Customer.CarrierProfile)).ToList(), out all);
                        all = TABS.DataHelper.GetFavorateRatesByZones(supplierZoneBlocks, this.Zone, all);
                        all = all.Where(
                        r => ((r.ServicesFlag & minServiceFlag) == minServiceFlag)).ToList();
                        if (this._overrideIncludeZoneBlock == 0)
                            all = all.Where(r => !blokcsByZoneSupplier.Keys.Contains(string.Concat(r.Zone.ZoneID, r.Supplier.CarrierAccountID))).ToList();
                        foreach (var group in all.GroupBy(s => s.SupplierID))
                        {

                            var Max = group.Max(i => i.Value.Value);
                            items.Add(group.FirstOrDefault(i => i.Value.Value.Equals(Max)));
                        }
                        items = items.OrderBy(r => r.Value.Value * decimal.Parse(TABS.DataHelper.GetCurrencyExchangeFactor(r.PriceList.Currency, this.Curreny).ToString())).Take(this.TopSuppierRates).ToList();
                        sortedRates.AddRange(items);

                        break;
                    case TABS.SupplierRatePolicy.Lowest_Rate:
                        all = supplyRates.Where(
                        r => !r.Supplier.CarrierProfile.Equals(this.Customer.CarrierProfile) && ((r.ServicesFlag & minServiceFlag) == minServiceFlag)).ToList();
                        if (this._overrideIncludeZoneBlock == 0)
                            all = all.Where(r => !blokcsByZoneSupplier.Keys.Contains(string.Concat(r.Zone.ZoneID, r.Supplier.CarrierAccountID))).ToList();
                        foreach (var group in all.GroupBy(s => s.SupplierID))
                        {
                            var min = group.Min(i => i.Value.Value);
                            items.Add(group.FirstOrDefault(i => i.Value.Value.Equals(min)));
                        }
                        items = items.OrderBy(r => r.Value.Value * decimal.Parse(TABS.DataHelper.GetCurrencyExchangeFactor(r.PriceList.Currency, this.Curreny).ToString())).Take(this.TopSuppierRates).ToList();
                        sortedRates.AddRange(items);
                        break;
                }
                items = null;

                List<TABS.Rate> tobeExcluded = new List<TABS.Rate>();
                this.AllSupplyRates.Clear();
                this.ValidSupplyRates.Clear();
                foreach (TABS.Rate supplierRate in sortedRates)//FavorateRates
                {



                    var currencyExchangeFactor = TABS.DataHelper.GetCurrencyExchangeFactor(supplierRate.PriceList.Currency, this.Curreny);

                    TABS.DTO.DTO_SupplyRate supplyRate = new TABS.DTO.DTO_SupplyRate();
                    supplyRate.Supplier = supplierRate.Supplier;
                    supplyRate.Rate = supplierRate;
                    supplyRate.Zone = supplierRate.Zone;
                    var key = string.Concat(supplierRate.ZoneID, supplierRate.SupplierID);
                    supplyRate.IsBlockAffected = blokcsByZoneSupplier.ContainsKey(key);

                    supplyRate.Normal = (double)supplierRate.Value * currencyExchangeFactor;
                    supplyRate.OffPeak = (double?)supplierRate.OffPeakRate * currencyExchangeFactor;
                    supplyRate.Weekend = (double?)supplierRate.WeekendRate * currencyExchangeFactor;
                    supplyRate.ServicesFlag = supplierRate.ServicesFlag;
                    supplierRate.BeginEffectiveDate = supplierRate.BeginEffectiveDate;
                    supplyRate.EndEffectiveDate = supplierRate.EndEffectiveDate;
                    supplyRate.ACD = 0;
                    supplyRate.ASR = 0;
                    supplyRate.Duration = 0;

                    // check if supplier already has another zone/rate
                    TABS.DTO.DTO_SupplyRate otherFound = null;
                    int indexFound = 0;

                    this.AllSupplyRates.Add(supplyRate);



                }
               // TABS.DataHelper.UpdateZoneRatesSupplierStatsForZone(IsStatsPeriodDefined, Days, ref StatsFrom, ref StatsTill, rates);
                TABS.DataHelper.UpdateZoneRatesSupplierStatsForZone(true,this.ZoneStatsDays, this.StatsFrom.Value, this.StatsTill.Value,this.Zone.ZoneID, this.AllSupplyRates);
                this.ValidSupplyRates = this.AllSupplyRates;
            }
        }

        protected Change _change;
        //public override Change Change
        //{
        //    get
        //    {
        //        var rateToCompare = this.EvaluatedValue != 0 ? this.EvaluatedValue : this.NewRate;

        //        var previuosRate = ZoneRates
        //            .Where(r => r.IsEffectiveOn(this.BeginEffectiveDate.Value) && r.BeginEffectiveDate != this.BeginEffectiveDate)
        //            .OrderBy(r => r.BeginEffectiveDate).LastOrDefault();


        //        if (previuosRate != null && rateToCompare > 0)
        //        {
        //            var factor = previuosRate.PriceListBase != null ? (decimal)(this.Curreny.LastRate / previuosRate.PriceListBase.Currency.LastRate) : 1;

        //            _change = previuosRate == null
        //                        ? TABS.Change.New
        //                       : (rateToCompare == (double)previuosRate.Value.Value * (double)factor
        //                           ? TABS.Change.None
        //                           : (rateToCompare < (double)previuosRate.Value.Value * (double)factor ? TABS.Change.Decrease : TABS.Change.Increase));
        //        }

        //        return _change;
        //    }
        //    set
        //    {
        //        _change = value;
        //    }
        //}

        public override Change Change
        {
            get
            {
                var rateToCompare = this.EvaluatedValue != 0 ? this.EvaluatedValue : this.NewRate;

                var previuosRate = ZoneRates
                    .Where(r => r.IsEffectiveOn(this.BeginEffectiveDate.Value) && r.BeginEffectiveDate != this.BeginEffectiveDate)
                    .OrderBy(r => r.BeginEffectiveDate).LastOrDefault();


                if (previuosRate != null && rateToCompare > 0)
                {
                    var factor = previuosRate.PriceListBase != null ? (double)(this.Curreny.LastRate / previuosRate.PriceListBase.Currency.LastRate) : 1;
                    _change = previuosRate == null
                                ? TABS.Change.New
                               : (rateToCompare == (double)previuosRate.Value.Value * factor
                                   ? TABS.Change.None
                                   : (rateToCompare < (double)previuosRate.Value.Value * factor ? TABS.Change.Decrease : TABS.Change.Increase));
                }
                if (this.HasTOD && _change == TABS.Change.None)
                {
                    var rateOffPeakToCompare = this.EvaluatedValue != 0 ? this.EvaluatedValue : (double)this.NewOffPeakRate;
                    if (previuosRate != null && rateOffPeakToCompare > 0)
                    {
                        decimal prOffPeakRate = 0;
                        prOffPeakRate = (previuosRate.OffPeakRate != null ? previuosRate.OffPeakRate.Value : 0);
                        var factor = previuosRate.PriceListBase != null ? (decimal)(this.Curreny.LastRate / previuosRate.PriceListBase.Currency.LastRate) : 1;

                        _change = previuosRate == null
                                    ? TABS.Change.New
                                    : (rateOffPeakToCompare == (double)(prOffPeakRate * factor)
                                       ? TABS.Change.None
                                       : (rateOffPeakToCompare < (double)(prOffPeakRate * factor) ? TABS.Change.Decrease : TABS.Change.Increase));

                    }
                    if (_change == TABS.Change.None)
                    {
                        var rateWeekEndToCompare = this.EvaluatedValue != 0 ? this.EvaluatedValue : (double)this.NewWeekEndRate;

                        if (previuosRate != null && rateWeekEndToCompare > 0)
                        {
                            decimal prWeekEndPeakRate = 0;
                            prWeekEndPeakRate = (previuosRate.WeekendRate != null ? previuosRate.WeekendRate.Value : 0);
                            var factor = previuosRate.PriceListBase != null ? (decimal)(this.Curreny.LastRate / previuosRate.PriceListBase.Currency.LastRate) : 1;

                            _change = previuosRate == null
                                        ? TABS.Change.New
                                       : (rateWeekEndToCompare == (double)(prWeekEndPeakRate * factor)
                                           ? TABS.Change.None
                                           : (rateWeekEndToCompare < (double)(prWeekEndPeakRate * factor) ? TABS.Change.Decrease : TABS.Change.Increase));

                        }
                    }
                }
                return _change;
            }
            set
            {
                _change = value;
            }
        }

        public DTO_EditablePlanningRate()
        {
            this.AllSupplyRates = new List<DTO_SupplyRate>();
            this.ValidSupplyRates = new List<DTO_SupplyRate>();
        }
        public void Dispose()
        {
            this._AllSupplyRates = null;
            this._ValidSupplyRates = null;
            this.Warnings = null;
            this.ZoneRates = null;
            this.PendingRate = null;
            _SupplierZoneBlocks = null;
        }
        ~DTO_EditablePlanningRate()
        {
            this._AllSupplyRates = null;
            this._ValidSupplyRates = null;
            this.Warnings = null;
            this.ZoneRates = null;
            this.PendingRate = null;
            this.EffectiveCodes = null;
            _SupplierZoneBlocks = null;
        }
        #region IComparable<DTO_EditablePlanningRate> Members

        public int CompareTo(DTO_EditablePlanningRate other)
        {
            return this.ZoneName.CompareTo(other.ZoneName);
        }

        #endregion
    }
}
