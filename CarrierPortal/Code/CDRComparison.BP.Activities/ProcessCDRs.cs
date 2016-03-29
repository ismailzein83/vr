using CDRComparison.Entities;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess;
using Vanrise.Queueing;

namespace CDRComparison.BP.Activities
{    
    #region Arguments Classes
    public class ProcessCDRsInput
    {
        public BaseQueue<CDRBatch> InputQueue { get; set; }
        public BaseQueue<CDRBatch> OutputQueue { get; set; }

    }

    #endregion
    public class ProcessCDRs : BaseAsyncActivity<ProcessCDRsInput>
    {
        #region Arguments
        [RequiredArgument]
        public InOutArgument<BaseQueue<CDRBatch>> InputQueue { get; set; }
       
        [RequiredArgument]
        public InOutArgument<BaseQueue<CDRBatch>> OutputQueue { get; set; }
        #endregion

        protected override void DoWork(ProcessCDRsInput inputArgument, AsyncActivityHandle handle)
        {
            throw new NotImplementedException();
        }

        protected override ProcessCDRsInput GetInputArgument(AsyncCodeActivityContext context)
        {
            throw new NotImplementedException();
        }
    }
}
