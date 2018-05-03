using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace TOne.WhS.RouteSync.FreeRadius
{
    public class FreeRadiusPostgresDataManagerConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "WhS_RouteSync_FreeRadiusPostgresDataManager";

        public string Editor { get; set; }
    }
}
