using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.Rules;
using Vanrise.GenericData.Entities;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.RouteSync.Entities;
using TOne.WhS.RouteSync.Ericsson.Data;
using TOne.WhS.RouteSync.Ericsson.Entities;
using TOne.WhS.RouteSync.Ericsson.Business;

namespace TOne.WhS.RouteSync.Ericsson
{
	public partial class EricssonSWSync : SwitchRouteSynchronizer
	{
		public override Guid ConfigId { get { return new Guid("94739CBC-00A7-4CEB-9285-B4CB35D7D003"); } }

		const string CommandPrompt = "<";
		public int FirstRCNumber { get; set; }
		public int NumberOfOptions { get; set; }
		public int MinCodeLength { get; set; }
		public int MaxCodeLength { get; set; }
		public string LocalCountryCode { get; set; }
		public int LocalNumberLength { get; set; }
		public string InterconnectGeneralPrefix { get; set; }
		public List<OutgoingTrafficCustomer> OutgoingTrafficCustomers { get; set; }
		public List<IncomingTrafficSupplier> IncomingTrafficSuppliers { get; set; }
		public List<LocalSupplierMapping> LocalSupplierMappings { get; set; }
		public Dictionary<string, CarrierMapping> CarrierMappings { get; set; }
		public List<ManualOverrides> ManualOverrides { get; set; }
		public List<InterconnectOverrides> InterconnectOverrides { get; set; }
		public List<EricssonSSHCommunication> SwitchCommunicationList { get; set; }
		public List<SwitchLogger> SwitchLoggerList { get; set; }

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
			routeCaseDataManager.Initialize(new RouteCaseInitializeContext() { FirstRCNumber = FirstRCNumber });

			CustomerMappingManager customerMappingManager = new CustomerMappingManager();
			customerMappingManager.Initialize(context.SwitchId, CarrierMappings.Values);
		}

