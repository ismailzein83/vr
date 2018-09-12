using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.BusinessProcess.Entities
{
    public class BPInstanceInsertHandlerConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "BusinessProcess_BPInstanceInsertHandler";
        public string Editor { get; set; }
    }
}
