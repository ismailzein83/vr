using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Linq;
namespace TABS
{
    /// <summary>
    /// The system code map. Provides an efficient and fast means of determining the current "Code" Supply... (Really fast).
    /// </summary>
    public class CodeMap : Interfaces.ICachedCollectionContainer, IDisposable
    {
        /// <summary>
        /// Return the current code map. if not already built, rebuilds it.
        /// </summary>
        public static CodeMap Current
        {
            get
            {
                lock (allSync)
                {
                    if (_Current == null)
                    {
                        log.Info("Building Code Map");
                        DateTime start = DateTime.Now;
                        _Current = new CodeMap(DateTime.Today);
                        TimeSpan spent = DateTime.Now.Subtract(start);
                        log.InfoFormat("Finished building Code Map, total time: {0}", spent);
                    }
                }
                return _Current;
            }
        }
        public static CodeMap CurrentCodeMap(CarrierAccount Customer, bool IncludeLossesSubCodes)
        {

            lock (allSync)
            {
                if (_Current == null || (_PrevCustomer != Customer && IncludeLossesSubCodes == true))
                {
                    log.Info("Building Code Map");
                    DateTime start = DateTime.Now;
                    _Current = null;//to release memory
                    _Current = new CodeMap(DateTime.Today, Customer, IncludeLossesSubCodes);
                    TimeSpan spent = DateTime.Now.Subtract(start);
                    log.InfoFormat("Finished building Code Map, total time: {0}", spent);
                }
            }

            return _Current;

        }
        /// <summary>
        /// Return the current code map. if not already built, rebuilds it.
        /// </summary>
        public static CodeMap CurrentOurCodes
        {
            get
            {
                lock (ourSync)
                {
                    if (_CurrentOurCodes == null)
                    {
                        log.Info("Building Our Code Map");
                        DateTime start = DateTime.Now;
                        string hql = "SELECT C FROM Code C WHERE (C.EndEffectiveDate IS NULL OR C.EndEffectiveDate > :effectiveSince) AND C.Zone.Supplier._CarrierAccountID = 'SYS' ORDER BY C.Value, C.BeginEffectiveDate DESC";
                        IList<TABS.Code> codes = TABS.DataConfiguration.CurrentSession.CreateQuery(hql)
                            .SetParameter("effectiveSince", DateTime.Today)
                            .List<TABS.Code>();
                        _CurrentOurCodes = new CodeMap(codes);
                        TimeSpan spent = DateTime.Now.Subtract(start);
                        _CurrentOurCodes.LoadingTime = spent;
                        log.InfoFormat("Finished building Our Code Map, total time: {0}", spent);
                    }
                }
                return _CurrentOurCodes;
            }
        }

        public DateTime StartDate { get; protected set; }
        public TimeSpan LoadingTime { get; protected set; }
        private static object _lock = new object();
        /// <summary>
        /// Needed for marker interface to be called by reflection
        /// </summary>
        public static void ClearCachedCollections()
        {
            _Current = null;
            _CurrentOurCodes = null;
            _PrevCustomer = null;
            log.Info("Current Code Maps set to null");
        }


        public override string ToString()
        {
            return string.Format("Code Map ({0}), Codes: {1}, Nodes: {2}", StartDate, CodeCount, NodeCount);
        }

        static log4net.ILog log = log4net.LogManager.GetLogger(typeof(CodeMap));
        static object allSync = new object();
        static object ourSync = new object();
        private static CodeMap _Current;
        private static CodeMap _CurrentOurCodes;

        protected int NodeCount { get { return SupplierCodes.Values.Sum(st => st.Count); } }
        public int CodeCount { get; protected set; }

