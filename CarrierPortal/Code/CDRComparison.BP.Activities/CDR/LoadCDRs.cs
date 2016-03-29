using CDRComparison.Business;
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
    #region Argument Classes

    public class LoadCDRsInput
    {
        public CDRSource CDRSource { get; set; }

        public bool IsPartnerCDRs { get; set; }

        public BaseQueue<CDRBatch> OutputQueue { get; set; }
    }
    
    #endregion

    public sealed class LoadCDRs : BaseAsyncActivity<LoadCDRsInput>
    {
        #region Arguments

        [RequiredArgument]
        public InArgument<CDRSource> CDRSource { get; set; }

        [RequiredArgument]
        public InArgument<bool> IsPartnerCDRs { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<CDRBatch>> OutputQueue { get; set; }
        
        #endregion

        protected override void DoWork(LoadCDRsInput inputArgument, AsyncActivityHandle handle)
        {
            Action<IEnumerable<CDR>> onCDRsReceived = (cdrs) =>
            {
                var list = new List<CDR>();
                foreach (CDR cdr in cdrs)
                {
                    var item = new CDR()
                    {
                        CDPN = cdr.CDPN,
                        CGPN = cdr.CGPN,
                        Time = cdr.Time,
                        DurationInSec = cdr.DurationInSec,
                        IsPartnerCDR = inputArgument.IsPartnerCDRs
                    };
                    list.Add(item);
                }
                var cdrBatch = new CDRBatch() { CDRs = list };
                inputArgument.OutputQueue.Enqueue(cdrBatch);
            };

            var context = new ReadCDRsFromSourceContext(onCDRsReceived);
            inputArgument.CDRSource.ReadCDRs(context);
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new MemoryQueue<CDRBatch>());
            base.OnBeforeExecute(context, handle);
        }

        protected override LoadCDRsInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new LoadCDRsInput()
            {
                CDRSource = this.CDRSource.Get(context),
                IsPartnerCDRs = this.IsPartnerCDRs.Get(context),
                OutputQueue = this.OutputQueue.Get(context)
            };
        }
    }
}
