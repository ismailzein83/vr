//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Data;
//using TABS;
//using System.Collections.Concurrent;
//using TOne.Business;

//namespace TOne.CDRProcess.Activities
//{    
//    internal class CDRBatchProcessor
//    {
//        static System.Text.RegularExpressions.Regex InvalidCGPNDigits = new System.Text.RegularExpressions.Regex("[^0-9]", System.Text.RegularExpressions.RegexOptions.Compiled);

//        internal CDRBatchProcessor(ConcurrentQueue<DataTable> queueDataTables, log4net.ILog logger, ProtCodeMap codeMap,
//            ProtPricingGenerator generator, int sampleMinute, Dictionary<string, TrafficStats> trafficStatistics, Dictionary<string, TrafficStats> dailyTrafficStatistics, CDRBatch cdrBatch)
//        {
//            _queueDataTables = queueDataTables;
//            _logger = logger;
//            _codeMap = codeMap;
//            _generator = generator;
//            _sampleMinute = sampleMinute;
//            _trafficStatistics = trafficStatistics;
//            _dailyTrafficStatistics = dailyTrafficStatistics;
//            _cdrBatch = cdrBatch;
//        }

//        ConcurrentQueue<DataTable> _queueDataTables;
//        log4net.ILog _logger;
//            ProtCodeMap _codeMap;
//        ProtPricingGenerator _generator;
//        int _sampleMinute; 
//        Dictionary<string, TrafficStats> _trafficStatistics;
//        Dictionary<string, TrafficStats> _dailyTrafficStatistics;
//        CDRBatch _cdrBatch;

//        internal System.Threading.Tasks.Task ExecuteAsync()
//        {
//            System.Threading.Tasks.Task t = new System.Threading.Tasks.Task(Execute);
//            t.Start();
//            return t;
//        }

//        private void Execute()
//        {
//            DateTime start = DateTime.Now;
//            List<Billing_CDR_Base> billingCDRs = new List<Billing_CDR_Base>();
//            foreach (TABS.CDR cdr in _cdrBatch.CDRs)
//            {

//                Billing_CDR_Base billingCDR = CreateBillingObj(cdr, _logger, _codeMap, _generator);
//                billingCDRs.Add(billingCDR);
//                UpdateTrafficStats(_trafficStatistics, _sampleMinute, billingCDR);
//                UpdateDailyTrafficStats(billingCDR);
//            }
//            CreateAndSendBillingTables(billingCDRs, _queueDataTables);
//            //Console.WriteLine("{0}: billing tables created for {1} CDRs in {2}", DateTime.Now, _cdrBatch.CDRs.Count, (DateTime.Now - start));
//        }

//        Billing_CDR_Base CreateBillingObj(TABS.CDR cdr, log4net.ILog logger, ProtCodeMap codeMap, ProtPricingGenerator generator)
//        {

//            Billing_CDR_Base billingCDR = GenerateBillingCdr(codeMap, cdr, logger);


//            // Generate Cost and Sale
//            if (billingCDR.IsValid)
//            {

//                Billing_CDR_Main main = (Billing_CDR_Main)billingCDR;
//                main.Billing_CDR_Cost = generator.GetRepricing<Billing_CDR_Cost>(main);
//                main.Billing_CDR_Sale = generator.GetRepricing<Billing_CDR_Sale>(main);
//                if (main.Billing_CDR_Sale == null)
//                    ((ProtPricingGenerator)generator).FixParentCodeSale(main, null, null, codeMap);
//                HandlePassThrough(main);

//                if (main != null && main.Billing_CDR_Cost != null && main.SupplierCode != null)
//                    main.Billing_CDR_Cost.Code = main.SupplierCode;

//                if (main != null && main.Billing_CDR_Sale != null && main.OurCode != null)
//                    main.Billing_CDR_Sale.Code = main.OurCode;
//            }


//            return billingCDR;
//        }


//        void CreateAndSendBillingTables(List<Billing_CDR_Base> billingCDRs, ConcurrentQueue<DataTable> queueDataTables)
//        {
//            if (billingCDRs != null && billingCDRs.Count > 0)
//            {

//                CDRManager cdrManager = new CDRManager();
//                var mainRecords = billingCDRs.Where(c => c.IsValid);
//                var invalidRecords = billingCDRs.Where(c => !c.IsValid);

//                long mainId = cdrManager.ReserveRePricingMainCDRIDs(mainRecords.Count());
//                long invalidId = cdrManager.ReserveRePricingInvalidCDRIDs(invalidRecords.Count());


//                DataTable dtMain = GetBillingCdrTable(mainRecords, BulkManager.MAIN_TABLE_NAME, mainId);

//                DataTable dtInvalid = GetBillingCdrTable(invalidRecords, BulkManager.INVALID_TABLE_NAME, invalidId);

//                DataTable dtCost;
//                DataTable dtSale;
//                GetPricingTables(mainRecords, out dtSale, out dtCost);
//                queueDataTables.Enqueue(dtMain);
//                queueDataTables.Enqueue(dtInvalid);
//                queueDataTables.Enqueue(dtCost);
//                queueDataTables.Enqueue(dtSale);
//            }
//        }

//        Billing_CDR_Base GenerateBillingCdr(ProtCodeMap codeMap, TABS.CDR cdr, log4net.ILog log)
//        {
//            Billing_CDR_Base billingCDR = null;
//            try
//            {
//                if (cdr.DurationInSeconds > 0)
//                {
//                    billingCDR = new Billing_CDR_Main();
//                }
//                else
//                    billingCDR = new Billing_CDR_Invalid();
//                //log.Info("Switch:" + cdr.Switch.SwitchID + "(" + cdr.Switch.Name + ")");

//                bool valid = cdr.Switch.SwitchManager.FillCDRInfo(cdr.Switch, cdr, billingCDR);

//                //billingCDR.CDPNOut = cdr.CDPNOut;

