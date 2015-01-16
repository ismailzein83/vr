using System;
using System.Collections.Generic;
using System.Linq;
using TABS.Addons.Utilities.ExtensionMethods;
using System.Data;
using System.Reflection;
namespace TABS.Components
{
    /// <summary>
    /// Provides a Centralized Pool for current and pending Sale-Purchase Mapping of Codes and Rates 
    /// </summary>
    public class RoutePool : Interfaces.ICachedCollectionContainer
    {
        public class CodeRoute
        {
            public RoutePool RoutePool { get; protected set; }
            public string Code { get; protected set; }
            public TABS.Code SaleCode { get; protected set; }
            public Dictionary<TABS.Code, TABS.Rate> SupplyRates { get; protected set; }
            public List<TABS.Rate> SupplierRates { get; protected set; }
            public CodeRoute(string code, RoutePool routePool)
            {
                
                this.RoutePool = routePool;
                var codeMap = this.RoutePool.CodeMap;
                var allSupplyRates = RoutePool.SupplyRates;
                this.SaleCode = codeMap.Find(code, TABS.CarrierAccount.SYSTEM, RoutePool.BaseDate);
                TABS.Zone saleZone = this.SaleCode != null ? this.SaleCode.Zone : null;
                this.Code = code;
                this.SupplyRates = new Dictionary<Code, Rate>();
                this.SupplierRates = new List<Rate>();
               

                // Try to find the matching purchase codes and thus the matching purchase zones
                var baseDate = this.RoutePool.BaseDate;
                var SupplyRatesBySaleZone = routePool.SupplyRatesBySaleZone;
                
                foreach (var supplier in this.RoutePool.ValidSuppliers)
                {
                    Code purchaseCode=null;//var purchaseCode
                    purchaseCode = codeMap.Find(code, supplier, baseDate);
                    if (purchaseCode != null)
                    {
                        var purchaseZone = purchaseCode.Zone;
                        TABS.Rate purchaseRate = null;

                        if (allSupplyRates.TryGetValue(purchaseZone, out purchaseRate))
                        {
                            //this.SupplyRates.Add(purchaseCode, purchaseRate);
                            purchaseRate.Zone.IsHaveMatchingCodeGroup = purchaseZone.IsHaveMatchingCodeGroup;
                            purchaseRate.Zone.IsCodeGroup = purchaseZone.IsCodeGroup;
                            this.SupplierRates.Add(purchaseRate);
                        }
                       
                        if (saleZone != null && purchaseRate != null)
                        {
                            HashSet<TABS.Rate> zoneSupplyRates = null;
                            lock (SupplyRatesBySaleZone)
                            {
                                if (!SupplyRatesBySaleZone.TryGetValue(saleZone, out zoneSupplyRates))
                                {
                                    zoneSupplyRates = new HashSet<Rate>();
                                    SupplyRatesBySaleZone[saleZone] = zoneSupplyRates;
                                }
                            }
                            lock (zoneSupplyRates)
                            {
                                int zoneid = purchaseRate.ZoneID;
                                string supplierid = purchaseRate.SupplierID;
                                bool isAlreadyadded = zoneSupplyRates.Any(r => r.SupplierID == supplierid && r.ZoneID == zoneid);
                                if(isAlreadyadded==false)
                                    zoneSupplyRates.Add(purchaseRate);
                            }
                        }
                    }
                }
                this.SupplierRates.OrderBy(r => r.Value).ToList();
            }
            public CodeRoute(string code, RoutePool routePool, CarrierAccount Customer,bool IncludeLossesSubCodes,bool IsCurentPool)
            {

                this.RoutePool = routePool;
                var codeMap = this.RoutePool.CodeMap;
                var allSupplyRates = RoutePool.SupplyRates;
                this.SaleCode = codeMap.Find(code, TABS.CarrierAccount.SYSTEM, RoutePool.BaseDate);
                if (this.SaleCode == null) return;
                TABS.Zone saleZone = this.SaleCode != null ? this.SaleCode.Zone : null;
                this.Code = code;
                this.SupplyRates = new Dictionary<Code, Rate>();
                this.SupplierRates = new List<Rate>();
                //Exact Match system
                bool IsExactCodeMatch = false;
                bool IsHaveOneLossesSubCode = false;
                //Exact Match system end

                // Try to find the matching purchase codes and thus the matching purchase zones
                var baseDate = this.RoutePool.BaseDate;
                var SupplyRatesBySaleZone = routePool.SupplyRatesBySaleZone;
                
                foreach (var supplier in this.RoutePool.ValidSuppliers)//this.RoutePool.ValidSuppliers
                {
                    Code purchaseCode = null;
                    purchaseCode = codeMap.FindUsingExactMatch(code, supplier, Customer, saleZone, baseDate, routePool, IncludeLossesSubCodes, IsCurentPool, out IsExactCodeMatch, out IsHaveOneLossesSubCode);
                   
                    if (purchaseCode != null)
                    {
                        var purchaseZone = purchaseCode.Zone;
                        TABS.Rate purchaseRate = null;
                        if (allSupplyRates.TryGetValue(purchaseZone, out purchaseRate))
                        {
                            //this.SupplyRates.Add(purchaseCode, purchaseRate);
                            this.SupplierRates.Add(purchaseRate);
                        }

                        if (saleZone != null && purchaseRate != null)
                        {
                            HashSet<TABS.Rate> zoneSupplyRates = null;
                            lock (SupplyRatesBySaleZone)
                            {
                                if (!SupplyRatesBySaleZone.TryGetValue(saleZone, out zoneSupplyRates))
                                {
                                    zoneSupplyRates = new HashSet<Rate>();
                                    SupplyRatesBySaleZone[saleZone] = zoneSupplyRates;
                                }
                            }
                            lock (zoneSupplyRates)
                            {
                                int zoneid = purchaseRate.ZoneID;
                                string supplierid = purchaseRate.SupplierID;
                                bool isAlreadyadded = zoneSupplyRates.Any(r => r.SupplierID == supplierid && r.ZoneID == zoneid);
                                if (isAlreadyadded == false)
                                    zoneSupplyRates.Add(purchaseRate);
                            }
                        }
                    }
                }
            }
        
        }
       
        /// <summary>
        /// Needed for marker interface to be called by reflection
        /// </summary>
        public static void ClearCachedCollections()
        {
            _Current = null;
            _Future = null;
            _FutureforCodeComparaison = null;
            _CurrentforCodeComparaison = null;
            _CurrentZoneCodes = null;
            _FutureZoneCodes = null;
            
            _ExactMatchCodeSuupliers = null;
            _FutureExactMatchCodeSuupliers = null;
            _ExactMatchCodeSuupliers =new Dictionary<string,Dictionary<string,List<CarrierAccount>>>();
            _FutureExactMatchCodeSuupliers = new Dictionary<string, Dictionary<string, List<CarrierAccount>>>();
            _ISCurrentState = true;
            _PrevState="";
            TABS.Addons.Utilities.RatePoolChecker.ChangeLastRoutePoolCachclear();
        }
        private static Dictionary<string, Dictionary<string, List<CarrierAccount>>> _ExactMatchCodeSuupliers = new Dictionary<string, Dictionary<string, List<CarrierAccount>>>();
        private static Dictionary<string, Dictionary<string, List<CarrierAccount>>> _FutureExactMatchCodeSuupliers = new Dictionary<string, Dictionary<string, List<CarrierAccount>>>();

        public static Dictionary<string, Dictionary<string, List<CarrierAccount>>> ExactMatchCodeSuupliers 
        { 
            get {
                if (_ExactMatchCodeSuupliers == null)
                    _ExactMatchCodeSuupliers = new Dictionary<string, Dictionary<string, List<CarrierAccount>>>();
                return _ExactMatchCodeSuupliers;
            }
            protected set { _ExactMatchCodeSuupliers = value; } 
        }
        public static Dictionary<string, Dictionary<string, List<CarrierAccount>>> FutureExactMatchCodeSuupliers { 
            get {
                if (_FutureExactMatchCodeSuupliers == null)
                    _FutureExactMatchCodeSuupliers = new Dictionary<string, Dictionary<string, List<CarrierAccount>>>();
                return _FutureExactMatchCodeSuupliers; 
            } 
            protected set { _FutureExactMatchCodeSuupliers = value; }
        }
        internal static RoutePool _CurrentforCodeComparaison;
        internal static RoutePool _Current;
        internal static RoutePool _Future;
        public static Boolean _ISCurrentState = true;
        internal static string _PrevState = (_ISCurrentState == true) ? "Current" : "Future";
        internal static RoutePool _FutureforCodeComparaison;
        //internal static CarrierAccount _PreviousCustomer = null;
        //internal static CarrierAccount _PreviousFutureCustomer = null;