        public Dictionary<TABS.CarrierAccount, Dictionary<string, List<Code>>> SupplierCodes { get; set; }//was be protected
        public Dictionary<TABS.CarrierAccount, List<string>> SupplierCodesStr { get; set; }
        public IEnumerable<TABS.CarrierAccount> Suppliers { get { return SupplierCodes.Keys; } }
        public HashSet<string> SaleCodes;
        public List<string> CustomerSaleCodes;
        //Exact Match Objects
        protected Dictionary<TABS.Zone, decimal> CustomerRates { get; set; }
        protected Dictionary<TABS.Zone, TABS.Rate> CustomerRate { get; set; }
        protected static CarrierAccount _PrevCustomer = null;
        public CodeMap(DateTime startDate)
        {
            InitSupplierCodes();
            this.StartDate = startDate;
            Build(this.StartDate);
        }
        public CodeMap(DateTime startDate, CarrierAccount Customer, bool IncludeLosses)
        {
            InitSupplierCodes();
            this.StartDate = startDate;
            Build(this.StartDate);
            List<CarrierAccount> ValidSupplier = SupplierCodes.Keys.Where(s => s.ActivationStatus != ActivationStatus.Inactive && s.RoutingStatus == RoutingStatus.Enabled || s.RoutingStatus == RoutingStatus.BlockedInbound).ToList();
            foreach (CarrierAccount supplier in ValidSupplier)
            {
                SupplierCodesStr.Add(supplier, SupplierCodes[supplier].Keys.ToList());
            }
            SaleCodes = new HashSet<string>(this.SupplierCodes[TABS.CarrierAccount.SYSTEM].Keys);
            _PrevCustomer = Customer;//for comparision on customer code map and customer change not for data usage
            CustomerRate = new Dictionary<Zone, Rate>();
            CustomerRates = new Dictionary<Zone, decimal>();
            CustomerSaleCodes = new List<string>();
            Dictionary<Zone, decimal> _CustomerRates = new Dictionary<Zone, decimal>();
            if (IncludeLosses == true)
            {
                CustomerRate = GetCustomerRates(Customer, out _CustomerRates);
                CustomerRates = _CustomerRates;
                if (CustomerRate != null)
                    CustomerRate.Keys.ToList().ForEach(z => { if (z.EffectiveCodes != null)CustomerSaleCodes.AddRange(z.EffectiveCodes.Select(c => c.Value)); });
                _CustomerRates = null;
            }

        }
        public CodeMap()
            : this(DateTime.Parse("1990-01-01"))
        {
        }
        public static IList<CarrierAccount> SysAccounts
        {
            get
            {
                int SysProfileId = CarrierAccount.SYSTEM.CarrierProfile.ProfileID;
                List<CarrierAccount> SysAccounts = TABS.CarrierProfile.All[SysProfileId].Accounts.ToList();
                SysAccounts = SysAccounts.Where(a => a.AccountType != AccountType.Client).ToList();
                return SysAccounts;
            }
        }
        public IList<string> GetAllCodes(IEnumerable<TABS.CarrierAccount> suppliers)
        {
            HashSet<string> codes = new HashSet<string>();
            foreach (var supplier in suppliers)
            {
                foreach (var code in GetCodes(supplier))
                    codes.Add(code.Value);
            }
            var codeList = codes.ToList();
            codeList.Sort();
            return codeList;
        }

        public List<TABS.Code> GetCodes(TABS.CarrierAccount supplier)
        {
            List<TABS.Code> codes = new List<TABS.Code>();
            Dictionary<string, List<Code>> supplierCodes = null;
            if (supplier != null)
                if (SupplierCodes.TryGetValue(supplier, out supplierCodes))
                {
                    foreach (var codeList in supplierCodes.Values)
                        codes.AddRange(codeList);
                }
            return codes;
        }

