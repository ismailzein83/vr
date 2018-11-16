using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.RouteSync.Entities;
using TOne.WhS.RouteSync.Huawei.Business;
using TOne.WhS.RouteSync.Huawei.Entities;
using Vanrise.Common;
using Vanrise.Entities;

namespace TOne.WhS.RouteSync.Huawei
{
    public class HuaweiSWSync : SwitchRouteSynchronizer
    {
        const string CommandPrompt = "<";

        public override Guid ConfigId { get { return new Guid("376687E2-268D-4DFA-AA39-3205C3CD18E5"); } }

        public int NumberOfOptions { get; set; }

        public int MinRNLength { get; set; }

        public Dictionary<string, CarrierMapping> CarrierMappings { get; set; }

        public List<HuaweiSSHCommunication> SwitchCommunicationList { get; set; }

        public List<SwitchLogger> SwitchLoggerList { get; set; }

        private Dictionary<int, CustomerMapping> _customerMappingsByRSSN;
        private Dictionary<int, CustomerMapping> CustomerMappingsByRSSN
        {
            get
            {
                if (_customerMappingsByRSSN != null)
                    return _customerMappingsByRSSN;

                if (CarrierMappings == null || CarrierMappings.Count == 0)
                    return null;

                _customerMappingsByRSSN = new Dictionary<int, CustomerMapping>();

                foreach (var carrierMappingKvp in CarrierMappings)
                {
                    var carrierMapping = carrierMappingKvp.Value;
                    if (carrierMapping.CustomerMapping != null)
                        _customerMappingsByRSSN.Add(carrierMapping.CustomerMapping.RSSN, carrierMapping.CustomerMapping);
                }

                if (_customerMappingsByRSSN.Count == 0)
                    _customerMappingsByRSSN = null;

                return _customerMappingsByRSSN;
            }
        }

        #region Public Methods

        public override void Initialize(ISwitchRouteSynchronizerInitializeContext context)
        {
            WhSRouteSyncHuaweiManager whSRouteSyncHuaweiManager = new WhSRouteSyncHuaweiManager(context.SwitchId);
            whSRouteSyncHuaweiManager.Initialize();

            RouteCaseManager routeCaseManager = new RouteCaseManager(context.SwitchId);
            routeCaseManager.Initialize();

            RouteManager routeManager = new RouteManager(context.SwitchId);
            routeManager.Initialize();
        }

        public override void ConvertRoutes(ISwitchRouteSynchronizerConvertRoutesContext context)
        {
            if (context.Routes == null || context.Routes.Count == 0 || CarrierMappings == null || CarrierMappings.Count == 0)
                return;

            RouteCaseManager routeCaseManager = new RouteCaseManager(context.SwitchId);

            List<HuaweiConvertedRoute> convertedRoutes = new List<HuaweiConvertedRoute>();
            List<RouteCaseToAdd> routeCasesToAdd = new List<RouteCaseToAdd>();

            Dictionary<string, RouteCase> routeCasesByRSName = routeCaseManager.GetCachedRouteCasesByRSName();

            foreach (var route in context.Routes)
            {
                var carrierMapping = CarrierMappings.GetRecord(route.CustomerId);
                if (carrierMapping == null)
                    continue;

                var customerMapping = carrierMapping.CustomerMapping;
                if (customerMapping == null)
                    continue;

                RouteAnalysis routeAnalysis = this.GetRouteAnalysis(customerMapping.RSSN, route.Options);
                string rsName = Helper.GetRSName(routeAnalysis, this.MinRNLength);

                RouteCase routeCase;
                if (rsName.CompareTo(HuaweiCommands.ROUTE_BLOCK) != 0 && (routeCasesByRSName == null || !routeCasesByRSName.TryGetValue(rsName, out routeCase)))
                    routeCasesToAdd.Add(new RouteCaseToAdd() { RSName = rsName, RouteCaseAsString = Helper.SerializeRouteCase(routeAnalysis) });

                HuaweiConvertedRoute huaweiConvertedRoute = new HuaweiConvertedRoute()
                {
                    RSSN = customerMapping.RSSN,
                    Code = route.Code,
                    RSName = rsName,
                    DNSet = customerMapping.DNSet
                };
                convertedRoutes.Add(huaweiConvertedRoute);
            }

            if (routeCasesToAdd.Count > 0)
                routeCasesByRSName = routeCaseManager.InsertAndGetRouteCases(routeCasesToAdd);

            if (convertedRoutes.Count == 0)
                return;

            HuaweiConvertedRoutesPayload huaweiConvertedRoutesPayload;
            if (context.ConvertedRoutesPayload != null)
                huaweiConvertedRoutesPayload = context.ConvertedRoutesPayload.CastWithValidate<HuaweiConvertedRoutesPayload>("context.ConvertedRoutesPayload");
            else
                huaweiConvertedRoutesPayload = new HuaweiConvertedRoutesPayload();

            huaweiConvertedRoutesPayload.ConvertedRoutes.AddRange(convertedRoutes);
            context.ConvertedRoutesPayload = huaweiConvertedRoutesPayload;
        }

