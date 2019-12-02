using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.RouteSync.Entities;
using TOne.WhS.RouteSync.Ericsson.Business;
using TOne.WhS.RouteSync.Ericsson.Data;
using TOne.WhS.RouteSync.Ericsson.Entities;
using TOne.WhS.RouteSync.MainExtensions.CodeCharge;
using TOne.WhS.RouteSync.MainExtensions.NumberLength;
using Vanrise.Caching;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;
using Vanrise.Rules;
using Vanrise.Runtime;

namespace TOne.WhS.RouteSync.Ericsson
{
    public partial class EricssonSWSync : SwitchRouteSynchronizer
    {
        public override Guid ConfigId { get { return new Guid("94739CBC-00A7-4CEB-9285-B4CB35D7D003"); } }
        public override bool SupportSyncWithinRouteBuild { get { return false; } }

        const string CommandPrompt = "<";
        public int FirstRCNumber { get; set; }
        public int NumberOfMappings { get; set; }
        public int LocalNumberLength { get; set; }
        public string InterconnectGeneralPrefix { get; set; }
        public Dictionary<string, CarrierMapping> CarrierMappings { get; set; }
        public List<EricssonCommunication> SwitchCommunicationList { get; set; }
        public List<SwitchLogger> SwitchLoggerList { get; set; }
        public BranchRouteSettings BranchRouteSettings { get; set; }
        public string PercentagePrefix { get; set; }
        public string ESR { get; set; }
        public EricssonManualRouteSettings ManualRouteSettings { get; set; }
        public int NumberOfBTables { get; set; }
        public List<int> ReservedBTables { get; set; }
        public List<ReservedBTableRange> ReservedBTableRanges { get; set; }
        public TrunkBackupsMode TrunkBackupsMode { get; set; }
        public int MinCodeLength { get; set; }
        public int MaxCodeLength { get; set; }

        private NumberLengthEvaluator _numberLengthEvaluator;
        public NumberLengthEvaluator NumberLengthEvaluator
        {
            get
            {
                if (_numberLengthEvaluator == null)
                    _numberLengthEvaluator = new FixedNumberLengthEvaluator() { MinCodeLength = MinCodeLength, MaxCodeLength = MaxCodeLength };

                return _numberLengthEvaluator;
            }
            set
            {
                _numberLengthEvaluator = value;
            }
        }

        public string CC { get; set; }

        private CodeChargeEvaluator _codeChargeEvaluator;
        public CodeChargeEvaluator CodeChargeEvaluator
        {
            get
            {
                //this.CC was not required before abstraction
                if (_codeChargeEvaluator == null && !string.IsNullOrEmpty(CC))
                    _codeChargeEvaluator = new FixedCodeChargeEvaluator() { CodeCharge = CC };

                return _codeChargeEvaluator;
            }
            set
            {
                _codeChargeEvaluator = value;
            }
        }

        #region Public Methods

        public override void Initialize(ISwitchRouteSynchronizerInitializeContext context)
        {
            if (CarrierMappings == null || CarrierMappings.Count == 0)
                return;

            IWhSRouteSyncEricssonDataManager whSRouteSyncEricssonDataManager = RouteSyncEricssonDataManagerFactory.GetDataManager<IWhSRouteSyncEricssonDataManager>();
            whSRouteSyncEricssonDataManager.SwitchId = context.SwitchId;
            whSRouteSyncEricssonDataManager.Initialize(new WhSRouteSyncEricssonInitializeContext());

            IRouteDataManager routeDataManager = RouteSyncEricssonDataManagerFactory.GetDataManager<IRouteDataManager>();
            routeDataManager.SwitchId = context.SwitchId;
            routeDataManager.Initialize(new RouteInitializeContext());

            IRouteCaseDataManager routeCaseDataManager = RouteSyncEricssonDataManagerFactory.GetDataManager<IRouteCaseDataManager>();
            routeCaseDataManager.SwitchId = context.SwitchId;
            routeCaseDataManager.Initialize(new RouteCaseInitializeContext() { FirstRCNumber = FirstRCNumber, BranchRouteSettings = BranchRouteSettings });

            ICodeGroupRouteDataManager codeGroupRouteDataManager = RouteSyncEricssonDataManagerFactory.GetDataManager<ICodeGroupRouteDataManager>();
            codeGroupRouteDataManager.SwitchId = context.SwitchId;
            codeGroupRouteDataManager.Initialize(new CodeGroupRouteInitializeContext());

            CustomerMappingManager customerMappingManager = new CustomerMappingManager();
            customerMappingManager.Initialize(context.SwitchId, CarrierMappings.Values);

            INextBTableRouteDataManager nextBTableDataManager = RouteSyncEricssonDataManagerFactory.GetDataManager<INextBTableRouteDataManager>();
            nextBTableDataManager.SwitchId = context.SwitchId;
            nextBTableDataManager.Initialize(new NextBTableInitializeContext());

            List<EricssonConvertedRoute> manualRoutes = new List<EricssonConvertedRoute>();
            if (ManualRouteSettings != null && ManualRouteSettings.EricssonManualRoutes != null && ManualRouteSettings.EricssonManualRoutes.Count > 0)
            {
                var allCustomers = new List<int>();
                var customers = CarrierMappings.Values.Where(item => item.CustomerMapping != null && item.CustomerMapping.BO != null);

                if (customers != null && customers.Any())
                {
                    foreach (var customer in customers)
                    {
                        if (customer.CustomerMapping == null || !customer.CustomerMapping.BO.HasValue)
                            continue;

                        if (customer.CustomerMapping.InternationalOBA.HasValue)
                            allCustomers.Add(customer.CustomerMapping.InternationalOBA.Value);

                        if (customer.CustomerMapping.NationalOBA.HasValue)
                            allCustomers.Add(customer.CustomerMapping.NationalOBA.Value);
                    }
                }

                foreach (var manualRoute in ManualRouteSettings.EricssonManualRoutes)
                {
                    var manualRouteActionContext = new EricssonManualRouteActionContext()
                    {
                        Customers = manualRoute.Customers,
                        Codes = manualRoute.ManualRouteOriginations.GetOriginationCodes(new EricssonManualRouteGetOriginationsContext()),
                        AllCustomers = allCustomers,
                        BlockRCNumber = FirstRCNumber
                    };
                    manualRoute.ManualRouteAction.Execute(manualRouteActionContext);
                    if (manualRouteActionContext.ManualConvertedRoutes != null && manualRouteActionContext.ManualConvertedRoutes.Count > 0)
                    {
                        foreach (var manualConvertedRoute in manualRouteActionContext.ManualConvertedRoutes)
                        {
                            if (!manualRoutes.Any(item => item.OBA == manualConvertedRoute.OBA && item.Code == manualConvertedRoute.Code && item.RouteType == manualConvertedRoute.RouteType))
                                manualRoutes.Add(manualConvertedRoute);
                        }
                    }
                }
            }
            if (manualRoutes.Count > 0)
                routeDataManager.InsertRoutesToTempTable(manualRoutes);

        }

        public override void ConvertRoutes(ISwitchRouteSynchronizerConvertRoutesContext context)
        {
            if (context.Routes == null || context.Routes.Count == 0 || CarrierMappings == null || CarrierMappings.Count == 0)
                return;

            var convertedRoutes = new List<EricssonConvertedRoute>();
            var routesToConvertByRCString = new Dictionary<string, List<EricssonConvertedRoute>>();
            var routeCases = GetCachedRouteCasesGroupedByOptions(context.SwitchId);
            var routeCasesToAdd = new HashSet<string>();
            CodeGroupManager codeGroupManager = new CodeGroupManager();
            var ruleTree = new EricssonSWSync().BuildSupplierTrunkGroupTree(CarrierMappings);
            TechnicalCodeManager technicalCodeManager = new TechnicalCodeManager();

            foreach (var route in context.Routes)
            {
                var customerCarrierMapping = CarrierMappings.GetRecord(route.CustomerId);
                if (customerCarrierMapping == null)
                    continue;

                var customerMapping = customerCarrierMapping.CustomerMapping;
                if (customerMapping == null)
                    continue;

                if (!customerMapping.BO.HasValue)
                    continue;

                if (!customerMapping.NationalOBA.HasValue && !customerMapping.InternationalOBA.HasValue)
                    continue;

                var codeGroupObject = codeGroupManager.GetMatchCodeGroup(route.Code);
                codeGroupObject.ThrowIfNull(string.Format("No Code Group found for code '{0}'.", route.Code));

                int routeCodeGroupId = codeGroupObject.CodeGroupId;
                string routeCodeGroup = codeGroupObject.Code;

                TechnicalCodePrefix technicalCodePrefix = technicalCodeManager.GetTechnicalCodeByNumberPrefix(route.Code);
                if (technicalCodePrefix == null)
                    throw new NullReferenceException($"No Technical Code Match is found for the following Code: '{route.Code}'");
                var trd = technicalCodePrefix.ZoneID;

                List<BRWithRouteCaseOptions> branchRouteWithRouteCaseOptionsList = GetBRWithRouteCaseOptionsList(route, routeCodeGroupId, routeCodeGroup, ruleTree);
                var routeCaseAsString = Helper.SerializeBRWithRouteCaseOptionsList(branchRouteWithRouteCaseOptionsList);

                if (customerMapping.InternationalOBA.HasValue)
                {
                    var ericssonConvertedRoute = new EricssonConvertedRoute
                    {
                        OBA = customerMapping.InternationalOBA.Value,
                        Code = route.Code,
                        RouteType = EricssonRouteType.BNumber,
                        TRD = trd
                    };

                    AddEricssonConvertedRouteToConvertedRoutes(convertedRoutes, routesToConvertByRCString, routeCases, routeCasesToAdd, routeCaseAsString, ericssonConvertedRoute);
                }

                //if (customerMapping.NationalOBA.HasValue)
                //{
                //    var ericssonConvertedRoute = new EricssonConvertedRoute
                //    {
                //        OBA = customerMapping.NationalOBA.Value,
                //        Code = routeCodeGroup,
                //        RouteType = EricssonRouteType.BNumber,
                //        TRD = trd
                //    };

                //    AddEricssonConvertedRouteToConvertedRoutes(convertedRoutes, routesToConvertByRCString, routeCases, routeCasesToAdd, routeCaseAsString, ericssonConvertedRoute);
                //}
            }

            if (routeCasesToAdd.Count > 0)
                routeCases = InsertAndGetRouteCases(context.SwitchId, routeCasesToAdd);

            routeCases.ThrowIfNull(string.Format("No route cases found for switch with id '{0}'", context.SwitchId));

            foreach (var routesToConvertKvp in routesToConvertByRCString)
            {
                RouteCase routeCase = routeCases.GetRecord(routesToConvertKvp.Key);
                routeCase.ThrowIfNull("No route case found");

                var routes = routesToConvertKvp.Value.Select(item => { item.RCNumber = routeCase.RCNumber; return item; });
                convertedRoutes.AddRange(routes);
            }
            EricssonConvertedRoutesPayload ericssonConvertedRoutesPayload;
            if (context.ConvertedRoutesPayload != null)
                ericssonConvertedRoutesPayload = context.ConvertedRoutesPayload.CastWithValidate<EricssonConvertedRoutesPayload>("context.ConvertedRoutesPayload");
            else
                ericssonConvertedRoutesPayload = new EricssonConvertedRoutesPayload();

            if (ericssonConvertedRoutesPayload.ConvertedRoutes == null)
                ericssonConvertedRoutesPayload.ConvertedRoutes = new List<EricssonConvertedRoute>();

            ericssonConvertedRoutesPayload.ConvertedRoutes.AddRange(convertedRoutes);
            context.ConvertedRoutesPayload = ericssonConvertedRoutesPayload;
        }

        public override void onAllRoutesConverted(ISwitchRouteSynchronizerOnAllRoutesConvertedContext context)
        {
            if (context.ConvertedRoutesPayload == null)
                return;

            EricssonConvertedRoutesPayload payload = context.ConvertedRoutesPayload.CastWithValidate<EricssonConvertedRoutesPayload>("context.ConvertedRoutesPayload");
            if (payload.ConvertedRoutes == null || payload.ConvertedRoutes.Count == 0)
                return;

            List<ConvertedRoute> routesAfterCompression = RouteSync.Business.Helper.CompressRoutesWithCodes(payload.ConvertedRoutes, CreateEricssionConvertedRoute);
            Dictionary<int, Dictionary<string, int>> routeCaseByCodeGroupByOBA;
            var convertedRoutes = ExpandConvertedRoutes(routesAfterCompression, out routeCaseByCodeGroupByOBA);

            #region NextBTablesRoutes

            var nextBTableDataManager = RouteSyncEricssonDataManagerFactory.GetDataManager<INextBTableRouteDataManager>();
            nextBTableDataManager.SwitchId = context.SwitchId;

            Dictionary<int, List<NextBTableDetails>> nextBTableDetailsByCustomerOBA = nextBTableDataManager.GetNextBTableDetailsByCustomerOBA();
            Dictionary<int, List<NextBTableDetails>> nextBTableDetailsByOBAToAdd = new Dictionary<int, List<NextBTableDetails>>();
            HashSet<int> allNextBTables = nextBTableDataManager.GetAllNextBTables();

            var nextBTableConvertedRoutes = new List<EricssonConvertedRoute>();
            TechnicalCodeManager technicalCodeManager = new TechnicalCodeManager();

            foreach (var convertedRoute in convertedRoutes)
            {
                var ericssonConvertedRoute = convertedRoute.CastWithValidate<EricssonConvertedRoute>("convertedRoute");
                if (ericssonConvertedRoute.Code.Length > 9)
                {
                    TechnicalCodePrefix technicalCodePrefix = technicalCodeManager.GetTechnicalCodeByNumberPrefix(ericssonConvertedRoute.Code);
                    if (technicalCodePrefix == null)
                        throw new NullReferenceException($"No Technical Code Match is found for the following Code: '{ericssonConvertedRoute.Code}'");
                    var trd = technicalCodePrefix.ZoneID;

                    var nextBTableNeedToBeAdd = false;
                    var nextBTableIndex = GetRouteNextBTable(nextBTableDetailsByCustomerOBA, nextBTableDetailsByOBAToAdd, allNextBTables, ericssonConvertedRoute, out nextBTableNeedToBeAdd);
                    if (!nextBTableIndex.HasValue)
                        throw new NullReferenceException("no available b tables");

                    if (nextBTableNeedToBeAdd)
                    {
                        allNextBTables.Add(nextBTableIndex.Value);
                        var obaNextBTableDetails = nextBTableDetailsByOBAToAdd.GetOrCreateItem(ericssonConvertedRoute.OBA);
                        obaNextBTableDetails.Add(new NextBTableDetails { OBA = ericssonConvertedRoute.OBA, NextBTable = nextBTableIndex.Value, Prefix = ericssonConvertedRoute.Code.Substring(0, 9) });
                    }

                    nextBTableConvertedRoutes.Add(new EricssonConvertedRoute
                    {
                        OBA = nextBTableIndex.Value,
                        Code = ericssonConvertedRoute.Code.Substring(9),
                        RCNumber = ericssonConvertedRoute.RCNumber,
                        TRD = ericssonConvertedRoute.TRD,
                        RouteType = EricssonRouteType.NextBTableRoute,
                        OriginCode = ericssonConvertedRoute.Code
                    });

                    ericssonConvertedRoute.NextBTable = nextBTableIndex;
                }
            }

            if (nextBTableConvertedRoutes.Count > 0)
                convertedRoutes.AddRange(nextBTableConvertedRoutes);

            if (nextBTableDetailsByOBAToAdd != null && nextBTableDetailsByOBAToAdd.Count > 0)
            {
                var newBTables = nextBTableDetailsByCustomerOBA.SelectMany(item => item.Value);
                nextBTableDataManager.InsertNextBTables(newBTables);
            }
            #endregion

            context.ConvertedRoutes = convertedRoutes;

            #region codeGroupRoutes
            if (routeCaseByCodeGroupByOBA != null)
            {
                List<CodeGroupRoute> codeGroupRoutes = new List<CodeGroupRoute>();
                foreach (var routeCaseByCodeGroupKVP in routeCaseByCodeGroupByOBA)
                {
                    int oba = routeCaseByCodeGroupKVP.Key;
                    Dictionary<string, int> routeCaseByCodeGroup = routeCaseByCodeGroupKVP.Value;
                    foreach (var kvp in routeCaseByCodeGroup)
                    {
                        string codeGroup = kvp.Key;
                        int rcNumber = kvp.Value;

                        CodeGroupRoute item = new CodeGroupRoute() { OBA = oba, CodeGroup = codeGroup, RCNumber = rcNumber };
                        codeGroupRoutes.Add(item);
                    }
                }

                ICodeGroupRouteDataManager codeGroupRouteDataManager = RouteSyncEricssonDataManagerFactory.GetDataManager<ICodeGroupRouteDataManager>();
                codeGroupRouteDataManager.SwitchId = context.SwitchId;
                codeGroupRouteDataManager.InsertRoutes(codeGroupRoutes);
            }
            #endregion
        }

