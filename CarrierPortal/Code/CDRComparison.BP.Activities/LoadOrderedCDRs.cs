using CDRComparison.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess;
using Vanrise.Queueing;

namespace CDRComparison.BP.Activities
{
    #region Arguments Classes
    public class LoadOrderedCDRsInput
    {
        public BaseQueue<CDRBatch> OutputQueue { get; set; }
    }

    #endregion
    public sealed class LoadOrderedCDRs : BaseAsyncActivity<LoadOrderedCDRsInput>
    {
        //[RequiredArgument]
        //public InOutArgument<BaseQueue<CancellingStrategyExecutionBatch>> OutputQueue { get; set; }
    }
}
