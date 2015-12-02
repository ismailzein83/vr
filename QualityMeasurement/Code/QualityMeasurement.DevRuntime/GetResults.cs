using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QualityMeasurement.DevRuntime
{
    public class GetResults
    {
        private static Form1 f;
        private static readonly object _syncRoot = new object();

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
                lock (_syncRoot)
                {

                    //Get Result

                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