//                GenerateZones(codeMap, billingCDR);

//                // If there is a duration and missing supplier (zone) or Customer (zone) info
//                // then it is considered invalid
//                if (billingCDR is Billing_CDR_Main)
//                    if (!valid
//                        || billingCDR.Customer.RepresentsASwitch
//                        || billingCDR.Supplier.RepresentsASwitch
//                        || billingCDR.CustomerID == null
//                        || billingCDR.SupplierID == null
//                        || billingCDR.OurZone == null
//                        || billingCDR.SupplierZone == null
//                        || billingCDR.Customer.ActivationStatus == ActivationStatus.Inactive
//                        || billingCDR.Supplier.ActivationStatus == ActivationStatus.Inactive)
//                    {
//                        billingCDR = new Billing_CDR_Invalid(billingCDR);
//                    }
//                return billingCDR;
//            }
//            catch (Exception EX)
//            {
//                log.Error("Error Generating Billing Cdr: " + EX.Message);
//            }
//            return billingCDR;
//        }

//        void GenerateZones(ProtCodeMap codeMap, Billing_CDR_Base cdr)
//        {
//            // Our Zone
//            Code ourCurrentCode = codeMap.Find(cdr.CDPN, CarrierAccount.SYSTEM, cdr.Attempt);
//            if (ourCurrentCode != null)
//            {
//                cdr.OurZone = ourCurrentCode.Zone;
//                cdr.OurCode = ourCurrentCode.Value;
//            }

//            // Originating Zone
//            if (cdr.CustomerID != null && CarrierAccount.All.ContainsKey(cdr.CustomerID))
//            {
//                CarrierAccount customer = CarrierAccount.All[cdr.CustomerID];
//                if (customer.IsOriginatingZonesEnabled)
//                {
//                    if (cdr.CGPN != null && cdr.CGPN.Trim().Length > 0)
//                    {
//                        string orginatingCode = InvalidCGPNDigits.Replace(cdr.CGPN, "");
//                        Code originatingCode = codeMap.Find(orginatingCode, CarrierAccount.SYSTEM, cdr.Attempt);
//                        if (originatingCode != null)
//                            cdr.OriginatingZone = originatingCode.Zone;
//                    }
//                }
//            }

//            // Supplier Zone
//            if (cdr.SupplierID != null && CarrierAccount.All.ContainsKey(cdr.SupplierID))
//            {
//                CarrierAccount supplier = CarrierAccount.All[cdr.SupplierID];
//                Code supplierCode = null;

//                if (TABS.SystemParameter.AllowCostZoneCalculationFromCDPNOut.BooleanValue.Value)
//                {
//                    if (string.IsNullOrEmpty(cdr.CDPNOut))
//                        supplierCode = codeMap.Find(cdr.CDPN, supplier, cdr.Attempt);
//                    else
//                        supplierCode = codeMap.Find(cdr.CDPNOut, supplier, cdr.Attempt);
//                }
//                else
//                    supplierCode = codeMap.Find(cdr.CDPN, supplier, cdr.Attempt);

//                if (supplierCode != null)
//                {
//                    cdr.SupplierZone = supplierCode.Zone;
//                    cdr.SupplierCode = supplierCode.Value;
//                }
//            }
//        }

//        public static void HandlePassThrough(Billing_CDR_Main cdr)
//        {
//            if (cdr.Customer.IsPassThroughCustomer && cdr.Billing_CDR_Cost != null)
//            {
//                var sale = new Billing_CDR_Sale();
//                cdr.Billing_CDR_Sale = sale;
//                sale.Copy(cdr.Billing_CDR_Cost);
//                sale.Zone = cdr.OurZone;
//            }
//            if (cdr.Supplier.IsPassThroughSupplier && cdr.Billing_CDR_Sale != null)
//            {
//                var cost = new Billing_CDR_Cost();
//                cdr.Billing_CDR_Cost = cost;
//                cost.Copy(cdr.Billing_CDR_Sale);
//                cost.Zone = cdr.SupplierZone;
//            }
//        }

//        void UpdateTrafficStats(Dictionary<string, TrafficStats> trafficStatistics, int SampleMinutes, Billing_CDR_Base cdr)
//        {
//            StringBuilder groupingKeyBuffer = new StringBuilder(512);
//            string portIn, portOut;
//            GetTrafficStatsKey(cdr, SampleMinutes, groupingKeyBuffer, out portIn, out portOut);
//            string groupingKey = groupingKeyBuffer.ToString();

//            TrafficStats trafficStats = null;
//            bool isNew = false;
//            lock (trafficStatistics)
//            {
//                if (!trafficStatistics.TryGetValue(groupingKey, out trafficStats))
//                {
//                    trafficStats = new TrafficStats();
//                    trafficStatistics.Add(groupingKey, trafficStats);
//                    isNew = true;
//                }
//            }

//            if (isNew)
//            {
//                // Initialize Grouping Fields
//                CarrierAccount customer = null;
//                CarrierAccount supplier = null;

//                if (cdr.CustomerID != null) customer = CarrierAccount.All.ContainsKey(cdr.CustomerID) ? CarrierAccount.All[cdr.CustomerID] : null;
//                if (cdr.SupplierID != null) supplier = CarrierAccount.All.ContainsKey(cdr.SupplierID) ? CarrierAccount.All[cdr.SupplierID] : null;

//                lock (trafficStats)
//                {
//                    if (cdr.CustomerID != null) trafficStats.Customer = customer;
//                    if (cdr.SupplierID != null) trafficStats.Supplier = supplier;

