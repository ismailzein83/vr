using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace TABS.Components
{
    public class CdrProcessor
    {
        public class Range
        {
            public int Start { get; protected set; }
            public int End { get; protected set; }
            public int ValidCount { get; set; }
            public Range(int start, int end) { this.Start = start; this.End = end; }
            public bool IsValid { get { return Start >= 0 && End >= Start; } }
            public override string ToString()
            {
                return string.Format("{0} - {1}", Start, End);
            }
        }

        log4net.ILog log;
        PricingGenerator generator;
        CodeMap codeMap;
        List<CDR> cdrs;
        List<Billing_CDR_Base> billingCdrs;
        bool generateTrafficStats;
        bool autoFlushStats;
        Dictionary<string, TrafficStats> trafficStats;
        private static Dictionary<string, TrafficStats> _DailyTrafficStats;
        //private static Dictionary<string, TrafficStats> _DailyOriginatingTrafficStats;
        CdrRepricingParameters parameters;

        protected CdrProcessor(CdrRepricingParameters parameters, log4net.ILog log, PricingGenerator generator, CodeMap codeMap, List<CDR> cdrs, List<Billing_CDR_Base> billingCdrs, Dictionary<string, TrafficStats> trafficStats, bool autoFlushStats)
        {
            // Copy Processing Parties
            this.log = log;
            this.generator = generator;
            this.codeMap = codeMap;
            this.cdrs = cdrs;
            this.billingCdrs = billingCdrs;
            this.generateTrafficStats = parameters.GenerateTrafficStats;
            this.autoFlushStats = autoFlushStats;
            this.trafficStats = trafficStats;
            this.parameters = parameters;
        }

        /// <summary>
        /// Launch Range Processing Threads according to the count specified. Best is 2 for Quad Core Servers.
        /// </summary>
        /// <param name="threadCount"></param>
        protected int LaunchAndWaitForRangeThreads(int threadCount)
        {
            if (threadCount < 1) threadCount = 1;

            Dictionary<System.Threading.Thread, Range> threads = new Dictionary<System.Threading.Thread, Range>();

            int start = 0;
            int rangeCount = cdrs.Count / threadCount;
            int diff = cdrs.Count % threadCount == 0 ? 0 : 1;
            for (int i = 0; i < threadCount; i++)
            {
                int end = start + rangeCount - 1 + diff;
                if (end >= cdrs.Count) end = cdrs.Count - 1;
                Range range = new Range(start, end);
                //SupplierTimeShift
                System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(this.Process));
                threads[thread] = range;
                thread.Start(range);
                start = end + 1;
            }

            foreach (var thread in threads.Keys) thread.Join();
            int validCount = threads.Values.Sum(r => r.ValidCount);
            foreach (var thread in threads.Keys)
                if (thread.ThreadState == System.Threading.ThreadState.Stopped)
                    thread.Abort();


            threads = null;
            return validCount;
        }

        public static void HandlePassThrough(Billing_CDR_Main cdr)
        {
            if (cdr.Customer.IsPassThroughCustomer && cdr.Billing_CDR_Cost != null)
            {
                var sale = new Billing_CDR_Sale();
                cdr.Billing_CDR_Sale = sale;
                sale.Copy(cdr.Billing_CDR_Cost);
                sale.Zone = cdr.OurZone;
            }
            if (cdr.Supplier.IsPassThroughSupplier && cdr.Billing_CDR_Sale != null)
            {
                var cost = new Billing_CDR_Cost();
                cdr.Billing_CDR_Cost = cost;
                cost.Copy(cdr.Billing_CDR_Sale);
                cost.Zone = cdr.SupplierZone;
            }
        }
        //public static string For_Testing = string.Empty;
        protected void Process(object rangeObj)
        {
            DateTime start = DateTime.Now;
            try
            {
                Range range = rangeObj as Range;

                // Not a valid range? return
                if (range == null || !range.IsValid) return;
                start = DateTime.Now;
                int validCount = 0;
                log.Info("Start Pricing Cdrs @" + start);
                // for each CDR
                for (int i = range.Start; i <= range.End; i++)
                {
                    CDR cdr = cdrs[i];
                    // Create Billing CDRs for this and add to list
                    //Billing_CDR_Base billingCDR = GenerateBillingCdr(codeMap, ref validCount, cdr);
                    Billing_CDR_Base billingCDR = GenerateBillingCdr(codeMap, ref validCount, cdr, log);
                    billingCdrs[i] = billingCDR;

                    // Generate Cost and Sale
                    if (billingCDR.IsValid)
                    {

                        Billing_CDR_Main main = (Billing_CDR_Main)billingCDR;
                        main.Billing_CDR_Cost = generator.GetRepricing<Billing_CDR_Cost>(main);
                        main.Billing_CDR_Sale = generator.GetRepricing<Billing_CDR_Sale>(main);
                        if (main.Billing_CDR_Sale == null)
                            generator.FixParentCodeSale(main, null, null, codeMap);
                        HandlePassThrough(main);


                        if (main != null && main.Billing_CDR_Cost != null && main.SupplierCode != null)
                            main.Billing_CDR_Cost.Code = main.SupplierCode;

                        if (main != null && main.Billing_CDR_Sale != null && main.OurCode != null)
                            main.Billing_CDR_Sale.Code = main.OurCode;
                    }
                }
                log.Info("End Pricing Cdrs @" + DateTime.Now);
                range.ValidCount = validCount;
            }
            catch (Exception ex)
            {
                log.Error(string.Format("Error Processing Range: ", rangeObj), ex);
            }
            finally
            {
                TimeSpan spent = DateTime.Now.Subtract(start);
                log.InfoFormat("Successfully Processed Billing Info for CDR Range: {0} in {1}", rangeObj, spent);
            }
        }

        protected DateTime start = DateTime.Now;
        protected void StartTiming() { start = DateTime.Now; }
        protected TimeSpan StopTiming() { return DateTime.Now.Subtract(start); }

        /// <summary>
        /// Run the CDR processor and return the last ID processed.
        /// The Number of parallel processing Threads in this Processor is defined in the Known System Parameter PricingThreadCount.
        /// </summary>
        /// <returns>The last Processed CDR id</returns>
        public long Run()
        {
            int threadCount = (int)SystemParameter.PricingThreadCount.NumericValue;

            // Prepare Billing CDRs
            billingCdrs.Capacity = cdrs.Count;
            for (int i = 0; i < cdrs.Count; i++) billingCdrs.Add(null);

            DateTime Start = DateTime.Now;
            log.InfoFormat("Start pricing by Launching Range Processing Threads according to the count specified. @", Start);
            // Launch a thread for each Range SupplierTimeShift
            int valid = LaunchAndWaitForRangeThreads(threadCount);
            log.InfoFormat("End pricing by Launching Range Processing Threads according to the count specified. @", DateTime.Now.Subtract(start));
            log.InfoFormat("Processed {0} cdrs ({1} main, {2} invalid)", cdrs.Count, valid, cdrs.Count - valid);

            start = DateTime.Now;
            log.InfoFormat("Start Removing non wanted billing cdrs if a particular customer or supplier is defined(This is required if the CDRs are not mapped to the customer/supplier) @", DateTime.Now);
            // Remove non wanted billing cdrs if a particular customer or supplier is defined 
            // This is required if the CDRs are not mapped to the customer/supplier 
            if (parameters.Customer != null || parameters.Supplier != null)
            {
                var filteredBillingCdrs = new List<Billing_CDR_Base>(billingCdrs.Capacity);
                foreach (var cdr in billingCdrs)
                {
                    // Check customer
                    if (parameters.Customer != null && !parameters.Customer.Equals(cdr.Customer))
                        continue;
                    // Check Supplier
                    if (parameters.Supplier != null && !parameters.Supplier.Equals(cdr.Supplier))
                        continue;
                    // No problem
                    filteredBillingCdrs.Add(cdr);
                }
                if (filteredBillingCdrs.Count != billingCdrs.Count)
                {
                    billingCdrs.Clear();
                    billingCdrs.AddRange(filteredBillingCdrs);
                }
            }

            log.InfoFormat("End Removing non wanted billing cdrs if a particular customer or supplier is defined(This is required if the CDRs are not mapped to the customer/supplier) Takes:", DateTime.Now.Subtract(Start));
            // Save Billing Info
            StartTiming();
            log.InfoFormat("Start Save Billing Info @", DateTime.Now);
            Components.BulkManager billingBulkManager = null;
            using (billingBulkManager = new BulkManager(log))
                billingBulkManager.Write(billingCdrs, true);
            billingBulkManager.Dispose();
            billingBulkManager = null;
            Components.BulkManager.Clear();
            var spent = StopTiming();
            log.InfoFormat("Successfully Saved Billing Info for {0} Billing CDRs and {1} Invalid in {2}", valid, cdrs.Count - valid, spent);

            int SampleMinutes = (int)(60 / SystemParameter.TrafficStatsSamplesPerHour.NumericValue.Value);

            // Update Traffic Stats if necessary
            StartTiming();
            log.InfoFormat("Start Updating Stats @", DateTime.Now);
            Start = DateTime.Now;
            log.InfoFormat("Start Updating Traffic Stats @", DateTime.Now);
            if (generateTrafficStats)
                Engine.UpdateTrafficStats(SampleMinutes, trafficStats, billingCdrs, autoFlushStats);
            log.InfoFormat("End Updating Traffic Stats takes", DateTime.Now.Subtract(start));
            //if (generateTrafficStats)
            Start = DateTime.Now;
            log.InfoFormat("Start Updating Daily Traffic Stats @", DateTime.Now);
            Engine.UpdateDailyTrafficStats(_DailyTrafficStats, billingCdrs, autoFlushStats);
            log.InfoFormat("End Updating Daily Traffic Stats takes", DateTime.Now.Subtract(start));
            //if (generateTrafficStats)
            //Engine.UpdateDailyOriginatingTrafficStats(_DailyOriginatingTrafficStats, billingCdrs, autoFlushStats);


            spent = StopTiming();
            log.InfoFormat("Successfully Updated Stats for {0} Billing CDRs and {1} Invalid in {2}", valid, cdrs.Count - valid, spent);

            return cdrs[cdrs.Count - 1].CDRID;
        }

        internal static System.Text.RegularExpressions.Regex InvalidCGPNDigits = new System.Text.RegularExpressions.Regex("[^0-9]", System.Text.RegularExpressions.RegexOptions.Compiled);

        /// <summary>
        /// Get the Minimum and Maximum IDs for CDRs in the date/time range from - till.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="till"></param>
        /// <param name="minID"></param>
        /// <param name="maxID"></param>
        protected static void GetCdrIdLimits(DateTime from, DateTime till, ref long minID, ref long maxID)
        {
            using (IDbConnection connection = SecondaryDataHelper.GetOpenConnection())
            {
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = @"SELECT @MinID = ISNULL(Min(CDRID), -1), @MaxID = ISNULL(Max(CDRID), -1) FROM CDR WITH(NOLOCK, INDEX(IX_CDR_AttemptDateTime)) WHERE AttemptDateTime BETWEEN @From AND @Till";
                    DataHelper.AddParameter(command, "@MinID", minID).Direction = ParameterDirection.InputOutput;
                    DataHelper.AddParameter(command, "@MaxID", maxID).Direction = ParameterDirection.InputOutput;
                    DataHelper.AddParameter(command, "@From", from);
                    DataHelper.AddParameter(command, "@Till", till);
                    command.ExecuteNonQuery();
                    minID = long.Parse(((IDataParameter)command.Parameters["@MinID"]).Value.ToString());
                    maxID = long.Parse(((IDataParameter)command.Parameters["@MaxID"]).Value.ToString());
                }
            }

        }
        /// <summary>
        /// Process CDRs from for the given period (start-end) | switch | customer | suppplier, in batches.
        /// </summary>
        /// <param name="startDate">The date to start with</param>
        /// <param name="endDate">The date to end with</param>
        /// <param name="selectedSwitch">The selected switch, if any (null means all)</param>
        /// <param name="customerID">The selected customer ID, if any (null means all)</param>
        /// <param name="supplierID">The selected supplier ID, if any (null means all)</param>
        /// <param name="batchSize">The batch processing size (recommended 25000-50000)</param>
        /// <param name="dailyChunks">The Number of chunks based on which to split a day for processing.</param>
        /// <param name="generateTrafficStats">False to process only CDRs with Durations > 0</param>
        public static void ExecuteRepricing(string Parameters)
        {
            try
            {
                char[] Sep = { ';' };
                CdrRepricingParameters _parameter = new CdrRepricingParameters();
                string[] Values = Parameters.Split(Sep);
                _parameter.From = DateTime.Parse(Values[0]);
                _parameter.Till = DateTime.Parse(Values[1]);
                if (!string.IsNullOrEmpty(Values[2]))
                    _parameter.SelectedSwitch = TABS.Switch.All[int.Parse(Values[2])];
                _parameter.Customer = (!string.IsNullOrEmpty(Values[3]) && TABS.CarrierAccount.All.ContainsKey(Values[3])) ? TABS.CarrierAccount.All[Values[3]] : null;
                _parameter.Supplier = (!string.IsNullOrEmpty(Values[4]) && TABS.CarrierAccount.All.ContainsKey(Values[4])) ? TABS.CarrierAccount.All[Values[4]] : null;
                _parameter.BatchSize = string.IsNullOrEmpty(Values[5]) ? 50000 : int.Parse(Values[5]);
                _parameter.DailyChunks = string.IsNullOrEmpty(Values[6]) ? 24 : int.Parse(Values[6]);
                _parameter.GenerateTrafficStats = string.IsNullOrEmpty(Values[7]) ? true : int.Parse(Values[7]) == 1 ? true : false;
                Process(_parameter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Process CDRs from for the given period (start-end) | switch | customer | suppplier, in batches.
        /// </summary>
        /// <param name="startDate">The date to start with</param>
        /// <param name="endDate">The date to end with</param>
        /// <param name="selectedSwitch">The selected switch, if any (null means all)</param>
        /// <param name="customerID">The selected customer ID, if any (null means all)</param>
        /// <param name="supplierID">The selected supplier ID, if any (null means all)</param>
        /// <param name="batchSize">The batch processing size (recommended 25000-50000)</param>
        /// <param name="dailyChunks">The Number of chunks based on which to split a day for processing.</param>
        /// <param name="generateTrafficStats">False to process only CDRs with Durations > 0</param>
        public static void Process(CdrRepricingParameters parameters)
        {
            // ensure last configuration (case of error updating a switch)
            // TABS.Switch.ClearCachedCollections();

            Engine.RepricingThread = System.Threading.Thread.CurrentThread;
            Engine.RepricingParameters = parameters;
            DateTime startDate = parameters.From.Date;
            DateTime endDate = parameters.Till.Date;
            Switch selectedSwitch = parameters.SelectedSwitch;
            string customerID = parameters.Customer == null ? null : parameters.Customer.CarrierAccountID;
            string supplierID = parameters.Supplier == null ? null : parameters.Supplier.CarrierAccountID;
            int batchSize = parameters.BatchSize;
            TimeSpan dailyProcessingTime = parameters.DailyChunkTime;
            bool generateTrafficStats = parameters.GenerateTrafficStats;

            parameters.Started = DateTime.Now;

            if (batchSize < 10000) batchSize = 10000;
            //if (batchSize > 100000) batchSize = 100000;

            TABS.CarrierAccount customer = customerID != null && TABS.CarrierAccount.All.ContainsKey(customerID) ? TABS.CarrierAccount.All[customerID] : null;
            TABS.CarrierAccount supplier = supplierID != null && TABS.CarrierAccount.All.ContainsKey(supplierID) ? TABS.CarrierAccount.All[supplierID] : null;

            log4net.ILog log = log4net.LogManager.GetLogger(Engine.RepricingLoggerName);
            string LicenceKey = string.Empty;
            if (!Vanrise.Components.Security.LicenceManagerControl.CheckLicence(out LicenceKey))
            {
                log.Info("TOne Repricing WCF service Licence was expired(key=" + LicenceKey + ")");
                return;
            }
            log.InfoFormat("Regenerating Billing CDRs. From: {0}, Till: {1}, Switch: {2}, Customer: {3}, Supplier: {4}, Batch Size: {5:#,###}, Chunk: {6}, Traffic Stats: {7}", startDate, endDate, selectedSwitch, customer, supplier, batchSize, dailyProcessingTime, generateTrafficStats);

            DateTime start = DateTime.Now; TimeSpan spent = TimeSpan.Zero;

            bool inconsistentData = false;

            try
            {
                start = DateTime.Now;
                //AddDays(-1) added for the StartDate to be Updated with Rates codes in codeMap with the SupplierTimeShift
                using (CodeMap codeMap = new CodeMap(startDate.AddDays(-1)))
                {
                    spent = DateTime.Now.Subtract(start);
                    log.InfoFormat("Built {0} in {1}", codeMap, spent);

                    // using (NHibernate.ISession session = DataConfiguration.OpenSession())
                    start = DateTime.Now;
                    PricingGenerator pricingGenerator;
                    //AddDays(-1) added for the StartDate to be Updated with Rates codes in _Rates in new PricingGenerator with the SupplierTimeShift
                    using (NHibernate.ISession session = DataConfiguration.OpenSession())
                    {
                        pricingGenerator = new PricingGenerator(session, startDate, startDate.AddDays(-1));
                        session.Flush();
                        session.Close();
                    }
                    spent = DateTime.Now.Subtract(start);
                    log.InfoFormat("Built Pricing Generator in {0}", spent);
                    int batchCount = 0;

                    DateTime allStart = DateTime.Now;

                    List<CDR> cdrs = new List<CDR>(batchSize);
                    List<Billing_CDR_Base> billingCDRs = new List<Billing_CDR_Base>(batchSize);
                    Dictionary<string, TrafficStats> _TrafficStats = new Dictionary<string, TrafficStats>();
                    _DailyTrafficStats = new Dictionary<string, TrafficStats>();
                    // _DailyOriginatingTrafficStats = new Dictionary<string, TrafficStats>();
                    #region Build Query
                    int? switchID = selectedSwitch == null ? null : (int?)selectedSwitch.SwitchID;
                    StringBuilder sb = new StringBuilder(
                        @"SELECT TOP " + batchSize + "\n" + string.Join(", ", CDR.CdrEnumerator.ReaderFields) + "\n"
                         + @" FROM CDR WITH(NOLOCK, INDEX(IX_CDR_AttemptDateTime)) 
                            WHERE 
                                CDRID >= @minID 
                            AND AttemptDateTime BETWEEN @from AND @till ");

                    if (!generateTrafficStats) sb.Append(" AND DurationInSeconds > 0 ");
                    if (switchID != null) sb.AppendFormat(" AND SwitchID = {0} \r\n", switchID);

                    // Add Customer Conditions
                    if (customer != null)
                    {
                        StringBuilder subConditions = new StringBuilder();
                        foreach (TABS.Switch definedSwitch in TABS.Switch.All.Values)
                        {
                            if (switchID == null || switchID == definedSwitch.SwitchID)
                            {
                                List<string> identifiers = new List<string>();
                                if (definedSwitch.SwitchManager == null)
                                { log.InfoFormat("Such Switch may have error on switch configuration or it is not found as dll {0}", definedSwitch.SwitchID); continue; }
                                if (!InterfaceChekerManager.IsImplementationOf(definedSwitch.SwitchManager.GetType(), typeof(TABS.Extensibility.ISwitchManagerExtended)))
                                    identifiers = definedSwitch.SwitchManager.GetCustomerCdrIdentifiers(definedSwitch, customer);
                                else
                                    identifiers = (definedSwitch.SwitchManager as TABS.Extensibility.ISwitchManagerExtended).GetCustomerCdrIdentifiersRepricing(definedSwitch, customer);
                                if (identifiers == null) { log.InfoFormat("Such Switch may have error on Customer Identification retreival {0}", definedSwitch.SwitchID); continue; }
                                if (identifiers == null) { log.Error(string.Format("Warning:Such Switch {0}-{1} have an error in Customer Cdr Identifiers", definedSwitch.SwitchID, definedSwitch.Name)); continue; }
                                if (subConditions.Length > 0) subConditions.Append(" OR ");
                                if (identifiers != null && identifiers.Count > 0)
                                    subConditions.AppendFormat("(SwitchID = {0} AND IN_CARRIER IN ('{1}'))", definedSwitch.SwitchID, string.Join("','", identifiers.ToArray()));
                                else
                                    subConditions.AppendFormat("(SwitchID = {0} AND 1 = 0)", definedSwitch.SwitchID);

                            }
                        }
                        if (subConditions.Length > 0) sb.AppendFormat(" AND ({0}) \r\n", subConditions);
                    }

                    // Add Supplier Conditions
                    if (supplier != null)
                    {
                        StringBuilder subConditions = new StringBuilder();
                        foreach (TABS.Switch definedSwitch in TABS.Switch.All.Values)
                        {
                            if (switchID == null || switchID == definedSwitch.SwitchID)
                            {
                                List<string> identifiers = new List<string>();
                                if (definedSwitch.SwitchManager == null)
                                { log.InfoFormat("Such Switch may have error on switch configuration or it is not found as dll {0}", definedSwitch.SwitchID); continue; }
                                if (!InterfaceChekerManager.IsImplementationOf(definedSwitch.SwitchManager.GetType(), typeof(TABS.Extensibility.ISwitchManagerExtended)))
                                    identifiers = definedSwitch.SwitchManager.GetSupplierCdrIdentifiers(definedSwitch, supplier);
                                else
                                    identifiers = (definedSwitch.SwitchManager as TABS.Extensibility.ISwitchManagerExtended).GetSupplierCdrIdentifiersRepricing(definedSwitch, supplier);
                                if (identifiers == null) { log.InfoFormat("Such Switch may have error on Supplier Identification retreival {0}", definedSwitch.SwitchID); continue; }
                                if (subConditions.Length > 0) subConditions.Append(" OR ");
                                if (identifiers != null && identifiers.Count > 0)
                                    subConditions.AppendFormat("(SwitchID = {0} AND OUT_CARRIER IN ('{1}'))", definedSwitch.SwitchID, string.Join("','", identifiers.ToArray()));
                                else
                                    subConditions.AppendFormat("(SwitchID = {0} AND 1 = 0)", definedSwitch.SwitchID);
                            }
                        }
                        if (subConditions.Length > 0) sb.AppendFormat(" AND ({0}) \r\n", subConditions);
                    }

                    sb.Append(" ORDER BY CDRID ");

                    string sql = sb.ToString();

                    #endregion Build Query

                    // Reprice for every day...
                    for (DateTime processedDay = startDate; processedDay <= endDate; processedDay = processedDay.AddDays(1))
                    {
                        inconsistentData = false;

                        // If Stop is requested, break...
                        if (parameters.IsStopRequested) { log.Error(string.Format("Stop Request Detected. Aborting Day {0:yyyy-MM-dd}.", processedDay)); break; }

                        // Check if CDR Import Is running and Repricing for today!
                        if (processedDay.Equals(DateTime.Today))
                        {
                            if (Engine.IsCDRImportRunning)
                            {
                                log.Error(string.Format("Cannot Process Repricing for Today ({0}) while CDR Import is running", processedDay));
                                break;
                            }
                            // CDR Import not running: clear Traffic Stats Cache
                            // This is a fix to the StaleState Exception.
                            else
                            {
                                Engine._TrafficStatsCache.Clear();
                            }
                        }

                        DateTime dayStart = DateTime.Now;

                        #region Clean Billing Info

                        // Clean the Billing and Traffic Stats for the selected CDRs
                        log.InfoFormat("Cleaning Billing Info for: {0}. Switch ID: {1}, Customer: {2}, Supplier: {3}",
                            processedDay,
                            switchID,
                            customer,
                            supplier);
                        start = DateTime.Now;

                        inconsistentData = true;

                        SecondaryDataHelper.ExecuteNonQuery(@"EXEC bp_CleanBillingAndStats 
                            @From = @P1, 
                            @Till = @P2, 
                            @SwitchID = @P3, 
                            @CustomerID = @P4, 
                            @SupplierID = @P5, 
                            @IncludeTrafficStats = @P6",
                                processedDay,
                                processedDay,
                                switchID,
                                customer == null ? null : customer.CarrierAccountID,
                                supplier == null ? null : supplier.CarrierAccountID,
                                generateTrafficStats ? "Y" : "N");

                        spent = DateTime.Now.Subtract(start);
                        log.InfoFormat("Cleaned Billing Info for: {0}. Switch ID: {1}, Customer: {2}, Supplier: {3}. Time: {4}",
                            processedDay,
                            switchID,
                            customer,
                            supplier,
                            spent);
                        #endregion Clean Billing Info

                        start = DateTime.Now;
                        GC.Collect();
                        spent = DateTime.Now.Subtract(start);
                        log.InfoFormat("Performed Garbage Collection in {0}", spent);

                        DateTime nextDate = processedDay.AddDays(1);

                        // Clear Traffic Stats Cache if needed
                        if (generateTrafficStats)
                            _TrafficStats.Clear();
                        _DailyTrafficStats.Clear();
                        long cdrCount = 0;

                        // Daily Chunks Loop
                        for (DateTime from = processedDay.Date; from < nextDate; from = from.Add(dailyProcessingTime))
                        {
                            DateTime till = from.Add(dailyProcessingTime).AddMilliseconds(-3.3);
                            if (till > nextDate.AddMilliseconds(-3.3)) till = nextDate.AddMilliseconds(-3.3);

                            // If Stop is requested, break...
                            if (parameters.IsStopRequested) { log.Error(string.Format("Stop Request Detected. Aborting Chunk {0}-{1}.", from, till)); break; }

                            long minID = -1;
                            long maxID = -1;

                            GetCdrIdLimits(from, till, ref minID, ref maxID);
                            StringBuilder fullLog = new StringBuilder();
                            // Batch Loop
                            do
                            {
                                fullLog = new StringBuilder();
                                batchCount++;
                                string message = string.Format("Processing Date {0:yyyy-MM-dd} - Batch #{1}, Chunk: {2} - {3}", processedDay, batchCount, from.TimeOfDay, till.TimeOfDay);
                                fullLog.Append(message).Append(" * ");
                                //log.Info(message);

                                // If Stop is requested, break...
                                if (parameters.IsStopRequested) { log.InfoFormat("Stop Request Detected. Aborting Batch {0}", batchCount); break; }

                                DateTime batchStart = DateTime.Now;

                                // Clear previous Batch
                                cdrs.Clear();
                                billingCDRs.Clear();

                                // Load Batch CDRs
                                start = DateTime.Now;
                                using (IDbConnection connection = TABS.SecondaryDataHelper.GetOpenConnection())
                                {
                                    using (IDbCommand command = connection.CreateCommand())
                                    {
                                        command.CommandText = sql;
                                        command.CommandTimeout = 0;
                                        DataHelper.AddParameter(command, "@minID", minID, DbType.Int64);
                                        DataHelper.AddParameter(command, "@from", from, DbType.DateTime);
                                        DataHelper.AddParameter(command, "@till", till, DbType.DateTime);
                                        var result = CDR.Get(command);
                                        foreach (CDR cdr in result)
                                            cdrs.Add(cdr);
                                    }
                                }
                                spent = DateTime.Now.Subtract(start);
                                message = string.Format("Loaded {0:#,###} CDRs in {1}", cdrs.Count, spent);
                                fullLog.Append(message).Append(" * ");
                                //log.InfoFormat(message);
                                cdrCount += cdrs.Count;

                                if (cdrs.Count > 0)
                                {
                                    // Process the Batch
                                    start = DateTime.Now;

                                    CdrProcessor processor = new CdrProcessor(parameters, log, pricingGenerator, codeMap, cdrs, billingCDRs, _TrafficStats, false);
                                    //SupplierTimeShift
                                    minID = processor.Run();

                                    // Finished?
                                    if (minID < maxID) minID++;
                                    else minID = 0;

                                    spent = DateTime.Now.Subtract(start);
                                    message = string.Format("Generated Billing Info for CDRs ({0} - {1}, count: {2:#,###}). Batch #{3}. Time: {4}", cdrs[0].CDRID, cdrs[cdrs.Count - 1].CDRID, cdrs.Count, batchCount, spent);
                                    fullLog.Append(message).Append(" * ");
                                    //log.InfoFormat(message);

                                    // Sleep 
                                    System.Threading.Thread.Sleep(1);
                                }
                                else
                                {
                                    minID = 0;
                                }

                                spent = DateTime.Now.Subtract(batchStart);
                                message = string.Format("Fully Processed {0:yyyy-MM-dd} [{1:HH:mm}-{2:HH:mm}] - Batch #{3} ({4:#,###} Cdrs). Time: {5}", processedDay, from, till, batchCount, cdrs.Count, spent);
                                //fullLog.Append(message);
                                // log.InfoFormat(message);
                                log.Info(fullLog.ToString());
                            } while (minID > 0);

                        }

                        log.InfoFormat(
                            "Finished Regenerating Billing CDRs. Date: {0:yyyy-MM-dd}. CDRs processed: {1:#,###}{2}",
                                processedDay,
                                cdrCount,
                                generateTrafficStats ?
                                    string.Format(", Minutes: {0:#,##0.00}", _TrafficStats.Sum(s => s.Value.DurationsInSeconds / 60.0m))
                                    :
                                    ""
                                );

                        // Traffic Statistics for this Day
                        if (!parameters.IsStopRequested)
                        {
                            if (generateTrafficStats)
                            {
                                start = DateTime.Now;

                                log.InfoFormat("Cleaning Traffic Stats for Date: {0}", processedDay);
                                lock (typeof(TrafficStats))
                                {
                                    SecondaryDataHelper.ExecuteNonQuery(@"EXEC bp_CleanTrafficStats 
                                                                @From = @P1, 
                                                                @Till = @P2, 
                                                                @SwitchID = @P3, 
                                                                @CustomerID = @P4, 
                                                                @SupplierID = @P5",
                                                                    processedDay,
                                                                    processedDay,
                                                                    switchID,
                                                                    customer == null ? null : customer.CarrierAccountID,
                                                                    supplier == null ? null : supplier.CarrierAccountID
                                                                    );
                                }
                                spent = DateTime.Now.Subtract(start);
                                log.InfoFormat("Cleaned Traffic Stats for Date: {0} in {1}", processedDay, spent);
                                start = DateTime.Now;
                                try
                                {
                                    Components.BulkManager bulkManager = null;
                                    using (bulkManager = new BulkManager(log))
                                    {
                                        start = DateTime.Now;
                                        log.InfoFormat("Start saving Traffic Stats for {0:yyyy-MM-dd}, Entries: {1:#,##0}. Time: {2}", processedDay, _TrafficStats.Count, DateTime.Now);
                                        bulkManager.Write(_TrafficStats.Values.ToList());
                                        spent = DateTime.Now.Subtract(start);
                                        log.InfoFormat("Saved Traffic Stats for {0:yyyy-MM-dd}, Entries: {1:#,##0}. Time: {2}", processedDay, _TrafficStats.Count, spent);
                                        //statsBatch.Clear();
                                        start = DateTime.Now;
                                        log.InfoFormat("Cleaning Daily Traffic Stats for Date: {0}", processedDay);
                                        DataHelper.ExecuteNonQuery(@"EXEC bp_CleanDailyTrafficStats 
                                                                @DataTime = @P1, 
                                                                @CustomerID = @P2, 
                                                                @SupplierID = @P3",
                                                                       processedDay,
                                                                       customer == null ? null : customer.CarrierAccountID,
                                                                       supplier == null ? null : supplier.CarrierAccountID);
                                        spent = DateTime.Now.Subtract(start);
                                        log.InfoFormat("Cleaned Daily Traffic Stats for Date: {0} in {1}", processedDay, spent);
                                        start = DateTime.Now;
                                        log.InfoFormat("Start saving Daily Traffic Stats for {0:yyyy-MM-dd}, Entries: {1:#,##0}. Time: {2}", processedDay, _DailyTrafficStats.Count, DateTime.Now);
                                        bulkManager.WriteDailyTrafficStat(_DailyTrafficStats.Values.ToList());
                                        spent = DateTime.Now.Subtract(start);
                                        log.InfoFormat("Saved Daily Traffic Stats for {0:yyyy-MM-dd}, Entries: {1:#,##0}. Time: {2}", processedDay, _DailyTrafficStats.Count, spent);
                                        //statsBatch = null;
                                        bulkManager.Dispose();
                                        bulkManager = null;
                                        BulkManager.Clear();
                                    }
                                    spent = DateTime.Now.Subtract(start);
                                    log.InfoFormat("Saved Traffic Stats for {0:yyyy-MM-dd}, Entries: {1:#,##0}. Time: {2}", processedDay, _TrafficStats.Count, spent);
                                }
                                catch (Exception ex)
                                {
                                    log.Error(string.Format("Error Saving Traffic Stats for Day: {0:yyyy-MM-dd}", processedDay), ex);
                                }
                            }
                        }
                        else
                            inconsistentData = true;

                        if (!parameters.IsStopRequested)
                        {
                            // Billing Statistics for this Day
                            start = DateTime.Now;
                            log.InfoFormat("Rebuilding Billing Stats for {0:yyyy-MM-dd}, Customer: {1}", processedDay, customer);
                            SecondaryDataHelper.ExecuteNonQuery("EXEC bp_BuildBillingStats @Day = @P1, @CustomerID = @P2", processedDay, customerID);
                            spent = DateTime.Now.Subtract(start);
                            log.InfoFormat("Finished Rebuilding Billing Stats for {0:yyyy-MM-dd}, Customer: {1}. Time: {2}", processedDay, customer, spent);

                            // Update Daily Prepaid Amounts
                            start = DateTime.Now;
                            log.InfoFormat("Update Daily Prepaid Amounts for {0:yyyy-MM-dd}", processedDay);
                            SecondaryDataHelper.ExecuteNonQuery("EXEC bp_PrepaidDailyTotalUpdate @FromCallDate = @P1, @ToCallDate = @P1", processedDay);
                            spent = DateTime.Now.Subtract(start);
                            log.InfoFormat("Finished Updating Daily Prepaid Amounts for {0:yyyy-MM-dd}. Time: {1}", processedDay, spent);

                            // Update Daily Postpaid Amounts
                            start = DateTime.Now;
                            log.InfoFormat("Update Daily Postpaid Amounts for {0:yyyy-MM-dd}", processedDay);
                            SecondaryDataHelper.ExecuteNonQuery("EXEC bp_PostpaidDailyTotalUpdate @FromCallDate = @P1, @ToCallDate = @P1", processedDay);
                            spent = DateTime.Now.Subtract(start);
                            log.InfoFormat("Finished Updating Daily Pospaid Amounts for {0:yyyy-MM-dd}. Time: {1}", processedDay, spent);
                        }
                        else
                            inconsistentData = true;

                        if (!parameters.IsStopRequested) inconsistentData = false;

                        spent = DateTime.Now.Subtract(dayStart);
                        log.InfoFormat("Processed Day {0:yyyy-MM-dd} in {1}, Total: {2}", processedDay, spent, DateTime.Now.Subtract(allStart));

                        Engine.PricingCompleteNotify(log, parameters);
                    }
                    _TrafficStats.Clear(); _TrafficStats = null;
                    pricingGenerator.Dispose();
                    pricingGenerator = null;
                    if (codeMap.SupplierCodes != null)
                        codeMap.SupplierCodes.Clear();
                    if (codeMap.SupplierCodesStr != null)
                        codeMap.SupplierCodesStr.Clear();
                    if (codeMap.SaleCodes != null)
                        codeMap.SaleCodes.Clear();
                    codeMap.SaleCodes = null;
                    codeMap.SupplierCodes = null; codeMap.SupplierCodesStr = null;
                    GC.Collect(); GC.Collect();
                }
                _DailyTrafficStats.Clear(); _DailyTrafficStats = null; GC.Collect(); GC.Collect();
                TABS.CodeMap.ClearCachedCollections();
                GC.Collect(); GC.Collect();
            }
            catch (Exception ex)
            {
                log.Error("Error Regenerating CDRs", ex);
                TABS.CodeMap.ClearCachedCollections();
                Engine.RepricingParameters = null;
                throw ex;
            }
            finally
            {

                log.InfoFormat("Finished Regenerating Billing CDRs. From: {0}, Till: {1}, Switch: {2}, Customer: {3}, Supplier: {4}, Batch Size: {5:#,###}, Chunk: {6}, Traffic Stats: {7}", startDate, endDate, selectedSwitch, customer, supplier, batchSize, dailyProcessingTime, generateTrafficStats);
                if (inconsistentData)
                    log.ErrorFormat("Repricing was stopped before its end, thus inconsistent billing and statistical data exist. You should re-run the operation till completion");
                // Garbage Collect and Return
                Engine.RepricingParameters = null;
                GC.Collect();
            }

            CdrRepricingParameters paramsToProcess = null;
            lock (CdrRepricingParameters.Queue)
            {
                if (CdrRepricingParameters.Queue.Count > 0)
                {
                    paramsToProcess = CdrRepricingParameters.Queue.Dequeue();
                }
            }
            if (paramsToProcess != null)
            {
                Process(paramsToProcess);
                GC.Collect();
            }
        }

        /// <summary>
        /// Generate Billing CDRs using the provided Code Map and NHibernate Session for the given CDRs.
        /// </summary>
        /// <param name="log">A Logger to log</param>
        /// <param name="codeMap">The Code Map</param>
        /// <param name="CDRs">The CDRs to Generate Pricing for</param>
        /// <param name="session">NHibernate Session</param>
        /// <returns>list of Billing CDRs (Main or Invalid)</returns>
        internal static void WriteBillingCDRs(log4net.ILog log, List<Billing_CDR_Base> billingCDRs)
        {
            // Generate Billing CDRs

            log.Info("Writing Billing Records from Imported CDRs");

            // Bulk Write generated billing CDRs in batches
            int batchSize = 10000;
            int index = 0;
            List<Billing_CDR_Base> cdrBatchList = new List<Billing_CDR_Base>(batchSize);
            using (BulkManager bulkManager = new BulkManager(log))
            {
                while (index < billingCDRs.Count)
                {
                    cdrBatchList.Add(billingCDRs[index]);
                    if (cdrBatchList.Count >= batchSize)
                    {
                        bulkManager.Write(cdrBatchList, false);
                        System.Threading.Thread.Sleep(0);
                        cdrBatchList.Clear();
                    }
                    index++;
                }
                if (cdrBatchList.Count > 0)
                    bulkManager.Write(cdrBatchList, false);
            }

            log.InfoFormat("Written {0} Billing Records", billingCDRs.Count);
        }

        /// <summary>
        /// Generate a billing CDR from a CDR standard
        /// </summary>
        /// <param name="codeMap">The Code Map</param>
        /// <param name="validCount">Counter for valid Cdrs</param>
        /// <param name="cdr">The CDR to generate a billing CDR from</param>
        /// <returns>the generated billing CDR</returns>
        public static Billing_CDR_Base GenerateBillingCdr(CodeMap codeMap, ref int validCount, CDR cdr)
        {
            try
            {
                Billing_CDR_Base billingCDR = null;
                if (cdr.DurationInSeconds > 0)
                {
                    billingCDR = new Billing_CDR_Main();
                    validCount++;
                }
                else
                    billingCDR = new Billing_CDR_Invalid();

                bool valid = cdr.Switch.SwitchManager.FillCDRInfo(cdr.Switch, cdr, billingCDR);

                //billingCDR.CDPNOut = cdr.CDPNOut;

                GenerateZones(codeMap, billingCDR);

                // If there is a duration and missing supplier (zone) or Customer (zone) info
                // then it is considered invalid
                if (billingCDR is Billing_CDR_Main)
                    if (!valid
                        || billingCDR.Customer.RepresentsASwitch
                        || billingCDR.Supplier.RepresentsASwitch
                        || billingCDR.CustomerID == null
                        || billingCDR.SupplierID == null
                        || billingCDR.OurZone == null
                        || billingCDR.SupplierZone == null
                        || billingCDR.Customer.ActivationStatus == ActivationStatus.Inactive
                        || billingCDR.Supplier.ActivationStatus == ActivationStatus.Inactive)
                    {
                        billingCDR = new Billing_CDR_Invalid(billingCDR);
                        validCount--;
                    }

                return billingCDR;
            }
            catch (Exception EX)
            {

                //this.log.Error("Error generating billing Cdr", EX);
                throw (EX);
            }
        }

        /// <summary>
        /// Generate a billing CDR from a CDR standard
        /// </summary>
        /// <param name="codeMap">The Code Map</param>
        /// <param name="validCount">Counter for valid Cdrs</param>
        /// <param name="cdr">The CDR to generate a billing CDR from</param>
        /// <returns>the generated billing CDR</returns>
        public static Billing_CDR_Base GenerateBillingCdr(CodeMap codeMap, ref int validCount, CDR cdr, log4net.ILog log)
        {
            Billing_CDR_Base billingCDR = null;
            try
            {
                if (cdr.DurationInSeconds > 0)
                {
                    billingCDR = new Billing_CDR_Main();
                    validCount++;
                }
                else
                    billingCDR = new Billing_CDR_Invalid();
                //log.Info("Switch:" + cdr.Switch.SwitchID + "(" + cdr.Switch.Name + ")");
                bool valid = cdr.Switch.SwitchManager.FillCDRInfo(cdr.Switch, cdr, billingCDR);

                //billingCDR.CDPNOut = cdr.CDPNOut;

                GenerateZones(codeMap, billingCDR);

                // If there is a duration and missing supplier (zone) or Customer (zone) info
                // then it is considered invalid
                if (billingCDR is Billing_CDR_Main)
                    if (!valid
                        || billingCDR.Customer.RepresentsASwitch
                        || billingCDR.Supplier.RepresentsASwitch
                        || billingCDR.CustomerID == null
                        || billingCDR.SupplierID == null
                        || billingCDR.OurZone == null
                        || billingCDR.SupplierZone == null
                        || billingCDR.Customer.ActivationStatus == ActivationStatus.Inactive
                        || billingCDR.Supplier.ActivationStatus == ActivationStatus.Inactive)
                    {
                        billingCDR = new Billing_CDR_Invalid(billingCDR);
                        validCount--;
                    }
                return billingCDR;
            }
            catch (Exception EX)
            {
                log.Error("Error Generating Billing Cdr: " + EX.Message);
            }
            return billingCDR;
        }

        /// <summary>
        /// Generate Our and the Supplier Zones for the given CDR
        /// </summary>
        /// <param name="codeMap">The code map to use to generate the zones</param>
        /// <param name="cdr">The CDR Record for which to generate the Zones</param>
        protected static void GenerateZones(CodeMap codeMap, Billing_CDR_Base cdr)
        {
            // Our Zone
            Code ourCurrentCode = codeMap.Find(cdr.CDPN, CarrierAccount.SYSTEM, cdr.Attempt);
            if (ourCurrentCode != null)
            {
                cdr.OurZone = ourCurrentCode.Zone;
                cdr.OurCode = ourCurrentCode.Value;
            }

            // Originating Zone
            if (cdr.CustomerID != null && CarrierAccount.All.ContainsKey(cdr.CustomerID))
            {
                CarrierAccount customer = CarrierAccount.All[cdr.CustomerID];
                if (customer.IsOriginatingZonesEnabled)
                {
                    if (cdr.CGPN != null && cdr.CGPN.Trim().Length > 0)
                    {
                        string orginatingCode = InvalidCGPNDigits.Replace(cdr.CGPN, "");
                        Code originatingCode = codeMap.Find(orginatingCode, CarrierAccount.SYSTEM, cdr.Attempt);
                        if (originatingCode != null)
                            cdr.OriginatingZone = originatingCode.Zone;
                    }
                }
            }

            // Supplier Zone
            if (cdr.SupplierID != null && CarrierAccount.All.ContainsKey(cdr.SupplierID))
            {
                CarrierAccount supplier = CarrierAccount.All[cdr.SupplierID];
                Code supplierCode = null;

                if (TABS.SystemParameter.AllowCostZoneCalculationFromCDPNOut.BooleanValue.Value)
                {
                    if (string.IsNullOrEmpty(cdr.CDPNOut))
                        supplierCode = codeMap.Find(cdr.CDPN, supplier, cdr.Attempt);
                    else
                        supplierCode = codeMap.Find(cdr.CDPNOut, supplier, cdr.Attempt);
                }
                else
                    supplierCode = codeMap.Find(cdr.CDPN, supplier, cdr.Attempt);

                if (supplierCode != null)
                {
                    cdr.SupplierZone = supplierCode.Zone;
                    cdr.SupplierCode = supplierCode.Value;
                }
            }
        }



        /// <summary>
        /// Generate Our and the Supplier Zones for the given CDR
        /// </summary>
        /// <param name="codeMap">The code map to use to generate the zones</param>
        /// <param name="cdr">The CDR Record for which to generate the Zones</param>
        //protected static void GenerateZones(CodeMap codeMap, Billing_CDR_Base cdr)
        //{
        //    // Our Zone
        //    Code ourCurrentCode = codeMap.Find(cdr.CDPN, CarrierAccount.SYSTEM, cdr.Attempt);
        //    if (ourCurrentCode != null)
        //    {
        //        //cdr.OurZone = ourCurrentCode.Zone;
        //        //cdr.OurCode = ourCurrentCode.Value;

        //        //Double check if OurZone,OurCode Is not effective in the Time Of CDR Attempt
        //        if (IsCdrAttemptBelongBaseEntities(cdr.Attempt, ourCurrentCode.Zone.BeginEffectiveDate.Value, ourCurrentCode.Zone.EndEffectiveDate))
        //            cdr.OurZone = ourCurrentCode.Zone;
        //        if (IsCdrAttemptBelongBaseEntities(cdr.Attempt, ourCurrentCode.BeginEffectiveDate.Value, ourCurrentCode.EndEffectiveDate))
        //            cdr.OurCode = ourCurrentCode.Value;
        //    }

        //    // Originating Zone
        //    if (cdr.CustomerID != null && CarrierAccount.All.ContainsKey(cdr.CustomerID))
        //    {
        //        CarrierAccount customer = CarrierAccount.All[cdr.CustomerID];
        //        if (customer.IsOriginatingZonesEnabled)
        //        {
        //            if (cdr.CGPN != null && cdr.CGPN.Trim().Length > 0)
        //            {
        //                string orginatingCode = InvalidCGPNDigits.Replace(cdr.CGPN, "");
        //                Code originatingCode = codeMap.Find(orginatingCode, CarrierAccount.SYSTEM, cdr.Attempt);
        //                if (originatingCode != null)
        //                {
        //                    //cdr.OriginatingZone = originatingCode.Zone;


        //                    //Double check if OriginatingZone Is not effective in the Time Of CDR Attempt
        //                    if (IsCdrAttemptBelongBaseEntities(cdr.Attempt, originatingCode.Zone.BeginEffectiveDate.Value, originatingCode.Zone.EndEffectiveDate))
        //                        cdr.OriginatingZone = originatingCode.Zone;
        //                }
        //            }
        //        }
        //    }

        //    // Supplier Zone and Taken into consideration The SupplierTime Shift
        //    if (cdr.SupplierID != null && CarrierAccount.All.ContainsKey(cdr.SupplierID))
        //    {
        //        CarrierAccount supplier = CarrierAccount.All[cdr.SupplierID];
        //        Code supplierCode = null;

        //        if (TABS.SystemParameter.AllowCostZoneCalculationFromCDPNOut.BooleanValue.Value)
        //        {
        //            if (string.IsNullOrEmpty(cdr.CDPNOut))
        //                supplierCode = codeMap.Find(cdr.CDPN, supplier, cdr.Attempt.AddMinutes(supplier.SupplierGMTTime));
        //            else
        //                supplierCode = codeMap.Find(cdr.CDPNOut, supplier, cdr.Attempt.AddMinutes(supplier.SupplierGMTTime));
        //        }
        //        else
        //            supplierCode = codeMap.Find(cdr.CDPN, supplier, cdr.Attempt.AddMinutes(supplier.SupplierGMTTime));

        //        if (supplierCode != null)
        //        {
        //            //cdr.SupplierZone = supplierCode.Zone;
        //            //cdr.SupplierCode = supplierCode.Value;


        //            //Double check if SupplierZone,SupplierCode Is not effective in the Time Of CDR Attempt
        //            if (IsCdrAttemptBelongBaseEntities(cdr.Attempt, supplierCode.Zone.BeginEffectiveDate.Value, supplierCode.Zone.EndEffectiveDate))
        //                cdr.SupplierZone = supplierCode.Zone;
        //            if (IsCdrAttemptBelongBaseEntities(cdr.Attempt, supplierCode.BeginEffectiveDate.Value, supplierCode.EndEffectiveDate))
        //                cdr.SupplierCode = supplierCode.Value;
        //        }
        //    }
        //}

        //private static bool IsCdrAttemptBelongBaseEntities(DateTime Datetocompare, DateTime DateIn, DateTime? DateOut)
        //{
        //    if (!DateOut.HasValue) return true;
        //    //Check if DateToCompare Belong or Equal to [DateIn , DateOut]
        //    if (DateTime.Compare(Datetocompare, DateIn) >= 0 && DateTime.Compare(Datetocompare, DateOut.Value) <= 0)
        //        return true;
        //    return false;
        //}
    }
}