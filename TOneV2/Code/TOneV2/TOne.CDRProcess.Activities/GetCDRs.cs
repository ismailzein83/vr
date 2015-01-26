using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Activities;
using Vanrise.BusinessProcess;
using TOne.Entities;
using System.Collections.Concurrent;
using TABS;
using TOne.CDRProcess.Arguments;
using TOne.Caching;
using Vanrise.Caching;
namespace TOne.CDRProcess.Activities
{


    #region Arguments Classes

    public class GetCDRsInput
    {
        public int SwitchID { get; set; }
        public TOneQueue<CDRBatch> OutputQueue { get; set; }
    }


    #endregion

    public sealed class GetCDRs : BaseAsyncActivity<GetCDRsInput>
    {
        [RequiredArgument]
        public InArgument<int> SwitchID { get; set; }

        [RequiredArgument]
        public InOutArgument<TOneQueue<CDRBatch>> OutputQueue { get; set; }


        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new TOneQueue<List<TABS.CDR>>());
            base.OnBeforeExecute(context, handle);
        }

        protected override GetCDRsInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new GetCDRsInput
            {
                SwitchID = this.SwitchID.Get(context),
                OutputQueue = this.OutputQueue.Get(context)
            };
        }

        protected override void DoWork(GetCDRsInput inputArgument, AsyncActivityHandle handle)
        {
            CDRManager manager = new CDRManager();
            inputArgument.OutputQueue.Enqueue(manager.GetCDRs(inputArgument.SwitchID));
        }
    }


}
