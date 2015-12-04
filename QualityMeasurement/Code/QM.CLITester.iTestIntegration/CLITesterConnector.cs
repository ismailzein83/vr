using QM.CLITester.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.CLITester.iTestIntegration
{
    public class CLITesterConnector : ICLITesterConnector
    {
        public InitiateTestOutput InitiateTest(IInitiateTestContext context)
        {
            throw new NotImplementedException();
        }

        public GetTestProgressOutput GetTestProgress(IGetTestProgressContext context)
        {
            throw new NotImplementedException();
        }
    }
}