        public static RoutePool CurrentforCodeComparaison
        {
            get
            {
                lock (typeof(RoutePool))
                {
                    if (_CurrentforCodeComparaison == null)
                        _CurrentforCodeComparaison = new RoutePool(DateTime.Today, true);
                }

                return _CurrentforCodeComparaison;
            }
        }
        public static RoutePool Current
        {
            get
            {
                lock (typeof(RoutePool))
                {
                    if (_Current == null)
                    {
                        //TABS.Addons.Utilities.MemoryConsumption.TotalMemoryStart("Memory Size before Create RoutePool");

                        _Current = new RoutePool(DateTime.Today);
                        //TABS.Addons.Utilities.MemoryConsumption.TotalMemoryStart("Memory Size before Create RoutePool");

                    }
                }
                return _Current;
            }
        }
        public static RoutePool CurrentPool(CarrierAccount Customer, bool IncludeLossesSubCodes)
        {
            
                lock (typeof(RoutePool))
                {
                    Dictionary<string, List<CarrierAccount>> CustomerExactMatchCodeSuupliers = null;
                    bool RefreshPool = false;
                    if (IncludeLossesSubCodes == true && Customer == null) return _Current;
                    if (IncludeLossesSubCodes == true && TABS.Components.RoutePool.ExactMatchCodeSuupliers != null && Customer!=null  && TABS.Components.RoutePool.ExactMatchCodeSuupliers.TryGetValue(Customer.CarrierAccountID, out CustomerExactMatchCodeSuupliers) == false)
                        RefreshPool = true;
                    if (_Current == null || RefreshPool == true)
                    {  
                        
                        _PrevState = (_ISCurrentState == true) ? "Current" : "Future";
                        _ISCurrentState = true;
                        _Current = null;
                        GC.Collect();
                        _Current = new RoutePool(DateTime.Today,Customer,IncludeLossesSubCodes);
                      
                       
                    }
                }
                return _Current;
            
        }
        public static Dictionary<string, List<CarrierAccount>> IncludeLossesForCustomer(CarrierAccount Customer)
        {
            Dictionary<string, List<CarrierAccount>> CustomerExactMatchCodeSuupliers = null;
            if (Customer == null) return null;
            if (TABS.Components.RoutePool.ExactMatchCodeSuupliers != null &&  TABS.Components.RoutePool.ExactMatchCodeSuupliers.TryGetValue(Customer.CarrierAccountID, out CustomerExactMatchCodeSuupliers) == true)
                return TABS.Components.RoutePool.ExactMatchCodeSuupliers[Customer.CarrierAccountID];
            else
            {
                RoutePool CustomerRoutePool = null;
                lock (typeof(RoutePool))
                {
                    CustomerRoutePool=new RoutePool(DateTime.Today, Customer, true,true);
                    
                }
                return TABS.Components.RoutePool.ExactMatchCodeSuupliers[Customer.CarrierAccountID];
            }
        }
        public static bool ISCustomerIncludedInLosses(CarrierAccount Customer)
        {
            return  TABS.Components.RoutePool.ExactMatchCodeSuupliers.Keys.Contains(Customer.CarrierAccountID);
        }
        public static RoutePool Future
        {
            get
            {
                lock (typeof(RoutePool))
                {
                    if (_Future == null )
                    {
                        var noticeDays = (double)TABS.SystemConfiguration.KnownParameters[KnownSystemParameter.sys_BeginEffectiveRateDays].NumericValue.Value;
                        DateTime noticeFuture = DateTime.Today.AddDays(noticeDays);
                        _Future = new RoutePool(noticeFuture);
                    }
                }
                return _Future;
            }
        }
        public static RoutePool FuturePool(CarrierAccount Customer, bool IncludeLossesSubCodes)
        {
            
                lock (typeof(RoutePool))
                {
                    Dictionary<string, List<CarrierAccount>> CustomerExactMatchCodeSuupliers = null;
                    bool RefreshPool = false;
                    if (IncludeLossesSubCodes == true && Customer == null) return _Future;
                    if (IncludeLossesSubCodes == true  && TABS.Components.RoutePool.FutureExactMatchCodeSuupliers!=null && TABS.Components.RoutePool.FutureExactMatchCodeSuupliers.TryGetValue(Customer.CarrierAccountID, out CustomerExactMatchCodeSuupliers) == false)
                        RefreshPool = true;
                    if (_Future == null || RefreshPool == true)
                    {
                        _PrevState = (_ISCurrentState == true) ? "Current" : "Future";
                        _ISCurrentState = false;
                        var noticeDays = (double)TABS.SystemConfiguration.KnownParameters[KnownSystemParameter.sys_BeginEffectiveRateDays].NumericValue.Value;
                        DateTime noticeFuture = DateTime.Today.AddDays(noticeDays);
                        
                        _Future = null; GC.Collect();
                        _Future = new RoutePool(noticeFuture,Customer,IncludeLossesSubCodes);
                    }
                }
                return _Future;
            
        }
        public static RoutePool FutureforCodeComparaison
        {
            get
            {
                lock (typeof(RoutePool))
                {
                    //if (_FutureforCodeComparaison == null)
                    //{
                        //var noticeDays = (double)TABS.SystemConfiguration.KnownParameters[KnownSystemParameter.sys_BeginEffectiveRateDays].NumericValue.Value;
                        //   DateTime noticeFuture = DateTime.Today.AddDays(noticeDays);
                        //_FutureforCodeComparaison = new RoutePool(noticeFuture, true);
                    //}
                    if (_Future == null)
                    {
                        var noticeDays = (double)TABS.SystemConfiguration.KnownParameters[KnownSystemParameter.sys_BeginEffectiveRateDays].NumericValue.Value;
                        DateTime noticeFuture = DateTime.Today.AddDays(noticeDays);
                        _Future = new RoutePool(noticeFuture);
                    }
                   
                }
                return _Future;// _FutureforCodeComparaison;
            }
        }

        public DateTime BaseDate { get; protected set; }

        public CodeMap CodeMap
        {
            get;
            protected set;
        }

        //internal Dictionary<string, CodeRoute> _SaleCodeRoutes;
        //public Dictionary<string, CodeRoute> SaleCodeRoutes
        //{
        //    get 
        //    {
        //        return RoutePool.Current.CodeRoutes.Where(c => RoutePool.CurrentZoneCodes .Contains(c.Key)).ToList() ; 
        //    }
        //}

        internal Dictionary<string, CodeRoute> _CodeRoutes;
        public Dictionary<string, CodeRoute> CodeRoutes
        {
            get { return _CodeRoutes; }
            set
            {
                _CodeRoutes = value;
            }

        }

        /// <summary>
        /// Gets Supplied Rates by Supply Zone
        /// </summary>
        public Dictionary<TABS.Zone, TABS.Rate> SupplyRates { get; protected set; }

        /// <summary>
        /// Provides a mapping of Effective and pending Sale Zones to Effective and Pending Supply Rates
        /// </summary>
        public Dictionary<Zone, HashSet<Rate>> SupplyRatesBySaleZone
        {
            get;
            protected set;
        }

        public Dictionary<string, TimeSpan> LoadingTimes { get; protected set; }

        public string[] AllCodes { get; protected set; }
        public TABS.CarrierAccount[] ValidSuppliers { get; protected set; }

        protected bool IncludeSupplierZoneBlock { get { return SystemParameter.Include_Blocked_Zones_In_ZoneRates.BooleanValue.Value; } }
        protected bool IncludeSupplierZoneBlockOnCustomers { get { return true; } }

        protected void LoadSupplyRates()
        {
            LoadSupplyRates(false);
        }

        protected void LoadSupplyRates(bool forCodeComparaison)
        {
            SupplyRates = new Dictionary<Zone, Rate>();

            var stopWatch = new TABS.Addons.Utilities.StopWatch();
            stopWatch.StartTiming();

            var allCarriers = TABS.ObjectAssembler.GetList<TABS.CarrierAccount>().ToDictionary(c => c.CarrierAccountID);
            Dictionary<int, TABS.Zone> zones = new Dictionary<int, TABS.Zone>();
            Dictionary<int, TABS.PriceList> pricelists = new Dictionary<int, TABS.PriceList>();

            List<TABS.Rate> rates = new List<Rate>();
            var sql = string.Format(@"
                {0}
                  
	            SELECT 
    	           P.PriceListID,
                   P.SupplierID,
                   P.CurrencyID,
                   P.BeginEffectiveDate AS PBED,
                   --P.SourceFileName,
                   Z.ZoneID,
                   Z.CodeGroup,
                   Z.Name,
                   Z.ServicesFlag AS ZSF,
                   Z.BeginEffectiveDate AS ZBED,
                   Z.EndEffectiveDate AS ZEED,
                   R.RateID,
                   R.Rate AS peakRate ,
                   R.OffPeakRate AS Offpeackrate,
                   R.WeekendRate AS WeekendRate,
                   R.Change,
                   R.ServicesFlag AS RSF,
                   R.BeginEffectiveDate AS RBED,
                   R.EndEffectiveDate,
                   P.CustomerID
                FROM PriceList P (NOLOCK), CarrierAccount S (NOLOCK), Rate R (NOLOCK), Zone Z (NOLOCK)
                WHERE 
                        R.PriceListID = P.PriceListID 
                    AND P.CustomerID = @P1
                    AND	S.CarrierAccountID = P.SupplierID
                    AND S.RoutingStatus IN (1,3)
                    AND S.ActivationStatus <> 0
                    AND (R.BeginEffectiveDate <= @P2)
                    AND (R.EndEffectiveDate is null or (R.EndEffectiveDate > @P2 And R.BeginEffectiveDate<>R.EndEffectiveDate))
                    AND (Z.BeginEffectiveDate <= @P2)
                    AND (Z.EndEffectiveDate is null or (Z.EndEffectiveDate > @P2 ))
                    AND R.ZoneID = Z.ZoneID
		            {1} 
                ORDER BY R.BeginEffectiveDate 
                ",
                    true || IncludeSupplierZoneBlock
                    ? ""
                    : @"
                        DECLARE @BlockedSupplierZones TABLE(ZoneID int PRIMARY KEY);
	                    INSERT INTO @BlockedSupplierZones(ZoneID)
		                    SELECT DISTINCT RB.ZoneID FROM RouteBlock RB 
                                WHERE 
                                    RB.ZoneID IS NOT NULL 
                                  AND RB.CustomerID IS NULL
                                AND RB.BeginEffectiveDate <= @P3 
                                AND ISNULL(RB.EndEffectiveDate,'2020-01-01') > @P3;
                        "
                ,
                true || IncludeSupplierZoneBlock
                    ? ""
                    : @" AND R.ZoneID NOT IN (SELECT ZoneID FROM @BlockedSupplierZones) "
                );


            IDbConnection connection = TABS.DataHelper.GetOpenConnection();
            IDbCommand command = connection.CreateCommand();
            command.CommandTimeout = 0;
            command.CommandText = sql;
            var now = DateTime.Now;
            TABS.DataHelper.CreateCommandParameters(command, new object[] { TABS.CarrierAccount.SystemAccountID, this.BaseDate, this.BaseDate.AddHours(now.Hour).AddMinutes(now.Minute).AddMilliseconds(now.Millisecond) });
            var reader = command.ExecuteReader(CommandBehavior.CloseConnection);

            var isCurrent = this.BaseDate.Equals(DateTime.Today);

            var ExtrachargeCommissions = CommissionCalculator.GetCommissions(true, isCurrent);
            var AmountCommissions = CommissionCalculator.GetCommissions(false, isCurrent);

            while (reader.Read())
            {
                int index = -1;
                int pricelistID = reader.GetInt32(++index);
                TABS.PriceList priceList = null;
                if (!pricelists.TryGetValue(pricelistID, out priceList))
                {

                    priceList = new TABS.PriceList()
                    {
                        ID = pricelistID,
                        Supplier = allCarriers[reader.GetString(++index)],
                        Customer = TABS.CarrierAccount.SYSTEM,
                        Currency = TABS.Currency.All[reader.GetString(++index)],
                        BeginEffectiveDate = reader.IsDBNull(++index) ? null : (DateTime?)reader.GetDateTime(index),
                        //SourceFileName = reader.IsDBNull(++index) ? null : reader.GetString(index)
                    };
                   
                    pricelists.Add(pricelistID, priceList);
                }
                else index += 3;//+4

                int zoneID = reader.GetInt32(++index);
                TABS.Zone zone = null;
                if (!zones.TryGetValue(zoneID, out zone))
                {
                    if (!TABS.CodeGroup.All.Keys.Contains(reader.GetString(++index)))
                    {
                        continue;
                    }
                    zone = new TABS.Zone();
                    //{
                    zone.ZoneID = zoneID;
                    zone.CodeGroup = TABS.CodeGroup.All[reader.GetString(index)];
                    zone.Name = (reader.GetString(++index).Length > 70) ? reader.GetString(index).Substring(0, 69) : reader.GetString(index);
                    zone.Supplier = priceList.Supplier;
                    zone.ServicesFlag = reader.GetInt16(++index);
                    zone.BeginEffectiveDate = reader.IsDBNull(++index) ? null : (DateTime?)reader.GetDateTime(index);
                    zone.EndEffectiveDate = reader.IsDBNull(++index) ? null : (DateTime?)reader.GetDateTime(index);
                    //};
                    zones.Add(zoneID, zone);
                }
                else index += 5;//+5

                TABS.Rate rate = new Rate();


                rate.ID = reader.GetInt64(++index);
                rate.PriceList = priceList;
                rate.Zone = zone;
                rate.Value = reader.IsDBNull(++index) ? null : (decimal?)reader.GetDecimal(index);
                rate.OffPeakRate = reader.IsDBNull(++index) ? null : (decimal?)reader.GetDecimal(index);
                rate.WeekendRate = reader.IsDBNull(++index) ? null : (decimal?)reader.GetDecimal(index);
                rate.Change = reader.IsDBNull(++index) ? Change.None : (Change)reader.GetInt16(index);
                rate.ServicesFlag = reader.GetInt16(++index);
                rate.BeginEffectiveDate = reader.IsDBNull(++index) ? null : (DateTime?)reader.GetDateTime(index);
                rate.EndEffectiveDate = reader.IsDBNull(++index) ? null : (DateTime?)reader.GetDateTime(index);

                var key = string.Concat(rate.PriceList.Supplier.CarrierAccountID
                    , rate.PriceList.Customer.CarrierAccountID
                    , rate.Zone == null ? string.Empty : rate.ZoneID.ToString());





                if (rate.Value.HasValue)
                {
                    var factor = (decimal)(TABS.Currency.Main.LastRate / rate.PriceList.Currency.LastRate);
                    var Operator = !rate.Supplier.Equals(TABS.CarrierAccount.SYSTEM) ? 1 : -1;
                    var rateValue = rate.Value.Value;

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
                    rate.Value += amountCommission + extrachargeCommission;
                    rates.Add(rate);

                }
            }


            reader.Close();
            reader.Dispose();

            // get supply rates
            foreach (var rate in rates)
                SupplyRates[rate.Zone] = rate;
            LoadingTimes["Supply Rates"] = stopWatch.StopTiming();
        }

