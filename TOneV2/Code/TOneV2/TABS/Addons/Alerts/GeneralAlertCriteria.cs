using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using TABS.Addons.Utilities;
using System.Xml.Serialization;
using System.Xml.Linq;

namespace TABS.Addons.Alerts
{

    [NamedAddon("General Alert Criteria", "Defines a general alert criteria based on selected Zone / Customer / Supplier / Release Code.")]
    [Serializable]
    public class GeneralAlertCriteria : Extensibility.IAlertCriteria, IXmlSerializable
    {
        public enum FilterNames
        {
            //underscores are replaced by spaces when serializing
            ReleaseCode,
            Zone,
            Customer,
            Supplier,
            Switch,
            //Customer_CLI_Check,
            Services_Flag,
            SupplierPort,
            CustomerPort
        }
        #region Members
        protected SerializableDictionary<string, object> _Filters = new SerializableDictionary<string, object>();
        protected SerializableDictionary<int, State> _ZoneStates = new SerializableDictionary<int, State>();
        protected SerializableDictionary<int, State> _PreviousZoneStates;
        protected TimeSpan? _AlertingTimeSpan;
        //protected int? _AlertingRunCount;
        protected DateTime? _LastChecked;
        protected DateTime? _FirstCheckeded;
        protected bool _IsEnabled;
        protected AlertLevel _AlertLevel = AlertLevel.Medium;
        //protected int _RunCount = 0;
        protected string _Tag;
        #endregion Members

        #region Properties
        public SerializableDictionary<string, object> Filters { get { return _Filters; } protected set { _Filters = value; } }
        public SerializableDictionary<int, State> ZoneStates { get { return _ZoneStates; } protected set { _ZoneStates = value; } }
        public SerializableDictionary<int, State> PreviousZoneStates { get { return _PreviousZoneStates == null ? ZoneStates : _PreviousZoneStates; } }
        public TimeSpan? AlertingTimeSpan { get { return _AlertingTimeSpan; } set { _AlertingTimeSpan = value; } }
        //public int? AlertingRunCount { get { return _AlertingRunCount; } set { _AlertingRunCount = value; } }
        public DateTime? LastChecked { get { return _LastChecked; } set { _LastChecked = value; } }
        public DateTime? FirstCheckeded { get { return _FirstCheckeded; } set { _FirstCheckeded = value; } }
        public bool IsEnabled { get { return _IsEnabled; } set { _IsEnabled = value; } }
        public AlertLevel AlertLevel { get { return _AlertLevel; } set { _AlertLevel = value; } }
        //public int RunCount { get { return _RunCount; } set { _RunCount = value; } }
        public string Tag { get { return _Tag; } set { _Tag = value; } }
        public string Source { get; set; }
        #endregion Properties

        #region Utility

        /// <summary>
        /// Create a filters summary for display 
        /// </summary>
        public string FiltersSummary
        {
            get
            {
                SerializableDictionary<string, object> clone = new SerializableDictionary<string, object>(Filters.Count);
                foreach (KeyValuePair<string, object> pair in Filters)
                {
                    if (pair.Value != null)
                    {
                        object value = pair.Value;
                        switch (pair.Key)
                        {
                            case "Zone": value = Zone; break;
                            case "Customer": value = Customer; break;
                            case "Supplier": value = Supplier; break;
                            case "Switch": value = Switch; break;
                            case "Services Flag": value = Components.FlaggedServicesEntity.GetServicesDisplayList(ServicesFlag.Value); break;
                        }
                        clone.Add(pair.Key, value);
                    }
                }
                return GetSerializableDictionarySummary(clone);
            }
        }

