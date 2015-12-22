using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QM.CLITester.Entities;

namespace QM.CLITester.Business
{
    public class TestCallTaskActionArgument : Vanrise.Runtime.Entities.BaseTaskActionArgument
    {
        public AddTestCallInput TestCallQueryInput { get; set; }
    }
}
