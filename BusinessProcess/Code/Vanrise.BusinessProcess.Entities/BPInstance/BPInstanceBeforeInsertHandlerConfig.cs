using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.BusinessProcess.Entities
{
    public class BPInstanceBeforeInsertHandlerConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "BusinessProcess_BPInstanceBeforeInsertHandler";
        public string Editor { get; set; }
    }
}