        protected class CodeWorker
        {
            public System.Threading.Thread WorkerThread { get; protected set; }
            public bool IsWorking { get { return WorkerThread.IsAlive; } }
            public RoutePool RoutePool { get; protected set; }
            public IEnumerable<string> Codes { get; protected set; }
            public CarrierAccount ExactMatchCustomer = null;
            public bool ExactMatchIncludeLosses = false;
            public bool IsCurrent = false;
            static object _lockrootpool = new object();
            public CodeWorker(IEnumerable<string> codes, RoutePool routePool)
            {
                this.Codes = codes;
                this.RoutePool = routePool;
                this.WorkerThread = new System.Threading.Thread(new System.Threading.ThreadStart(Execute));
            }
            public CodeWorker(IEnumerable<string> codes, RoutePool routePool, CarrierAccount Customer, bool IncludeLossesSubCodes,bool IsCurrentPool)
            {
                this.ExactMatchCustomer = Customer;
                ExactMatchIncludeLosses = IncludeLossesSubCodes;
                IsCurrent = IsCurrentPool;
                this.Codes = codes;
                this.RoutePool = routePool;
                //System.Threading.Thread newThread = new System.Threading.Thread(() => { this.MethodLauncher("ExecuteExact",Customer); });
                // this.WorkerThread = newThread; 
                this.WorkerThread = new System.Threading.Thread(new System.Threading.ThreadStart(ExecuteExact));
                this.WorkerThread.Priority = System.Threading.ThreadPriority.BelowNormal;
            }
            public void Run()
            {
                WorkerThread.Start();
            }

            void Execute()
            {
                
                var routePool = RoutePool;
                CodeRoute codeRoute=null; ;
             
                foreach (var code in Codes)
                {

                    codeRoute = new CodeRoute(code, routePool);
                    lock (routePool)
                    {
                        routePool.CodeRoutes[code] = codeRoute;
                    }
                    System.Threading.Thread.Sleep(0);
                }
            }

            void ExecuteExact()
            {

                var routePool = RoutePool;
                CodeRoute codeRoute = null;
                Dictionary<string, List<CarrierAccount>> CustomerExactMatchCodeSuupliers = null;
                if (this.ExactMatchCustomer == null) this.ExactMatchIncludeLosses = false;
                
                    //case if the customer lossses already checked and it not make loss
                if (IsCurrent == true)
                {
                    lock (_lockrootpool)
                    {
                        if (this.ExactMatchCustomer != null && TABS.Components.RoutePool.ExactMatchCodeSuupliers.TryGetValue(this.ExactMatchCustomer.CarrierAccountID, out CustomerExactMatchCodeSuupliers) == true)
                            this.ExactMatchIncludeLosses = false;
                    }
                }
                else
                {
                    lock (_lockrootpool)
                    {
                        if (this.ExactMatchCustomer != null && TABS.Components.RoutePool.FutureExactMatchCodeSuupliers.TryGetValue(this.ExactMatchCustomer.CarrierAccountID, out CustomerExactMatchCodeSuupliers) == true)
                            this.ExactMatchIncludeLosses = false;
                    }
                }
            
                foreach (var code in Codes)
                {
                    
                        codeRoute = new CodeRoute(code, routePool, this.ExactMatchCustomer, this.ExactMatchIncludeLosses,this.IsCurrent);

                        if (codeRoute == null) continue;
                        if (codeRoute.SaleCode == null) continue;
                        lock (routePool)
                        {
                           routePool.CodeRoutes[code] = codeRoute;
                        }
                   
                    System.Threading.Thread.Sleep(0);
                }
            }
            public void MethodLauncher(string methodName, object param)
            {

                Type t = this.GetType();

                // Other overloads of GetMethod allow access to private etc.
                MethodInfo method = t.GetMethod(methodName, new Type[] { });

                method.Invoke(this, new object[] { param });

            }

        }

