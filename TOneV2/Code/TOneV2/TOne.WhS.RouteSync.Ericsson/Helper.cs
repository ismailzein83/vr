using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.RouteSync.Ericsson.Entities;

namespace TOne.WhS.RouteSync.Ericsson
{
	public static class Helper
	{
		#region RouteCaseOptionsSerialization

		public const string RouteCaseFieldSeparatorAsString = "~#";

		public const string OptionsSeparatorAsString = "~@";

		public const string OptionFieldsSeparatorAsString = "~$";

		public const string BranchRouteSeparatorAsString = "~&";

		public const string BranchRouteFieldSeparatorAsString = "~*";

		public static string SerializeRouteCase(List<RouteCaseOption> routeCaseOptions, List<BranchRoute> branchRoutes)
		{
			string serializedRouteCaseOptions = SerializeRouteCaseOptions(routeCaseOptions);

			string serializedBranchRoutes = branchRoutes != null && branchRoutes.Count > 0 ? SerializeBranchRoutes(branchRoutes) : string.Empty;

			return string.Format("{1}{0}{2}", RouteCaseFieldSeparatorAsString, serializedRouteCaseOptions, serializedBranchRoutes);
		}

		public static DeserializedRouteCase DeserializeRouteCase(string serializedRouteCase)
		{
			DeserializedRouteCase result = new DeserializedRouteCase();
			if (string.IsNullOrEmpty(serializedRouteCase))
				return null;

			string[] serializedRouteCaseAsString = serializedRouteCase.Split(new string[] { RouteCaseFieldSeparatorAsString }, StringSplitOptions.None);
			if (serializedRouteCaseAsString == null || serializedRouteCaseAsString.Count() != 2)
				return null;

			string serializedRouteCaseOptions = serializedRouteCaseAsString[0];
			if (!string.IsNullOrEmpty(serializedRouteCaseOptions))
				result.RouteCaseOptions = DeserializeRouteCaseOptions(serializedRouteCaseOptions);

			string serializedBranchRoutes = serializedRouteCaseAsString[1];
			if (!string.IsNullOrEmpty(serializedBranchRoutes))
				result.BranchRoutes = DeserializeBranchRoutes(serializedBranchRoutes);

			return result;
		}


		public static string SerializeRouteCaseOptions(List<RouteCaseOption> routeCaseOptions)
		{
			if (routeCaseOptions == null || routeCaseOptions.Count == 0)
				return SerializeRouteCaseOption(GetBlockedRouteCaseOption());

			List<string> serializedOptions = new List<string>();
			foreach (RouteCaseOption routeCaseOption in routeCaseOptions)
			{
				string serializedRouteCaseOption = SerializeRouteCaseOption(routeCaseOption);
				if (!string.IsNullOrEmpty(serializedRouteCaseOption))
					serializedOptions.Add(serializedRouteCaseOption);
			}
			return String.Join<string>(OptionsSeparatorAsString, serializedOptions);
		}

		public static string SerializeRouteCaseOption(RouteCaseOption routeCaseOption)
		{
			if (routeCaseOption == null)
				return null;

			string percentage = routeCaseOption.Percentage.HasValue ? routeCaseOption.Percentage.Value.ToString() : string.Empty;
			string trunkPercentage = routeCaseOption.TrunkPercentage.HasValue ? routeCaseOption.TrunkPercentage.Value.ToString() : string.Empty;

			return string.Format("{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}{0}{8}{0}{9}", OptionFieldsSeparatorAsString, percentage, routeCaseOption.IsSwitch ? 1 : 0, routeCaseOption.OutTrunk, (int)routeCaseOption.Type,
				routeCaseOption.BNT, routeCaseOption.SP, trunkPercentage, routeCaseOption.IsBackup ? 1 : 0, routeCaseOption.GroupID);
		}

		public static List<RouteCaseOption> DeserializeRouteCaseOptions(string serializedRouteCaseOptions)
		{
			if (string.IsNullOrEmpty(serializedRouteCaseOptions))
				return null;

			string[] routeCaseOptionsAsString = serializedRouteCaseOptions.Split(new string[] { OptionsSeparatorAsString }, StringSplitOptions.None);
			if (routeCaseOptionsAsString == null)
				return null;

			List<RouteCaseOption> routeCaseOptions = new List<RouteCaseOption>();
			foreach (string serializedRouteCase in routeCaseOptionsAsString)
			{
				RouteCaseOption routeCaseOption = DeserializeRouteCaseOption(serializedRouteCase);
				if (routeCaseOption != null)
					routeCaseOptions.Add(routeCaseOption);
			}

			return routeCaseOptions;
		}

