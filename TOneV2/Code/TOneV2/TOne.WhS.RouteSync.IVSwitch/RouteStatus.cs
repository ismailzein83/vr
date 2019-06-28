using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NP.IVSwitch.Entities;

namespace TOne.WhS.RouteSync.IVSwitch
{
	public class RouteStatus
	{
		public int RouteId { get; set; }
		public State Status { get; set; }
	}
}
