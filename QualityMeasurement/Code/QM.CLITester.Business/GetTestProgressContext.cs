using QM.CLITester.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.CLITester.Business
{
    public class GetTestProgressContext : IGetTestProgressContext
    {
        public object InitiateTestInformation
        {
            get;
            set;
        }

        public object RecentTestProgress
        {
            get;
            set;
        }

        public Measure RecentMeasure
        {
            get;
            set;
        }
    }
}
