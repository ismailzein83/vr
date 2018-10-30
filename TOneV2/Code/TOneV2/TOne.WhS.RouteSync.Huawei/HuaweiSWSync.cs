using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TOne.WhS.RouteSync.Entities;
using TOne.WhS.RouteSync.Huawei.Business;
using TOne.WhS.RouteSync.Huawei.Data;
using TOne.WhS.RouteSync.Huawei.Entities;
using Vanrise.Common;
using Vanrise.Entities;

namespace TOne.WhS.RouteSync.Huawei
{
    public partial class HuaweiSWSync : SwitchRouteSynchronizer
    {
        const int _supplierRNLength = 3;
        const string CommandPrompt = "<";

        public override Guid ConfigId { get { return new Guid("376687E2-268D-4DFA-AA39-3205C3CD18E5"); } }

        public int NumberOfOptions { get; set; }

        public Dictionary<string, CarrierMapping> CarrierMappings { get; set; }

        public List<HuaweiSSHCommunication> SwitchCommunicationList { get; set; }

        public List<SwitchLogger> SwitchLoggerList { get; set; }

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
            List<RouteCase> routeCasesToAdd = new List<RouteCase>();

            Dictionary<string, RouteCase> routeCasesByRSName = routeCaseManager.GetCachedRouteCasesByRSName();

