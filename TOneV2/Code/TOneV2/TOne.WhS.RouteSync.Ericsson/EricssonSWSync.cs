using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Common;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.RouteSync.Entities;
using TOne.WhS.RouteSync.Ericsson.Data;
using TOne.WhS.RouteSync.Ericsson.Entities;
using TOne.WhS.RouteSync.Ericsson.Business;
using System.Text;
using Vanrise.Entities;
using Vanrise.Rules;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.MainExtensions.GenericRuleCriteriaFieldValues;
using System.Threading;

namespace TOne.WhS.RouteSync.Ericsson
{
	public partial class EricssonSWSync : SwitchRouteSynchronizer
	{
		public override Guid ConfigId { get { return new Guid("94739CBC-00A7-4CEB-9285-B4CB35D7D003"); } }
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

				List<RouteCaseOption> routeCaseOptions = GetRouteCaseOptions(route, routeCodeGroupId, routeCodeGroup);
				routeCaseOptions = routeCaseOptions.OrderBy(item => item.Priority).ToList();
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
				throw new Exception("No Logger");

			var ftpLogger = SwitchLoggerList.First(item => item.IsActive);
			ftpLogger.ThrowIfNull("ftpLogger");
			#endregion

			#region Initialize
			Dictionary<string, CustomerMappingWithActionType> customersToDeleteByBO = new Dictionary<string, CustomerMappingWithActionType>();
			List<RouteCaseWithCommands> routeCasesWithCommands = new List<RouteCaseWithCommands>();
			List<CustomerMappingWithCommands> customerMappingsWithCommands = new List<CustomerMappingWithCommands>();
			Dictionary<string, List<EricssonRouteWithCommands>> ericssonRoutesWithCommandsByBo = new Dictionary<string, List<EricssonRouteWithCommands>>();

			IRouteCaseDataManager routeCaseDataManager = RouteSyncEricssonDataManagerFactory.GetDataManager<IRouteCaseDataManager>();
			routeCaseDataManager.SwitchId = context.SwitchId;
			IRouteDataManager routeDataManager = RouteSyncEricssonDataManagerFactory.GetDataManager<IRouteDataManager>();
			routeDataManager.SwitchId = context.SwitchId;
			ICustomerMappingDataManager customerMappingDataManager = RouteSyncEricssonDataManagerFactory.GetDataManager<ICustomerMappingDataManager>();
			customerMappingDataManager.SwitchId = context.SwitchId;

			#endregion

			var routeCasesDictionary = new RouteCaseManager().GetCachedRouteCasesGroupedByOptions(context.SwitchId);
			var routeCases = (routeCasesDictionary != null && routeCasesDictionary.Count > 0) ? routeCasesDictionary.FindAllRecords(item => item.Synced == false).ToList() : null;
			routeCasesWithCommands = GetRouteCasesWithCommands(routeCases);

			customerMappingsWithCommands = GetCustomerMappingsWithCommands(context.SwitchId, customersToDeleteByBO, ericssonRoutesWithCommandsByBo);

			GetEricssonRoutesWithCommands(context.SwitchId, customersToDeleteByBO, ericssonRoutesWithCommandsByBo);

			LogCustomerMappingCommands(customerMappingsWithCommands, null, ftpLogger);
			customerMappingDataManager.Swap(new CustomerMappingFinalizeContext());

			LogRouteCaseCommands(routeCasesWithCommands, null, ftpLogger);
			if (routeCasesWithCommands != null && routeCasesWithCommands.Any())
				routeCaseDataManager.UpdateSyncedRouteCases(routeCasesWithCommands.Select(item => item.RouteCase.RCNumber));

			LogEricssonRouteCommands(ericssonRoutesWithCommandsByBo, null, ftpLogger);
			routeDataManager.Swap(new RouteFinalizeContext());
		}