        private ConvertedRouteWithCode CreateEricssionConvertedRoute(ICreateConvertedRouteWithCodeContext context)
        {
            int customerOBA = 0;
            if (!Int32.TryParse(context.Customer, out customerOBA))
            {
                throw new NotSupportedException("context.Customer should be integer.");
            }
            var identifiers = context.RouteOptionIdentifier.Split('@');
            return new EricssonConvertedRoute() { OBA = customerOBA, Code = context.Code, RouteType = EricssonRouteType.BNumber, TRD = int.Parse(identifiers[1]), RCNumber = int.Parse(identifiers[0]) };
        }

        public override object PrepareDataForApply(ISwitchRouteSynchronizerPrepareDataForApplyContext context)
        {
            IRouteDataManager routeDataManager = RouteSyncEricssonDataManagerFactory.GetDataManager<IRouteDataManager>();
            routeDataManager.SwitchId = context.SwitchId;
            var dbApplyStream = routeDataManager.InitialiazeStreamForDBApply();

            foreach (var convertedRoute in context.ConvertedRoutes)
                routeDataManager.WriteRecordToStream(convertedRoute as EricssonConvertedRoute, dbApplyStream);

            return routeDataManager.FinishDBApplyStream(dbApplyStream);
        }

        public override void ApplySwitchRouteSyncRoutes(ISwitchRouteSynchronizerApplyRoutesContext context)
        {
            IRouteDataManager routeDataManager = RouteSyncEricssonDataManagerFactory.GetDataManager<IRouteDataManager>();
            routeDataManager.SwitchId = context.SwitchId;
            routeDataManager.ApplyRouteForDB(context.PreparedItemsForApply);
        }

        public override void Finalize(ISwitchRouteSynchronizerFinalizeContext context)
        {
            #region conditions
            if (CarrierMappings == null || CarrierMappings.Count == 0)
                return;

            if (SwitchLoggerList == null || SwitchLoggerList.Count == 0)
                throw new Exception(string.Format("No logger specify for the switch with id {0}", context.SwitchId));

            var ftpLogger = SwitchLoggerList.First(item => item.IsActive);
            ftpLogger.ThrowIfNull(string.Format("No logger specify for the switch with id {0}", context.SwitchId));
            #endregion

            #region Managers
            ICustomerMappingSucceededDataManager customerMappingSucceededDataManager = RouteSyncEricssonDataManagerFactory.GetDataManager<ICustomerMappingSucceededDataManager>();
            customerMappingSucceededDataManager.SwitchId = context.SwitchId;

            IRouteSucceededDataManager routeSucceededDataManager = RouteSyncEricssonDataManagerFactory.GetDataManager<IRouteSucceededDataManager>();
            routeSucceededDataManager.SwitchId = context.SwitchId;

            IRouteCaseDataManager routeCaseDataManager = RouteSyncEricssonDataManagerFactory.GetDataManager<IRouteCaseDataManager>();
            routeCaseDataManager.SwitchId = context.SwitchId;

            IRouteDataManager routeDataManager = RouteSyncEricssonDataManagerFactory.GetDataManager<IRouteDataManager>();
            routeDataManager.SwitchId = context.SwitchId;

            ICustomerMappingDataManager customerMappingDataManager = RouteSyncEricssonDataManagerFactory.GetDataManager<ICustomerMappingDataManager>();
            customerMappingDataManager.SwitchId = context.SwitchId;

            ICodeGroupRouteDataManager codeGroupRouteDataManager = RouteSyncEricssonDataManagerFactory.GetDataManager<ICodeGroupRouteDataManager>();
            codeGroupRouteDataManager.SwitchId = context.SwitchId;

            #endregion

            #region Handle Special Routing
            var specialRoutes = new List<EricssonConvertedRoute>();
            if (ManualRouteSettings != null && ManualRouteSettings.EricssonSpecialRoutes != null && ManualRouteSettings.EricssonSpecialRoutes.Count > 0)
            {
                IEnumerable<int> specialRouteSourceOBAs = ManualRouteSettings.EricssonSpecialRoutes.Select(itm => itm.SourceOBA);
                Dictionary<int, List<EricssonConvertedRoute>> sourceEricssonConvertedRoutesByOBAs = routeDataManager.GetFilteredConvertedRouteByOBA(specialRouteSourceOBAs);

                Dictionary<int, List<CodeGroupRoute>> codeGroupRoutesByOBAs = codeGroupRouteDataManager.GetFilteredCodeGroupRouteByOBA(specialRouteSourceOBAs);

                if (sourceEricssonConvertedRoutesByOBAs != null && sourceEricssonConvertedRoutesByOBAs.Count > 0)
                {
                    foreach (var ericssonSpecialRoute in ManualRouteSettings.EricssonSpecialRoutes)
                    {
                        List<EricssonConvertedRoute> sourceEricssonConvertedRoutes;
                        if (sourceEricssonConvertedRoutesByOBAs.TryGetValue(ericssonSpecialRoute.SourceOBA, out sourceEricssonConvertedRoutes))
                        {
                            var specialRoutecontext = new GetSpecialRoutesContext()
                            {
                                SourceRoutes = sourceEricssonConvertedRoutes,
                                CodeGroupRoutes = codeGroupRoutesByOBAs != null ? codeGroupRoutesByOBAs.GetRecord(ericssonSpecialRoute.SourceOBA) : null
                            };

                            var routes = ericssonSpecialRoute.GetSpecialRoutes(specialRoutecontext);
                            if (routes != null && routes.Count > 0)
                                specialRoutes.AddRange(routes);
                        }
                    }
                }
            }

            if (specialRoutes.Count > 0)
                routeDataManager.InsertRoutesToTempTable(specialRoutes);

            #endregion

            #region Get Commands
            var routeCasesToBeAdded = new RouteCaseManager().GetNotSyncedRouteCases(context.SwitchId);
            var routeCasesToBeAddedWithCommands = GetRouteCasesWithCommands(routeCasesToBeAdded);

            Dictionary<int, CustomerMappingWithActionType> customersToDeleteByOBA;
            Dictionary<int, List<EricssonRouteWithCommands>> ericssonRoutesToDeleteWithCommandsByOBA;
            Dictionary<int, List<EricssonRouteWithCommands>> ericssonARoutesToDeleteWithCommandsByOBA;
            var customerMappingsWithCommands = GetCustomerMappingsWithCommands(context.SwitchId, out ericssonRoutesToDeleteWithCommandsByOBA, out ericssonARoutesToDeleteWithCommandsByOBA, out customersToDeleteByOBA);

            var ericssonRoutesWithCommandsByOBA = GetEricssonRoutesWithCommands(context.SwitchId, customersToDeleteByOBA, ericssonRoutesToDeleteWithCommandsByOBA, ericssonARoutesToDeleteWithCommandsByOBA);
            #endregion

            EricssonCommunication ericssonCommunication = null;
            if (SwitchCommunicationList != null)
            {
                ericssonCommunication = SwitchCommunicationList.FirstOrDefault(itm => itm.IsActive);
            }

            RemoteCommunicator remoteCommunicator = null;
            try
            {
                if (ericssonCommunication != null && ericssonCommunication.RemoteCommunicatorSettings != null)
                    remoteCommunicator = ericssonCommunication.RemoteCommunicatorSettings.GetCommunicator();

                DateTime finalizeTime = DateTime.Now;

                List<CommandResult> commandResults = new List<CommandResult>();

                List<string> faultCodes = null;
                int maxNumberOfRetries = 0;
                if (remoteCommunicator != null)
                {
                    var configManager = new TOne.WhS.RouteSync.Business.ConfigManager();
                    EricssonSwitchRouteSynchronizerSettings switchSettings = configManager.GetRouteSynchronizerSwitchSettings(ConfigId) as EricssonSwitchRouteSynchronizerSettings;
                    if (switchSettings == null || switchSettings.FaultCodes == null)
                        throw new NullReferenceException("Ericsson switch settings is not defined. Please go to Route Sync under Component Settings and add related settings.");
                    faultCodes = switchSettings.FaultCodes;
                    maxNumberOfRetries = switchSettings.NumberOfRetries;
                }

                #region execute and log Customer Mapping
                List<CustomerMappingWithCommands> succeedCustomerMappingsWithCommands;
                List<CustomerMappingWithCommands> failedCustomerMappingsWithCommands;
                List<CustomerMappingWithCommands> succeedCustomerMappingsWithFailedTrunk;

                var executeStatus = ExecuteCustomerMappingsCommands(customerMappingsWithCommands, ericssonCommunication, remoteCommunicator, commandResults, out succeedCustomerMappingsWithCommands, out failedCustomerMappingsWithCommands, out succeedCustomerMappingsWithFailedTrunk, faultCodes, maxNumberOfRetries);
                LogCustomerMappingCommands(succeedCustomerMappingsWithCommands, failedCustomerMappingsWithCommands, ftpLogger, finalizeTime);

                if (succeedCustomerMappingsWithCommands != null && succeedCustomerMappingsWithCommands.Count > 0)
                    customerMappingSucceededDataManager.SaveCustomerMappingsSucceededToDB(succeedCustomerMappingsWithCommands.Select(item => item.CustomerMappingWithActionType));

                List<CustomerMappingWithCommands> failedCustomerMappingToUpdate = new List<CustomerMappingWithCommands>();
                if (failedCustomerMappingsWithCommands != null && failedCustomerMappingsWithCommands.Count > 0)
                {
                    var failedAdded = failedCustomerMappingsWithCommands.FindAllRecords(item => item.CustomerMappingWithActionType.ActionType == CustomerMappingActionType.Add);
                    var failedUpdated = failedCustomerMappingsWithCommands.FindAllRecords(item => item.CustomerMappingWithActionType.ActionType == CustomerMappingActionType.Update);

                    if (failedAdded != null && failedAdded.Any())
                    {
                        if (failedAdded.Any(item => !item.CustomerMappingWithActionType.CustomerMapping.BO.HasValue))
                            throw new NullReferenceException("CustomerMappingWithActionType.CustomerMapping.BO");
                        var failedToAddBOs = failedAdded.Select(item => item.CustomerMappingWithActionType.CustomerMapping.BO.Value);
                        customerMappingDataManager.RemoveCutomerMappingsFromTempTable(failedToAddBOs);
                    }
                    if (failedUpdated != null && failedUpdated.Any())
                        failedCustomerMappingToUpdate.AddRange(failedUpdated);
                }

                if (succeedCustomerMappingsWithFailedTrunk != null && succeedCustomerMappingsWithFailedTrunk.Count > 0)
                    failedCustomerMappingToUpdate.AddRange(succeedCustomerMappingsWithFailedTrunk);

                if (failedCustomerMappingToUpdate.Count > 0)
                    customerMappingDataManager.UpdateCustomerMappingsInTempTable(failedCustomerMappingToUpdate.Select(item => item.CustomerMappingWithActionType.CustomerMapping));
                #endregion

                #region execute and log Route Cases
                List<RouteCaseWithCommands> succeedRouteCasesWithCommands;
                List<RouteCaseWithCommands> failedRouteCasesWithCommands;

                ExecuteRouteCasesCommands(routeCasesToBeAddedWithCommands, ericssonCommunication, remoteCommunicator, commandResults, out succeedRouteCasesWithCommands, out failedRouteCasesWithCommands, routeCaseDataManager, maxNumberOfRetries);
                LogRouteCaseCommands(succeedRouteCasesWithCommands, failedRouteCasesWithCommands, ftpLogger, finalizeTime);
                #endregion

                IEnumerable<int> failedRouteCaseNumbers = failedRouteCasesWithCommands?.Select(item => item.RouteCase.RCNumber);

                #region execute and log Routes

                if (failedCustomerMappingsWithCommands != null && failedCustomerMappingsWithCommands.Any(item => !item.CustomerMappingWithActionType.CustomerMapping.BO.HasValue))
                    throw new NullReferenceException("CustomerMappingWithActionType.CustomerMapping.BO");

                IEnumerable<int> failedCustomerMappingBOs = failedCustomerMappingsWithCommands?.Select(item => item.CustomerMappingWithActionType.CustomerMapping.BO.Value);

                Dictionary<int, List<EricssonRouteWithCommands>> allFailedEricssonRoutesWithCommandsByBo = new Dictionary<int, List<EricssonRouteWithCommands>>();
                List<CustomerMappingWithActionType> customerMappingsToDeleteSucceed = new List<CustomerMappingWithActionType>();
                List<CustomerMappingWithActionType> customerMappingsToDeleteFailed = new List<CustomerMappingWithActionType>();

                Dictionary<int, List<EricssonRouteWithCommands>> succeedEricssonRoutesWithCommandsByBo = null;
                Dictionary<int, List<EricssonRouteWithCommands>> failedEricssonRoutesWithCommandsByBo = null;

                Dictionary<int, List<EricssonRouteWithCommands>> succeedEricssonARoutesWithCommandsByBo = null;
                Dictionary<int, List<EricssonRouteWithCommands>> failedEricssonARoutesWithCommandsByBo = null;

                if (ericssonRoutesWithCommandsByOBA != null)
                {
                    ExecuteRoutesCommands(ericssonRoutesWithCommandsByOBA.BNumberEricssonRouteWithCommands, ericssonCommunication, remoteCommunicator, customersToDeleteByOBA, commandResults,
                      out succeedEricssonRoutesWithCommandsByBo, out failedEricssonRoutesWithCommandsByBo, allFailedEricssonRoutesWithCommandsByBo, customerMappingsToDeleteSucceed, customerMappingsToDeleteFailed
                      , failedCustomerMappingBOs, failedRouteCaseNumbers, faultCodes, maxNumberOfRetries, false);

                    ExecuteRoutesCommands(ericssonRoutesWithCommandsByOBA.ANumberEricssonRouteWithCommands, ericssonCommunication, remoteCommunicator, customersToDeleteByOBA, commandResults,
                      out succeedEricssonARoutesWithCommandsByBo, out failedEricssonARoutesWithCommandsByBo, allFailedEricssonRoutesWithCommandsByBo, customerMappingsToDeleteSucceed, customerMappingsToDeleteFailed
                      , failedCustomerMappingBOs, failedRouteCaseNumbers, faultCodes, maxNumberOfRetries, true);
                }

                LogEricssonRouteCommands(succeedEricssonRoutesWithCommandsByBo, failedEricssonRoutesWithCommandsByBo, ftpLogger, finalizeTime, false);
                LogEricssonRouteCommands(succeedEricssonARoutesWithCommandsByBo, failedEricssonARoutesWithCommandsByBo, ftpLogger, finalizeTime, true);

                #region Update the deleted customer
                if (customerMappingsToDeleteSucceed != null && customerMappingsToDeleteSucceed.Count > 0)
                    customerMappingSucceededDataManager.SaveCustomerMappingsSucceededToDB(customerMappingsToDeleteSucceed);

                if (customerMappingsToDeleteFailed != null && customerMappingsToDeleteFailed.Count > 0)
                {
                    customerMappingDataManager.InsertCutomerMappingsToTempTable(customerMappingsToDeleteFailed.Select(item => item.CustomerMapping));
                    //check bo for null
                    routeDataManager.CopyCustomerRoutesToTempTable(customerMappingsToDeleteFailed.Select(item => item.CustomerMapping.BO.Value));
                }
                customerMappingDataManager.Finalize(new CustomerMappingFinalizeContext());
                #endregion

                if (succeedEricssonRoutesWithCommandsByBo != null && succeedEricssonRoutesWithCommandsByBo.Count > 0)// save succeeded routes to succeeded table
                    routeSucceededDataManager.SaveRoutesSucceededToDB(succeedEricssonRoutesWithCommandsByBo);

                if (succeedEricssonARoutesWithCommandsByBo != null && succeedEricssonARoutesWithCommandsByBo.Count > 0)// save succeeded routes to succeeded table
                    routeSucceededDataManager.SaveRoutesSucceededToDB(succeedEricssonARoutesWithCommandsByBo);

                if (allFailedEricssonRoutesWithCommandsByBo != null && allFailedEricssonRoutesWithCommandsByBo.Count > 0)
                {
                    var failedAdded = new List<EricssonConvertedRoute>();
                    var failedUpdated = new List<EricssonConvertedRoute>();
                    var failedDeleted = new List<EricssonConvertedRoute>();

                    foreach (var failedEricssonRoutesWithCommands in allFailedEricssonRoutesWithCommandsByBo)
                    {
                        foreach (var route in failedEricssonRoutesWithCommands.Value)
                        {
                            if (route.ActionType == RouteActionType.Add)
                                failedAdded.Add(route.RouteCompareResult.Route);
                            if (route.ActionType == RouteActionType.Update)
                                failedUpdated.Add(route.RouteCompareResult.OriginalValue);
                            if (route.ActionType == RouteActionType.Delete)
                                failedDeleted.Add(route.RouteCompareResult.Route);
                        }
                    }
                    if (failedDeleted != null && failedDeleted.Count > 0)
                        routeDataManager.InsertRoutesToTempTable(failedDeleted);

                    if (failedAdded != null && failedAdded.Count > 0)
                        routeDataManager.RemoveRoutesFromTempTable(failedAdded);

                    if (failedUpdated != null && failedUpdated.Count > 0)
                        routeDataManager.UpdateRoutesInTempTable(failedUpdated);
                }

                routeDataManager.Finalize(new RouteFinalizeContext());
                #endregion

                LogAllCommands(commandResults, ftpLogger, finalizeTime);
            }
            finally
            {
                try
                {
                    if (remoteCommunicator != null)
                        remoteCommunicator.Dispose();
                }
                catch (Exception ex)
                {
                    context.WriteBusinessHandledException(ex, true);
                }
            }
        }

