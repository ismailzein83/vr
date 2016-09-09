using QM.CLITester.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.CLITester.Business
{
    public class ConfigManager
    {
        public CLITesterConnectorBase GetCLITesterConnector()
        {
            var connectorType = Vanrise.Common.Utilities.GetAllImplementations<CLITesterConnectorBase>().ToList()[0];
            return Activator.CreateInstance(connectorType) as CLITesterConnectorBase;
        }
    }
}
