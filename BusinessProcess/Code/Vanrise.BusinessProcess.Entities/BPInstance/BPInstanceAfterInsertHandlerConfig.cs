using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.BusinessProcess.Entities
{
    public class BPInstanceAfterInsertHandlerConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "BusinessProcess_BPInstanceAfterInsertHandler";
        public string Editor { get; set; }
    }
}
