using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace QM.CLITester.Entities
{
    public class CliTesterConnectorTestProgressConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "QM_CLITester_ConnectorTestProgress";
        public string Editor { get; set; }
    }
}