        public override void onAllRoutesConverted(ISwitchRouteSynchronizerOnAllRoutesConvertedContext context)
        {
            if (context.ConvertedRoutesPayload == null)
                return;

            HuaweiConvertedRoutesPayload payload = context.ConvertedRoutesPayload.CastWithValidate<HuaweiConvertedRoutesPayload>("context.ConvertedRoutesPayload");
            if (payload.ConvertedRoutes == null || payload.ConvertedRoutes.Count == 0)
                return;

            context.ConvertedRoutes = RouteSync.Business.Helper.CompressRoutesWithCodes(payload.ConvertedRoutes, CreateHuaweiConvertedRoute);
        }

        public override object PrepareDataForApply(ISwitchRouteSynchronizerPrepareDataForApplyContext context)
        {
            RouteManager routeManager = new RouteManager(context.SwitchId);
            var dbApplyStream = routeManager.InitialiazeStreamForDBApply();

            foreach (var convertedRoute in context.ConvertedRoutes)
                routeManager.WriteRecordToStream(convertedRoute as HuaweiConvertedRoute, dbApplyStream);

            return routeManager.FinishDBApplyStream(dbApplyStream);
        }

        public override void ApplySwitchRouteSyncRoutes(ISwitchRouteSynchronizerApplyRoutesContext context)
        {
            RouteManager routeManager = new RouteManager(context.SwitchId);
            routeManager.ApplyRouteForDB(context.PreparedItemsForApply);
        }