        public override bool IsSwitchRouteSynchronizerValid(IIsSwitchRouteSynchronizerValidContext context)
        {
            List<string> validationMessages = new List<string>();
            HashSet<int> allBOs = new HashSet<int>();
            HashSet<int> duplicatedBOs = new HashSet<int>();
            HashSet<int> allOBAs = new HashSet<int>();
            HashSet<int> duplicatedOBAs = new HashSet<int>();
            HashSet<int> manualRoutesMissingOBAs = new HashSet<int>();
            HashSet<int> specialRoutingTargetInvalidOBAs = new HashSet<int>();
            HashSet<int> specialRoutingSourceMissingOBAs = new HashSet<int>();

            HashSet<Guid> allOutTrunks = new HashSet<Guid>();
            HashSet<string> supplierHavingManySupplierTrunkBackup = new HashSet<string>();
            HashSet<string> suppliersWithDeletedOutTrunksInTrunkGroup = new HashSet<string>();
            HashSet<string> suppliersWithDeletedOutTrunksInBackup = new HashSet<string>();

            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();

            HashSet<int> customerMappingOBAsExistingInNextBTables = new HashSet<int>();
            HashSet<int> reservedBTablesExistingInNextBTables = new HashSet<int>();
            HashSet<int> targetOBAsExistingInNextBTables = new HashSet<int>();

            HashSet<int> nextBTables = new HashSet<int>();

            if (!string.IsNullOrEmpty(context.SwitchId))
            {
                var nextBTableDataManager = RouteSyncEricssonDataManagerFactory.GetDataManager<INextBTableRouteDataManager>();
                nextBTableDataManager.SwitchId = context.SwitchId.ToString();
                nextBTables = nextBTableDataManager.GetAllNextBTables();
            }

            if (this.CarrierMappings == null || this.CarrierMappings.Count == 0)
            {
                if (ManualRouteSettings != null)
                {
                    if (ManualRouteSettings.EricssonManualRoutes != null && ManualRouteSettings.EricssonManualRoutes.Count > 0)
                        manualRoutesMissingOBAs.UnionWith(ManualRouteSettings.EricssonManualRoutes.SelectMany(item => item.Customers));

                    if (this.ManualRouteSettings.EricssonSpecialRoutes != null && this.ManualRouteSettings.EricssonSpecialRoutes.Count > 0)
                    {
                        specialRoutingSourceMissingOBAs.UnionWith(ManualRouteSettings.EricssonSpecialRoutes.Select(item => item.SourceOBA));

                        foreach (var specialRoute in ManualRouteSettings.EricssonSpecialRoutes)
                        {
                            if (nextBTables.Contains(specialRoute.TargetOBA))
                                targetOBAsExistingInNextBTables.Add(specialRoute.TargetOBA);
                        }
                    }
                }
            }
            else
            {
                foreach (var carrierMapping in this.CarrierMappings)
                {
                    CustomerMapping customerMapping = carrierMapping.Value.CustomerMapping;

                    if (customerMapping != null && customerMapping.BO.HasValue)
                    {
                        if (!allBOs.Contains(customerMapping.BO.Value))
                            allBOs.Add(customerMapping.BO.Value);

                        else
                            duplicatedBOs.Add(customerMapping.BO.Value);

                        if (customerMapping.NationalOBA.HasValue)
                        {
                            if (!allOBAs.Contains(customerMapping.NationalOBA.Value))
                                allOBAs.Add(customerMapping.NationalOBA.Value);
                            else
                                duplicatedOBAs.Add(customerMapping.NationalOBA.Value);


                            if (nextBTables.Contains(customerMapping.NationalOBA.Value))
                                customerMappingOBAsExistingInNextBTables.Add(customerMapping.BO.Value);
                        }

                        if (customerMapping.InternationalOBA.HasValue)
                        {
                            if (!allOBAs.Contains(customerMapping.InternationalOBA.Value))
                                allOBAs.Add(customerMapping.InternationalOBA.Value);
                            else
                                duplicatedOBAs.Add(customerMapping.InternationalOBA.Value);


                            if (nextBTables.Contains(customerMapping.InternationalOBA.Value))
                                customerMappingOBAsExistingInNextBTables.Add(customerMapping.BO.Value);
                        }
                    }

                    SupplierMapping supplierMapping = carrierMapping.Value.SupplierMapping;
                    if (supplierMapping == null)
                        continue;

                    string supplierName = carrierAccountManager.GetCarrierAccountName(carrierMapping.Value.CarrierId);

                    if (supplierMapping.OutTrunks != null)
                    {
                        foreach (var outTrunk in supplierMapping.OutTrunks)
                            allOutTrunks.Add(outTrunk.TrunkId);
                    }

                    if (supplierMapping.TrunkGroups != null)
                    {
                        foreach (var trunkGroup in supplierMapping.TrunkGroups)
                        {
                            if (trunkGroup.TrunkTrunkGroups == null)
                                continue;

                            foreach (var trunkTrunkGroup in trunkGroup.TrunkTrunkGroups)
                            {
                                if (trunkTrunkGroup.Backups == null)
                                    continue;

                                foreach (var backup in trunkTrunkGroup.Backups)
                                {
                                    if (backup.Trunks == null || backup.Trunks.Count < 2)
                                        continue;

                                    supplierHavingManySupplierTrunkBackup.Add(supplierName);
                                }
                            }
                        }
                    }
                }

                foreach (var carrierMapping in this.CarrierMappings)
                {
                    string supplierName = carrierAccountManager.GetCarrierAccountName(carrierMapping.Value.CarrierId);

                    SupplierMapping supplierMapping = carrierMapping.Value.SupplierMapping;
                    if (supplierMapping == null || supplierMapping.TrunkGroups == null)
                        continue;

                    foreach (var trunkGroup in supplierMapping.TrunkGroups)
                    {
                        if (trunkGroup.TrunkTrunkGroups == null)
                            continue;

                        foreach (var trunkTrunkGroup in trunkGroup.TrunkTrunkGroups)
                        {
                            if (!allOutTrunks.Contains(trunkTrunkGroup.TrunkId))
                                suppliersWithDeletedOutTrunksInTrunkGroup.Add(supplierName);

                            if (trunkTrunkGroup.Backups == null)
                                continue;

                            foreach (var backup in trunkTrunkGroup.Backups)
                            {
                                if (backup.Trunks == null)
                                    continue;

                                foreach (var supplierTrunkBackup in backup.Trunks)
                                {
                                    if (supplierTrunkBackup != null && !allOutTrunks.Contains(supplierTrunkBackup.TrunkId))
                                        suppliersWithDeletedOutTrunksInBackup.Add(supplierName);
                                }
                            }
                        }
                    }
                }

                if (this.ManualRouteSettings != null)
                {
                    if (ManualRouteSettings.EricssonManualRoutes != null)
                    {
                        foreach (var manualRoute in ManualRouteSettings.EricssonManualRoutes)
                        {
                            foreach (var customer in manualRoute.Customers)
                            {
                                if (!allOBAs.Contains(customer))
                                    manualRoutesMissingOBAs.Add(customer);
                            }
                        }
                    }

                    if (ManualRouteSettings.EricssonSpecialRoutes != null)
                    {
                        foreach (var specialRoute in ManualRouteSettings.EricssonSpecialRoutes)
                        {
                            if (!allOBAs.Contains(specialRoute.SourceOBA))
                                specialRoutingSourceMissingOBAs.Add(specialRoute.SourceOBA);

                            if (!allOBAs.Contains(specialRoute.TargetOBA))
                                specialRoutingTargetInvalidOBAs.Add(specialRoute.TargetOBA);

                            if (nextBTables.Contains(specialRoute.TargetOBA))
                                targetOBAsExistingInNextBTables.Add(specialRoute.TargetOBA);
                        }
                    }
                }
            }

            if (ReservedBTables != null && ReservedBTables.Count > 0)
            {
                foreach (var reservedBTable in ReservedBTables)
                {
                    if (nextBTables.Contains(reservedBTable))
                        reservedBTablesExistingInNextBTables.Add(reservedBTable);
                }
            }

            if (ReservedBTableRanges != null && ReservedBTableRanges.Count > 0)
            {
                foreach (var nextBTable in nextBTables)
                {
                    foreach (var reservedBTableRange in ReservedBTableRanges)
                    {
                        if (reservedBTableRange.IsBTableIncluded(nextBTable))
                        {
                            reservedBTablesExistingInNextBTables.Add(nextBTable);
                            break;
                        }
                    }
                }
            }

            if (duplicatedBOs.Count > 0)
                validationMessages.Add($"Carrier Mapping Duplicated BOs: {string.Join(", ", duplicatedBOs)}");

            if (duplicatedOBAs.Count > 0)
                validationMessages.Add($"Carrier Mapping Duplicated OBAs: {string.Join(", ", duplicatedOBAs)}");

            if (manualRoutesMissingOBAs.Count > 0)
                validationMessages.Add($"Following manual routes OBAs are not defined in carrier mapping : {string.Join(", ", manualRoutesMissingOBAs)}");

            if (specialRoutingSourceMissingOBAs.Count > 0)
                validationMessages.Add($"Following special routes Source OBAs are not defined in carrier mapping : {string.Join(", ", specialRoutingSourceMissingOBAs)}");

            if (specialRoutingTargetInvalidOBAs.Count > 0)
                validationMessages.Add($"Following special routes Target OBAs already defined in carrier mapping : {string.Join(", ", specialRoutingTargetInvalidOBAs)}");

            if (supplierHavingManySupplierTrunkBackup.Count > 0)
                validationMessages.Add($"Following suppliers have trunk group backups with more than one trunk : {string.Join(", ", supplierHavingManySupplierTrunkBackup)}");

            if (suppliersWithDeletedOutTrunksInTrunkGroup.Count > 0)
                validationMessages.Add($"Following suppliers have deleted trunks in their trunk groups : {string.Join(", ", suppliersWithDeletedOutTrunksInTrunkGroup)}");

            if (suppliersWithDeletedOutTrunksInBackup.Count > 0)
                validationMessages.Add($"Following suppliers have deleted trunks in their trunk group backups : {string.Join(", ", suppliersWithDeletedOutTrunksInBackup)}");

            if (customerMappingOBAsExistingInNextBTables.Count > 0)
                validationMessages.Add(string.Format("Carrier Mapping OBAs existing in Next BTables: {0}", string.Join(", ", customerMappingOBAsExistingInNextBTables)));

            if (reservedBTablesExistingInNextBTables.Count > 0)
                validationMessages.Add(string.Format("Reserved BTable OBAs existing in Next BTables: {0}", string.Join(", ", reservedBTablesExistingInNextBTables)));

            if (targetOBAsExistingInNextBTables.Count > 0)
                validationMessages.Add(string.Format("Special Route Target OBAs existing in Next BTables: {0}", string.Join(", ", targetOBAsExistingInNextBTables)));

            context.ValidationMessages = validationMessages.Count > 0 ? validationMessages : null;
            return validationMessages.Count == 0;
        }

        public override void RemoveConnection(ISwitchRouteSynchronizerRemoveConnectionContext context)
        {
            SwitchCommunicationList = null;
            SwitchLoggerList = null;
        }

        #endregion

        #region Private Methods

        #region Get Route Case Options

