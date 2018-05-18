using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.RouteSync.Entities
{
	public class RouteSyncSettings : Vanrise.Entities.SettingData
	{
		public const string SETTING_TYPE = "WhS_RouteSync_Settings";

		public RouteSyncProcess RouteSyncProcess { get; set; }

		public Dictionary<Guid, RouteSynchronizerSwitchSettings> SwitchSettingsByConfigId { get; set; }

	}

	public class RouteSyncProcess
	{
		public int RouteBatchSize { get; set; }

		public int DifferentialRoutesPerTransaction { get; set; }

		public int IndexCommandTimeoutInMinutes { get; set; }

		public bool ExecuteFullRouteSyncWhenPartialNotSupported { get; set; }
	}

	public abstract class RouteSynchronizerSwitchSettings
	{
	}
}