		public override bool IsSwitchRouteSynchronizerValid(IIsSwitchRouteSynchronizerValidContext context)
		{
			if (this.CarrierMappings == null || this.CarrierMappings.Count == 0)
				return true;

			Dictionary<string, List<int>> carrierAccountIdsByTrunkName = new Dictionary<string, List<int>>();

			foreach (var mapping in this.CarrierMappings.Values)
			{
				if (mapping.CustomerMapping != null && mapping.CustomerMapping.InTrunks != null)
				{
					foreach (var inTrunk in mapping.CustomerMapping.InTrunks)
					{
						List<int> carrierAccountIds = carrierAccountIdsByTrunkName.GetOrCreateItem(inTrunk.TrunkName);
						carrierAccountIds.Add(mapping.CarrierId);
					}
				}
			}

			Dictionary<string, List<int>> duplicatedInTrunks = carrierAccountIdsByTrunkName.Where(itm => itm.Value.Count > 1).ToDictionary(itm => itm.Key, itm => itm.Value);
			if (duplicatedInTrunks.Count == 0)
				return true;

			List<string> validationMessages = new List<string>();

			if (duplicatedInTrunks.Count > 0)
			{
				CarrierAccountManager carrierAccountManager = new CarrierAccountManager();

				foreach (var kvp in duplicatedInTrunks)
				{
					string trunkName = kvp.Key;
					List<int> customerIds = kvp.Value;

					List<string> carrierAccountNames = new List<string>();

					foreach (var customerId in customerIds)
					{
						string customerName = carrierAccountManager.GetCarrierAccountName(customerId);
						carrierAccountNames.Add(string.Format("'{0}'", customerName));
					}

					validationMessages.Add(string.Format("Trunk Name: '{0}' is duplicated at Carrier Accounts: {1}", trunkName, string.Join(", ", carrierAccountNames)));
				}
			}

			context.ValidationMessages = validationMessages;
			return false;
		}

		#endregion

		#region Private Methods