            foreach (var route in context.Routes)
            {
                var customerCarrierMapping = CarrierMappings.GetRecord(route.CustomerId);
                if (customerCarrierMapping == null)
                    continue;

                var customerMapping = customerCarrierMapping.CustomerMapping;
                if (customerMapping == null)
                    continue;

                RTANA rtana = this.GetRTANA(customerMapping.RSSN, route.Options);
                string rsName = Helper.GetRSName(rtana, _supplierRNLength);

                RouteCase routeCase;
                if (!routeCasesByRSName.TryGetValue(rsName, out routeCase))
                    routeCasesToAdd.Add(new RouteCase() { RSName = rsName, RouteCaseAsString = Helper.SerializeRouteCase(rtana) });

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

            //Get Commands
            var routeCasesToBeAdded = routeCaseManager.GetNotSyncedRouteCases();
            var routeCasesToBeAddedWithCommands = GetRouteCasesWithCommands(routeCasesToBeAdded);

            Dictionary<int, HuaweiConvertedRouteDifferences> routeDifferencesByRSSN;
            var huaweiRoutesWithCommandsByRSSN = GetHuaweiRoutesWithCommands(context.SwitchId, routeCaseManager, out routeDifferencesByRSSN);

            //Communication
            HuaweiSSHCommunication huaweiSSHCommunication = null;
            if (SwitchCommunicationList != null)
                huaweiSSHCommunication = SwitchCommunicationList.FirstOrDefault(itm => itm.IsActive);

            SSHCommunicator sshCommunicator = null;
            if (huaweiSSHCommunication != null)
                sshCommunicator = new SSHCommunicator(huaweiSSHCommunication.SSHCommunicatorSettings);

            DateTime finalizeTime = DateTime.Now;

            List<CommandResult> commandResults = new List<CommandResult>();

            List<string> faultCodes = null;
            int maxNumberOfRetries = 0;
            //if (sshCommunicator != null)
            //{
            //    var configManager = new TOne.WhS.RouteSync.Business.ConfigManager();
            //    HuaweiSwitchRouteSynchronizerSettings switchSettings = configManager.GetRouteSynchronizerSwitchSettings(ConfigId) as HuaweiSwitchRouteSynchronizerSettings;
            //    if (switchSettings == null || switchSettings.FaultCodes == null)
            //        throw new NullReferenceException("Huawei switch settings is not defined. Please go to Route Sync under Component Settings and add related settings.");
            //    faultCodes = switchSettings.FaultCodes;
            //    maxNumberOfRetries = switchSettings.NumberOfRetries;
            //}

            #region Execute and Log Route Cases
            List<RouteCaseWithCommands> succeedRouteCasesWithCommands;
            List<RouteCaseWithCommands> failedRouteCasesWithCommands;

            ExecuteRouteCasesCommands(routeCasesToBeAddedWithCommands, huaweiSSHCommunication, sshCommunicator, commandResults, out succeedRouteCasesWithCommands,
                out failedRouteCasesWithCommands, routeCaseManager, maxNumberOfRetries, context.SwitchId);

            LogRouteCaseCommands(succeedRouteCasesWithCommands, failedRouteCasesWithCommands, ftpLogger, finalizeTime);
            #endregion

            #region Execute and Log Routes
            IEnumerable<string> failedRSNames = failedRouteCasesWithCommands == null ? null : failedRouteCasesWithCommands.Select(item => item.RouteCase.RSName);

            Dictionary<int, List<HuaweiRouteWithCommands>> succeedHuaweiRoutesWithCommandsByRSSN;
            Dictionary<int, List<HuaweiRouteWithCommands>> failedHuaweiRoutesWithCommandsByRSSN;
            Dictionary<int, List<HuaweiRouteWithCommands>> allFailedHuaweiRoutesWithCommandsByRSSN;

            ExecuteRoutesCommands(huaweiRoutesWithCommandsByRSSN, huaweiSSHCommunication, sshCommunicator, commandResults,
                out succeedHuaweiRoutesWithCommandsByRSSN, out failedHuaweiRoutesWithCommandsByRSSN, out allFailedHuaweiRoutesWithCommandsByRSSN,
                failedRSNames, faultCodes, maxNumberOfRetries);

            LogHuaweiRouteCommands(succeedHuaweiRoutesWithCommandsByRSSN, failedHuaweiRoutesWithCommandsByRSSN, ftpLogger, finalizeTime);

            if (succeedHuaweiRoutesWithCommandsByRSSN != null && succeedHuaweiRoutesWithCommandsByRSSN.Count > 0)// save succeeded routes to succeeded table
                routeSucceededManager.SaveRoutesSucceededToDB(succeedHuaweiRoutesWithCommandsByRSSN);

            if (allFailedHuaweiRoutesWithCommandsByRSSN != null && allFailedHuaweiRoutesWithCommandsByRSSN.Count > 0)
            {
                var failedAdded = new List<HuaweiConvertedRoute>();
                var failedUpdated = new List<HuaweiConvertedRoute>();
                var failedDeleted = new List<HuaweiConvertedRoute>();

                foreach (var failedHuaweiRoutesWithCommands in allFailedHuaweiRoutesWithCommandsByRSSN)
                {
                    foreach (var route in failedHuaweiRoutesWithCommands.Value)
                    {
                        switch (route.ActionType)
                        {
                            case RouteActionType.Add: failedAdded.Add(route.RouteCompareResult.NewRoute); break;
                            case RouteActionType.Update: failedUpdated.Add(route.RouteCompareResult.ExistingRoute); break;
                            case RouteActionType.Delete: failedDeleted.Add(route.RouteCompareResult.NewRoute); break;
                        }
                    }
                }

                if (failedDeleted != null && failedDeleted.Count > 0)
                    routeManager.InsertRoutesToTempTable(failedDeleted);

                if (failedAdded != null && failedAdded.Count > 0)
                    routeManager.RemoveRoutesFromTempTable(failedAdded);

                if (failedUpdated != null && failedUpdated.Count > 0)
                    routeManager.UpdateRoutesInTempTable(failedUpdated);
            }

            routeManager.Finalize(new RouteFinalizeContext());
            #endregion
        }

        public override object PrepareDataForApply(ISwitchRouteSynchronizerPrepareDataForApplyContext context)
        {
            throw new NotImplementedException();
        }

        public override void ApplySwitchRouteSyncRoutes(ISwitchRouteSynchronizerApplyRoutesContext context)
        {
            throw new NotImplementedException();
        }

        public override void RemoveConnection(ISwitchRouteSynchronizerRemoveConnectionContext context)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Private Methods

        private ConvertedRouteWithCode CreateHuaweiConvertedRoute(ICreateConvertedRouteWithCodeContext context)
        {
            var customerCarrierMapping = CarrierMappings.GetRecord(context.Customer);
            var customerMapping = customerCarrierMapping.CustomerMapping;
            return new HuaweiConvertedRoute() { RSSN = int.Parse(context.Customer), Code = context.Code, RSName = context.RouteOptionIdentifier, DNSet = customerMapping.DNSet };
        }

