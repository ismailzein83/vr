using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TOne.WhS.RouteSync.Ericsson.Data;
using TOne.WhS.RouteSync.Ericsson.Entities;
using Vanrise.Caching;
using Vanrise.Runtime;

namespace TOne.WhS.RouteSync.Ericsson.Business
{
	public class RouteCaseManager
	{
		public List<RouteCase> GetAllRouteCases(string switchId)
		{
			IRouteCaseDataManager dataManager = RouteSyncEricssonDataManagerFactory.GetDataManager<IRouteCaseDataManager>();
			dataManager.SwitchId = switchId;
			return dataManager.GetAllRouteCases();
		}

		public List<RouteCase> GetNotSyncedRouteCases(string switchId)
		{
			IRouteCaseDataManager dataManager = RouteSyncEricssonDataManagerFactory.GetDataManager<IRouteCaseDataManager>();
			dataManager.SwitchId = switchId;
			return dataManager.GetNotSyncedRouteCases();
		}
	}
}
