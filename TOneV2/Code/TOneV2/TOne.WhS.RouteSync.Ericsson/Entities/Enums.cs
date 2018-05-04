using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.RouteSync.Ericsson
{
	public enum TrunkType { TDM = 0, IP = 1 }

	public enum CustomerMappingActionType { Add = 0, Update = 1, Delete = 2 }
	
	public enum RouteActionType { Add = 0, Update = 1, Delete = 2, DeleteCustomer = 3 }
}