		public static RouteCaseOption DeserializeRouteCaseOption(string serializedRouteCaseOption)
		{
			if (string.IsNullOrEmpty(serializedRouteCaseOption))
				return null;

			string[] routeCaseOptionAsString = serializedRouteCaseOption.Split(new string[] { OptionFieldsSeparatorAsString }, StringSplitOptions.None);
			if (routeCaseOptionAsString == null || routeCaseOptionAsString.Count() != 9)
				return null;

			RouteCaseOption routeCaseOption = new RouteCaseOption();

			string percentage = routeCaseOptionAsString[0];
			if (!string.IsNullOrEmpty(percentage))
				routeCaseOption.Percentage = int.Parse(percentage);

			string isSwitch = routeCaseOptionAsString[1];
			if (!string.IsNullOrEmpty(isSwitch))
				routeCaseOption.IsSwitch = int.Parse(isSwitch) > 0;

			routeCaseOption.OutTrunk = routeCaseOptionAsString[2];

			string type = routeCaseOptionAsString[3];
			if (!string.IsNullOrEmpty(type))
				routeCaseOption.Type = (TrunkType)int.Parse(type);

			string bnt = routeCaseOptionAsString[4];
			if (!string.IsNullOrEmpty(bnt))
				routeCaseOption.BNT = int.Parse(bnt);

			string sp = routeCaseOptionAsString[5];
			if (!string.IsNullOrEmpty(sp))
				routeCaseOption.SP = short.Parse(sp);

			string trunkPercentage = routeCaseOptionAsString[6];
			if (!string.IsNullOrEmpty(trunkPercentage))
				routeCaseOption.TrunkPercentage = int.Parse(trunkPercentage);

			string isBackup = routeCaseOptionAsString[7];
			if (!string.IsNullOrEmpty(isBackup))
				routeCaseOption.IsBackup = int.Parse(isBackup) > 0;

			string groupID = routeCaseOptionAsString[8];
			if (!string.IsNullOrEmpty(groupID))
				routeCaseOption.GroupID = int.Parse(groupID);

			return routeCaseOption;
		}

		public static RouteCaseOption GetBlockedRouteCaseOption()
		{
			return new RouteCaseOption()
			{
				BNT = 1,
				SP = 1,
				OutTrunk = "BLK",
				GroupID = 1,
			};
		}

		#region branch routes
		public static string SerializeBranchRoutes(List<BranchRoute> branchRoutes)
		{
			if (branchRoutes == null || branchRoutes.Count == 0)
				return null;

			List<string> serializedBranchRoutes = new List<string>();
			foreach (BranchRoute branchRoute in branchRoutes)
			{
				string serializedBranchRoute = SerializeBranchRoute(branchRoute);
				if (!string.IsNullOrEmpty(serializedBranchRoute))
					serializedBranchRoutes.Add(serializedBranchRoute);
			}
			return String.Join<string>(BranchRouteSeparatorAsString, serializedBranchRoutes);
		}

		public static string SerializeBranchRoute(BranchRoute branchRoute)
		{
			if (branchRoute == null)
				return null;

			return string.Format("{1}{0}{2}{0}{3}", BranchRouteFieldSeparatorAsString, branchRoute.Name, branchRoute.IncludeTrunkAsSwitch, branchRoute.AlternativeName);
		}

		public static List<BranchRoute> DeserializeBranchRoutes(string serializedBranchRoutes)
		{
			if (string.IsNullOrEmpty(serializedBranchRoutes))
				return null;

			string[] branchRoutesAsString = serializedBranchRoutes.Split(new string[] { BranchRouteSeparatorAsString }, StringSplitOptions.None);
			if (branchRoutesAsString == null)
				return null;

			List<BranchRoute> branchRoutes = new List<BranchRoute>();
			foreach (string serializedBranchRoute in branchRoutesAsString)
			{
				BranchRoute branchRoute = DeserializeBranchRoute(serializedBranchRoute);
				if (branchRoute != null)
					branchRoutes.Add(branchRoute);
			}

			return branchRoutes;
		}

