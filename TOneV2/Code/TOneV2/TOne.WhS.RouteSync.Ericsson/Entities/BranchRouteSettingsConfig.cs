using System;
using Vanrise.Entities;

namespace TOne.WhS.RouteSync.Ericsson.Entities
{
	public class BranchRouteSettingsConfig : ExtensionConfiguration
	{
		public const string EXTENSION_TYPE = "WhS_RouteSync_EricssonBranchRouteSettings";

		public string Editor { get; set; }
	}
}
