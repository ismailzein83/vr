using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.RouteSync.Entities;

namespace TOne.WhS.RouteSync.Ericsson
{
	public class EricssonSwitchRouteSynchronizerSettings : RouteSynchronizerSwitchSettings
	{
		public int NumberOfRetries { get; set; }
		public List<string> FaultCodes { get; set; }
	}
}
