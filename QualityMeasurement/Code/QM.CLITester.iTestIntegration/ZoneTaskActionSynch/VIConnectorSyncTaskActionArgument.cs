using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QM.BusinessEntity.Entities;
using QM.CLITester.Entities;

namespace QM.CLITester.iTestIntegration
{
    public class VIConnectorSyncTaskActionArgument : Vanrise.Runtime.Entities.BaseTaskActionArgument
    {
        //public CLITesterConnectorBase CLITestConnector { get; set; }

        public int ParallelThreadsCount { get; set; }

        public int MaximumRetryCount { get; set; }

        public int DownloadResultWaitTime { get; set; }

        public int TimeOut { get; set; }
        public int ProfileId { get; set; }
        public int SupplierId { get; set; }
        public int Quantity { get; set; }
    }
}
