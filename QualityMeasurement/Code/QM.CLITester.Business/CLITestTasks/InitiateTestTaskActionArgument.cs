using QM.CLITester.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.CLITester.Business
{
    public class InitiateTestTaskActionArgument : Vanrise.Runtime.Entities.BaseTaskActionArgument
    {
        public CLITesterConnectorBase CLITestConnector { get; set; }
    }
}
