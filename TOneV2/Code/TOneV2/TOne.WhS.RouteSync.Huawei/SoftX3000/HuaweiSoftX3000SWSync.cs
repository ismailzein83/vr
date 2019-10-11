using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.RouteSync.Business;
using TOne.WhS.RouteSync.Entities;
using TOne.WhS.RouteSync.Huawei.SoftX3000.Business;
using TOne.WhS.RouteSync.Huawei.SoftX3000.Entities;
using TOne.WhS.RouteSync.MainExtensions.ChargeCode;
using TOne.WhS.RouteSync.MainExtensions.NumberLength;
using Vanrise.Common;

namespace TOne.WhS.RouteSync.Huawei.SoftX3000
{
    public class HuaweiSoftX3000SWSync : SwitchRouteSynchronizer
    {
        public override Guid ConfigId { get { return new Guid("2574530B-B9FD-44E2-BD0C-D1F46DB80E68"); } }

        public override bool SupportSyncWithinRouteBuild { get { return false; } }

        public SwitchSettings Settings { get; set; }

        public int NumberOfOptions { get; set; }

        public Dictionary<string, CarrierMapping> CarrierMappings { get; set; }

        public List<HuaweiSSHCommunication> SwitchCommunicationList { get; set; }

        public List<SwitchLogger> SwitchLoggerList { get; set; }


        private Dictionary<int, CustomerMapping> _customerMappingsByRSSC;

        private Dictionary<int, int> _customerIdsByRSSC;
        private Dictionary<int, CustomerMapping> CustomerMappingsByRSSC
        {
            get
            {
                if (_customerMappingsByRSSC != null)
                    return _customerMappingsByRSSC;

                if (CarrierMappings == null || CarrierMappings.Count == 0)
                    return null;

                _customerMappingsByRSSC = new Dictionary<int, CustomerMapping>();

                foreach (var carrierMappingKvp in CarrierMappings)
                {
                    var carrierMapping = carrierMappingKvp.Value;
                    if (carrierMapping.CustomerMapping != null)
                    {
                        _customerMappingsByRSSC.Add(carrierMapping.CustomerMapping.RSSC, carrierMapping.CustomerMapping);
                        _customerIdsByRSSC.Add(carrierMapping.CustomerMapping.RSSC, carrierMapping.CarrierId);
                    }
                }

                if (_customerMappingsByRSSC.Count == 0)
                    _customerMappingsByRSSC = null;

                return _customerMappingsByRSSC;
            }
        }

        #region Managers / Evaluators

        private NumberLengthEvaluator _numberLengthEvaluator;
        public NumberLengthEvaluator NumberLengthEvaluator
        {
            get
            {
                if (_numberLengthEvaluator != null)
                    _numberLengthEvaluator = new InternationalNumberLengthEvaluator();

                return _numberLengthEvaluator;
            }
        }

        private CodeChargeEvaluator _codeChargeEvaluator;
        public CodeChargeEvaluator CodeChargeEvaluator
        {
            get
            {
                if (_codeChargeEvaluator != null)
                    _codeChargeEvaluator = new InternationalCodeChargeEvaluator();

                return _codeChargeEvaluator;
            }
        }

        RouteOptionsManager _routeOptionsManager;
        RouteCaseManager _routeCaseManager;
        RouteManager _routeManager;
        SaleZoneManager _saleZoneManager;
        private object succeedRoutesWithCommandsByRSSN;

        #endregion

        #region Public Methods

        public override void Initialize(ISwitchRouteSynchronizerInitializeContext context)
        {
            WhSRouteSyncHuaweiManager whSRouteSyncHuaweiManager = new WhSRouteSyncHuaweiManager(context.SwitchId);
            whSRouteSyncHuaweiManager.Initialize();

            _routeOptionsManager = new RouteOptionsManager(context.SwitchId);
            _routeOptionsManager.Initialize();

            _routeCaseManager = new RouteCaseManager(context.SwitchId);
            _routeCaseManager.Initialize();

            _routeManager = new RouteManager(context.SwitchId);
            _routeManager.Initialize();

            _saleZoneManager = new SaleZoneManager();
        }

        public override void ConvertRoutes(ISwitchRouteSynchronizerConvertRoutesContext context)
        {
            if (context.Routes == null || context.Routes.Count == 0 || CarrierMappings == null || CarrierMappings.Count == 0)
                return;

            List<HuaweiConvertedRoute> convertedRoutes = new List<HuaweiConvertedRoute>();
            List<RouteOptions> routesOptionsToAdd = new List<RouteOptions>();
            List<RouteCaseToAdd> routeCasesToAdd = new List<RouteCaseToAdd>();

            Dictionary<string, RouteOptions> routesOptionsByName = _routeOptionsManager.GetCachedRoutesOptionsByName();
            Dictionary<string, Dictionary<int, RouteCase>> routeCasesByRSSCByRAN = _routeCaseManager.GetCachedRouteCasesByRSSCByRAN();

            foreach (var route in context.Routes)
            {
                CarrierMapping carrierMapping;
                if (!CarrierMappings.TryGetValue(route.CustomerId, out carrierMapping))
                    continue;

                var customerMapping = carrierMapping.CustomerMapping;
                if (customerMapping == null)
                    continue;

                RouteAnalysis routeAnalysis = this.GetRouteAnalysis(customerMapping.RSSC, route.Options);
                if (routeAnalysis == null)
                    continue;

                string routeOptionsAsString = Helper.SerializeRouteAnalysis(routeAnalysis);

                //if (routeOptionsAsString.CompareTo(HuaweiSoftX3000Commands.ROUTE_BLOCK) == 0)
                //    continue;

                if (routeOptionsAsString.CompareTo(HuaweiSoftX3000Commands.ROUTE_BLOCK) != 0)
                {
                    RouteOptions routeOptions;
                    if (!routesOptionsByName.TryGetValue(routeOptionsAsString, out routeOptions))
                        routesOptionsToAdd.Add(new RouteOptions { RouteOptionsAsString = routeOptionsAsString });

                    if (!Helper.RAN_RSSCExists(routeCasesByRSSCByRAN, routeOptionsAsString, customerMapping.RSSC))
                        routeCasesToAdd.Add(new RouteCaseToAdd() { RSSC = customerMapping.RSSC, RAN = routeOptionsAsString });
                }

                var codeData = TOne.WhS.RouteSync.Business.Helper.GetCodeData(route.Code, this.NumberLengthEvaluator, this.CodeChargeEvaluator);
                if (!int.TryParse(codeData.CodeCharge, out int cc))
                    throw new Exception($"Invalid Code Charge : {codeData.CodeCharge}");

                HuaweiConvertedRoute huaweiConvertedRoute = new HuaweiConvertedRoute()
                {
                    RSSC = customerMapping.RSSC,
                    Code = route.Code,
                    RAN = routeOptionsAsString,
                    DNSet = customerMapping.DNSet,
                    MaxL = codeData.MaxCodeLength,
                    MinL = codeData.MinCodeLength,
                    CC = cc
                };

                convertedRoutes.Add(huaweiConvertedRoute);
            }

            if (routesOptionsToAdd.Count > 0)
                routesOptionsByName = _routeOptionsManager.InsertAndGetRoutesOptions(routesOptionsToAdd, this.Settings);

            if (routeCasesToAdd.Count > 0)
                routeCasesByRSSCByRAN = _routeCaseManager.InsertAndGetRouteCases(routeCasesToAdd, this.Settings);

            if (convertedRoutes.Count == 0)
                return;

            HuaweiConvertedRoutesPayload huaweiConvertedRoutesPayload;
            if (context.ConvertedRoutesPayload != null)
                huaweiConvertedRoutesPayload = context.ConvertedRoutesPayload.CastWithValidate<HuaweiConvertedRoutesPayload>("context.ConvertedRoutesPayload");
            else
                huaweiConvertedRoutesPayload = new HuaweiConvertedRoutesPayload();

            foreach (var convertedRoute in convertedRoutes)
            {
                var routeCase = Helper.GetRouteCaseByRANThenRSSC(routeCasesByRSSCByRAN, convertedRoute.RAN, convertedRoute.RSSC);
                routeCase.ThrowIfNull($"Route Case is null for RAN '{convertedRoute.RAN}' and RSSC '{convertedRoute.RSSC}'");
                convertedRoute.RouteCaseId = routeCase.RouteCaseId;
            }

            huaweiConvertedRoutesPayload.ConvertedRoutes.AddRange(convertedRoutes);
            context.ConvertedRoutesPayload = huaweiConvertedRoutesPayload;
        }