        protected void BuildCodeRoutesAndZoneSupply()
        {
            // Fill in all Codes
            TABS.Addons.Utilities.StopWatch stopWatch = new TABS.Addons.Utilities.StopWatch();
            stopWatch.StartTiming();
            this.ValidSuppliers = this.CodeMap.Suppliers.Where(s => s.ActivationStatus != ActivationStatus.Inactive && s.RoutingStatus == RoutingStatus.Enabled || s.RoutingStatus == RoutingStatus.BlockedInbound).ToArray();


            this.AllCodes = this.CodeMap.GetAllCodes(this.ValidSuppliers).ToArray();
            // get all codes 
            //var codes = this.BaseDate.Equals(DateTime.Today) ? CurrentZoneCodes : FutureZoneCodes;

            //HashSet<string> hashCodes = new HashSet<string>():
            //foreach (var supplier in this.ValidSuppliers)
            //{
            //    codes.
            //}




            CodeRoutes = new Dictionary<string, CodeRoute>();
            this.SupplyRatesBySaleZone = new Dictionary<Zone, HashSet<Rate>>();
            bool IsCurrent = true;
            IsCurrent = (RoutePool._ISCurrentState == true) ? true : false;

            if (IsCurrent)
                RoutePool.ExactMatchCodeSuupliers = (RoutePool.ExactMatchCodeSuupliers == null) ? new Dictionary<string, Dictionary<string, List<CarrierAccount>>>() : RoutePool.ExactMatchCodeSuupliers;
            else
                RoutePool.FutureExactMatchCodeSuupliers = (RoutePool.FutureExactMatchCodeSuupliers == null) ? new Dictionary<string, Dictionary<string, List<CarrierAccount>>>() : RoutePool.FutureExactMatchCodeSuupliers;
            
            int numOfBatches = 6;
            List<CodeWorker> workers = new List<CodeWorker>();
            var batchCount = (this.AllCodes.Length / numOfBatches) + numOfBatches;
            var codeBatchesGroupings = this.AllCodes.GroupByBatch(batchCount);
            List<List<string>> codeBatches = new List<List<string>>();
            foreach (var codeBatchGrouping in codeBatchesGroupings) codeBatches.Add(codeBatchGrouping.ToList());
            foreach (var codeBatch in codeBatches)
            {
                var worker = new CodeWorker(codeBatch, this);
                workers.Add(worker);
                worker.Run();
            }

            // Wait for workers to finish
            bool notFinished = true;
            while (notFinished)
            {
                notFinished = false;
                System.Threading.Thread.Sleep(250);
                foreach (var worker in workers)
                    if (worker.IsWorking)
                    {
                        notFinished = true;
                        break;
                    }
            }

            var spent = stopWatch.StopTiming();
            LoadingTimes["Code Routes and Zone Supply"] = spent;
        }
        protected void BuildCodeRoutesAndZoneSupply(CarrierAccount Customer, bool IncludeLossesSubCodes)
        {
            // Fill in all Codes
            TABS.Addons.Utilities.StopWatch stopWatch = new TABS.Addons.Utilities.StopWatch();
            stopWatch.StartTiming();
            this.ValidSuppliers = this.CodeMap.Suppliers.Where(s => s.ActivationStatus != ActivationStatus.Inactive && s.RoutingStatus == RoutingStatus.Enabled || s.RoutingStatus == RoutingStatus.BlockedInbound).ToArray();

            bool IsCurrent = true;
            this.AllCodes = this.CodeMap.GetAllCodes(this.ValidSuppliers).ToArray();
            CodeRoutes = new Dictionary<string, CodeRoute>();
            this.SupplyRatesBySaleZone = new Dictionary<Zone, HashSet<Rate>>();
            
            IsCurrent = (RoutePool._ISCurrentState == true) ? true : false;
           
            if (IsCurrent)
                RoutePool.ExactMatchCodeSuupliers = (RoutePool.ExactMatchCodeSuupliers == null) ? new Dictionary<string, Dictionary<string, List<CarrierAccount>>>() : RoutePool.ExactMatchCodeSuupliers;
            else  
                RoutePool.FutureExactMatchCodeSuupliers = (RoutePool.FutureExactMatchCodeSuupliers == null) ? new Dictionary<string, Dictionary<string, List<CarrierAccount>>>() : RoutePool.FutureExactMatchCodeSuupliers;
            
            
           
            int numOfBatches =(IncludeLossesSubCodes==false)? 6:6;
            List<CodeWorker> workers = new List<CodeWorker>();

            string[] SaleCodesforSys = this.CodeMap.SupplierCodes[TABS.CarrierAccount.SYSTEM].Keys.ToArray();
            //SaleCodesforSys = SaleCodesforSys.Where(c => c.StartsWith("93")).ToArray();
            var batchCount = (this.AllCodes.Length / numOfBatches) + numOfBatches;
            batchCount = (SaleCodesforSys.Length / numOfBatches) + numOfBatches;
            //batchCount = SaleCodesforSys.Length+1;
            var codeBatchesGroupings = SaleCodesforSys.GroupByBatch(batchCount);
           // codeBatchesGroupings = SaleCodesforSys.Take(2200).GroupByBatch(2200);
            List<List<string>> codeBatches = new List<List<string>>();
            foreach (var codeBatchGrouping in codeBatchesGroupings) codeBatches.Add(codeBatchGrouping.ToList());
            foreach (var codeBatch in codeBatches)
            {
                var worker = new CodeWorker(codeBatch, this, Customer, IncludeLossesSubCodes, IsCurrent);
                workers.Add(worker);
                worker.Run();
            }

            // Wait for workers to finish
            bool notFinished = true;
            while (notFinished)
            {
                notFinished = false;
                System.Threading.Thread.Sleep(250);
                foreach (var worker in workers)
                    if (worker.IsWorking)
                    {
                        notFinished = true;
                        break;
                    }
            }
            if (IncludeLossesSubCodes == true)
            {
                Dictionary<string, List<CarrierAccount>> CustomerExactMatchCodeSuupliers = new Dictionary<string, List<CarrierAccount>>();
                
                    //case if the customer lossses already checked and it not make loss
                
                if (IsCurrent == true && TABS.Components.RoutePool.ExactMatchCodeSuupliers.TryGetValue(Customer.CarrierAccountID, out CustomerExactMatchCodeSuupliers) == false)
                    TABS.Components.RoutePool.ExactMatchCodeSuupliers.Add(Customer.CarrierAccountID, CustomerExactMatchCodeSuupliers);

                if (IsCurrent == false && TABS.Components.RoutePool.FutureExactMatchCodeSuupliers.TryGetValue(Customer.CarrierAccountID, out CustomerExactMatchCodeSuupliers) == false)
                    TABS.Components.RoutePool.FutureExactMatchCodeSuupliers.Add(Customer.CarrierAccountID, CustomerExactMatchCodeSuupliers);
                
                    
                
               
            }
            var spent = stopWatch.StopTiming();
            LoadingTimes["Code Routes and Zone Supply"] = spent;
        }
        protected void BuildCodeRoutesAndZoneSupply(CarrierAccount Customer, bool IncludeLossesSubCodes,bool SaleCodeOnly)
        {
            // Fill in all Codes
            TABS.Addons.Utilities.StopWatch stopWatch = new TABS.Addons.Utilities.StopWatch();
            stopWatch.StartTiming();
            this.ValidSuppliers = this.CodeMap.Suppliers.Where(s => s.ActivationStatus != ActivationStatus.Inactive && s.RoutingStatus == RoutingStatus.Enabled || s.RoutingStatus == RoutingStatus.BlockedInbound).ToArray();

            bool IsCurrent = true;
            this.AllCodes = this.CodeMap.GetAllCodes(this.ValidSuppliers).ToArray();
            CodeRoutes = new Dictionary<string, CodeRoute>();
            this.SupplyRatesBySaleZone = new Dictionary<Zone, HashSet<Rate>>();

            IsCurrent = (RoutePool._ISCurrentState == true) ? true : false;

            if (IsCurrent)
                RoutePool.ExactMatchCodeSuupliers = (RoutePool.ExactMatchCodeSuupliers == null) ? new Dictionary<string, Dictionary<string, List<CarrierAccount>>>() : RoutePool.ExactMatchCodeSuupliers;
            else
                RoutePool.FutureExactMatchCodeSuupliers = (RoutePool.FutureExactMatchCodeSuupliers == null) ? new Dictionary<string, Dictionary<string, List<CarrierAccount>>>() : RoutePool.FutureExactMatchCodeSuupliers;



            int numOfBatches = (IncludeLossesSubCodes == false) ? 6 : 6;
            List<CodeWorker> workers = new List<CodeWorker>();
            string[] SaleCodesforSys =null;

            if (SaleCodeOnly == true)
                SaleCodesforSys = this.CodeMap.CustomerSaleCodes.ToArray();
            else
                SaleCodesforSys = this.CodeMap.SupplierCodes[TABS.CarrierAccount.SYSTEM].Keys.ToArray();

            var batchCount = (this.AllCodes.Length / numOfBatches) + numOfBatches;
            batchCount = (SaleCodesforSys.Length / numOfBatches) + numOfBatches;
            var codeBatchesGroupings = SaleCodesforSys.GroupByBatch(batchCount);
            // codeBatchesGroupings = SaleCodesforSys.Take(2200).GroupByBatch(2200);
            List<List<string>> codeBatches = new List<List<string>>();
            foreach (var codeBatchGrouping in codeBatchesGroupings) codeBatches.Add(codeBatchGrouping.ToList());
            foreach (var codeBatch in codeBatches)
            {
                var worker = new CodeWorker(codeBatch, this, Customer, IncludeLossesSubCodes, IsCurrent);
                workers.Add(worker);
                worker.Run();
            }

            // Wait for workers to finish
            bool notFinished = true;
            while (notFinished)
            {
                notFinished = false;
                System.Threading.Thread.Sleep(250);
                foreach (var worker in workers)
                    if (worker.IsWorking)
                    {
                        notFinished = true;
                        break;
                    }
            }
            if (IncludeLossesSubCodes == true)
            {
                Dictionary<string, List<CarrierAccount>> CustomerExactMatchCodeSuupliers = new Dictionary<string, List<CarrierAccount>>();

                //case if the customer lossses already checked and it not make loss

                if (IsCurrent == true && TABS.Components.RoutePool.ExactMatchCodeSuupliers.TryGetValue(Customer.CarrierAccountID, out CustomerExactMatchCodeSuupliers) == false)
                    TABS.Components.RoutePool.ExactMatchCodeSuupliers.Add(Customer.CarrierAccountID, CustomerExactMatchCodeSuupliers);

                if (IsCurrent == false && TABS.Components.RoutePool.FutureExactMatchCodeSuupliers.TryGetValue(Customer.CarrierAccountID, out CustomerExactMatchCodeSuupliers) == false)
                    TABS.Components.RoutePool.FutureExactMatchCodeSuupliers.Add(Customer.CarrierAccountID, CustomerExactMatchCodeSuupliers);




            }
            var spent = stopWatch.StopTiming();
            LoadingTimes["Code Routes and Zone Supply"] = spent;
        }
        protected RoutePool(DateTime baseDate)
        {
            this.BaseDate = baseDate;
            LoadingTimes = new Dictionary<string, TimeSpan>();
            TABS.Addons.Utilities.StopWatch stopWatch = new TABS.Addons.Utilities.StopWatch();
            stopWatch.StartTiming();

            this.CodeMap = this.BaseDate >= CodeMap.Current.StartDate ? CodeMap.Current : new CodeMap(this.BaseDate);
            var spent = stopWatch.StopTiming();
            LoadingTimes["Code Map"] = spent;

            // Load Supply Rates
            LoadSupplyRates();

            // Build Code Routes and Zone-Supply Match
            BuildCodeRoutesAndZoneSupply();

            // Finished
            LoadingTimes["Rate Pool"] = stopWatch.StopTiming();
            TABS.Addons.Utilities.RatePoolChecker.ChangeLastRoutePoolCachclear();
            this.SupplyRates = null;
            CodeMap.ClearCachedCollections();
            this.CodeMap = null;
            GC.Collect();
        }
        protected RoutePool(DateTime baseDate,CarrierAccount Customer,bool IncludeLossesSubCodes)
        {
            this.BaseDate = baseDate;
            LoadingTimes = new Dictionary<string, TimeSpan>();
            TABS.Addons.Utilities.StopWatch stopWatch = new TABS.Addons.Utilities.StopWatch();
            stopWatch.StartTiming();

            this.CodeMap = this.BaseDate >= CodeMap.CurrentCodeMap(Customer, IncludeLossesSubCodes).StartDate ? CodeMap.CurrentCodeMap(Customer, IncludeLossesSubCodes) : new CodeMap(this.BaseDate, Customer, IncludeLossesSubCodes);
            var spent = stopWatch.StopTiming();
            LoadingTimes["Code Map"] = spent;

            // Load Supply Rates
            LoadSupplyRates();

            // Build Code Routes and Zone-Supply Match
            BuildCodeRoutesAndZoneSupply(Customer, IncludeLossesSubCodes);

            // Finished
            LoadingTimes["Rate Pool"] = stopWatch.StopTiming();
            TABS.Addons.Utilities.RatePoolChecker.ChangeLastRoutePoolCachclear();
            this.SupplyRates = null;
            CodeMap.ClearCachedCollections();
            this.CodeMap = null;
            GC.Collect();
        }
        protected RoutePool(DateTime baseDate, CarrierAccount Customer,bool IncludeLossesSubCodes, bool SaleCodeOnly)
        {
           
            this.BaseDate = baseDate;
            LoadingTimes = new Dictionary<string, TimeSpan>();
            TABS.Addons.Utilities.StopWatch stopWatch = new TABS.Addons.Utilities.StopWatch();
            stopWatch.StartTiming();

            this.CodeMap = this.BaseDate >= CodeMap.CurrentCodeMap(Customer, IncludeLossesSubCodes).StartDate ? CodeMap.CurrentCodeMap(Customer, IncludeLossesSubCodes) : new CodeMap(this.BaseDate, Customer, IncludeLossesSubCodes);
            var spent = stopWatch.StopTiming();
            LoadingTimes["Code Map"] = spent;

            // Load Supply Rates
            LoadSupplyRates();

            // Build Code Routes and Zone-Supply Match
            BuildCodeRoutesAndZoneSupply(Customer, IncludeLossesSubCodes,SaleCodeOnly);

            // Finished
            LoadingTimes["Rate Pool"] = stopWatch.StopTiming();
            TABS.Addons.Utilities.RatePoolChecker.ChangeLastRoutePoolCachclear();
            this.SupplyRates = null;
            CodeMap.ClearCachedCollections();
            this.CodeMap = null;
            GC.Collect();
        }
        protected RoutePool(DateTime baseDate, bool forCodeComparaison)
        {
            this.BaseDate = baseDate;
            LoadingTimes = new Dictionary<string, TimeSpan>();
            TABS.Addons.Utilities.StopWatch stopWatch = new TABS.Addons.Utilities.StopWatch();
            stopWatch.StartTiming();

            this.CodeMap = this.BaseDate >= CodeMap.Current.StartDate ? CodeMap.Current : new CodeMap(this.BaseDate);
            var spent = stopWatch.StopTiming();
            LoadingTimes["Code Map"] = spent;

            // Load Supply Rates
            LoadSupplyRates(forCodeComparaison);

            // Build Code Routes and Zone-Supply Match
            BuildCodeRoutesAndZoneSupply();

            // Finished
            LoadingTimes["Rate Pool"] = stopWatch.StopTiming();
            TABS.Addons.Utilities.RatePoolChecker.ChangeLastRoutePoolCachclear();
            this.SupplyRates = null;
            CodeMap.ClearCachedCollections();
            this.CodeMap = null;
            GC.Collect();
        }

