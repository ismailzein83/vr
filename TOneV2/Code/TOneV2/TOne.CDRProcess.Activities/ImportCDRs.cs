using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess;
using Vanrise.Queueing;

namespace TOne.CDRProcess.Activities
{
    #region Arguments Classes

    public class ImportCDRsInput
    {
        public int SwitchID { get; set; }

        public BaseQueue<TOne.CDR.Entities.CDRBatch> OutputQueue { get; set; }
    }

    #endregion

    public class ImportCDRs : BaseAsyncActivity<ImportCDRsInput>
    {

        #region Arguments

        [RequiredArgument]
        public InArgument<int> SwitchID { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<TOne.CDR.Entities.CDRBatch>> OutputQueue { get; set; }

        #endregion

        #region Private Methods

        private TOne.CDR.Entities.CDRBatch GetCDRsBySwitchId(int switchID)
        {
            List<TABS.CDR> toneCdrs = new List<TABS.CDR>();
            TOne.CDR.Entities.CDRBatch batchCdrs = new TOne.CDR.Entities.CDRBatch();
            DateTime start = DateTime.Now;
            TABS.Switch currentSwitch = null;
            if (TABS.Switch.All.ContainsKey(switchID))
                currentSwitch = TABS.Switch.All[switchID];
            if (currentSwitch != null && currentSwitch.Enable_CDR_Import && currentSwitch.SwitchManager != null)
            {
                var rawCDRs = currentSwitch.SwitchManager.GetCDR(currentSwitch);

                // create CDRs from Standard CDRs
                foreach (TABS.Addons.Utilities.Extensibility.CDR rawCDR in rawCDRs)
                    toneCdrs.Add(new TABS.CDR(currentSwitch, rawCDR));
            }
            batchCdrs.SwitchId = currentSwitch.SwitchID;
            batchCdrs.CDRs = toneCdrs;
            Console.WriteLine("{0}: GetCDRs is done in {1}", DateTime.Now, (DateTime.Now - start));
            return batchCdrs;
        }

        #endregion



        protected override void DoWork(ImportCDRsInput inputArgument, AsyncActivityHandle handle)
        {
            TOne.CDR.Entities.CDRBatch cdrBatch = GetCDRsBySwitchId(inputArgument.SwitchID);
            if (cdrBatch != null && cdrBatch.CDRs != null && cdrBatch.CDRs.Count > 0)
                inputArgument.OutputQueue.Enqueue(cdrBatch);
        }

        protected override ImportCDRsInput GetInputArgument(System.Activities.AsyncCodeActivityContext context)
        {
            return new ImportCDRsInput
            {
                SwitchID = this.SwitchID.Get(context),
                OutputQueue = this.OutputQueue.Get(context),
            };
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, Vanrise.BusinessProcess.AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new MemoryQueue<TOne.CDR.Entities.CDRBatch>());

            base.OnBeforeExecute(context, handle);
        }

    }
}