		#region Get Route Case Options
		private List<RouteCaseOption> GetRouteCaseOptions(Route route, int codeGroupId, string routeCodeGroup)
		{
			List<RouteCaseOption> routeCaseOptions = new List<RouteCaseOption>();
			var ruleTree = new EricssonSWSync().BuildSupplierTrunkGroupTree(CarrierMappings);
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

					foreach (var trunkGroupTrunk in trunkGroup.TrunkTrunkGroups)
					{
						routeCaseOptions.Add(GetRouteCaseOption(routeCodeGroup, option.SupplierId, option.Percentage, supplierMapping, trunkGroup, trunkGroupTrunk, groupId));
						numberOfOptions++;
						if (numberOfOptions == NumberOfOptions)
							return routeCaseOptions;
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

							var backupTrunkGroup = GetTrunkGroup(ruleTree, backupOption.SupplierId, codeGroupId, route.CustomerId, false);
							if (backupTrunkGroup == null)
								continue;

							foreach (var trunkGroupTrunk in backupTrunkGroup.TrunkTrunkGroups)
							{
								routeCaseOptions.Add(GetRouteCaseOption(routeCodeGroup, backupOption.SupplierId, null, supplierMapping, backupTrunkGroup, trunkGroupTrunk, groupId));
								numberOfOptions++;
								if (numberOfOptions == NumberOfOptions)
									return routeCaseOptions;
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

			if (routeCodeGroup == LocalCountryCode)
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
		private List<CustomerMappingWithCommands> GetCustomerMappingsWithCommands(string switchId, Dictionary<string, CustomerMappingWithActionType> customersToDeleteByBO, Dictionary<string, List<EricssonRouteWithCommands>> routeCommandsByBo)
		{
			CustomerMappingTablesContext customerMappingTablesContext = new CustomerMappingTablesContext();
			ICustomerMappingDataManager customerMappingDataManager = RouteSyncEricssonDataManagerFactory.GetDataManager<ICustomerMappingDataManager>();
			customerMappingDataManager.SwitchId = switchId;
			customerMappingDataManager.CompareTables(customerMappingTablesContext);
			List<CustomerMappingWithCommands> customerMappingsWithCommands = new List<CustomerMappingWithCommands>();

			if (customerMappingTablesContext.CustomerMappingsToAdd != null && customerMappingTablesContext.CustomerMappingsToAdd.Count > 0)
			{
				foreach (var customerMappingToAdd in customerMappingTablesContext.CustomerMappingsToAdd)
				{
					var commands = GetCustomerMappingCommands(customerMappingToAdd);
					customerMappingsWithCommands.Add(new CustomerMappingWithCommands() { Commands = commands, CustomerMappingWithActionType = new CustomerMappingWithActionType() { CustomerMappingSerialized = customerMappingToAdd, ActionType = CustomerMappingActionType.Add } });
				}
			}

			if (customerMappingTablesContext.CustomerMappingsToUpdate != null && customerMappingTablesContext.CustomerMappingsToUpdate.Count > 0)
			{
				foreach (var customerMappingsToUpdate in customerMappingTablesContext.CustomerMappingsToUpdate)
				{
					var commands = GetCustomerMappingCommands(customerMappingsToUpdate);
					customerMappingsWithCommands.Add(new CustomerMappingWithCommands() { Commands = commands, CustomerMappingWithActionType = new CustomerMappingWithActionType() { CustomerMappingSerialized = customerMappingsToUpdate, ActionType = CustomerMappingActionType.Update } });
				}
			}

			if (customerMappingTablesContext.CustomerMappingsToDelete != null && customerMappingTablesContext.CustomerMappingsToDelete.Count > 0)
			{
				foreach (var customerMappingsToDelete in customerMappingTablesContext.CustomerMappingsToDelete)
				{
					var commands = GetCustomerMappingDeleteCommands(customerMappingsToDelete);
					customerMappingsWithCommands.Add(new CustomerMappingWithCommands() { Commands = commands, CustomerMappingWithActionType = new CustomerMappingWithActionType() { CustomerMappingSerialized = customerMappingsToDelete, ActionType = CustomerMappingActionType.Delete } });

					List<EricssonRouteWithCommands> ericssonRoutesWithCommands = routeCommandsByBo.GetOrCreateItem(customerMappingsToDelete.BO);
					ericssonRoutesWithCommands.Add(new EricssonRouteWithCommands() { Commands = commands, ActionType = RouteActionType.DeleteCustomer });
					if (!customersToDeleteByBO.ContainsKey(customerMappingsToDelete.BO))
						customersToDeleteByBO.Add(customerMappingsToDelete.BO, new CustomerMappingWithActionType() { CustomerMappingSerialized = customerMappingsToDelete, ActionType = CustomerMappingActionType.Delete });
				}
			}
			return customerMappingsWithCommands;
		}
		private List<string> GetCustomerMappingCommands(CustomerMappingSerialized customerMappingToAdd)
		{
			CustomerMapping customerMapping = Helper.DeserializeCustomerMapping(customerMappingToAdd.CustomerMappingAsString);
			List<string> customerMappingCommands = new List<string>();

			customerMappingCommands.Add(string.Format("{0}: BO={1}, NAPI=1, BNT=1, OBA={2};", EricssonCommands.PNBSI_Command, customerMapping.BO, customerMapping.InternationalOBA));
			customerMappingCommands.Add(string.Format("{0}: BO={1}, NAPI=1, BNT=4, OBA={2};", EricssonCommands.PNBSI_Command, customerMapping.BO, customerMapping.NationalOBA));

			if (customerMapping.InTrunks != null && customerMapping.InTrunks.Count > 0)
			{
				foreach (var trunk in customerMapping.InTrunks)
					customerMappingCommands.Add(string.Format("EXRBC:R={0}, BO:{1};", trunk.TrunkName, customerMapping.BO));
			}
			return customerMappingCommands;
		}
		private List<string> GetCustomerMappingDeleteCommands(CustomerMappingSerialized customerMappingToDelete)
		{
			return new List<string>() { string.Format("{0}: B={1};", EricssonCommands.ANBSE_Command, customerMappingToDelete.BO) };
		}
		#endregion

		#region Commands For RouteCase changes
		private List<RouteCaseWithCommands> GetRouteCasesWithCommands(List<RouteCase> routeCases)
		{
			if (routeCases == null || routeCases.Count == 0)
				return null;

			List<RouteCaseWithCommands> routeCasesWithCommands = new List<RouteCaseWithCommands>();

			foreach (var routeCaseToAdd in routeCases)
			{
				var commands = GetRouteCaseCommands(routeCaseToAdd);
				routeCasesWithCommands.Add(new RouteCaseWithCommands() { RouteCase = routeCaseToAdd, Commands = commands });
			}

			return routeCasesWithCommands;
		}
		private List<string> GetRouteCaseCommands(RouteCase routeCase)
		{
			List<string> routeCaseCommands = new List<string>();
			string command;
			routeCaseCommands.Add(string.Format("{0}: RC={1},CCH=NO;", EricssonCommands.ANRPI_Command, routeCase.RCNumber));
			var routeCaseOptions = Helper.DeserializeRouteCaseOptions(routeCase.RouteCaseOptionsAsString);
			if (routeCaseOptions != null || routeCaseOptions.Count > 0)
			{
				var optionGroups = routeCaseOptions.GroupBy(item => item.GroupID);
				int totalPercentage = 0;
				int priority = 1;
				int pValue = 1;
				foreach (var optionGroupKvp in optionGroups)
				{
					var option = optionGroupKvp.First();
					if (option != null)
						totalPercentage += option.Percentage.HasValue ? option.Percentage.Value : 0; ;
				}

				if (routeCaseOptions.Any(item => item.IsBackup) || totalPercentage == 100)
				{
					foreach (var optionGroup in optionGroups)
					{
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
			else routeCaseCommands.Add(string.Format("{0}: BR=CL-0&&-9&-11&&-15&TMR-0&-3, P01=1, R=BLK, BNT=1, SP=MM1;", EricssonCommands.ANRSI_Command));

			routeCaseCommands.Add(string.Format("{0};", EricssonCommands.ANRPE_Command));
			//routeCaseCommands.Add(string.Format("{0}:RC={1};", EricssonCommands.ANRAI_Command, routeCase.RCNumber));
			//routeCaseCommands.Add(";");
			return routeCaseCommands;
		}
		#endregion

		#region Commands For Route changes
		private void GetEricssonRoutesWithCommands(string switchId, Dictionary<string, CustomerMappingWithActionType> customersToDeleteByBO, Dictionary<string, List<EricssonRouteWithCommands>> ericssonRoutesWithCommands)
		{
			var routeCases = new RouteCaseManager().GetCachedRouteCasesGroupedByOptions(switchId).Values.ToList();
			if (routeCases == null || routeCases.Count == 0)
				throw new VRBusinessException("No Route Cases Found");

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

			foreach (var routeDifferencesKvp in routeCompareTablesContext.RouteDifferencesByBO)
			{
				var routeDifferences = routeDifferencesKvp.Value;
				var customerEricssonRoutesWithCommands = ericssonRoutesWithCommands.GetOrCreateItem(routeDifferencesKvp.Key);

				if (routeDifferences.RoutesToAdd != null && routeDifferences.RoutesToAdd.Count > 0)
				{
					foreach (var route in routeDifferences.RoutesToAdd)
					{
						var commands = GetRouteCommand(carrierMappingByCustomerBo, supplierByOutTrunk, routeCases, route);
						customerEricssonRoutesWithCommands.Add(new EricssonRouteWithCommands() { Route = route, Commands = commands, ActionType = RouteActionType.Add });
					}
				}

				if (routeDifferences.RoutesToUpdate != null && routeDifferences.RoutesToUpdate.Count > 0)
				{
					foreach (var route in routeDifferences.RoutesToUpdate)
					{
						var commands = GetRouteCommand(carrierMappingByCustomerBo, supplierByOutTrunk, routeCases, route);
						customerEricssonRoutesWithCommands.Add(new EricssonRouteWithCommands() { Route = route, Commands = commands, ActionType = RouteActionType.Update });
					}
				}

				if (routeDifferences.RoutesToDelete != null && routeDifferences.RoutesToDelete.Count > 0)
				{
					foreach (var route in routeDifferences.RoutesToDelete)
					{
						if (!customersToDeleteByBO.ContainsKey(route.BO))
						{
							var commands = GetDeletedRouteCommands(carrierMappingByCustomerBo, supplierByOutTrunk, routeCases, route);
							customerEricssonRoutesWithCommands.Add(new EricssonRouteWithCommands() { Route = route, Commands = commands, ActionType = RouteActionType.Delete });
						}
					}
				}
			}
		}
		private List<string> GetDeletedRouteCommands(Dictionary<string, CarrierMapping> carrierMappingByCustomerBo, Dictionary<string, int> supplierByOutTrunk, List<RouteCase> routeCases, EricssonConvertedRoute route)
		{
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

			string routeIBNT = "1";
			string routeNBNT = "4";
			string routeIOBA = carrierCustomerMapping.CustomerMapping.InternationalOBA;
			string routeNOBA = carrierCustomerMapping.CustomerMapping.NationalOBA;
			string routeM = LocalCountryCode.Length.ToString();
			string routeCC = null;
			string routeCCL = null;
			string routeL = null;
			string routeD = null;
			EricssonConvertedRouteType routeType = EricssonConvertedRouteType.Normal;
			string routeNationalM = null;
			string routeFBO = null;
			bool isOverride = false;
			bool isInterconnectOverride = false;

			GetRouteTypeAndParameters(supplierByOutTrunk, route, ref routeCodeGroup, carrierCustomerMapping, options, ref routeIBNT, ref routeNBNT, ref routeIOBA, ref routeNOBA, ref routeM, ref routeCC, ref routeCCL, ref routeL, ref routeD, ref routeType, ref routeNationalM, ref routeFBO, ref isOverride, ref isInterconnectOverride);

			switch (routeType)
			{
				case EricssonConvertedRouteType.Forward:
				case EricssonConvertedRouteType.Transit:
				case EricssonConvertedRouteType.Local:
					deletedRouteCommands.Add(string.Format("{0}:B={1}-{2};", EricssonCommands.ANBSE_Command, routeIOBA, route.Code));
					deletedRouteCommands.Add(string.Format("{0}:B={1}-{2};", EricssonCommands.ANBSE_Command, routeNOBA, route.Code.Substring(routeCodeGroup.Length)));
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
		private List<string> GetRouteCommand(Dictionary<string, CarrierMapping> carrierMappingByCustomerBo, Dictionary<string, int> supplierByOutTrunk, List<RouteCase> routeCases, EricssonConvertedRoute route)
		{
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

			string routeIBNT = "1";
			string routeNBNT = "4";
			string routeIOBA = carrierCustomerMapping.CustomerMapping.InternationalOBA;
			string routeNOBA = carrierCustomerMapping.CustomerMapping.NationalOBA;
			string routeM = LocalCountryCode.Length.ToString();
			string routeCC = null;
			string routeCCL = null;
			string routeL = null;
			string routeD = null;
			EricssonConvertedRouteType routeType = EricssonConvertedRouteType.Normal;
			string routeNationalM = null;
			string routeFBO = null;
			bool isOverride = false;
			bool isInterconnectOverride = false;

			GetRouteTypeAndParameters(supplierByOutTrunk, route, ref routeCodeGroup, carrierCustomerMapping, options, ref routeIBNT, ref routeNBNT, ref routeIOBA, ref routeNOBA, ref routeM, ref routeCC, ref routeCCL, ref routeL, ref routeD, ref routeType, ref routeNationalM, ref routeFBO, ref isOverride, ref isInterconnectOverride);

			routeCommands.Add(GetRouteCommandString(route, routeCodeGroup, routeNBNT, routeIOBA, routeNOBA, routeM, routeCC, routeCCL, routeL, routeD, routeType, routeNationalM, routeFBO));
			return routeCommands;
		}
		private void GetRouteTypeAndParameters(Dictionary<string, int> supplierByOutTrunk, EricssonConvertedRoute route, ref string routeCodeGroup, CarrierMapping carrierCustomerMapping, List<RouteCaseOption> options, ref string routeIBNT, ref string routeNBNT, ref string routeIOBA, ref string routeNOBA, ref string routeM, ref string routeCC, ref string routeCCL, ref string routeL, ref string routeD, ref EricssonConvertedRouteType routeType, ref string routeNationalM, ref string routeFBO, ref bool isOverride, ref bool isInterconnectOverride)
		{
			#region routeParameter and Types
			#region Manual Override
			var matchedmanualOverride = (ManualOverrides != null && ManualOverrides.Count > 0) ? ManualOverrides.FindRecord(item => item.BO == route.BO && item.Code == route.Code) : null;
			if (matchedmanualOverride != null)
			{
				isOverride = true;
				routeType = EricssonConvertedRouteType.Override;
				routeCCL = matchedmanualOverride.CCL;
				routeM = matchedmanualOverride.M;
				routeL = matchedmanualOverride.L;
				routeD = matchedmanualOverride.D;
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

				isInterconnectOverride = true;
				routeType = EricssonConvertedRouteType.InterconnectOverride;
				routeD = matchedInterconnectOverride.D;
				routeIBNT = matchedInterconnectOverride.IBNT;
				routeNBNT = matchedInterconnectOverride.NBNT;
				routeIOBA = matchedInterconnectOverride.IOBA;
				routeNOBA = matchedInterconnectOverride.NOBA;
				routeM = matchedInterconnectOverride.M;
				routeL = matchedInterconnectOverride.L;
				routeCCL = codeGroup.Length.ToString();
				routeCodeGroup = codeGroup;
				routeNationalM = matchedInterconnectOverride.NationalM;
				routeFBO = matchedInterconnectOverride.FBO;
				routeCC = matchedInterconnectOverride.CC;
				route.Code = matchedInterconnectOverride.Prefix;
				//R routePrefix = matchedInterconnectOverride.Prefix;
				//R routeIsActive = matchedInterconnectOverride.IsActive;
				//R routeTRD = matchedInterconnectOverride.TRD;
			}
			#endregion

			#region Local,Transit,Forward,Normal
			if (routeCodeGroup == LocalCountryCode && !isInterconnectOverride)
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
					routeType = EricssonConvertedRouteType.Forward;
					routeL = isOverride ? routeL : LocalNumberLength.ToString();
					routeNationalM = isOverride ? routeM : "";
					routeFBO = localSupplierMapping.BO;
					routeCCL = isOverride ? routeCCL : "";
				}
				else if (incomingTrafficSupplier != null)
				{
					routeType = EricssonConvertedRouteType.Local;
					routeL = isOverride ? routeL : LocalNumberLength.ToString();
					routeNationalM = isOverride ? routeM : "";
					routeFBO = routeNOBA;
					routeCCL = isOverride ? routeCCL : "";
					routeD = isOverride ? routeD : "4-0";
					routeCC = isOverride ? routeCC : "1";
				}
				else
				{
					routeType = EricssonConvertedRouteType.Transit;
					routeL = isOverride ? routeL : LocalNumberLength.ToString();
					routeNationalM = isOverride ? routeM : "0-" + LocalCountryCode;
					routeCCL = isOverride ? routeCCL : LocalCountryCode.Length.ToString();
					routeFBO = routeNOBA;
					routeD = isOverride ? routeD : "7-0";
					routeCC = isOverride ? routeCC : "3";
				}
			}
			else if (!isOverride && !isInterconnectOverride)
			{
				routeType = EricssonConvertedRouteType.Normal;
				routeL = MinCodeLength + "-" + MaxCodeLength;
				routeM = InterconnectGeneralPrefix;
				routeCCL = routeCodeGroup.StartsWith("1") ? "1" : routeCodeGroup.Length > 3 ? "3" : routeCodeGroup.Length.ToString();
				routeCC = "1";
				if (OutgoingTrafficCustomers.Any(item => item.CustomerId == carrierCustomerMapping.CarrierId))
					routeD = "6-0";
				else
					routeD = "7-0";
			}
			#endregion
			#endregion
		}
		private string GetRouteCommandString(EricssonConvertedRoute route, string routeCodeGroup, string routeNBNT, string routeIOBA, string routeNOBA, string routeM, string routeCC, string routeCCL, string routeL, string routeD, EricssonConvertedRouteType routeType, string routeNationalM, string routeFBO)
		{
			StringBuilder strCommand = new StringBuilder();
			string L = null;
			string B = null;

			switch (routeType)
			{
				case EricssonConvertedRouteType.Override:
					B = string.Format("{0}-{1}", route.BO, route.Code);

					L = routeL;
					if (!string.IsNullOrEmpty(routeL) && route.Code.Length > MinCodeLength)
						L = route.Code.Length.ToString() + "-" + MaxCodeLength.ToString();

					strCommand.Append(GetRouteCommandStringText(B, route.RCNumber, L, routeM, routeD, routeCC, routeCCL, null, null));
					break;

				case EricssonConvertedRouteType.Normal:
					B = string.Format("{0}-{1}", route.BO, route.Code);

					if (route.Code.Length > MinCodeLength)
						L = route.Code.Length.ToString() + "-" + MaxCodeLength.ToString();

					strCommand.Append(GetRouteCommandStringText(B, route.RCNumber, L, string.Format("0-{0}", routeM), routeD, routeCC, routeCCL, null, null));
					break;

				case EricssonConvertedRouteType.Forward:
					B = string.Format("{0}-{1}", routeIOBA, route.Code);
					strCommand.Append(GetRouteCommandStringText(B, null, null, routeM, null, null, null, routeFBO, routeNBNT));

					B = string.Format("{0}-{1}", routeNOBA, route.Code.Substring(routeCodeGroup.Length));
					strCommand.Append(GetRouteCommandStringText(B, null, null, routeNationalM, null, null, routeCCL, routeFBO, routeNBNT));
					break;

				case EricssonConvertedRouteType.Transit:
					B = string.Format("{0}-{1}", routeIOBA, route.Code);
					strCommand.Append(GetRouteCommandStringText(B, null, null, routeM, null, null, null, routeNOBA, routeNBNT));

					B = string.Format("{0}-{1}", routeNOBA, route.Code.Substring(routeCodeGroup.Length));
					strCommand.Append(GetRouteCommandStringText(B, route.RCNumber, routeL, string.IsNullOrEmpty(routeNationalM) ? routeM : routeNationalM, routeD, routeCC, routeCCL, null, null));
					break;

				case EricssonConvertedRouteType.Local:
					B = string.Format("{0}-{1}", routeIOBA, route.Code);
					strCommand.Append(GetRouteCommandStringText(B, null, null, routeM, null, null, null, routeNOBA, routeNBNT));

					B = string.Format("{0}-{1}", routeNOBA, route.Code.Substring(routeCodeGroup.Length));
					strCommand.Append(GetRouteCommandStringText(B, route.RCNumber, routeL, routeNationalM, routeD, routeCC, routeCCL, null, null));
					break;

				case EricssonConvertedRouteType.InterconnectOverride:
					B = string.Format("{0}-{1}{2}", route.BO, InterconnectGeneralPrefix, routeCodeGroup);
					strCommand.Append(GetRouteCommandStringText(B, route.RCNumber, routeL, routeM, routeD, routeCC, routeCCL, null, null));
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
		private static void LogEricssonRouteCommands(Dictionary<string, List<EricssonRouteWithCommands>> succeedEricssonRoutesWithCommandsByBo, Dictionary<string, List<EricssonRouteWithCommands>> failedEricssonRoutesWithCommandsByBo, SwitchLogger ftpLogger)
		{
			if (succeedEricssonRoutesWithCommandsByBo != null && succeedEricssonRoutesWithCommandsByBo.Count > 0)
			{
				foreach (var customerRoutesWithCommandsKvp in succeedEricssonRoutesWithCommandsByBo)
				{
					var customerRoutesWithCommands = customerRoutesWithCommandsKvp.Value;
					var commandResults = customerRoutesWithCommands.Select(item => new CommandResult() { Command = string.Join(Environment.NewLine, item.Commands) }).ToList();
					ILogRoutesContext logRoutesContext = new LogRoutesContext() { ExecutionDateTime = DateTime.Now, ExecutionStatus = ExecutionStatus.Succeeded, CommandResults = commandResults, BONumber = Convert.ToInt32(customerRoutesWithCommandsKvp.Key) };
					ftpLogger.LogRoutes(logRoutesContext);
				}
			}

			if (failedEricssonRoutesWithCommandsByBo != null && failedEricssonRoutesWithCommandsByBo.Count > 0)
			{
				foreach (var customerRoutesWithCommandsKvp in succeedEricssonRoutesWithCommandsByBo)
				{
					var customerRoutesWithCommands = customerRoutesWithCommandsKvp.Value;
					var commandResults = customerRoutesWithCommands.Select(item => new CommandResult() { Command = string.Join(Environment.NewLine, item.Commands) }).ToList();
					ILogRoutesContext logRoutesContext = new LogRoutesContext() { ExecutionDateTime = DateTime.Now, ExecutionStatus = ExecutionStatus.Failed, CommandResults = commandResults, BONumber = Convert.ToInt32(customerRoutesWithCommandsKvp.Key) };
					ftpLogger.LogRoutes(logRoutesContext);
				}
			}
		}

		private static void LogCustomerMappingCommands(List<CustomerMappingWithCommands> succeedCustomerMappingsWithCommands, List<CustomerMappingWithCommands> failedCustomerMappingsWithCommands, SwitchLogger ftpLogger)
		{
			if (succeedCustomerMappingsWithCommands != null && succeedCustomerMappingsWithCommands.Count > 0)
			{
				var commandResults = succeedCustomerMappingsWithCommands.Select(item => new CommandResult() { Command = string.Join(Environment.NewLine, item.Commands) }).ToList();
				ILogCarrierMappingsContext logRouteCasesContext = new LogCarrierMappingsContext() { ExecutionDateTime = DateTime.Now, ExecutionStatus = ExecutionStatus.Succeeded, CommandResults = commandResults };
				ftpLogger.LogCarrierMappings(logRouteCasesContext);
			}

			if (failedCustomerMappingsWithCommands != null && failedCustomerMappingsWithCommands.Count > 0)
			{
				var commandResults = failedCustomerMappingsWithCommands.Select(item => new CommandResult() { Command = string.Join(Environment.NewLine, item.Commands) }).ToList();
				ILogCarrierMappingsContext logRouteCasesContext = new LogCarrierMappingsContext() { ExecutionDateTime = DateTime.Now, ExecutionStatus = ExecutionStatus.Failed, CommandResults = commandResults };
				ftpLogger.LogCarrierMappings(logRouteCasesContext);
			}
		}

		private static void LogRouteCaseCommands(List<RouteCaseWithCommands> succeedRouteCasesWithCommands, List<RouteCaseWithCommands> failedRouteCasesWithCommands, SwitchLogger ftpLogger)
		{
			if (succeedRouteCasesWithCommands != null && succeedRouteCasesWithCommands.Count > 0)
			{
				var commandResults = succeedRouteCasesWithCommands.Select(item => new CommandResult() { Command = string.Join(Environment.NewLine, item.Commands) }).ToList();
				ILogRouteCasesContext logRouteCasesContext = new LogRouteCasesContext() { ExecutionDateTime = DateTime.Now, ExecutionStatus = ExecutionStatus.Succeeded, CommandResults = commandResults };
				ftpLogger.LogRouteCases(logRouteCasesContext);
			}

			if (failedRouteCasesWithCommands != null && failedRouteCasesWithCommands.Count > 0)
			{
				var commandResults = failedRouteCasesWithCommands.Select(item => new CommandResult() { Command = string.Join(Environment.NewLine, item.Commands) }).ToList();
				ILogRouteCasesContext logRouteCasesContext = new LogRouteCasesContext() { ExecutionDateTime = DateTime.Now, ExecutionStatus = ExecutionStatus.Failed, CommandResults = commandResults };
				ftpLogger.LogRouteCases(logRouteCasesContext);
			}
		}
		#endregion

		#endregion

	}
}