        protected List<TABS.Zone> GetMatchingSaleZone(TABS.Zone purchaseZone)
        {
            List<TABS.Zone> saleZones = new List<Zone>();

            var purchaseCodes = new List<Code>();
            // FillMatchingCodes(this.CodeMap.SupplierCodes[purchaseZone.Supplier], purchaseZone, purchaseCodes);

            return saleZones;
        }

        public static Dictionary<int, List<CodeInfo>> LoadZoneCodes(DateTime date)
        {

            var result = new Dictionary<int, List<CodeInfo>>();
            string sqlQuery = string.Format(@"
                                                DECLARE @date DATETIME 
                                                SET @date = '{0:yyyy-MM-dd HH:mm:ss}';

                                                   WITH TempZones AS (
                                                    SELECT z.ZoneID,
                                                           z.CodeGroup,
                                                           z.SupplierID,
                                                           z.[Name]
                                                    FROM   Zone z WITH(NOLOCK, INDEX(IX_Zone_Dates))
                                                    WHERE  Z.BeginEffectiveDate <= @date
                                                           AND (ISNULL(Z.EndEffectiveDate, '2020-01-01') >= @date)
                                                    )
                                                    SELECT c.Code,
                                                           c.ZoneID,
                                                           z.CodeGroup,
                                                            z.SupplierID,
                                                            z.NAME 
                                                    FROM   Code c WITH(NOLOCK, INDEX(ix_Code_BED))
                                                           LEFT JOIN TempZones z
                                                                ON  c.ZoneID = z.ZoneID
                                                    WHERE  C.BeginEffectiveDate <= @date
                                                           AND (ISNULL(C.EndEffectiveDate, '2020-01-01') >= @date)
                                      ", date);

            List<CodeInfo> codes = new List<CodeInfo>();
            _ZoneNames = new Dictionary<int, string>();
            using (IDataReader codeReader = TABS.DataHelper
                      .ExecuteReader(sqlQuery))
            {
                while (codeReader.Read())
                {
                    int index = -1;
                    CodeInfo code = new CodeInfo();
                    index++; var codevalue = codeReader.GetString(index); code.Code = codevalue;
                    index++; var zoneId = codeReader.GetInt32(index); code.ZoneID = zoneId;
                    index++; var codeGroup = codeReader.IsDBNull(index)
                        ? string.Empty : codeReader.GetString(index); code.CodeGroup = codeGroup;
                    index++; var supplierID = codeReader.IsDBNull(index) ? string.Empty : codeReader.GetString(index); code.SupplierID = supplierID;
                    index++; var zoneName = codeReader.IsDBNull(index) ? string.Empty : codeReader.GetString(index); code.ZoneName = zoneName;
                    codes.Add(code);
                }
            }

            foreach (var code in codes)
            {
                if (!result.ContainsKey(code.ZoneID))
                    result[code.ZoneID] = new List<CodeInfo>();

                result[code.ZoneID].Add(code);

                _ZoneNames[code.ZoneID] = code.ZoneName;

            }

            return result;
        }

        internal static object dummyObject = new object();

        internal static Dictionary<int, List<CodeInfo>> _CurrentSaleZoneCodes;
        public static Dictionary<int, List<CodeInfo>> CurrentSaleZoneCodes
        {
            get
            {
                return _CurrentSaleZoneCodes;
            }
        }

        internal static Dictionary<int, List<CodeInfo>> _CurrentZoneCodes;
        public static Dictionary<int, List<CodeInfo>> CurrentZoneCodes
        {
            get
            {
                if (_CurrentZoneCodes == null)
                    _CurrentZoneCodes = LoadZoneCodes(DateTime.Today);

                //_CurrentSaleZoneCodes = new Dictionary<int, List<CodeInfo>>();
                //foreach (var kvp in _CurrentZoneCodes)
                //{
                //    foreach (var codeinfo in kvp.Value)
                //    {
                //        if (codeinfo.SupplierID == TABS.CarrierAccount.SYSTEM.CarrierAccountID)
                //        {
                //            _CurrentSaleZoneCodes[kvp.Key] = kvp.Value;
                //            break;
                //        }
                //    }
                //}
                return _CurrentZoneCodes;

            }
        }

        internal static Dictionary<int, List<CodeInfo>> _FutureZoneCodes;
        public static Dictionary<int, List<CodeInfo>> FutureZoneCodes
        {
            get
            {
                if (_FutureZoneCodes == null)
                {
                    var noticeDays = (double)TABS.SystemConfiguration.KnownParameters[TABS.KnownSystemParameter.sys_BeginEffectiveRateDays].NumericValue.Value;
                    _FutureZoneCodes = LoadZoneCodes(DateTime.Today.AddDays(noticeDays));
                }
                return _FutureZoneCodes;

            }
        }

        internal static Dictionary<int, string> _ZoneNames;
        public static Dictionary<int, string> ZoneNames
        {
            get
            {
                return _ZoneNames;
            }
        }
        public class CodeInfo
        {
            public string Code { get; set; }
            public int ZoneID { get; set; }
            public string CodeGroup { get; set; }
            public string SupplierID { get; set; }
            public string ZoneName { get; set; }

        }
    }
    # region LCR Result Class
    public class LCR_Result
    {
        # region Properties
        /// <summary>
        /// "Current" or "Future" Route Pool
        /// </summary>
        private string rblCF { get; set; }
        private bool _Current { get; set; }
        /// <summary>
        /// Check Selected Customer
        /// </summary>
        private bool CheckCustomer { get; set; }
        private TABS.CarrierAccount _SelectedCustomer { get; set; }
        private string _LastSelectedCustomer { get; set; }
        /// <summary>
        /// Specify the supplier rate policy can be "None" or "Lowest_Rate" or "Highest_Rate"
        /// </summary>
        private TABS.SupplierRatePolicy SupplierRateDDl { get; set; }
        private TABS.SupplierRatePolicy SelectedPolicy { get { return SupplierRateDDl; } }
        /// <summary>
        /// "Code" or "Zone" Selection
        /// </summary>
        private string rblByZoneCode { get; set; }
        /// <summary>
        /// Select the flagged services e.g. whole sale 0
        /// </summary>
        private short FlaggedServicesSelection { get; set; }
        private string ServiceSelected { get; set; }
        /// <summary>
        /// Get the Route Pool
        /// </summary>
        //private TABS.Components.RoutePool _SelectedPool { get { return rblCF == "Current" ? TABS.Components.RoutePool.Current : TABS.Components.RoutePool.Future; } }
        private TABS.Components.RoutePool _SelectedPool
        {

            get
            {
                bool IsExactMatchProcessing = (bool)TABS.SystemConfiguration.KnownParameters[TABS.KnownSystemParameter.sys_ExactMatchProcessing].Value;
                if (IsExactMatchProcessing == false)
                    return rblCF == "Current" ? TABS.Components.RoutePool.Current : TABS.Components.RoutePool.Future;
                else
                {
                    TABS.CarrierAccount customer = _SelectedCustomer;
                    bool IncludeLosess = true;
                    if (rblCF == "Current") return TABS.Components.RoutePool.CurrentPool(customer, IncludeLosess);
                    else
                        return TABS.Components.RoutePool.FuturePool(customer, IncludeLosess);
                }
            }
        }

