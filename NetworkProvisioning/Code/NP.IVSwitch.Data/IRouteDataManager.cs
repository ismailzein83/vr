using NP.IVSwitch.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NP.IVSwitch.Data
{
	public interface IRouteDataManager : IDataManager
	{
		List<Route> GetRoutes();
		bool Update(Route route);
		int? Insert(Route route);
		bool UpdateVendorUSer(Route route);
		DateTime GetSwitchDateTime();
		int GetGlobalTariffTableId();
		bool DeleteRoute(Route route);
		bool BlockRoute(Route route);
		bool InActivateRoute(Route route);
		bool ActivateRoute(Route route);

	}
}
