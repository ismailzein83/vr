using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NP.IVSwitch.Entities
{
	public interface IRouteManager : IBEManager
	{
		bool IsCacheExpired(ref DateTime? lastCheckTime);
		List<int> GetCarrierAccountRouteIds(int carrierAccountId);
		Route GetRoute(int routeId);
		void SetCacheExpired();
		List<Route> GetCarrierAccountRoutes(int carrierAccountId);

	}
}
