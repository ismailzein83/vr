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
    public class GetResults
    {
        private static Form1 f;
        private static readonly object _syncRoot = new object();
        private static bool _locked;

        public void Start(Form1 f)
        {
            GetResults.f = f;
            Thread thread = new Thread(GetResult);
            thread.IsBackground = true;
            thread.Start();
        }
        private void GetResult()
        {
            try
            {
                while (_locked != true)
                {
                    _locked = true;
                    lock (_syncRoot)
                    {
                        //Get Result
                        TestCallManager manager = new TestCallManager();
                        List<TestCallResult> listTestCallResults = manager.GetRequestedTestCallResults();
                        foreach (TestCallResult testCallResult in listTestCallResults)
                            manager.TestCallResult(testCallResult);

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