		public override void ConvertRoutes(ISwitchRouteSynchronizerConvertRoutesContext context)
		{
			if (context.Routes == null || context.Routes.Count == 0 || CarrierMappings == null || CarrierMappings.Count == 0)
				return;

			var convertedRoutes = new List<ConvertedRoute>();
			var routesToConvertByRCString = new Dictionary<string, List<EricssonConvertedRoute>>();
			var routeCases = new RouteCaseManager().GetCachedRouteCasesGroupedByOptions(context.SwitchId);
			var routeCasesToAdd = new HashSet<string>();
			CodeGroupManager codeGroupManager = new CodeGroupManager();
			var ruleTree = new EricssonSWSync().BuildSupplierTrunkGroupTree(CarrierMappings);

			foreach (var route in context.Routes)
			{
				var customerCarrierMapping = CarrierMappings.GetRecord(route.CustomerId);
				if (customerCarrierMapping == null)
					continue;

				var customerMapping = customerCarrierMapping.CustomerMapping;
				if (customerMapping == null)
					continue;

				if (string.IsNullOrEmpty(customerMapping.BO))
					continue;

				var codeGroupObject = codeGroupManager.GetMatchCodeGroup(route.Code);
				codeGroupObject.ThrowIfNull(string.Format("No Code Group found for code '{0}'.", route.Code));

				int routeCodeGroupId = codeGroupObject.CodeGroupId;
				string routeCodeGroup = codeGroupObject.Code;

				EricssonConvertedRoute ericssonConvertedRoute = new EricssonConvertedRoute() { BO = customerMapping.BO, Code = route.Code };

				List<RouteCaseOption> routeCaseOptions = GetRouteCaseOptions(route, routeCodeGroupId, routeCodeGroup, ruleTree);
				var routeCaseOptionsAsString = Helper.SerializeRouteCaseOptions(routeCaseOptions);

				RouteCase routeCase;
				if (routeCases.TryGetValue(routeCaseOptionsAsString, out routeCase))
				{
					ericssonConvertedRoute.RCNumber = routeCase.RCNumber;
					convertedRoutes.Add(ericssonConvertedRoute);
				}
				else
				{
					List<EricssonConvertedRoute> routesToConvert = routesToConvertByRCString.GetOrCreateItem(routeCaseOptionsAsString);
					routesToConvert.Add(ericssonConvertedRoute);
					routeCasesToAdd.Add(routeCaseOptionsAsString);
				}
			}

			if (routeCasesToAdd.Count > 0)
				routeCases = new RouteCaseManager().InsertAndGetRouteCases(context.SwitchId, routeCasesToAdd);

			routeCases.ThrowIfNull("routeCases");

			foreach (var routesToConvertKvp in routesToConvertByRCString)
			{
				RouteCase routeCase = routeCases.GetRecord(routesToConvertKvp.Key);
				routeCase.ThrowIfNull("routeCase");

				var routes = routesToConvertKvp.Value.Select(item => { item.RCNumber = routeCase.RCNumber; return item; });
				convertedRoutes.AddRange(routes);
			}
			context.ConvertedRoutes = convertedRoutes;
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
			#endregion

			#region Get Commands
			var routeCasesDictionary = new RouteCaseManager().GetCachedRouteCasesGroupedByOptions(context.SwitchId);
			var routeCasesToBeAdded = (routeCasesDictionary != null && routeCasesDictionary.Count > 0) ? routeCasesDictionary.FindAllRecords(item => !item.Synced) : null;
			var routeCasesToBeAddedWithCommands = GetRouteCasesWithCommands(routeCasesToBeAdded);

			Dictionary<string, CustomerMappingWithActionType> customersToDeleteByBO;
			Dictionary<string, List<EricssonRouteWithCommands>> ericssonRoutesToDeleteWithCommandsByBo;
			Dictionary<string, EricssonConvertedRouteDifferences> routeDifferencesByBO;
			var customerMappingsWithCommands = GetCustomerMappingsWithCommands(context.SwitchId, out ericssonRoutesToDeleteWithCommandsByBo, out customersToDeleteByBO);

			var ericssonRoutesWithCommandsByBo = GetEricssonRoutesWithCommands(context.SwitchId, customersToDeleteByBO, ericssonRoutesToDeleteWithCommandsByBo, out routeDifferencesByBO);
			#endregion

			EricssonSSHCommunication ericssonSSHCommunication = null;
			if (SwitchCommunicationList != null)
			{
				ericssonSSHCommunication = SwitchCommunicationList.FirstOrDefault(itm => itm.IsActive);
			}
			SSHCommunicator sshCommunicator = null;
			if (ericssonSSHCommunication != null)
				sshCommunicator = new SSHCommunicator(ericssonSSHCommunication.SSHCommunicatorSettings);

			DateTime finalizeTime = DateTime.Now;

			List<CommandResult> commandResults = new List<CommandResult>();

			List<string> faultCodes = null;
			int maxNumberOfRetries = 0;
			if (sshCommunicator != null)
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

			var executeStatus = ExecuteCustomerMappingsCommands(customerMappingsWithCommands, ericssonSSHCommunication, sshCommunicator, commandResults, out succeedCustomerMappingsWithCommands, out failedCustomerMappingsWithCommands, out succeedCustomerMappingsWithFailedTrunk, faultCodes, maxNumberOfRetries);
			LogCustomerMappingCommands(succeedCustomerMappingsWithCommands, failedCustomerMappingsWithCommands, ftpLogger, finalizeTime);

			if (succeedCustomerMappingsWithCommands != null && succeedCustomerMappingsWithCommands.Count > 0)
				customerMappingSucceededDataManager.SaveCustomerMappingsSucceededToDB(succeedCustomerMappingsWithCommands.Select(item => item.CustomerMappingWithActionType));

			List<CustomerMappingWithCommands> failedCustomerMappingToUpdate = new List<CustomerMappingWithCommands>();
			if (failedCustomerMappingsWithCommands != null && failedCustomerMappingsWithCommands.Count > 0)
			{
				var failedAdded = failedCustomerMappingsWithCommands.FindAllRecords(item => item.CustomerMappingWithActionType.ActionType == CustomerMappingActionType.Add);
				var failedUpdated = failedCustomerMappingsWithCommands.FindAllRecords(item => item.CustomerMappingWithActionType.ActionType == CustomerMappingActionType.Update);

				if (failedAdded != null && failedAdded.Any())
					customerMappingDataManager.RemoveCutomerMappingsFromTempTable(failedAdded.Select(item => item.CustomerMappingWithActionType.CustomerMapping.BO));

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

			ExecuteRouteCasesCommands(routeCasesToBeAddedWithCommands, ericssonSSHCommunication, sshCommunicator, commandResults, out succeedRouteCasesWithCommands, out failedRouteCasesWithCommands, routeCaseDataManager, maxNumberOfRetries);
			LogRouteCaseCommands(succeedRouteCasesWithCommands, failedRouteCasesWithCommands, ftpLogger, finalizeTime);
			#endregion

			#region execute and log Routes

			IEnumerable<int> failedRouteCaseNumbers = (failedRouteCasesWithCommands == null) ? null : failedRouteCasesWithCommands.Select(item => item.RouteCase.RCNumber);
			IEnumerable<string> failedCustomerMappingBOs = (failedCustomerMappingsWithCommands == null) ? null : failedCustomerMappingsWithCommands.Select(item => item.CustomerMappingWithActionType.CustomerMapping.BO);

			Dictionary<string, List<EricssonRouteWithCommands>> succeedEricssonRoutesWithCommandsByBo;
			Dictionary<string, List<EricssonRouteWithCommands>> failedEricssonRoutesWithCommandsByBo;
			Dictionary<string, List<EricssonRouteWithCommands>> allFailedEricssonRoutesWithCommandsByBo;
			List<CustomerMappingWithActionType> customerMappingsToDeleteSucceed;
			List<CustomerMappingWithActionType> customerMappingsToDeleteFailed;

			ExecuteRoutesCommands(ericssonRoutesWithCommandsByBo, ericssonSSHCommunication, sshCommunicator, customersToDeleteByBO, commandResults,
			out succeedEricssonRoutesWithCommandsByBo, out failedEricssonRoutesWithCommandsByBo, out allFailedEricssonRoutesWithCommandsByBo, out customerMappingsToDeleteSucceed, out customerMappingsToDeleteFailed
			, failedCustomerMappingBOs, failedRouteCaseNumbers, faultCodes, maxNumberOfRetries);

			LogEricssonRouteCommands(succeedEricssonRoutesWithCommandsByBo, failedEricssonRoutesWithCommandsByBo, ftpLogger, finalizeTime);

			#region Update the deleted customer
			if (customerMappingsToDeleteSucceed != null && customerMappingsToDeleteSucceed.Count > 0)
				customerMappingSucceededDataManager.SaveCustomerMappingsSucceededToDB(customerMappingsToDeleteSucceed);

			if (customerMappingsToDeleteFailed != null && customerMappingsToDeleteFailed.Count > 0)
			{
				customerMappingDataManager.InsertCutomerMappingsToTempTable(customerMappingsToDeleteFailed.Select(item => item.CustomerMapping));
				routeDataManager.CopyCustomerRoutesToTempTable(customerMappingsToDeleteFailed.Select(item => item.CustomerMapping.BO));
			}
			customerMappingDataManager.Finalize(new CustomerMappingFinalizeContext());
			#endregion

			if (succeedEricssonRoutesWithCommandsByBo != null && succeedEricssonRoutesWithCommandsByBo.Count > 0)// save succeeded routes to succeeded table
				routeSucceededDataManager.SaveRoutesSucceededToDB(succeedEricssonRoutesWithCommandsByBo);

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

		public override bool IsSwitchRouteSynchronizerValid(IIsSwitchRouteSynchronizerValidContext context)
		{
			if (this.CarrierMappings == null || this.CarrierMappings.Count == 0)
				return true;
			return true;

			//Dictionary<string, List<int>> carrierAccountIdsByTrunkName = new Dictionary<string, List<int>>();

			//foreach (var mapping in this.CarrierMappings.Values)
			//{
			//	if (mapping.CustomerMapping != null && mapping.CustomerMapping.InTrunks != null)
			//	{
			//		foreach (var inTrunk in mapping.CustomerMapping.InTrunks)
			//		{
			//			List<int> carrierAccountIds = carrierAccountIdsByTrunkName.GetOrCreateItem(inTrunk.TrunkName);
			//			carrierAccountIds.Add(mapping.CarrierId);
			//		}
			//	}
			//}

			//Dictionary<string, List<int>> duplicatedInTrunks = carrierAccountIdsByTrunkName.Where(itm => itm.Value.Count > 1).ToDictionary(itm => itm.Key, itm => itm.Value);
			//if (duplicatedInTrunks.Count == 0)
			//	return true;

			//List<string> validationMessages = new List<string>();

			//if (duplicatedInTrunks.Count > 0)
			//{
			//	CarrierAccountManager carrierAccountManager = new CarrierAccountManager();

			//	foreach (var kvp in duplicatedInTrunks)
			//	{
			//		string trunkName = kvp.Key;
			//		List<int> customerIds = kvp.Value;

			//		List<string> carrierAccountNames = new List<string>();

			//		foreach (var customerId in customerIds)
			//		{
			//			string customerName = carrierAccountManager.GetCarrierAccountName(customerId);
			//			carrierAccountNames.Add(string.Format("'{0}'", customerName));
			//		}

			//		validationMessages.Add(string.Format("Trunk Name: '{0}' is duplicated at Carrier Accounts: {1}", trunkName, string.Join(", ", carrierAccountNames)));
			//	}
			//}

			//context.ValidationMessages = validationMessages;
			//return false;
		}

		#endregion

		#region Private Methods

		#region Get Route Case Options
		private List<RouteCaseOption> GetRouteCaseOptions(Route route, int codeGroupId, string routeCodeGroup, RuleTree ruleTree)
		{
			List<RouteCaseOption> routeCaseOptions = new List<RouteCaseOption>();
			int numberOfOptions = 0;
			int groupId = 0;

			if (route.Options != null)
			{
				foreach (var option in route.Options)
				{
					if (option.IsBlocked)
						continue;

					var supplierMapping = GetSupplierMapping(option.SupplierId);
					if (supplierMapping == null)
						continue;

					var trunkGroup = GetTrunkGroup(ruleTree, option.SupplierId, codeGroupId, route.CustomerId, false);
					if (trunkGroup == null)
						continue;

					groupId++;
					if (trunkGroup.TrunkTrunkGroups != null && trunkGroup.TrunkTrunkGroups.Count > 0)
					{
						foreach (var trunkGroupTrunk in trunkGroup.TrunkTrunkGroups.OrderBy(itm => itm.Priority))
						{
							routeCaseOptions.Add(GetRouteCaseOption(routeCodeGroup, option.SupplierId, option.Percentage, supplierMapping, trunkGroup, trunkGroupTrunk, groupId));
							numberOfOptions++;
							if (numberOfOptions == NumberOfOptions)
								return routeCaseOptions;
						}
					}

					#region option backups
					if (option.Backups != null)
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
								foreach (var trunkGroupTrunk in backupTrunkGroup.TrunkTrunkGroups.OrderBy(itm => itm.Priority))
								{
									routeCaseOptions.Add(GetRouteCaseOption(routeCodeGroup, backupOption.SupplierId, null, supplierMapping, backupTrunkGroup, trunkGroupTrunk, groupId));
									numberOfOptions++;
									if (numberOfOptions == NumberOfOptions)
										return routeCaseOptions;
								}
							}
						}
					}
					#endregion
				}
			}
			return routeCaseOptions;
		}

		private RouteCaseOption GetRouteCaseOption(string routeCodeGroup, string supplierId, int? percentage, SupplierMapping supplierMapping, TrunkGroup trunkGroup, TrunkTrunkGroup trunkGroupTrunk, int groupId)
		{
			var trunk = supplierMapping.OutTrunks.FindRecord(item => item.TrunkId == trunkGroupTrunk.TrunkId);
			RouteCaseOption routeCaseOption = new RouteCaseOption();
			routeCaseOption.Percentage = percentage;
			routeCaseOption.Priority = trunkGroupTrunk.Priority;
			routeCaseOption.OutTrunk = trunk.TrunkName;
			routeCaseOption.Type = trunk.TrunkType;
			routeCaseOption.TrunkPercentage = trunkGroupTrunk.Percentage;
			routeCaseOption.IsBackup = trunkGroup.IsBackup;
			routeCaseOption.GroupID = groupId;
			routeCaseOption.BNT = 1;
			routeCaseOption.SP = 1;

			if (string.Compare(routeCodeGroup, LocalCountryCode) == 0)
			{
				if (trunk.TrunkName.StartsWith(InterconnectGeneralPrefix) || IncomingTrafficSuppliers.Any(item => item.SupplierId.ToString() == supplierId))
				{
					routeCaseOption.BNT = 4;
					routeCaseOption.SP = Convert.ToInt16(LocalCountryCode.Length + 1);
				}
			}
			return routeCaseOption;
		}

		private SupplierMapping GetSupplierMapping(string supplierId)
		{
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
		private List<CustomerMappingWithCommands> GetCustomerMappingsWithCommands(string switchId, out Dictionary<string, List<EricssonRouteWithCommands>> routesToDeleteCommandsByBo,
		out Dictionary<string, CustomerMappingWithActionType> customersToDeleteByBO)
		{
			customersToDeleteByBO = new Dictionary<string, CustomerMappingWithActionType>();
			routesToDeleteCommandsByBo = new Dictionary<string, List<EricssonRouteWithCommands>>();

			ICustomerMappingDataManager customerMappingDataManager = RouteSyncEricssonDataManagerFactory.GetDataManager<ICustomerMappingDataManager>();
			customerMappingDataManager.SwitchId = switchId;

			CustomerMappingTablesContext customerMappingTablesContext = new CustomerMappingTablesContext();
			customerMappingDataManager.CompareTables(customerMappingTablesContext);

			List<CustomerMappingWithCommands> customerMappingsWithCommands = new List<CustomerMappingWithCommands>();

			if (customerMappingTablesContext.CustomerMappingsToAdd != null && customerMappingTablesContext.CustomerMappingsToAdd.Count > 0)
			{
				foreach (var customerMappingSerializedToAdd in customerMappingTablesContext.CustomerMappingsToAdd)
				{
					var customerMappingToAdd = Helper.DeserializeCustomerMapping(customerMappingSerializedToAdd.CustomerMappingAsString);

					if (customerMappingToAdd != null)
					{
						var customerMappingToAddOBACommands = GetCustomerMappingOBACommands(customerMappingToAdd);
						//var customerMappingToAddTrunksCommands = GetCustomerMappingTrunksCommands(customerMappingToAdd);

						customerMappingsWithCommands.Add(new CustomerMappingWithCommands()
						{
							CustomerMappingWithActionType = new CustomerMappingWithActionType() { CustomerMapping = customerMappingToAdd, ActionType = CustomerMappingActionType.Add },
							OBACommands = customerMappingToAddOBACommands,
							//TrunkCommandsByTrunkId = customerMappingToAddTrunksCommands
						});
					}
				}
			}

			if (customerMappingTablesContext.CustomerMappingsToUpdate != null && customerMappingTablesContext.CustomerMappingsToUpdate.Count > 0)
			{
				foreach (var customerMappingSerializedToUpdate in customerMappingTablesContext.CustomerMappingsToUpdate)
				{
					var customerMappingToUpdate = Helper.DeserializeCustomerMapping(customerMappingSerializedToUpdate.CustomerMappingAsString);
					var customerMappingOldValue = Helper.DeserializeCustomerMapping(customerMappingSerializedToUpdate.CustomerMappingOldValueAsString);

					if (customerMappingToUpdate != null)
					{
						var customerMappingToUpdateOBACommands = GetCustomerMappingOBACommands(customerMappingToUpdate);
						//var customerMappingToUpdateTrunksCommands = GetCustomerMappingTrunksCommands(customerMappingToUpdate);

						customerMappingsWithCommands.Add(new CustomerMappingWithCommands()
						{
							OBACommands = customerMappingToUpdateOBACommands,
							//TrunkCommandsByTrunkId = customerMappingToUpdateTrunksCommands,
							CustomerMappingWithActionType = new CustomerMappingWithActionType() { CustomerMapping = customerMappingToUpdate, CustomerMappingOldValue = customerMappingOldValue, ActionType = CustomerMappingActionType.Update }
						});
					}
				}
			}

			if (customerMappingTablesContext.CustomerMappingsToDelete != null && customerMappingTablesContext.CustomerMappingsToDelete.Count > 0)
			{
				foreach (var customerMappingsSerializedToDelete in customerMappingTablesContext.CustomerMappingsToDelete)
				{
					var customerMappingsToDelete = Helper.DeserializeCustomerMapping(customerMappingsSerializedToDelete.CustomerMappingAsString);
					if (customerMappingsToDelete != null)
					{
						var customerMappingToDeleteCommands = GetCustomerMappingDeleteCommands(customerMappingsToDelete);

						List<EricssonRouteWithCommands> ericssonRoutesWithCommands = routesToDeleteCommandsByBo.GetOrCreateItem(customerMappingsToDelete.BO);
						ericssonRoutesWithCommands.Add(new EricssonRouteWithCommands() { Commands = customerMappingToDeleteCommands, ActionType = RouteActionType.DeleteCustomer });

						if (customersToDeleteByBO.ContainsKey(customerMappingsToDelete.BO))
							throw new DataIntegrityValidationException(string.Format("There is two deleted customer mapping with the same BO {0}", customerMappingsToDelete.BO));

						customersToDeleteByBO.Add(customerMappingsToDelete.BO, new CustomerMappingWithActionType() { CustomerMapping = customerMappingsToDelete, ActionType = CustomerMappingActionType.Delete });
					}
				}
			}

			return customerMappingsWithCommands;
		}
		private List<string> GetCustomerMappingOBACommands(CustomerMapping customerMappingToAdd)
		{
			List<string> customerMappingOBACommands = new List<string>();

			customerMappingOBACommands.Add(string.Format("{0}: BO={1}, NAPI=1, BNT=1, OBA={2};", EricssonCommands.PNBSI_Command, customerMappingToAdd.BO, customerMappingToAdd.InternationalOBA));
			customerMappingOBACommands.Add(string.Format("{0}: BO={1}, NAPI=1, BNT=4, OBA={2};", EricssonCommands.PNBSI_Command, customerMappingToAdd.BO, customerMappingToAdd.NationalOBA));
			return customerMappingOBACommands;
		}
		/*
		private Dictionary<Guid, string> GetCustomerMappingTrunksCommands(CustomerMapping customerMappingToAdd)
		{
			var customerMappingTrunksCommands = new Dictionary<Guid, string>();
			if (customerMappingToAdd.InTrunks != null && customerMappingToAdd.InTrunks.Count > 0)
			{
				foreach (var trunk in customerMappingToAdd.InTrunks)
				{
					if (customerMappingTrunksCommands.ContainsKey(trunk.TrunkId))
						throw new DataIntegrityValidationException(string.Format("There is two trunk with same Id {0}", trunk.TrunkId));

					customerMappingTrunksCommands.Add(trunk.TrunkId, string.Format("EXRBC:R={0}, BO:{1};", trunk.TrunkName, customerMappingToAdd.BO));
				}
			}
			return customerMappingTrunksCommands;
		}
		*/
		private List<string> GetCustomerMappingDeleteCommands(CustomerMapping customerMappingToDelete)
		{
			return new List<string>() { string.Format("{0}: B={1};", EricssonCommands.ANBSE_Command, customerMappingToDelete.BO) };
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
			routeCaseCommands.Add(string.Format("{0}: RC={1},CCH=NO;", EricssonCommands.ANRPI_Command, routeCase.RCNumber));
			var routeCaseOptions = Helper.DeserializeRouteCaseOptions(routeCase.RouteCaseOptionsAsString);
			if (routeCaseOptions != null || routeCaseOptions.Count > 0)
			{
				var optionGroups = routeCaseOptions.GroupBy(item => item.GroupID);
				int totalPercentage = 0;

				foreach (var optionGroupKvp in optionGroups)
				{
					var option = optionGroupKvp.First();
					if (option != null)
						totalPercentage += option.Percentage.HasValue ? option.Percentage.Value : 0; ;
				}

				if (routeCaseOptions.Any(item => item.IsBackup) || totalPercentage == 100)
				{
					int pValue = 1;
					foreach (var optionGroup in optionGroups)
					{
						int priority = 1;

						var orderedOptions = optionGroup.OrderBy(r => Convert.ToInt32(r.IsBackup));
						foreach (RouteCaseOption option in orderedOptions)
						{
							command = option.Percentage.HasValue ? string.Format("{0}: BR=CL-0&&-9&-11&&-15&TMR-0&-3&{1}, P0{2}={3}, R={4}, BNT={5}, SP=MM{6};", EricssonCommands.ANRSI_Command, option.Percentage, pValue, priority, option.OutTrunk, option.BNT, option.SP)
								: string.Format("{0}: BR=CL-0&&-9&-11&&-15&TMR-0&-3, P0{1}={2}, R={3}, BNT={4}, SP=MM{5};", EricssonCommands.ANRSI_Command, pValue, priority, option.OutTrunk, option.BNT, option.SP);
							routeCaseCommands.Add(command);
							priority++;
						}
						pValue++;
					}
				}
				else
				{
					foreach (var optionGroup in optionGroups)
					{
						foreach (RouteCaseOption option in optionGroup)
						{
							if (optionGroup.Key == 1)
							{
								command = option.Percentage.HasValue ? string.Format("{0}: BR=CL-0&&-9&-11&&-15&TMR-0&-3&{1}, P0{2}={3}, R={4}, BNT={5}, SP=MM{6};", EricssonCommands.ANRSI_Command, option.Percentage, optionGroup.Key, option.Priority, option.OutTrunk, option.BNT, option.SP)
									: string.Format("{0}: BR=CL-0&&-9&-11&&-15&TMR-0&-3, P0{1}={2}, R={3}, BNT={4}, SP=MM{5};", EricssonCommands.ANRSI_Command, optionGroup.Key, option.Priority, option.OutTrunk, option.BNT, option.SP);
							}
							else
							{
								command = option.Percentage.HasValue ? string.Format("{0}: BR=CL-0&&-9&-11&&-15&TMR-1&{1}, P0{2}={3}, R={4}, BNT={5}, SP=MM{6};", EricssonCommands.ANRSI_Command, option.Percentage, optionGroup.Key, option.Priority, option.OutTrunk, option.BNT, option.SP)
									: string.Format("{0}: BR=CL-0&&-9&-11&&-15&TMR-1, P0{1}={2}, R={3}, BNT={4}, SP=MM{5};", EricssonCommands.ANRSI_Command, optionGroup.Key, option.Priority, option.OutTrunk, option.BNT, option.SP);
							}
							routeCaseCommands.Add(command);
						}
					}
				}
			}
			else
			{
				routeCaseCommands.Add(string.Format("{0}: BR=CL-0&&-9&-11&&-15&TMR-0&-3, P01=1, R=BLK, BNT=1, SP=MM1;", EricssonCommands.ANRSI_Command));
			}

			routeCaseCommands.Add(string.Format("{0};", EricssonCommands.ANRPE_Command));
			return routeCaseCommands;
		}
		#endregion

		#region Commands For Route changes
		private Dictionary<string, List<EricssonRouteWithCommands>> GetEricssonRoutesWithCommands(string switchId, Dictionary<string, CustomerMappingWithActionType> customersToDeleteByBO,
		Dictionary<string, List<EricssonRouteWithCommands>> ericssonRoutesToDeleteWithCommands, out Dictionary<string, EricssonConvertedRouteDifferences> routeDifferencesByBO)
		{
			var cachedRouteCases = new RouteCaseManager().GetCachedRouteCasesGroupedByOptions(switchId);
			if (cachedRouteCases == null || cachedRouteCases.Count == 0)
				throw new VRBusinessException("No Route Cases Found");

			var routeCases = cachedRouteCases.Values.ToList();
			Dictionary<string, List<EricssonRouteWithCommands>> result = ericssonRoutesToDeleteWithCommands != null ? new Dictionary<string, List<EricssonRouteWithCommands>>(ericssonRoutesToDeleteWithCommands) : new Dictionary<string, List<EricssonRouteWithCommands>>();

			Dictionary<string, CarrierMapping> carrierMappingByCustomerBo = new Dictionary<string, CarrierMapping>();
			Dictionary<string, int> supplierByOutTrunk = new Dictionary<string, int>();

			foreach (var carrierMappingKvp in CarrierMappings)
			{
				var carrierMapping = carrierMappingKvp.Value;

				if (carrierMapping.CustomerMapping != null && !string.IsNullOrEmpty(carrierMapping.CustomerMapping.BO))
					carrierMappingByCustomerBo.Add(carrierMapping.CustomerMapping.BO, carrierMapping);

				if (carrierMapping.SupplierMapping != null && carrierMapping.SupplierMapping.OutTrunks != null)
				{
					foreach (var trunk in carrierMapping.SupplierMapping.OutTrunks)
					{
						if (!supplierByOutTrunk.Any(item => item.Key == trunk.TrunkName))
							supplierByOutTrunk.Add(trunk.TrunkName, carrierMapping.CarrierId);
					}
				}
			}

			RouteCompareTablesContext routeCompareTablesContext = new RouteCompareTablesContext();
			IRouteDataManager routeDataManager = RouteSyncEricssonDataManagerFactory.GetDataManager<IRouteDataManager>();
			routeDataManager.SwitchId = switchId;
			routeDataManager.CompareTables(routeCompareTablesContext);
			routeDifferencesByBO = routeCompareTablesContext.RouteDifferencesByBO;
			if (routeDifferencesByBO != null && routeDifferencesByBO.Count > 0)
			{
				foreach (var routeDifferencesKvp in routeCompareTablesContext.RouteDifferencesByBO)
				{
					var routeDifferences = routeDifferencesKvp.Value;
					var customerEricssonRoutesWithCommands = result.GetOrCreateItem(routeDifferencesKvp.Key);

					if (routeDifferences.RoutesToAdd != null && routeDifferences.RoutesToAdd.Count > 0)
					{
						foreach (var routeCompareResult in routeDifferences.RoutesToAdd)
						{
							var commands = GetRouteCommand(carrierMappingByCustomerBo, supplierByOutTrunk, routeCases, routeCompareResult);
							customerEricssonRoutesWithCommands.Add(new EricssonRouteWithCommands() { RouteCompareResult = routeCompareResult, Commands = commands, ActionType = RouteActionType.Add });
						}
					}

					if (routeDifferences.RoutesToUpdate != null && routeDifferences.RoutesToUpdate.Count > 0)
					{
						foreach (var routeCompareResult in routeDifferences.RoutesToUpdate)
						{
							var commands = GetRouteCommand(carrierMappingByCustomerBo, supplierByOutTrunk, routeCases, routeCompareResult);
							customerEricssonRoutesWithCommands.Add(new EricssonRouteWithCommands() { RouteCompareResult = routeCompareResult, Commands = commands, ActionType = RouteActionType.Update });
						}
					}

					if (routeDifferences.RoutesToDelete != null && routeDifferences.RoutesToDelete.Count > 0)
					{
						foreach (var routeCompareResult in routeDifferences.RoutesToDelete)
						{
							if (!customersToDeleteByBO.ContainsKey(routeCompareResult.Route.BO))
							{
								var commands = GetDeletedRouteCommands(carrierMappingByCustomerBo, supplierByOutTrunk, routeCases, routeCompareResult);
								customerEricssonRoutesWithCommands.Add(new EricssonRouteWithCommands() { RouteCompareResult = routeCompareResult, Commands = commands, ActionType = RouteActionType.Delete });
							}
						}
					}
				}
			}
			return result;
		}
		private List<string> GetDeletedRouteCommands(Dictionary<string, CarrierMapping> carrierMappingByCustomerBo, Dictionary<string, int> supplierByOutTrunk, List<RouteCase> routeCases, EricssonConvertedRouteCompareResult routeCompareResult)
		{
			var route = routeCompareResult.Route;
			List<string> deletedRouteCommands = new List<string>();

			var codeGroupObject = new CodeGroupManager().GetMatchCodeGroup(route.Code);
			codeGroupObject.ThrowIfNull(string.Format("CodeGroup not found for code {0}.", route.Code));
			string routeCodeGroup = codeGroupObject.Code;

			CarrierMapping carrierCustomerMapping;
			if (!carrierMappingByCustomerBo.TryGetValue(route.BO, out carrierCustomerMapping))
			{
				carrierCustomerMapping.ThrowIfNull(string.Format("No customer mapping found with BO: {0}.", route.BO));
			}

			var routeCase = routeCases.FindRecord(item => item.RCNumber == route.RCNumber);
			var options = Helper.DeserializeRouteCaseOptions(routeCase.RouteCaseOptionsAsString);

			EricssonRouteProperties ericssonRouteProperties = GetRouteTypeAndParameters(supplierByOutTrunk, route, ref routeCodeGroup, carrierCustomerMapping, options);

			switch (ericssonRouteProperties.Type)
			{
				case EricssonConvertedRouteType.Forward:
				case EricssonConvertedRouteType.Transit:
				case EricssonConvertedRouteType.Local:
					deletedRouteCommands.Add(string.Format("{0}:B={1}-{2};", EricssonCommands.ANBSE_Command, ericssonRouteProperties.IOBA, route.Code));
					deletedRouteCommands.Add(string.Format("{0}:B={1}-{2};", EricssonCommands.ANBSE_Command, ericssonRouteProperties.NOBA, route.Code.Substring(routeCodeGroup.Length)));
					break;
				case EricssonConvertedRouteType.Normal:
				case EricssonConvertedRouteType.Override:
					deletedRouteCommands.Add(string.Format("{0}:B={1}-{2};", EricssonCommands.ANBSE_Command, route.BO, route.Code));
					break;
				case EricssonConvertedRouteType.InterconnectOverride:
					deletedRouteCommands.Add(string.Format("{0}:B={1}-{2}{3};", EricssonCommands.ANBSE_Command, route.BO, route.Code, routeCodeGroup));
					break;
				default:
					break;
			}
			return deletedRouteCommands;
		}
		private List<string> GetRouteCommand(Dictionary<string, CarrierMapping> carrierMappingByCustomerBo, Dictionary<string, int> supplierByOutTrunk, List<RouteCase> routeCases, EricssonConvertedRouteCompareResult routeCompareResult)
		{
			var route = routeCompareResult.Route;
			List<string> routeCommands = new List<string>();
			#region GetCodeGroup
			var codeGroupObject = new CodeGroupManager().GetMatchCodeGroup(route.Code);
			codeGroupObject.ThrowIfNull(string.Format("CodeGroup not found for code {0}.", route.Code));
			string routeCodeGroup = codeGroupObject.Code;
			#endregion

			CarrierMapping carrierCustomerMapping;
			if (!carrierMappingByCustomerBo.TryGetValue(route.BO, out carrierCustomerMapping))
			{
				carrierCustomerMapping.ThrowIfNull(string.Format("No customer mapping found with BO: {0}.", route.BO));
			}

			var routeCase = routeCases.FindRecord(item => item.RCNumber == route.RCNumber);
			var options = Helper.DeserializeRouteCaseOptions(routeCase.RouteCaseOptionsAsString);
			EricssonRouteProperties ericssonRouteParamerters = GetRouteTypeAndParameters(supplierByOutTrunk, route, ref routeCodeGroup, carrierCustomerMapping, options);

			routeCommands.Add(GetRouteCommandString(route, routeCodeGroup, ericssonRouteParamerters));
			return routeCommands;
		}
		private EricssonRouteProperties GetRouteTypeAndParameters(Dictionary<string, int> supplierByOutTrunk, EricssonConvertedRoute route, ref string routeCodeGroup, CarrierMapping carrierCustomerMapping, List<RouteCaseOption> options)
		{
			var ericssonRouteParamerters = new EricssonRouteProperties();

			#region Manual Override
			var matchedmanualOverride = (ManualOverrides != null && ManualOverrides.Count > 0) ? ManualOverrides.FindRecord(item => item.BO == route.BO && item.Code == route.Code) : null;
			if (matchedmanualOverride != null)
			{
				ericssonRouteParamerters.IsOverride = true;
				ericssonRouteParamerters.Type = EricssonConvertedRouteType.Override;
				ericssonRouteParamerters.CCL = matchedmanualOverride.CCL;
				ericssonRouteParamerters.M = matchedmanualOverride.M;
				ericssonRouteParamerters.L = matchedmanualOverride.L;
				ericssonRouteParamerters.D = matchedmanualOverride.D;
				//R routeTRD = matchedmanualOverride.TRD;
			}
			#endregion

			#region InterconnectOverride
			var matchedInterconnectOverride = (InterconnectOverrides != null && InterconnectOverrides.Count > 0) ? InterconnectOverrides.FindRecord(item => item.BO == route.BO && item.Code == route.Code) : null;
			if (matchedInterconnectOverride != null)
			{
				string codeGroup = routeCodeGroup.StartsWith("1") ? "1" : routeCodeGroup;
				if (codeGroup.Length > 3)
					codeGroup = codeGroup.Substring(0, 3);

				ericssonRouteParamerters.IsInterconnectOverride = true;
				ericssonRouteParamerters.Type = EricssonConvertedRouteType.InterconnectOverride;
				ericssonRouteParamerters.D = matchedInterconnectOverride.D;
				ericssonRouteParamerters.IBNT = matchedInterconnectOverride.IBNT;
				ericssonRouteParamerters.NBNT = matchedInterconnectOverride.NBNT;
				ericssonRouteParamerters.IOBA = matchedInterconnectOverride.IOBA;
				ericssonRouteParamerters.NOBA = matchedInterconnectOverride.NOBA;
				ericssonRouteParamerters.M = matchedInterconnectOverride.M;
				ericssonRouteParamerters.L = matchedInterconnectOverride.L;
				ericssonRouteParamerters.CCL = codeGroup.Length.ToString();
				ericssonRouteParamerters.NationalM = matchedInterconnectOverride.NationalM;
				ericssonRouteParamerters.FBO = matchedInterconnectOverride.FBO;
				ericssonRouteParamerters.CC = matchedInterconnectOverride.CC;
				routeCodeGroup = codeGroup;
				route.Code = matchedInterconnectOverride.Prefix;
				//R routePrefix = matchedInterconnectOverride.Prefix;
				//R routeIsActive = matchedInterconnectOverride.IsActive;
				//R routeTRD = matchedInterconnectOverride.TRD;
			}
			#endregion

			#region Local,Transit,Forward,Normal
			if (routeCodeGroup == LocalCountryCode && !ericssonRouteParamerters.IsInterconnectOverride)
			{
				int supplierId;
				LocalSupplierMapping localSupplierMapping = null;
				IncomingTrafficSupplier incomingTrafficSupplier = null;
				if (options != null && options.Any())
				{
					if (supplierByOutTrunk.TryGetValue(options.First().OutTrunk, out supplierId))
					{
						localSupplierMapping = (LocalSupplierMappings == null) ? null : LocalSupplierMappings.FindRecord(item => item.SupplierId == supplierId);
						incomingTrafficSupplier = (IncomingTrafficSuppliers == null) ? null : IncomingTrafficSuppliers.FindRecord(item => item.SupplierId == supplierId);
					}
				}

				if (localSupplierMapping != null)
				{
					ericssonRouteParamerters.Type = EricssonConvertedRouteType.Forward;
					ericssonRouteParamerters.L = ericssonRouteParamerters.IsOverride ? ericssonRouteParamerters.L : LocalNumberLength.ToString();
					ericssonRouteParamerters.NationalM = ericssonRouteParamerters.IsOverride ? ericssonRouteParamerters.M : "";
					ericssonRouteParamerters.FBO = localSupplierMapping.BO;
					ericssonRouteParamerters.CCL = ericssonRouteParamerters.IsOverride ? ericssonRouteParamerters.CCL : "";
				}
				else if (incomingTrafficSupplier != null)
				{
					ericssonRouteParamerters.Type = EricssonConvertedRouteType.Local;
					ericssonRouteParamerters.L = ericssonRouteParamerters.IsOverride ? ericssonRouteParamerters.L : LocalNumberLength.ToString();
					ericssonRouteParamerters.NationalM = ericssonRouteParamerters.IsOverride ? ericssonRouteParamerters.M : "";
					ericssonRouteParamerters.FBO = ericssonRouteParamerters.NOBA;
					ericssonRouteParamerters.CCL = ericssonRouteParamerters.IsOverride ? ericssonRouteParamerters.CCL : "";
					ericssonRouteParamerters.D = ericssonRouteParamerters.IsOverride ? ericssonRouteParamerters.D : "4-0";
					ericssonRouteParamerters.CC = ericssonRouteParamerters.IsOverride ? ericssonRouteParamerters.CC : "1";
				}
				else
				{
					ericssonRouteParamerters.Type = EricssonConvertedRouteType.Transit;
					ericssonRouteParamerters.L = ericssonRouteParamerters.IsOverride ? ericssonRouteParamerters.L : LocalNumberLength.ToString();
					ericssonRouteParamerters.NationalM = ericssonRouteParamerters.IsOverride ? ericssonRouteParamerters.M : "0-" + LocalCountryCode;
					ericssonRouteParamerters.CCL = ericssonRouteParamerters.IsOverride ? ericssonRouteParamerters.CCL : LocalCountryCode.Length.ToString();
					ericssonRouteParamerters.FBO = ericssonRouteParamerters.NOBA;
					ericssonRouteParamerters.D = ericssonRouteParamerters.IsOverride ? ericssonRouteParamerters.D : "7-0";
					ericssonRouteParamerters.CC = ericssonRouteParamerters.IsOverride ? ericssonRouteParamerters.CC : "3";
				}
			}
			else if (!ericssonRouteParamerters.IsOverride && !ericssonRouteParamerters.IsInterconnectOverride)
			{
				ericssonRouteParamerters.Type = EricssonConvertedRouteType.Normal;
				ericssonRouteParamerters.L = MinCodeLength + "-" + MaxCodeLength;
				ericssonRouteParamerters.M = InterconnectGeneralPrefix;
				ericssonRouteParamerters.CCL = routeCodeGroup.StartsWith("1") ? "1" : routeCodeGroup.Length > 3 ? "3" : routeCodeGroup.Length.ToString();
				ericssonRouteParamerters.CC = "1";
				if (OutgoingTrafficCustomers.Any(item => item.CustomerId == carrierCustomerMapping.CarrierId))
					ericssonRouteParamerters.D = "6-0";
				else
					ericssonRouteParamerters.D = "7-0";
			}
			#endregion

			return ericssonRouteParamerters;
		}
		private string GetRouteCommandString(EricssonConvertedRoute route, string routeCodeGroup, EricssonRouteProperties routeParamerters)
		{
			StringBuilder strCommand = new StringBuilder();
			string L = null;
			string B = null;

			switch (routeParamerters.Type)
			{
				case EricssonConvertedRouteType.Override:
					B = string.Format("{0}-{1}", route.BO, route.Code);

					L = routeParamerters.L;
					if (!string.IsNullOrEmpty(routeParamerters.L) && route.Code.Length > MinCodeLength)
						L = route.Code.Length.ToString() + "-" + MaxCodeLength.ToString();

					strCommand.Append(GetRouteCommandStringText(B, route.RCNumber, L, routeParamerters.M, routeParamerters.D, routeParamerters.CC, routeParamerters.CCL, null, null));
					break;

				case EricssonConvertedRouteType.Normal:
					B = string.Format("{0}-{1}", route.BO, route.Code);

					if (route.Code.Length > MinCodeLength)
						L = route.Code.Length.ToString() + "-" + MaxCodeLength.ToString();

					strCommand.Append(GetRouteCommandStringText(B, route.RCNumber, L, string.Format("0-{0}", routeParamerters.M), routeParamerters.D, routeParamerters.CC, routeParamerters.CCL, null, null));
					break;

				case EricssonConvertedRouteType.Forward:
					B = string.Format("{0}-{1}", routeParamerters.IOBA, route.Code);
					strCommand.Append(GetRouteCommandStringText(B, null, null, routeParamerters.M, null, null, null, routeParamerters.FBO, routeParamerters.NBNT));

					B = string.Format("{0}-{1}", routeParamerters.NOBA, route.Code.Substring(routeCodeGroup.Length));
					strCommand.Append(GetRouteCommandStringText(B, null, null, routeParamerters.NationalM, null, null, routeParamerters.CCL, routeParamerters.FBO, routeParamerters.NBNT));
					break;

				case EricssonConvertedRouteType.Transit:
					B = string.Format("{0}-{1}", routeParamerters.IOBA, route.Code);
					strCommand.Append(GetRouteCommandStringText(B, null, null, routeParamerters.M, null, null, null, routeParamerters.NOBA, routeParamerters.NBNT));

					B = string.Format("{0}-{1}", routeParamerters.NOBA, route.Code.Substring(routeCodeGroup.Length));
					strCommand.Append(GetRouteCommandStringText(B, route.RCNumber, routeParamerters.L, string.IsNullOrEmpty(routeParamerters.NationalM) ? routeParamerters.M : routeParamerters.NationalM, routeParamerters.D, routeParamerters.CC, routeParamerters.CCL, null, null));
					break;

				case EricssonConvertedRouteType.Local:
					B = string.Format("{0}-{1}", routeParamerters.IOBA, route.Code);
					strCommand.Append(GetRouteCommandStringText(B, null, null, routeParamerters.M, null, null, null, routeParamerters.NOBA, routeParamerters.NBNT));

					B = string.Format("{0}-{1}", routeParamerters.NOBA, route.Code.Substring(routeCodeGroup.Length));
					strCommand.Append(GetRouteCommandStringText(B, route.RCNumber, routeParamerters.L, routeParamerters.NationalM, routeParamerters.D, routeParamerters.CC, routeParamerters.CCL, null, null));
					break;

				case EricssonConvertedRouteType.InterconnectOverride:
					B = string.Format("{0}-{1}{2}", route.BO, InterconnectGeneralPrefix, routeCodeGroup);
					strCommand.Append(GetRouteCommandStringText(B, route.RCNumber, routeParamerters.L, routeParamerters.M, routeParamerters.D, routeParamerters.CC, routeParamerters.CCL, null, null));
					break;

				default:
					throw new Exception();
			}
			return strCommand.ToString();
		}
		private StringBuilder GetRouteCommandStringText(string B, int? RC, string L, string M, string D, string CC, string CCL, string F, string BNT)
		{
			StringBuilder script = new StringBuilder(string.Format("{0}: ", EricssonCommands.ANBSI_Command));
			if (!string.IsNullOrEmpty(B))
				script.AppendFormat("B={0}", B);

			if (RC.HasValue)
				script.AppendFormat(",RC={0}", RC);

			if (!string.IsNullOrEmpty(L))
				script.AppendFormat(",L={0}", L);

			if (!string.IsNullOrEmpty(M))
				script.AppendFormat(",M={0}", M);

			if (!string.IsNullOrEmpty(D))
				script.AppendFormat(",D={0}", D);

			if (!string.IsNullOrEmpty(CC))
				script.AppendFormat(",CC={0}", CC);

			if (!string.IsNullOrEmpty(CCL))
				script.AppendFormat(",CCL={0}", CCL);

			if (!string.IsNullOrEmpty(F))
				script.AppendFormat(",F={0}", F);

			if (!string.IsNullOrEmpty(BNT))
				script.AppendFormat(",BNT={0}", BNT);

			script.AppendLine(";");
			return script;
		}
		#endregion

		#region LOG File
		private static void LogEricssonRouteCommands(Dictionary<string, List<EricssonRouteWithCommands>> succeedEricssonRoutesWithCommandsByBo, Dictionary<string, List<EricssonRouteWithCommands>> failedEricssonRoutesWithCommandsByBo, SwitchLogger ftpLogger, DateTime dateTime)
		{
			if (succeedEricssonRoutesWithCommandsByBo != null && succeedEricssonRoutesWithCommandsByBo.Count > 0)
			{
				foreach (var customerRoutesWithCommandsKvp in succeedEricssonRoutesWithCommandsByBo)
				{
					var customerRoutesWithCommands = customerRoutesWithCommandsKvp.Value;
					var commandResults = customerRoutesWithCommands.Select(item => new CommandResult() { Command = string.Join(Environment.NewLine, item.Commands) }).ToList();
					ILogRoutesContext logRoutesContext = new LogRoutesContext() { ExecutionDateTime = dateTime, ExecutionStatus = ExecutionStatus.Succeeded, CommandResults = commandResults, BONumber = Convert.ToInt32(customerRoutesWithCommandsKvp.Key) };
					ftpLogger.LogRoutes(logRoutesContext);
				}
			}

			if (failedEricssonRoutesWithCommandsByBo != null && failedEricssonRoutesWithCommandsByBo.Count > 0)
			{
				foreach (var customerRoutesWithCommandsKvp in succeedEricssonRoutesWithCommandsByBo)
				{
					var customerRoutesWithCommands = customerRoutesWithCommandsKvp.Value;
					var commandResults = customerRoutesWithCommands.Select(item => new CommandResult() { Command = string.Join(Environment.NewLine, item.Commands) }).ToList();
					ILogRoutesContext logRoutesContext = new LogRoutesContext() { ExecutionDateTime = dateTime, ExecutionStatus = ExecutionStatus.Failed, CommandResults = commandResults, BONumber = Convert.ToInt32(customerRoutesWithCommandsKvp.Key) };
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
					//var trunkCommands = string.Join(Environment.NewLine, succeedCustomerMappingWithCommands.TrunkCommandsByTrunkId.Values);
					//var commands = string.Join(Environment.NewLine, obaCommands, trunkCommands);
					var commands = string.Join(Environment.NewLine, obaCommands);
					commandResults.Add(new CommandResult() { Command = commands });
				}
				ILogCarrierMappingsContext logRouteCasesContext = new LogCarrierMappingsContext() { ExecutionDateTime = dateTime, ExecutionStatus = ExecutionStatus.Succeeded, CommandResults = commandResults };
				ftpLogger.LogCarrierMappings(logRouteCasesContext);
			}

			if (failedCustomerMappingsWithCommands != null && failedCustomerMappingsWithCommands.Count > 0)
			{
				List<CommandResult> commandResults = new List<CommandResult>();
				foreach (var succeedCustomerMappingWithCommands in succeedCustomerMappingsWithCommands)
				{
					var obaCommands = string.Join(Environment.NewLine, succeedCustomerMappingWithCommands.OBACommands);
					//var trunkCommands = string.Join(Environment.NewLine, succeedCustomerMappingWithCommands.TrunkCommandsByTrunkId.Values);
					//var commands = string.Join(Environment.NewLine, obaCommands, trunkCommands);
					var commands = string.Join(Environment.NewLine, obaCommands);
					commandResults.Add(new CommandResult() { Command = commands });
				}
				ILogCarrierMappingsContext logRouteCasesContext = new LogCarrierMappingsContext() { ExecutionDateTime = dateTime, ExecutionStatus = ExecutionStatus.Succeeded, CommandResults = commandResults };
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

		#region SSH
		private string OpenConnectionWithSwitch(SSHCommunicator sshCommunicator, List<CommandResult> commandResults)
		{
			string response;
			sshCommunicator.OpenConnection();
			sshCommunicator.OpenShell();
			sshCommunicator.ReadPrompt(">");
			sshCommunicator.ExecuteCommand(EricssonCommands.MML_Command, "<", out response);
			commandResults.Add(new CommandResult() { Command = EricssonCommands.MML_Command, Output = new List<string>() { response } });
			return response;
		}

		private bool ExecuteCustomerMappingsCommands(List<CustomerMappingWithCommands> customerMappingsWithCommands, EricssonSSHCommunication sshCommunication,
		SSHCommunicator sshCommunicator, List<CommandResult> commandResults, out List<CustomerMappingWithCommands> succeededCustomerMappingsWithCommands,
		out List<CustomerMappingWithCommands> failedCustomerMappingsWithCommands, out List<CustomerMappingWithCommands> succeedCustomerMappingsWithFailedTrunk,
		IEnumerable<string> faultCodes, int maxNumberOfRetries)
		{
			succeededCustomerMappingsWithCommands = new List<CustomerMappingWithCommands>();
			failedCustomerMappingsWithCommands = new List<CustomerMappingWithCommands>();
			succeedCustomerMappingsWithFailedTrunk = new List<CustomerMappingWithCommands>();

			if (customerMappingsWithCommands == null || customerMappingsWithCommands.Count == 0)
				return false;

			if (sshCommunication == null)
			{
				succeededCustomerMappingsWithCommands = customerMappingsWithCommands;
				return true;
			}

			bool isPreTableSucceed = false;
			int numberOfTriesDone = 0;

			string response;
			response = OpenConnectionWithSwitch(sshCommunicator, commandResults);

			while (!isPreTableSucceed && numberOfTriesDone < maxNumberOfRetries)
			{
				try
				{
					sshCommunicator.ExecuteCommand(EricssonCommands.PNBAR_Command, CommandPrompt, out response);
					commandResults.Add(new CommandResult() { Command = EricssonCommands.PNBAR_Command, Output = new List<string>() { response } });

					sshCommunicator.ExecuteCommand(";", CommandPrompt, out response);
					commandResults.Add(new CommandResult() { Command = ";", Output = new List<string>() { response } });

					if (IsCommandFailed(response, faultCodes))
					{
						sshCommunicator.ExecuteCommand(EricssonCommands.PNBZI_Command, CommandPrompt, out response);
						commandResults.Add(new CommandResult() { Command = EricssonCommands.PNBZI_Command, Output = new List<string>() { response } });

						sshCommunicator.ExecuteCommand(EricssonCommands.PNBCI_Command, CommandPrompt, out response);
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
								foreach (var command in customerMappingWithCommands.OBACommands)
								{
									sshCommunicator.ExecuteCommand(command, CommandPrompt, out response);
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
									//customerMappingSucceededWithCommands.TrunkCommandsByTrunkId = new Dictionary<Guid, string>();
								}

								/*var trunkFailed = false;
								if (isCustomerSucceed && customerMappingWithCommands.TrunkCommandsByTrunkId != null)
								{
									foreach (var trunkCommandKvp in customerMappingWithCommands.TrunkCommandsByTrunkId)
									{
										var trunkCommand = trunkCommandKvp.Value;
										sshCommunicator.ExecuteCommand(trunkCommand, CommandPrompt, out response);
										commandResults.Add(new CommandResult() { Command = trunkCommand, Output = new List<string>() { response } });

										if (IsCommandSucceed(response))
										{
											if (customerMappingSucceededWithCommands.CustomerMappingWithActionType.CustomerMapping.InTrunks == null)
												customerMappingSucceededWithCommands.CustomerMappingWithActionType.CustomerMapping.InTrunks = new List<InTrunk>();
											var trunk = customerMappingWithCommands.CustomerMappingWithActionType.CustomerMapping.InTrunks.FindRecord(item => item.TrunkId == trunkCommandKvp.Key);
											customerMappingSucceededWithCommands.CustomerMappingWithActionType.CustomerMapping.InTrunks.Add(trunk);
											customerMappingSucceededWithCommands.TrunkCommandsByTrunkId.Add(trunkCommandKvp.Key, trunkCommand);
										}
										else
										{
											trunkFailed = true;
											customerMappingFailedWithCommands = new CustomerMappingWithCommands();
											var trunk = customerMappingWithCommands.CustomerMappingWithActionType.CustomerMapping.InTrunks.FindRecord(item => item.TrunkId == trunkCommandKvp.Key);
											customerMappingFailedWithCommands.CustomerMappingWithActionType.CustomerMapping.InTrunks.Add(trunk);
											customerMappingFailedWithCommands.TrunkCommandsByTrunkId.Add(trunkCommandKvp.Key, trunkCommand);
										}
									}
								}*/

								if (isCustomerSucceed)
									succeededCustomerMappingsWithCommands.Add(customerMappingSucceededWithCommands);
								/*if (trunkFailed)
									succeedCustomerMappingsWithFailedTrunk.Add(customerMappingSucceededWithCommands);*/
								if (customerMappingFailedWithCommands != null)
									failedCustomerMappingsWithCommands.Add(customerMappingFailedWithCommands);
							}
							catch (Exception ex)
							{
								customerNumberOfTriesDone++;
								isCustomerSucceed = false;
							}
						}
					}

					sshCommunicator.ExecuteCommand(EricssonCommands.PNBAI_Command, CommandPrompt, out response);
					commandResults.Add(new CommandResult() { Command = EricssonCommands.PNBAI_Command, Output = new List<string>() { response } });

					sshCommunicator.ExecuteCommand(";", CommandPrompt, out response);
					commandResults.Add(new CommandResult() { Command = ";", Output = new List<string>() { response } });

					sshCommunicator.ExecuteCommand(EricssonCommands.Exit_Command, ">", out response);
					commandResults.Add(new CommandResult() { Command = EricssonCommands.Exit_Command, Output = new List<string>() { response } });

					sshCommunicator.ExecuteCommand(EricssonCommands.Exit_Command);
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

		private void ExecuteRouteCasesCommands(List<RouteCaseWithCommands> routeCasesWithCommands, EricssonSSHCommunication sshCommunication,
		SSHCommunicator sshCommunicator, List<CommandResult> commandResults, out List<RouteCaseWithCommands> succeedRouteCaseNumbers, out List<RouteCaseWithCommands> failedRouteCaseNumbers, IRouteCaseDataManager routeCaseDataManager, int maxNumberOfRetries)
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
				return;
			}

			bool isSuccessfull;
			int numberOfTriesDone;

			string response;

			response = OpenConnectionWithSwitch(sshCommunicator, commandResults);

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
						sshCommunicator.ExecuteCommand(command, CommandPrompt, out response);
						commandResults.Add(new CommandResult() { Command = command, Output = new List<string>() { response } });
						if (!IsCommandSucceed(response))
						{
							string commandTemp = string.Format("{0}:RC={1};", EricssonCommands.ANRAR_Command, rcNumber);
							sshCommunicator.ExecuteCommand(commandTemp, CommandPrompt, out response);
							commandResults.Add(new CommandResult() { Command = commandTemp, Output = new List<string>() { response } });

							sshCommunicator.ExecuteCommand(";", CommandPrompt, out response);
							commandResults.Add(new CommandResult() { Command = ";", Output = new List<string>() { response } });

							if (IsCommandSucceed(response))
							{
								commandTemp = string.Format("{0}:RC={1};", EricssonCommands.ANRZI_Command, rcNumber);
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
						command = string.Format("{0}:RC={1};", EricssonCommands.ANRAI_Command, rcNumber);
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

						if (!commandsSucceed)
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
			}

			if (routeCaseNumbersToUpdate.Count > 0)
			{
				routeCaseDataManager.UpdateSyncedRouteCases(routeCaseNumbersToUpdate);
				routeCaseNumbersToUpdate = new List<int>();
			}
		}

		private void ExecuteRoutesCommands(Dictionary<string, List<EricssonRouteWithCommands>> routesWithCommandsByBo, EricssonSSHCommunication sshCommunication,
		SSHCommunicator sshCommunicator, Dictionary<string, CustomerMappingWithActionType> customersToDeleteByBO, List<CommandResult> commandResults, out Dictionary<string, List<EricssonRouteWithCommands>> succeededRoutesWithCommandsByBo,
		out Dictionary<string, List<EricssonRouteWithCommands>> failedRoutesWithCommandsByBo, out Dictionary<string, List<EricssonRouteWithCommands>> allFailedRoutesWithCommandsByBo,
		out List<CustomerMappingWithActionType> customerMappingsToDeleteSucceed, out List<CustomerMappingWithActionType> customerMappingsToDeleteFailed,
		IEnumerable<string> failedCustomerMappingBOs, IEnumerable<int> faildRouteCaseNumbers, IEnumerable<string> faultCodes, int maxNumberOfRetries)
		{
			succeededRoutesWithCommandsByBo = new Dictionary<string, List<EricssonRouteWithCommands>>();
			failedRoutesWithCommandsByBo = new Dictionary<string, List<EricssonRouteWithCommands>>();
			allFailedRoutesWithCommandsByBo = new Dictionary<string, List<EricssonRouteWithCommands>>();
			customerMappingsToDeleteSucceed = new List<CustomerMappingWithActionType>();
			customerMappingsToDeleteFailed = new List<CustomerMappingWithActionType>();

			if (routesWithCommandsByBo == null || routesWithCommandsByBo.Count == 0)
				return;

			if (sshCommunication == null)
			{
				succeededRoutesWithCommandsByBo = routesWithCommandsByBo;
				customerMappingsToDeleteSucceed = (customersToDeleteByBO.Values == null) ? null : customersToDeleteByBO.Values.ToList();
				return;
			}

			bool isSuccessfull;
			int numberOfTriesDone;

			string response;

			OpenConnectionWithSwitch(sshCommunicator, commandResults);
			sshCommunicator.ExecuteCommand(EricssonCommands.ANBAR_Command, CommandPrompt, out response);
			commandResults.Add(new CommandResult() { Command = EricssonCommands.ANBAR_Command, Output = new List<string>() { response } });
			sshCommunicator.ExecuteCommand(";", CommandPrompt, out response);
			commandResults.Add(new CommandResult() { Command = ";", Output = new List<string>() { response } });

			if (IsCommandFailed(response, faultCodes))
			{
				sshCommunicator.ExecuteCommand(EricssonCommands.ANBZI_Command, CommandPrompt, out response);
				commandResults.Add(new CommandResult() { Command = EricssonCommands.ANBZI_Command, Output = new List<string>() { response } });
				Thread.Sleep(5000);

				if (response.ToUpper().Contains(EricssonCommands.ORDERED))
				{
					sshCommunicator.ExecuteCommand(EricssonCommands.Exit_Command, ">", out response);
					commandResults.Add(new CommandResult() { Command = EricssonCommands.Exit_Command, Output = new List<string>() { response } });
					Thread.Sleep(2000);
					sshCommunicator.ExecuteCommand(EricssonCommands.MML_Command, "<", out response);
					commandResults.Add(new CommandResult() { Command = EricssonCommands.MML_Command, Output = new List<string>() { response } });
					Thread.Sleep(1000);
				}
				else if (!IsCommandSucceed(response))
				{
					throw new Exception("ANBAR Not Executed");
				}

				sshCommunicator.ExecuteCommand(EricssonCommands.ANBCI_Command, CommandPrompt, out response);
				commandResults.Add(new CommandResult() { Command = EricssonCommands.ANBCI_Command, Output = new List<string>() { response } });

				Thread.Sleep(5000);
				while (!response.ToUpper().Contains(EricssonCommands.ORDERED) && response.ToUpper().Contains("new List<string>() { response }  BUSY"))
				{
					sshCommunicator.ExecuteCommand(EricssonCommands.ANBCI_Command, CommandPrompt, out response);
					commandResults.Add(new CommandResult() { Command = EricssonCommands.ANBCI_Command, Output = new List<string>() { response } });
					Thread.Sleep(3000);
				}
				if (response.ToUpper().Contains(EricssonCommands.ORDERED))
				{
					sshCommunicator.ExecuteCommand(EricssonCommands.Exit_Command, ">", out response);
					commandResults.Add(new CommandResult() { Command = EricssonCommands.Exit_Command, Output = new List<string>() { response } });
					Thread.Sleep(2000);
					sshCommunicator.ExecuteCommand(EricssonCommands.MML_Command, "<", out response);
					commandResults.Add(new CommandResult() { Command = EricssonCommands.MML_Command, Output = new List<string>() { response } });
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

			foreach (var ericssonRouteWithCommandsKvp in routesWithCommandsByBo)
			{
				var customerBo = ericssonRouteWithCommandsKvp.Key;
				var ericssonRoutesWithCommands = ericssonRouteWithCommandsKvp.Value;
				if (failedCustomerMappingBOs.Contains(customerBo))
				{
					var allFailedRoutesWithCommands = allFailedRoutesWithCommandsByBo.GetOrCreateItem(customerBo);
					allFailedRoutesWithCommands.AddRange(ericssonRoutesWithCommands);
					continue;
				}
				foreach (var ericssonRouteWithCommands in ericssonRoutesWithCommands)
				{
					if (ericssonRouteWithCommands.RouteCompareResult != null && faildRouteCaseNumbers.Contains(ericssonRouteWithCommands.RouteCompareResult.Route.RCNumber))
					{
						var allFailedRoutesWithCommands = allFailedRoutesWithCommandsByBo.GetOrCreateItem(customerBo);
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
								sshCommunicator.ExecuteCommand(command, CommandPrompt, out response);
								commandResults.Add(new CommandResult() { Command = command, Output = new List<string>() { response } });
								while (response.ToUpper().Contains(EricssonCommands.FUNCTION_BUSY))
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
								var succeededEricssonRoutesWithCommands = succeededRoutesWithCommandsByBo.GetOrCreateItem(customerBo);
								succeededEricssonRoutesWithCommands.Add(ericssonRouteWithCommands);
								if (ericssonRouteWithCommands.ActionType == RouteActionType.DeleteCustomer)
								{
									CustomerMappingWithActionType customersToDeleteSucceed;
									if (customersToDeleteByBO.TryGetValue(customerBo, out customersToDeleteSucceed))
										customerMappingsToDeleteSucceed.Add(customersToDeleteSucceed);
								}
							}
							else
							{

								var allFailedEricssonRoutesWithCommands = allFailedRoutesWithCommandsByBo.GetOrCreateItem(customerBo);
								var failedEricssonRoutesWithCommands = failedRoutesWithCommandsByBo.GetOrCreateItem(customerBo);
								failedEricssonRoutesWithCommands.Add(ericssonRouteWithCommands);
								allFailedEricssonRoutesWithCommands.Add(ericssonRouteWithCommands);
								if (ericssonRouteWithCommands.ActionType == RouteActionType.DeleteCustomer)
								{
									CustomerMappingWithActionType customersToDeleteSucceed;
									if (customersToDeleteByBO.TryGetValue(customerBo, out customersToDeleteSucceed))
										customerMappingsToDeleteFailed.Add(customersToDeleteSucceed);
								}
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

			sshCommunicator.ExecuteCommand(EricssonCommands.ANBAI_Command, CommandPrompt, out response);
			commandResults.Add(new CommandResult() { Command = EricssonCommands.ANBAI_Command, Output = new List<string>() { response } });
			sshCommunicator.ExecuteCommand(";", CommandPrompt, out response);
			commandResults.Add(new CommandResult() { Command = ";", Output = new List<string>() { response } });

			Thread.Sleep(3000);
			sshCommunicator.ExecuteCommand(EricssonCommands.Exit_Command, ">", out response);
			commandResults.Add(new CommandResult() { Command = EricssonCommands.Exit_Command, Output = new List<string>() { response } });
			sshCommunicator.ExecuteCommand(EricssonCommands.Exit_Command);
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
		#endregion
		#endregion
	}
}