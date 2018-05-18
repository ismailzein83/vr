using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace TOne.WhS.RouteSync.Entities
{
    public class SwitchRouteSynchronizerConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "WhS_RouteSync_SwitchRouteSynchronizer";

        public string Editor { get; set; }

		public string SettingsEditor { get; set; }
	}
}