//                    trafficStats.OriginatingZone = cdr.OriginatingZone;
//                    trafficStats.OurZone = cdr.OurZone;
//                    trafficStats.SupplierZone = cdr.SupplierZone;
//                    trafficStats.Port_IN = portIn;
//                    trafficStats.Port_OUT = portOut;
//                    trafficStats.Switch = cdr.Switch;
//                    trafficStats.FirstCDRAttempt = cdr.Attempt;
//                    trafficStats.LastCDRAttempt = cdr.Attempt;
//                    trafficStats.MaxDurationInSeconds = cdr.DurationInSeconds;
//                    trafficStats.PDDInSeconds = 0;
//                    trafficStats.PGAD = 0;
//                    trafficStats.ReleaseSourceAParty = 0;
//                }
//            }

//            SwitchReleaseCode releaseCode = null;
//            SwitchReleaseCode releaseCode2 = null;
//            if (cdr.ReleaseCode != null)
//            {
//                if (cdr.Switch != null && cdr.Switch.ReleaseCodes != null)
//                    cdr.Switch.ReleaseCodes.TryGetValue(cdr.ReleaseCode, out releaseCode);

//                Dictionary<string, SwitchReleaseCode> switchReleasesCodes;
//                if (cdr.Switch != null)
//                    if (SwitchReleaseCode.All.TryGetValue(cdr.Switch, out switchReleasesCodes))
//                        switchReleasesCodes.TryGetValue(cdr.ReleaseCode, out releaseCode2);
//            }


//            lock (trafficStats)
//            {

//                trafficStats.Saveable = true;

//                // Update Calculated fields
//                // Attempts
//                trafficStats.Attempts++;
//                // Calls (Non-Rerouted Calls)
//                if (!cdr.IsRerouted)
//                {
//                    trafficStats.NumberOfCalls++;
//                    if (cdr.DurationInSeconds > 0)
//                        trafficStats.DeliveredNumberOfCalls++;
//                    else if (releaseCode != null)
//                    {
//                        if (releaseCode.IsDelivered)
//                            trafficStats.DeliveredNumberOfCalls++;
//                    }
//                }

//                // Utilization
//                if (cdr.Disconnect.HasValue && cdr.Disconnect.Value != DateTime.MinValue) trafficStats.Utilization = trafficStats.Utilization.Add(cdr.Disconnect.Value.Subtract(cdr.Attempt));

//                // Duration? then sucessful and delivered
//                if (cdr.DurationInSeconds > 0)
//                {
//                    trafficStats.SuccessfulAttempts++;
//                    trafficStats.DeliveredAttempts++;
//                    // PDD 
//                    if (cdr.PDDInSeconds > 0)
//                    {
//                        decimal n = (decimal)trafficStats.SuccessfulAttempts - 1;
//                        trafficStats.PDDInSeconds = ((n * trafficStats.PDDInSeconds) + cdr.PDDInSeconds) / (n + 1);
//                    }
//                    if (cdr.Connect.HasValue)
//                    {
//                        decimal n = (decimal)trafficStats.SuccessfulAttempts - 1;
//                        trafficStats.PGAD = ((n * trafficStats.PGAD) + cdr.Connect.Value.Subtract(cdr.Attempt).Seconds) / (n + 1);
//                    }
//                }
//                else // No Duration check if release code can give us a hint about delivery
//                {
//                    if (cdr.ReleaseCode != null)
//                    {
//                        //old code
//                        //SwitchReleaseCode releaseCode = null;
//                        //if (cdr.Switch.ReleaseCodes.TryGetValue(cdr.ReleaseCode, out releaseCode))
//                        //new code                       

//                        if (releaseCode2 != null)
//                            if (releaseCode2.IsDelivered)
//                                trafficStats.DeliveredAttempts++;
//                    }
//                }

//                // Sum up Durations
//                trafficStats.DurationsInSeconds += cdr.DurationInSeconds;

//                //Sum up ceiled durations
//                trafficStats.CeiledDuration += (int)Math.Ceiling(cdr.DurationInSeconds);

//                // Update Min/Max Date/ID of CDRs
//                if (cdr.Attempt > trafficStats.LastCDRAttempt) trafficStats.LastCDRAttempt = cdr.Attempt;
//                if (cdr.Attempt < trafficStats.FirstCDRAttempt) trafficStats.FirstCDRAttempt = cdr.Attempt;
//                if (cdr.DurationInSeconds >= trafficStats.MaxDurationInSeconds) trafficStats.MaxDurationInSeconds = cdr.DurationInSeconds;

//                //if (cdr.Connect.HasValue) trafficStats.PGAD = trafficStats.PGAD + cdr.Connect.Value.Subtract(cdr.Attempt).Seconds/billingCDRs.Count;

//                //if (trafficStats.Switch != null && !string.IsNullOrEmpty(trafficStats.Switch.SwitchManagerName))
//                //{
//                //    if (trafficStats.Switch.SwitchManagerName == "TABS.Addons.TelesSwitchLibrary.SwitchManager")
//                if (cdr.ReleaseSource != null && cdr.ReleaseSource.ToUpper().Equals("A")) trafficStats.ReleaseSourceAParty += 1;
//                //}
//            }
//        }

//        void GetTrafficStatsKey(Billing_CDR_Base cdr, int SampleMinutes, StringBuilder groupingKeyBuffer, out string portIn, out string portOut)
//        {
//            groupingKeyBuffer.Length = 0;

//            portIn = TABS.Components.Engine.GetIPOrTrunc(cdr.Port_IN);
//            portOut = TABS.Components.Engine.GetIPOrTrunc(cdr.Port_OUT);

//            groupingKeyBuffer
//                .Append(cdr.Switch.SwitchID).Append('\t')
//                .Append(portIn).Append('\t')
//                .Append(portOut).Append('\t')
//                .Append(cdr.CustomerID).Append('\t')
//                .Append(cdr.OurZone == null ? "" : cdr.OurZone.ZoneID.ToString()).Append('\t')
//                //.Append(cdr.OriginatingZone == null ? "" : cdr.OriginatingZone.ZoneID.ToString()).Append('\t')
//                .Append(cdr.SupplierID).Append('\t')
//                .Append(cdr.SupplierZone == null ? "" : cdr.SupplierZone.ZoneID.ToString()).Append('\t')
//                .Append(string.Format("{0}{1}", cdr.Attempt.ToString("yyyyMMddHH"), cdr.Attempt.Minute / SampleMinutes));
//        }