        public override void onAllRoutesConverted(ISwitchRouteSynchronizerOnAllRoutesConvertedContext context)
        {
            if (context.ConvertedRoutesPayload == null)
                return;

            HuaweiConvertedRoutesPayload payload = context.ConvertedRoutesPayload.CastWithValidate<HuaweiConvertedRoutesPayload>("context.ConvertedRoutesPayload", context.SwitchId);
            if (payload.ConvertedRoutes == null || payload.ConvertedRoutes.Count == 0)
                return;

            context.ConvertedRoutes = RouteSync.Business.Helper.CompressRoutesWithCodes(payload.ConvertedRoutes, CreateHuaweiConvertedRoute);
        }

        public override object PrepareDataForApply(ISwitchRouteSynchronizerPrepareDataForApplyContext context)
        {
            var dbApplyStream = _routeManager.InitialiazeStreamForDBApply();

            foreach (var convertedRoute in context.ConvertedRoutes)
                _routeManager.WriteRecordToStream(convertedRoute as HuaweiConvertedRoute, dbApplyStream);

            return _routeManager.FinishDBApplyStream(dbApplyStream);
        }

        public override void ApplySwitchRouteSyncRoutes(ISwitchRouteSynchronizerApplyRoutesContext context)
        {
            _routeManager.ApplyRouteForDB(context.PreparedItemsForApply);
        }

        public override void Finalize(ISwitchRouteSynchronizerFinalizeContext context)
        {
            //Conditions
            if (CarrierMappings == null || CarrierMappings.Count == 0)
                return;

            string switchId = context.SwitchId;

            if (SwitchLoggerList == null || SwitchLoggerList.Count == 0)
                throw new Exception($"No logger specify for the switch with id {switchId}");

            var ftpLogger = SwitchLoggerList.First(item => item.IsActive);
            ftpLogger.ThrowIfNull($"No logger specify for the switch with id {switchId}");

            //Manager
            RouteSucceededManager routeSucceededManager = new RouteSucceededManager(switchId);

            //Communication
            HuaweiSSHCommunication huaweiSSHCommunication = null;
            if (SwitchCommunicationList != null)
                huaweiSSHCommunication = SwitchCommunicationList.FirstOrDefault(itm => itm.IsActive);

            SSHCommunicator sshCommunicator = null;
            if (huaweiSSHCommunication != null)
                sshCommunicator = new SSHCommunicator(huaweiSSHCommunication.SSHCommunicatorSettings);

            List<CommandResult> commandResults = new List<CommandResult>();

            int maxNumberOfTries = 0;
            if (sshCommunicator != null)
            {
                var configManager = new TOne.WhS.RouteSync.Business.ConfigManager();
                var huaweiSwitchRouteSynchronizerSettings = configManager.GetRouteSynchronizerSwitchSettings(ConfigId) as HuaweiSwitchRouteSynchronizerSettings;
                if (huaweiSwitchRouteSynchronizerSettings != null)
                    maxNumberOfTries = huaweiSwitchRouteSynchronizerSettings.NumberOfTries;
            }

            //Get Commands
            var notSyncedRoutesOptions = _routeOptionsManager.GetNotSyncedRoutesOptions();
            var notSyncedRouteCases = _routeCaseManager.GetNotSyncedRouteCases();

            var routesOptionsToBeAddedWithCommands = GetRoutesOptionsWithCommands(notSyncedRoutesOptions);
            var routeCasesToBeAddedWithCommands = GetRouteCasesWithCommands(notSyncedRouteCases);
            var routesWithCommandsByRSSC = GetRoutesWithCommandsByRSSC();

            //Execute and Log Route Options
            List<RouteOptionsWithCommands> succeedRouteOptionsWithCommands;
            List<RouteOptionsWithCommands> failedRouteOptionsWithCommands;

            ExecuteRouteOptionsCommands(routesOptionsToBeAddedWithCommands, huaweiSSHCommunication, sshCommunicator, commandResults, maxNumberOfTries,
                switchId, context.ProcessInstanceID, out succeedRouteOptionsWithCommands, out failedRouteOptionsWithCommands);

            DateTime finalizeTime = DateTime.Now;
            LogRouteOptionsCommands(succeedRouteOptionsWithCommands, failedRouteOptionsWithCommands, ftpLogger, finalizeTime);

            //Execute and Log Route Cases
            IEnumerable<long> failedRouteOptionsIds = failedRouteOptionsWithCommands?.Select(item => item.RouteOptions.RouteId);

            List<RouteCaseWithCommands> succeedRouteCasesWithCommands;
            List<RouteCaseWithCommands> failedRouteCasesWithCommands;

            ExecuteRouteCasesCommands(routeCasesToBeAddedWithCommands, huaweiSSHCommunication, sshCommunicator, commandResults, failedRouteOptionsIds, maxNumberOfTries,
                switchId, context.ProcessInstanceID, out succeedRouteCasesWithCommands, out failedRouteCasesWithCommands);

            finalizeTime = DateTime.Now;
            LogRouteCaseCommands(succeedRouteCasesWithCommands, failedRouteCasesWithCommands, ftpLogger, finalizeTime);

            //Execute and Log Routes
            IEnumerable<long> failedRouteCasesIds = failedRouteCasesWithCommands == null ? null : failedRouteCasesWithCommands.Select(item => item.RouteCase.RouteCaseId);

            Dictionary<int, List<HuaweiRouteWithCommands>> succeedRoutesWithCommandsByRSSC;
            Dictionary<int, List<HuaweiRouteWithCommands>> failedRoutesWithCommandsByRSSC;

            ExecuteRoutesCommands(routesWithCommandsByRSSC, huaweiSSHCommunication, sshCommunicator, commandResults, failedRouteCasesIds, maxNumberOfTries,
                switchId, context.ProcessInstanceID, out succeedRoutesWithCommandsByRSSC, out failedRoutesWithCommandsByRSSC);

            finalizeTime = DateTime.Now;
            LogHuaweiRouteCommands(succeedRoutesWithCommandsByRSSC, failedRoutesWithCommandsByRSSC, ftpLogger, finalizeTime);

            if (succeedRoutesWithCommandsByRSSC != null && succeedRoutesWithCommandsByRSSC.Count > 0) //save succeeded routes to succeeded table
                routeSucceededManager.SaveRoutesSucceededToDB(succeedRoutesWithCommandsByRSSC);

            if (failedRoutesWithCommandsByRSSC != null && failedRoutesWithCommandsByRSSC.Count > 0)
            {
                var failedAdded = new List<HuaweiConvertedRoute>();
                var failedUpdated = new List<HuaweiConvertedRoute>();
                var failedDeleted = new List<HuaweiConvertedRoute>();

                foreach (var failedRoutesWithCommands in failedRoutesWithCommandsByRSSC)
                {
                    foreach (var failedRoute in failedRoutesWithCommands.Value)
                    {
                        switch (failedRoute.ActionType)
                        {
                            case RouteActionType.Add: failedAdded.Add(failedRoute.RouteCompareResult.Route); break;
                            case RouteActionType.Update: failedUpdated.Add(failedRoute.RouteCompareResult.ExistingRoute); break;
                            case RouteActionType.Delete: failedDeleted.Add(failedRoute.RouteCompareResult.Route); break;
                        }
                    }
                }

                if (failedAdded != null && failedAdded.Count > 0)
                    _routeManager.RemoveRoutesFromTempTable(failedAdded);

                if (failedUpdated != null && failedUpdated.Count > 0)
                    _routeManager.UpdateRoutesInTempTable(failedUpdated);

                if (failedDeleted != null && failedDeleted.Count > 0)
                    _routeManager.InsertRoutesToTempTable(failedDeleted);
            }

            _routeManager.Finalize(new RouteFinalizeContext());

            //Logging AllCommands
            LogAllCommands(commandResults, ftpLogger, finalizeTime);
        }

