using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Mediation.Generic.Entities
{
    public class MediationOutputHandlerConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "Mediation_MediationOutputHandler";
        public string Editor { get; set; }
    }
}
