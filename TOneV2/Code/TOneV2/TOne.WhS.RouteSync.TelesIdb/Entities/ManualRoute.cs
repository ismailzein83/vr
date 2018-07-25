using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.RouteSync.TelesIdb.Entities
{
	public class ManualRoute
	{
		public string Pref { get; set; }
		public string Route { get; set; }
		public bool IncludeSubcodes { get; set; }
		public string Note { get; set; }
	}
}