        public override void Finalize(ISwitchRouteSynchronizerFinalizeContext context)
        {
            //Conditions
            if (CarrierMappings == null || CarrierMappings.Count == 0)
                return;

            if (SwitchLoggerList == null || SwitchLoggerList.Count == 0)
                throw new Exception(string.Format("No logger specify for the switch with id {0}", context.SwitchId));

            var ftpLogger = SwitchLoggerList.First(item => item.IsActive);
            ftpLogger.ThrowIfNull(string.Format("No logger specify for the switch with id {0}", context.SwitchId));

            //Managers
            RouteManager routeManager = new RouteManager(context.SwitchId);
            RouteCaseManager routeCaseManager = new RouteCaseManager(context.SwitchId);
            RouteSucceededManager routeSucceededManager = new RouteSucceededManager(context.SwitchId);

            //Communication
            HuaweiSSHCommunication huaweiSSHCommunication = null;
            if (SwitchCommunicationList != null)
                huaweiSSHCommunication = SwitchCommunicationList.FirstOrDefault(itm => itm.IsActive);

            SSHCommunicator sshCommunicator = null;
            if (huaweiSSHCommunication != null)
                sshCommunicator = new SSHCommunicator(huaweiSSHCommunication.SSHCommunicatorSettings);

            DateTime finalizeTime = DateTime.Now;

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
            var routeCasesToBeAdded = routeCaseManager.GetNotSyncedRouteCases();
            var routeCasesToBeAddedWithCommands = GetRouteCasesWithCommands(routeCasesToBeAdded);
            var routesWithCommandsByRSSN = GetRoutesWithCommands(context.SwitchId, routeCaseManager, routeManager);

            //Execute and Log Route Cases
            List<RouteCaseWithCommands> succeedRouteCasesWithCommands;
            List<RouteCaseWithCommands> failedRouteCasesWithCommands;

            ExecuteRouteCasesCommands(routeCasesToBeAddedWithCommands, huaweiSSHCommunication, sshCommunicator, commandResults, routeCaseManager, maxNumberOfTries,
                out succeedRouteCasesWithCommands, out failedRouteCasesWithCommands);

            LogRouteCaseCommands(succeedRouteCasesWithCommands, failedRouteCasesWithCommands, ftpLogger, finalizeTime);

            //Execute and Log Routes
            IEnumerable<string> failedRSNames = failedRouteCasesWithCommands == null ? null : failedRouteCasesWithCommands.Select(item => item.RouteCase.RSName);

            Dictionary<int, List<HuaweiRouteWithCommands>> succeedRoutesWithCommandsByRSSN;
            Dictionary<int, List<HuaweiRouteWithCommands>> failedRoutesWithCommandsByRSSN;

            ExecuteRoutesCommands(routesWithCommandsByRSSN, huaweiSSHCommunication, sshCommunicator, commandResults, failedRSNames, maxNumberOfTries,
                out succeedRoutesWithCommandsByRSSN, out failedRoutesWithCommandsByRSSN);

            LogHuaweiRouteCommands(succeedRoutesWithCommandsByRSSN, failedRoutesWithCommandsByRSSN, ftpLogger, finalizeTime);

            if (succeedRoutesWithCommandsByRSSN != null && succeedRoutesWithCommandsByRSSN.Count > 0) //save succeeded routes to succeeded table
                routeSucceededManager.SaveRoutesSucceededToDB(succeedRoutesWithCommandsByRSSN);

            if (failedRoutesWithCommandsByRSSN != null && failedRoutesWithCommandsByRSSN.Count > 0)
            {
                var failedAdded = new List<HuaweiConvertedRoute>();
                var failedUpdated = new List<HuaweiConvertedRoute>();
                var failedDeleted = new List<HuaweiConvertedRoute>();

                foreach (var failedRoutesWithCommands in failedRoutesWithCommandsByRSSN)
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
                    routeManager.RemoveRoutesFromTempTable(failedAdded);

                if (failedUpdated != null && failedUpdated.Count > 0)
                    routeManager.UpdateRoutesInTempTable(failedUpdated);

                if (failedDeleted != null && failedDeleted.Count > 0)
                    routeManager.InsertRoutesToTempTable(failedDeleted);
            }

            routeManager.Finalize(new RouteFinalizeContext());

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

            HashSet<int> allRSSNs = new HashSet<int>();
            HashSet<int> duplicatedRSSNs = new HashSet<int>();

            HashSet<string> allCSCNames = new HashSet<string>();
            HashSet<string> duplicatedCSCNames = new HashSet<string>();

            HashSet<string> allRouteNames = new HashSet<string>();
            HashSet<string> duplicatedRouteNames = new HashSet<string>();
            HashSet<string> shortRouteNames = new HashSet<string>();

            foreach (var carrierMapping in this.CarrierMappings)
            {
                CustomerMapping customerMapping = carrierMapping.Value.CustomerMapping;
                if (customerMapping != null && !string.IsNullOrEmpty(customerMapping.CSCName))
                {
                    var isCSCNameDuplicated = allCSCNames.Any(cscName => string.Compare(cscName, customerMapping.CSCName, true) == 0);
                    if (!isCSCNameDuplicated)
                        allCSCNames.Add(customerMapping.CSCName);
                    else
                        duplicatedCSCNames.Add(customerMapping.CSCName);


                    if (!allRSSNs.Contains(customerMapping.RSSN))
                        allRSSNs.Add(customerMapping.RSSN);
                    else
                        duplicatedRSSNs.Add(customerMapping.RSSN);
                }

                SupplierMapping supplierMapping = carrierMapping.Value.SupplierMapping;
                if (supplierMapping != null && !string.IsNullOrEmpty(supplierMapping.RouteName))
                {
                    string supplierRouteNameWithoutEmptySpaces = supplierMapping.RouteName.Replace(" ", "");

                    if (supplierRouteNameWithoutEmptySpaces.Length >= this.MinRNLength)
                    {
                        string modifiedSupplierRouteName = supplierRouteNameWithoutEmptySpaces.Substring(0, this.MinRNLength);
                        var isRouteNameDuplicated = allRouteNames.Any(routeName => !string.IsNullOrEmpty(routeName) && string.Compare(routeName.Replace(" ", "").Substring(0, this.MinRNLength), modifiedSupplierRouteName, true) == 0);
                        if (!isRouteNameDuplicated)
                            allRouteNames.Add(supplierMapping.RouteName);
                        else
                            duplicatedRouteNames.Add(supplierMapping.RouteName);
                    }
                    else
                    {
                        shortRouteNames.Add(supplierMapping.RouteName);
                    }
                }
            }

            List<string> validationMessages = new List<string>();
            if (duplicatedRSSNs.Count > 0)
                validationMessages.Add(string.Format("Duplicated RSSNs: {0}", string.Join(", ", duplicatedRSSNs)));

            if (duplicatedCSCNames.Count > 0)
                validationMessages.Add(string.Format("Duplicated CSC Names: {0}", string.Join(", ", duplicatedCSCNames)));

            if (duplicatedRouteNames.Count > 0)
                validationMessages.Add(string.Format("Duplicated Route Names: {0}", string.Join(", ", duplicatedRouteNames)));

            if (shortRouteNames.Count > 0)
                validationMessages.Add(string.Format("Invalid Route Names: {0}. Length should be greater than {1}", string.Join(", ", shortRouteNames), this.MinRNLength));

            context.ValidationMessages = validationMessages.Count > 0 ? validationMessages : null;
            return validationMessages.Count == 0;
        }