        private List<BRWithRouteCaseOptions> GetBRWithRouteCaseOptionsList(Route route, int codeGroupId, string routeCodeGroup, RuleTree ruleTree)
        {
            List<BaseBRWithRouteCaseOptions> result = new List<BaseBRWithRouteCaseOptions>();
            var branchRoutes = BranchRouteSettings.GetBaseBranchRoutes(new GetBaseBranchRoutesContext());

            foreach (var branchRoute in branchRoutes)
            {
                BaseBRWithRouteCaseOptions record = new BaseBRWithRouteCaseOptions();
                var routeCaseOptions = new List<RouteCaseOption>();
                record.BranchRoute = branchRoute;
                int numberOfMappings = 0;
                int groupId = 1;

                var routeCaseOptionWithSupplierList = new List<RouteCaseOptionWithSupplier>();
                var routeCaseOptionsBySupplierId = new Dictionary<string, List<RouteCaseOption>>();

                if (route.Options != null && route.Options.Count > 0)
                {
                    bool isPercentageOption = route.Options.Any(itm => itm.Percentage.HasValue && itm.Percentage.Value > 0);
                    bool isFirstSupplier = true;

                    foreach (var option in route.Options)
                    {
                        var optionRouteCaseOptions = new List<RouteCaseOption>();

                        if (option.IsBlocked)
                            continue;

                        if (isPercentageOption && (option.Percentage == null || option.Percentage.Value == 0))
                            continue;

                        var supplierMapping = GetSupplierMapping(option.SupplierId);
                        if (supplierMapping == null)
                            continue;

                        var trunkGroup = GetTrunkGroup(ruleTree, option.SupplierId, codeGroupId, route.CustomerId, false);
                        if (trunkGroup == null)
                            continue;

                        if (trunkGroup.TrunkTrunkGroups != null && trunkGroup.TrunkTrunkGroups.Count > 0)
                        {
                            foreach (var trunkGroupTrunk in trunkGroup.TrunkTrunkGroups)
                            {
                                var trunk = supplierMapping.OutTrunks.FindRecord(item => item.TrunkId == trunkGroupTrunk.TrunkId);

                                if (trunk.IsSwitch && !branchRoute.IncludeTrunkAsSwitch)
                                    continue;

                                if (trunk.IsSwitch && branchRoute.OverflowOnFirstOptionOnly && !isFirstSupplier)
                                    continue;

                                numberOfMappings++;

                                optionRouteCaseOptions.Add(GetRouteCaseOption(trunk, routeCodeGroup, option.SupplierId, option.Percentage, supplierMapping, trunkGroup, trunkGroupTrunk, groupId, numberOfMappings));

                                if (numberOfMappings == NumberOfMappings)
                                    break;
                            }

                            if (option.Percentage.HasValue && optionRouteCaseOptions != null && optionRouteCaseOptions.Count > 0)
                            {
                                routeCaseOptionWithSupplierList.Add(new RouteCaseOptionWithSupplier() { SupplierId = option.SupplierId, Percentage = option.Percentage.Value, RouteCaseOptions = optionRouteCaseOptions });
                            }

                            if (optionRouteCaseOptions != null && optionRouteCaseOptions.Count > 0)
                            {
                                routeCaseOptions.AddRange(optionRouteCaseOptions);
                                isFirstSupplier = false;
                            }
                        }

                        #region option backups
                        if (option.Backups != null && numberOfMappings < NumberOfMappings)
                        {
                            foreach (var backupOption in option.Backups)
                            {
                                if (backupOption.IsBlocked)
                                    continue;

                                var backupSupplierMapping = GetSupplierMapping(backupOption.SupplierId);
                                if (backupSupplierMapping == null)
                                    continue;

                                var backupTrunkGroup = GetTrunkGroup(ruleTree, backupOption.SupplierId, codeGroupId, route.CustomerId, true);
                                if (backupTrunkGroup == null)
                                    continue;

                                if (backupTrunkGroup.TrunkTrunkGroups != null && backupTrunkGroup.TrunkTrunkGroups.Count > 0)
                                {
                                    foreach (var trunkGroupTrunk in backupTrunkGroup.TrunkTrunkGroups)
                                    {
                                        var trunk = backupSupplierMapping.OutTrunks.FindRecord(item => item.TrunkId == trunkGroupTrunk.TrunkId);
                                        if (trunk.IsSwitch && !branchRoute.IncludeTrunkAsSwitch)
                                            continue;

                                        if (trunk.IsSwitch && branchRoute.OverflowOnFirstOptionOnly)
                                            continue;

                                        numberOfMappings++;

                                        routeCaseOptions.Add(GetRouteCaseOption(trunk, routeCodeGroup, backupOption.SupplierId, null, backupSupplierMapping, backupTrunkGroup, trunkGroupTrunk, groupId, numberOfMappings));

                                        if (numberOfMappings == NumberOfMappings)
                                            break;
                                    }
                                }
                            }
                        }
                        #endregion

                        if (isPercentageOption)
                        {
                            if (numberOfMappings > 0)
                                groupId++;

                            numberOfMappings = 0;
                        }

                        else if (numberOfMappings == NumberOfMappings)
                        {
                            break;
                        }
                    }

                    ReevaluatePercentageDistributionForSuppliersList(routeCaseOptionWithSupplierList);

                    if (routeCaseOptions.Count > 0)
                        result.Add(new BaseBRWithRouteCaseOptions() { BranchRoute = branchRoute, RouteCaseOptions = routeCaseOptions });
                }
            }

            var routeCaseOptionsWithBranchRoutes = BranchRouteSettings.GetMergedBaseBranchRoutesWithRouteCaseOptions(new GetMergedBaseBranchRoutesWithRouteCaseOptionsContext() { BaseBRWithRouteCaseOptions = result });
            if (routeCaseOptionsWithBranchRoutes == null || routeCaseOptionsWithBranchRoutes.Count == 0)
                routeCaseOptionsWithBranchRoutes = BranchRouteSettings.GetBlockedBRWithRouteCaseOptions(new GetBlockedBRWithRouteCaseOptionsContext());

            return routeCaseOptionsWithBranchRoutes;
        }

        private void ReevaluatePercentageDistributionForSuppliersList(List<RouteCaseOptionWithSupplier> routeCaseOptionWithSupplierList)
        {
            if (routeCaseOptionWithSupplierList == null || routeCaseOptionWithSupplierList.Count == 0)
                return;

            Utilities.RedistributePercentagePerWeight(routeCaseOptionWithSupplierList.Select(itm => itm as IPercentageItem).ToList());

            foreach (var currentRouteCaseOptionWithSupplier in routeCaseOptionWithSupplierList)
                ReevaluatePercentageDistributionForSupplier(currentRouteCaseOptionWithSupplier);
        }

        private void ReevaluatePercentageDistributionForSupplier(RouteCaseOptionWithSupplier routeCaseOptionWithSupplier)
        {
            if (routeCaseOptionWithSupplier.RouteCaseOptions == null || routeCaseOptionWithSupplier.RouteCaseOptions.Count == 0)
                return;

            if (!routeCaseOptionWithSupplier.RouteCaseOptions.Any(itm => itm.TrunkPercentage.HasValue && itm.TrunkPercentage > 0))
            {
                routeCaseOptionWithSupplier.RouteCaseOptions.First().TrunkPercentage = routeCaseOptionWithSupplier.Percentage;
                return;
            }

            var orderedOptions = routeCaseOptionWithSupplier.RouteCaseOptions.OrderBy(item => item.TrunkPercentage).ToList();
            var assignedPercentage = orderedOptions.Where(item => item.TrunkPercentage.HasValue).Sum(item => item.TrunkPercentage.Value);

            int percentageDiff = routeCaseOptionWithSupplier.Percentage - assignedPercentage;
            var lastIndex = orderedOptions.Count - 1;

            if (percentageDiff > 0)
            {
                while (percentageDiff > 0)
                {
                    for (int i = 0; i < orderedOptions.Count; i++)
                    {
                        if (orderedOptions[lastIndex - i].TrunkPercentage.HasValue && orderedOptions[lastIndex - i].TrunkPercentage > 0)
                        {
                            orderedOptions[lastIndex - i].TrunkPercentage++;
                            percentageDiff--;
                        }
                        if (percentageDiff == 0)
                            break;
                    }
                }
            }

            else if (percentageDiff < 0)
            {
                int minValue = 1;
                int oldPercentageDiff = percentageDiff;
                while (percentageDiff < 0)
                {
                    for (int i = 0; i < orderedOptions.Count; i++)
                    {
                        if (orderedOptions[lastIndex - i].TrunkPercentage.HasValue && orderedOptions[i].TrunkPercentage > minValue)
                        {
                            orderedOptions[i].TrunkPercentage--;
                            percentageDiff++;
                        }
                        if (percentageDiff == 0)
                            break;
                    }

                    if (oldPercentageDiff == percentageDiff)
                        minValue = 0;

                    oldPercentageDiff = percentageDiff;
                }
            }
        }

        private RouteCaseOption GetRouteCaseOption(OutTrunk trunk, string routeCodeGroup, string supplierId, int? percentage, SupplierMapping supplierMapping, TrunkGroup trunkGroup, TrunkTrunkGroup trunkGroupTrunk, int groupId, int numberOfMappings)
        {
            RouteCaseOption routeCaseOption = new RouteCaseOption();
            int? routeCaseOptionPercentage = null;
            if (percentage.HasValue)
            {
                if (trunkGroupTrunk.Percentage.HasValue)
                    routeCaseOptionPercentage = Convert.ToInt32(percentage.Value * trunkGroupTrunk.Percentage.Value / 100.0);
            }

            routeCaseOption.Percentage = percentage;
            routeCaseOption.IsSwitch = trunk.IsSwitch;
            routeCaseOption.OutTrunk = trunk.TrunkName;
            routeCaseOption.Type = trunk.TrunkType;
            routeCaseOption.TrunkPercentage = routeCaseOptionPercentage;
            routeCaseOption.IsBackup = trunkGroup.IsBackup;
            routeCaseOption.GroupID = groupId;
            routeCaseOption.BNT = 1;
            routeCaseOption.SP = 1;
            routeCaseOption.SupplierId = supplierId;

            if (!string.IsNullOrEmpty(trunk.NationalCountryCode))
            {
                var nationalCountryCodes = trunk.NationalCountryCode.Split(';');
                if (nationalCountryCodes.Contains(routeCodeGroup))
                {
                    routeCaseOption.BNT = 4;
                    routeCaseOption.SP = Convert.ToInt16(routeCodeGroup.Length + 1);
                }
            }

            return routeCaseOption;
        }

        private SupplierMapping GetSupplierMapping(string supplierId)
        {
            if (CarrierMappings == null || CarrierMappings.Count == 0)
                return null;

            var supplierCarrierMapping = CarrierMappings.GetRecord(supplierId);

            if (supplierCarrierMapping == null)
                return null;

            return supplierCarrierMapping.SupplierMapping;
        }

        private TrunkGroup GetTrunkGroup(RuleTree ruleTree, string supplierId, int codeGroupId, string customerId, bool isBackup)
        {
            GenericRuleTarget target = new GenericRuleTarget();
            target.TargetFieldValues = new Dictionary<string, object>();
            target.TargetFieldValues.Add("Supplier", int.Parse(supplierId));
            target.TargetFieldValues.Add("CodeGroup", codeGroupId);
            target.TargetFieldValues.Add("Customer", int.Parse(customerId));
            target.TargetFieldValues.Add("IsBackUp", isBackup);

            TrunkGroupRuleAsGeneric matchingRule = Vanrise.GenericData.Business.GenericRuleManager<GenericRule>.GetMatchRule<TrunkGroupRuleAsGeneric>(ruleTree, null, target);
            if (matchingRule == null)
                return null;

            return matchingRule.TrunkGroup;
        }
        #endregion

        #region Commands For CustomerMapping changes

        private List<CustomerMappingWithCommands> GetCustomerMappingsWithCommands(string switchId, out Dictionary<int, List<EricssonRouteWithCommands>> routesToDeleteCommandsByBo, out Dictionary<int, List<EricssonRouteWithCommands>> aRoutesToDeleteCommandsByBo,
        out Dictionary<int, CustomerMappingWithActionType> customersToDeleteByBO)
        {
            customersToDeleteByBO = new Dictionary<int, CustomerMappingWithActionType>();
            routesToDeleteCommandsByBo = new Dictionary<int, List<EricssonRouteWithCommands>>();
            aRoutesToDeleteCommandsByBo = new Dictionary<int, List<EricssonRouteWithCommands>>();

            CustomerMappingTablesContext customerMappingTablesContext = new CustomerMappingTablesContext();
            ICustomerMappingDataManager customerMappingDataManager = RouteSyncEricssonDataManagerFactory.GetDataManager<ICustomerMappingDataManager>();
            customerMappingDataManager.SwitchId = switchId;

            customerMappingDataManager.CompareTables(customerMappingTablesContext);

            List<CustomerMappingWithCommands> customerMappingsWithCommands = new List<CustomerMappingWithCommands>();

            if (customerMappingTablesContext.CustomerMappingsToAdd != null && customerMappingTablesContext.CustomerMappingsToAdd.Count > 0)//Add
            {
                foreach (var customerMappingSerializedToAdd in customerMappingTablesContext.CustomerMappingsToAdd)
                {
                    var customerMappingToAdd = Helper.DeserializeCustomerMapping(customerMappingSerializedToAdd.CustomerMappingAsString);

                    if (customerMappingToAdd != null)
                    {
                        var customerMappingToAddOBACommands = GetCustomerMappingOBACommands(customerMappingToAdd);

                        customerMappingsWithCommands.Add(new CustomerMappingWithCommands()
                        {
                            CustomerMappingWithActionType = new CustomerMappingWithActionType() { CustomerMapping = customerMappingToAdd, ActionType = CustomerMappingActionType.Add },
                            OBACommands = customerMappingToAddOBACommands,
                        });
                    }
                }
            }

            if (customerMappingTablesContext.CustomerMappingsToUpdate != null && customerMappingTablesContext.CustomerMappingsToUpdate.Count > 0)//Update
            {
                foreach (var customerMappingSerializedToUpdate in customerMappingTablesContext.CustomerMappingsToUpdate)
                {
                    var customerMappingToUpdate = Helper.DeserializeCustomerMapping(customerMappingSerializedToUpdate.CustomerMappingAsString);
                    var customerMappingOldValue = Helper.DeserializeCustomerMapping(customerMappingSerializedToUpdate.CustomerMappingOldValueAsString);

                    if (customerMappingToUpdate != null)
                    {
                        var customerMappingToUpdateOBACommands = GetCustomerMappingOBACommands(customerMappingToUpdate);

                        customerMappingsWithCommands.Add(new CustomerMappingWithCommands()
                        {
                            OBACommands = customerMappingToUpdateOBACommands,
                            CustomerMappingWithActionType = new CustomerMappingWithActionType() { CustomerMapping = customerMappingToUpdate, CustomerMappingOldValue = customerMappingOldValue, ActionType = CustomerMappingActionType.Update }
                        });
                    }
                }
            }

            if (customerMappingTablesContext.CustomerMappingsToDelete != null && customerMappingTablesContext.CustomerMappingsToDelete.Count > 0)//Delete
            {
                foreach (var customerMappingsSerializedToDelete in customerMappingTablesContext.CustomerMappingsToDelete)
                {
                    var customerMappingsToDelete = Helper.DeserializeCustomerMapping(customerMappingsSerializedToDelete.CustomerMappingAsString);
                    if (customerMappingsToDelete != null)
                    {
                        var customerMappingToDeleteCommands = GetCustomerMappingDeleteCommands(customerMappingsToDelete);
                        var customerMappingToDeleteARouteCommands = GetARouteCustomerMappingDeleteCommands(customerMappingsToDelete);

                        if (!customerMappingsToDelete.BO.HasValue)
                            throw new NullReferenceException("customerMappingsToDelete.BO");

                        List<EricssonRouteWithCommands> ericssonRoutesWithCommands = routesToDeleteCommandsByBo.GetOrCreateItem(customerMappingsToDelete.BO.Value);
                        ericssonRoutesWithCommands.Add(new EricssonRouteWithCommands() { Commands = customerMappingToDeleteCommands, ActionType = RouteActionType.DeleteCustomer });

                        List<EricssonRouteWithCommands> ericssonARoutesWithCommands = aRoutesToDeleteCommandsByBo.GetOrCreateItem(customerMappingsToDelete.BO.Value);
                        ericssonARoutesWithCommands.Add(new EricssonRouteWithCommands() { Commands = customerMappingToDeleteARouteCommands, ActionType = RouteActionType.DeleteCustomer });

                        if (customersToDeleteByBO.ContainsKey(customerMappingsToDelete.BO.Value))
                            throw new DataIntegrityValidationException(string.Format("There is two deleted customer mapping with the same BO {0}", customerMappingsToDelete.BO));

                        customersToDeleteByBO.Add(customerMappingsToDelete.BO.Value, new CustomerMappingWithActionType() { CustomerMapping = customerMappingsToDelete, ActionType = CustomerMappingActionType.Delete });
                    }
                }
            }

            return customerMappingsWithCommands;
        }