//        void UpdateDailyTrafficStats(Billing_CDR_Base cdr)
//        {
//            StringBuilder groupingKeyBuffer = new StringBuilder(512);
//            string portIn, portOut;
//            GetDailyTrafficStatsKey(cdr, groupingKeyBuffer, out portIn, out portOut);
//            string groupingKey = groupingKeyBuffer.ToString();

//            TrafficStats dailyTrafficStats = null;

//            bool isNew = false;
//            lock (_dailyTrafficStatistics)
//            {
//                if (!_dailyTrafficStatistics.TryGetValue(groupingKey, out dailyTrafficStats))
//                {
//                    dailyTrafficStats = new TrafficStats();
//                    _dailyTrafficStatistics.Add(groupingKey, dailyTrafficStats);
//                    isNew = true;
//                }
//            }

//            if (isNew)
//            {
//                // Initialize Grouping Fields
//                CarrierAccount customer = null;
//                CarrierAccount supplier = null;

//                if (cdr.CustomerID != null) customer = CarrierAccount.All.ContainsKey(cdr.CustomerID) ? CarrierAccount.All[cdr.CustomerID] : null;
//                if (cdr.SupplierID != null) supplier = CarrierAccount.All.ContainsKey(cdr.SupplierID) ? CarrierAccount.All[cdr.SupplierID] : null;

//                lock (dailyTrafficStats)
//                {
//                    dailyTrafficStats.CallDate = DateTime.Parse(cdr.Attempt.ToString("yyyy-MM-dd"));
//                    dailyTrafficStats.Switch = cdr.Switch;

//                    if (cdr.CustomerID != null) dailyTrafficStats.Customer = customer;
//                    if (cdr.SupplierID != null) dailyTrafficStats.Supplier = supplier;

//                    dailyTrafficStats.OurZone = cdr.OurZone;
//                    dailyTrafficStats.OriginatingZone = cdr.OriginatingZone;
//                    dailyTrafficStats.SupplierZone = cdr.SupplierZone;
//                    dailyTrafficStats.Port_IN = portIn;
//                    dailyTrafficStats.Port_OUT = portOut;


//                    dailyTrafficStats.FirstCDRAttempt = cdr.Attempt;
//                    dailyTrafficStats.LastCDRAttempt = cdr.Attempt;
//                    dailyTrafficStats.MaxDurationInSeconds = cdr.DurationInSeconds;
//                    dailyTrafficStats.PDDInSeconds = 0;
//                    dailyTrafficStats.PGAD = 0;
//                    dailyTrafficStats.ReleaseSourceAParty = 0;
//                }
//            }

//            SwitchReleaseCode releaseCode = null;
//            if (cdr.ReleaseCode != null)
//            {
//                if (cdr.Switch != null && cdr.Switch.ReleaseCodes != null)
//                    cdr.Switch.ReleaseCodes.TryGetValue(cdr.ReleaseCode, out releaseCode);
//            }

//            lock (dailyTrafficStats)
//            {
//                dailyTrafficStats.Saveable = true;

//                // Update Calculated fields
//                // Attempts
//                dailyTrafficStats.Attempts++;
//                // Calls (Non-Rerouted Calls)


//                if (!cdr.IsRerouted)
//                {
//                    dailyTrafficStats.NumberOfCalls++;
//                    if (cdr.ReleaseCode != null && cdr.DurationInSeconds == 0)
//                    {
//                        if (releaseCode != null && releaseCode.IsDelivered)
//                            dailyTrafficStats.DeliveredNumberOfCalls++;
//                    }
//                }

//                // Utilization
//                if (cdr.Disconnect.HasValue && cdr.Disconnect.Value != DateTime.MinValue) dailyTrafficStats.Utilization = dailyTrafficStats.Utilization.Add(cdr.Disconnect.Value.Subtract(cdr.Attempt));

//                // Duration? then sucessful and delivered
//                if (cdr.DurationInSeconds > 0)
//                {
//                    dailyTrafficStats.SuccessfulAttempts++;
//                    dailyTrafficStats.DeliveredAttempts++;
//                    // PDD 
//                    if (cdr.PDDInSeconds > 0)
//                    {
//                        decimal n = (decimal)dailyTrafficStats.SuccessfulAttempts - 1;
//                        dailyTrafficStats.PDDInSeconds = ((n * dailyTrafficStats.PDDInSeconds) + cdr.PDDInSeconds) / (n + 1);
//                    }
//                    if (cdr.Connect.HasValue)
//                    {
//                        decimal n = (decimal)dailyTrafficStats.SuccessfulAttempts - 1;
//                        dailyTrafficStats.PGAD = ((n * dailyTrafficStats.PGAD) + cdr.Connect.Value.Subtract(cdr.Attempt).Seconds) / (n + 1);
//                    }
//                }
//                else // No Duration check if release code can give us a hint about delivery
//                {
//                    if (cdr.ReleaseCode != null)
//                    {
//                        if (releaseCode != null && releaseCode.IsDelivered)
//                            dailyTrafficStats.DeliveredAttempts++;
//                    }
//                }


//                // Sum up Durations
//                dailyTrafficStats.DurationsInSeconds += cdr.DurationInSeconds;

//                //Sum up Ceiled Durations
//                dailyTrafficStats.CeiledDuration += (int)Math.Ceiling(cdr.DurationInSeconds);

