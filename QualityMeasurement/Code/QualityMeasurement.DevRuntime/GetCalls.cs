using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using QM.CLITester.Business;
using QM.CLITester.Entities;

namespace QualityMeasurement.DevRuntime
{
    public class GetCalls
    {
        private static bool _locked;
        private static Form1 _form1;
        private static readonly object _syncRoot = new object();

        public void Start(Form1 f)
        {

            _locked = false;
            GetCalls._form1 = f;
            Thread thread = new Thread(new ThreadStart(GetCall));
            thread.IsBackground = true;
            thread.Start();
        }

        ICLITesterConnector cliTestConnector = new QM.CLITester.iTestIntegration.CLITesterConnector();

        private void GetCall()
        {
            try
            {
                while (_locked != true)
                {
                    _locked = true;
                    lock (_syncRoot)
                    {
                        //Get Calls
                        TestCallManager manager = new TestCallManager();
                        List<TestCallResult> listTestCallResults = manager.GetRequestedTestCalls();
                        foreach (TestCallResult testCallResult in listTestCallResults)
                            manager.TestCall(testCallResult);

                        _locked = false;
                        Thread.Sleep(1000);
                    }

                }
            }
            catch (Exception ex)
            {
                
            }
        }
    }
}