        private RTANA GetRTANA(int rssn, List<RouteOption> routeOptions)
        {
            bool isSequence = true;
            List<RouteCaseOption> routeCaseOptions = new List<RouteCaseOption>();

            foreach (var routeOption in routeOptions)
            {
                var carrierMapping = CarrierMappings.GetRecord(routeOption.SupplierId);
                if (carrierMapping == null)
                    continue;

                var supplierMapping = carrierMapping.SupplierMapping;
                if (supplierMapping == null)
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

            RTANA rtana = new RTANA();
            rtana.RSSN = rssn;
            rtana.IsSequence = isSequence;
            rtana.RouteCaseOptions = routeCaseOptions;
            return rtana;
        }

        #region Commands For RouteCase changes
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
            return null;
        }
        #endregion

        #region Commands For Route changes
        private Dictionary<int, List<HuaweiRouteWithCommands>> GetHuaweiRoutesWithCommands(string switchId, RouteCaseManager routeCaseManager, out Dictionary<int, HuaweiConvertedRouteDifferences> routeDifferencesByRSSN)
        {
            var routeCases = routeCaseManager.GetCachedRouteCases();
            if (routeCases == null || routeCases.Count == 0)
                throw new VRBusinessException(string.Format("No route cases found for switch with id {0}", switchId));

            Dictionary<int, CarrierMapping> carrierMappingByRSSN = new Dictionary<int, CarrierMapping>();

            foreach (var carrierMappingKvp in CarrierMappings)
            {
                var carrierMapping = carrierMappingKvp.Value;

                if (carrierMapping.CustomerMapping != null)
                    carrierMappingByRSSN.Add(carrierMapping.CustomerMapping.RSSN, carrierMapping);
            }

            RouteCompareTablesContext routeCompareTablesContext = new RouteCompareTablesContext();
            IRouteDataManager routeDataManager = RouteSyncHuaweiDataManagerFactory.GetDataManager<IRouteDataManager>();
            routeDataManager.SwitchId = switchId;
            routeDataManager.CompareTables(routeCompareTablesContext);
            routeDifferencesByRSSN = routeCompareTablesContext.RouteDifferencesByRSSN;

            Dictionary<int, List<HuaweiRouteWithCommands>> results = new Dictionary<int, List<HuaweiRouteWithCommands>>();

            if (routeDifferencesByRSSN != null && routeDifferencesByRSSN.Count > 0)
            {
                foreach (var routeDifferencesKvp in routeDifferencesByRSSN)
                {
                    var routeDifferences = routeDifferencesKvp.Value;
                    List<HuaweiRouteWithCommands> customerHuaweiRoutesWithCommands = results.GetOrCreateItem(routeDifferencesKvp.Key);

                    if (routeDifferences.RoutesToAdd != null && routeDifferences.RoutesToAdd.Count > 0)
                    {
                        foreach (var routeCompareResult in routeDifferences.RoutesToAdd)
                        {
                            var commands = GetRouteCommand(carrierMappingByRSSN, routeCases, routeCompareResult);
                            customerHuaweiRoutesWithCommands.Add(new HuaweiRouteWithCommands() { RouteCompareResult = routeCompareResult, Commands = commands, ActionType = RouteActionType.Add });
                        }
                    }

                    if (routeDifferences.RoutesToUpdate != null && routeDifferences.RoutesToUpdate.Count > 0)
                    {
                        foreach (var routeCompareResult in routeDifferences.RoutesToUpdate)
                        {
                            var commands = GetRouteCommand(carrierMappingByRSSN, routeCases, routeCompareResult);
                            customerHuaweiRoutesWithCommands.Add(new HuaweiRouteWithCommands() { RouteCompareResult = routeCompareResult, Commands = commands, ActionType = RouteActionType.Update });
                        }
                    }

                    if (routeDifferences.RoutesToDelete != null && routeDifferences.RoutesToDelete.Count > 0)
                    {
                        foreach (var routeCompareResult in routeDifferences.RoutesToDelete)
                        {
                            var commands = GetDeletedRouteCommands(carrierMappingByRSSN, routeCases, routeCompareResult);
                            customerHuaweiRoutesWithCommands.Add(new HuaweiRouteWithCommands() { RouteCompareResult = routeCompareResult, Commands = commands, ActionType = RouteActionType.Delete });
                        }
                    }
                }
            }
            return results;
        }
        private List<string> GetRouteCommand(Dictionary<int, CarrierMapping> carrierMappingByCustomerBo, List<RouteCase> routeCases, HuaweiConvertedRouteCompareResult routeCompareResult)
        {
            throw new NotImplementedException();
        }
        private List<string> GetDeletedRouteCommands(Dictionary<int, CarrierMapping> carrierMappingByCustomerBo, List<RouteCase> routeCases, HuaweiConvertedRouteCompareResult routeCompareResult)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region LOG File

