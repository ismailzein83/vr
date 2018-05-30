using System;
using System.Collections.Generic;

namespace TOne.WhS.RouteSync.Ericsson.Entities
{
	public static class EricssonCommands
	{
		public static string ANRPI_Command = "ANRPI";

		public static string ANRSI_Command = "ANRSI";

		public static string ANRPE_Command = "ANRPE";

		public static string ANRAI_Command = "ANRAI";

		public static string ANBSI_Command = "ANBSI";

		public static string EXRBC_Command = "EXRBC";

		public static string ANBAI_Command = "ANBAI";

		public static string PNBSI_Command = "PNBSI";

		public static string PNBAI_Command = "PNBAI";

		public static string PNBZI_Command = "PNBZI";

		public static string PNBCI_Command = "PNBCI";

		public static string ANBAR_Command = "ANBAR";

		public static string ANBSE_Command = "ANBSE";

		public static string PNBAR_Command = "PNBAR";

		public static string ANRAR_Command = "ANRAR";

		public static string ANRZI_Command = "ANRZI";

		public static string ANBZI_Command = "ANBZI";

		public static string ANBCI_Command = "ANBCI";

		public static string Exit_Command = "exit;";

		public static string MML_Command = "mml";

		public static string PROTECTION_PERIOD_ELAPSED = "PROTECTION_PERIOD_ELAPSED";

		public static string PROTECTIVE_PERIOD_ELAPSED = "PROTECTIVE PERIOD ELAPSED";

		public static string EXECUTED = "EXECUTED";

		public static string ORDERED = "ORDERED";

		public static string FUNCTION_BUSY = "FUNCTION BUSY";

	}

	#region Commands Classes
	public class RouteCaseWithCommands
	{
		public RouteCase RouteCase { get; set; }
		public List<string> Commands { get; set; }
	}

	public class EricssonRouteWithCommands
	{
		public EricssonConvertedRouteCompareResult RouteCompareResult { get; set; }
		public List<string> Commands { get; set; }
		public RouteActionType ActionType { get; set; }
	}

	public class CustomerMappingWithCommands
	{
		public CustomerMappingWithActionType CustomerMappingWithActionType { get; set; }
		public List<string> OBACommands { get; set; }
		//public Dictionary<Guid, string> TrunkCommandsByTrunkId { get; set; }
	}
	public class CustomerMappingWithActionType
	{
		public CustomerMapping CustomerMapping { get; set; }
		public CustomerMapping CustomerMappingOldValue { get; set; }
		public CustomerMappingActionType ActionType { get; set; }
	}
	#endregion
}