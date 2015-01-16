using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace TABS.Components
{
    public partial class Engine
    {
        public static char[] PortSeparators = new char[] { ':' };


        public static string GetIPOrTrunc(string ioPort)
        {
            if (ioPort == null || ioPort == string.Empty) return "";
            return ioPort.Trim().Split(PortSeparators, StringSplitOptions.RemoveEmptyEntries)[0];
        }

        /// <summary>
        /// A container to hold the traffic stats for the last 3 hours
        /// </summary>
        internal static Dictionary<string, TrafficStats> _TrafficStatsCache = new Dictionary<string, TrafficStats>();

        /// <summary>
        /// A container to hold the Daily traffic stats 
        /// </summary>
        private static Dictionary<string, TrafficStats> _DailyTrafficStats;
        /// <summary>
        /// Returns the current size (Count of items) in the Traffic Stats Cache
        /// </summary>
        public static int TrafficCacheSize { get { return _TrafficStatsCache.Count; } }

        /// <summary>
        /// Returns the current size (Count of items) in the Traffic Stats Cache
        /// </summary>
        public static IEnumerable<TrafficStats> TrafficCache { get { return _TrafficStatsCache.Values; } }

        /// <summary>
        /// Remove Traffic Stats in the Cache older than allowed TrafficStatsCacheTime Timespan value (System Parameter)
        /// </summary>
        protected static void FlushTrafficStatsCache(Dictionary<string, TrafficStats> stats, TimeSpan allowedInCache)
        {
            List<String> removableStats = new List<string>();
            foreach (KeyValuePair<string, TrafficStats> pair in stats)
                if (DateTime.Now.Subtract(pair.Value.FirstCDRAttempt) > allowedInCache)
                    removableStats.Add(pair.Key);
                else
                    pair.Value.Saveable = false;
            foreach (string key in removableStats)
                stats.Remove(key);
        }

        internal static void GetTrafficStatsKey(Billing_CDR_Base cdr, int SampleMinutes, StringBuilder groupingKeyBuffer, out string portIn, out string portOut)
        {
            groupingKeyBuffer.Length = 0;

            portIn = GetIPOrTrunc(cdr.Port_IN);
            portOut = GetIPOrTrunc(cdr.Port_OUT);

            groupingKeyBuffer
                .Append(cdr.Switch.SwitchID).Append('\t')
                .Append(portIn).Append('\t')
                .Append(portOut).Append('\t')
                .Append(cdr.CustomerID).Append('\t')
                .Append(cdr.OurZone == null ? "" : cdr.OurZone.ZoneID.ToString()).Append('\t')
                //.Append(cdr.OriginatingZone == null ? "" : cdr.OriginatingZone.ZoneID.ToString()).Append('\t')
                .Append(cdr.SupplierID).Append('\t')
                .Append(cdr.SupplierZone == null ? "" : cdr.SupplierZone.ZoneID.ToString()).Append('\t')
                .Append(string.Format("{0}{1}", cdr.Attempt.ToString("yyyyMMddHH"), cdr.Attempt.Minute / SampleMinutes));
        }


        internal static void GetDailyTrafficStatsKey(Billing_CDR_Base cdr, StringBuilder groupingKeyBuffer, out string portIn, out string portOut)
        {
            groupingKeyBuffer.Length = 0;

            portIn = GetIPOrTrunc(cdr.Port_IN);
            portOut = GetIPOrTrunc(cdr.Port_OUT);

            groupingKeyBuffer
                .Append(cdr.CustomerID).Append('\t')
                .Append(cdr.OurZone == null ? "" : cdr.OurZone.ZoneID.ToString()).Append('\t')
                .Append(cdr.OriginatingZone == null ? "" : cdr.OriginatingZone.ZoneID.ToString()).Append('\t')
                .Append(cdr.SupplierID).Append('\t')
                .Append(cdr.SupplierZone == null ? "" : cdr.SupplierZone.ZoneID.ToString()).Append('\t')
                .Append(string.Format("{0}", cdr.Attempt.ToString("yyyyMMdd")));
        }

        internal static void GetDailyTrafficStatsOriginatingKey(Billing_CDR_Base cdr, StringBuilder groupingKeyBuffer, out string portIn, out string portOut)
        {
            groupingKeyBuffer.Length = 0;

            portIn = GetIPOrTrunc(cdr.Port_IN);
            portOut = GetIPOrTrunc(cdr.Port_OUT);

            groupingKeyBuffer
                .Append(cdr.CustomerID).Append('\t')
                .Append(cdr.OriginatingZone == null ? "" : cdr.OriginatingZone.ZoneID.ToString()).Append('\t')
                .Append(cdr.SupplierID).Append('\t')
                .Append(cdr.SupplierZone == null ? "" : cdr.SupplierZone.ZoneID.ToString()).Append('\t')
                .Append(string.Format("{0}", cdr.Attempt.ToString("yyyyMMdd")));
        }

        public static void FlushInvalidEntries()
        {
            List<TrafficStats> listOfUpdatableStats = TABS.Components.Engine.TrafficCache.Where(s => s.ID > 0).ToList();

            //var cacheIds = listOfUpdatableStats.Select(s => s.ID.ToString()).ToList();
            //var InCondition = cacheIds.Aggregate((id1, id2) => id1 + "," + id2);

            //if (InCondition == string.Empty) return;

            //List<string> foundedIDs = new List<string>();
            //var sql = string.Format("SELECT ID FROM TrafficStats WITH(NOLOCK) WHERE ID in ({0})", InCondition);

            //var data = TABS.DataHelper.GetDataTable(sql);

            //foreach (System.Data.DataRow row in data.Rows)
            //    foundedIDs.Add(row[0].ToString());

            //var idsToBeremoved = cacheIds.Except(foundedIDs);

            //if(idsToBeremoved.Count() == 0) return;


            foreach (TABS.TrafficStats stat in listOfUpdatableStats)
            {
                object found = TABS.DataHelper.ExecuteScalar("SELECT ID FROM TrafficStats WITH(NOLOCK) WHERE ID = @P1", stat.ID);
                if (found == null || found == DBNull.Value)
                {
                    stat.ID = 0;
                    stat.Saveable = false;
                }
            }
        }
        /// <summary>
        /// Get the traffic statistics from the given CDRs
        /// </summary>
        /// <param name="billingCDRs">The CDR sample to generate the stats from</param>
        /// <returns>An enumerable collection of Traffic Stats</returns>
        internal static void UpdateTrafficStats_old(int SampleMinutes, Dictionary<string, TrafficStats> stats, IList<Billing_CDR_Base> billingCDRs, bool autoFlush)
        {
            lock (ObjectAssembler.SyncRoot)
            {
                // Remove invalid cache entries
                if (autoFlush) FlushTrafficStatsCache(stats, SystemParameter.TrafficStatsCacheTime.TimeSpanValue.Value);

                StringBuilder groupingKeyBuffer = new StringBuilder(512);

                foreach (Billing_CDR_Base cdr in billingCDRs)
                {
                    string portIn, portOut;
                    GetTrafficStatsKey(cdr, SampleMinutes, groupingKeyBuffer, out portIn, out portOut);
                    string groupingKey = groupingKeyBuffer.ToString();

                    TrafficStats trafficStats = null;
                    if (!stats.TryGetValue(groupingKey, out trafficStats))
                    {
                        trafficStats = new TrafficStats();
                        stats[groupingKey] = trafficStats;

                        // Initialize Grouping Fields
                        if (cdr.CustomerID != null) trafficStats.Customer = CarrierAccount.All.ContainsKey(cdr.CustomerID) ? CarrierAccount.All[cdr.CustomerID] : null;
                        if (cdr.SupplierID != null) trafficStats.Supplier = CarrierAccount.All.ContainsKey(cdr.SupplierID) ? CarrierAccount.All[cdr.SupplierID] : null;
                        trafficStats.OriginatingZone = cdr.OriginatingZone;
                        trafficStats.OurZone = cdr.OurZone;
                        trafficStats.SupplierZone = cdr.SupplierZone;
                        trafficStats.Port_IN = portIn;
                        trafficStats.Port_OUT = portOut;
                        trafficStats.Switch = cdr.Switch;
                        trafficStats.FirstCDRAttempt = cdr.Attempt;
                        trafficStats.LastCDRAttempt = cdr.Attempt;
                        trafficStats.MaxDurationInSeconds = cdr.DurationInSeconds;
                        trafficStats.PDDInSeconds = 0;
                        trafficStats.PGAD = 0;
                        trafficStats.ReleaseSourceAParty = 0;
                    }

                    trafficStats.Saveable = true;

                    // Update Calculated fields
                    // Attempts
                    trafficStats.Attempts++;
                    // Calls (Non-Rerouted Calls)
                    if (!cdr.IsRerouted)
                    {
                        trafficStats.NumberOfCalls++;
                        if (cdr.DurationInSeconds > 0)
                            trafficStats.DeliveredNumberOfCalls++;
                        else if (cdr.ReleaseCode != null)
                        {
                            SwitchReleaseCode releaseCode = null;
                            if (cdr.Switch.ReleaseCodes.TryGetValue(cdr.ReleaseCode, out releaseCode))
                                if (releaseCode.IsDelivered)
                                    trafficStats.DeliveredNumberOfCalls++;
                        }
                    }

                    // Utilization
                    if (cdr.Disconnect.HasValue) trafficStats.Utilization = trafficStats.Utilization.Add(cdr.Disconnect.Value.Subtract(cdr.Attempt));

                    // Duration? then sucessful and delivered
                    if (cdr.DurationInSeconds > 0)
                    {
                        trafficStats.SuccessfulAttempts++;
                        trafficStats.DeliveredAttempts++;
                        // PDD 
                        if (cdr.PDDInSeconds > 0)
                        {
                            decimal n = (decimal)trafficStats.SuccessfulAttempts - 1;
                            trafficStats.PDDInSeconds = ((n * trafficStats.PDDInSeconds) + cdr.PDDInSeconds) / (n + 1);
                        }
                        if (cdr.Connect.HasValue)
                        {
                            decimal n = (decimal)trafficStats.SuccessfulAttempts - 1;
                            trafficStats.PGAD = ((n * trafficStats.PGAD) + cdr.Connect.Value.Subtract(cdr.Attempt).Seconds) / (n + 1);
                        }
                    }
                    else // No Duration check if release code can give us a hint about delivery
                    {
                        if (cdr.ReleaseCode != null)
                        {
                            SwitchReleaseCode releaseCode = null;
                            if (cdr.Switch.ReleaseCodes.TryGetValue(cdr.ReleaseCode, out releaseCode))
                                if (releaseCode.IsDelivered)
                                    trafficStats.DeliveredAttempts++;
                        }
                    }

                    // Sum up Durations
                    trafficStats.DurationsInSeconds += cdr.DurationInSeconds;

                    //Sum up ceiled durations
                    trafficStats.CeiledDuration += (int)Math.Ceiling(cdr.DurationInSeconds);

                    // Update Min/Max Date/ID of CDRs
                    if (cdr.Attempt > trafficStats.LastCDRAttempt) trafficStats.LastCDRAttempt = cdr.Attempt;
                    if (cdr.Attempt < trafficStats.FirstCDRAttempt) trafficStats.FirstCDRAttempt = cdr.Attempt;
                    if (cdr.DurationInSeconds >= trafficStats.MaxDurationInSeconds) trafficStats.MaxDurationInSeconds = cdr.DurationInSeconds;

                    //if (cdr.Connect.HasValue) trafficStats.PGAD = trafficStats.PGAD + cdr.Connect.Value.Subtract(cdr.Attempt).Seconds/billingCDRs.Count;

                    //if (trafficStats.Switch != null && !string.IsNullOrEmpty(trafficStats.Switch.SwitchManagerName))
                    //{
                    //    if (trafficStats.Switch.SwitchManagerName == "TABS.Addons.TelesSwitchLibrary.SwitchManager")
                    if (cdr.ReleaseSource != null && cdr.ReleaseSource.ToUpper().Equals("A")) trafficStats.ReleaseSourceAParty += 1;
                    //}
                }
            }
        }


        /// <summary>
        /// Get the traffic statistics from the given CDRs
        /// </summary>
        /// <param name="billingCDRs">The CDR sample to generate the stats from</param>
        /// <returns>An enumerable collection of Traffic Stats</returns>
        internal static void UpdateTrafficStats(int SampleMinutes, Dictionary<string, TrafficStats> stats, IList<Billing_CDR_Base> billingCDRs, bool autoFlush)
        {
            lock (ObjectAssembler.SyncRoot)
            {
                // Remove invalid cache entries
                if (autoFlush) FlushTrafficStatsCache(stats, SystemParameter.TrafficStatsCacheTime.TimeSpanValue.Value);

                StringBuilder groupingKeyBuffer = new StringBuilder(512);

                foreach (Billing_CDR_Base cdr in billingCDRs)
                {
                    string portIn, portOut;
                    GetTrafficStatsKey(cdr, SampleMinutes, groupingKeyBuffer, out portIn, out portOut);
                    string groupingKey = groupingKeyBuffer.ToString();

                    TrafficStats trafficStats = null;
                    if (!stats.TryGetValue(groupingKey, out trafficStats))
                    {
                        trafficStats = new TrafficStats();
                        stats[groupingKey] = trafficStats;

                        // Initialize Grouping Fields
                        if (cdr.CustomerID != null) trafficStats.Customer = CarrierAccount.All.ContainsKey(cdr.CustomerID) ? CarrierAccount.All[cdr.CustomerID] : null;
                        if (cdr.SupplierID != null) trafficStats.Supplier = CarrierAccount.All.ContainsKey(cdr.SupplierID) ? CarrierAccount.All[cdr.SupplierID] : null;
                        trafficStats.OriginatingZone = cdr.OriginatingZone;
                        trafficStats.OurZone = cdr.OurZone;
                        trafficStats.SupplierZone = cdr.SupplierZone;
                        trafficStats.Port_IN = portIn;
                        trafficStats.Port_OUT = portOut;
                        trafficStats.Switch = cdr.Switch;
                        trafficStats.FirstCDRAttempt = cdr.Attempt;
                        trafficStats.LastCDRAttempt = cdr.Attempt;
                        trafficStats.MaxDurationInSeconds = cdr.DurationInSeconds;
                        trafficStats.PDDInSeconds = 0;
                        trafficStats.PGAD = 0;
                        trafficStats.ReleaseSourceAParty = 0;
                    }

                    trafficStats.Saveable = true;

                    // Update Calculated fields
                    // Attempts
                    trafficStats.Attempts++;
                    // Calls (Non-Rerouted Calls)
                    if (!cdr.IsRerouted)
                    {
                        trafficStats.NumberOfCalls++;
                        if (cdr.DurationInSeconds > 0)
                            trafficStats.DeliveredNumberOfCalls++;
                        else if (cdr.ReleaseCode != null)
                        {
                            SwitchReleaseCode releaseCode = null;
                            if (cdr.Switch.ReleaseCodes.TryGetValue(cdr.ReleaseCode, out releaseCode))
                                if (releaseCode.IsDelivered)
                                    trafficStats.DeliveredNumberOfCalls++;
                        }
                    }

                    // Utilization
                    if (cdr.Disconnect.HasValue) trafficStats.Utilization = trafficStats.Utilization.Add(cdr.Disconnect.Value.Subtract(cdr.Attempt));

                    // Duration? then sucessful and delivered
                    if (cdr.DurationInSeconds > 0)
                    {
                        trafficStats.SuccessfulAttempts++;
                        trafficStats.DeliveredAttempts++;
                        // PDD 
                        if (cdr.PDDInSeconds > 0)
                        {
                            decimal n = (decimal)trafficStats.SuccessfulAttempts - 1;
                            trafficStats.PDDInSeconds = ((n * trafficStats.PDDInSeconds) + cdr.PDDInSeconds) / (n + 1);
                        }
                        if (cdr.Connect.HasValue)
                        {
                            decimal n = (decimal)trafficStats.SuccessfulAttempts - 1;
                            trafficStats.PGAD = ((n * trafficStats.PGAD) + cdr.Connect.Value.Subtract(cdr.Attempt).Seconds) / (n + 1);
                        }
                    }
                    else // No Duration check if release code can give us a hint about delivery
                    {
                        if (cdr.ReleaseCode != null)
                        {
                            //old code
                            //SwitchReleaseCode releaseCode = null;
                            //if (cdr.Switch.ReleaseCodes.TryGetValue(cdr.ReleaseCode, out releaseCode))
                            //new code
                            SwitchReleaseCode releaseCode = null;
                            if (SwitchReleaseCode.All.ContainsKey(cdr.Switch))
                                if (SwitchReleaseCode.All[cdr.Switch].ContainsKey(cdr.ReleaseCode))
                                    releaseCode = SwitchReleaseCode.All[cdr.Switch][cdr.ReleaseCode];

                            if (releaseCode != null)
                                if (releaseCode.IsDelivered)
                                    trafficStats.DeliveredAttempts++;
                        }
                    }

                    // Sum up Durations
                    trafficStats.DurationsInSeconds += cdr.DurationInSeconds;

                    //Sum up ceiled durations
                    trafficStats.CeiledDuration += (int)Math.Ceiling(cdr.DurationInSeconds);

                    // Update Min/Max Date/ID of CDRs
                    if (cdr.Attempt > trafficStats.LastCDRAttempt) trafficStats.LastCDRAttempt = cdr.Attempt;
                    if (cdr.Attempt < trafficStats.FirstCDRAttempt) trafficStats.FirstCDRAttempt = cdr.Attempt;
                    if (cdr.DurationInSeconds >= trafficStats.MaxDurationInSeconds) trafficStats.MaxDurationInSeconds = cdr.DurationInSeconds;

                    //if (cdr.Connect.HasValue) trafficStats.PGAD = trafficStats.PGAD + cdr.Connect.Value.Subtract(cdr.Attempt).Seconds/billingCDRs.Count;

                    //if (trafficStats.Switch != null && !string.IsNullOrEmpty(trafficStats.Switch.SwitchManagerName))
                    //{
                    //    if (trafficStats.Switch.SwitchManagerName == "TABS.Addons.TelesSwitchLibrary.SwitchManager")
                    if (cdr.ReleaseSource != null && cdr.ReleaseSource.ToUpper().Equals("A")) trafficStats.ReleaseSourceAParty += 1;
                    //}
                }
            }
        }


        /// <summary>
        /// Get the traffic statistics from the given CDRs
        /// </summary>
        /// <param name="billingCDRs">The CDR sample to generate the stats from</param>
        /// <returns>An enumerable collection of Traffic Stats</returns>
        internal static void UpdateDailyTrafficStats(Dictionary<string, TrafficStats> stats, IList<Billing_CDR_Base> billingCDRs, bool autoFlush)
        {
            lock (ObjectAssembler.SyncRoot)
            {
                // Remove invalid cache entries
                if (autoFlush) FlushTrafficStatsCache(stats, SystemParameter.TrafficStatsCacheTime.TimeSpanValue.Value);

                StringBuilder groupingKeyBuffer = new StringBuilder(512);

                foreach (Billing_CDR_Base cdr in billingCDRs)
                {
                    string portIn, portOut;
                    GetDailyTrafficStatsKey(cdr, groupingKeyBuffer, out portIn, out portOut);
                    string groupingKey = groupingKeyBuffer.ToString();

                    TrafficStats DialytrafficStats = null;
                    if (!stats.TryGetValue(groupingKey, out DialytrafficStats))
                    {
                        DialytrafficStats = new TrafficStats();
                        stats[groupingKey] = DialytrafficStats;

                        // Initialize Grouping Fields
                        DialytrafficStats.CallDate = DateTime.Parse(cdr.Attempt.ToString("yyyy-MM-dd"));
                        DialytrafficStats.Switch = cdr.Switch;
                        if (cdr.CustomerID != null) DialytrafficStats.Customer = CarrierAccount.All.ContainsKey(cdr.CustomerID) ? CarrierAccount.All[cdr.CustomerID] : null;
                        if (cdr.SupplierID != null) DialytrafficStats.Supplier = CarrierAccount.All.ContainsKey(cdr.SupplierID) ? CarrierAccount.All[cdr.SupplierID] : null;
                        //trafficStats.OriginatingZone = cdr.OriginatingZone;
                        DialytrafficStats.OurZone = cdr.OurZone;
                        DialytrafficStats.OriginatingZone = cdr.OriginatingZone;
                        DialytrafficStats.SupplierZone = cdr.SupplierZone;
                        DialytrafficStats.Port_IN = portIn;
                        DialytrafficStats.Port_OUT = portOut;


                        DialytrafficStats.FirstCDRAttempt = cdr.Attempt;
                        DialytrafficStats.LastCDRAttempt = cdr.Attempt;
                        DialytrafficStats.MaxDurationInSeconds = cdr.DurationInSeconds;
                        DialytrafficStats.PDDInSeconds = 0;
                        DialytrafficStats.PGAD = 0;
                        DialytrafficStats.ReleaseSourceAParty = 0;
                    }

                    DialytrafficStats.Saveable = true;

                    // Update Calculated fields
                    // Attempts
                    DialytrafficStats.Attempts++;
                    // Calls (Non-Rerouted Calls)
                    if (!cdr.IsRerouted)
                    {
                        DialytrafficStats.NumberOfCalls++;
                        if (cdr.ReleaseCode != null && cdr.DurationInSeconds == 0)
                        {
                            SwitchReleaseCode releaseCode = null;
                            if (cdr.Switch.ReleaseCodes.TryGetValue(cdr.ReleaseCode, out releaseCode))
                                if (releaseCode.IsDelivered)
                                    DialytrafficStats.DeliveredNumberOfCalls++;
                        }
                    }

                    // Utilization
                    if (cdr.Disconnect.HasValue) DialytrafficStats.Utilization = DialytrafficStats.Utilization.Add(cdr.Disconnect.Value.Subtract(cdr.Attempt));

                    // Duration? then sucessful and delivered
                    if (cdr.DurationInSeconds > 0)
                    {
                        DialytrafficStats.SuccessfulAttempts++;
                        DialytrafficStats.DeliveredAttempts++;
                        // PDD 
                        if (cdr.PDDInSeconds > 0)
                        {
                            decimal n = (decimal)DialytrafficStats.SuccessfulAttempts - 1;
                            DialytrafficStats.PDDInSeconds = ((n * DialytrafficStats.PDDInSeconds) + cdr.PDDInSeconds) / (n + 1);
                        }
                        if (cdr.Connect.HasValue)
                        {
                            decimal n = (decimal)DialytrafficStats.SuccessfulAttempts - 1;
                            DialytrafficStats.PGAD = ((n * DialytrafficStats.PGAD) + cdr.Connect.Value.Subtract(cdr.Attempt).Seconds) / (n + 1);
                        }
                    }
                    else // No Duration check if release code can give us a hint about delivery
                    {
                        if (cdr.ReleaseCode != null)
                        {
                            SwitchReleaseCode releaseCode = null;
                            if (cdr.Switch.ReleaseCodes.TryGetValue(cdr.ReleaseCode, out releaseCode))
                                if (releaseCode.IsDelivered)
                                    DialytrafficStats.DeliveredAttempts++;
                        }
                    }


                    // Sum up Durations
                    DialytrafficStats.DurationsInSeconds += cdr.DurationInSeconds;

                    //Sum up Ceiled Durations
                    DialytrafficStats.CeiledDuration += (int)Math.Ceiling(cdr.DurationInSeconds);

                    // Update Min/Max Date/ID of CDRs
                    if (cdr.Attempt > DialytrafficStats.LastCDRAttempt) DialytrafficStats.LastCDRAttempt = cdr.Attempt;
                    if (cdr.Attempt < DialytrafficStats.FirstCDRAttempt) DialytrafficStats.FirstCDRAttempt = cdr.Attempt;
                    if (cdr.DurationInSeconds >= DialytrafficStats.MaxDurationInSeconds) DialytrafficStats.MaxDurationInSeconds = cdr.DurationInSeconds;
                    //if (cdr.Connect.HasValue) trafficStats.PGAD = trafficStats.PGAD + cdr.Connect.Value.Subtract(cdr.Attempt).Seconds/billingCDRs.Count;                   

                    //if (DialytrafficStats.Switch != null && !string.IsNullOrEmpty(DialytrafficStats.Switch.SwitchManagerName))
                    //{
                    //    if (DialytrafficStats.Switch.SwitchManagerName == "TABS.Addons.TelesSwitchLibrary.SwitchManager")
                    if (cdr.ReleaseSource != null && cdr.ReleaseSource.ToUpper().Equals("A")) DialytrafficStats.ReleaseSourceAParty += 1;
                    //}
                }
            }
        }


        /// <summary>
        /// Get the traffic statistics from the given CDRs
        /// </summary>
        /// <param name="billingCDRs">The CDR sample to generate the stats from</param>
        /// <returns>An enumerable collection of Traffic Stats</returns>
        internal static void UpdateDailyOriginatingTrafficStats(Dictionary<string, TrafficStats> stats, IList<Billing_CDR_Base> billingCDRs, bool autoFlush)
        {
            lock (ObjectAssembler.SyncRoot)
            {
                // Remove invalid cache entries
                if (autoFlush) FlushTrafficStatsCache(stats, SystemParameter.TrafficStatsCacheTime.TimeSpanValue.Value);

                StringBuilder groupingKeyBuffer = new StringBuilder(512);

                foreach (Billing_CDR_Base cdr in billingCDRs)
                {
                    string portIn, portOut;
                    GetDailyTrafficStatsOriginatingKey(cdr, groupingKeyBuffer, out portIn, out portOut);
                    string groupingKey = groupingKeyBuffer.ToString();

                    TrafficStats DialyOriginatingtrafficStats = null;
                    if (!stats.TryGetValue(groupingKey, out DialyOriginatingtrafficStats))
                    {
                        DialyOriginatingtrafficStats = new TrafficStats();
                        stats[groupingKey] = DialyOriginatingtrafficStats;

                        // Initialize Grouping Fields
                        DialyOriginatingtrafficStats.CallDate = DateTime.Parse(cdr.Attempt.ToString("yyyy-MM-dd"));
                        DialyOriginatingtrafficStats.Switch = cdr.Switch;
                        if (cdr.CustomerID != null) DialyOriginatingtrafficStats.Customer = CarrierAccount.All.ContainsKey(cdr.CustomerID) ? CarrierAccount.All[cdr.CustomerID] : null;
                        if (cdr.SupplierID != null) DialyOriginatingtrafficStats.Supplier = CarrierAccount.All.ContainsKey(cdr.SupplierID) ? CarrierAccount.All[cdr.SupplierID] : null;
                        DialyOriginatingtrafficStats.OriginatingZone = cdr.OriginatingZone;
                        DialyOriginatingtrafficStats.SupplierZone = cdr.SupplierZone;
                        DialyOriginatingtrafficStats.Port_IN = portIn;
                        DialyOriginatingtrafficStats.Port_OUT = portOut;


                        DialyOriginatingtrafficStats.FirstCDRAttempt = cdr.Attempt;
                        DialyOriginatingtrafficStats.LastCDRAttempt = cdr.Attempt;
                        DialyOriginatingtrafficStats.MaxDurationInSeconds = cdr.DurationInSeconds;
                        DialyOriginatingtrafficStats.PDDInSeconds = 0;
                        DialyOriginatingtrafficStats.PGAD = 0;
                        DialyOriginatingtrafficStats.ReleaseSourceAParty = 0;
                    }

                    DialyOriginatingtrafficStats.Saveable = true;

                    // Update Calculated fields
                    // Attempts
                    DialyOriginatingtrafficStats.Attempts++;
                    // Calls (Non-Rerouted Calls)
                    if (!cdr.IsRerouted)
                    {
                        DialyOriginatingtrafficStats.NumberOfCalls++;
                        if (cdr.ReleaseCode != null && cdr.DurationInSeconds == 0)
                        {
                            SwitchReleaseCode releaseCode = null;
                            if (cdr.Switch.ReleaseCodes.TryGetValue(cdr.ReleaseCode, out releaseCode))
                                if (releaseCode.IsDelivered)
                                    DialyOriginatingtrafficStats.DeliveredNumberOfCalls++;
                        }
                    }

                    // Utilization
                    if (cdr.Disconnect.HasValue) DialyOriginatingtrafficStats.Utilization = DialyOriginatingtrafficStats.Utilization.Add(cdr.Disconnect.Value.Subtract(cdr.Attempt));

                    // Duration? then sucessful and delivered
                    if (cdr.DurationInSeconds > 0)
                    {
                        DialyOriginatingtrafficStats.SuccessfulAttempts++;
                        DialyOriginatingtrafficStats.DeliveredAttempts++;
                        // PDD 
                        if (cdr.PDDInSeconds > 0)
                        {
                            decimal n = (decimal)DialyOriginatingtrafficStats.SuccessfulAttempts - 1;
                            DialyOriginatingtrafficStats.PDDInSeconds = ((n * DialyOriginatingtrafficStats.PDDInSeconds) + cdr.PDDInSeconds) / (n + 1);
                        }
                        if (cdr.Connect.HasValue)
                        {
                            decimal n = (decimal)DialyOriginatingtrafficStats.SuccessfulAttempts - 1;
                            DialyOriginatingtrafficStats.PGAD = ((n * DialyOriginatingtrafficStats.PGAD) + cdr.Connect.Value.Subtract(cdr.Attempt).Seconds) / (n + 1);
                        }
                    }
                    else // No Duration check if release code can give us a hint about delivery
                    {
                        if (cdr.ReleaseCode != null)
                        {
                            SwitchReleaseCode releaseCode = null;
                            if (cdr.Switch.ReleaseCodes.TryGetValue(cdr.ReleaseCode, out releaseCode))
                                if (releaseCode.IsDelivered)
                                    DialyOriginatingtrafficStats.DeliveredAttempts++;
                        }
                    }


                    // Sum up Durations
                    DialyOriginatingtrafficStats.DurationsInSeconds += cdr.DurationInSeconds;

                    //sum up ceiled durations
                    DialyOriginatingtrafficStats.CeiledDuration += (int)Math.Ceiling(cdr.DurationInSeconds);

                    // Update Min/Max Date/ID of CDRs
                    if (cdr.Attempt > DialyOriginatingtrafficStats.LastCDRAttempt) DialyOriginatingtrafficStats.LastCDRAttempt = cdr.Attempt;
                    if (cdr.Attempt < DialyOriginatingtrafficStats.FirstCDRAttempt) DialyOriginatingtrafficStats.FirstCDRAttempt = cdr.Attempt;
                    if (cdr.DurationInSeconds >= DialyOriginatingtrafficStats.MaxDurationInSeconds) DialyOriginatingtrafficStats.MaxDurationInSeconds = cdr.DurationInSeconds;
                    //if (cdr.Connect.HasValue) trafficStats.PGAD = trafficStats.PGAD + cdr.Connect.Value.Subtract(cdr.Attempt).Seconds;

                    //if (DialyOriginatingtrafficStats.Switch != null && !string.IsNullOrEmpty(DialyOriginatingtrafficStats.Switch.SwitchManagerName))
                    //{
                    //    if (DialyOriginatingtrafficStats.Switch.SwitchManagerName == "TABS.Addons.TelesSwitchLibrary.SwitchManager")
                    if (cdr.ReleaseSource != null && cdr.ReleaseSource.ToUpper().Equals("A")) DialyOriginatingtrafficStats.ReleaseSourceAParty += 1;
                    //}
                }
            }
        }
    }
}
