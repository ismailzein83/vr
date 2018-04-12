using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.RouteSync.Entities;

namespace TOne.WhS.RouteSync.Ericsson
{
    public class EricssonConvertedRoute : ConvertedRoute
    {
		public string BO { get; set; }
		public string Code { get; set; }
		public string RCNumber { get; set; }
	}
}