        public override void RemoveConnection(ISwitchRouteSynchronizerRemoveConnectionContext context)
        {
            SwitchCommunicationList = null;
            SwitchLoggerList = null;
        }

        public override bool IsSwitchRouteSynchronizerValid(IIsSwitchRouteSynchronizerValidContext context)
        {
            if (this.CarrierMappings == null || this.CarrierMappings.Count == 0)
                return true;

            List<string> validationMessages = new List<string>();

            if (this.Settings == null)
            {
                validationMessages.Add($"Invalid Settings: Empty");
            }
            else
            {
                if (this.Settings.StartRouteCaseId == default(long))
                    validationMessages.Add($"Invalid Settings: Start RSC Number");

                if (this.Settings.StartRouteId == default(long))
                    validationMessages.Add($"Invalid Settings: Start Route Number");
            }

            HashSet<int> allRSSCs = new HashSet<int>();
            HashSet<int> duplicatedRSSCs = new HashSet<int>();

            HashSet<int> allSRTs = new HashSet<int>();
            HashSet<int> duplicatedSRTs = new HashSet<int>();

            foreach (var carrierMappingKvp in this.CarrierMappings)
            {
                CarrierMapping carrierMapping = carrierMappingKvp.Value;

                CustomerMapping customerMapping = carrierMapping.CustomerMapping;
                if (customerMapping != null)
                {
                    var isRSSCDuplicated = allRSSCs.Contains(customerMapping.RSSC);
                    if (!isRSSCDuplicated)
                        allRSSCs.Add(customerMapping.RSSC);
                    else
                        duplicatedRSSCs.Add(customerMapping.RSSC);
                }

                SupplierMapping supplierMapping = carrierMapping.SupplierMapping;
                if (supplierMapping != null)
                {
                    var isSRTDuplicated = allSRTs.Contains(supplierMapping.SRT);
                    if (!isSRTDuplicated)
                        allSRTs.Add(supplierMapping.SRT);
                    else
                        duplicatedSRTs.Add(supplierMapping.SRT);
                }
            }

            if (duplicatedRSSCs.Count > 0)
                validationMessages.Add($"Duplicated RSSCs: {string.Join(", ", duplicatedRSSCs)}");

            if (duplicatedSRTs.Count > 0)
                validationMessages.Add($"Duplicated SRTs: { string.Join(", ", duplicatedSRTs)}");

            context.ValidationMessages = validationMessages.Count > 0 ? validationMessages : null;
            return validationMessages.Count == 0;
        }

        #endregion

        #region Private Methods
        private RouteAnalysis GetRouteAnalysis(int rssc, List<RouteSync.Entities.RouteOption> routeOptions)
        {
            if (routeOptions == null || routeOptions.Count == 0)
                return null;

            bool isSequence = true;
            List<Entities.RouteCaseOption> routeCaseOptions = new List<Entities.RouteCaseOption>();

            foreach (var routeOption in routeOptions)
            {
                if (routeOption.IsBlocked)
                    continue;

                CarrierMapping carrierMapping;
                if (!CarrierMappings.TryGetValue(routeOption.SupplierId, out carrierMapping))
                    continue;

                var supplierMapping = carrierMapping.SupplierMapping;
                if (supplierMapping == null)
                    continue;

                if (!isSequence && (!routeOption.Percentage.HasValue || routeOption.Percentage.Value == 0))
                    continue;

                Entities.RouteCaseOption routeCaseOption = new Entities.RouteCaseOption();
                routeCaseOption.SRT = supplierMapping.SRT;

                if (routeOption.Percentage.HasValue)
                {
                    isSequence = false;
                    routeCaseOption.Percentage = routeOption.Percentage.Value;
                }

                if (routeOption.Backups != null && routeOption.Backups.Count > 0)
                {
                    foreach (var routeOptionBackup in routeOption.Backups)
                    {
                        if (routeOptionBackup.IsBlocked)
                            continue;

                        CarrierMapping carrierMappingBkup;
                        if (!CarrierMappings.TryGetValue(routeOptionBackup.SupplierId, out carrierMappingBkup))
                            continue;

                        var supplierMappingBkup = carrierMapping.SupplierMapping;
                        if (supplierMappingBkup == null)
                            continue;

                        routeCaseOptions.Add(new Entities.RouteCaseOption() { SRT = supplierMappingBkup.SRT });
                    }
                }

                routeCaseOptions.Add(routeCaseOption);

                if (routeCaseOptions.Count >= this.NumberOfOptions)
                    break;
            }

            if (routeCaseOptions.Count == 0)
                return null;

            RouteCaseOptionsType routeCaseOptionsType;
            if (isSequence)
            {
                routeCaseOptionsType = RouteCaseOptionsType.Sequence;
            }
            else
            {
                routeCaseOptionsType = RouteCaseOptionsType.Percentage;
                Utilities.RedistributePercentagePerWeight(routeCaseOptions.Select(itm => itm as IPercentageItem).ToList());
            }

            return new RouteAnalysis()
            {
                RSSC = rssc,
                RouteOptionsType = routeCaseOptionsType,
                RouteCaseOptions = routeCaseOptions.Count > 0 ? routeCaseOptions : null,
            };
        }

