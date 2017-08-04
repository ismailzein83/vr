using CDRComparison.Data;
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
    #region Public Classes

    public class PrepareInvalidCDRsForDBApplyInput
    {
        public string TableKey { get; set; }
        public BaseQueue<InvalidCDRBatch> InputQueue { get; set; }
        public BaseQueue<Object> OutputQueue { get; set; }
    }

    #endregion

    public sealed class PrepareInvalidCDRsForDBApply : DependentAsyncActivity<PrepareInvalidCDRsForDBApplyInput>
    {
        #region Arguments

        [RequiredArgument]
        public InArgument<string> TableKey { get; set; }

        [RequiredArgument]
        public InArgument<BaseQueue<InvalidCDRBatch>> InputQueue { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<Object>> OutputQueue { get; set; }

        #endregion

        protected override PrepareInvalidCDRsForDBApplyInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new PrepareInvalidCDRsForDBApplyInput()
            {
                TableKey = this.TableKey.Get(context),
                InputQueue = this.InputQueue.Get(context),
                OutputQueue = this.OutputQueue.Get(context)
            };
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new MemoryQueue<object>());
            base.OnBeforeExecute(context, handle);
        }

        protected override void DoWork(PrepareInvalidCDRsForDBApplyInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            IInvalidCDRDataManager dataManager = CDRComparisonDataManagerFactory.GetDataManager<IInvalidCDRDataManager>();
            dataManager.TableNameKey = inputArgument.TableKey;
            PrepareDataForDBApply(previousActivityStatus, handle, dataManager, inputArgument.InputQueue, inputArgument.OutputQueue, invalidCDRBatch => invalidCDRBatch.InvalidCDRs);
        }
    }
}