        /// <summary>
        /// Find the best matching Code for the given supplier in the Code Map.
        /// </summary>
        /// <param name="code">The destination code (or dialed number)</param>
        /// <param name="supplier">The supplier</param>
        /// <returns>A Code object or null if not found</returns>
        public TABS.Code Find(string code, TABS.CarrierAccount supplier, DateTime whenEffective)
        {
            if (string.IsNullOrEmpty(code)) return null;
            TABS.Code found = null;
            Dictionary<string, List<Code>> supplierCodes = null;
            if (SupplierCodes.TryGetValue(supplier, out supplierCodes))
            {
                List<Code> matchingCodes = null;
                StringBuilder codeValue = new StringBuilder(GetDigits(code));
                while (found == null && codeValue.Length > 0)
                {

                    if (supplierCodes.TryGetValue(codeValue.ToString(), out matchingCodes))
                    {
                        foreach (TABS.Code possibleCode in matchingCodes)
                        {
                            if (possibleCode.IsEffectiveOn(whenEffective))
                            {
                                found = possibleCode;
                                bool IsCodeGroup = TABS.CodeGroup.All.Keys.Contains(codeValue.ToString());
                                if (found.Zone.IsHaveMatchingCodeGroup == false)
                                    found.Zone.IsHaveMatchingCodeGroup = IsCodeGroup;
                                found.Zone.IsCodeGroup = IsCodeGroup;
                                break;
                            }
                        }
                    }
                    codeValue.Length--;
                }
            }
            return found;
        }
        private Dictionary<TABS.Zone, TABS.Rate> GetCustomerRates(CarrierAccount Customer, out Dictionary<TABS.Zone, decimal> ourRatesNoCust)
        {
            string currentFutureCondition = "r.BeginEffectiveDate <= :when and (r.EndEffectiveDate is null or r.EndEffectiveDate >= :when)";

            ourRatesNoCust = null;
            Dictionary<TABS.Zone, TABS.Rate> ourRatesCust = null;
            if (Customer == null)
            {
                var SystemIncreaseDays = (double)(decimal)TABS.SystemConfiguration.KnownParameters[TABS.KnownSystemParameter.sys_BeginEffectiveRateDays].Value;

                ourRatesNoCust = null;

                var q = TABS.ObjectAssembler.CurrentSession
                    .CreateQuery("from Rate r where r.PriceList.Supplier = :sys and " + currentFutureCondition)
                    .SetParameter("sys", TABS.CarrierAccount.SYSTEM)
                    .SetParameter("when", this.StartDate);

                float currency = (TABS.CarrierAccount.SYSTEM.CarrierProfile.Currency.LastRate == null) ? 1 : TABS.CarrierAccount.SYSTEM.CarrierProfile.Currency.LastRate;
                ourRatesNoCust = q
                    .List<TABS.Rate>()
                    .GroupBy(x => x.Zone)
                    .ToDictionary(x => x.Key, x => x.Average(r => r.Value ?? 0 / (decimal)(r.PriceList.Currency.LastRate * currency)));//_CustExchangeRate
                return ourRatesCust;
            }
            else
            {


                var q = TABS.ObjectAssembler.CurrentSession
                .CreateQuery("from Rate r where r.PriceList.Customer = :customer and " + currentFutureCondition);
                q.SetParameter("customer", Customer)
                .SetParameter("when", this.StartDate);

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
                ourRatesNoCust = null;
                return ourRatesCust;
            }
        }
        private List<TABS.Code> CodeChilds(string Code, CarrierAccount supplier)
        {
            List<Code> Childs = new List<Code>();
            Dictionary<string, List<Code>> supplierCodes = null;
            Dictionary<string, List<Code>> _supplierCodes = null;

            string _Code = Code;
            int len = _Code.Length;
            if (SupplierCodes.TryGetValue(supplier, out supplierCodes))
            {
                _supplierCodes = supplierCodes;

                //_supplierCodes.Keys.Where(k => k.Length >= len && k.StartsWith(_Code)).ToList()
                // .ForEach(key => { Childs.AddRange(_supplierCodes[key]); });
                //IList (_Array and for) is fastest
                //IEnumerable<string> keys = _supplierCodes.Keys.OrderBy(s => s.Length);
                //keys = keys.Where(k => k.Length >= len);
                //keys = keys.Where(k => k.StartsWith(_Code));
                //keys = keys.Except(SaleCodes); 

                foreach (string k in _supplierCodes.Keys)
                {
                    //if (k.Length < len || !k.StartsWith(_Code) ) continue;
                    if (k.Length < len || !(k.Substring(0, len).Equals(_Code)) || SaleCodes.Contains(k)) continue;

                    Childs.AddRange(_supplierCodes[k]);
                }

            }
            return Childs;
        }
        private List<TABS.Code> CodeChilds2(string Code, CarrierAccount supplier)
        {
            List<Code> Childs = new List<Code>();
            Dictionary<string, List<Code>> _supplierCodes = null;
            HashSet<string> Keys = new HashSet<string>(SupplierCodesStr[supplier]);
            // Dictionary<string, string> Keys = SupplierCodesStr[supplier].ToDictionary(s => s, v => v);
            //Keys.Sort();
            //int len = Keys.Count;
            // int index = Keys.BinarySearch(Code);
            // if (index < 0)
            //     return Childs;
            // List<string> _Keys = Keys.GetRange(index, len - 1 - index);
            string _Code = Code;
            int len = _Code.Length;
            if (SupplierCodes.TryGetValue(supplier, out _supplierCodes))
            {
                string _K;
                foreach (string k in Keys)
                {
                    _K = k.ToString();
                    //if (_K.Length < len || (_K.Substring(0, len) != Code) || SaleCodes.BinarySearch(_K) > 0) continue;
                    if (_K.Length < len || _K.IndexOf(Code) < 0 || SaleCodes.Contains(_K) == true) continue;
                    Childs.Add(_supplierCodes[_K][0]);

                }


            }
            return Childs;
        }
        public TABS.Code FindUsingExactMatch(string code, TABS.CarrierAccount supplier, TABS.CarrierAccount customer, TABS.Zone saleZone, DateTime whenEffective, TABS.Components.RoutePool routepool, bool IncludeLossesSubCodes, bool IsCurentPool, out bool IsExactCodeMatche, out bool IsHaveOneSubCodeCauseLoss)
        {
            IsExactCodeMatche = false;
            IsHaveOneSubCodeCauseLoss = false;
            if (string.IsNullOrEmpty(code)) return null;

            TABS.Code found = null;
            Dictionary<string, List<Code>> supplierCodes = null;
            decimal saleRateValue = 0;
            TABS.Rate InitialPurchaseRate = null;
            decimal InitialPurchaseRateValue = 0;
            TABS.Rate saleRate = null;
            if (IncludeLossesSubCodes == true)
            {
                if (customer != null)
                {
                    this.CustomerRate.TryGetValue(saleZone, out saleRate);
                    saleRateValue = (saleRate != null) ? (decimal)saleRate.Value : -1;
                }
                else
                    this.CustomerRates.TryGetValue(saleZone, out saleRateValue);
            }
            bool iscustomerexists = false;
            bool isSupplierexistsforcustomer = false;
            if (IncludeLossesSubCodes == true)
            {
                if (IsCurentPool == true)
                {
                    lock (_lock)
                    {
                        iscustomerexists = (TABS.Components.RoutePool.ExactMatchCodeSuupliers.Keys.Contains(customer.CarrierAccountID)) ? true : false;
                        isSupplierexistsforcustomer = (iscustomerexists == true && TABS.Components.RoutePool.ExactMatchCodeSuupliers[customer.CarrierAccountID.ToString()].Keys.Contains(code)) ? true : false;
                        isSupplierexistsforcustomer = (isSupplierexistsforcustomer == true && TABS.Components.RoutePool.ExactMatchCodeSuupliers[customer.CarrierAccountID.ToString()][code].Contains(supplier)) ? true : false;
                    }
                }
                else
                {
                    lock (_lock)
                    {
                        iscustomerexists = (TABS.Components.RoutePool.FutureExactMatchCodeSuupliers.Keys.Contains(customer.CarrierAccountID)) ? true : false;
                        isSupplierexistsforcustomer = (iscustomerexists == true && TABS.Components.RoutePool.FutureExactMatchCodeSuupliers[customer.CarrierAccountID.ToString()].Keys.Contains(code)) ? true : false;
                        isSupplierexistsforcustomer = (isSupplierexistsforcustomer == true && TABS.Components.RoutePool.FutureExactMatchCodeSuupliers[customer.CarrierAccountID.ToString()][code].Contains(supplier)) ? true : false;
                    }
                }
            }
            if (SupplierCodes.TryGetValue(supplier, out supplierCodes))
            {

                List<Code> matchingCodes = null;
                StringBuilder codeValue = new StringBuilder(GetDigits(code));
                int Initiallen = codeValue.Length;
                if (supplierCodes.TryGetValue(codeValue.ToString(), out matchingCodes))
                {
                    foreach (TABS.Code possibleCode in matchingCodes)
                    {
                        if (possibleCode.IsEffectiveOn(whenEffective))
                        {
                            IsExactCodeMatche = true; found = possibleCode;
                            found.Zone.IsHaveMatchingCodeGroup = (found.Zone.IsHaveMatchingCodeGroup == false && TABS.CodeGroup.All.Keys.Contains(codeValue.ToString()));
                            found.Zone.IsCodeGroup = TABS.CodeGroup.All.Keys.Contains(codeValue.ToString());
                        }
                        if (IsExactCodeMatche == true && IncludeLossesSubCodes == true && (iscustomerexists == false || isSupplierexistsforcustomer == false))//&& (InitialPurchaseRateValue == 0 || (InitialPurchaseRateValue < saleRateValue))
                        {
                            List<Code> childs = CodeChilds2(code, supplier);//new List<Code>();  ;
                            foreach (Code ccode in childs)
                            {
                                if (IsHaveOneSubCodeCauseLoss == true)
                                    break;
                                StringBuilder ccodeValue = new StringBuilder(GetDigits(ccode.Value));
                                ccodeValue.Length--;
                                while (ccodeValue.Length >= Initiallen)
                                {
                                    if (ccodeValue.Length == Initiallen)
                                    {
                                        TABS.Rate purchaseRate = null;
                                        if (saleZone != null && routepool.SupplyRates.TryGetValue(ccode.Zone, out purchaseRate) && saleRateValue >= 0)
                                        {
                                            if (purchaseRate.Value > saleRateValue)
                                            {
                                                IsHaveOneSubCodeCauseLoss = true;
                                                if (IsCurentPool == true)
                                                {
                                                    lock (_lock)
                                                    {
                                                        Dictionary<string, List<CarrierAccount>> CustomerExactMatchCodeSuupliers = null;
                                                        if (TABS.Components.RoutePool.ExactMatchCodeSuupliers.TryGetValue(customer.CarrierAccountID, out CustomerExactMatchCodeSuupliers) == true)
                                                            CustomerExactMatchCodeSuupliers = TABS.Components.RoutePool.ExactMatchCodeSuupliers[customer.CarrierAccountID];
                                                        else
                                                        {
                                                            CustomerExactMatchCodeSuupliers = new Dictionary<string, List<CarrierAccount>>();
                                                            TABS.Components.RoutePool.ExactMatchCodeSuupliers.Add(customer.CarrierAccountID, CustomerExactMatchCodeSuupliers);
                                                        }
                                                        List<CarrierAccount> LossesSupplier = null;
                                                        if (CustomerExactMatchCodeSuupliers.TryGetValue(code, out LossesSupplier) == false)
                                                        {
                                                            LossesSupplier = new List<CarrierAccount>();
                                                            LossesSupplier.Add(supplier);
                                                            CustomerExactMatchCodeSuupliers.Add(code, LossesSupplier);
                                                        }
                                                        else
                                                        {
                                                            LossesSupplier = CustomerExactMatchCodeSuupliers[code];
                                                            LossesSupplier.Add(supplier);
                                                            CustomerExactMatchCodeSuupliers[code] = LossesSupplier;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    lock (_lock)
                                                    {
                                                        Dictionary<string, List<CarrierAccount>> CustomerExactMatchCodeSuupliers = null;
                                                        if (TABS.Components.RoutePool.FutureExactMatchCodeSuupliers.TryGetValue(customer.CarrierAccountID, out CustomerExactMatchCodeSuupliers) == true)
                                                            CustomerExactMatchCodeSuupliers = TABS.Components.RoutePool.FutureExactMatchCodeSuupliers[customer.CarrierAccountID];
                                                        else
                                                        {
                                                            CustomerExactMatchCodeSuupliers = new Dictionary<string, List<CarrierAccount>>();
                                                            TABS.Components.RoutePool.FutureExactMatchCodeSuupliers.Add(customer.CarrierAccountID, CustomerExactMatchCodeSuupliers);
                                                        }
                                                        List<CarrierAccount> LossesSupplier = null;
                                                        if (CustomerExactMatchCodeSuupliers.TryGetValue(code, out LossesSupplier) == false)
                                                        {
                                                            LossesSupplier = new List<CarrierAccount>();
                                                            LossesSupplier.Add(supplier);
                                                            CustomerExactMatchCodeSuupliers.Add(code, LossesSupplier);
                                                        }
                                                        else
                                                        {
                                                            LossesSupplier.Add(supplier);
                                                            CustomerExactMatchCodeSuupliers[code] = LossesSupplier;
                                                        }
                                                    }


                                                }
                                                break;
                                            }
                                        }
                                    }
                                    if (ccodeValue.Length != Initiallen && this.SupplierCodes[TABS.CarrierAccount.SYSTEM].Keys.Contains(ccodeValue.ToString())) //&& this.SupplierCodes[supplier].Keys.Contains(codeValue.ToString())
                                        break;
                                    ccodeValue.Length--;
                                }
                            }

                        }
                    }



                }
            }
            return found;
        }

        protected CodeMap(IEnumerable<TABS.Code> codes)
        {
            InitSupplierCodes();
            Generate(codes);
        }

        private void InitSupplierCodes()
        {
            this.SupplierCodes = new Dictionary<CarrierAccount, Dictionary<string, List<Code>>>();
            this.SupplierCodesStr = new Dictionary<CarrierAccount, List<string>>();
            foreach (TABS.CarrierAccount supplier in TABS.CarrierAccount.All.Values)
            {
                this.SupplierCodes[supplier] = new Dictionary<string, List<Code>>();
            }
        }

        protected void Build(DateTime effectiveSince)
        {
            GC.Collect();
            var stopWatch = new TABS.Addons.Utilities.StopWatch();
            stopWatch.StartTiming();
            List<TABS.Code> allCodes = new List<TABS.Code>();
            var allCarriers = TABS.ObjectAssembler.GetList<TABS.CarrierAccount>().ToDictionary(c => c.CarrierAccountID);
            Dictionary<int, TABS.Zone> zones = new Dictionary<int, TABS.Zone>();
            // TABS.DataConfiguration.OpenSession().Connection
            //using (var sqlConnection = (SqlConnection)DataConfiguration.Default.SessionFactory.ConnectionProvider.GetConnection())
            //{
            using (var sqlConnection = new System.Data.SqlClient.SqlConnection(DataConfiguration.Default.Properties["connection.connection_string"].ToString()))//hibernate.connection.connection_string
            {
                sqlConnection.Open();
                string sqlQuery = string.Format(@"
                    SELECT 
                        C.ID, 
                        C.Code, 
                        C.BeginEffectiveDate, 
                        C.EndEffectiveDate,
                        C.ZoneID, 
                        Z.CodeGroup, 
                        Z.Name,
                        Z.SupplierID,
                        Z.ServicesFlag,
                        Z.BeginEffectiveDate as ZoneBED,
                        Z.EndEffectiveDate as ZoneEED
                    FROM Code C WITH(NOLOCK), Zone Z WITH(NOLOCK),CarrierAccount CA
                    WHERE C.ZoneID = Z.ZoneID 
                        AND Z.SupplierID = CA.CarrierAccountID AND CA.ActivationStatus = 2
                        AND (C.EndEffectiveDate IS NULL OR (C.EndEffectiveDate > '{0:yyyy-MM-dd}' And C.BeginEffectiveDate<>C.EndEffectiveDate)) 
                        AND (Z.EndEffectiveDate IS NULL OR (Z.EndEffectiveDate > '{0:yyyy-MM-dd}' And Z.BeginEffectiveDate<>Z.EndEffectiveDate)) 
                    ORDER BY Z.SupplierID, C.Code, C.BeginEffectiveDate DESC",
                        effectiveSince);

                SqlCommand command = new SqlCommand(sqlQuery, sqlConnection);
                command.CommandTimeout = 0;
                var reader = command.ExecuteReader(CommandBehavior.CloseConnection);
                int resultcount = 0;
                while (reader.Read())
                {
                    resultcount++;
                    int index = -1;
                    var code = new TABS.Code()
                    {
                        ID = reader.GetInt64(++index),
                        Value = reader.GetString(++index),
                        BeginEffectiveDate = reader.IsDBNull(++index) ? null : (DateTime?)reader.GetDateTime(index),
                        EndEffectiveDate = reader.IsDBNull(++index) ? null : (DateTime?)reader.GetDateTime(index)
                    };

                    int zoneID = reader.GetInt32(++index);

                    TABS.Zone zone = null;
                    if (!zones.TryGetValue(zoneID, out zone))
                    {
                        zone = new TABS.Zone()
                        {
                            ZoneID = zoneID,
                            CodeGroup = reader.IsDBNull(++index) ? TABS.CodeGroup.None : TABS.CodeGroup.All[reader.GetString(index)],
                            Name = reader.GetString(++index),
                            Supplier = allCarriers[reader.GetString(++index)],
                            ServicesFlag = reader.GetInt16(++index),
                            BeginEffectiveDate = reader.IsDBNull(++index) ? null : (DateTime?)reader.GetDateTime(index),
                            EndEffectiveDate = reader.IsDBNull(++index) ? null : (DateTime?)reader.GetDateTime(index)
                        };
                        zones.Add(zoneID, zone);
                    }

                    code.Zone = zone;
                    allCodes.Add(code);
                }
            }
            Generate(allCodes);
            this.LoadingTime = stopWatch.StopTiming();
        }

        protected string GetDigits(string codeValue)
        {
            StringBuilder sb = new StringBuilder(codeValue.Length);
            for (int i = 0; i < codeValue.Length; i++)
            {
                char c = codeValue[i];
                if (char.IsDigit(c)) sb.Append(c);
            }
            return sb.ToString();
        }

        protected void Generate(IEnumerable<TABS.Code> codes)
        {
            foreach (TABS.Code code in codes)
            {
                CodeCount++;
                if (code.Zone.Supplier.IsDeleted) continue;
                Dictionary<string, List<Code>> supplierCodes = SupplierCodes[code.Zone.Supplier];
                string codeValue = GetDigits(code.Value);
                List<Code> codeList = null;
                if (!supplierCodes.TryGetValue(codeValue, out codeList))
                {
                    codeList = new List<Code>();
                    supplierCodes[codeValue] = codeList;
                }
                codeList.Add(code);
            }
        }

        #region Static

        #region Helper Classes
        internal class CodeMapSupplier
        {
            internal static Dictionary<string, CodeMapSupplier> All = new Dictionary<string, CodeMapSupplier>(100);
            internal string ID = null;
            internal Tst.TstDictionary Codes = new Tst.TstDictionary();

            internal static CodeMapSupplier Get(IDataReader reader, int index)
            {
                CodeMapSupplier supplier = null;
                string ID = reader.GetString(index);
                if (!All.TryGetValue(ID, out supplier))
                {
                    supplier = new CodeMapSupplier();
                    supplier.ID = ID;
                    All.Add(ID, supplier);
                }
                return supplier;
            }

            public override string ToString()
            {
                return ID;
            }
        }

        internal class MappedCode : IComparable<MappedCode>
        {
            internal static long TotalCount = 0;
            internal static Dictionary<string, List<string>> All = new Dictionary<string, List<string>>();

            internal long ID;
            internal int ZoneID;
            internal string Value;
            internal CodeMapSupplier Supplier;

            internal MappedCode(IDataReader reader)
            {
                int index = -1;
                index++; this.ID = reader.GetInt64(index);
                index++; this.ZoneID = reader.GetInt32(index);
                index++; this.Supplier = CodeMapSupplier.Get(reader, index);
                index++; this.Value = reader.GetString(index);
                TotalCount++;
                if (!this.Supplier.Codes.ContainsKey(this.Value))
                    this.Supplier.Codes.Add(this.Value, this);
            }

            internal static MappedCode Add(IDataReader reader)
            {
                MappedCode code = new MappedCode(reader);
                List<string> subCodes = null;
                if (!All.TryGetValue(code.Value, out subCodes))
                {
                    subCodes = new List<string>(code.Value.Length);
                    int L = code.Value.Length;
                    while (L > 0)
                    {
                        subCodes.Add(code.Value.Substring(0, L));
                        L--;
                    }
                    All.Add(code.Value, subCodes);
                }
                return code;
            }

            #region IComparable<Code> Members

            public int CompareTo(MappedCode other)
            {
                return this.Value.CompareTo(other.Value);
            }

            #endregion

            public override string ToString()
            {
                return string.Concat(Supplier, ":", Value);
            }
        }

        #endregion Helper Classes

        static Nullable<TimeSpan> _LastBuildTime;
        static Nullable<int> _LastBuildCount;

        public static Nullable<TimeSpan> LastBuildTime { get { return _LastBuildTime; } }
        public static Nullable<int> LastBuildCount { get { return _LastBuildCount; } }

        private static void LoadCodes(SqlConnection connection)
        {
            // Clear previous values if any
            CodeMapSupplier.All.Clear();
            MappedCode.All.Clear();

            // Get the distinct codes from all suppliers including our own
            SqlCommand command = connection.CreateCommand();
            command.CommandText = string.Format(@"
                        SELECT DISTINCT 
                                C.ID, 
                                C.ZoneID, 
                                Z.SupplierID, 
                                C.Code 
                            FROM Code C WITH(NOLOCK), Zone Z WITH(NOLOCK)
                            WHERE 
                                    C.ZoneID = Z.ZoneID 
                                AND C.IsEffective='Y' 
                                AND Z.IsEffective='Y' 
                                AND Z.SupplierID IN 
                                    (
                                        SELECT CarrierAccountID 
                                            FROM CarrierAccount ca  WITH(NOLOCK)
                                            WHERE 
                                                    ca.IsDeleted = 'N' 
                                                AND ca.ActivationStatus IN ({0}, {1})
                                                AND ca.RoutingStatus IN ({2}, {3})
                                    )
                        "
                            , (byte)ActivationStatus.Active, (byte)ActivationStatus.Testing
                            , (byte)RoutingStatus.Enabled, (byte)RoutingStatus.BlockedInbound
                         );

            SqlDataReader reader = command.ExecuteReader();

            // Read all codes
            while (reader.Read())
                MappedCode.Add(reader);

            // close reader
            reader.Close();
            reader.Dispose();
        }
        private static void LoadCodesForCodeComparison(SqlConnection connection)
        {
            //for code comparision
            CodeMapSupplier.All.Clear();
            MappedCode.All.Clear();

            // Get the distinct codes from all suppliers including our own
            SqlCommand command = connection.CreateCommand();
            command.CommandText = string.Format(@"
                        SELECT DISTINCT 
                                C.ID, 
                                C.ZoneID, 
                                Z.SupplierID, 
                                C.Code 
                            FROM Code C WITH(NOLOCK), Zone Z WITH(NOLOCK)
                            WHERE 
                                    C.ZoneID = Z.ZoneID 
                                AND C.EndEffectiveDate IS NULL   
                                AND Z.EndEffectiveDate IS NULL
                                AND Z.SupplierID IN 
                                    (
                                        SELECT CarrierAccountID 
                                            FROM CarrierAccount ca  WITH(NOLOCK)
                                            WHERE 
                                                    ca.IsDeleted = 'N' 
                                                AND ca.ActivationStatus IN ({0}, {1})
                                                AND ca.RoutingStatus IN ({2}, {3})
                                    )
                        "
                            , (byte)ActivationStatus.Active, (byte)ActivationStatus.Testing
                            , (byte)RoutingStatus.Enabled, (byte)RoutingStatus.BlockedInbound, DateTime.Now
                         );

            SqlDataReader reader = command.ExecuteReader();

            // Read all codes
            while (reader.Read())
                MappedCode.Add(reader);

            // close reader
            reader.Close();
            reader.Dispose();
        }

        /// <summary>
        /// Run the Code Map matching algorithm and Build the map.
        /// When Done the whole map is saved to the database. This could take a considerable 
        /// amount of time depending on: Processor Speed, Server-Db connection speed, DB server speed.
        /// </summary>
        /// <param name="ExecutionTime">The execution time</param>
        /// <returns></returns>
        public static int Build(out TimeSpan ExecutionTime)
        {
            DateTime start = DateTime.Now;

            // Get a connection and make sure it is open
            SqlConnection connection = (SqlConnection)DataHelper.GetOpenConnection();
            // If not connected
            if (connection.State != ConnectionState.Open)
                connection.Open();

            // Load Supplier Codes
            LoadCodes(connection);

            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Code", typeof(string));
            dataTable.Columns.Add("SupplierCodeID", typeof(long));
            dataTable.Columns.Add("SupplierZoneID", typeof(int));
            dataTable.Columns.Add("SupplierID", typeof(string));

            // Delete all previous matches
            SqlCommand command = connection.CreateCommand();
            command.CommandTimeout = 1000;

            // Empty Code Matchin table and drop indexes for faster data insertion
            command.Parameters.Clear();
            command.CommandText = @"TRUNCATE TABLE CodeMatch;";
            command.ExecuteNonQuery();
            command.CommandText = @"DROP INDEX CodeMatch.IDX_CodeMatch_Code;                                    
						            DROP INDEX CodeMatch.IDX_CodeMatch_Zone;
						            DROP INDEX CodeMatch.IDX_CodeMatch_Supplier;
                                    ";
            try { command.ExecuteNonQuery(); }
            catch { }

            // Loop through all the codes we have 
            foreach (string codeValue in MappedCode.All.Keys)
            {
                List<string> subCodes = MappedCode.All[codeValue];

                // Loop for all suppliers (including us: <SYS>)
                foreach (CodeMapSupplier supplier in CodeMapSupplier.All.Values)
                {
                    MappedCode code = null;

                    foreach (string subCodeValue in subCodes)
                        if (supplier.Codes.ContainsKey(subCodeValue))
                        {
                            code = (MappedCode)supplier.Codes[subCodeValue];
                            break;
                        }

                    if (code != null)
                    {
                        DataRow row = dataTable.NewRow();
                        row[0] = codeValue;
                        row[1] = code.ID;
                        row[2] = code.ZoneID;
                        row[3] = supplier.ID;
                        dataTable.Rows.Add(row);
                    }
                }
            }

            // Copy -- really fast
            SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.ReadUncommitted);
            SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.TableLock, transaction);
            sqlBulkCopy.BulkCopyTimeout = 600; // 10 Minutes.. 
            sqlBulkCopy.DestinationTableName = "CodeMatch";
            sqlBulkCopy.WriteToServer(dataTable);
            transaction.Commit();
            _LastBuildCount = dataTable.Rows.Count;
            dataTable.Dispose();
            dataTable = null;

            // Recreate Indexes
            command.Parameters.Clear();
            command.CommandText = @"CREATE INDEX IDX_CodeMatch_Code ON CodeMatch(Code);									
									CREATE INDEX IDX_CodeMatch_Zone ON CodeMatch(SupplierZoneID);
									CREATE INDEX IDX_CodeMatch_Supplier ON CodeMatch(SupplierID);";
            command.ExecuteNonQuery();

            // Build Zone Match
            command.Parameters.Clear();
            command.CommandText =
                @"TRUNCATE TABLE ZoneMatch;
                  INSERT INTO ZoneMatch (OurZoneID, SupplierZoneID)
	                            SELECT DISTINCT OC.SupplierZoneID, SC.SupplierZoneID 
		                            FROM CodeMatch OC WITH(NOLOCK), CodeMatch SC WITH(NOLOCK)
		                            WHERE 
                                            OC.Code = SC.Code 
                                        AND OC.SupplierID = @SystemAccountID 
                                        AND SC.SupplierID <> @SystemAccountID
                                        ";

            DataHelper.AddParameter(command, "@SystemAccountID", CarrierAccount.SystemAccountID);
            command.ExecuteNonQuery();

            connection.Close();
            connection.Dispose();

            ExecutionTime = DateTime.Now.Subtract(start);

            _LastBuildTime = ExecutionTime;

            GC.Collect();

            return _LastBuildCount.Value;
        }
        public static int BuildForCodeComparison(out TimeSpan ExecutionTime)
        {
            DateTime start = DateTime.Now;

            // Get a connection and make sure it is open
            SqlConnection connection = (SqlConnection)DataHelper.GetOpenConnection();
            // If not connected
            if (connection.State != ConnectionState.Open)
                connection.Open();

            // Load Supplier Codes
            LoadCodesForCodeComparison(connection);

            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Code", typeof(string));
            dataTable.Columns.Add("MatchingCode", typeof(string));
            dataTable.Columns.Add("SupplierCodeID", typeof(long));
            dataTable.Columns.Add("SupplierZoneID", typeof(int));
            dataTable.Columns.Add("SupplierID", typeof(string));

            // Delete all previous matches
            SqlCommand command = connection.CreateCommand();
            command.CommandTimeout = 1000;

            // Empty Code Matchin table and drop indexes for faster data insertion
            command.Parameters.Clear();
            command.CommandText = @"IF   EXISTS (SELECT * FROM sys.objects 
                    WHERE object_id = OBJECT_ID(N'CodeMatchForCodeComparison') AND type in (N'U'))
                    drop  table [dbo].[CodeMatchForCodeComparison]