//                // Update Min/Max Date/ID of CDRs
//                if (cdr.Attempt > dailyTrafficStats.LastCDRAttempt) dailyTrafficStats.LastCDRAttempt = cdr.Attempt;
//                if (cdr.Attempt < dailyTrafficStats.FirstCDRAttempt) dailyTrafficStats.FirstCDRAttempt = cdr.Attempt;
//                if (cdr.DurationInSeconds >= dailyTrafficStats.MaxDurationInSeconds) dailyTrafficStats.MaxDurationInSeconds = cdr.DurationInSeconds;
//                //if (cdr.Connect.HasValue) trafficStats.PGAD = trafficStats.PGAD + cdr.Connect.Value.Subtract(cdr.Attempt).Seconds/billingCDRs.Count;                   

//                //if (DialytrafficStats.Switch != null && !string.IsNullOrEmpty(DialytrafficStats.Switch.SwitchManagerName))
//                //{
//                //    if (DialytrafficStats.Switch.SwitchManagerName == "TABS.Addons.TelesSwitchLibrary.SwitchManager")
//                if (cdr.ReleaseSource != null && cdr.ReleaseSource.ToUpper().Equals("A")) dailyTrafficStats.ReleaseSourceAParty += 1;
//                //}
//            }
//        }

//        internal static void GetDailyTrafficStatsKey(Billing_CDR_Base cdr, StringBuilder groupingKeyBuffer, out string portIn, out string portOut)
//        {
//            groupingKeyBuffer.Length = 0;

//            portIn = TABS.Components.Engine.GetIPOrTrunc(cdr.Port_IN);
//            portOut = TABS.Components.Engine.GetIPOrTrunc(cdr.Port_OUT);

//            groupingKeyBuffer
//                .Append(cdr.CustomerID).Append('\t')
//                .Append(cdr.OurZone == null ? "" : cdr.OurZone.ZoneID.ToString()).Append('\t')
//                .Append(cdr.OriginatingZone == null ? "" : cdr.OriginatingZone.ZoneID.ToString()).Append('\t')
//                .Append(cdr.SupplierID).Append('\t')
//                .Append(cdr.SupplierZone == null ? "" : cdr.SupplierZone.ZoneID.ToString()).Append('\t')
//                .Append(string.Format("{0}", cdr.Attempt.ToString("yyyyMMdd")));
//        }

//        void GetPricingTables(IEnumerable<Billing_CDR_Base> billingMainData, out DataTable saleTable, out DataTable costTable)
//        {
//            //log.DebugFormat("Bulk Building Data table: {0}", tableName);
//            saleTable = PricingTable.Clone();
//            saleTable.TableName = BulkManager.SALE_TABLE_NAME;
//            saleTable.BeginLoadData();

//            costTable = PricingTable.Clone();
//            costTable.TableName = BulkManager.COST_TABLE_NAME;
//            costTable.BeginLoadData();

//            Action<Billing_CDR_Pricing_Base, DataRow> fillRow = (pricing, row) =>
//            {
//                int index = -1;
//                index++; row[index] = pricing.Billing_CDR_Main.ID;
//                index++; row[index] = pricing.Zone.ZoneID;
//                index++; row[index] = pricing.Net;
//                index++; row[index] = pricing.Currency.Symbol;
//                index++; row[index] = pricing.RateValue;
//                index++; row[index] = pricing.Rate.ID;
//                index++; row[index] = pricing.Discount.HasValue ? (object)pricing.Discount.Value : DBNull.Value;
//                index++; row[index] = pricing.RateType;
//                index++; row[index] = pricing.ToDConsideration == null ? DBNull.Value : (object)pricing.ToDConsideration.ToDConsiderationID;
//                index++; row[index] = pricing.FirstPeriod.HasValue ? (object)pricing.FirstPeriod.Value : DBNull.Value;
//                index++; row[index] = pricing.RepeatFirstperiod.HasValue ? (object)pricing.RepeatFirstperiod.Value : DBNull.Value;
//                index++; row[index] = pricing.FractionUnit.HasValue ? (object)pricing.FractionUnit.Value : DBNull.Value;
//                index++; row[index] = pricing.Tariff == null ? DBNull.Value : (object)pricing.Tariff.TariffID;
//                index++; row[index] = pricing.CommissionValue;
//                index++; row[index] = pricing.Commission == null ? DBNull.Value : (object)pricing.Commission.CommissionID;
//                index++; row[index] = pricing.ExtraChargeValue;
//                index++; row[index] = pricing.ExtraCharge == null ? DBNull.Value : (object)pricing.ExtraCharge.CommissionID;
//                index++; row[index] = pricing.Updated;
//                index++; row[index] = pricing.DurationInSeconds;
//                index++; row[index] = pricing.Code == null ? DBNull.Value : (object)pricing.Code;
//                index++; row[index] = pricing.Attempt = pricing.Billing_CDR_Main.Attempt;
//            };


//            foreach (Billing_CDR_Main pricing in billingMainData)
//            {
//                if (pricing.Billing_CDR_Sale != null)
//                {
//                    DataRow row = saleTable.NewRow();
//                    fillRow(pricing.Billing_CDR_Sale, row);
//                    saleTable.Rows.Add(row);
//                }

//                if (pricing.Billing_CDR_Cost != null)
//                {
//                    DataRow row = costTable.NewRow();
//                    fillRow(pricing.Billing_CDR_Cost, row);
//                    costTable.Rows.Add(row);
//                }

//            }
//            saleTable.EndLoadData();
//            costTable.EndLoadData();
//        }

//        DataTable GetBillingCdrTable(IEnumerable<Billing_CDR_Base> cdrData, string tableName, long startingID)
//        {
//            //log.DebugFormat("Bulk Building Data table: {0}", tableName);
//            DataTable dt = BillingCdrTable.Clone();
//            bool includeRerouted = tableName.ToLower().EndsWith("invalid");
//            if (!includeRerouted) dt.Columns.Remove("IsRerouted");
//            dt.TableName = tableName;
//            dt.BeginLoadData();

