using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Routing.Data;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.Business
{
	public class SwitchSyncDataManager : ISwitchSyncDataManager
	{
		public void ResetSwitchSyncData(string switchid)
		{
			if (!string.IsNullOrEmpty(switchid))
			{
				RoutingDatabase routingDatabase = new RoutingDatabaseManager().GetLatestRoutingDatabase(RoutingProcessType.CustomerRoute, RoutingDatabaseType.Current);
				ISwitchSyncDataDataManager dataManager = RoutingDataManagerFactory.GetDataManager<ISwitchSyncDataDataManager>();
				dataManager.RoutingDatabase = routingDatabase;
				dataManager.ResetSwitchSyncData(switchid);
			}
		}

	}
}