        private HuaweiConvertedRoute CreateHuaweiConvertedRoute(ICreateConvertedRouteWithCodeContext context)
        {
            CustomerMappingsByRSSC.ThrowIfNull("CustomerMappingsByRSSC");

            if (!int.TryParse(context.Customer, out int rssc))
                throw new Exception($"Invalid RSSC : {rssc}");

            if (!int.TryParse(context.RouteOptionIdentifier, out int routeCaseId))
                throw new Exception($"Invalid RSC : {routeCaseId}");

            var codeData = TOne.WhS.RouteSync.Business.Helper.GetCodeData(context.Code, this.NumberLengthEvaluator, this.CodeChargeEvaluator);

            if (!int.TryParse(codeData.CodeCharge, out int cc))
                throw new Exception($"Invalid Code Charge : {codeData.CodeCharge}");

            var customerMapping = CustomerMappingsByRSSC.GetRecord(rssc);
            return new HuaweiConvertedRoute()
            {
                DNSet = customerMapping.DNSet,
                RSSC = rssc,
                Code = context.Code,
                RouteCaseId = routeCaseId,
                MaxL = codeData.MaxCodeLength,
                MinL = codeData.MinCodeLength,
                CC = cc
            };
        }

        #region Route Options Commands
        private List<RouteOptionsWithCommands> GetRoutesOptionsWithCommands(IEnumerable<RouteOptions> routesOptions)
        {
            if (routesOptions == null || routesOptions.Count() == 0)
                return null;

            List<RouteOptionsWithCommands> routesOptionsWithCommands = new List<RouteOptionsWithCommands>();

            foreach (var routeOptions in routesOptions)
            {
                RouteOptionsWithCommands routeOptionsWithCommands = new RouteOptionsWithCommands()
                {
                    RouteOptions = routeOptions,
                    Commands = GetRouteOptionsCommands(routeOptions)
                };
                routesOptionsWithCommands.Add(routeOptionsWithCommands);
            }

            return routesOptionsWithCommands;
        }

        private List<string> GetRouteOptionsCommands(RouteOptions routeOptions)
        {
            routeOptions.ThrowIfNull("routeOptions");

            string routeOptionsAsString = routeOptions.RouteOptionsAsString;
            List<RouteCaseOption> routeCaseOptions = Helper.DeserializeRouteOptions(routeOptionsAsString, out bool isSequence);

            string addRTCommand = HuaweiSoftX3000Commands.Full_Add_RT;
            addRTCommand = addRTCommand.Replace(HuaweiSoftX3000Commands.RouteId_Pattern, routeOptions.RouteId.ToString());

            switch (isSequence)
            {
                case true:
                    addRTCommand = addRTCommand.Replace(HuaweiSoftX3000Commands.RouteCaseOptionsType_Pattern, HuaweiSoftX3000Commands.SRST_SEQ);
                    addRTCommand = addRTCommand.Replace(HuaweiSoftX3000Commands.PERCCFG_Pattern, "NO");
                    break;
                case false:
                    addRTCommand = addRTCommand.Replace(HuaweiSoftX3000Commands.RouteCaseOptionsType_Pattern, HuaweiSoftX3000Commands.SRST_PERC);
                    addRTCommand = addRTCommand.Replace(HuaweiSoftX3000Commands.PERCCFG_Pattern, "YES");
                    break;
            }

            List<string> routeSRTs = new List<string>();
            List<string> percentages = new List<string>();

            for (var index = 0; index < routeCaseOptions.Count; index++)
            {
                RouteCaseOption currentRouteCaseOption = routeCaseOptions[index];

                routeSRTs.Add(string.Format(HuaweiSoftX3000Commands.RT_SR, index + 1, currentRouteCaseOption.SRT));

                if (currentRouteCaseOption.Percentage.HasValue)
                    percentages.Add(string.Format(HuaweiSoftX3000Commands.RT_PERC, index + 1, currentRouteCaseOption.Percentage.Value));
            }

            string routeSRTsAsString = string.Join(", ", routeSRTs);
            string percentagesAsString = string.Empty;
            if (percentages.Count > 0)
                percentagesAsString = string.Concat(", ", string.Join(", ", percentages));

            addRTCommand = addRTCommand.Replace(HuaweiSoftX3000Commands.AllSR_Pattern, string.Concat(routeSRTsAsString, percentagesAsString));

            return new List<string>() { addRTCommand };
        }

        #endregion

        #region Route Cases Commands
        private List<RouteCaseWithCommands> GetRouteCasesWithCommands(IEnumerable<RouteCase> routeCases)
        {
            if (routeCases == null || routeCases.Count() == 0)
                return null;

            List<RouteCaseWithCommands> routeCasesWithCommands = new List<RouteCaseWithCommands>();

            foreach (var routeCase in routeCases)
            {
                RouteCaseWithCommands routeCaseWithCommands = new RouteCaseWithCommands()
                {
                    RouteCase = routeCase,
                    Commands = GetRouteCaseCommands(routeCase)
                };
                routeCasesWithCommands.Add(routeCaseWithCommands);
            }

            return routeCasesWithCommands;
        }

        private List<string> GetRouteCaseCommands(RouteCase routeCase)
        {
            routeCase.ThrowIfNull("routeCase");

            string addRTANACommand = HuaweiSoftX3000Commands.Full_ADD_RTANA;
            addRTANACommand = addRTANACommand.Replace(HuaweiSoftX3000Commands.RouteCaseId_Pattern, routeCase.RouteCaseId.ToString());
            addRTANACommand = addRTANACommand.Replace(HuaweiSoftX3000Commands.RouteId_Pattern, routeCase.RouteId.ToString());
            addRTANACommand = addRTANACommand.Replace(HuaweiSoftX3000Commands.RSSC_Pattern, routeCase.RSSC.ToString());

            return new List<string>() { addRTANACommand };
        }

        #endregion

        #region Route Commands

        private Dictionary<int, List<HuaweiRouteWithCommands>> GetRoutesWithCommandsByRSSC()
        {
            RouteCompareTablesContext routeCompareTablesContext = new RouteCompareTablesContext();
            _routeManager.CompareTables(routeCompareTablesContext);

            Dictionary<int, HuaweiConvertedRouteDifferences> routeDifferencesByRSSC = routeCompareTablesContext.RouteDifferencesByRSSC;
            if (routeDifferencesByRSSC == null || routeDifferencesByRSSC.Count == 0)
                return null;

            Dictionary<int, List<HuaweiRouteWithCommands>> results = new Dictionary<int, List<HuaweiRouteWithCommands>>();

            foreach (var routeDifferencesKvp in routeDifferencesByRSSC)
            {
                int RSSC = routeDifferencesKvp.Key;
                var routeDifferences = routeDifferencesKvp.Value;
                List<HuaweiRouteWithCommands> huaweiRoutesWithCommands = results.GetOrCreateItem(RSSC);

                // Routes To Add Commands

                if (routeDifferences.RoutesToAdd != null && routeDifferences.RoutesToAdd.Count > 0)
                {
                    foreach (var routeCompareResult in routeDifferences.RoutesToAdd)
                    {
                        huaweiRoutesWithCommands.Add(new HuaweiRouteWithCommands()
                        {
                            RouteCompareResult = routeCompareResult,
                            ActionType = RouteActionType.Add,
                            Commands = GetAddedRouteCommands(routeCompareResult)
                        });
                    }
                }

                // Routes To Update Commands

                if (routeDifferences.RoutesToUpdate != null && routeDifferences.RoutesToUpdate.Count > 0)
                {
                    foreach (var routeCompareResult in routeDifferences.RoutesToUpdate)
                    {
                        var commands = GetUpdatedRouteCommands(routeCompareResult);
                        huaweiRoutesWithCommands.Add(new HuaweiRouteWithCommands()
                        {
                            RouteCompareResult = routeCompareResult,
                            ActionType = RouteActionType.Update,
                            Commands = commands
                        });
                    }
                }

                // Routes To Delete Commands

                if (routeDifferences.RoutesToDelete != null && routeDifferences.RoutesToDelete.Count > 0)
                {
                    foreach (var routeCompareResult in routeDifferences.RoutesToDelete)
                    {
                        var commands = GetDeletedRouteCommands(routeCompareResult);
                        huaweiRoutesWithCommands.Add(new HuaweiRouteWithCommands()
                        {
                            RouteCompareResult = routeCompareResult,
                            ActionType = RouteActionType.Delete,
                            Commands = commands
                        });
                    }
                }
            }

            return results;
        }