        /// <summary>
        /// Filter code or zone according to the rblCF, e.g. all codes: ".*" or zone: "Afgh%"
        /// </summary>
        private string txtFilter { get; set; }
        private string TargetFilterValue { get { return txtFilter.Trim().Length > 0 ? txtFilter.Trim() : null; } }
        /// <summary>
        /// Rate less than
        /// </summary>
        private bool checkRate { get; set; }
        private double? __RateLessThan { get; set; }
        private double? ExcludeRateCondition { get; set; }
        private double _RateLessThan { get { if (__RateLessThan == null) __RateLessThan = (checkRate && ExcludeRateCondition != null) ? ExcludeRateCondition : (double)int.MaxValue; return __RateLessThan.Value; } }
        /// <summary>
        /// Include supplier block zone
        /// </summary>
        private bool IncludeSZB { get; set; }
        private bool IncludeBlockedZones { get { return IncludeSZB; } }
        private List<int> _supplierZoneBlocks { get; set; }
        private int _supplierZoneBlocksCount { get; set; }
        private Dictionary<int, List<string>> _supplierBlockedInZone { get; set; }
        /// <summary>
        /// Customer Exchange Rate
        /// </summary>
        private Dictionary<TABS.Zone, TABS.Rate> _OurRateCustomer = null;
        private float? __CustExchangeRate = null;
        private float _CustExchangeRate { get { if (__CustExchangeRate == null) __CustExchangeRate = _SelectedCustomer != null ? _SelectedCustomer.CarrierProfile.Currency.LastRate : 1; return __CustExchangeRate.Value; } }
        /// <summary>
        /// Result ZoneRate or CodeRate
        /// </summary>
        public Dictionary<int, TABS.DTO.DTO_ZoneRate> DataSource { get; set; }
        private List<TABS.DTO.DTO_ZoneRate> rgRates { get; set; }
        public Dictionary<string, TABS.DTO.DTO_CodeRate> DataSourceByCode { get; set; }
        private List<TABS.DTO.DTO_CodeRate> gvLCRByCode { get; set; }
        # endregion
        # region Supplier Zone Block
        private List<int> GetBlockedSupplierZones()
        {
            IEnumerable<TABS.RouteBlock> supplierZoneBlocks;
            //IEnumerable<TABS.RouteBlock> supplierCodeBlocks;
            bool Isfuture = (rblCF == "Current") ? false : true;
            if (_SelectedCustomer == null)
                supplierZoneBlocks = TABS.RouteBlock.SupplierZoneBlocksList(Isfuture);
            else
                supplierZoneBlocks = TABS.RouteBlock.SupplierZoneBlocksList(Isfuture)
                            .Where(s => s.Customer == null || (s.Customer.Equals(_SelectedCustomer) && s.Customer != null));
            //if (rblByZoneCode.SelectedItem.Text == "Code")
            //{//special R and SZb codes
            //    if (_SelectedCustomer == null)
            //        supplierCodeBlocks = TABS.RouteBlock.SupplierCodeBlocks;
            //    else
            //        supplierCodeBlocks = TABS.RouteBlock.SupplierCodeBlocks
            //                    .Where(s => s.Customer == null || (s.Customer.Equals(_SelectedCustomer) && s.Customer != null));
            //    _supplierCodeBlocks = supplierCodeBlocks.Where(s => (s.Zone == null)).Select(s => s.Code).ToList(); _LastLCRType = rblByZoneCode.SelectedItem.Text; 

            //}
            var zoneIds = supplierZoneBlocks.Select(s => s.Zone.ZoneID).ToList();
            zoneIds.Sort();
            _supplierBlockedInZone.Clear();
            _supplierBlockedInZone = supplierZoneBlocks.GroupBy(z => z.Zone.ZoneID).ToDictionary(g => g.Key, g => g.Where(gg => gg.Supplier != null).Select(gg => gg.Supplier.Name).ToList());
            //foreach(int zoneid in zoneIds)
            //{
            //    _supplierBlockedInZone[zoneid] = supplierZoneBlocks.Where(r =>  r.Supplier!=null && r.Zone.ZoneID == zoneid ).Select(r => r.Supplier.Name).ToList();
            //}
            //if all codes is blocked
            _supplierZoneBlocksCount = zoneIds.Count();
            return zoneIds;
        }
        private List<int> SupplierZoneBlocks
        {
            get
            {
                if (_supplierZoneBlocks == null)
                    _supplierZoneBlocks = new List<int>();
                bool Isfuture = (rblCF == "Current") ? false : true;
                string selectedcustomer = (_SelectedCustomer != null) ? _SelectedCustomer.CarrierAccountID : null;
                if (((CheckCustomer == true && _LastSelectedCustomer != selectedcustomer) || _supplierZoneBlocks.Count() == 0) || (CheckCustomer == false && TABS.RouteBlock.SupplierZoneBlocksList(Isfuture).Count() != _supplierZoneBlocksCount))//|| rblByZoneCode.SelectedItem.Text != _LastLCRType
                {
                    _LastSelectedCustomer = (_LastSelectedCustomer == null) ? selectedcustomer : _LastSelectedCustomer;
                    _LastSelectedCustomer = (_SelectedCustomer == null) ? null : _LastSelectedCustomer;
                    _supplierZoneBlocks = GetBlockedSupplierZones();
                    //_supplierZoneBlocks.AddRange(SupplierZoneROBlocksIDS.Keys);
                }
                return _supplierZoneBlocks;
            }
            set { _supplierZoneBlocks = value; }
        }
        # endregion
        # region Get Rates: Favorite Rates, Supplier Rate and Our Rates
        private Dictionary<TABS.Zone, List<TABS.Rate>> GetFavorateRates(Dictionary<TABS.Zone, List<TABS.Rate>> supplyRates)
        {
            bool Isfuture = (rblCF == "Current") ? false : true;
            var blocks = TABS.RouteBlock.SupplierZoneBlocksList(Isfuture).ToList();
            //List<TABS.RouteBlock> iraq = blocks.Where(z => z.Zone.Name == "Iraq-Baghdad").ToList();
            //iraq = blocks.Where(z => z.Zone.ZoneID == 102689).ToList();
            Dictionary<TABS.Zone, List<TABS.Rate>> result = new Dictionary<TABS.Zone, List<TABS.Rate>>();
            foreach (var item in supplyRates)
            {
                HashSet<TABS.Rate> rates = new HashSet<TABS.Rate>();
                foreach (var rate in item.Value)
                    rates.Add(rate);

                var Warnings = new List<string>();
                var Favorates = TABS.DataHelper.GetFavorateRates(_SelectedCustomer, blocks.ToArray(), item.Key, rates, _Current, Warnings, IncludeSZB);
                result[item.Key] = insurePolicy(Favorates);
            }
            return result;
        }
        private Dictionary<TABS.Zone, List<TABS.Rate>> GetSupplierRatesByZone()
        {
            var BlockedSupplierZonesIDs = SupplierZoneBlocks;
            var zoneRatesOfCustomer =
                    _SelectedPool.SupplyRatesBySaleZone
                    .Where(kvp =>
                        (TargetFilterValue == null
                        ||
                        System.Text.RegularExpressions.Regex.IsMatch(kvp.Key.Name ?? "", TargetFilterValue, System.Text.RegularExpressions.RegexOptions.IgnoreCase)
                        ||
                        kvp.Key.EffectiveCodes.FirstOrDefault(c => System.Text.RegularExpressions.Regex.IsMatch(c.Value ?? "", TargetFilterValue, System.Text.RegularExpressions.RegexOptions.IgnoreCase)) != null));
            short servicesFlag = _SelectedCustomer != null ? FlaggedServicesSelection : TABS.FlaggedService.Default.FlaggedServiceID;
            TABS.SupplierRatePolicy policy = SupplierRateDDl;
            Dictionary<TABS.Zone, List<TABS.Rate>> zoneRates = new Dictionary<TABS.Zone, List<TABS.Rate>>();
            List<TABS.Rate> Rates = new List<TABS.Rate>();
            foreach (var item in zoneRatesOfCustomer)
            {
                zoneRates[item.Key] = item.Value.ToList();
            }
            return policy == TABS.SupplierRatePolicy.Highest_Rate ? GetFavorateRates(zoneRates) : zoneRates;
        }
        private void GetOurRates(out Dictionary<TABS.Zone, decimal> ourRatesNoCust, out Dictionary<TABS.Zone, TABS.Rate> ourRatesCust)
        {
            string currentFutureCondition = "r.BeginEffectiveDate <= :when and (r.EndEffectiveDate is null or r.EndEffectiveDate >= :when)";
            string ServiceSelected = TABS.FlaggedService.All.First().Key.ToString(); //FlaggedServicesSelection.ServicesFlag.ToString();

            var SystemIncreaseDays = (double)(decimal)TABS.SystemConfiguration.KnownParameters[TABS.KnownSystemParameter.sys_BeginEffectiveRateDays].Value;
            ourRatesCust = null;
            ourRatesNoCust = null;
            if (_SelectedCustomer == null)
            {
                var q = TABS.ObjectAssembler.CurrentSession
                    .CreateQuery("from Rate r where r.PriceList.Supplier = :sys and " + currentFutureCondition)
                    .SetParameter("sys", TABS.CarrierAccount.SYSTEM)
                    .SetParameter("when", _Current ? DateTime.Now : DateTime.Today.AddDays(SystemIncreaseDays));

                ourRatesNoCust = q
                    .List<TABS.Rate>()
                    .GroupBy(x => x.Zone)
                    .ToDictionary(x => x.Key, x => x.Average(r => r.Value ?? 0 / (decimal)(r.PriceList.Currency.LastRate * _CustExchangeRate)));
            }
            else
            {
                if (_OurRateCustomer == null)
                    _OurRateCustomer = new Dictionary<TABS.Zone, TABS.Rate>();
                if ((CheckCustomer == true && _LastSelectedCustomer != _SelectedCustomer.CarrierAccountID) || _OurRateCustomer.Count() == 0)
                {
                    var q = TABS.ObjectAssembler.CurrentSession
                    .CreateQuery("from Rate r where r.PriceList.Customer = :customer and " + currentFutureCondition);
                    q.SetParameter("customer", _SelectedCustomer)
                    .SetParameter("when", _Current ? DateTime.Now : DateTime.Today.AddDays(SystemIncreaseDays));

                    List<TABS.Rate> DublicateRatesObjects = q.List<TABS.Rate>().OrderBy(z => z.ZoneID).OrderByDescending(z => z.BeginEffectiveDate).ToList<TABS.Rate>();
                    int ZoneIDFirst = 0;
                    List<long> duplicateRates = new List<long>();
                    List<int> ZonesID = new List<int>();
                    foreach (TABS.Rate r in DublicateRatesObjects)
                    { if (ZonesID.Contains(r.ZoneID) == false) ZonesID.Add(r.ZoneID); }
                    foreach (int id in ZonesID)
                    {
                        ZoneIDFirst = 0;
                        foreach (TABS.Rate r in DublicateRatesObjects.Where(r => r.ZoneID == id).OrderByDescending(r => r.BeginEffectiveDate).ToList())
                        {
                            if (ZoneIDFirst == r.ZoneID)
                                duplicateRates.Add(r.ID);
                            ZoneIDFirst = r.ZoneID;
                        }
                    }
                    ourRatesCust = q.List<TABS.Rate>().Where(r => duplicateRates.Contains(r.ID) == false).ToDictionary(z => z.Zone);
                    _OurRateCustomer = ourRatesCust;
                }
                else
                    ourRatesCust = _OurRateCustomer;
            }
        }
        # endregion
        # region Functions
        private List<TABS.Rate> insurePolicy(List<TABS.Rate> supplyRates)
        {
            var rates = new List<TABS.Rate>();
            var ratesBySupplier = supplyRates.GroupBy(r => r.Supplier);
            foreach (var sr in ratesBySupplier)
            {
                TABS.Rate foundRate = sr.FirstOrDefault();
                switch (SelectedPolicy)
                {
                    case TABS.SupplierRatePolicy.Highest_Rate:
                        foreach (TABS.Rate r in sr)
                            if (r.Value > foundRate.Value)
                                foundRate = r;
                        rates.Add(foundRate);
                        break;
                    case TABS.SupplierRatePolicy.Lowest_Rate:
                        foreach (TABS.Rate r in sr)
                            if (r.Value < foundRate.Value)
                                foundRate = r;
                        rates.Add(foundRate);
                        break;
                    default:
                        rates.AddRange(sr);
                        break;
                }
            }
            return rates;
        }
        private decimal Convert(TABS.Rate r)
        {
            float custExchangeRate = _SelectedCustomer != null ? _SelectedCustomer.CarrierProfile.Currency.LastRate : 1;
            return r.Value.Value * (decimal)(custExchangeRate / r.PriceList.Currency.LastRate);
        }
        private bool InsureServices(short serviceFlag)
        {
            return (serviceFlag & FlaggedServicesSelection) == FlaggedServicesSelection;
        }
        private bool InsureServices(Dictionary<TABS.Zone, TABS.Rate> rates, TABS.Zone zone, short serviceFlag)
        {
            TABS.Rate saleRate;
            short minServiceFlag = FlaggedServicesSelection;
            //minServiceFlag = (minServiceFlag != 0) ? serviceFlag : minServiceFlag;
            if (rates.TryGetValue(zone, out saleRate))
            // minServiceFlag |= saleRate.ServicesFlag;
            {
                minServiceFlag = (minServiceFlag != 0) ? minServiceFlag : saleRate.ServicesFlag;
            }
            return (serviceFlag & minServiceFlag) == minServiceFlag;
        }
        private Dictionary<TABS.Zone, TABS.Rate> OurRateCustomer
        {
            get
            {
                if (_OurRateCustomer == null)
                    _OurRateCustomer = new Dictionary<TABS.Zone, TABS.Rate>();
                Dictionary<TABS.Zone, decimal> ourRatesNoCust = null;
                if ((CheckCustomer == true && _LastSelectedCustomer != _SelectedCustomer.CarrierAccountID) || _OurRateCustomer.Count() == 0)
                    GetOurRates(out ourRatesNoCust, out _OurRateCustomer);
                return _OurRateCustomer;
            }
            set { _OurRateCustomer = value; }
        }
        # endregion
        # region LCR By Zone
        private Dictionary<int, TABS.DTO.DTO_ZoneRate> GetLcrByZoneNoCustomer(Dictionary<TABS.Zone, List<TABS.Rate>> ratesByZone,
                                                                              Dictionary<TABS.Zone, decimal> ourRatesNoCust,
                                                                              List<string> supplierIds)
        {
            var BlockedSupplierZonesIDs = SupplierZoneBlocks;
            //List<TABS.RouteBlock> iraq = blocks.Where(z => z.Zone.Name == "Iraq-Baghdad").ToList();
            List<int> iraq = BlockedSupplierZonesIDs.Where(z => z == 102689).ToList();
            return
                ratesByZone
                .Where(d =>
                    (ourRatesNoCust.Count > 0 && ourRatesNoCust.ContainsKey(d.Key))
                //|| (supplierIds.Count == 0 && d.Value.Count > 0)
                //|| (d.Value.FirstOrDefault(sr => sr.Supplier.CarrierAccountID.IsIn(supplierIds.ToArray())) != null)
                    )// either we or the suppliers have rates for this zone
                .ToDictionary(d => d.Key.ZoneID, d => new TABS.DTO.DTO_ZoneRate()
                {
                    OurZone = d.Key,
                    Normal = ourRatesNoCust.Count > 0 && ourRatesNoCust.ContainsKey(d.Key) ? (double?)ourRatesNoCust[d.Key] : 0,
                    OurRate = TABS.Rate.None,
                    ServicesFlag = FlaggedServicesSelection,
                    SupplierRates = d.Value
                    .Where(r => (!r.Supplier.IsDeleted && r.Supplier.isActive && (r.Supplier.RoutingStatus == RoutingStatus.Enabled || r.Supplier.RoutingStatus == RoutingStatus.BlockedInbound)) && Convert(r) <= (decimal)_RateLessThan)//&& (supplierIds.Count == 0 || r.Supplier.CarrierAccountID.IsIn(supplierIds.ToArray())))
                    .Select(r => new TABS.DTO.DTO_SupplyRate()
                    {
                        Normal = (double?)Convert(r),
                        Supplier = r.Supplier,
                        EndEffectiveDate = r.EndEffectiveDate,
                        Zone = r.Zone,
                        ServicesFlag = r.ServicesFlag,
                        Rate = r
                    }
                    )
                    .Where(x => InsureServices(x.ServicesFlag) &&
                                  (IncludeBlockedZones
                               || (!IncludeBlockedZones && !BlockedSupplierZonesIDs.Contains(x.Zone.ZoneID))))//no specific customer, so no specific service flag
                    .OrderBy(x => x.Normal)
                    .ToList()
                });
        }
        private Dictionary<int, TABS.DTO.DTO_ZoneRate> GetLcrByZoneCustomer(Dictionary<TABS.Zone, List<TABS.Rate>> ratesByZone,
                                                                            Dictionary<TABS.Zone, TABS.Rate> ourRatesCust,
                                                                            List<string> supplierIds)
        {
            List<int> OurCustomerZonesID = OurRateCustomer.Keys.Select(k => k.ZoneID).ToList();
            var BlockedSupplierZonesIDs = SupplierZoneBlocks;
            if (!CheckCustomer)
                return ratesByZone
                   .Where(d =>
                       (ourRatesCust.Count > 0 && ourRatesCust.ContainsKey(d.Key))
                    //|| (supplierIds.Count == 0 && d.Value.Count > 0)
                    //|| (d.Value.FirstOrDefault(sr => sr.Supplier.CarrierAccountID.IsIn(supplierIds.ToArray())) != null)
                       )// either we or the suppliers have rates for this zone
                   .ToDictionary(d => d.Key.ZoneID, d => new TABS.DTO.DTO_ZoneRate()
                   {
                       OurZone = d.Key,
                       Normal = ourRatesCust.ContainsKey(d.Key) ? (double?)ourRatesCust[d.Key].Value / ourRatesCust[d.Key].PriceList.Currency.LastRate * _CustExchangeRate : 0,
                       OurRate = ourRatesCust.ContainsKey(d.Key) ? ourRatesCust[d.Key] : TABS.Rate.None,
                       ServicesFlag = (short)((ourRatesCust.ContainsKey(d.Key) ? ourRatesCust[d.Key].ServicesFlag : FlaggedServicesSelection) | FlaggedServicesSelection),
                       SupplierRates = d.Value
                       .Where(r => (!r.Supplier.IsDeleted && r.Supplier.isActive && (r.Supplier.RoutingStatus == RoutingStatus.Enabled || r.Supplier.RoutingStatus == RoutingStatus.BlockedInbound)) && Convert(r) <= (decimal)_RateLessThan)//&& (supplierIds.Count == 0 || r.Supplier.CarrierAccountID.IsIn(supplierIds.ToArray())))
                       .Select(r => new TABS.DTO.DTO_SupplyRate()
                       {
                           Normal = (double?)Convert(r),
                           Supplier = r.Supplier,
                           EndEffectiveDate = r.EndEffectiveDate,
                           Zone = r.Zone,
                           ServicesFlag = r.ServicesFlag,
                           Rate = r
                       }
                       )
                       .Where(x => InsureServices(ourRatesCust, d.Key, x.ServicesFlag) && !x.Supplier.CarrierAccountID.Equals(_SelectedCustomer.CarrierAccountID) &&
                           (IncludeBlockedZones
                              || (!IncludeBlockedZones && !BlockedSupplierZonesIDs.Contains(x.Zone.ZoneID))))
                       .OrderBy(x => x.Normal)
                       .ToList()
                   });
            else//get only the zones for the selected customer
                return
                    ratesByZone
                   .Where(d =>
                       (ourRatesCust.Count > 0 && ourRatesCust.ContainsKey(d.Key))
                    //&& ((supplierIds.Count == 0 && d.Value.Count > 0)
                    // || (d.Value.FirstOrDefault(sr => sr.Supplier.CarrierAccountID.IsIn(supplierIds.ToArray())) != null))
                       )
                   .ToDictionary(d => d.Key.ZoneID, d => new TABS.DTO.DTO_ZoneRate()
                   {
                       OurZone = d.Key,
                       Normal = ourRatesCust.ContainsKey(d.Key) ? (double?)ourRatesCust[d.Key].Value / ourRatesCust[d.Key].PriceList.Currency.LastRate * _CustExchangeRate : 0,
                       OurRate = ourRatesCust.ContainsKey(d.Key) ? ourRatesCust[d.Key] : TABS.Rate.None,
                       ServicesFlag = (short)((ourRatesCust.ContainsKey(d.Key) ? ourRatesCust[d.Key].ServicesFlag : FlaggedServicesSelection) | FlaggedServicesSelection),
                       SupplierRates = d.Value
                       .Where(r => (!r.Supplier.IsDeleted && r.Supplier.isActive && (r.Supplier.RoutingStatus == RoutingStatus.Enabled || r.Supplier.RoutingStatus == RoutingStatus.BlockedInbound)) && Convert(r) <= (decimal)_RateLessThan)//&& (supplierIds.Count == 0 || r.Supplier.CarrierAccountID.IsIn(supplierIds.ToArray())))
                       .Select(r => new TABS.DTO.DTO_SupplyRate()
                       {
                           Normal = (double?)Convert(r),
                           Supplier = r.Supplier,
                           EndEffectiveDate = r.EndEffectiveDate,
                           Zone = r.Zone,
                           ServicesFlag = r.ServicesFlag,
                           Rate = r
                       }
                       )
                           //.Where(x => InsureServices(ourRatesCust, d.Key, x.ServicesFlag) && !x.Supplier.CarrierAccountID.Equals(_SelectedCustomer.CarrierAccountID) &&
                           //    (IncludeBlockedZones
                           //       || (!IncludeBlockedZones && !BlockedSupplierZonesIDs.Contains(x.Zone.ZoneID))))
                       .Where(x => InsureServices(ourRatesCust, d.Key, x.ServicesFlag) && !x.Supplier.CarrierProfile.ProfileID.Equals(_SelectedCustomer.CarrierProfile.ProfileID)
                         &&
                             (IncludeBlockedZones
                             || //(!IncludeBlockedZones && !BlockedSupplierZonesIDs.Contains(x.Zone.ZoneID))))
                             !(IncludeBlockedZones || BlockedSupplierZonesIDs.Contains(x.Zone.ZoneID))))
                       .OrderBy(x => x.Normal)
                       .ToList()
                   });
        }
        # endregion
        # region Supplier Rates By Code
        private Dictionary<string, TABS.Components.RoutePool.CodeRoute> GetSupplierRatesByCode()
        {
            var result =
                    _SelectedPool.CodeRoutes
                    .Where(kvp =>  //kvp.Value.SupplyRates.Count > 0 && //has supplier rates

                         TargetFilterValue == null
                        ||
                        kvp.Value.SaleCode != null && System.Text.RegularExpressions.Regex.IsMatch(kvp.Value.SaleCode.Zone.Name ?? "", TargetFilterValue, System.Text.RegularExpressions.RegexOptions.IgnoreCase)
                        ||
                          System.Text.RegularExpressions.Regex.IsMatch(kvp.Key ?? "", TargetFilterValue, System.Text.RegularExpressions.RegexOptions.IgnoreCase)
                        ).ToDictionary(x => x.Key, x => x.Value);

            return result;
        }
        # endregion
        # region LCR By Code
        private Dictionary<string, TABS.DTO.DTO_CodeRate> GetLcrByCodeNoCustomer(Dictionary<string, TABS.Components.RoutePool.CodeRoute> ratesByCode,
                                                                                 Dictionary<TABS.Zone, decimal> ourRatesNoCust,
                                                                                 List<string> supplierIds)
        {
            var BlockedSupplierZonesIDs = SupplierZoneBlocks;
            var rates =
                ratesByCode
                    .Where(x => (ourRatesNoCust.Count > 0 && x.Value.SaleCode != null && ourRatesNoCust.ContainsKey(x.Value.SaleCode.Zone))
                //|| (supplierIds.Count == 0 && x.Value.SupplyRates.Count > 0)
                //|| (x.Value.SupplyRates.Values.FirstOrDefault(sr => sr.Supplier.CarrierAccountID.IsIn(supplierIds.ToArray())) != null)
                                )// either we or the suppliers have rates for this zone;
                    .ToDictionary(d => d.Key, d => new TABS.DTO.DTO_CodeRate()
                    {
                        OurCode = d.Key,
                        OurZone = d.Value.SaleCode == null ? TABS.Zone.UndefinedZone : d.Value.SaleCode.Zone,
                        Normal = d.Value.SaleCode != null && ourRatesNoCust.Count > 0 && ourRatesNoCust.ContainsKey(d.Value.SaleCode.Zone) ? (double?)ourRatesNoCust[d.Value.SaleCode.Zone] : null,
                        OurRate = TABS.Rate.None,
                        ServicesFlag = FlaggedServicesSelection,
                        SupplierRates = d.Value.SupplierRates
                        .Where(cr => (!cr.Supplier.IsDeleted && cr.Supplier.isActive && (cr.Supplier.RoutingStatus == RoutingStatus.Enabled || cr.Supplier.RoutingStatus == RoutingStatus.BlockedInbound)) && Convert(cr) <= (decimal)_RateLessThan)//&& (supplierIds.Count == 0 || cr.Value.Supplier.CarrierAccountID.IsIn(supplierIds.ToArray())))
                        .Select(cr => new TABS.DTO.DTO_SupplyRate()
                        {
                            Normal = (double?)Convert(cr),
                            Supplier = cr.Supplier,
                            EndEffectiveDate = cr.EndEffectiveDate,
                            Zone = cr.Zone,
                            ServicesFlag = cr.ServicesFlag,
                            Rate = cr
                        }
                        )
                        .Where(x =>
                            InsureServices(x.ServicesFlag)
                            &&
                            (IncludeBlockedZones
                               || (!IncludeBlockedZones && !BlockedSupplierZonesIDs.Contains(x.Zone.ZoneID)))
                            )//no specific customer, so no specific service flag
                        .OrderBy(x => x.Normal)
                        .ToList()
                    });
            return rates;
        }
        private Dictionary<string, TABS.DTO.DTO_CodeRate> GetLcrByCodeCustomer(Dictionary<string, TABS.Components.RoutePool.CodeRoute> ratesByCode,
                                                                               Dictionary<TABS.Zone, TABS.Rate> ourRatesCust,
                                                                               List<string> supplierIds)
        {
            var BlockedSupplierZonesIDs = SupplierZoneBlocks;
            var rates =
                ratesByCode
                    .Where(x =>
                        (ourRatesCust.Count > 0 && x.Value.SaleCode != null && ourRatesCust.ContainsKey(x.Value.SaleCode.Zone))
                //|| (supplierIds.Count == 0 && x.Value.SupplyRates.Count > 0)
                //|| (x.Value.SupplyRates.Values.FirstOrDefault(sr => sr.Supplier.CarrierAccountID.IsIn(supplierIds.ToArray())) != null)
                        )// either we or the suppliers have rates for this zone;
                    .ToDictionary(d => d.Key, d => new TABS.DTO.DTO_CodeRate()
                    {
                        OurCode = d.Key,
                        OurZone = d.Value.SaleCode == null ? TABS.Zone.UndefinedZone : d.Value.SaleCode.Zone,
                        Normal = d.Value.SaleCode != null && ourRatesCust.Count > 0 && ourRatesCust.ContainsKey(d.Value.SaleCode.Zone) ? (double?)ourRatesCust[d.Value.SaleCode.Zone].Value / ourRatesCust[d.Value.SaleCode.Zone].PriceList.Currency.LastRate * _CustExchangeRate : null,
                        OurRate = d.Value.SaleCode != null && ourRatesCust.ContainsKey(d.Value.SaleCode.Zone) ? ourRatesCust[d.Value.SaleCode.Zone] : TABS.Rate.None,
                        ServicesFlag = (short)((d.Value.SaleCode != null && ourRatesCust.ContainsKey(d.Value.SaleCode.Zone) ? ourRatesCust[d.Value.SaleCode.Zone].ServicesFlag : FlaggedServicesSelection) | FlaggedServicesSelection),
                        SupplierRates = d.Value.SupplierRates//.SupplyRates
                        .Where(cr => (!cr.Supplier.IsDeleted && cr.Supplier.isActive && (cr.Supplier.RoutingStatus == RoutingStatus.Enabled || cr.Supplier.RoutingStatus == RoutingStatus.BlockedInbound)) && Convert(cr) <= (decimal)_RateLessThan)//&& (supplierIds.Count == 0 || cr.Value.Supplier.CarrierAccountID.IsIn(supplierIds.ToArray())))
                        .Select(cr => new TABS.DTO.DTO_SupplyRate()
                        {
                            Normal = (double?)Convert(cr),
                            Supplier = cr.Supplier,
                            EndEffectiveDate = cr.EndEffectiveDate,
                            Zone = cr.Zone,
                            ServicesFlag = cr.ServicesFlag,
                            Rate = cr
                        }
                        )
                        .Where(x => InsureServices(ourRatesCust, d.Value.SaleCode != null ? d.Value.SaleCode.Zone : TABS.Zone.UndefinedZone, x.ServicesFlag) && !x.Supplier.CarrierProfile.ProfileID.Equals(_SelectedCustomer.CarrierProfile.ProfileID)
                         &&
                             (IncludeBlockedZones
                             || //(!IncludeBlockedZones && !BlockedSupplierZonesIDs.Contains(x.Zone.ZoneID))))
                             !(IncludeBlockedZones || BlockedSupplierZonesIDs.Contains(x.Zone.ZoneID))))
                            //.Where(x =>
                            //    InsureServices(ourRatesCust, d.Value.SaleCode != null ? d.Value.SaleCode.Zone : TABS.Zone.UndefinedZone, x.ServicesFlag)
                            //       && (IncludeBlockedZones || (!IncludeBlockedZones && !BlockedSupplierZonesIDs.Contains(x.Zone.ZoneID)))
                            //       && !x.Supplier.CarrierAccountID.Equals(_SelectedCustomer.CarrierAccountID)
                            //    )
                        .OrderBy(x => x.Normal)
                        .ToList()
                    });
            return rates;
        }
        # endregion
        # region BuildRoutes
        /// <summary>
        /// Build Routes
        /// </summary>
        /// <param name="time">"Current" or "Future"</param>
        /// <param name="customerID">CustomerID or null when no customer</param>
        /// <param name="byCode">Code or zone rates</param>
        /// <param name="flaggedServices">A short value e.g. zero for whole sale</param>
        /// <param name="filer">Code e.g. ".*" or Zone e.g. "%"</param>
        /// <param name="supplierRatePolicy">"None","Lowest_Rate" or "Highest_Rate" </param>
        /// <param name="includeSupplierZoneBlock">True in case of including supplier zone block</param>
        /// <returns>True if done</returns>
        public bool BuildRoutes(string time,
                                       string customerID,
                                       bool byCode,
                                       short flaggedServices,
                                       string filer,
                                       SupplierRatePolicy supplierRatePolicy,
                                       bool includeSupplierZoneBlock)
        {
            try
            {
                rblCF = time;
                CheckCustomer = (customerID != null);
                if (customerID != null)
                {
                    string CustomerID = customerID;
                    _SelectedCustomer = TABS.CarrierAccount.All[CustomerID];
                }
                rblByZoneCode = byCode ? "Code" : "Zone";
                bool _ByZone = !byCode;
                FlaggedServicesSelection = flaggedServices;
                txtFilter = filer;// ".*"
                SupplierRateDDl = supplierRatePolicy;
                IncludeSZB = includeSupplierZoneBlock;

                _supplierBlockedInZone = new Dictionary<int, List<string>>();

                //List<string> supplierIds = new List<string>();
                //supplierIds.AddRange(TABS.CarrierAccount.Suppliers.Select(s => s.CarrierAccountID));

                checkRate = false;
                ExcludeRateCondition = 0;
                _OurRateCustomer = null;
                string _LastLCRType = rblByZoneCode;

                TABS.Addons.Utilities.RatePoolChecker.CheckAndClear();
                Dictionary<TABS.Zone, decimal> ourRatesNoCust = null;
                Dictionary<TABS.Zone, TABS.Rate> ourRatesCust = null;
                GetOurRates(out ourRatesNoCust, out ourRatesCust);

                if (_ByZone)
                {
                    var ratesByZone_ = GetSupplierRatesByZone();
                    // CheckIfAllCodesInZoneIsBlocked(ratesByZone_.Keys.ToList());
                    Dictionary<TABS.Zone, List<TABS.Rate>> ratesByZone = new Dictionary<TABS.Zone, List<TABS.Rate>>();
                    if (SelectedPolicy != TABS.SupplierRatePolicy.Highest_Rate)
                    {
                        foreach (var item in ratesByZone_)
                            ratesByZone[item.Key] = insurePolicy(item.Value);
                    }
                    else
                        ratesByZone = ratesByZone_;

                    if (_SelectedCustomer == null)
                        //this.DataSource = GetLcrByZoneNoCustomer(ratesByZone, ourRatesNoCust, supplierIds);
                        this.DataSource = GetLcrByZoneNoCustomer(ratesByZone, ourRatesNoCust, null);
                    else
                        //this.DataSource = GetLcrByZoneCustomer(ratesByZone, ourRatesCust, supplierIds);
                        this.DataSource = GetLcrByZoneCustomer(ratesByZone, ourRatesCust, null);
                    //rgRates = this.DataSource.Values.OrderBy(x => x.OurZone.Name).ToList();

                    gvLCRByCode = null;
                    this.DataSourceByCode = null;
                }
                else
                {
                    var ratesByCode = GetSupplierRatesByCode();

                    if (_SelectedCustomer == null)
                        //this.DataSourceByCode = GetLcrByCodeNoCustomer(ratesByCode, ourRatesNoCust, supplierIds);
                        this.DataSourceByCode = GetLcrByCodeNoCustomer(ratesByCode, ourRatesNoCust, null);
                    else
                        //this.DataSourceByCode = GetLcrByCodeCustomer(ratesByCode, ourRatesCust, supplierIds);
                        this.DataSourceByCode = GetLcrByCodeCustomer(ratesByCode, ourRatesCust, null);

                    //gvLCRByCode = this.DataSourceByCode.Values.OrderBy(x => x.OurZone == null ? string.Empty : x.OurZone.Name).ToList();


                    rgRates = null;
                    this.DataSource = null;
                }
                return true;
            }
            catch (Exception exc)
            {
                return false;
            }
        }
        # endregion
    }
    # endregion
}
