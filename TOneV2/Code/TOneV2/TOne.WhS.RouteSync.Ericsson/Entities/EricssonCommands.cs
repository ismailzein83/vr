using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
	}

	#region Commands Classes
	public class CustomerMappingCommands : List<string> { }
	public class RouteCaseCommands : List<string> { }
	public class RouteCommands : List<string> { }
	#endregion
}