                    CREATE TABLE [dbo].[CodeMatchForCodeComparison](
	                    [Code] [varchar](15) NOT NULL,
	                    [MatchingCode] [varchar](25) NOT NULL,
	                    [SupplierCodeID] [bigint] NOT NULL,
	                    [SupplierZoneID] [int] NOT NULL,
	                    [SupplierID] [varchar](5) NULL
                    )";
            command.ExecuteNonQuery();

            // Loop through all the codes we have 
            foreach (string codeValue in MappedCode.All.Keys)
            {
                List<string> subCodes = MappedCode.All[codeValue];

                // Loop for all suppliers (including us: <SYS>)
                foreach (CodeMapSupplier supplier in CodeMapSupplier.All.Values)
                {
                    MappedCode code = null;

                    foreach (string subCodeValue in subCodes)
                        if (supplier.Codes.ContainsKey(subCodeValue))
                        {
                            code = (MappedCode)supplier.Codes[subCodeValue];
                            break;
                        }

                    if (code != null)
                    {
                        DataRow row = dataTable.NewRow();
                        row[0] = codeValue;
                        row[1] = code.Value;
                        row[2] = code.ID;
                        row[3] = code.ZoneID;
                        row[4] = supplier.ID;
                        dataTable.Rows.Add(row);
                    }
                }
            }