        private List<string> GetCustomerMappingOBACommands(CustomerMapping customerMappingToAdd)
        {
            List<string> customerMappingOBACommands = new List<string>();
            customerMappingOBACommands.Add(string.Format("{0}:BO={1},NAPI=1,BNT=1,OBA={2};", EricssonCommands.PNBSI_Command, customerMappingToAdd.BO, customerMappingToAdd.InternationalOBA));
            customerMappingOBACommands.Add(string.Format("{0}:BO={1},NAPI=1,BNT=4,OBA={2};", EricssonCommands.PNBSI_Command, customerMappingToAdd.BO, customerMappingToAdd.NationalOBA));

            return customerMappingOBACommands;
        }

        private List<string> GetCustomerMappingDeleteCommands(CustomerMapping customerMappingToDelete)
        {
            return new List<string>() { string.Format("{0}:B={1};", EricssonCommands.ANBSE_Command, customerMappingToDelete.BO) };
        }

        private List<string> GetARouteCustomerMappingDeleteCommands(CustomerMapping customerMappingToDelete)
        {
            return new List<string>() { string.Format("{0}:B={1};", EricssonCommands.ANASE_Command, customerMappingToDelete.BO) };//Remove ARoute
        }


        #endregion

        #region Commands For RouteCase changes
        private List<RouteCaseWithCommands> GetRouteCasesWithCommands(IEnumerable<RouteCase> routeCases)
        {
            if (routeCases == null || !routeCases.Any())
                return null;

            List<RouteCaseWithCommands> routeCasesWithCommands = new List<RouteCaseWithCommands>();

            foreach (var routeCaseToAdd in routeCases)
            {
                var routeCaseCommands = GetRouteCaseCommands(routeCaseToAdd);
                routeCasesWithCommands.Add(new RouteCaseWithCommands() { RouteCase = routeCaseToAdd, Commands = routeCaseCommands });
            }

            return routeCasesWithCommands;
        }

        private List<string> GetRouteCaseCommands(RouteCase routeCase)
        {
            if (routeCase == null)
                return null;

            List<string> routeCaseCommands = new List<string>();
            string command;
            routeCaseCommands.Add(string.Format("{0}:RC={1},CCH=NO;", EricssonCommands.ANRPI_Command, routeCase.RCNumber));
            var deserializedBRWithRouteCaseOptionsList = Helper.DeserializeBRWithRouteCaseOptionsList(routeCase.RouteCaseAsString);

            string esrCommand = ",";
            int pValue = 0;

            foreach (var branchRouteWithRouteCaseOptions in deserializedBRWithRouteCaseOptionsList)
            {
                var brName = branchRouteWithRouteCaseOptions.BranchRoute.Name;
                var brAlternativeName = (!string.IsNullOrEmpty(branchRouteWithRouteCaseOptions.BranchRoute.AlternativeName)) ? branchRouteWithRouteCaseOptions.BranchRoute.AlternativeName : branchRouteWithRouteCaseOptions.BranchRoute.Name;

                if (branchRouteWithRouteCaseOptions.RouteCaseOptions == null || branchRouteWithRouteCaseOptions.RouteCaseOptions.Count == 0)
                    throw new ArgumentNullException("branchRouteWithRouteCaseOptions is null or empty");

                var optionGroups = branchRouteWithRouteCaseOptions.RouteCaseOptions.GroupBy(item => item.GroupID);

                if (branchRouteWithRouteCaseOptions.RouteCaseOptions.Any(item => item.IsBackup) || branchRouteWithRouteCaseOptions.RouteCaseOptions.Any(item => item.Percentage.HasValue && item.Percentage > 0))
                {
                    foreach (var optionGroup in optionGroups)
                    {
                        pValue++;
                        int priority = 1;

                        foreach (RouteCaseOption option in optionGroup)
                        {
                            if (!option.IsSwitch && !string.IsNullOrEmpty(ESR))
                                esrCommand = string.Format(",ESR={0},", ESR);
                            else esrCommand = ",";

                            command = option.TrunkPercentage.HasValue && option.TrunkPercentage > 0 ? string.Format("{0}:BR={1}{2}{3},P0{4}={5},R={6}{7}BNT={8},SP=MM{9};", EricssonCommands.ANRSI_Command, brName, PercentagePrefix, option.TrunkPercentage, pValue, priority, option.OutTrunk, esrCommand, option.BNT, option.SP)
                                : string.Format("{0}:BR={1},P0{2}={3},R={4}{5}BNT={6},SP=MM{7};", EricssonCommands.ANRSI_Command, brName, pValue, priority, option.OutTrunk, esrCommand, option.BNT, option.SP);
                            routeCaseCommands.Add(command);
                            if (priority == NumberOfMappings)
                                break;
                            priority++;
                        }
                    }
                }
                else
                {
                    pValue++;
                    int priority = 1;

                    foreach (var optionGroup in optionGroups)
                    {
                        foreach (RouteCaseOption option in optionGroup)
                        {
                            if (!option.IsSwitch && !string.IsNullOrEmpty(ESR))
                                esrCommand = string.Format(",ESR={0},", ESR);
                            else esrCommand = ",";

                            if (optionGroup.Key == 1)
                            {
                                command = option.TrunkPercentage.HasValue && option.TrunkPercentage > 0 ? string.Format("{0}:BR={1}{2}{3},P0{4}={5},R={6}{7}BNT={8},SP=MM{9};", EricssonCommands.ANRSI_Command, brName, PercentagePrefix, option.TrunkPercentage, pValue, priority, option.OutTrunk, esrCommand, option.BNT, option.SP)
                                    : string.Format("{0}:BR={1},P0{2}={3},R={4}{5}BNT={6},SP=MM{7};", EricssonCommands.ANRSI_Command, brName, pValue, priority, option.OutTrunk, esrCommand, option.BNT, option.SP);
                            }
                            else
                            {
                                command = option.TrunkPercentage.HasValue && option.TrunkPercentage > 0 ? string.Format("{0}:BR={1}{2}{3},P0{4}={5},R={6}{7}BNT={8},SP=MM{9};", EricssonCommands.ANRSI_Command, brAlternativeName, PercentagePrefix, option.TrunkPercentage, pValue, priority, option.OutTrunk, esrCommand, option.BNT, option.SP)
                                    : string.Format("{0}:BR={1},P0{2}={3},R={4}{5}BNT={6},SP=MM{7};", EricssonCommands.ANRSI_Command, brAlternativeName, pValue, priority, option.OutTrunk, esrCommand, option.BNT, option.SP);
                            }
                            routeCaseCommands.Add(command);
                            if (priority == NumberOfMappings)
                                break;
                            priority++;
                        }
                        //if (priority == NumberOfMappings)
                        //  break;
                    }
                }
            }

            routeCaseCommands.Add(string.Format("{0};", EricssonCommands.ANRPE_Command));
            return routeCaseCommands;
        }

        #endregion

        #region NextBTable
        private int? GetRouteNextBTable(Dictionary<int, List<NextBTableDetails>> nextBTableDetailsByCustomerOBA, Dictionary<int, List<NextBTableDetails>> nextBTableDetailsByCustomerOBAToAdd, HashSet<int> allNextBTables, EricssonConvertedRoute route, out bool nextBTableNeedToBeAdd)
        {
            nextBTableNeedToBeAdd = false;
            var code = route.Code.Substring(9);
            var prefix = route.Code.Substring(0, 9);
            List<NextBTableDetails> customerBTables = new List<NextBTableDetails>();
            if (nextBTableDetailsByCustomerOBA.TryGetValue(route.OBA, out customerBTables))
            {
                var customerBTable = customerBTables.Find(item => item.Prefix == prefix);
                if (customerBTable != null)
                    return customerBTable.NextBTable;
            }

            if (nextBTableDetailsByCustomerOBAToAdd.TryGetValue(route.OBA, out customerBTables))
            {
                var customerBTable = customerBTables.Find(item => item.Prefix == prefix);
                if (customerBTable != null)
                    return customerBTable.NextBTable;
            }

            nextBTableNeedToBeAdd = true;
            return GetNextAvailableBTable(allNextBTables);
        }

        private int? GetNextAvailableBTable(HashSet<int> allNextBTables)
        {
            IEnumerable<CarrierMapping> customers = null;
            if (CarrierMappings != null && CarrierMappings.Count > 0)
                customers = CarrierMappings.Values.Where(item => item.CustomerMapping != null && item.CustomerMapping.BO.HasValue);

            List<int> customersOBAs = new List<int>();
            if (customers != null && customers.Any())
            {
                foreach (var customer in customers)
                {
                    if (customer.CustomerMapping.NationalOBA.HasValue)
                        customersOBAs.Add(customer.CustomerMapping.NationalOBA.Value);

                    if (customer.CustomerMapping.InternationalOBA.HasValue)
                        customersOBAs.Add(customer.CustomerMapping.InternationalOBA.Value);
                }
            }

            for (int i = 1; i <= NumberOfBTables; i++)
            {
                if (ReservedBTables != null && ReservedBTables.Contains(i))
                    continue;

                if (ReservedBTableRanges != null && ReservedBTableRanges.Any((item => item.IsBTableIncluded(i))))
                    continue;

                if (customersOBAs.Contains(i))
                    continue;

                if (ManualRouteSettings.EricssonSpecialRoutes.Any(item => item.TargetOBA == i))
                    continue;

                if (allNextBTables != null && allNextBTables.Contains(i))
                    continue;

                return i;
            }
            return null;
        }

        #endregion

        #region Commands For Route changes
        private EricssonRoutesWithCommands GetEricssonRoutesWithCommands(string switchId, Dictionary<int, CustomerMappingWithActionType> customersToDeleteByOBA,
        Dictionary<int, List<EricssonRouteWithCommands>> ericssonRoutesToDeleteWithCommands, Dictionary<int, List<EricssonRouteWithCommands>> ericssonARoutesToDeleteWithCommands)
        {

            var result = ericssonRoutesToDeleteWithCommands != null ? new Dictionary<int, List<EricssonRouteWithCommands>>(ericssonRoutesToDeleteWithCommands) : new Dictionary<int, List<EricssonRouteWithCommands>>();
            var aRouteResult = ericssonARoutesToDeleteWithCommands != null ? new Dictionary<int, List<EricssonRouteWithCommands>>(ericssonARoutesToDeleteWithCommands) : new Dictionary<int, List<EricssonRouteWithCommands>>();

            RouteCompareTablesContext routeCompareTablesContext = new RouteCompareTablesContext();
            IRouteDataManager routeDataManager = RouteSyncEricssonDataManagerFactory.GetDataManager<IRouteDataManager>();
            routeDataManager.SwitchId = switchId;
            routeDataManager.CompareTables(routeCompareTablesContext);

            var routeDifferencesByOBA = routeCompareTablesContext.RouteDifferencesByOBA;

            if (routeDifferencesByOBA != null && routeDifferencesByOBA.Count > 0)
            {
                foreach (var routeDifferencesKvp in routeCompareTablesContext.RouteDifferencesByOBA)
                {
                    var routeDifferences = routeDifferencesKvp.Value;
                    var customerEricssonRoutesWithCommands = result.GetOrCreateItem(routeDifferencesKvp.Key);
                    var aRouteCustomerEricssonRoutesWithCommands = aRouteResult.GetOrCreateItem(routeDifferencesKvp.Key);

                    if (routeDifferences.RoutesToAdd != null && routeDifferences.RoutesToAdd.Count > 0) //Add
                    {
                        foreach (var routeCompareResult in routeDifferences.RoutesToAdd)
                        {
                            var commands = GetRouteCommand(routeCompareResult.Route);

                            if (Utilities.GetEnumAttribute<EricssonRouteType, EricssonRouteTypeAttribute>(routeCompareResult.Route.RouteType).IsARoute)
                                aRouteCustomerEricssonRoutesWithCommands.Add(new EricssonRouteWithCommands() { RouteCompareResult = routeCompareResult, Commands = commands, ActionType = RouteActionType.Add });
                            else
                                customerEricssonRoutesWithCommands.Add(new EricssonRouteWithCommands() { RouteCompareResult = routeCompareResult, Commands = commands, ActionType = RouteActionType.Add });
                        }
                    }

                    if (routeDifferences.RoutesToUpdate != null && routeDifferences.RoutesToUpdate.Count > 0) //Update
                    {
                        foreach (var routeCompareResult in routeDifferences.RoutesToUpdate)
                        {
                            var commands = GetRouteCommand(routeCompareResult.Route);

                            if (Utilities.GetEnumAttribute<EricssonRouteType, EricssonRouteTypeAttribute>(routeCompareResult.Route.RouteType).IsARoute)
                                aRouteCustomerEricssonRoutesWithCommands.Add(new EricssonRouteWithCommands() { RouteCompareResult = routeCompareResult, Commands = commands, ActionType = RouteActionType.Update });

                            else customerEricssonRoutesWithCommands.Add(new EricssonRouteWithCommands() { RouteCompareResult = routeCompareResult, Commands = commands, ActionType = RouteActionType.Update });
                        }
                    }

                    if (routeDifferences.RoutesToDelete != null && routeDifferences.RoutesToDelete.Count > 0)//Delete
                    {
                        foreach (var routeCompareResult in routeDifferences.RoutesToDelete)
                        {
                            if (!customersToDeleteByOBA.ContainsKey(routeCompareResult.Route.OBA))
                            {
                                var commands = GetDeletedRouteCommands(routeCompareResult);

                                if (Utilities.GetEnumAttribute<EricssonRouteType, EricssonRouteTypeAttribute>(routeCompareResult.Route.RouteType).IsARoute)
                                    aRouteCustomerEricssonRoutesWithCommands.Add(new EricssonRouteWithCommands() { RouteCompareResult = routeCompareResult, Commands = commands, ActionType = RouteActionType.Delete });

                                else customerEricssonRoutesWithCommands.Add(new EricssonRouteWithCommands() { RouteCompareResult = routeCompareResult, Commands = commands, ActionType = RouteActionType.Delete });
                            }
                        }
                    }
                }
            }

            return new EricssonRoutesWithCommands() { BNumberEricssonRouteWithCommands = result, ANumberEricssonRouteWithCommands = aRouteResult };
        }

        private List<string> GetDeletedRouteCommands(EricssonConvertedRouteCompareResult routeCompareResult)
        {
            List<string> deletedRouteCommands = new List<string>();

            var route = routeCompareResult.Route;
            EricssonRouteProperties ericssonRouteProperties = GetRouteTypeAndParameters(route.Code, route.OriginCode, route.RouteType);

            string deleteEricssonCommands = null;

            if (Utilities.GetEnumAttribute<EricssonRouteType, EricssonRouteTypeAttribute>(routeCompareResult.Route.RouteType).IsARoute)
                deleteEricssonCommands = EricssonCommands.ANASE_Command;
            else deleteEricssonCommands = EricssonCommands.ANBSE_Command;

            switch (ericssonRouteProperties.Type)
            {
                case EricssonConvertedRouteType.Normal:
                    deletedRouteCommands.Add(string.Format("{0}:B={1}-{2};", deleteEricssonCommands, route.OBA, route.Code));
                    break;
                default:
                    throw new ArgumentNullException("Route type is not supported.");
            }

            return deletedRouteCommands;
        }