        private static void LogHuaweiRouteCommands(Dictionary<int, List<HuaweiRouteWithCommands>> succeedHuaweiRoutesWithCommandsByRSSN, Dictionary<int, List<HuaweiRouteWithCommands>> failedHuaweiRoutesWithCommandsByRSSN, SwitchLogger ftpLogger, DateTime dateTime)
        {
            if (succeedHuaweiRoutesWithCommandsByRSSN != null && succeedHuaweiRoutesWithCommandsByRSSN.Count > 0)
            {
                foreach (var customerRoutesWithCommandsKvp in succeedHuaweiRoutesWithCommandsByRSSN)
                {
                    var customerRoutesWithCommands = customerRoutesWithCommandsKvp.Value;
                    var commandResults = customerRoutesWithCommands.Select(item => new CommandResult() { Command = string.Join(Environment.NewLine, item.Commands) }).ToList();
                    ILogRoutesContext logRoutesContext = new LogRoutesContext() { ExecutionDateTime = dateTime, ExecutionStatus = ExecutionStatus.Succeeded, CommandResults = commandResults, RSSNNumber = Convert.ToInt32(customerRoutesWithCommandsKvp.Key) };
                    ftpLogger.LogRoutes(logRoutesContext);
                }
            }

            if (failedHuaweiRoutesWithCommandsByRSSN != null && failedHuaweiRoutesWithCommandsByRSSN.Count > 0)
            {
                foreach (var customerRoutesWithCommandsKvp in succeedHuaweiRoutesWithCommandsByRSSN)
                {
                    var customerRoutesWithCommands = customerRoutesWithCommandsKvp.Value;
                    var commandResults = customerRoutesWithCommands.Select(item => new CommandResult() { Command = string.Join(Environment.NewLine, item.Commands) }).ToList();
                    ILogRoutesContext logRoutesContext = new LogRoutesContext() { ExecutionDateTime = dateTime, ExecutionStatus = ExecutionStatus.Failed, CommandResults = commandResults, RSSNNumber = Convert.ToInt32(customerRoutesWithCommandsKvp.Key) };
                    ftpLogger.LogRoutes(logRoutesContext);
                }
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

        #region SSH

        private void ExecuteRouteCasesCommands(List<RouteCaseWithCommands> routeCasesWithCommands, HuaweiSSHCommunication sshCommunication, SSHCommunicator sshCommunicator,
            List<CommandResult> commandResults, out List<RouteCaseWithCommands> succeedRouteCaseNumbers, out List<RouteCaseWithCommands> failedRouteCaseNumbers,
            RouteCaseManager routeCaseManager, int maxNumberOfRetries, string switchId)
        {
            int batchSize = 100;
            var routeCaseNumbersToUpdate = new List<int>();
            succeedRouteCaseNumbers = new List<RouteCaseWithCommands>();
            failedRouteCaseNumbers = new List<RouteCaseWithCommands>();

            if (routeCasesWithCommands == null || routeCasesWithCommands.Count == 0)
                return;

            if (sshCommunication == null)
            {
                succeedRouteCaseNumbers = routeCasesWithCommands;
                routeCaseManager.UpdateSyncedRouteCases(succeedRouteCaseNumbers.Select(item => item.RouteCase.RCNumber));
                return;
            }

            string response;

            response = OpenConnectionWithSwitch(sshCommunicator, commandResults);

            foreach (var routeCaseWithCommands in routeCasesWithCommands)
            {
                bool isSuccessfull = false;
                int numberOfTriesDone = 0;

                var rcNumber = routeCaseWithCommands.RouteCase.RCNumber;

                while (!isSuccessfull && numberOfTriesDone < maxNumberOfRetries)
                {
                    try
                    {
                        string command = routeCaseWithCommands.Commands[0];
                        sshCommunicator.ExecuteCommand(command, CommandPrompt, out response);
                        commandResults.Add(new CommandResult() { Command = command, Output = new List<string>() { response } });
                        if (!IsCommandSucceed(response))
                        {
                            string commandTemp = string.Format("{0}:RC={1};", HuaweiCommands.ANRAR_Command, rcNumber);
                            sshCommunicator.ExecuteCommand(commandTemp, CommandPrompt, out response);
                            commandResults.Add(new CommandResult() { Command = commandTemp, Output = new List<string>() { response } });

                            sshCommunicator.ExecuteCommand(";", CommandPrompt, out response);
                            commandResults.Add(new CommandResult() { Command = ";", Output = new List<string>() { response } });

                            if (IsCommandSucceed(response))
                            {
                                commandTemp = string.Format("{0}:RC={1};", HuaweiCommands.ANRZI_Command, rcNumber);
                                sshCommunicator.ExecuteCommand(commandTemp, CommandPrompt, out response);
                                commandResults.Add(new CommandResult() { Command = commandTemp, Output = new List<string>() { response } });
                            }

                            sshCommunicator.ExecuteCommand(command, CommandPrompt, out response);
                            commandResults.Add(new CommandResult() { Command = command, Output = new List<string>() { response } });
                            if (!IsCommandSucceed(response))
                            {
                                failedRouteCaseNumbers.Add(routeCaseWithCommands);
                                break;
                            }
                        }

                        var commandsSucceed = true;
                        for (int i = 1; i < routeCaseWithCommands.Commands.Count; i++)
                        {
                            string routeCaseCommand = routeCaseWithCommands.Commands[i];
                            sshCommunicator.ExecuteCommand(routeCaseCommand, CommandPrompt, out response);
                            commandResults.Add(new CommandResult() { Command = routeCaseCommand, Output = new List<string>() { response } });
                            if (!IsCommandSucceed(response))
                            {
                                commandsSucceed = false;
                                break;
                            }
                        }

                        if (!commandsSucceed)
                        {
                            failedRouteCaseNumbers.Add(routeCaseWithCommands);
                            break;
                        }

                        command = string.Format("{0}:RC={1};", HuaweiCommands.ANRAI_Command, rcNumber);
                        sshCommunicator.ExecuteCommand(command, CommandPrompt, out response);
                        commandResults.Add(new CommandResult() { Command = command, Output = new List<string>() { response } });
                        if (!IsCommandSucceed(response))
                        {
                            failedRouteCaseNumbers.Add(routeCaseWithCommands);
                            break;
                        }

                        sshCommunicator.ExecuteCommand(";", CommandPrompt, out response);
                        commandResults.Add(new CommandResult() { Command = ";", Output = new List<string>() { response } });
                        if (!IsCommandSucceed(response))
                        {
                            failedRouteCaseNumbers.Add(routeCaseWithCommands);
                            break;
                        }

                        succeedRouteCaseNumbers.Add(routeCaseWithCommands);
                        routeCaseNumbersToUpdate.Add(rcNumber);
                        if (routeCaseNumbersToUpdate.Count == batchSize)
                        {
                            routeCaseManager.UpdateSyncedRouteCases(routeCaseNumbersToUpdate);
                            routeCaseNumbersToUpdate = new List<int>();
                        }
                        isSuccessfull = true;
                    }
                    catch (Exception ex)
                    {
                        numberOfTriesDone++;
                        isSuccessfull = false;
                    }
                }
            }

            if (routeCaseNumbersToUpdate.Count > 0)
                routeCaseManager.UpdateSyncedRouteCases(routeCaseNumbersToUpdate);
        }

        private void ExecuteRoutesCommands(Dictionary<int, List<HuaweiRouteWithCommands>> routesWithCommandsByRSSN, HuaweiSSHCommunication sshCommunication,
            SSHCommunicator sshCommunicator, List<CommandResult> commandResults, out Dictionary<int, List<HuaweiRouteWithCommands>> succeededRoutesWithCommandsByRSSN,
            out Dictionary<int, List<HuaweiRouteWithCommands>> failedRoutesWithCommandsByRSSN, out Dictionary<int, List<HuaweiRouteWithCommands>> allFailedRoutesWithCommandsByRSSN,
            IEnumerable<string> failedRSNames, IEnumerable<string> faultCodes, int maxNumberOfRetries)
        {
            succeededRoutesWithCommandsByRSSN = new Dictionary<int, List<HuaweiRouteWithCommands>>();
            failedRoutesWithCommandsByRSSN = new Dictionary<int, List<HuaweiRouteWithCommands>>();
            allFailedRoutesWithCommandsByRSSN = new Dictionary<int, List<HuaweiRouteWithCommands>>();

            if (routesWithCommandsByRSSN == null || routesWithCommandsByRSSN.Count == 0)
                return;

            if (sshCommunication == null)
            {
                succeededRoutesWithCommandsByRSSN = routesWithCommandsByRSSN;
                return;
            }

            string response;

            OpenConnectionWithSwitch(sshCommunicator, commandResults);
            sshCommunicator.ExecuteCommand(HuaweiCommands.ANBAR_Command, CommandPrompt, out response);
            commandResults.Add(new CommandResult() { Command = HuaweiCommands.ANBAR_Command, Output = new List<string>() { response } });
            sshCommunicator.ExecuteCommand(";", CommandPrompt, out response);
            commandResults.Add(new CommandResult() { Command = ";", Output = new List<string>() { response } });

            if (IsCommandFailed(response, faultCodes))
            {
                sshCommunicator.ExecuteCommand(HuaweiCommands.ANBZI_Command, CommandPrompt, out response);
                commandResults.Add(new CommandResult() { Command = HuaweiCommands.ANBZI_Command, Output = new List<string>() { response } });
                Thread.Sleep(5000);

                if (response.ToUpper().Contains(HuaweiCommands.ORDERED))
                {
                    sshCommunicator.ExecuteCommand(HuaweiCommands.Exit_Command, ">", out response);
                    commandResults.Add(new CommandResult() { Command = HuaweiCommands.Exit_Command, Output = new List<string>() { response } });
                    Thread.Sleep(2000);

                    sshCommunicator.ExecuteCommand(HuaweiCommands.MML_Command, "<", out response);
                    commandResults.Add(new CommandResult() { Command = HuaweiCommands.MML_Command, Output = new List<string>() { response } });
                    Thread.Sleep(1000);
                }
                else if (!IsCommandSucceed(response))
                {
                    throw new Exception("ANBAR Not Executed");
                }

                sshCommunicator.ExecuteCommand(HuaweiCommands.ANBCI_Command, CommandPrompt, out response);
                commandResults.Add(new CommandResult() { Command = HuaweiCommands.ANBCI_Command, Output = new List<string>() { response } });

                Thread.Sleep(5000);
                while (!response.ToUpper().Contains(HuaweiCommands.ORDERED) && response.ToUpper().Contains("new List<string>() { response }  BUSY"))
                {
                    sshCommunicator.ExecuteCommand(HuaweiCommands.ANBCI_Command, CommandPrompt, out response);
                    commandResults.Add(new CommandResult() { Command = HuaweiCommands.ANBCI_Command, Output = new List<string>() { response } });
                    Thread.Sleep(3000);
                }
                if (response.ToUpper().Contains(HuaweiCommands.ORDERED))
                {
                    sshCommunicator.ExecuteCommand(HuaweiCommands.Exit_Command, ">", out response);
                    commandResults.Add(new CommandResult() { Command = HuaweiCommands.Exit_Command, Output = new List<string>() { response } });
                    Thread.Sleep(2000);
                    sshCommunicator.ExecuteCommand(HuaweiCommands.MML_Command, "<", out response);
                    commandResults.Add(new CommandResult() { Command = HuaweiCommands.MML_Command, Output = new List<string>() { response } });
                    Thread.Sleep(1000);
                }
                else if (!IsCommandSucceed(response))
                {
                    throw new Exception("ANBAR Not Executed");
                }
            }
            else if (!IsCommandSucceed(response))
            {
                throw new Exception("ANBAR Not Executed");
            }

            foreach (var huaweiRouteWithCommandsKvp in routesWithCommandsByRSSN)
            {
                var rssn = huaweiRouteWithCommandsKvp.Key;
                var huaweiRoutesWithCommands = huaweiRouteWithCommandsKvp.Value;

                foreach (var huaweiRouteWithCommands in huaweiRoutesWithCommands)
                {
                    if (huaweiRouteWithCommands.RouteCompareResult != null && failedRSNames.Contains(huaweiRouteWithCommands.RouteCompareResult.NewRoute.RSName))
                    {
                        var allFailedRoutesWithCommands = allFailedRoutesWithCommandsByRSSN.GetOrCreateItem(rssn);
                        allFailedRoutesWithCommands.Add(huaweiRouteWithCommands);
                        continue;
                    }

                    bool isSuccessfull = false;
                    int numberOfTriesDone = 0;

                    var commandsSucceed = true;
                    while (!isSuccessfull && numberOfTriesDone < maxNumberOfRetries)
                    {
                        try
                        {
                            foreach (var command in huaweiRouteWithCommands.Commands)
                            {
                                sshCommunicator.ExecuteCommand(command, CommandPrompt, out response);
                                commandResults.Add(new CommandResult() { Command = command, Output = new List<string>() { response } });
                                while (response.ToUpper().Contains(HuaweiCommands.FUNCTION_BUSY))
                                {
                                    Thread.Sleep(3000);
                                    sshCommunicator.ExecuteCommand(command, CommandPrompt, out response);
                                    commandResults.Add(new CommandResult() { Command = command, Output = new List<string>() { response } });
                                }

                                if (!IsCommandSucceed(response))
                                {
                                    commandsSucceed = false;
                                    break;
                                }
                            }

                            if (commandsSucceed)
                            {
                                isSuccessfull = true;
                                var succeededHuaweiRoutesWithCommands = succeededRoutesWithCommandsByRSSN.GetOrCreateItem(rssn);
                                succeededHuaweiRoutesWithCommands.Add(huaweiRouteWithCommands);
                            }
                            else
                            {
                                var allFailedHuaweiRoutesWithCommands = allFailedRoutesWithCommandsByRSSN.GetOrCreateItem(rssn);
                                allFailedHuaweiRoutesWithCommands.Add(huaweiRouteWithCommands);

                                var failedHuaweiRoutesWithCommands = failedRoutesWithCommandsByRSSN.GetOrCreateItem(rssn);
                                failedHuaweiRoutesWithCommands.Add(huaweiRouteWithCommands);
                                break;
                            }
                        }
                        catch (Exception ex)
                        {
                            numberOfTriesDone++;
                            isSuccessfull = false;
                        }
                    }
                }
            }

            sshCommunicator.ExecuteCommand(HuaweiCommands.ANBAI_Command, CommandPrompt, out response);
            commandResults.Add(new CommandResult() { Command = HuaweiCommands.ANBAI_Command, Output = new List<string>() { response } });

            sshCommunicator.ExecuteCommand(";", CommandPrompt, out response);
            commandResults.Add(new CommandResult() { Command = ";", Output = new List<string>() { response } });

            Thread.Sleep(3000);
            sshCommunicator.ExecuteCommand(HuaweiCommands.Exit_Command, ">", out response);
            commandResults.Add(new CommandResult() { Command = HuaweiCommands.Exit_Command, Output = new List<string>() { response } });

            sshCommunicator.ExecuteCommand(HuaweiCommands.Exit_Command);
            commandResults.Add(new CommandResult() { Command = HuaweiCommands.Exit_Command });
        }


        private string OpenConnectionWithSwitch(SSHCommunicator sshCommunicator, List<CommandResult> commandResults)
        {
            string response;
            sshCommunicator.OpenConnection();
            sshCommunicator.OpenShell();
            sshCommunicator.ReadPrompt(">");
            sshCommunicator.ExecuteCommand(HuaweiCommands.MML_Command, "<", out response);
            commandResults.Add(new CommandResult() { Command = HuaweiCommands.MML_Command, Output = new List<string>() { response } });
            return response;
        }

        private bool IsCommandSucceed(string response)
        {
            string responseToUpper = response.ToUpper();
            if (string.IsNullOrEmpty(response) || response.Equals(";") || responseToUpper.Contains(HuaweiCommands.EXECUTED) && !responseToUpper.Contains("NOT") || responseToUpper.Contains(HuaweiCommands.ORDERED))
                return true;

            return false;
        }

        private bool IsCommandFailed(string response, IEnumerable<string> faultCodes)
        {
            string responseToUpper = response.ToUpper();

            if (string.IsNullOrEmpty(response))
                return false;

            if (responseToUpper.Contains(HuaweiCommands.PROTECTION_PERIOD_ELAPSED) || responseToUpper.Contains(HuaweiCommands.PROTECTIVE_PERIOD_ELAPSED) || faultCodes.Any(item => responseToUpper.Contains(item)))
                return true;

            return false;
        }

        #endregion

        #endregion
    }
}