        private List<string> GetAddedRouteCommands(HuaweiConvertedRouteCompareResult routeCompareResult)
        {
            HuaweiConvertedRoute addedRoute = routeCompareResult.Route;

            string addCNACLDCommand = HuaweiSoftX3000Commands.Full_ADD_CNACLD;
            addCNACLDCommand = addCNACLDCommand.Replace(HuaweiSoftX3000Commands.DNSet_Pattern, addedRoute.DNSet.ToString());
            addCNACLDCommand = addCNACLDCommand.Replace(HuaweiSoftX3000Commands.RSSC_Pattern, addedRoute.RSSC.ToString());
            addCNACLDCommand = addCNACLDCommand.Replace(HuaweiSoftX3000Commands.Code_Pattern, addedRoute.Code);
            addCNACLDCommand = addCNACLDCommand.Replace(HuaweiSoftX3000Commands.MINL_Pattern, addedRoute.MinL.ToString());
            addCNACLDCommand = addCNACLDCommand.Replace(HuaweiSoftX3000Commands.MAXL_Pattern, addedRoute.MaxL.ToString());
            addCNACLDCommand = addCNACLDCommand.Replace(HuaweiSoftX3000Commands.CHSC_Pattern, addedRoute.CC.ToString());
            addCNACLDCommand = addCNACLDCommand.Replace(HuaweiSoftX3000Commands.RouteCaseId_Pattern, addedRoute.RouteCaseId.ToString());
            addCNACLDCommand = addCNACLDCommand.Replace(HuaweiSoftX3000Commands.ZoneName_Pattern, GetZoneName(addedRoute.RSSC, addedRoute.Code));

            return new List<string>() { addCNACLDCommand };
        }

        private string GetZoneName(int RSSC, string code)
        {
            int customerId = _customerIdsByRSSC.GetRecord(RSSC);
            var saleZone = _saleZoneManager.GetCustomerSaleZoneByCode(customerId, code);
            saleZone.ThrowIfNull($"Invalid Sale Zone for: CustomerID '{customerId}', RSSC '{RSSC}', Code '{code}'");

            return saleZone.Name;
        }

        private List<string> GetUpdatedRouteCommands(HuaweiConvertedRouteCompareResult routeCompareResult)
        {
            HuaweiConvertedRoute newRoute = routeCompareResult.Route;
            HuaweiConvertedRoute existingRoute = routeCompareResult.ExistingRoute;

            string command = "Need a response from Support team about Update Command";

            return new List<string>() { command };
        }

        private List<string> GetDeletedRouteCommands(HuaweiConvertedRouteCompareResult routeCompareResult)
        {
            HuaweiConvertedRoute deletedRoute = routeCompareResult.Route;
            string command = "Need a response from Support team about Delete Command";
            return new List<string>() { command };
        }

        #endregion

        #region Execute and Log Route Options Commands

        private void ExecuteRouteOptionsCommands(List<RouteOptionsWithCommands> routesOptionsWithCommands, HuaweiSSHCommunication huaweiSSHCommunication, SSHCommunicator sshCommunicator,
           List<CommandResult> commandResults, int maxNumberOfTries, string switchId, long processInstanceId,
           out List<RouteOptionsWithCommands> succeedRouteOptionsWithCommands, out List<RouteOptionsWithCommands> failedRouteOptionsWithCommands)
        {
            succeedRouteOptionsWithCommands = null;
            failedRouteOptionsWithCommands = null;

            if (routesOptionsWithCommands == null || routesOptionsWithCommands.Count == 0)
                return;

            if (huaweiSSHCommunication == null)
            {
                succeedRouteOptionsWithCommands = routesOptionsWithCommands;
                _routeOptionsManager.UpdateSyncedRoutesOptions(succeedRouteOptionsWithCommands.Select(item => item.RouteOptions.RouteId));
                return;
            }

            SwitchCommandLogManager switchCommandLogManager = new SwitchCommandLogManager();

            if (!TryOpenConnectionWithSwitch(huaweiSSHCommunication, sshCommunicator, commandResults, switchCommandLogManager, switchId, processInstanceId))
            {
                failedRouteOptionsWithCommands = routesOptionsWithCommands;
                return;
            }

            try
            {
                int batchSize = 100;
                var routeOptionsIdsToUpdate = new List<long>();
                succeedRouteOptionsWithCommands = new List<RouteOptionsWithCommands>();
                failedRouteOptionsWithCommands = new List<RouteOptionsWithCommands>();

                foreach (var routeOptionsWithCommands in routesOptionsWithCommands)
                {
                    bool isCommandExecuted = false;
                    int numberOfTriesDone = 0;

                    while (!isCommandExecuted && numberOfTriesDone < maxNumberOfTries)
                    {
                        try
                        {
                            string command = routeOptionsWithCommands.Commands[0];

                            string response;
                            sshCommunicator.ExecuteCommand(command, new List<string>() { "END" }, false, false, out response);
                            if (IsExecutedCommandSucceeded(response, HuaweiSoftX3000Commands.RouteOptionsAddedSuccessfully) || IsExecutedCommandSucceeded(response, HuaweiSoftX3000Commands.RouteOptionsExists))
                            {
                                succeedRouteOptionsWithCommands.Add(routeOptionsWithCommands);

                                routeOptionsIdsToUpdate.Add(routeOptionsWithCommands.RouteOptions.RouteId);
                                if (routeOptionsIdsToUpdate.Count == batchSize)
                                {
                                    _routeOptionsManager.UpdateSyncedRoutesOptions(routeOptionsIdsToUpdate);
                                    routeOptionsIdsToUpdate = new List<long>();
                                }
                            }
                            else
                            {
                                failedRouteOptionsWithCommands.Add(routeOptionsWithCommands);
                            }

                            isCommandExecuted = true;
                            commandResults.Add(new CommandResult() { Command = command, Output = new List<string>() { response } });

                            SwitchCommandLog switchCommandLog = new SwitchCommandLog() { ProcessInstanceId = processInstanceId, SwitchId = switchId, Command = command, Response = response, };
                            switchCommandLogManager.Insert(switchCommandLog, out long intsertedId);
                        }
                        catch (Exception ex)
                        {
                            numberOfTriesDone++;
                            isCommandExecuted = false;
                        }
                    }

                    if (!isCommandExecuted)
                        failedRouteOptionsWithCommands.Add(routeOptionsWithCommands);
                }

                if (routeOptionsIdsToUpdate.Count > 0)
                    _routeOptionsManager.UpdateSyncedRoutesOptions(routeOptionsIdsToUpdate);
            }
            finally
            {
                CloseConnectionWithSwitch(huaweiSSHCommunication, sshCommunicator, commandResults, switchCommandLogManager, switchId, processInstanceId);
            }
        }

