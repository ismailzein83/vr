using CDRComparison.Data;
using CDRComparison.Entities;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.Queueing;

namespace CDRComparison.BP.Activities
{
    #region Arguments Classes
    public class LoadOrderedCDRsInput
    {
        public BaseQueue<CDRBatch> OutputQueue { get; set; }
        public string TableKey { get; set; }
    }

    #endregion
    public sealed class LoadOrderedCDRs : BaseAsyncActivity<LoadOrderedCDRsInput>
    {
        #region Arguments
        [RequiredArgument]
        public InOutArgument<BaseQueue<CDRBatch>> OutputQueue { get; set; }
        [RequiredArgument]
        public InArgument<string> TableKey { get; set; }
        #endregion

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new MemoryQueue<CDRBatch>());

            base.OnBeforeExecute(context, handle);
        }
        protected override void DoWork(LoadOrderedCDRsInput inputArgument, AsyncActivityHandle handle)
        {            
            long batchSize = 100000;
            List<CDR> CDRs = new List<CDR>();
            ICDRDataManager dataManager = CDRComparisonDataManagerFactory.GetDataManager<ICDRDataManager>();
            dataManager.TableNameKey = inputArgument.TableKey;
            long batch = 0;
            long totalBatches = 0;
            dataManager.LoadCDRs((cdr) =>
                {
                    batch++;
                    totalBatches++;
                    CDRs.Add(cdr);
                    if (batch == batchSize)
                    {
                        inputArgument.OutputQueue.Enqueue(new CDRBatch() { CDRs = CDRs });
                        Console.WriteLine("{0} CDRs Loaded", totalBatches);
                        batch = 0;
                        CDRs = new List<CDR>();
                    }
                });
            if (CDRs.Count > 0)
            {
                inputArgument.OutputQueue.Enqueue(new CDRBatch() { CDRs = CDRs });
                CDRs = new List<CDR>();
            }
        }
        protected override LoadOrderedCDRsInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new LoadOrderedCDRsInput
            {
                OutputQueue = this.OutputQueue.Get(context),
                TableKey = this.TableKey.Get(context)
            };
        }
    }
}