        private EricssonRouteProperties GetRouteTypeAndParameters(string routeCode, string routeOriginCode, EricssonRouteType routeType)
        {
            string routeCodeGroup;
            if (routeType == EricssonRouteType.NextBTableRoute)
                routeCodeGroup = GetMatchedCodeGroupCode(routeOriginCode);
            else
                routeCodeGroup = GetMatchedCodeGroupCode(routeCode);

            var ericssonRouteParameters = new EricssonRouteProperties();
            ericssonRouteParameters.Type = EricssonConvertedRouteType.Normal;
            ericssonRouteParameters.M = InterconnectGeneralPrefix;
            ericssonRouteParameters.CCL = routeCodeGroup.StartsWith("1") ? "1" : routeCodeGroup.Length > 3 ? "3" : routeCodeGroup.Length.ToString();
            ericssonRouteParameters.CC = "1";
            ericssonRouteParameters.D = "7-0";

            return ericssonRouteParameters;
        }

        private string GetMatchedCodeGroupCode(string code)
        {
            var codeGroupObject = new CodeGroupManager().GetMatchCodeGroup(code);
            codeGroupObject.ThrowIfNull(string.Format("CodeGroup not found for code '{0}'.", code));
            return codeGroupObject.Code;
        }

        private List<string> GetRouteCommand(EricssonConvertedRoute route)
        {
            StringBuilder strCommand = new StringBuilder();
            string L = null;
            string B = null;

            EricssonRouteProperties routeParamerters = GetRouteTypeAndParameters(route.Code, route.OriginCode, route.RouteType);

            switch (routeParamerters.Type)
            {
                case EricssonConvertedRouteType.Normal:

                    if (!route.NextBTable.HasValue)
                    {
                        B = string.Format("{0}-{1}", route.OBA, route.Code);

                        if (Utilities.GetEnumAttribute<EricssonRouteType, EricssonRouteTypeAttribute>(route.RouteType).IsSpecialRoute)
                            L = (Convert.ToInt32(routeParamerters.CCL) + 1).ToString() + "-" + MaxCodeLength.ToString();

                        else if (route.Code.Length > MinCodeLength)
                            L = route.Code.Length.ToString() + "-" + MaxCodeLength.ToString();

                        string mValue = string.IsNullOrEmpty(routeParamerters.M) ? null : string.Format("0-{0}", routeParamerters.M);

                        strCommand.Append(GetRouteCommandText(B, route.RCNumber, L, mValue, routeParamerters.D, routeParamerters.CC, routeParamerters.CCL, null, null, route.RouteType, route.TRD));
                    }
                    else
                    {
                        B = string.Format("{0}-{1}", route.OBA, route.Code.Substring(0, 9));
                        strCommand.Append(string.Format("{0}:B={1},N={2};", EricssonCommands.ANBSI_Command, B, route.NextBTable.Value.ToString()));
                    }
                    break;

                default:
                    throw new ArgumentNullException("Route type is not supported.");
            }

            return new List<string> { strCommand.ToString() };
        }

        private StringBuilder GetRouteCommandText(string B, int RC, string L, string M, string D, string cc, string CCL, string F, string BNT, EricssonRouteType routeType, int trd)
        {
            string insertCommand = "";
            string bCommand = "";

            if (Utilities.GetEnumAttribute<EricssonRouteType, EricssonRouteTypeAttribute>(routeType).IsARoute)
            {
                insertCommand = EricssonCommands.ANASI_Command;
                bCommand = "A";
            }
            else
            {
                insertCommand = EricssonCommands.ANBSI_Command;
                bCommand = "B";
            }

            StringBuilder script = new StringBuilder(string.Format("{0}:", insertCommand));

            if (!string.IsNullOrEmpty(B))
                script.AppendFormat("{0}={1}", bCommand, B);

            if (Utilities.GetEnumAttribute<EricssonRouteType, EricssonRouteTypeAttribute>(routeType).IsARoute && RC == FirstRCNumber)
            {
                script.AppendFormat(",ES=550");
            }
            else
            {
                script.AppendFormat(",RC={0},TRD={1}", RC, trd);

                if (!string.IsNullOrEmpty(L))
                    script.AppendFormat(",L={0}", L);

                if (Utilities.GetEnumAttribute<EricssonRouteType, EricssonRouteTypeAttribute>(routeType).IsSpecialRoute)
                    script.AppendFormat(",CW,LAD", M);

                if (!string.IsNullOrEmpty(M))
                    script.AppendFormat(",M={0}", M);

                if (!string.IsNullOrEmpty(D))
                    script.AppendFormat(",D={0}", D);

                var ccValue = (!string.IsNullOrEmpty(CC)) ? CC : cc;
                if (!string.IsNullOrEmpty(ccValue))
                    script.AppendFormat(",CC={0}", ccValue);

                if (!string.IsNullOrEmpty(CCL))
                    script.AppendFormat(",CCL={0}", CCL);

                if (!string.IsNullOrEmpty(F))
                    script.AppendFormat(",F={0}", F);

                if (!string.IsNullOrEmpty(BNT))
                    script.AppendFormat(",BNT={0}", BNT);
            }

            script.Append(";");
            return script;
        }

        #endregion

        #region LOG File

        private static void LogEricssonRouteCommands(Dictionary<int, List<EricssonRouteWithCommands>> succeedEricssonRoutesWithCommandsByOBA, Dictionary<int, List<EricssonRouteWithCommands>> failedEricssonRoutesWithCommandsByOBA, SwitchLogger ftpLogger, DateTime dateTime, bool isAroute)
        {
            if (succeedEricssonRoutesWithCommandsByOBA != null && succeedEricssonRoutesWithCommandsByOBA.Count > 0)
            {
                foreach (var customerRoutesWithCommandsKvp in succeedEricssonRoutesWithCommandsByOBA)
                {
                    var customerRoutesWithCommands = customerRoutesWithCommandsKvp.Value;
                    var commandResults = customerRoutesWithCommands.Select(item => new CommandResult() { Command = string.Join(Environment.NewLine, item.Commands) }).ToList();
                    ILogRoutesContext logRoutesContext = new LogRoutesContext() { ExecutionDateTime = dateTime, ExecutionStatus = ExecutionStatus.Succeeded, CommandResults = commandResults, CustomerIdentifier = customerRoutesWithCommandsKvp.Key.ToString() };

                    if (isAroute)
                        ftpLogger.LogARoutes(logRoutesContext);
                    else
                        ftpLogger.LogRoutes(logRoutesContext);
                }
            }

            if (failedEricssonRoutesWithCommandsByOBA != null && failedEricssonRoutesWithCommandsByOBA.Count > 0)
            {
                foreach (var customerRoutesWithCommandsKvp in failedEricssonRoutesWithCommandsByOBA)
                {
                    var customerRoutesWithCommands = customerRoutesWithCommandsKvp.Value;
                    var commandResults = customerRoutesWithCommands.Select(item => new CommandResult() { Command = string.Join(Environment.NewLine, item.Commands) }).ToList();
                    ILogRoutesContext logRoutesContext = new LogRoutesContext() { ExecutionDateTime = dateTime, ExecutionStatus = ExecutionStatus.Failed, CommandResults = commandResults, CustomerIdentifier = customerRoutesWithCommandsKvp.Key.ToString() };
                    if (isAroute)
                        ftpLogger.LogARoutes(logRoutesContext);
                    else
                        ftpLogger.LogRoutes(logRoutesContext);
                }
            }
        }

        private static void LogCustomerMappingCommands(List<CustomerMappingWithCommands> succeedCustomerMappingsWithCommands, List<CustomerMappingWithCommands> failedCustomerMappingsWithCommands, SwitchLogger ftpLogger, DateTime dateTime)
        {
            if (succeedCustomerMappingsWithCommands != null && succeedCustomerMappingsWithCommands.Count > 0)
            {
                List<CommandResult> commandResults = new List<CommandResult>();
                foreach (var succeedCustomerMappingWithCommands in succeedCustomerMappingsWithCommands)
                {
                    var obaCommands = string.Join(Environment.NewLine, succeedCustomerMappingWithCommands.OBACommands);

                    commandResults.Add(new CommandResult() { Command = obaCommands });
                }
                ILogCarrierMappingsContext logRouteCasesContext = new LogCarrierMappingsContext() { ExecutionDateTime = dateTime, ExecutionStatus = ExecutionStatus.Succeeded, CommandResults = commandResults };
                ftpLogger.LogCarrierMappings(logRouteCasesContext);
            }

            if (failedCustomerMappingsWithCommands != null && failedCustomerMappingsWithCommands.Count > 0)
            {
                List<CommandResult> commandResults = new List<CommandResult>();
                foreach (var failedCustomerMappingWithCommands in failedCustomerMappingsWithCommands)
                {
                    var obaCommands = string.Join(Environment.NewLine, failedCustomerMappingWithCommands.OBACommands);

                    commandResults.Add(new CommandResult() { Command = obaCommands });
                }
                ILogCarrierMappingsContext logRouteCasesContext = new LogCarrierMappingsContext() { ExecutionDateTime = dateTime, ExecutionStatus = ExecutionStatus.Failed, CommandResults = commandResults };
                ftpLogger.LogCarrierMappings(logRouteCasesContext);
            }
        }

        private static void LogRouteCaseCommands(List<RouteCaseWithCommands> succeedRouteCasesWithCommands, List<RouteCaseWithCommands> failedRouteCasesWithCommands, SwitchLogger ftpLogger, DateTime dateTime)
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

        private static void LogAllCommands(List<CommandResult> allCommands, SwitchLogger ftpLogger, DateTime dateTime)
        {
            if (allCommands != null && allCommands.Count > 0)
            {
                ILogCommandsContext logRouteCasesContext = new LogCommandsContext() { ExecutionDateTime = dateTime, CommandResults = allCommands };
                ftpLogger.LogCommands(logRouteCasesContext);
            }
        }
        #endregion

        #region Communicator
        private string OpenConnectionWithSwitch(RemoteCommunicator remoteCommunicator, List<CommandResult> commandResults)
        {
            string response;
            remoteCommunicator.OpenConnection();
            remoteCommunicator.OpenShell();
            remoteCommunicator.ReadPrompt(">");
            remoteCommunicator.ExecuteCommand(EricssonCommands.MML_Command, "<", out response);
            commandResults.Add(new CommandResult() { Command = EricssonCommands.MML_Command, Output = new List<string>() { response } });
            return response;
        }

        private bool ExecuteCustomerMappingsCommands(List<CustomerMappingWithCommands> customerMappingsWithCommands, EricssonCommunication ericssonCommunication,
        RemoteCommunicator remoteCommunicator, List<CommandResult> commandResults, out List<CustomerMappingWithCommands> succeededCustomerMappingsWithCommands,
        out List<CustomerMappingWithCommands> failedCustomerMappingsWithCommands, out List<CustomerMappingWithCommands> succeedCustomerMappingsWithFailedTrunk,
        IEnumerable<string> faultCodes, int maxNumberOfRetries)
        {
            succeededCustomerMappingsWithCommands = new List<CustomerMappingWithCommands>();
            failedCustomerMappingsWithCommands = new List<CustomerMappingWithCommands>();
            succeedCustomerMappingsWithFailedTrunk = new List<CustomerMappingWithCommands>();

            if (customerMappingsWithCommands == null || customerMappingsWithCommands.Count == 0)
                return false;

            if (ericssonCommunication == null)
            {
                succeededCustomerMappingsWithCommands = customerMappingsWithCommands;
                return true;
            }

            bool isPreTableSucceed = false;
            int numberOfTriesDone = 0;

            string response;
            response = OpenConnectionWithSwitch(remoteCommunicator, commandResults);

            while (!isPreTableSucceed && numberOfTriesDone < maxNumberOfRetries)
            {
                try
                {
                    remoteCommunicator.ExecuteCommand(EricssonCommands.PNBAR_Command, CommandPrompt, out response);
                    commandResults.Add(new CommandResult() { Command = EricssonCommands.PNBAR_Command, Output = new List<string>() { response } });

                    remoteCommunicator.ExecuteCommand(";", CommandPrompt, out response);
                    commandResults.Add(new CommandResult() { Command = ";", Output = new List<string>() { response } });

                    if (IsCommandFailed(response, faultCodes))
                    {
                        remoteCommunicator.ExecuteCommand(EricssonCommands.PNBZI_Command, CommandPrompt, out response);
                        commandResults.Add(new CommandResult() { Command = EricssonCommands.PNBZI_Command, Output = new List<string>() { response } });

                        remoteCommunicator.ExecuteCommand(EricssonCommands.PNBCI_Command, CommandPrompt, out response);
                        commandResults.Add(new CommandResult() { Command = EricssonCommands.PNBCI_Command, Output = new List<string>() { response } });
                    }

                    foreach (var customerMappingWithCommands in customerMappingsWithCommands)
                    {
                        int customerNumberOfTriesDone = 0;
                        bool isCustomerSucceed = false;
                        var customerMappingWithActionType = customerMappingWithCommands.CustomerMappingWithActionType;
                        var customerMapping = customerMappingWithCommands.CustomerMappingWithActionType.CustomerMapping;

                        while (!isCustomerSucceed && customerNumberOfTriesDone < maxNumberOfRetries)
                        {
                            try
                            {
                                isCustomerSucceed = true;
                                CustomerMappingWithCommands customerMappingSucceededWithCommands = null;
                                CustomerMappingWithCommands customerMappingFailedWithCommands = null;

                                if (customerMappingWithCommands.OBACommands == null || customerMappingWithCommands.OBACommands.Count == 0)
                                    throw new NullReferenceException(string.Format("There is no OBA commands for customer with BO '{0}'", customerMapping.BO));

                                foreach (var command in customerMappingWithCommands.OBACommands)
                                {
                                    remoteCommunicator.ExecuteCommand(command, CommandPrompt, out response);
                                    commandResults.Add(new CommandResult() { Command = command, Output = new List<string>() { response } });
                                    if (!IsCommandSucceed(response))
                                    {
                                        customerMappingFailedWithCommands = customerMappingWithCommands;
                                        isCustomerSucceed = false;
                                        break;
                                    }
                                }

                                if (isCustomerSucceed)
                                {
                                    customerMappingSucceededWithCommands = new CustomerMappingWithCommands();
                                    customerMappingSucceededWithCommands.CustomerMappingWithActionType = new CustomerMappingWithActionType()
                                    {
                                        CustomerMapping = new CustomerMapping() { BO = customerMapping.BO, InternationalOBA = customerMapping.InternationalOBA, NationalOBA = customerMapping.NationalOBA },
                                        ActionType = customerMappingWithActionType.ActionType
                                    };

                                    customerMappingSucceededWithCommands.OBACommands = customerMappingWithCommands.OBACommands;
                                    succeededCustomerMappingsWithCommands.Add(customerMappingSucceededWithCommands);
                                }
                                if (customerMappingFailedWithCommands != null)
                                    failedCustomerMappingsWithCommands.Add(customerMappingFailedWithCommands);
                            }
                            catch (Exception ex)
                            {
                                customerNumberOfTriesDone++;
                                isCustomerSucceed = false;
                            }
                        }
                        if (!isCustomerSucceed)
                        {
                            var customerMappingFailedWithCommands = customerMappingWithCommands;
                            failedCustomerMappingsWithCommands.Add(customerMappingFailedWithCommands);
                        }
                    }

                    remoteCommunicator.ExecuteCommand(EricssonCommands.PNBAI_Command, CommandPrompt, out response);
                    commandResults.Add(new CommandResult() { Command = EricssonCommands.PNBAI_Command, Output = new List<string>() { response } });

                    remoteCommunicator.ExecuteCommand(";", CommandPrompt, out response);
                    commandResults.Add(new CommandResult() { Command = ";", Output = new List<string>() { response } });

                    remoteCommunicator.ExecuteCommand(EricssonCommands.Exit_Command, ">", out response);
                    commandResults.Add(new CommandResult() { Command = EricssonCommands.Exit_Command, Output = new List<string>() { response } });

                    remoteCommunicator.ExecuteCommand(EricssonCommands.Exit_Command);
                    commandResults.Add(new CommandResult() { Command = EricssonCommands.Exit_Command });

                    isPreTableSucceed = true;
                }

                catch (Exception ex)
                {
                    numberOfTriesDone++;
                    isPreTableSucceed = false;
                }
            }
            return isPreTableSucceed;
        }

