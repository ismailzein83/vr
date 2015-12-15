using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QM.CLITester.Entities;

namespace QM.CLITester.Business
{
    public class TestProgressTaskActionArgument : Vanrise.Runtime.Entities.BaseTaskActionArgument
    {
        public CLITesterConnectorBase CLITestConnector { get; set; }
    }
}