            // Copy -- really fast
            SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.ReadUncommitted);
            SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.TableLock, transaction);
            sqlBulkCopy.BulkCopyTimeout = 600; // 10 Minutes.. 
            sqlBulkCopy.DestinationTableName = "CodeMatchForCodeComparison";
            sqlBulkCopy.WriteToServer(dataTable);
            transaction.Commit();
            _LastBuildCount = dataTable.Rows.Count;
            dataTable.Dispose();
            dataTable = null;

            // Recreate Indexes
            command.Parameters.Clear();
            command.CommandText = @"CREATE INDEX IDX_CodeMatch_Code ON CodeMatchForCodeComparison(Code);									
									CREATE INDEX IDX_CodeMatch_Zone ON CodeMatchForCodeComparison(SupplierZoneID);
									CREATE INDEX IDX_CodeMatch_Supplier ON CodeMatchForCodeComparison(SupplierID);";
            command.ExecuteNonQuery();

            connection.Close();
            connection.Dispose();

            ExecutionTime = DateTime.Now.Subtract(start);

            _LastBuildTime = ExecutionTime;

            GC.Collect();

            return _LastBuildCount.Value;
        }

        #endregion Static

        #region IDisposable Members

        public void Dispose()
        {
            if (SupplierCodes != null)
                SupplierCodes.Clear();
            if (SupplierCodesStr != null)
                SupplierCodesStr.Clear();
            if (SaleCodes != null)
                SaleCodes.Clear();
            SaleCodes = null;
            SupplierCodes = null; SupplierCodesStr = null;
            GC.Collect(); GC.Collect();
        }

        #endregion

    }
}
