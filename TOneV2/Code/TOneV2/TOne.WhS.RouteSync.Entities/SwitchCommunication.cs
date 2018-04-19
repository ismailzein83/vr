using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace TOne.WhS.RouteSync.Entities
{
    public abstract class SwitchCommunication
    {
        public abstract Guid ConfigId { get; }

        public bool IsActive { get; set; }
    }

    public class SwitchCommunicationConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "WhS_RouteSync_SwitchCommunication";

        public string Editor { get; set; }
    }
}
