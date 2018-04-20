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

namespace TOne.WhS.RouteSync.Ericsson
{
	public class EricssonSWSync : SwitchRouteSynchronizer
	{
		public override Guid ConfigId { get { return new Guid("94739CBC-00A7-4CEB-9285-B4CB35D7D003"); } }
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
		public List<SwitchCommunication> SwitchCommunicationList { get; set; }
		public int LastRCNumber { get; set; }

		#region Public Methods

		public override void Initialize(ISwitchRouteSynchronizerInitializeContext context)
		{
			IWhSRouteSyncEricssonDataManager whSRouteSyncEricssonDataManager = RouteSyncEricssonDataManagerFactory.GetDataManager<IWhSRouteSyncEricssonDataManager>();
			whSRouteSyncEricssonDataManager.SwitchId = context.SwitchId;
			whSRouteSyncEricssonDataManager.Initialize(new WhSRouteSyncEricssonInitializeContext());

			IRouteDataManager routeDataManager = RouteSyncEricssonDataManagerFactory.GetDataManager<IRouteDataManager>();
			routeDataManager.SwitchId = context.SwitchId;
			routeDataManager.Initialize(new RouteInitializeContext());

			IRouteCaseDataManager routeCaseDataManager = RouteSyncEricssonDataManagerFactory.GetDataManager<IRouteCaseDataManager>();
			routeCaseDataManager.SwitchId = context.SwitchId;
			routeCaseDataManager.Initialize(new RouteCaseInitializeContext());

			if (CarrierMappings == null)//Check Null
				return;
			CustomerMappingManager CustomerMappingManager = new CustomerMappingManager();
			CustomerMappingManager.Initialize(context.SwitchId, CarrierMappings.Values);
		}
		public override void ConvertRoutes(ISwitchRouteSynchronizerConvertRoutesContext context)
		{
			if (context.Routes == null || context.Routes.Count == 0)//Check With Anthony
				return;

			var convertedRoutes = new List<ConvertedRoute>();
			var routesToConvertByRCString = new Dictionary<string, List<EricssonConvertedRoute>>();
			var routeCases = new RouteCaseManager().GetCachedRouteCasesGroupedByOptions(context.SwitchId);
			var routeCasesToAdd = new HashSet<string>();

			foreach (var route in context.Routes)
			{
				CarrierMappings.ThrowIfNull("No CarrierMappings found for customer with id:{0}");//Check With Anthony
				var customerCarrierMapping = CarrierMappings.FindRecord(item => item.CarrierId == int.Parse(route.CustomerId));
				customerCarrierMapping.ThrowIfNull(string.Format("No CarrierMappings found for customer with id:{0}", route.CustomerId));
				var customerMapping = customerCarrierMapping.CustomerMapping;
				customerMapping.ThrowIfNull(string.Format("No CustomerMapping found for customer with id:{0}", route.CustomerId));

				var codeGroupObject = new CodeGroupManager().GetMatchCodeGroup(route.Code);
				codeGroupObject.ThrowIfNull(string.Format("No CodeGroup found for code {0}.", route.Code));
				string routeCodeGroup = codeGroupObject.Code;

				EricssonConvertedRoute ericssonConvertedRoute = new EricssonConvertedRoute() { BO = customerMapping.BO, Code = route.Code};

				List<RouteCaseOption> routeCaseOptions = GetRouteCaseOptions(route, routeCodeGroup);
				var routeCaseOptionsAsString = Helper.SerializeRouteCaseOptions(routeCaseOptions);
				routeCaseOptionsAsString.ThrowIfNull("No Route Case Options");//Check if we need this exception

				RouteCase routeCase;
				if (routeCases.TryGetValue(routeCaseOptionsAsString, out routeCase))
				{
					ericssonConvertedRoute.RCNumber = routeCase.RCNumber;
					convertedRoutes.Add(ericssonConvertedRoute);
				}
				else
				{
					List<EricssonConvertedRoute> convertedRoutesByRouteCaseOption = routesToConvertByRCString.GetOrCreateItem(routeCaseOptionsAsString);
					convertedRoutesByRouteCaseOption.Add(ericssonConvertedRoute);
					routeCasesToAdd.Add(routeCaseOptionsAsString);
				}
			}

			routeCases = new RouteCaseManager().InsertAndGetRouteCases(context.SwitchId, routeCasesToAdd);

			foreach (var routesToConverKvp in routesToConvertByRCString)
			{
				RouteCase routeCase;
				if (!routeCases.TryGetValue(routesToConverKvp.Key, out routeCase))
					throw new Exception();
				var routes = routesToConverKvp.Value.Select(item => { item.RCNumber = routeCase.RCNumber; return item; });
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
			{
				routeDataManager.WriteRecordToStream((EricssonConvertedRoute)convertedRoute, dbApplyStream);
			}
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
			Dictionary<string, List<RouteCommands>> routeCommandsByBo = new Dictionary<string, List<RouteCommands>>();
			List<string> deletedBOs = new List<string>();

			var routeCases = new RouteCaseManager().GetCachedRouteCasesGroupedByOptions(context.SwitchId).FindAllRecords(item => item.RCNumber > LastRCNumber).ToList();

			List<CustomerMappingCommands> customerMappingsCommands = GetCustomerMappingsCommands(context.SwitchId, deletedBOs, routeCommandsByBo);
			List<RouteCaseCommands> routeCasesCommands = GetRouteCasesCommands(routeCases);
			GetRoutesCommands(context.SwitchId, deletedBOs, routeCommandsByBo);
		}

		#region Commands From CustomerMapping changes
		private void GetRoutesCommands(string switchId, List<string> deletedBOs, Dictionary<string, List<RouteCommands>> routeCommandsByBo)
		{
			var routeCases = new RouteCaseManager().GetCachedRouteCasesGroupedByOptions(switchId).Values.ToList();
			Dictionary<string, CarrierMapping> carrierMappingByCustomerBo = new Dictionary<string, CarrierMapping>();
			Dictionary<string, int> supplierByOutTrunk = new Dictionary<string, int>();
			foreach (var carrierMappingKvp in CarrierMappings)
			{
				var carrierMapping = carrierMappingKvp.Value;
				if (carrierMapping.CustomerMapping != null)
				{
					carrierMappingByCustomerBo.Add(carrierMapping.CustomerMapping.BO, carrierMapping);
				}
				if (carrierMapping.SupplierMapping != null)
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
				var routeCommands = routeCommandsByBo.GetOrCreateItem(routeDifferencesKvp.Key);

				foreach (var route in routeDifferences.RoutesToAdd)
					routeCommands.Add(GetRouteCommand(carrierMappingByCustomerBo, supplierByOutTrunk, routeCases, route));

				foreach (var route in routeDifferences.RoutesToUpdate)
					routeCommands.Add(GetRouteCommand(carrierMappingByCustomerBo, supplierByOutTrunk, routeCases, route));

				foreach (var route in routeDifferences.RoutesToDelete)
				{
					if (!deletedBOs.Contains(route.BO))
						routeCommands.Add(GetDeletedRouteCommands(carrierMappingByCustomerBo, supplierByOutTrunk, routeCases, route));
				}
			}
		}
		private RouteCommands GetDeletedRouteCommands(Dictionary<string, CarrierMapping> carrierMappingByCustomerBo, Dictionary<string, int> supplierByOutTrunk, List<RouteCase> routeCases, EricssonConvertedRoute route)
		{
			RouteCommands deletedRouteCommands = new RouteCommands();

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
		private RouteCommands GetRouteCommand(Dictionary<string, CarrierMapping> carrierMappingByCustomerBo, Dictionary<string, int> supplierByOutTrunk, List<RouteCase> routeCases, EricssonConvertedRoute route)
		{
			RouteCommands routeCommands = new RouteCommands();
			#region GetCodeGroup
			var codeGroupObject = new CodeGroupManager().GetMatchCodeGroup(route.Code);
			codeGroupObject.ThrowIfNull(string.Format("CodeGroup not found for code {0}.", route.Code));
			string routeCodeGroup = codeGroupObject.Code;
			#endregion

			#region getCustomerMapping
			CarrierMapping carrierCustomerMapping;
			if (!carrierMappingByCustomerBo.TryGetValue(route.BO, out carrierCustomerMapping))
			{
				carrierCustomerMapping.ThrowIfNull(string.Format("No customer mapping found with BO: {0}.", route.BO));
			}

			/*var carrierMappings = CarrierMappings.FindAllRecords(item => item.CustomerMapping != null);
			carrierMappings.ThrowIfNull(string.Format("No customer mapping found with BO: {0}.", route.BO));
			var carrierMapping = carrierMappings.FindRecord(item => item.CustomerMapping.BO == route.BO);
			carrierMapping.ThrowIfNull(string.Format("No customer mapping found with BO: {0}.", route.BO));
			var customerMapping = carrierMapping.CustomerMapping;
			customerMapping.ThrowIfNull(string.Format("No customer mapping found with BO: {0}.", route.BO));*/
			#endregion

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
				if (options == null || !options.Any())
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

		#region Commands From CustomerMapping changes
		private List<RouteCaseCommands> GetRouteCasesCommands(List<RouteCase> routeCases)
		{
			List<RouteCaseCommands> routeCasesCommands = new List<RouteCaseCommands>();

			foreach (var routeCaseToAdd in routeCases)
				routeCasesCommands.Add(GetRouteCaseCommands(routeCaseToAdd));

			return routeCasesCommands;
		}

		private RouteCaseCommands GetRouteCaseCommands(RouteCase routeCase)
		{
			RouteCaseCommands routeCaseCommands = new RouteCaseCommands();
			string command;
			routeCaseCommands.Add(string.Format("{0}: RC={1},CCH=NO;", EricssonCommands.ANRPI_Command, routeCase.RCNumber));
			var routeCaseOptions = Helper.DeserializeRouteCaseOptions(routeCase.RouteCaseOptionsAsString);
			if (routeCaseOptions == null || routeCaseOptions.Count == 0)
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
			//routeCaseCommands.Add(string.Format("{0}:	RC={1};", EricssonCommands.ANRAI_Command, routeCase.RCNumber));
			return routeCaseCommands;
		}
		#endregion

		#region Commands From CustomerMapping changes
		private List<CustomerMappingCommands> GetCustomerMappingsCommands(string switchId, List<string> deletedBOs, Dictionary<string, List<RouteCommands>> routeCommandsByBo)
		{
			CustomerMappingTablesContext customerMappingTablesContext = new CustomerMappingTablesContext();
			ICustomerMappingDataManager customerMappingDataManager = RouteSyncEricssonDataManagerFactory.GetDataManager<ICustomerMappingDataManager>();
			customerMappingDataManager.SwitchId = switchId;
			customerMappingDataManager.CompareTables(customerMappingTablesContext);
			List<CustomerMappingCommands> customerMappingsCommands = new List<CustomerMappingCommands>();

			if (customerMappingTablesContext.CustomerMappingsToAdd == null || customerMappingTablesContext.CustomerMappingsToAdd.Count == 0)
			{
				foreach (var customerMappingToAdd in customerMappingTablesContext.CustomerMappingsToAdd)
					customerMappingsCommands.Add(GetCustomerMappingCommands(customerMappingToAdd));
			}

			if (customerMappingTablesContext.CustomerMappingsToUpdate == null || customerMappingTablesContext.CustomerMappingsToUpdate.Count == 0)
			{
				foreach (var customerMappingsToUpdate in customerMappingTablesContext.CustomerMappingsToUpdate)
				{
					customerMappingsCommands.Add(GetCustomerMappingCommands(customerMappingsToUpdate));
				}
			}

			if (customerMappingTablesContext.CustomerMappingsToDelete == null || customerMappingTablesContext.CustomerMappingsToDelete.Count == 0)
			{
				foreach (var customerMappingsToDelete in customerMappingTablesContext.CustomerMappingsToDelete)
				{
					List<RouteCommands> routeCommands = routeCommandsByBo.GetOrCreateItem(customerMappingsToDelete.BO);
					routeCommands.Add(GetCustomerMappingDeleteCommands(customerMappingsToDelete));
					deletedBOs.Add(customerMappingsToDelete.BO);
				}
			}
			return customerMappingsCommands;
		}

		private CustomerMappingCommands GetCustomerMappingCommands(CustomerMappingSerialized customerMappingToAdd)
		{
			CustomerMapping customerMapping = CustomerMappingManager.DeserializeCustomerMapping(customerMappingToAdd.CustomerMappingAsString);
			CustomerMappingCommands customerMappingCommands = new CustomerMappingCommands();

			//customerMappingCommands.Add(string.Format("{0};", EricssonCommands.PNBZI_Command));
			//customerMappingCommands.Add(string.Format("{0};", EricssonCommands.PNBCI_Command));
			customerMappingCommands.Add(string.Format("{0}: BO={1}, NAPI=1, BNT=1, OBA={2};", EricssonCommands.PNBSI_Command, customerMapping.BO, customerMapping.InternationalOBA));
			customerMappingCommands.Add(string.Format("{0}: BO={1}, NAPI=1, BNT=4, OBA={2};", EricssonCommands.PNBSI_Command, customerMapping.BO, customerMapping.NationalOBA));

			foreach (var trunk in customerMapping.InTrunks)
			{
				customerMappingCommands.Add(string.Format("EXRBC:R={0}, BO:{1};", trunk.TrunkName, customerMapping.BO));
			}

			//customerMappingCommands.Add(string.Format("{0};", EricssonCommands.PNBAI_Command));
			return customerMappingCommands;
		}

		private RouteCommands GetCustomerMappingDeleteCommands(CustomerMappingSerialized customerMappingToDelete)
		{
			RouteCommands deleteRouteCommand = new RouteCommands();

			//customerMappingDeleteCommands.Add(string.Format("{0};", EricssonCommands.ANBAR_Command));
			deleteRouteCommand.Add(string.Format("{0}: B={1};", EricssonCommands.ANBSE_Command, customerMappingToDelete.BO));
			//customerMappingDeleteCommands.Add(string.Format("{0};", EricssonCommands.ANBAI_Command));

			return deleteRouteCommand;
		}
		#endregion

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

		private List<RouteCaseOption> GetRouteCaseOptions(Route route, string codeGroup)
		{
			List<RouteCaseOption> routeCaseOptions = new List<RouteCaseOption>();
			var options = route.Options.Take(NumberOfOptions);
			foreach (var option in options)
			{
				#region getSupplierMapping
				var supplierCarrierMapping = CarrierMappings.FindRecord(item => item.CarrierId == int.Parse(option.SupplierId));
				supplierCarrierMapping.ThrowIfNull("CarrierMappings", option.SupplierId);
				var supplierMapping = supplierCarrierMapping.SupplierMapping;
				supplierMapping.ThrowIfNull("SupplierMapping", option.SupplierId);
				#endregion

				var trunkGroups = supplierMapping.TrunkGroups.FindAllRecords(item => item.CustomerTrunkGroups.Any(ctgItem => ctgItem.CustomerId == int.Parse(route.CustomerId)));
				var trunkGroup = trunkGroups.FindRecord(item => item.CodeGroupTrunkGroups.Any(cg => cg.CodeGroup == codeGroup));
				//var trunkGroup = Helper.GetMatchedTrunkGroup(trunkGroups, route.Code);

				foreach (var trunkGroupTrunk in trunkGroup.TrunkTrunkGroups)
					routeCaseOptions.Add(GetRouteCaseOption(codeGroup, option, supplierMapping, trunkGroup, trunkGroupTrunk));
			}

			routeCaseOptions = routeCaseOptions.OrderBy(item => item.Priority).ToList();
			return routeCaseOptions;
		}

		private RouteCaseOption GetRouteCaseOption(string routeCodeGroup, RouteOption option, SupplierMapping supplierMapping, TrunkGroup trunkGroup, TrunkTrunkGroup trunkGroupTrunk)
		{
			var trunk = supplierMapping.OutTrunks.FindRecord(item => item.TrunkId == trunkGroupTrunk.TrunkId);
			RouteCaseOption routeCaseOption = new RouteCaseOption();
			routeCaseOption.Percentage = option.Percentage;
			routeCaseOption.Priority = trunkGroupTrunk.Priority;
			routeCaseOption.OutTrunk = trunk.TrunkName;
			routeCaseOption.Type = trunk.TrunkType;
			routeCaseOption.TrunkPercentage = trunkGroupTrunk.Percentage;
			routeCaseOption.IsBackup = trunkGroup.IsBackup;
			routeCaseOption.GroupID = supplierMapping.TrunkGroups.IndexOf(trunkGroup);
			routeCaseOption.BNT = 1;
			routeCaseOption.SP = 1;

			if (routeCodeGroup == LocalCountryCode)
			{
				if (trunk.TrunkName.StartsWith(InterconnectGeneralPrefix) || IncomingTrafficSuppliers.Any(item => item.SupplierId == int.Parse(option.SupplierId)))
				{
					routeCaseOption.BNT = 4;
					routeCaseOption.SP = Convert.ToInt16(LocalCountryCode.Length + 1);
				}
			}
			return routeCaseOption;
		}
	}
}