//            long billingCDRID = startingID;
//            foreach (var cdr in cdrData)
//            {
//                cdr.ID = billingCDRID;
//                billingCDRID--;
//                DataRow row = dt.NewRow();
//                int index = -1;
//                index++; row[index] = cdr.ID;
//                index++; row[index] = cdr.Attempt;
//                index++; row[index] = cdr.Alert.HasValue && cdr.Alert.Value != DateTime.MinValue ? (object)cdr.Alert : DBNull.Value;
//                index++; row[index] = cdr.Connect.HasValue && cdr.Connect.Value != DateTime.MinValue ? (object)cdr.Connect : DBNull.Value;
//                index++; row[index] = cdr.Disconnect.HasValue && cdr.Disconnect.Value != DateTime.MinValue ? (object)cdr.Disconnect : DBNull.Value;
//                index++; row[index] = cdr.DurationInSeconds;
//                index++; row[index] = cdr.CustomerID;
//                index++; row[index] = cdr.OurZone == null ? DBNull.Value : (object)cdr.OurZone.ZoneID;
//                index++; row[index] = cdr.OriginatingZone == null ? DBNull.Value : (object)cdr.OriginatingZone.ZoneID;
//                index++; row[index] = cdr.SupplierID;
//                index++; row[index] = cdr.SupplierZone == null ? DBNull.Value : (object)cdr.SupplierZone.ZoneID;
//                index++; row[index] = cdr.CDPN;
//                index++; row[index] = cdr.CGPN;
//                index++; row[index] = cdr.CDPNOut;
//                index++; row[index] = cdr.ReleaseCode;
//                index++; row[index] = cdr.ReleaseSource;
//                index++; row[index] = (byte)cdr.Switch.SwitchID;
//                index++; row[index] = cdr.SwitchCdrID;
//                index++; row[index] = cdr.Tag;
//                index++; row[index] = cdr.Extra_Fields;
//                index++; row[index] = cdr.Port_IN;
//                index++; row[index] = cdr.Port_OUT;
//                index++; row[index] = cdr.OurCode != null ? (object)cdr.OurCode : DBNull.Value;
//                index++; row[index] = cdr.SupplierCode != null ? (object)cdr.SupplierCode : DBNull.Value;

//                if (includeRerouted) { index++; row[index] = cdr.IsRerouted ? "Y" : "N"; }
//                dt.Rows.Add(row);
//            }
//            dt.EndLoadData();
//            return dt;
//        }

//        #region DataTable Mappings

//        static object s_DataTableMappingsLock = new object();
//        protected static DataTable _BillingCdrTable, _PricingTable, _BillingStatsTable, _TrafficStatsTable, _CdrTable, _TrafficStatsDailyTable, _TrafficStatsByOriginationDaily, _CodeTable, _Zone, _TrafficStatsTempTable;

//        protected static DataTable CodeTable
//        {
//            get
//            {
//                lock (s_DataTableMappingsLock)
//                {
//                    if (_CodeTable == null)
//                    {
//                        _CodeTable = new DataTable();
//                        _CodeTable.Columns.Add("Code", typeof(string));
//                        _CodeTable.Columns.Add("ZoneID", typeof(string));
//                        _CodeTable.Columns.Add("BeginEffectiveDate", typeof(DateTime));
//                        _CodeTable.Columns.Add("EndEffectiveDate", typeof(DateTime));
//                        _CodeTable.Columns.Add("UserID", typeof(int));
//                    }
//                }
//                return _CodeTable;
//            }
//        }

//        protected static DataTable TrafficStatsTable
//        {
//            get
//            {
//                lock (s_DataTableMappingsLock)
//                {
//                    if (_TrafficStatsTable == null)
//                    {
//                        _TrafficStatsTable = new DataTable();
//                        _TrafficStatsTable.Columns.Add("SwitchId", typeof(int));
//                        _TrafficStatsTable.Columns.Add("Port_IN", typeof(string));
//                        _TrafficStatsTable.Columns.Add("Port_OUT", typeof(string));
//                        _TrafficStatsTable.Columns.Add("CustomerID", typeof(string));
//                        _TrafficStatsTable.Columns.Add("OurZoneID", typeof(int));
//                        _TrafficStatsTable.Columns.Add("OriginatingZoneID", typeof(int));
//                        _TrafficStatsTable.Columns.Add("SupplierID", typeof(string));
//                        _TrafficStatsTable.Columns.Add("SupplierZoneID", typeof(int));
//                        _TrafficStatsTable.Columns.Add("FirstCDRAttempt", typeof(DateTime));
//                        _TrafficStatsTable.Columns.Add("LastCDRAttempt", typeof(DateTime));
//                        _TrafficStatsTable.Columns.Add("Attempts", typeof(int));
//                        _TrafficStatsTable.Columns.Add("DeliveredAttempts", typeof(int));
//                        _TrafficStatsTable.Columns.Add("SuccessfulAttempts", typeof(int));
//                        _TrafficStatsTable.Columns.Add("DurationsInSeconds", typeof(decimal));
//                        _TrafficStatsTable.Columns.Add("PDDInSeconds", typeof(decimal));
//                        _TrafficStatsTable.Columns.Add("MaxDurationInSeconds", typeof(decimal));
//                        _TrafficStatsTable.Columns.Add("UtilizationInSeconds", typeof(decimal));
//                        _TrafficStatsTable.Columns.Add("NumberOfCalls", typeof(int));
//                        _TrafficStatsTable.Columns.Add("DeliveredNumberOfCalls", typeof(int));
//                        _TrafficStatsTable.Columns.Add("PGAD", typeof(int));
//                        _TrafficStatsTable.Columns.Add("CeiledDuration", typeof(int));
//                        _TrafficStatsTable.Columns.Add("ReleaseSourceAParty", typeof(int));
//                    }
//                }
//                return _TrafficStatsTable;
//            }
//        }

