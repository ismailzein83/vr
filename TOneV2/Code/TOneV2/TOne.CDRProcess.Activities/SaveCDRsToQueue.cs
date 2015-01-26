using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Entities;
using Vanrise.BusinessProcess;

namespace TOne.CDRProcess.Activities
{
    #region Arguments Classes

    public class SaveCDRsToQueueInput
    {
        public int SwitchID { get; set; }

        public List<CDRBatch> CDRs { get; set; }

        public TOneQueue<List<CDRBatch>> OutputQueue { get; set; }
    }

    #endregion

    public sealed class SaveCDRsToQueue : BaseAsyncActivity<SaveCDRsToQueueInput>
    {
        [RequiredArgument]
        public InArgument<List<CDRBatch>> CDRs { get; set; }

        [RequiredArgument]
        public InArgument<int> SwitchID { get; set; }

        [RequiredArgument]
        public InOutArgument<TOneQueue<List<CDRBatch>>> OutputQueue { get; set; }

        //TOne.Business.SharedQueueManager.GetManager<CDRSharedQueueManager>().GetCDRQueue(inputArgument.SwitchID);


        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.CDRs.Get(context) == null)
                this.OutputQueue.Set(context, new TOneQueue<List<CDRBatch>>());
            base.OnBeforeExecute(context, handle);
        }


        protected override void DoWork(SaveCDRsToQueueInput inputArgument, AsyncActivityHandle handle)
        {
            //CDRSharedQueueManager queue = new CDRSharedQueueManager();
            inputArgument.OutputQueue.Enqueue(inputArgument.CDRs);
        }

        protected override SaveCDRsToQueueInput GetInputArgument(System.Activities.AsyncCodeActivityContext context)
        {
            return new SaveCDRsToQueueInput
            {
                CDRs = this.CDRs.Get(context),
                SwitchID = this.SwitchID.Get(context),
                OutputQueue = this.OutputQueue.Get(context),
            };
        }
    }
}