        public string StateSummary
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                foreach (int zoneID in PreviousThresholdsCrossedByZones.Keys)
                {
                    string zoneName =
                        TABS.ObjectAssembler.CurrentSession.Get<TABS.Zone>(zoneID).Name;
                    State state = PreviousZoneStates[zoneID];

                    sb.AppendFormat("Zone: {0} *-* ", zoneName);
                    sb.Append(state.ToString());
                    sb.Append(" *-* ");
                    sb.Append(" *-* ");
                }
                return sb.ToString();
            }
        }

        public string ThresholdsSummary
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                foreach (Threshold t in Thresholds)
                {
                    sb.Append(t.ToString());
                    sb.Append("_");
                }
                return sb.ToString();
            }
        }

        public string GetSerializableDictionarySummary(SerializableDictionary<string, object> collection)
        {
            StringBuilder result = new StringBuilder();
            if (collection != null && collection.Count > 0)
                foreach (KeyValuePair<string, object> pair in collection)
                {
                    if (pair.Value != null)
                        result
                            .Append(result.Length > 0 ? " *-* " : "")
                            .AppendFormat("{0}: {1}", pair.Key, pair.Value);
                }
            return result.ToString();
        }
        /// <summary>
        /// Try to get a value from a collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="colletion"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        protected T SafeGet<T>(SerializableDictionary<string, object> colletion, string key)
        {
            if (!colletion.ContainsKey(key) || colletion[key] == null) return default(T);
            else return (T)colletion[key];
        }

        protected T SafeGetFilter<T>(FilterNames key)
        {
            return SafeGet<T>(Filters, key.ToString().Replace("_", " "));//underscores are replaced by space for backward compat
        }

        protected void SetFilter(FilterNames key, object value)
        {
            Filters[key.ToString().Replace("_", " ")] = value;//underscores are replaced by space for backward compat
        }
        #endregion
        static log4net.ILog log = log4net.LogManager.GetLogger(typeof(GeneralAlertCriteria));

        [NonSerialized]
        protected IList<Extensibility.IAlert> _Alerts;
        public IList<Extensibility.IAlert> Alerts { get { return _Alerts; } set { _Alerts = value; } }

        #region Criteria
        [NonSerialized]
        private Zone _Zone;
        [NonSerialized]
        private CarrierAccount _Customer;
        [NonSerialized]
        private CarrierAccount _Supplier;
        [NonSerialized]
        private Switch _Switch;
        [NonSerialized]
        //private bool IsCLICheckFailed = false;
        private bool _ShouldSendEmail = false;
        private string _SendEmailTo = "";
        public bool ShouldSendEmail { get { return _ShouldSendEmail; } set { _ShouldSendEmail = value; } }
        public string SendEmailTo { get { return _SendEmailTo; } set { _SendEmailTo = value; } }

        public List<Threshold> Thresholds { get; set; }

        /// <summary>
        /// SerializableDictionary of thresholds where the key is the ID of the zone that
        /// has a state crossing the thresholds.
        /// </summary>
        public SerializableDictionary<int, List<Threshold>> ThresholdsCrossedByZones
        {
            get
            {
                SerializableDictionary<int, List<Threshold>> result = new SerializableDictionary<int, List<Threshold>>();
                if (this.Zone != null)
                {
                    if (!ZoneStates.ContainsKey(Zone.ZoneID)) return result;//empty collection
                    List<Threshold> crossed = new List<Threshold>();
                    foreach (Threshold threshold in Thresholds)
                    {
                        State zoneState = ZoneStates[Zone.ZoneID];
                        if (LastChecked.HasValue && threshold.CheckStateCrossed(zoneState, LastChecked.Value))
                            crossed.Add(threshold);
                    }
                    if (crossed.Count > 0)
                        result.Add(Zone.ZoneID, crossed);
                }
                else
                {
                    foreach (var kvp in ZoneStates)
                    {
                        List<Threshold> crossed = new List<Threshold>();
                        foreach (Threshold threshold in Thresholds)
                        {
                            State zoneState = kvp.Value;
                            if (LastChecked.HasValue && threshold.CheckStateCrossed(zoneState, LastChecked.Value))
                                crossed.Add(threshold);
                        }
                        if (crossed.Count > 0)
                            result.Add(kvp.Key, crossed);
                    }
                }

                return result;
            }
        }


        /// <summary>
        /// This is just to give the Crossed Thresholds before the state is reset
        /// </summary>
        public SerializableDictionary<int, List<Threshold>> PreviousThresholdsCrossedByZones
        {
            get
            {
                SerializableDictionary<int, List<Threshold>> result = new SerializableDictionary<int, List<Threshold>>();
                if (this.Zone != null)
                {
                    if (!PreviousZoneStates.ContainsKey(Zone.ZoneID)) return result;//empty collection
                    List<Threshold> crossed = new List<Threshold>();
                    foreach (Threshold threshold in Thresholds)
                    {
                        State zoneState = PreviousZoneStates[Zone.ZoneID];
                        if (LastChecked.HasValue && threshold.CheckStateCrossed(zoneState, LastChecked.Value))
                            crossed.Add(threshold);
                    }
                    if (crossed.Count > 0)
                        result.Add(Zone.ZoneID, crossed);
                }
                else
                {
                    foreach (var kvp in PreviousZoneStates)
                    {
                        List<Threshold> crossed = new List<Threshold>();
                        foreach (Threshold threshold in Thresholds)
                        {
                            State zoneState = kvp.Value;
                            if (LastChecked.HasValue && threshold.CheckStateCrossed(zoneState, LastChecked.Value))
                                crossed.Add(threshold);
                        }
                        if (crossed.Count > 0)
                            result.Add(kvp.Key, crossed);
                    }
                }

                return result;
            }
        }


        public string ReleaseCode
        {
            get { return SafeGetFilter<string>(FilterNames.ReleaseCode); }
            set { SetFilter(FilterNames.ReleaseCode, value); }
        }

        public Zone Zone
        {
            get
            {
                int? filterVal = SafeGetFilter<int?>(FilterNames.Zone);
                if (_Zone == null && filterVal != null)
                    _Zone = ObjectAssembler.Get<Zone>(filterVal.Value);
                return _Zone;
            }
            set { _Zone = value; SetFilter(FilterNames.Zone, (value == null) ? null : (int?)value.ZoneID); }
        }

        public CarrierAccount Customer
        {
            get
            {
                string filterVal = SafeGetFilter<string>(FilterNames.Customer);
                if (_Customer == null && filterVal != null)
                    _Customer = ObjectAssembler.Get<CarrierAccount>(filterVal);
                return _Customer;
            }
            set { _Customer = value; SetFilter(FilterNames.Customer, (value == null) ? null : value.CarrierAccountID); }
        }

        public CarrierAccount Supplier
        {
            get
            {
                string filterVal = SafeGetFilter<string>(FilterNames.Supplier);
                if (_Supplier == null && filterVal != null)
                    _Supplier = ObjectAssembler.Get<CarrierAccount>(filterVal);
                return _Supplier;
            }
            set { _Supplier = value; SetFilter(FilterNames.Supplier, (value == null) ? null : value.CarrierAccountID); }
        }
        public Switch Switch
        {
            get
            {
                int? filterVal = SafeGetFilter<int?>(FilterNames.Switch);
                if (_Switch == null && filterVal != null)
                    _Switch = ObjectAssembler.Get<Switch>(filterVal.Value);
                return _Switch;
            }
            set { _Switch = value; SetFilter(FilterNames.Switch, (value == null) ? null : (int?)value.SwitchID); }
        }


        //public bool? CheckForCLI { get { return SafeGetFilter<bool?>(FilterNames.Customer_CLI_Check); } set { SetFilter(FilterNames.Customer_CLI_Check, value); } }
        public short? ServicesFlag { get { return SafeGetFilter<short?>(FilterNames.Services_Flag); } set { SetFilter(FilterNames.Services_Flag, value); } }
        public string SupplierPort { get { return SafeGetFilter<string>(FilterNames.SupplierPort); } set { SetFilter(FilterNames.SupplierPort, value); } }
        public string CustomerPort { get { return SafeGetFilter<string>(FilterNames.CustomerPort); } set { SetFilter(FilterNames.CustomerPort, value); } }
        #endregion Criteria



        /// <summary>
        /// Get Alerts related to timing issues. This should be called periodically 
        /// to test for alerts when time advances and probably traffic is not issued
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TABS.Extensibility.IAlert> GetTimeAlerts()
        {
            // If not flagged as started, flag it now
            if (FirstCheckeded == null) FirstCheckeded = DateTime.Now;

            // Create an alert list
            List<Extensibility.IAlert> alerts = new List<Extensibility.IAlert>();

            // Do not generate alerts for CLI
            //IsCLICheckFailed = false;

            // Generate alerts according to criteria and state (and time)
            NewGenerateAlerts(alerts);

            // return results
            return alerts;
        }


        /// <summary>
        /// Process the traffic and generate any possible alerts
        /// </summary>
        /// <param name="trafficStats">The Traffic to analyze</param>
        /// <returns>An enumerable collection of alerts</returns>
        public IEnumerable<TABS.Extensibility.IAlert> ProcessTraffic(IEnumerable<Billing_CDR_Base> cdrs)
        {
            return new List<TABS.Extensibility.IAlert>();

        }

        public IEnumerable<TABS.Extensibility.IAlert> ProcessTrafficStats(IEnumerable<TrafficStats> tStats)
        {
            if (FirstCheckeded == null) FirstCheckeded = DateTime.Now;
            bool shouldCheck = false;
            List<Extensibility.IAlert> alerts = new List<Extensibility.IAlert>();
            var start = DateTime.Now;
            var LastAttempt = tStats.Select(ts => ts.LastCDRAttempt).Max();
            foreach (TrafficStats ts in tStats)
            {
                if (passesCriteriaFilters(ts, LastAttempt))
                {
                    updateZonesFromTrafficStats(ts);
                    shouldCheck = true;
                }
            }

            if (shouldCheck) NewGenerateAlerts(alerts);

            return (IEnumerable<Extensibility.IAlert>)alerts;
        }


        public bool passesCriteriaFilters(TrafficStats ts, DateTime lastAttempt)
        {

            if (Zone != null && (ts.OurZone == null || !ts.OurZone.Equals(Zone))) return false;
            if (Customer != null && (ts.Customer.CarrierAccountID == null || !ts.Customer.CarrierAccountID.Equals(Customer.CarrierAccountID))) return false;
            if (Supplier != null && (ts.Supplier.CarrierAccountID == null || !ts.Supplier.CarrierAccountID.Equals(Supplier.CarrierAccountID))) return false;
            if (Switch != null && (ts.Switch == null || !ts.Switch.SwitchID.Equals(Switch.SwitchID))) return false;
            if (ServicesFlag != null && (ts.SupplierZone == null || !((ts.SupplierZone.ServicesFlag & ServicesFlag.Value) == ServicesFlag.Value))) return false;
            if (ReleaseCode != null && (ts.ReleaseCode == null || !ts.ReleaseCode.Equals(ReleaseCode))) return false;
            if (AlertingTimeSpan.HasValue)
                if (ts.LastCDRAttempt < lastAttempt.Subtract(AlertingTimeSpan.Value)) return false;
            //if (CheckForCLI != null && CheckForCLI.Value && ts.OriginatingZone == null) IsCLICheckFailed = true;
            if (!string.IsNullOrEmpty(SupplierPort) && (ts.Port_OUT == null || !ts.Port_OUT.Equals(SupplierPort)))
                return false;
            if (!string.IsNullOrEmpty(CustomerPort) && (ts.Port_IN == null || !ts.Port_IN.Equals(CustomerPort)))
                return false;

            return true;

        }

        public void updateZonesFromTrafficStats(TrafficStats ts)
        {
            //use SerializableDictionary<int, State> ZoneStates, where key is Zone.ZoneID
            //use cdr.OurZone.ZoneID to get a State from the above SerializableDictionary
            //update the fetched State
            //process this CDR only if it has a zone
            if (ts.OurZone != null)
            {
                int CDRZoneID = ts.OurZone.ZoneID;
                //only update state if we have a Zone and the cdr has same Zone or we don't have a Zone
                bool shouldUpdateState = ((Zone != null && CDRZoneID == Zone.ZoneID) || Zone == null);
                if (shouldUpdateState)
                {
                    State state;
                    //create a new state if one does not exist in our ZoneStates for zoneID as key
                    if (!ZoneStates.TryGetValue(CDRZoneID, out state))
                    {
                        state = new State();
                        ZoneStates.Add(CDRZoneID, state);
                    }
                    //udpating state (either existing or new state)
                    state.DurationsInSeconds += ts.DurationsInSeconds;
                    state.Attempts += ts.Attempts;
                    state.SuccessfulAttempts += ts.SuccessfulAttempts;
                    state.Average_PDD = ((ts.PDDInSeconds.Value * (decimal)ts.SuccessfulAttempts) + (state.Average_PDD != null ? state.Average_PDD * (decimal)(state.SuccessfulAttempts - ts.SuccessfulAttempts) : 0)) / (decimal)state.SuccessfulAttempts;
                    state.MaxDurationInSeconds = Math.Max(state.MaxDurationInSeconds, ts.DurationsInSeconds);
                }
            }
        }

        //private void GenerateAlerts(List<Extensibility.IAlert> newAlerts)
        //{

        //    LastChecked = DateTime.Now;
        //    bool criteriaChecked = false;
        //    bool newAlertsGenerated = CheckAndGenerateAlerts(out criteriaChecked, newAlerts);
        //    // If run count or time exceeded
        //    if (criteriaChecked)
        //    {
        //        IList<Extensibility.IAlert> previousAlerts = Alerts;
        //        Alerts = newAlerts;
        //        ResetZoneStates();
        //        if (newAlertsGenerated)
        //        {
        //            CheckAndSendEmail();

        //        }
        //        // Hide previous alerts from this Criteria
        //        if (previousAlerts != null)
        //            HideAlerts(previousAlerts);
        //    }
        //}

        private void NewGenerateAlerts(List<Extensibility.IAlert> newAlerts)
        {

            LastChecked = DateTime.Now;
            bool criteriaChecked = false;
            bool newAlertsGenerated = NewCheckAndGenerateAlerts(out criteriaChecked, newAlerts);
            // If run count or time exceeded
            if (criteriaChecked)
            {
                IList<Extensibility.IAlert> previousAlerts = Alerts;
                Alerts = newAlerts;
                ResetZoneStates();
                if (newAlertsGenerated)
                {
                    CheckAndSendEmail();

                }
                // Hide previous alerts from this Criteria
                if (previousAlerts != null)
                    HideAlerts(previousAlerts);
            }
        }

        private void HideAlerts(IList<Extensibility.IAlert> alertsToHide)
        {
            if (alertsToHide.Count > 0)
            {
                using (NHibernate.ISession session = DataConfiguration.OpenSession())
                {
                    NHibernate.ITransaction transaction = session.BeginTransaction();
                    foreach (Extensibility.IAlert alert in alertsToHide)
                    {
                        Alert persisted = alert as Alert;
                        if (persisted != null)
                        {
                            persisted.IsVisible = false;
                            session.SaveOrUpdate(persisted);
                        }
                    }
                    transaction.Commit();
                    session.Flush();
                    session.Clear();
                }
            }
        }



        /// <summary>
        /// Check current state against criteria and generate alerts if necessary
        /// </summary>
        /// <param name="alerts"></param>
        //private bool CheckAndGenerateAlerts(out bool isChecked, List<Extensibility.IAlert> alerts)
        //{
        //    ///use a foreach to loop through the ZoneStates SerializableDictionary and check every ZoneState
        //    ///if one ZoneState is found to match the thresholds then the alert is fired

        //    isChecked = false;

        //    // Cli Check
        //    if (IsCLICheckFailed)
        //        alerts.Add(
        //            new Alert
        //                (
        //                this.Source, string.Format("CLI Check failed for {0}", Customer),
        //                this.Tag, this.AlertLevel
        //                )
        //                  );

        //    bool runCountCriteriaApplies = (AlertingRunCount.HasValue && RunCount >= AlertingRunCount);
        //    bool timeSpanCriteriaApplies = (AlertingTimeSpan.HasValue && LastChecked.Value.Subtract(FirstCheckeded.Value) > AlertingTimeSpan.Value);

        //    if (runCountCriteriaApplies || timeSpanCriteriaApplies)
        //    {
        //        isChecked = true;
        //        foreach (var kvp in ThresholdsCrossedByZones)
        //        {
        //            int zoneID = kvp.Key;
        //            foreach (Threshold threshold in kvp.Value)
        //            {
        //                State state = ZoneStates[zoneID];
        //                string zoneName =
        //                    TABS.ObjectAssembler.CurrentSession.Get<TABS.Zone>(zoneID).Name;
        //                Alert alert =
        //                 new Alert
        //                    (
        //                    this.Source,
        //                    string.Format(
        //                        "[Filters] *-* {0} *-* *-* [State] *-* Zone: {1} *-* {2} *-* *-* [Threshold] *-* {3}",
        //                                FiltersSummary,
        //                                zoneName,
        //                                state.ToString(),
        //                                threshold.ToString()),
        //                    this.Tag,
        //                    this.AlertLevel
        //                    );

        //                //If there are no thresholds of type Min Attempts among the crossed thrsholds
        //                bool canCheckAverages = kvp.Value.Count(t => t.Type == ThresholdType.Min_Attempts) == 0;

        //                decimal? currentCrossingValue = null;
        //                switch (threshold.Type)
        //                {
        //                    case ThresholdType.Min_Attempts:
        //                        currentCrossingValue = state.Attempts;
        //                        alerts.Add(alert);
        //                        break;
        //                    case ThresholdType.Max_Attempts:
        //                        currentCrossingValue = state.Attempts;
        //                        alerts.Add(alert);
        //                        break;
        //                    case ThresholdType.Min_ASR:
        //                        currentCrossingValue = state.Current_ASR;
        //                        if (canCheckAverages)
        //                            alerts.Add(alert);
        //                        break;
        //                    case ThresholdType.Max_ASR:
        //                        currentCrossingValue = state.Current_ASR;
        //                        if (canCheckAverages)
        //                            alerts.Add(alert);
        //                        break;
        //                    case ThresholdType.Min_ACD:
        //                        currentCrossingValue = state.Current_ACD;
        //                        if (canCheckAverages)
        //                            alerts.Add(alert);
        //                        break;
        //                    case ThresholdType.Max_ACD:
        //                        currentCrossingValue = state.Current_ACD;
        //                        if (canCheckAverages)
        //                            alerts.Add(alert);
        //                        break;
        //                    case ThresholdType.Min_Duration:
        //                        currentCrossingValue = state.DurationsInSeconds;
        //                        alerts.Add(alert);
        //                        break;
        //                    case ThresholdType.Max_Duration:
        //                        currentCrossingValue = state.DurationsInSeconds;
        //                        alerts.Add(alert);
        //                        break;
        //                    case ThresholdType.PerCall_Duration:
        //                        currentCrossingValue = state.MaxDurationInSeconds;
        //                        alerts.Add(alert);
        //                        break;
        //                    case ThresholdType.Complex:
        //                        if (!string.IsNullOrEmpty(threshold.ProgressExpression))
        //                            currentCrossingValue = threshold.GetProgressValue(state);
        //                        alerts.Add(alert);
        //                        break;
        //                }
        //                if (currentCrossingValue.HasValue && threshold.LastCrossingZoneValues.ContainsKey(zoneID) && threshold.LastCrossingZoneValues[zoneID].HasValue)
        //                {
        //                    decimal lastCrossingValue = threshold.LastCrossingZoneValues[zoneID].Value;
        //                    if (currentCrossingValue.Value < lastCrossingValue) alert.Progress = AlertProgress.Negative;
        //                    if (currentCrossingValue.Value > lastCrossingValue) alert.Progress = AlertProgress.Positive;
        //                }
        //                threshold.LastCrossingZoneValues[zoneID] = currentCrossingValue;
        //            }
        //        }
        //    }
        //    return (alerts.Count > 0);
        //}

        private bool NewCheckAndGenerateAlerts(out bool isChecked, List<Extensibility.IAlert> alerts)
        {
            ///use a foreach to loop through the ZoneStates SerializableDictionary and check every ZoneState
            ///if one ZoneState is found to match the thresholds then the alert is fired

            isChecked = false;


            //bool timeSpanCriteriaApplies = (AlertingTimeSpan.HasValue && LastChecked.Value.Subtract(FirstCheckeded.Value) > AlertingTimeSpan.Value);

            //if (timeSpanCriteriaApplies)
            //{
            isChecked = true;
            foreach (var kvp in ThresholdsCrossedByZones)
            {
                int zoneID = kvp.Key;
                foreach (Threshold threshold in kvp.Value)
                {
                    State state = ZoneStates[zoneID];
                    string zoneName =
                        TABS.ObjectAssembler.CurrentSession.Get<TABS.Zone>(zoneID).Name;
                    Alert alert =
                     new Alert
                        (
                        this.Source,
                        string.Format(
                            "[Filters] *-* {0} *-* *-* [State] *-* Zone: {1} *-* {2} *-* *-* [Threshold] *-* {3}",
                                    FiltersSummary,
                                    zoneName,
                                    state.ToString(),
                                    threshold.ToString()),
                        this.Tag,
                        this.AlertLevel
                        );

                    //If there are no thresholds of type Min Attempts among the crossed thrsholds
                    bool canCheckAverages = kvp.Value.Count(t => t.Type == ThresholdType.Min_Attempts) == 0;

                    decimal? currentCrossingValue = null;
                    switch (threshold.Type)
                    {
                        case ThresholdType.Min_Attempts:
                            currentCrossingValue = state.Attempts;
                            alerts.Add(alert);
                            break;
                        case ThresholdType.Max_Attempts:
                            currentCrossingValue = state.Attempts;
                            alerts.Add(alert);
                            break;
                        case ThresholdType.Min_ASR:
                            currentCrossingValue = state.Current_ASR;
                            if (canCheckAverages)
                                alerts.Add(alert);
                            break;
                        case ThresholdType.Max_ASR:
                            currentCrossingValue = state.Current_ASR;
                            if (canCheckAverages)
                                alerts.Add(alert);
                            break;
                        case ThresholdType.Min_ACD:
                            currentCrossingValue = state.Current_ACD;
                            if (canCheckAverages)
                                alerts.Add(alert);
                            break;
                        case ThresholdType.Max_ACD:
                            currentCrossingValue = state.Current_ACD;
                            if (canCheckAverages)
                                alerts.Add(alert);
                            break;
                        case ThresholdType.Min_Duration:
                            currentCrossingValue = state.DurationsInSeconds;
                            alerts.Add(alert);
                            break;
                        case ThresholdType.Max_Duration:
                            currentCrossingValue = state.DurationsInSeconds;
                            alerts.Add(alert);
                            break;
                        case ThresholdType.PerCall_Duration:
                            currentCrossingValue = state.MaxDurationInSeconds;
                            alerts.Add(alert);
                            break;
                        case ThresholdType.Max_PDD:
                            currentCrossingValue = state.Average_PDD;
                            alerts.Add(alert);
                            break;
                        case ThresholdType.Min_PDD:
                            currentCrossingValue = state.Average_PDD;
                            alerts.Add(alert);
                            break;
                        case ThresholdType.Complex:
                            if (!string.IsNullOrEmpty(threshold.ProgressExpression))
                                currentCrossingValue = threshold.GetProgressValue(state);
                            alerts.Add(alert);
                            break;
                    }
                    if (currentCrossingValue.HasValue && threshold.LastCrossingZoneValues.ContainsKey(zoneID) && threshold.LastCrossingZoneValues[zoneID].HasValue)
                    {
                        decimal lastCrossingValue = threshold.LastCrossingZoneValues[zoneID].Value;
                        if (currentCrossingValue.Value < lastCrossingValue) alert.Progress = AlertProgress.Negative;
                        if (currentCrossingValue.Value > lastCrossingValue) alert.Progress = AlertProgress.Positive;
                    }
                    threshold.LastCrossingZoneValues[zoneID] = currentCrossingValue;
                }
            }
            //}
            return (alerts.Count > 0);
        }

        private void UpdateZoneStates(Billing_CDR_Base cdr)
        {
            //use SerializableDictionary<int, State> ZoneStates, where key is Zone.ZoneID
            //use cdr.OurZone.ZoneID to get a State from the above SerializableDictionary
            //update the fetched State
            //process this CDR only if it has a zone
            if (cdr.OurZone != null)
            {
                int CDRZoneID = cdr.OurZone.ZoneID;
                //only update state if we have a Zone and the cdr has same Zone or we don't have a Zone
                bool shouldUpdateState = ((Zone != null && CDRZoneID == Zone.ZoneID) || Zone == null);
                if (shouldUpdateState)
                {
                    State state;
                    //create a new state if one does not exist in our ZoneStates for zoneID as key
                    if (!ZoneStates.TryGetValue(CDRZoneID, out state))
                    {
                        state = new State();
                        ZoneStates.Add(CDRZoneID, state);
                    }
                    //udpating state (either existing or new state)
                    state.DurationsInSeconds += cdr.DurationInSeconds;
                    state.Attempts++;
                    state.SuccessfulAttempts += (cdr.DurationInSeconds > 0 ? 1 : 0);
                    state.MaxDurationInSeconds = Math.Max(state.MaxDurationInSeconds, cdr.DurationInSeconds);
                }
            }
        }
        public void ResetState()
        {
            ResetZoneStates();
        }
        public void ResetZoneStates()
        {
            _PreviousZoneStates = ZoneStates;
            ZoneStates = new SerializableDictionary<int, State>();
            //RunCount = 0;
            //IsCLICheckFailed = false;
            FirstCheckeded = null;
            LastChecked = null;
        }

        //private bool PassesCriteriaFilters(Billing_CDR_Base cdr)
        //{
        //    if (Zone != null && (cdr.OurZone == null || !cdr.OurZone.Equals(Zone))) return false;
        //    if (Customer != null && (cdr.CustomerID == null || !cdr.CustomerID.Equals(Customer.CarrierAccountID))) return false;
        //    if (Supplier != null && (cdr.SupplierID == null || !cdr.SupplierID.Equals(Supplier.CarrierAccountID))) return false;
        //    if (Switch != null && (cdr.Switch == null || !cdr.Switch.SwitchID.Equals(Switch.SwitchID))) return false;
        //    if (ServicesFlag != null && (cdr.SupplierZone == null || !((cdr.SupplierZone.ServicesFlag & ServicesFlag.Value) == ServicesFlag.Value))) return false;
        //    if (ReleaseCode != null && (cdr.ReleaseCode == null || !cdr.ReleaseCode.Equals(ReleaseCode))) return false;

        //    if (CheckForCLI != null && CheckForCLI.Value && cdr.OriginatingZone == null) IsCLICheckFailed = true;
        //    if (!string.IsNullOrEmpty(SupplierPort) && (cdr.Port_OUT == null || !cdr.Port_OUT.Equals(SupplierPort)))
        //        return false;
        //    if (!string.IsNullOrEmpty(CustomerPort) && (cdr.Port_IN == null || !cdr.Port_IN.Equals(CustomerPort)))
        //        return false;
        //    return true;
        //}



        /// <summary>
        /// Create an instance of a general alert criteria
        /// </summary>
        public GeneralAlertCriteria()
        {
            Thresholds = new List<Threshold>();
        }


        public void CheckAndSendEmail()
        {
            if (ShouldSendEmail)
            {
                try
                {
                    System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();

                    if (!string.IsNullOrEmpty(SendEmailTo.Trim())) SendEmailTo.Trim()
                                                                       .Split(',', ';')
                                                                       .ToList().ForEach(t => message.To.Add(t.Trim()));


                    message.Subject = "T.One - Alert" + ((this.Tag == null || this.Tag == "") ? "" : string.Format(" - {0}", this.Tag));
                    message.IsBodyHtml = true;
                    StringBuilder sb = new StringBuilder();
                    string sCheckCondition = "";
                    //if (AlertingRunCount.HasValue)
                    //    sCheckCondition = string.Format("Alerting Run Count: {0}", AlertingRunCount.Value);
                    //else
                    if (AlertingTimeSpan.HasValue)
                        sCheckCondition = string.Format("Alerting Time Span: {0}", AlertingTimeSpan.Value);

                    foreach (Alert alert in Alerts)
                    {
                        sb.AppendFormat("{0} *-* Created: {1}*-**-*", sCheckCondition, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                        sb.Append(alert.Description.Replace("*-*", "\r\n<br />"));
                        sb.Append("<hr />");
                    }
                    message.Body = sb.ToString();

                    Exception ex;
                    TABS.Components.EmailSender.Send(message, out ex);
                    if (ex != null)
                    {
                        log.Error(string.Format("Error Sending Email to Alert from {0} ", Source), ex);
                    }
                }
                catch (Exception ex)
                {
                    log.Error(string.Format("Error Creating Mail Message for {0} ", Source), ex);
                }
            }
        }

        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }
        static XmlSerializer filtersSerializer = new XmlSerializer(typeof(SerializableDictionary<string, object>), null, new Type[0], new XmlRootAttribute("Filters"), string.Empty);
        //XmlSerializer statesSerializer = new XmlSerializer(typeof(SerializableDictionary<int, State>), null, new Type[0], new XmlRootAttribute("ZoneStates"), string.Empty);
        static XmlSerializer thresholdSerializer = new XmlSerializer(typeof(Threshold), null, new Type[0], new XmlRootAttribute("Threshold"), string.Empty);


        public void ReadXml(System.Xml.XmlReader reader)
        {
            XElement criteriaXML = XElement.Load(reader);
            if (criteriaXML.IsEmpty || !criteriaXML.HasElements) return;

            if (criteriaXML.Element("Filters") != null && !criteriaXML.Element("Filters").IsEmpty)
                _Filters = (SerializableDictionary<string, object>)filtersSerializer.Deserialize(criteriaXML.Element("Filters").CreateReader());
            else
                _Filters = new SerializableDictionary<string, object>();

            //_ZoneStates = (SerializableDictionary<int, State>)statesSerializer.Deserialize(reader);

            if (criteriaXML.Element("AlertingTimeSpan") != null && !criteriaXML.Element("AlertingTimeSpan").IsEmpty)
                _AlertingTimeSpan = (TimeSpan?)new TimeSpan(long.Parse(criteriaXML.Element("AlertingTimeSpan").Value));

            _IsEnabled = (criteriaXML.Element("IsEnabled") != null && !criteriaXML.Element("IsEnabled").IsEmpty) ?
                (criteriaXML.Element("IsEnabled").Value == "1") ? true : false
                : false;

            _AlertLevel = (criteriaXML.Element("AlertLevel") != null && !criteriaXML.Element("AlertLevel").IsEmpty) ? (AlertLevel)int.Parse(criteriaXML.Element("AlertLevel").Value) : AlertLevel.Low;
            _Tag = (criteriaXML.Element("Tag") != null && !criteriaXML.Element("Tag").IsEmpty) ? criteriaXML.Element("Tag").Value : "";
            Source = (criteriaXML.Element("Source") != null && !criteriaXML.Element("Source").IsEmpty) ? criteriaXML.Element("Source").Value : "";
            ShouldSendEmail = (criteriaXML.Element("ShouldSendEmail") != null && !criteriaXML.Element("ShouldSendEmail").IsEmpty) ? int.Parse(criteriaXML.Element("ShouldSendEmail").Value) == 1 ? true : false : false;
            SendEmailTo = (criteriaXML.Element("SendEmailTo") != null && !criteriaXML.Element("SendEmailTo").IsEmpty) ? criteriaXML.Element("SendEmailTo").Value : "";
            if (criteriaXML.Element("Thresholds") != null && !criteriaXML.Element("Thresholds").IsEmpty)
            {
                if (Thresholds == null)
                    Thresholds = new List<Threshold>();

                criteriaXML.Element("Thresholds").Descendants("Threshold").ToList().ForEach(th =>
                {
                    var deserlializedThreshold = (Threshold)thresholdSerializer.Deserialize(th.CreateReader());
                    Thresholds.Add(deserlializedThreshold);
                });
            }

            //bool wasEmpty = reader.IsEmptyElement;
            //reader.Read();
            //if (wasEmpty)
            //    return;

            //_Filters = (SerializableDictionary<string, object>)filtersSerializer.Deserialize(reader);
            ////_ZoneStates = (SerializableDictionary<int, State>)statesSerializer.Deserialize(reader);

            //try
            //{
            //    _AlertingTimeSpan = (TimeSpan?)new TimeSpan(reader.ReadElementContentAsLong("AlertingTimeSpan", ""));
            //}
            //catch
            //{
            //}

            ////try
            ////{
            ////    _AlertingRunCount = (int?)reader.ReadElementContentAsInt("AlertingRunCount", "");
            ////}
            ////catch
            ////{
            ////}

            //_IsEnabled = reader.ReadElementContentAsBoolean("IsEnabled", "");
            //_AlertLevel = (AlertLevel)reader.ReadElementContentAsInt("AlertLevel", "");
            ////_RunCount = reader.ReadElementContentAsInt("RunCount", "");
            //_Tag = reader.ReadElementContentAsString("Tag", "");
            //Source = reader.ReadElementContentAsString("Source", "");
            //ShouldSendEmail = reader.ReadElementContentAsBoolean("ShouldSendEmail", "");
            //SendEmailTo = reader.ReadElementContentAsString("SendEmailTo", "");
            //reader.ReadStartElement("Thresholds");
            //while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
            //{
            //    if (Thresholds == null)
            //        Thresholds = new List<Threshold>();
            //    Thresholds.Add((Threshold)thresholdSerializer.Deserialize(reader));
            //}
            //reader.ReadEndElement();
            //reader.ReadEndElement();
        }
        /// <summary>
        /// XML related to an old structure of this class could still exist.
        /// This cleans old XML.
        /// </summary>
        public void Clean()
        {
            List<string> filtersToRemove = new List<string>();
            foreach (var item in Filters)
            {
                string[] filterNames = Enum.GetNames(typeof(FilterNames))
                    .Select(n => n.Replace("_", " "))
                    .ToArray();
                if (!filterNames.Contains(item.Key))
                    filtersToRemove.Add(item.Key);
            }
            foreach (string key in filtersToRemove)
                Filters.Remove(key);
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            DateTime dtStart = DateTime.Now;
            XmlSerializerNamespaces xmlnsEmpty = new XmlSerializerNamespaces();
            xmlnsEmpty.Add("", "");

            filtersSerializer.Serialize(writer, _Filters, xmlnsEmpty);
            //statesSerializer.Serialize(writer, _ZoneStates, xmlnsEmpty);

            writer.WriteElementString("AlertingTimeSpan",
                AlertingTimeSpan.HasValue ?
                AlertingTimeSpan.Value.Ticks.ToString() :
                string.Empty);

            //writer.WriteElementString("AlertingRunCount",
            //    AlertingRunCount.HasValue ?
            //    AlertingRunCount.Value.ToString() :
            //    string.Empty);
            writer.WriteElementString("IsEnabled", Convert.ToInt32(_IsEnabled).ToString());
            writer.WriteElementString("AlertLevel", ((int)AlertLevel).ToString());
            //writer.WriteElementString("RunCount", RunCount.ToString());
            writer.WriteElementString("Tag", Tag);
            writer.WriteElementString("Source", Source);
            writer.WriteElementString("ShouldSendEmail", Convert.ToInt32(ShouldSendEmail).ToString());
            writer.WriteElementString("SendEmailTo", SendEmailTo);

            writer.WriteStartElement("Thresholds");
            foreach (Threshold t in Thresholds)
                thresholdSerializer.Serialize(writer, t, xmlnsEmpty);
            writer.WriteEndElement();
            DateTime dtEnd = DateTime.Now;
        }

        #endregion
    }
}