//        protected static DataTable TrafficStatsTempTable
//        {
//            get
//            {
//                lock (s_DataTableMappingsLock)
//                {
//                    if (_TrafficStatsTempTable == null)
//                    {
//                        _TrafficStatsTempTable = new DataTable();
//                        _TrafficStatsTempTable.Columns.Add("ID", typeof(long));
//                        _TrafficStatsTempTable.Columns.Add("SwitchId", typeof(int));
//                        _TrafficStatsTempTable.Columns.Add("Port_IN", typeof(string));
//                        _TrafficStatsTempTable.Columns.Add("Port_OUT", typeof(string));
//                        _TrafficStatsTempTable.Columns.Add("CustomerID", typeof(string));
//                        _TrafficStatsTempTable.Columns.Add("OurZoneID", typeof(int));
//                        _TrafficStatsTempTable.Columns.Add("OriginatingZoneID", typeof(int));
//                        _TrafficStatsTempTable.Columns.Add("SupplierID", typeof(string));
//                        _TrafficStatsTempTable.Columns.Add("SupplierZoneID", typeof(int));
//                        _TrafficStatsTempTable.Columns.Add("FirstCDRAttempt", typeof(DateTime));
//                        _TrafficStatsTempTable.Columns.Add("LastCDRAttempt", typeof(DateTime));
//                        _TrafficStatsTempTable.Columns.Add("Attempts", typeof(int));
//                        _TrafficStatsTempTable.Columns.Add("DeliveredAttempts", typeof(int));
//                        _TrafficStatsTempTable.Columns.Add("SuccessfulAttempts", typeof(int));
//                        _TrafficStatsTempTable.Columns.Add("DurationsInSeconds", typeof(decimal));
//                        _TrafficStatsTempTable.Columns.Add("PDDInSeconds", typeof(decimal));
//                        _TrafficStatsTempTable.Columns.Add("MaxDurationInSeconds", typeof(decimal));
//                        _TrafficStatsTempTable.Columns.Add("UtilizationInSeconds", typeof(decimal));
//                        _TrafficStatsTempTable.Columns.Add("NumberOfCalls", typeof(int));
//                        _TrafficStatsTempTable.Columns.Add("DeliveredNumberOfCalls", typeof(int));
//                        _TrafficStatsTempTable.Columns.Add("PGAD", typeof(int));
//                        _TrafficStatsTempTable.Columns.Add("CeiledDuration", typeof(int));
//                        _TrafficStatsTempTable.Columns.Add("ReleaseSourceAParty", typeof(int));
//                    }
//                }
//                return _TrafficStatsTempTable;
//            }
//        }

//        protected static DataTable BillingCdrTable
//        {
//            get
//            {
//                lock (s_DataTableMappingsLock)
//                {
//                    if (_BillingCdrTable == null)
//                    {
//                        _BillingCdrTable = new DataTable();
//                        _BillingCdrTable.Columns.Add("ID", typeof(long));
//                        _BillingCdrTable.Columns.Add("Attempt", typeof(DateTime));
//                        _BillingCdrTable.Columns.Add("Alert", typeof(DateTime));
//                        _BillingCdrTable.Columns.Add("Connect", typeof(DateTime));
//                        _BillingCdrTable.Columns.Add("Disconnect", typeof(DateTime));
//                        _BillingCdrTable.Columns.Add("DurationInSeconds", typeof(decimal));
//                        _BillingCdrTable.Columns.Add("CustomerID", typeof(string));
//                        _BillingCdrTable.Columns.Add("OurZoneID", typeof(int));
//                        _BillingCdrTable.Columns.Add("OriginatingZoneID", typeof(int));
//                        _BillingCdrTable.Columns.Add("SupplierID", typeof(string));
//                        _BillingCdrTable.Columns.Add("SupplierZoneID", typeof(int));
//                        _BillingCdrTable.Columns.Add("CDPN", typeof(string));
//                        _BillingCdrTable.Columns.Add("CGPN", typeof(string));
//                        _BillingCdrTable.Columns.Add("CDPNOut", typeof(string));
//                        _BillingCdrTable.Columns.Add("ReleaseCode", typeof(string));
//                        _BillingCdrTable.Columns.Add("ReleaseSource", typeof(string));
//                        _BillingCdrTable.Columns.Add("SwitchID", typeof(byte));
//                        _BillingCdrTable.Columns.Add("SwitchCdrID", typeof(long));
//                        _BillingCdrTable.Columns.Add("Tag", typeof(string));
//                        _BillingCdrTable.Columns.Add("Extra_Fields", typeof(string));
//                        _BillingCdrTable.Columns.Add("Port_IN", typeof(string));
//                        _BillingCdrTable.Columns.Add("Port_OUT", typeof(string));
//                        _BillingCdrTable.Columns.Add("OurCode", typeof(string));
//                        _BillingCdrTable.Columns.Add("SupplierCode", typeof(string));
//                        _BillingCdrTable.Columns.Add("IsRerouted", typeof(string));
//                    }
//                }
//                return _BillingCdrTable;
//            }
//        }

