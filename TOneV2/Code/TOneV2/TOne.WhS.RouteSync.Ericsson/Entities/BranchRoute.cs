using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.RouteSync.Ericsson.Entities
{
	public class BranchRoute
	{
		public string Name { get; set; }
		public string AlternativeName { get; set; }
		public bool IncludeTrunkAsSwitch { get; set; }
	}
}
