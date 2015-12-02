using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QualityMeasurement.DevRuntime
{
    public class GetCalls
    {
        private static bool _locked;
        private static Form1 _form1;

        public void Start(Form1 f)
        {
            _locked = false;
            _form1 = f;
            Thread thread = new Thread(GetCall);
            thread.IsBackground = true;
            thread.Start();
        }

        private void GetCall()
        {
            try
            {
                while (_locked != true)
                {
                    _locked = true;

                    //Get Calls

                    _locked = false;
                    Thread.Sleep(1000);
                }
            }
            catch (Exception ex)
            {
                
            }
        }
    }
}