//        protected static DataTable CdrTable
//        {
//            get
//            {
//                lock (s_DataTableMappingsLock)
//                {
//                    if (_CdrTable == null)
//                    {
//                        _CdrTable = new DataTable();
//                        _CdrTable.Columns.Add("SwitchID", typeof(byte));
//                        _CdrTable.Columns.Add("IDonSwitch", typeof(long));
//                        _CdrTable.Columns.Add("Tag", typeof(string));
//                        _CdrTable.Columns.Add("AttemptDateTime", typeof(DateTime));
//                        _CdrTable.Columns.Add("AlertDateTime", typeof(DateTime));
//                        _CdrTable.Columns.Add("ConnectDateTime", typeof(DateTime));
//                        _CdrTable.Columns.Add("DisconnectDateTime", typeof(DateTime));
//                        _CdrTable.Columns.Add("DurationInSeconds", typeof(decimal));
//                        _CdrTable.Columns.Add("IN_TRUNK", typeof(string));
//                        _CdrTable.Columns.Add("IN_CIRCUIT", typeof(short));
//                        _CdrTable.Columns.Add("IN_CARRIER", typeof(string));
//                        _CdrTable.Columns.Add("IN_IP", typeof(string));
//                        _CdrTable.Columns.Add("OUT_TRUNK", typeof(string));
//                        _CdrTable.Columns.Add("OUT_CIRCUIT", typeof(short));
//                        _CdrTable.Columns.Add("OUT_CARRIER", typeof(string));
//                        _CdrTable.Columns.Add("OUT_IP", typeof(string));
//                        _CdrTable.Columns.Add("CGPN", typeof(string));
//                        _CdrTable.Columns.Add("CDPN", typeof(string));
//                        _CdrTable.Columns.Add("CDPNOut", typeof(string));
//                        _CdrTable.Columns.Add("CAUSE_FROM", typeof(string));
//                        _CdrTable.Columns.Add("CAUSE_TO", typeof(string));
//                        _CdrTable.Columns.Add("CAUSE_FROM_RELEASE_CODE", typeof(string));
//                        _CdrTable.Columns.Add("CAUSE_TO_RELEASE_CODE", typeof(string));
//                        _CdrTable.Columns.Add("Extra_Fields", typeof(string));
//                        _CdrTable.Columns.Add("IsRerouted", typeof(string));
//                        //_CdrTable.Columns.Add("SIP", typeof(string));
//                    }
//                }
//                return _CdrTable;
//            }
//        }

//        protected static DataTable BillingStatsTable
//        {
//            get
//            {
//                lock (s_DataTableMappingsLock)
//                {
//                    if (_BillingStatsTable == null)
//                    {
//                        _BillingStatsTable = new DataTable();
//                        _BillingStatsTable.Columns.Add("CallDate", typeof(DateTime));
//                        _BillingStatsTable.Columns.Add("CustomerID", typeof(string));
//                        _BillingStatsTable.Columns.Add("SupplierID", typeof(string));
//                        _BillingStatsTable.Columns.Add("CostZoneID", typeof(int));
//                        _BillingStatsTable.Columns.Add("SaleZoneID", typeof(int));
//                        _BillingStatsTable.Columns.Add("Cost_Currency", typeof(string));
//                        _BillingStatsTable.Columns.Add("Sale_Currency", typeof(string));
//                        _BillingStatsTable.Columns.Add("SaleDuration", typeof(decimal));
//                        _BillingStatsTable.Columns.Add("CostDuration", typeof(decimal));
//                        _BillingStatsTable.Columns.Add("NumberOfCalls", typeof(int));
//                        _BillingStatsTable.Columns.Add("FirstCallTime", typeof(string));
//                        _BillingStatsTable.Columns.Add("LastCallTime", typeof(string));
//                        _BillingStatsTable.Columns.Add("MinDuration", typeof(decimal));
//                        _BillingStatsTable.Columns.Add("MaxDuration", typeof(decimal));
//                        _BillingStatsTable.Columns.Add("AvgDuration", typeof(decimal));
//                        _BillingStatsTable.Columns.Add("Cost_Nets", typeof(float));
//                        _BillingStatsTable.Columns.Add("Cost_Discounts", typeof(float));
//                        _BillingStatsTable.Columns.Add("Cost_Commissions", typeof(float));
//                        _BillingStatsTable.Columns.Add("Cost_ExtraCharges", typeof(float));
//                        _BillingStatsTable.Columns.Add("Sale_Nets", typeof(float));
//                        _BillingStatsTable.Columns.Add("Sale_Discounts", typeof(float));
//                        _BillingStatsTable.Columns.Add("Sale_Commissions", typeof(float));
//                        _BillingStatsTable.Columns.Add("Sale_ExtraCharges", typeof(float));
//                        _BillingStatsTable.Columns.Add("Sale_Rate", typeof(float));
//                        _BillingStatsTable.Columns.Add("Cost_Rate", typeof(float));
//                    }
//                }
//                return _BillingStatsTable;
//            }
//        }

//        protected static DataTable PricingTable
//        {
//            get
//            {
//                lock (s_DataTableMappingsLock)
//                {
//                    if (_PricingTable == null)
//                    {
//                        _PricingTable = new DataTable();
//                        _PricingTable.Columns.Add("ID", typeof(long));
//                        _PricingTable.Columns.Add("ZoneID", typeof(int));
//                        _PricingTable.Columns.Add("Net", typeof(float));
//                        _PricingTable.Columns.Add("CurrencyID", typeof(string));
//                        _PricingTable.Columns.Add("RateValue", typeof(float));
//                        _PricingTable.Columns.Add("RateID", typeof(long));
//                        _PricingTable.Columns.Add("Discount", typeof(float));
//                        _PricingTable.Columns.Add("RateType", typeof(byte));
//                        _PricingTable.Columns.Add("ToDConsiderationID", typeof(long));
//                        _PricingTable.Columns.Add("FirstPeriod", typeof(float));
//                        _PricingTable.Columns.Add("RepeatFirstperiod", typeof(byte));
//                        _PricingTable.Columns.Add("FractionUnit", typeof(byte));
//                        _PricingTable.Columns.Add("TariffID", typeof(long));
//                        _PricingTable.Columns.Add("CommissionValue", typeof(float));
//                        _PricingTable.Columns.Add("CommissionID", typeof(int));
//                        _PricingTable.Columns.Add("ExtraChargeValue", typeof(float));
//                        _PricingTable.Columns.Add("ExtraChargeID", typeof(int));
//                        _PricingTable.Columns.Add("Updated", typeof(DateTime));
//                        _PricingTable.Columns.Add("DurationInSeconds", typeof(decimal));
//                        _PricingTable.Columns.Add("Code", typeof(string));
//                        _PricingTable.Columns.Add("Attempt", typeof(DateTime));

//                    }
//                }
//                return _PricingTable;
//            }
//        }

//        #endregion
//    }
//}