        #endregion

        #region Private Methods

        private HuaweiConvertedRoute CreateHuaweiConvertedRoute(ICreateConvertedRouteWithCodeContext context)
        {
            if (CustomerMappingsByRSSN == null)
                throw new NullReferenceException("CustomerMappingsByRSSN");

            int rssn = int.Parse(context.Customer);
            var customerMapping = CustomerMappingsByRSSN.GetRecord(rssn); ;
            return new HuaweiConvertedRoute() { RSSN = rssn, Code = context.Code, RSName = context.RouteOptionIdentifier, DNSet = customerMapping.DNSet };
        }

        private RouteAnalysis GetRouteAnalysis(int rssn, List<RouteOption> routeOptions)
        {
            if (routeOptions == null || routeOptions.Count == 0)
                return null;

            bool isSequence = true;
            List<RouteCaseOption> routeCaseOptions = new List<RouteCaseOption>();

            foreach (var routeOption in routeOptions)
            {
                if (routeOption.IsBlocked)
                    continue;

                var carrierMapping = CarrierMappings.GetRecord(routeOption.SupplierId);
                if (carrierMapping == null)
                    continue;

                var supplierMapping = carrierMapping.SupplierMapping;
                if (supplierMapping == null || string.IsNullOrEmpty(supplierMapping.RouteName))
                    continue;

                if (!isSequence && (!routeOption.Percentage.HasValue || routeOption.Percentage.Value == 0))
                    continue;

                RouteCaseOption routeCaseOption = new RouteCaseOption();
                routeCaseOption.RouteName = supplierMapping.RouteName;
                routeCaseOption.ISUP = supplierMapping.ISUP;

                if (routeOption.Percentage.HasValue)
                {
                    isSequence = false;
                    routeCaseOption.Percentage = routeOption.Percentage.Value;
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

            return new RouteAnalysis() { RSSN = rssn, RouteCaseOptionsType = routeCaseOptionsType, RouteCaseOptions = routeCaseOptions };
        }

        #region RouteCase Commands

        private List<RouteCaseWithCommands> GetRouteCasesWithCommands(IEnumerable<RouteCase> routeCases)
        {
            if (routeCases == null || !routeCases.Any())
                return null;

            List<RouteCaseWithCommands> routeCasesWithCommands = new List<RouteCaseWithCommands>();

            foreach (var routeCaseToAdd in routeCases)
            {
                RouteCaseWithCommands routeCaseWithCommands = new RouteCaseWithCommands()
                {
                    RouteCase = routeCaseToAdd,
                    Commands = GetRouteCaseCommands(routeCaseToAdd)
                };
                routeCasesWithCommands.Add(routeCaseWithCommands);
            }

            return routeCasesWithCommands;
        }

        private List<string> GetRouteCaseCommands(RouteCase routeCase)
        {
            if (routeCase == null)
                return null;

            RouteAnalysis routeAnalysis = Helper.DeserializeRouteCase(routeCase.RouteCaseAsString);
            if (routeAnalysis == null)
                return null;

            string rtsm;
            switch (routeAnalysis.RouteCaseOptionsType)
            {
                case RouteCaseOptionsType.Sequence: rtsm = HuaweiCommands.RTSM_SEQ; break;
                case RouteCaseOptionsType.Percentage: rtsm = HuaweiCommands.RTSM_PERC; break;
                default: throw new NotSupportedException(string.Format("rtana.RouteCaseOptionsType {0} not supported", routeAnalysis.RouteCaseOptionsType));
            }

            List<string> routeNames = new List<string>();
            List<string> percentages = new List<string>();

            RouteCaseOption firstRouteCaseOption = routeAnalysis.RouteCaseOptions.First();
            routeNames.Add(string.Format("RN=\"{0}\"", firstRouteCaseOption.RouteName));

            if (firstRouteCaseOption.Percentage.HasValue)
                percentages.Add(string.Format("PRT1={0}", firstRouteCaseOption.Percentage.Value));

            for (var index = 1; index < routeAnalysis.RouteCaseOptions.Count; index++)
            {
                RouteCaseOption currentRouteCaseOption = routeAnalysis.RouteCaseOptions[index];

                routeNames.Add(string.Format("R{0}N=\"{1}\"", index + 1, currentRouteCaseOption.RouteName));

                if (currentRouteCaseOption.Percentage.HasValue)
                    percentages.Add(string.Format("PRT{0}={1}", index + 1, currentRouteCaseOption.Percentage.Value));
            }

            string routeNamesAsString = string.Join(", ", routeNames);
            string percentagesAsString = string.Empty;
            if (percentages.Count > 0)
                percentagesAsString = string.Concat(", ", string.Join(", ", percentages));

            string command = string.Format("ADD RTANA: RSN=\"{0}\", RSSN=\"{1}\", TSN=\"DEFAULT”, RTSM={2}, {3}{4}, ISUP={5};", routeCase.RSName, routeAnalysis.RSSN, rtsm,
                routeNamesAsString, percentagesAsString, firstRouteCaseOption.ISUP);

            return new List<string>() { command };
        }

        #endregion

        #region Route Commands

        private Dictionary<int, List<HuaweiRouteWithCommands>> GetRoutesWithCommands(string switchId, RouteCaseManager routeCaseManager, RouteManager routeManager)
        {
            RouteCompareTablesContext routeCompareTablesContext = new RouteCompareTablesContext();
            routeManager.CompareTables(routeCompareTablesContext);

            Dictionary<int, HuaweiConvertedRouteDifferences> routeDifferencesByRSSN = routeCompareTablesContext.RouteDifferencesByRSSN;
            if (routeDifferencesByRSSN == null || routeDifferencesByRSSN.Count == 0)
                return null;

            Dictionary<int, List<HuaweiRouteWithCommands>> results = new Dictionary<int, List<HuaweiRouteWithCommands>>();

            foreach (var routeDifferencesKvp in routeDifferencesByRSSN)
            {
                var routeDifferences = routeDifferencesKvp.Value;
                List<HuaweiRouteWithCommands> customerHuaweiRoutesWithCommands = results.GetOrCreateItem(routeDifferencesKvp.Key);

                if (routeDifferences.RoutesToAdd != null && routeDifferences.RoutesToAdd.Count > 0)
                {
                    foreach (var routeCompareResult in routeDifferences.RoutesToAdd)
                    {
                        var commands = GetAddedRouteCommands(routeCompareResult);
                        customerHuaweiRoutesWithCommands.Add(new HuaweiRouteWithCommands() { RouteCompareResult = routeCompareResult, ActionType = RouteActionType.Add, Commands = commands });
                    }
                }

                if (routeDifferences.RoutesToUpdate != null && routeDifferences.RoutesToUpdate.Count > 0)
                {
                    foreach (var routeCompareResult in routeDifferences.RoutesToUpdate)
                    {
                        var commands = GetUpdatedRouteCommands(routeCompareResult);
                        customerHuaweiRoutesWithCommands.Add(new HuaweiRouteWithCommands() { RouteCompareResult = routeCompareResult, ActionType = RouteActionType.Update, Commands = commands });
                    }
                }

                if (routeDifferences.RoutesToDelete != null && routeDifferences.RoutesToDelete.Count > 0)
                {
                    foreach (var routeCompareResult in routeDifferences.RoutesToDelete)
                    {
                        var commands = GetDeletedRouteCommands(routeCompareResult);
                        customerHuaweiRoutesWithCommands.Add(new HuaweiRouteWithCommands() { RouteCompareResult = routeCompareResult, ActionType = RouteActionType.Delete, Commands = commands });
                    }
                }
            }

            return results;
        }

        private List<string> GetAddedRouteCommands(HuaweiConvertedRouteCompareResult routeCompareResult)
        {
            HuaweiConvertedRoute addedRoute = routeCompareResult.Route;

            string command;

            if (addedRoute.RSName.CompareTo(HuaweiCommands.ROUTE_BLOCK) != 0)
            {
                command = string.Format("ADD CNACLD: P={0}, PFX=K'00{1}, CSA={2}, RSNAME=\"{3}\", MINL=10, MAXL=32, ICLDTYPE=PS, ISERVICECHECKNAME=\"INVALID\", NUMNAME=\"INVALID\", " +
                    "TARIFF=CI, CHGNAME=\"INVALID\", NCN=\"INVALID\", SDCSN=\"INVALID\";", addedRoute.DNSet, addedRoute.Code, HuaweiCommands.CSA_ITT, addedRoute.RSName);
            }
            else
            {
                command = string.Format("ADD CNACLD: P={0}, PFX=K'00{1}, CSA={2}, MINL=10, MAXL=32, ICLDTYPE=PS, SDESCRIPTION=\"T-One_Blocked\", ISERVICECHECKNAME=\"INVALID\", DNPREPARE=FALSE, " +
                    "NUMNAME=\"INVALID\", TARIFF=CI, CHGNAME=\"INVALID\", NCN=\"INVALID\", SDCSN=\"INVALID\";", addedRoute.DNSet, addedRoute.Code, HuaweiCommands.CSA_TON);
            }

            return new List<string>() { command };
        }

        private List<string> GetUpdatedRouteCommands(HuaweiConvertedRouteCompareResult routeCompareResult)
        {
            HuaweiConvertedRoute newRoute = routeCompareResult.Route;
            HuaweiConvertedRoute existingRoute = routeCompareResult.ExistingRoute;

            string command = null;

            if (newRoute.RSName.CompareTo(HuaweiCommands.ROUTE_BLOCK) != 0 && existingRoute.RSName.CompareTo(HuaweiCommands.ROUTE_BLOCK) != 0)
            {
                command = string.Format("MOD CNACLD: P={0}, PFX=K'00{1}, ADDR=ALL, RSNAME=\"{2}\";", newRoute.DNSet, newRoute.Code, newRoute.RSName);
            }
            else if (newRoute.RSName.CompareTo(HuaweiCommands.ROUTE_BLOCK) == 0)
            {
                command = string.Format("MOD CNACLD: P={0}, PFX=K'00{1}, ADDR=ALL, CSA={2};", newRoute.DNSet, newRoute.Code, HuaweiCommands.CSA_TON);
            }
            else if (existingRoute.RSName.CompareTo(HuaweiCommands.ROUTE_BLOCK) == 0)
            {
                command = string.Format("MOD CNACLD: P={0}, PFX=K'00{1}, ADDR=ALL, CSA=ITT, RSNAME=\"{2}\";", newRoute.DNSet, newRoute.Code, newRoute.RSName);
            }

            if (command == null)
                return null;

            return new List<string>() { command };
        }

        private List<string> GetDeletedRouteCommands(HuaweiConvertedRouteCompareResult routeCompareResult)
        {
            HuaweiConvertedRoute deletedRoute = routeCompareResult.Route;
            string command = string.Format("RMV CNACLD: P={0}, PFX=K'00{1}, ADDR=ALL;", deletedRoute.DNSet, deletedRoute.Code);
            return new List<string>() { command };
        }

        #endregion

        #region LOG Files

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

        private static void LogHuaweiRouteCommands(Dictionary<int, List<HuaweiRouteWithCommands>> succeedRoutesWithCommandsByRSSN, Dictionary<int, List<HuaweiRouteWithCommands>> failedRoutesWithCommandsByRSSN,
            SwitchLogger ftpLogger, DateTime dateTime)
        {
            if (succeedRoutesWithCommandsByRSSN != null && succeedRoutesWithCommandsByRSSN.Count > 0)
            {
                foreach (var succeedRoutesWithCommandsByKvp in succeedRoutesWithCommandsByRSSN)
                {
                    int rssn = succeedRoutesWithCommandsByKvp.Key;
                    var customerRoutesWithCommands = succeedRoutesWithCommandsByKvp.Value;

                    var commandResults = customerRoutesWithCommands.Select(item => new CommandResult() { Command = string.Join(Environment.NewLine, item.Commands) }).ToList();
                    ILogRoutesContext logRoutesContext = new LogRoutesContext()
                    {
                        ExecutionDateTime = dateTime,
                        ExecutionStatus = ExecutionStatus.Succeeded,
                        CommandResults = commandResults,
                        CustomerIdentifier = rssn.ToString()
                    };
                    ftpLogger.LogRoutes(logRoutesContext);
                }
            }

            if (failedRoutesWithCommandsByRSSN != null && failedRoutesWithCommandsByRSSN.Count > 0)
            {
                foreach (var failedRoutesWithCommandsKvp in failedRoutesWithCommandsByRSSN)
                {
                    int rssn = failedRoutesWithCommandsKvp.Key;
                    var customerRoutesWithCommands = failedRoutesWithCommandsKvp.Value;

                    var commandResults = customerRoutesWithCommands.Select(item => new CommandResult() { Command = string.Join(Environment.NewLine, item.Commands) }).ToList();
                    ILogRoutesContext logRoutesContext = new LogRoutesContext()
                    {
                        ExecutionDateTime = dateTime,
                        ExecutionStatus = ExecutionStatus.Failed,
                        CommandResults = commandResults,
                        CustomerIdentifier = rssn.ToString()
                    };
                    ftpLogger.LogRoutes(logRoutesContext);
                }
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

        #region SSH

        private void ExecuteRouteCasesCommands(List<RouteCaseWithCommands> routeCasesWithCommands, HuaweiSSHCommunication huaweiSSHCommunication, SSHCommunicator sshCommunicator,
            List<CommandResult> commandResults, RouteCaseManager routeCaseManager, int maxNumberOfTries, out List<RouteCaseWithCommands> succeedRouteCasesWithCommands,
            out List<RouteCaseWithCommands> failedRouteCasesWithCommands)
        {
            succeedRouteCasesWithCommands = null;
            failedRouteCasesWithCommands = null;

            if (routeCasesWithCommands == null || routeCasesWithCommands.Count == 0)
                return;

            if (huaweiSSHCommunication == null)
            {
                succeedRouteCasesWithCommands = routeCasesWithCommands;
                routeCaseManager.UpdateSyncedRouteCases(succeedRouteCasesWithCommands.Select(item => item.RouteCase.RCNumber));
                return;
            }

            if (!TryOpenConnectionWithSwitch(huaweiSSHCommunication, sshCommunicator, commandResults))
            {
                failedRouteCasesWithCommands = routeCasesWithCommands;
                return;
            }

            int batchSize = 100;
            var routeCaseNumbersToUpdate = new List<int>();
            succeedRouteCasesWithCommands = new List<RouteCaseWithCommands>();
            failedRouteCasesWithCommands = new List<RouteCaseWithCommands>();

            foreach (var routeCaseWithCommands in routeCasesWithCommands)
            {
                bool isCommandExecuted = false;
                int numberOfTriesDone = 0;

                while (!isCommandExecuted && numberOfTriesDone < maxNumberOfTries)
                {
                    try
                    {
                        string command = routeCaseWithCommands.Commands[0];

                        string response;
                        sshCommunicator.ExecuteCommand(command, CommandPrompt, out response);
                        commandResults.Add(new CommandResult() { Command = command, Output = new List<string>() { response } });
                        if (IsCommandSucceed(response))
                        {
                            succeedRouteCasesWithCommands.Add(routeCaseWithCommands);

                            routeCaseNumbersToUpdate.Add(routeCaseWithCommands.RouteCase.RCNumber);
                            if (routeCaseNumbersToUpdate.Count == batchSize)
                            {
                                routeCaseManager.UpdateSyncedRouteCases(routeCaseNumbersToUpdate);
                                routeCaseNumbersToUpdate = new List<int>();
                            }
                        }
                        else
                        {
                            failedRouteCasesWithCommands.Add(routeCaseWithCommands);
                        }

                        isCommandExecuted = true;
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

            if (routeCaseNumbersToUpdate.Count > 0)
                routeCaseManager.UpdateSyncedRouteCases(routeCaseNumbersToUpdate);
        }

        private void ExecuteRoutesCommands(Dictionary<int, List<HuaweiRouteWithCommands>> routesWithCommandsByRSSN, HuaweiSSHCommunication huaweiSSHCommunication,
            SSHCommunicator sshCommunicator, List<CommandResult> commandResults, IEnumerable<string> failedRSNames, int maxNumberOfTries,
            out Dictionary<int, List<HuaweiRouteWithCommands>> succeededRoutesWithCommandsByRSSN, out Dictionary<int, List<HuaweiRouteWithCommands>> failedRoutesWithCommandsByRSSN)
        {
            succeededRoutesWithCommandsByRSSN = null;
            failedRoutesWithCommandsByRSSN = null;

            if (routesWithCommandsByRSSN == null || routesWithCommandsByRSSN.Count == 0)
                return;

            if (huaweiSSHCommunication == null)
            {
                succeededRoutesWithCommandsByRSSN = routesWithCommandsByRSSN;
                return;
            }

            if (!TryOpenConnectionWithSwitch(huaweiSSHCommunication, sshCommunicator, commandResults))
            {
                failedRoutesWithCommandsByRSSN = routesWithCommandsByRSSN;
                return;
            }

            succeededRoutesWithCommandsByRSSN = new Dictionary<int, List<HuaweiRouteWithCommands>>();
            failedRoutesWithCommandsByRSSN = new Dictionary<int, List<HuaweiRouteWithCommands>>();

            foreach (var routeWithCommandsKvp in routesWithCommandsByRSSN)
            {
                var rssn = routeWithCommandsKvp.Key;
                var orderedRoutesWithCommands = routeWithCommandsKvp.Value.OrderBy(itm => itm.RouteCompareResult.Route.Code);

                foreach (var routeWithCommands in orderedRoutesWithCommands)
                {
                    if (routeWithCommands.RouteCompareResult.Route != null && failedRSNames != null && failedRSNames.Contains(routeWithCommands.RouteCompareResult.Route.RSName))
                    {
                        var failedRoutesWithCommands = failedRoutesWithCommandsByRSSN.GetOrCreateItem(rssn);
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

                            sshCommunicator.ExecuteCommand(command, CommandPrompt, out response);
                            commandResults.Add(new CommandResult() { Command = command, Output = new List<string>() { response } });
                            if (IsCommandSucceed(response))
                            {
                                List<HuaweiRouteWithCommands> tempHuaweiRouteWithCommands = succeededRoutesWithCommandsByRSSN.GetOrCreateItem(rssn);
                                tempHuaweiRouteWithCommands.Add(routeWithCommands);
                            }
                            else
                            {
                                List<HuaweiRouteWithCommands> tempHuaweiRouteWithCommands = failedRoutesWithCommandsByRSSN.GetOrCreateItem(rssn);
                                tempHuaweiRouteWithCommands.Add(routeWithCommands);
                            }

                            isCommandExecuted = true;
                        }
                        catch (Exception ex)
                        {
                            numberOfTriesDone++;
                            isCommandExecuted = false;
                        }
                    }

                    if (!isCommandExecuted)
                    {
                        List<HuaweiRouteWithCommands> tempHuaweiRouteWithCommands = failedRoutesWithCommandsByRSSN.GetOrCreateItem(rssn);
                        tempHuaweiRouteWithCommands.Add(routeWithCommands);
                    }
                }
            }
        }

        private bool TryOpenConnectionWithSwitch(HuaweiSSHCommunication sshCommunication, SSHCommunicator sshCommunicator, List<CommandResult> commandResults)
        {
            sshCommunicator.OpenConnection();
            sshCommunicator.OpenShell();
            sshCommunicator.ReadPrompt(">");

            string registerToMSCServerCommand = string.Format("REG NE:IP=\"{0}\";", sshCommunication.InterfaceIP);

            string response;
            sshCommunicator.ExecuteCommand(registerToMSCServerCommand, CommandPrompt, out response);
            commandResults.Add(new CommandResult() { Command = registerToMSCServerCommand, Output = new List<string>() { response } });

            if (string.IsNullOrEmpty(response))
                return false;

            string responseToUpper = response.Replace(" ", "").ToUpper();
            if (!responseToUpper.Contains(HuaweiCommands.RegistrationSucceeded))
                return false;

            return true;
        }

        private bool IsCommandSucceed(string response)
        {
            if (string.IsNullOrEmpty(response))
                return false;

            string responseToUpper = response.Replace(" ", "").ToUpper();
            if (!responseToUpper.Contains(HuaweiCommands.OperationSucceeded))
                return false;

            return true;
        }

        #endregion

        #endregion
    }
}