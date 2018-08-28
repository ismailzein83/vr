using System.Collections.Generic;
using TOne.WhS.RouteSync.Ericsson.Entities;
using Vanrise.Common.Business;

namespace TOne.WhS.RouteSync.Ericsson.Business
{
	public class BranchRouteSettingsManager
	{
		public IEnumerable<BranchRouteSettingsConfig> GetEricssonBranchRouteSettingsConfigs()
		{
			ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
			return manager.GetExtensionConfigurations<BranchRouteSettingsConfig>(BranchRouteSettingsConfig.EXTENSION_TYPE);
		}
	}
}