        private static void LogRouteOptionsCommands(List<RouteOptionsWithCommands> succeedRouteOptionsWithCommands, List<RouteOptionsWithCommands> failedRouteOptionsWithCommands,
            SwitchLogger ftpLogger, DateTime dateTime)
        {
            if (succeedRouteOptionsWithCommands != null && succeedRouteOptionsWithCommands.Count > 0)
            {
                var commandResults = succeedRouteOptionsWithCommands.Select(item => new CommandResult() { Command = string.Join(Environment.NewLine, item.Commands) }).ToList();
                ILogRouteOptionsContext logRouteOptionsContext = new LogRouteOptionsContext() { ExecutionDateTime = dateTime, ExecutionStatus = ExecutionStatus.Succeeded, CommandResults = commandResults };
                ftpLogger.LogRouteOptions(logRouteOptionsContext);
            }

            if (failedRouteOptionsWithCommands != null && failedRouteOptionsWithCommands.Count > 0)
            {
                var commandResults = failedRouteOptionsWithCommands.Select(item => new CommandResult() { Command = string.Join(Environment.NewLine, item.Commands) }).ToList();
                ILogRouteOptionsContext logRouteOptionsContext = new LogRouteOptionsContext() { ExecutionDateTime = dateTime, ExecutionStatus = ExecutionStatus.Failed, CommandResults = commandResults };
                ftpLogger.LogRouteOptions(logRouteOptionsContext);
            }
        }

        #endregion

        #region Execute and Log Route Cases Commands
     
        private void ExecuteRouteCasesCommands(List<RouteCaseWithCommands> routeCasesWithCommands, HuaweiSSHCommunication huaweiSSHCommunication, SSHCommunicator sshCommunicator,
           List<CommandResult> commandResults, IEnumerable<long> failedRouteOptionsIds , int maxNumberOfTries, string switchId, long processInstanceId,
           out List<RouteCaseWithCommands> succeedRouteCasesWithCommands, out List<RouteCaseWithCommands> failedRouteCasesWithCommands)
        {
            succeedRouteCasesWithCommands = null;
            failedRouteCasesWithCommands = null;

            if (routeCasesWithCommands == null || routeCasesWithCommands.Count == 0)
                return;

            if (huaweiSSHCommunication == null)
            {
                succeedRouteCasesWithCommands = routeCasesWithCommands;
                _routeCaseManager.UpdateSyncedRouteCases(succeedRouteCasesWithCommands.Select(item => item.RouteCase.RouteCaseId));
                return;
            }

            SwitchCommandLogManager switchCommandLogManager = new SwitchCommandLogManager();

            if (!TryOpenConnectionWithSwitch(huaweiSSHCommunication, sshCommunicator, commandResults, switchCommandLogManager, switchId, processInstanceId))
            {
                failedRouteCasesWithCommands = routeCasesWithCommands;
                return;
            }

            try
            {
                int batchSize = 100;
                var routeCaseIdsToUpdate = new List<long>();
                succeedRouteCasesWithCommands = new List<RouteCaseWithCommands>();
                failedRouteCasesWithCommands = new List<RouteCaseWithCommands>();

                foreach (var routeCaseWithCommands in routeCasesWithCommands)
                {
                    if (routeCaseWithCommands.RouteCase != null && failedRouteOptionsIds != null && failedRouteOptionsIds.Contains(routeCaseWithCommands.RouteCase.RouteId))
                    {
                        failedRouteCasesWithCommands.Add(routeCaseWithCommands);
                        continue;
                    }

                    bool isCommandExecuted = false;
                    int numberOfTriesDone = 0;

                    while (!isCommandExecuted && numberOfTriesDone < maxNumberOfTries)
                    {
                        try
                        {
                            string command = routeCaseWithCommands.Commands[0];

                            string response;
                            sshCommunicator.ExecuteCommand(command, new List<string>() { "END" }, false, false, out response);
                            if (IsExecutedCommandSucceeded(response, HuaweiSoftX3000Commands.RouteCaseAddedSuccessfully) || IsExecutedCommandSucceeded(response, HuaweiSoftX3000Commands.RouteCaseExists))
                            {
                                succeedRouteCasesWithCommands.Add(routeCaseWithCommands);

                                routeCaseIdsToUpdate.Add(routeCaseWithCommands.RouteCase.RouteCaseId);
                                if (routeCaseIdsToUpdate.Count == batchSize)
                                {
                                    _routeCaseManager.UpdateSyncedRouteCases(routeCaseIdsToUpdate);
                                    routeCaseIdsToUpdate = new List<long>();
                                }
                            }
                            else
                            {
                                failedRouteCasesWithCommands.Add(routeCaseWithCommands);
                            }

                            isCommandExecuted = true;
                            commandResults.Add(new CommandResult() { Command = command, Output = new List<string>() { response } });

                            SwitchCommandLog switchCommandLog = new SwitchCommandLog() { ProcessInstanceId = processInstanceId, SwitchId = switchId, Command = command, Response = response, };
                            switchCommandLogManager.Insert(switchCommandLog, out long intsertedId);
                        }
                        catch (Exception ex)
                        {
                            numberOfTriesDone++;
                            isCommandExecuted = false;
                        }
                    }

                    if (!isCommandExecuted)
                        failedRouteCasesWithCommands.Add(routeCaseWithCommands);
                }

                if (routeCaseIdsToUpdate.Count > 0)
                    _routeCaseManager.UpdateSyncedRouteCases(routeCaseIdsToUpdate);
            }
            finally
            {
                CloseConnectionWithSwitch(huaweiSSHCommunication, sshCommunicator, commandResults, switchCommandLogManager, switchId, processInstanceId);
            }
        }

        private static void LogRouteCaseCommands(List<RouteCaseWithCommands> succeedRouteCasesWithCommands, List<RouteCaseWithCommands> failedRouteCasesWithCommands,
         SwitchLogger ftpLogger, DateTime dateTime)
        {
            if (succeedRouteCasesWithCommands != null && succeedRouteCasesWithCommands.Count > 0)
            {
                var commandResults = succeedRouteCasesWithCommands.Select(item => new CommandResult() { Command = string.Join(Environment.NewLine, item.Commands) }).ToList();
                ILogRouteCasesContext logRouteCasesContext = new LogRouteCasesContext() { ExecutionDateTime = dateTime, ExecutionStatus = ExecutionStatus.Succeeded, CommandResults = commandResults };
                ftpLogger.LogRouteCases(logRouteCasesContext);
            }

            if (failedRouteCasesWithCommands != null && failedRouteCasesWithCommands.Count > 0)
            {
                var commandResults = failedRouteCasesWithCommands.Select(item => new CommandResult() { Command = string.Join(Environment.NewLine, item.Commands) }).ToList();
                ILogRouteCasesContext logRouteCasesContext = new LogRouteCasesContext() { ExecutionDateTime = dateTime, ExecutionStatus = ExecutionStatus.Failed, CommandResults = commandResults };
                ftpLogger.LogRouteCases(logRouteCasesContext);
            }
        }

