using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace TOne.WhS.RouteSync.FreeRadius
{
    public class FreeRadiusDataManagerConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "WhS_RouteSync_FreeRadiusDataManager";

        public string Editor { get; set; }
    }
}