        private void ExecuteRouteCasesCommands(List<RouteCaseWithCommands> routeCasesWithCommands, EricssonCommunication ericssonCommunication,
        RemoteCommunicator remoteCommunicator, List<CommandResult> commandResults, out List<RouteCaseWithCommands> succeedRouteCaseNumbers, out List<RouteCaseWithCommands> failedRouteCaseNumbers, IRouteCaseDataManager routeCaseDataManager, int maxNumberOfRetries)
        {
            int batchSize = 100;
            var routeCaseNumbersToUpdate = new List<int>();
            succeedRouteCaseNumbers = new List<RouteCaseWithCommands>();
            failedRouteCaseNumbers = new List<RouteCaseWithCommands>();
            if (routeCasesWithCommands == null || routeCasesWithCommands.Count == 0)
                return;

            if (ericssonCommunication == null)
            {
                succeedRouteCaseNumbers = routeCasesWithCommands;
                routeCaseDataManager.UpdateSyncedRouteCases(succeedRouteCaseNumbers.Select(item => item.RouteCase.RCNumber));
                return;
            }

            bool isSuccessfull;
            int numberOfTriesDone;

            string response;

            response = OpenConnectionWithSwitch(remoteCommunicator, commandResults);

            foreach (var routeCaseWithCommands in routeCasesWithCommands)
            {
                isSuccessfull = false;
                numberOfTriesDone = 0;
                var rcNumber = routeCaseWithCommands.RouteCase.RCNumber;

                while (!isSuccessfull && numberOfTriesDone < maxNumberOfRetries)
                {
                    try
                    {
                        string command = routeCaseWithCommands.Commands[0];
                        remoteCommunicator.ExecuteCommand(command, CommandPrompt, out response);
                        commandResults.Add(new CommandResult() { Command = command, Output = new List<string>() { response } });
                        if (!IsCommandSucceed(response))
                        {
                            string commandTemp = string.Format("{0}:RC={1};", EricssonCommands.ANRAR_Command, rcNumber);
                            remoteCommunicator.ExecuteCommand(commandTemp, CommandPrompt, out response);
                            commandResults.Add(new CommandResult() { Command = commandTemp, Output = new List<string>() { response } });

                            remoteCommunicator.ExecuteCommand(";", CommandPrompt, out response);
                            commandResults.Add(new CommandResult() { Command = ";", Output = new List<string>() { response } });

                            if (IsCommandSucceed(response))
                            {
                                commandTemp = string.Format("{0}:RC={1};", EricssonCommands.ANRZI_Command, rcNumber);
                                remoteCommunicator.ExecuteCommand(commandTemp, CommandPrompt, out response);
                                commandResults.Add(new CommandResult() { Command = commandTemp, Output = new List<string>() { response } });
                            }

                            remoteCommunicator.ExecuteCommand(command, CommandPrompt, out response);
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
                            remoteCommunicator.ExecuteCommand(routeCaseCommand, CommandPrompt, out response);
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
                        command = string.Format("{0}:RC={1};", EricssonCommands.ANRAI_Command, rcNumber);
                        remoteCommunicator.ExecuteCommand(command, CommandPrompt, out response);
                        commandResults.Add(new CommandResult() { Command = command, Output = new List<string>() { response } });
                        if (!IsCommandSucceed(response))
                        {
                            failedRouteCaseNumbers.Add(routeCaseWithCommands);
                            break;
                        }
                        remoteCommunicator.ExecuteCommand(";", CommandPrompt, out response);
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
                            routeCaseDataManager.UpdateSyncedRouteCases(routeCaseNumbersToUpdate);
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
                if (!isSuccessfull)
                {
                    failedRouteCaseNumbers.Add(routeCaseWithCommands);
                }
            }

            if (routeCaseNumbersToUpdate.Count > 0)
            {
                routeCaseDataManager.UpdateSyncedRouteCases(routeCaseNumbersToUpdate);
                routeCaseNumbersToUpdate = new List<int>();
            }
        }

        private void ExecuteRoutesCommands(Dictionary<int, List<EricssonRouteWithCommands>> routesWithCommandsByOBA, EricssonCommunication ericssonCommunication,
        RemoteCommunicator remoteCommunicator, Dictionary<int, CustomerMappingWithActionType> customersToDeleteByOBA, List<CommandResult> commandResults, out Dictionary<int, List<EricssonRouteWithCommands>> succeededRoutesWithCommandsByOBA,
        out Dictionary<int, List<EricssonRouteWithCommands>> failedRoutesWithCommandsByOBA, Dictionary<int, List<EricssonRouteWithCommands>> allFailedRoutesWithCommandsByOBA,
        List<CustomerMappingWithActionType> customerMappingsToDeleteSucceed, List<CustomerMappingWithActionType> customerMappingsToDeleteFailed,
        IEnumerable<int> failedCustomerMappingOBAs, IEnumerable<int> faildRouteCaseNumbers, IEnumerable<string> faultCodes, int maxNumberOfRetries, bool isAroute)
        {
            succeededRoutesWithCommandsByOBA = new Dictionary<int, List<EricssonRouteWithCommands>>();
            failedRoutesWithCommandsByOBA = new Dictionary<int, List<EricssonRouteWithCommands>>();

            if (routesWithCommandsByOBA == null || routesWithCommandsByOBA.Count == 0)
                return;

            foreach (var ericssonRouteWithCommandsKvp in routesWithCommandsByOBA)
            {
                var ericssonRoutesWithCommands = ericssonRouteWithCommandsKvp.Value;
                ericssonRoutesWithCommands.Sort((x, y) => x.RouteCompareResult.Route.Code.CompareTo(y.RouteCompareResult.Route.Code));
            }
            if (ericssonCommunication == null)
            {
                succeededRoutesWithCommandsByOBA = routesWithCommandsByOBA;
                if (customersToDeleteByOBA.Values != null && customersToDeleteByOBA.Values.Count > 0)
                {
                    foreach (var customerToDeleteKvp in customersToDeleteByOBA)
                    {
                        if (!customerMappingsToDeleteSucceed.Any(item =>
                        {
                            return (item.CustomerMapping.NationalOBA.HasValue && item.CustomerMapping.NationalOBA.Value == customerToDeleteKvp.Key) ||
                            (item.CustomerMapping.InternationalOBA.HasValue && item.CustomerMapping.InternationalOBA.Value == customerToDeleteKvp.Key);
                        }))
                            customerMappingsToDeleteSucceed.Add(customerToDeleteKvp.Value);
                    }
                }
                return;
            }

            bool isSuccessfull;
            int numberOfTriesDone;
            string response;

            var aNxARCommand = EricssonCommands.ANBAR_Command;
            var aNxZICommand = EricssonCommands.ANBZI_Command;
            var aNxCICommand = EricssonCommands.ANBCI_Command;
            var aNxAICommand = EricssonCommands.ANBAI_Command;

            if (isAroute)
            {
                aNxARCommand = EricssonCommands.ANAAR_Command;
                aNxZICommand = EricssonCommands.ANAZI_Command;
                aNxCICommand = EricssonCommands.ANACI_Command;
                aNxAICommand = EricssonCommands.ANAAI_Command;
            }

            OpenConnectionWithSwitch(remoteCommunicator, commandResults);
            remoteCommunicator.ExecuteCommand(aNxARCommand, CommandPrompt, out response);
            commandResults.Add(new CommandResult() { Command = aNxARCommand, Output = new List<string>() { response } });
            remoteCommunicator.ExecuteCommand(";", CommandPrompt, out response);
            commandResults.Add(new CommandResult() { Command = ";", Output = new List<string>() { response } });

            if (IsCommandFailed(response, faultCodes))
            {
                remoteCommunicator.ExecuteCommand(aNxZICommand, CommandPrompt, out response);
                commandResults.Add(new CommandResult() { Command = aNxZICommand, Output = new List<string>() { response } });
                Thread.Sleep(5000);

                if (response.ToUpper().Contains(EricssonCommands.ORDERED))
                {
                    remoteCommunicator.ExecuteCommand(EricssonCommands.Exit_Command, ">", out response);
                    commandResults.Add(new CommandResult() { Command = EricssonCommands.Exit_Command, Output = new List<string>() { response } });
                    Thread.Sleep(2000);
                    remoteCommunicator.ExecuteCommand(EricssonCommands.MML_Command, "<", out response);
                    commandResults.Add(new CommandResult() { Command = EricssonCommands.MML_Command, Output = new List<string>() { response } });
                    Thread.Sleep(1000);
                }
                else if (!IsCommandSucceed(response))
                {
                    throw new Exception(string.Format("{0} Not Executed", aNxARCommand));
                }

                remoteCommunicator.ExecuteCommand(aNxCICommand, CommandPrompt, out response);
                commandResults.Add(new CommandResult() { Command = aNxCICommand, Output = new List<string>() { response } });

                Thread.Sleep(5000);
                while (!response.ToUpper().Contains(EricssonCommands.ORDERED) && response.ToUpper().Contains("BUSY"))
                {
                    remoteCommunicator.ExecuteCommand(aNxCICommand, CommandPrompt, out response);
                    commandResults.Add(new CommandResult() { Command = aNxCICommand, Output = new List<string>() { response } });
                    Thread.Sleep(3000);
                }
                if (response.ToUpper().Contains(EricssonCommands.ORDERED))
                {
                    remoteCommunicator.ExecuteCommand(EricssonCommands.Exit_Command, ">", out response);
                    commandResults.Add(new CommandResult() { Command = EricssonCommands.Exit_Command, Output = new List<string>() { response } });
                    Thread.Sleep(2000);
                    remoteCommunicator.ExecuteCommand(EricssonCommands.MML_Command, "<", out response);
                    commandResults.Add(new CommandResult() { Command = EricssonCommands.MML_Command, Output = new List<string>() { response } });
                    Thread.Sleep(1000);
                }
                else if (!IsCommandSucceed(response))
                {
                    throw new Exception(string.Format("{0} Not Executed", aNxARCommand));
                }
            }
            else if (!IsCommandSucceed(response))
            {
                throw new Exception(string.Format("{0} Not Executed", aNxARCommand));
            }

            foreach (var ericssonRouteWithCommandsKvp in routesWithCommandsByOBA)
            {
                var customerOBA = ericssonRouteWithCommandsKvp.Key;
                var ericssonRoutesWithCommands = ericssonRouteWithCommandsKvp.Value;
                if (failedCustomerMappingOBAs.Contains(customerOBA))
                {
                    var allFailedRoutesWithCommands = allFailedRoutesWithCommandsByOBA.GetOrCreateItem(customerOBA);
                    allFailedRoutesWithCommands.AddRange(ericssonRoutesWithCommands);
                    continue;
                }

                if (customerMappingsToDeleteFailed != null && customerMappingsToDeleteFailed.Any(item =>
                {
                    return (item.CustomerMapping.NationalOBA.HasValue && item.CustomerMapping.NationalOBA.Value == customerOBA) ||
                     (item.CustomerMapping.InternationalOBA.HasValue && item.CustomerMapping.InternationalOBA.Value == customerOBA);
                }))
                {
                    var allFailedRoutesWithCommands = allFailedRoutesWithCommandsByOBA.GetOrCreateItem(customerOBA);
                    allFailedRoutesWithCommands.AddRange(ericssonRoutesWithCommands);
                    var failedEricssonRoutesWithCommands = failedRoutesWithCommandsByOBA.GetOrCreateItem(customerOBA);
                    failedEricssonRoutesWithCommands.AddRange(ericssonRoutesWithCommands);
                    continue;
                }

                foreach (var ericssonRouteWithCommands in ericssonRoutesWithCommands)
                {
                    if (ericssonRouteWithCommands.RouteCompareResult != null && faildRouteCaseNumbers.Contains(ericssonRouteWithCommands.RouteCompareResult.Route.RCNumber))
                    {
                        var allFailedRoutesWithCommands = allFailedRoutesWithCommandsByOBA.GetOrCreateItem(customerOBA);
                        allFailedRoutesWithCommands.Add(ericssonRouteWithCommands);
                        continue;
                    }

                    isSuccessfull = false;
                    numberOfTriesDone = 0;
                    var commandsSucceed = true;
                    while (!isSuccessfull && numberOfTriesDone < maxNumberOfRetries)
                    {
                        try
                        {
                            foreach (var command in ericssonRouteWithCommands.Commands)
                            {
                                remoteCommunicator.ExecuteCommand(command, CommandPrompt, out response);
                                commandResults.Add(new CommandResult() { Command = command, Output = new List<string>() { response } });
                                while (response.ToUpper().Contains(EricssonCommands.FUNCTION_BUSY))
                                {
                                    Thread.Sleep(3000);
                                    remoteCommunicator.ExecuteCommand(command, CommandPrompt, out response);
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
                                var succeededEricssonRoutesWithCommands = succeededRoutesWithCommandsByOBA.GetOrCreateItem(customerOBA);
                                succeededEricssonRoutesWithCommands.Add(ericssonRouteWithCommands);
                                if (ericssonRouteWithCommands.ActionType == RouteActionType.DeleteCustomer)
                                {
                                    CustomerMappingWithActionType customersToDeleteSucceed;
                                    if (customersToDeleteByOBA.TryGetValue(customerOBA, out customersToDeleteSucceed))
                                        customerMappingsToDeleteSucceed.Add(customersToDeleteSucceed);
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                        catch (Exception ex)
                        {
                            numberOfTriesDone++;
                            isSuccessfull = false;
                        }
                    }
                    if (!isSuccessfull)
                    {
                        var allFailedEricssonRoutesWithCommands = allFailedRoutesWithCommandsByOBA.GetOrCreateItem(customerOBA);
                        var failedEricssonRoutesWithCommands = failedRoutesWithCommandsByOBA.GetOrCreateItem(customerOBA);
                        failedEricssonRoutesWithCommands.Add(ericssonRouteWithCommands);
                        allFailedEricssonRoutesWithCommands.Add(ericssonRouteWithCommands);
                        if (ericssonRouteWithCommands.ActionType == RouteActionType.DeleteCustomer)
                        {
                            CustomerMappingWithActionType customersToDelete;
                            if (customersToDeleteByOBA.TryGetValue(customerOBA, out customersToDelete))
                                customerMappingsToDeleteFailed.Add(customersToDelete);

                            var customerMappingIndex = customerMappingsToDeleteSucceed.FindIndex(item =>
                            {
                                return (item.CustomerMapping.NationalOBA.HasValue && item.CustomerMapping.NationalOBA.Value == customerOBA) ||
                                 (item.CustomerMapping.InternationalOBA.HasValue && item.CustomerMapping.InternationalOBA.Value == customerOBA);
                            });
                            if (customerMappingIndex >= 0)
                                customerMappingsToDeleteSucceed.RemoveAt(customerMappingIndex);
                        }
                    }
                }
            }

            remoteCommunicator.ExecuteCommand(aNxAICommand, CommandPrompt, out response);
            commandResults.Add(new CommandResult() { Command = aNxAICommand, Output = new List<string>() { response } });
            remoteCommunicator.ExecuteCommand(";", CommandPrompt, out response);
            commandResults.Add(new CommandResult() { Command = ";", Output = new List<string>() { response } });

            Thread.Sleep(3000);
            remoteCommunicator.ExecuteCommand(EricssonCommands.Exit_Command, ">", out response);
            commandResults.Add(new CommandResult() { Command = EricssonCommands.Exit_Command, Output = new List<string>() { response } });
            remoteCommunicator.ExecuteCommand(EricssonCommands.Exit_Command);
            commandResults.Add(new CommandResult() { Command = EricssonCommands.Exit_Command });
        }

        private bool IsCommandSucceed(string response)
        {
            string responseToUpper = response.ToUpper();
            if (string.IsNullOrEmpty(response) || response.Equals(";") || responseToUpper.Contains(EricssonCommands.EXECUTED) && !responseToUpper.Contains("NOT") || responseToUpper.Contains(EricssonCommands.ORDERED))
                return true;

            return false;
        }
        private bool IsCommandFailed(string response, IEnumerable<string> faultCodes)
        {
            string responseToUpper = response.ToUpper();

            if (string.IsNullOrEmpty(response))
                return false;

            if (responseToUpper.Contains(EricssonCommands.PROTECTION_PERIOD_ELAPSED) || responseToUpper.Contains(EricssonCommands.PROTECTIVE_PERIOD_ELAPSED) || faultCodes.Any(item => responseToUpper.Contains(item)))
                return true;

            return false;
        }

        private List<ConvertedRoute> ExpandConvertedRoutes(List<ConvertedRoute> routesAfterCompression, out Dictionary<int, Dictionary<string, int>> routeCaseByCodeGroupByOBA)
        {
            routeCaseByCodeGroupByOBA = null;

            if (routesAfterCompression == null || routesAfterCompression.Count == 0)
                return null;

            routeCaseByCodeGroupByOBA = new Dictionary<int, Dictionary<string, int>>();
            CodeGroupManager codeGroupManager = new CodeGroupManager();

            List<ConvertedRoute> finalConvertedRoutes = new List<ConvertedRoute>();

            EricssonConvertedRouteByCodeByPrefixLengthByFirstPrefixByOBA structuredRoutes = new EricssonConvertedRouteByCodeByPrefixLengthByFirstPrefixByOBA();
            Func<EricssonConvertedRouteByCodeByPrefixLengthByFirstPrefix> createConvertedRoutesByOBA = () =>
            {
                return new EricssonConvertedRouteByCodeByPrefixLengthByFirstPrefix()
                    {
                        { '1', new EricssonConvertedRouteByCodeByPrefixLength() },
                        { '2', new EricssonConvertedRouteByCodeByPrefixLength() },
                        { '3', new EricssonConvertedRouteByCodeByPrefixLength() },
                        { '4', new EricssonConvertedRouteByCodeByPrefixLength() },
                        { '5', new EricssonConvertedRouteByCodeByPrefixLength() },
                        { '6', new EricssonConvertedRouteByCodeByPrefixLength() },
                        { '7', new EricssonConvertedRouteByCodeByPrefixLength() },
                        { '8', new EricssonConvertedRouteByCodeByPrefixLength() },
                        { '9', new EricssonConvertedRouteByCodeByPrefixLength() }
                    };
            };

            foreach (ConvertedRoute route in routesAfterCompression)
            {
                EricssonConvertedRoute convertedRoute = route as EricssonConvertedRoute;
                EricssonConvertedRouteByCodeByPrefixLengthByFirstPrefix convertedRoutesByOBA = structuredRoutes.GetOrCreateItem(convertedRoute.OBA, createConvertedRoutesByOBA);
                EricssonConvertedRouteByCodeByPrefixLength convertedRoutesByPrefixLength = convertedRoutesByOBA.GetRecord(convertedRoute.Code[0]);
                EricssonConvertedRouteByCode convertedRoutesByCode = convertedRoutesByPrefixLength.GetOrCreateItem(convertedRoute.Code.Length);
                convertedRoutesByCode.Add(convertedRoute.Code, convertedRoute);

                Dictionary<string, int> routeCaseByCode = routeCaseByCodeGroupByOBA.GetOrCreateItem(convertedRoute.OBA);

                var codeGroupObject = codeGroupManager.GetMatchCodeGroup(convertedRoute.Code);
                if (string.Compare(codeGroupObject.Code, convertedRoute.Code) == 0)
                    routeCaseByCode.Add(codeGroupObject.Code, convertedRoute.RCNumber);
            }

            foreach (var structuredRoutesByOBAKvp in structuredRoutes)
            {
                int oba = structuredRoutesByOBAKvp.Key;
                EricssonConvertedRouteByCodeByPrefixLengthByFirstPrefix structuredRoutesByFirstPrefix = structuredRoutesByOBAKvp.Value;

                if (structuredRoutesByFirstPrefix.Count == 0)
                    continue;

                foreach (var structuredRoutesByPrefixLentghKvp in structuredRoutesByFirstPrefix)
                {
                    EricssonConvertedRouteByCodeByPrefixLength structuredRoutesByPrefixLength = structuredRoutesByPrefixLentghKvp.Value;

                    if (structuredRoutesByPrefixLength.Count == 0)
                        continue;

                    var keys = structuredRoutesByPrefixLength.Keys;
                    int minCodeLength = keys.MinBy(itm => itm);
                    int maxCodeLength = keys.MaxBy(itm => itm);

                    for (int currentLength = minCodeLength; currentLength <= maxCodeLength; currentLength++)
                    {
                        EricssonConvertedRouteByCode structuredRoutesByCode = structuredRoutesByPrefixLength.GetRecord(currentLength);

                        if (structuredRoutesByCode == null)
                            continue;

                        if (currentLength == maxCodeLength)
                        {
                            finalConvertedRoutes.AddRange(structuredRoutesByCode.Values);
                            continue;
                        }

                        foreach (var structuredRouteKvp in structuredRoutesByCode)
                        {
                            string code = structuredRouteKvp.Key;
                            EricssonConvertedRoute convertedRoute = structuredRouteKvp.Value;

                            bool hasChildCodes = false;

                            for (int codeLength = currentLength + 1; codeLength <= maxCodeLength; codeLength++)
                            {
                                EricssonConvertedRouteByCode childRoutesByCodeLength = structuredRoutesByPrefixLength.GetRecord(codeLength);
                                hasChildCodes = childRoutesByCodeLength != null && childRoutesByCodeLength.Values.Any(itm => itm.Code.StartsWith(code));

                                if (hasChildCodes)
                                    break;
                            }

                            if (!hasChildCodes)
                            {
                                finalConvertedRoutes.Add(convertedRoute);
                            }
                            else
                            {
                                List<string> newCodes = new List<string>() { $"{code}{0}", $"{code}{1}", $"{code}{2}", $"{code}{3}", $"{code}{4}", $"{code}{5}", $"{code}{6}", $"{code}{7}", $"{code}{8}", $"{code}{9}" };
                                EricssonConvertedRouteByCode directChildRoutesByCodeLength = structuredRoutesByPrefixLength.GetOrCreateItem(currentLength + 1);
                                foreach (string newCode in newCodes)
                                {
                                    if (directChildRoutesByCodeLength.ContainsKey(newCode))
                                        continue;

                                    var clonedConvertedRoute = Vanrise.Common.Utilities.CloneObject(convertedRoute);
                                    clonedConvertedRoute.Code = newCode;
                                    directChildRoutesByCodeLength.Add(newCode, clonedConvertedRoute);
                                }
                            }

                        }
                    }
                }
            }

            return finalConvertedRoutes.OrderBy(itm => (itm as EricssonConvertedRoute).OBA).ThenBy(itm => (itm as EricssonConvertedRoute).Code).ToList();
        }
        #endregion

        #region routeCases

        private Dictionary<string, RouteCase> InsertAndGetRouteCases(string switchId, HashSet<string> RouteCasesToAddAsString)
        {
            int maxLockRetryCount = Int32.MaxValue;
            TimeSpan lockRetryInterval = new TimeSpan(0, 0, 1);
            IRouteCaseDataManager dataManager = RouteSyncEricssonDataManagerFactory.GetDataManager<IRouteCaseDataManager>();
            dataManager.SwitchId = switchId;

            string transactionLockName = String.Concat("WhS_Ericsson_{0}.RouteCase", switchId);
            int retryCount = 0;
            List<RouteCase> routeCasesToAdd = new List<RouteCase>();

            Dictionary<string, RouteCase> routeCases = GetCachedRouteCasesGroupedByOptions(switchId);
            int lastCurrentRCNumber = 0;
            if (routeCases != null)
                lastCurrentRCNumber = routeCases.Select(itm => itm.Value.RCNumber).Max();

            while (retryCount < maxLockRetryCount)
            {
                if (TransactionLocker.Instance.TryLock(transactionLockName, () =>
                {
                    Dictionary<string, RouteCase> newRouteCasesByOptions = dataManager.GetRouteCasesAfterRCNumber(lastCurrentRCNumber);

                    int rcNumber = lastCurrentRCNumber;
                    if (newRouteCasesByOptions != null && newRouteCasesByOptions.Count > 0)
                    {
                        foreach (var newRouteCaseByOptionsKVP in newRouteCasesByOptions)
                        {
                            rcNumber = Math.Max(rcNumber, newRouteCaseByOptionsKVP.Value.RCNumber);
                            routeCases.Add(newRouteCaseByOptionsKVP.Key, newRouteCaseByOptionsKVP.Value);
                        }
                    }
                    rcNumber++;

                    Object dbApplyStream = dataManager.InitialiazeStreamForDBApply();
                    var appliedRouteCases = new Dictionary<string, RouteCase>();
                    foreach (string RouteCaseToAddAsString in RouteCasesToAddAsString)
                    {
                        if (newRouteCasesByOptions == null || !newRouteCasesByOptions.ContainsKey(RouteCaseToAddAsString))
                        {
                            RouteCase routeCaseToAdd = new RouteCase() { RCNumber = rcNumber, RouteCaseAsString = RouteCaseToAddAsString };
                            appliedRouteCases.Add(RouteCaseToAddAsString, routeCaseToAdd);
                            dataManager.WriteRecordToStream(routeCaseToAdd, dbApplyStream);
                            rcNumber++;
                        }
                    }
                    object obj = dataManager.FinishDBApplyStream(dbApplyStream);
                    dataManager.ApplyRouteCaseForDB(obj);

                    if (appliedRouteCases != null && appliedRouteCases.Count > 0)
                    {
                        foreach (var appliedRouteCaseKvp in appliedRouteCases)
                            routeCases.Add(appliedRouteCaseKvp.Key, appliedRouteCaseKvp.Value);
                    }
                }))
                {
                    return routeCases;
                }
                else
                {
                    Thread.Sleep(lockRetryInterval);
                    retryCount++;
                }
            }
            throw new Exception(String.Format("Cannot Lock WhS_Ericsson_{0}.RouteCase", switchId));
        }

        private struct GetCachedRouteCasesGroupedByOptionsCacheName
        {
            public string SwitchId { get; set; }
        }

        private Dictionary<string, RouteCase> GetCachedRouteCasesGroupedByOptions(string switchId)
        {
            var cacheManager = Vanrise.Caching.CacheManagerFactory.GetCacheManager<RouteCaseCacheManager>();
            var cacheName = new GetCachedRouteCasesGroupedByOptionsCacheName() { SwitchId = switchId };

            return cacheManager.GetOrCreateObject(cacheName, RouteCaseCacheExpirationChecker.Instance, () =>
            {
                Dictionary<string, RouteCase> result = new Dictionary<string, RouteCase>();
                IRouteCaseDataManager dataManager = RouteSyncEricssonDataManagerFactory.GetDataManager<IRouteCaseDataManager>();
                dataManager.SwitchId = switchId;
                IEnumerable<RouteCase> routeCases = dataManager.GetAllRouteCases();
                if (routeCases != null)
                {
                    foreach (RouteCase routeCase in routeCases)
                        result.Add(routeCase.RouteCaseAsString, routeCase);
                }
                return result;
            });
        }

        #region Private Classes

        private class RouteCaseCacheManager : BaseCacheManager
        {

        }

        private class RouteCaseCacheExpirationChecker : CacheExpirationChecker
        {
            static RouteCaseCacheExpirationChecker s_instance = new RouteCaseCacheExpirationChecker();
            public static RouteCaseCacheExpirationChecker Instance { get { return s_instance; } }

            public override bool IsCacheExpired(Vanrise.Caching.ICacheExpirationCheckerContext context)
            {
                TimeSpan entitiesTimeSpan = TimeSpan.FromMinutes(15);
                SlidingWindowCacheExpirationChecker slidingWindowCacheExpirationChecker = new SlidingWindowCacheExpirationChecker(entitiesTimeSpan);
                return slidingWindowCacheExpirationChecker.IsCacheExpired(context);
            }
        }

        #endregion

        #endregion

        #region Converted Routes

        private void AddEricssonConvertedRouteToConvertedRoutes(List<EricssonConvertedRoute> convertedRoutes, Dictionary<string, List<EricssonConvertedRoute>> routesToConvertByRCString, Dictionary<string, RouteCase> routeCases, HashSet<string> routeCasesToAdd, string routeCaseAsString, EricssonConvertedRoute ericssonConvertedRoute)
        {
            RouteCase routeCase;
            if (routeCases.TryGetValue(routeCaseAsString, out routeCase))
            {
                ericssonConvertedRoute.RCNumber = routeCase.RCNumber;
                convertedRoutes.Add(ericssonConvertedRoute);
            }
            else
            {
                List<EricssonConvertedRoute> routesToConvert = routesToConvertByRCString.GetOrCreateItem(routeCaseAsString);
                routesToConvert.Add(ericssonConvertedRoute);
                routeCasesToAdd.Add(routeCaseAsString);
            }
        }

        #endregion

        #endregion

        #region Private Classes
        private class EricssonConvertedRouteByCode : Dictionary<string, EricssonConvertedRoute> { }
        private class EricssonConvertedRouteByCodeByPrefixLength : Dictionary<int, EricssonConvertedRouteByCode> { }
        private class EricssonConvertedRouteByCodeByPrefixLengthByFirstPrefix : Dictionary<int, EricssonConvertedRouteByCodeByPrefixLength> { }
        private class EricssonConvertedRouteByCodeByPrefixLengthByFirstPrefixByOBA : Dictionary<int, EricssonConvertedRouteByCodeByPrefixLengthByFirstPrefix> { }
        public class RouteCaseOptionWithSupplier : IPercentageItem
        {
            public string SupplierId { get; set; }
            public int Percentage { get; set; }
            public List<RouteCaseOption> RouteCaseOptions { get; set; }

            public decimal? GetInputPercentage()
            {
                return Percentage;
            }

            public void SetOutputPercentage(int? percentage)
            {
                Percentage = percentage.Value;
                RouteCaseOptions = RouteCaseOptions.Select(item => { item.Percentage = percentage; return item; }).ToList();
            }

            public bool ShouldHavePercentage()
            {
                return true;
            }
        }
        public class EricssonRoutesWithCommands
        {
            public Dictionary<int, List<EricssonRouteWithCommands>> BNumberEricssonRouteWithCommands { get; set; }
            public Dictionary<int, List<EricssonRouteWithCommands>> ANumberEricssonRouteWithCommands { get; set; }
        }
        public class ReservedBTableRange
        {
            public int From { get; set; }
            public int To { get; set; }

            public bool IsBTableIncluded(int bTble)
            {
                if (bTble >= From && bTble <= To)
                    return true;
                return false;
            }
        }
        #endregion

    }
}