		public static BranchRoute DeserializeBranchRoute(string serializedBranchRoute)
		{
			if (string.IsNullOrEmpty(serializedBranchRoute))
				return null;

			string[] branchRouteAsString = serializedBranchRoute.Split(new string[] { BranchRouteFieldSeparatorAsString }, StringSplitOptions.None);
			if (branchRouteAsString == null || branchRouteAsString.Count() != 3)
				return null;

			BranchRoute branchRoute = new BranchRoute();

			branchRoute.Name = branchRouteAsString[0];
			string includeTrunkAsSwitch = branchRouteAsString[1];
			if (!string.IsNullOrEmpty(includeTrunkAsSwitch))
				branchRoute.IncludeTrunkAsSwitch = bool.Parse(includeTrunkAsSwitch);
			branchRoute.AlternativeName = branchRouteAsString[2];

			return branchRoute;
		}

		#endregion

		#endregion

		#region CustomerMappingSerialization
		public const char CustomerMappingPropertySeperator = '~';
		public const string CustomerMappingPropertySeperatorAsString = "~";
		public const char CustomerMappingTrunkSeperator = '#';
		public const string CustomerMappingTrunkSeperatorAsString = "#";
		public const char CustomerMappingTrunkPropertySeperator = '$';
		public const string CustomerMappingTrunkPropertySeperatorAsString = "$";

		public static string SerializeCustomerMapping(CustomerMapping customerMapping)
		{
			if (customerMapping == null)
				return null;
			//string trunksAsString = null;
			//if (customerMapping.InTrunks != null && customerMapping.InTrunks.Count > 0)
			//{
			//	List<string> trunks = new List<string>();
			//	foreach (var trunk in customerMapping.InTrunks.OrderBy(item => item.TrunkId))
			//	{
			//		trunks.Add(string.Format("{1}{0}{2}{0}{3}", CustomerMappingTrunkPropertySeperatorAsString, trunk.TrunkId, trunk.TrunkName, Convert.ToInt32(trunk.TrunkType)));
			//	}
			//	trunksAsString = string.Join(CustomerMappingTrunkSeperatorAsString, trunks);
			//}
			//return string.Format("{1}{0}{2}{0}{3}{0}{4}", CustomerMappingPropertySeperatorAsString, customerMapping.BO, customerMapping.NationalOBA, customerMapping.InternationalOBA, trunksAsString);
			return string.Format("{1}{0}{2}{0}{3}", CustomerMappingPropertySeperatorAsString, customerMapping.BO, customerMapping.NationalOBA, customerMapping.InternationalOBA);
		}

		public static CustomerMapping DeserializeCustomerMapping(string serializedCustomerMapping)
		{
			if (string.IsNullOrEmpty(serializedCustomerMapping))
				return null;

			string[] customerMappingPropertiesAsString = serializedCustomerMapping.Split(CustomerMappingPropertySeperator);
			if (customerMappingPropertiesAsString == null || customerMappingPropertiesAsString.Count() != 3)
				return null;

			CustomerMapping customerMapping = new CustomerMapping();
			customerMapping.BO = customerMappingPropertiesAsString[0];
			customerMapping.NationalOBA = customerMappingPropertiesAsString[1];
			customerMapping.InternationalOBA = customerMappingPropertiesAsString[2];
			//customerMapping.InTrunks = new List<InTrunk>();

			//string trunksAsString = customerMappingPropertiesAsString[3];
			//if (!string.IsNullOrEmpty(trunksAsString))
			//{
			//	string[] trunks = trunksAsString.Split(CustomerMappingTrunkSeperator);
			//	if (trunks != null && trunks.Any())
			//	{
			//		foreach (var trunkAsString in trunks)
			//		{
			//			if (string.IsNullOrEmpty(trunkAsString))
			//				continue;
			//			string[] trunkProperties = trunkAsString.Split(CustomerMappingTrunkPropertySeperator);
			//			if (trunkProperties == null || !trunkProperties.Any())
			//				continue;
			//			InTrunk trunk = new InTrunk();
			//			trunk.TrunkId = Guid.Parse(trunkProperties[0]);
			//			trunk.TrunkName = trunkProperties[1];
			//			trunk.TrunkType = (TrunkType)int.Parse(trunkProperties[2]);
			//			//customerMapping.InTrunks.Add(trunk);
			//		}
			//	}
			//}

			//if (customerMapping.InTrunks.Count == 0)
			//	customerMapping.InTrunks = null;

			return customerMapping;
		}
		#endregion
	}
}