        #endregion

        #region Execute and Log Route Commands
   
        private void ExecuteRoutesCommands(Dictionary<int, List<HuaweiRouteWithCommands>> routesWithCommandsByRSSC, HuaweiSSHCommunication huaweiSSHCommunication,
            SSHCommunicator sshCommunicator, List<CommandResult> commandResults, IEnumerable<long> failedRouteCasesIds, int maxNumberOfTries, string switchId, long processInstanceId,
            out Dictionary<int, List<HuaweiRouteWithCommands>> succeededRoutesWithCommandsByRSSC, out Dictionary<int, List<HuaweiRouteWithCommands>> failedRoutesWithCommandsByRSSC)
        {
            succeededRoutesWithCommandsByRSSC = null;
            failedRoutesWithCommandsByRSSC = null;

            if (routesWithCommandsByRSSC == null || routesWithCommandsByRSSC.Count == 0)
                return;

            if (huaweiSSHCommunication == null)
            {
                succeededRoutesWithCommandsByRSSC = routesWithCommandsByRSSC;
                return;
            }

            SwitchCommandLogManager switchCommandLogManager = new SwitchCommandLogManager();

            if (!TryOpenConnectionWithSwitch(huaweiSSHCommunication, sshCommunicator, commandResults, switchCommandLogManager, switchId, processInstanceId))
            {
                failedRoutesWithCommandsByRSSC = routesWithCommandsByRSSC;
                return;
            }

            try
            {
                succeededRoutesWithCommandsByRSSC = new Dictionary<int, List<HuaweiRouteWithCommands>>();
                failedRoutesWithCommandsByRSSC = new Dictionary<int, List<HuaweiRouteWithCommands>>();

                foreach (var routeWithCommandsKvp in routesWithCommandsByRSSC)
                {
                    var rssc = routeWithCommandsKvp.Key;
                    var orderedRoutesWithCommands = routeWithCommandsKvp.Value.OrderBy(itm => itm.RouteCompareResult.Route.Code);

                    foreach (var routeWithCommands in orderedRoutesWithCommands)
                    {
                        if (routeWithCommands.RouteCompareResult.Route != null && failedRouteCasesIds != null && failedRouteCasesIds.Contains(routeWithCommands.RouteCompareResult.Route.RouteCaseId))
                        {
                            var failedRoutesWithCommands = failedRoutesWithCommandsByRSSC.GetOrCreateItem(rssc);
                            failedRoutesWithCommands.Add(routeWithCommands);
                            continue;
                        }

                        int numberOfTriesDone = 0;
                        bool isCommandExecuted = false;

                        while (!isCommandExecuted && numberOfTriesDone < maxNumberOfTries)
                        {
                            try
                            {
                                string command = routeWithCommands.Commands[0];

                                string response;

                                sshCommunicator.ExecuteCommand(command, new List<string>() { "END" }, false, false, out response);
                                if (IsExecutedCommandSucceeded(response, HuaweiSoftX3000Commands.RouteOperationSucceeded))
                                {
                                    List<HuaweiRouteWithCommands> tempHuaweiRouteWithCommands = succeededRoutesWithCommandsByRSSC.GetOrCreateItem(rssc);
                                    tempHuaweiRouteWithCommands.Add(routeWithCommands);
                                }
                                else
                                {
                                    List<HuaweiRouteWithCommands> tempHuaweiRouteWithCommands = failedRoutesWithCommandsByRSSC.GetOrCreateItem(rssc);
                                    tempHuaweiRouteWithCommands.Add(routeWithCommands);
                                }

                                isCommandExecuted = true;
                                commandResults.Add(new CommandResult() { Command = command, Output = new List<string>() { response } });

                                SwitchCommandLog switchCommandLog = new SwitchCommandLog() { ProcessInstanceId = processInstanceId, SwitchId = switchId, Command = command, Response = response, };
                                switchCommandLogManager.Insert(switchCommandLog, out long intsertedId);
                            }
                            catch (Exception ex)
                            {
                                numberOfTriesDone++;
                                isCommandExecuted = false;
                            }
                        }

                        if (!isCommandExecuted)
                        {
                            List<HuaweiRouteWithCommands> tempHuaweiRouteWithCommands = failedRoutesWithCommandsByRSSC.GetOrCreateItem(rssc);
                            tempHuaweiRouteWithCommands.Add(routeWithCommands);
                        }
                    }
                }
            }
            finally
            {
                CloseConnectionWithSwitch(huaweiSSHCommunication, sshCommunicator, commandResults, switchCommandLogManager, switchId, processInstanceId);
            }
        }

        private static void LogHuaweiRouteCommands(Dictionary<int, List<HuaweiRouteWithCommands>> succeedRoutesWithCommandsByRSSC, Dictionary<int, List<HuaweiRouteWithCommands>> failedRoutesWithCommandsByRSSC,
        SwitchLogger ftpLogger, DateTime dateTime)
        {
            if (succeedRoutesWithCommandsByRSSC != null && succeedRoutesWithCommandsByRSSC.Count > 0)
            {
                foreach (var succeedRoutesWithCommandsByKvp in succeedRoutesWithCommandsByRSSC)
                {
                    int rssc = succeedRoutesWithCommandsByKvp.Key;
                    var customerRoutesWithCommands = succeedRoutesWithCommandsByKvp.Value;

                    var commandResults = customerRoutesWithCommands.Select(item => new CommandResult() { Command = string.Join(Environment.NewLine, item.Commands) }).ToList();
                    ILogRoutesContext logRoutesContext = new LogRoutesContext()
                    {
                        ExecutionDateTime = dateTime,
                        ExecutionStatus = ExecutionStatus.Succeeded,
                        CommandResults = commandResults,
                        CustomerIdentifier = rssc.ToString()
                    };
                    ftpLogger.LogRoutes(logRoutesContext);
                }
            }

            if (failedRoutesWithCommandsByRSSC != null && failedRoutesWithCommandsByRSSC.Count > 0)
            {
                foreach (var failedRoutesWithCommandsKvp in failedRoutesWithCommandsByRSSC)
                {
                    int rssc = failedRoutesWithCommandsKvp.Key;
                    var customerRoutesWithCommands = failedRoutesWithCommandsKvp.Value;

                    var commandResults = customerRoutesWithCommands.Select(item => new CommandResult() { Command = string.Join(Environment.NewLine, item.Commands) }).ToList();
                    ILogRoutesContext logRoutesContext = new LogRoutesContext()
                    {
                        ExecutionDateTime = dateTime,
                        ExecutionStatus = ExecutionStatus.Failed,
                        CommandResults = commandResults,
                        CustomerIdentifier = rssc.ToString()
                    };
                    ftpLogger.LogRoutes(logRoutesContext);
                }
            }
        }

        #endregion

        #region Common Commands Execution/Log Methods

        private bool TryOpenConnectionWithSwitch(HuaweiSSHCommunication sshCommunication, SSHCommunicator sshCommunicator, List<CommandResult> commandResults,
           SwitchCommandLogManager switchCommandLogManager, string switchId, long processInstanceId)
        {
            sshCommunication.ThrowIfNull("sshCommunication");
            sshCommunication.SSLSettings.ThrowIfNull("sshCommunication.SSLSettings");

            try
            {
                sshCommunicator.OpenConnection();
                sshCommunicator.OpenShell();

                string openSSLCommand = $"openssl s_client -port {sshCommunication.SSLSettings.Port} -host {sshCommunication.SSLSettings.Host} -ssl3 -quiet -crlf";
                bool isOpenSSLSucceeded = this.ExecuteCommand(openSSLCommand, sshCommunicator, commandResults, new List<string>() { "RETURN:0", "ERRNO" }, false, true,
                    HuaweiSoftX3000Commands.OpenSSLSucceeded, switchCommandLogManager, switchId, processInstanceId);
                if (!isOpenSSLSucceeded)
                {
                    sshCommunicator.Dispose();
                    return false;
                }

                string loginCommand = $"LGI:OP=\"{sshCommunication.SSLSettings.Username}\",PWD=\"{sshCommunication.SSLSettings.Password}\";";
                string loginCommandToLog = $"LGI:OP=\"#USERNAME#\",PWD=\"#PASSWORD#\";";
                bool isLoginSucceeded = this.ExecuteCommand(loginCommand, loginCommandToLog, sshCommunicator, commandResults, new List<string>() { "END" }, false, false,
                    HuaweiSoftX3000Commands.LoginSucceeded, switchCommandLogManager, switchId, processInstanceId, (response) =>
                    {
                        if (string.IsNullOrEmpty(response))
                            return null;

                        string modifiedResponse = Regex.Replace(response, sshCommunication.SSLSettings.Username, "#USERNAME#", RegexOptions.IgnoreCase);
                        modifiedResponse = Regex.Replace(modifiedResponse, sshCommunication.SSLSettings.Password, "#PASSWORD#", RegexOptions.IgnoreCase);

                        return modifiedResponse;
                    });

                if (!isLoginSucceeded)
                {
                    sshCommunicator.Dispose();
                    return false;
                }

                string registerToMSCServerCommand = $"REG NE:IP=\"{sshCommunication.SSLSettings.InterfaceIP}\";";
                bool isRegistrationSucceeded = this.ExecuteCommand(registerToMSCServerCommand, sshCommunicator, commandResults, new List<string>() { "END" }, false, false,
                    HuaweiSoftX3000Commands.RegistrationSucceeded, switchCommandLogManager, switchId, processInstanceId);
                if (!isRegistrationSucceeded)
                {
                    string logoutCommand = $"LGO:OP=\"{sshCommunication.SSLSettings.Username}\";";
                    string logoutCommandToLog = $"LGO:OP=\"#USERNAME#\";";
                    this.ExecuteCommand(logoutCommand, logoutCommandToLog, sshCommunicator, commandResults, new List<string>() { "END" }, false, false, HuaweiSoftX3000Commands.LogoutSucceeded,
                        switchCommandLogManager, switchId, processInstanceId, (response) =>
                        {
                            if (string.IsNullOrEmpty(response))
                                return null;

                            string modifiedResponse = Regex.Replace(response, sshCommunication.SSLSettings.Username, "#USERNAME#", RegexOptions.IgnoreCase);
                            return modifiedResponse;
                        });

                    sshCommunicator.Dispose();
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                sshCommunicator.Dispose();
                throw;
            }
        }

        private bool ExecuteCommand(string command, SSHCommunicator sshCommunicator, List<CommandResult> commandResults, List<string> endOfResponseList, bool isEndOfResponseCaseSensitive,
            bool ignoreEmptySpacesInResponse, string responseSuccessKey, SwitchCommandLogManager switchCommandLogManager, string switchId, long processInstanceId)
        {
            return this.ExecuteCommand(command, command, sshCommunicator, commandResults, endOfResponseList, isEndOfResponseCaseSensitive, ignoreEmptySpacesInResponse,
                            responseSuccessKey, switchCommandLogManager, switchId, processInstanceId);
        }

        private bool ExecuteCommand(string command, string commandToLog, SSHCommunicator sshCommunicator, List<CommandResult> commandResults, List<string> endOfResponseList,
            bool isEndOfResponseCaseSensitive, bool ignoreEmptySpacesInResponse, string responseSuccessKey, SwitchCommandLogManager switchCommandLogManager, string switchId, long processInstanceId,
            Func<string, string> getFinalResponse = null)
        {
            string response;
            sshCommunicator.ExecuteCommand(command, endOfResponseList, isEndOfResponseCaseSensitive, ignoreEmptySpacesInResponse, out response);

            if (getFinalResponse != null)
                response = getFinalResponse(response);

            commandResults.Add(new CommandResult() { Command = commandToLog, Output = new List<string>() { response } });

            SwitchCommandLog switchCommandLog = new SwitchCommandLog() { ProcessInstanceId = processInstanceId, SwitchId = switchId, Command = commandToLog, Response = response, };
            switchCommandLogManager.Insert(switchCommandLog, out long intsertedId);

            return IsExecutedCommandSucceeded(response, responseSuccessKey);
        }

        private bool IsExecutedCommandSucceeded(string response, string responseSuccessKey)
        {
            if (string.IsNullOrEmpty(response))
                return false;

            string responseToUpper = response.Replace(" ", "").ToUpper();
            if (!responseToUpper.Contains(responseSuccessKey))
                return false;

            return true;
        }

        private void CloseConnectionWithSwitch(HuaweiSSHCommunication sshCommunication, SSHCommunicator sshCommunicator, List<CommandResult> commandResults,
           SwitchCommandLogManager switchCommandLogManager, string switchId, long processInstanceId)
        {
            try
            {
                string unregisterCommand = $"UNREG NE:IP=\"{sshCommunication.SSLSettings.InterfaceIP}\";";
                bool isUnregistrationSucceeded = this.ExecuteCommand(unregisterCommand, sshCommunicator, commandResults, new List<string>() { "END" }, false, false,
                    HuaweiSoftX3000Commands.UnregistrationSucceeded, switchCommandLogManager, switchId, processInstanceId);
                if (isUnregistrationSucceeded)
                {
                    string logoutCommand = $"LGO:OP=\"{sshCommunication.SSLSettings.Username}\";";
                    string logoutCommandToLog = $"LGO:OP=\"#USERNAME#\";";
                    this.ExecuteCommand(logoutCommand, logoutCommandToLog, sshCommunicator, commandResults, new List<string>() { "END" }, false, false, HuaweiSoftX3000Commands.LogoutSucceeded,
                        switchCommandLogManager, switchId, processInstanceId, (response) =>
                        {
                            if (string.IsNullOrEmpty(response))
                                return null;

                            string modifiedResponse = Regex.Replace(response, sshCommunication.SSLSettings.Username, "#USERNAME#", RegexOptions.IgnoreCase);
                            return modifiedResponse;
                        });
                }
            }
            finally
            {
                sshCommunicator.Dispose();
            }
        }

        private static void LogAllCommands(List<CommandResult> allCommands, SwitchLogger ftpLogger, DateTime dateTime)
        {
            if (allCommands != null && allCommands.Count > 0)
            {
                ILogCommandsContext logRouteCasesContext = new LogCommandsContext() { ExecutionDateTime = dateTime, CommandResults = allCommands };
                ftpLogger.LogCommands(logRouteCasesContext);
            }
        }

        #endregion

        #endregion
    }
}