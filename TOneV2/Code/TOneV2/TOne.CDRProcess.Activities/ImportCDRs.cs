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

        private TOne.CDR.Entities.CDRBatch GetCDRsBySwitchId(int SwitchID)
        {
            List<TABS.CDR> ToneCdrs = new List<TABS.CDR>();
            TOne.CDR.Entities.CDRBatch BatchCdrs = new TOne.CDR.Entities.CDRBatch();
            DateTime start = DateTime.Now;
            TABS.Switch CurrentSwitch = null;
            if (TABS.Switch.All.ContainsKey(SwitchID))
                CurrentSwitch = TABS.Switch.All[SwitchID];
            if (CurrentSwitch != null && CurrentSwitch.Enable_CDR_Import && CurrentSwitch.SwitchManager != null)
            {
                var rawCDRs = CurrentSwitch.SwitchManager.GetCDR(CurrentSwitch);

                // create CDRs from Standard CDRs
                foreach (TABS.Addons.Utilities.Extensibility.CDR rawCDR in rawCDRs)
                    ToneCdrs.Add(new TABS.CDR(CurrentSwitch, rawCDR));
            }
            BatchCdrs.CDRs = ToneCdrs;
            Console.WriteLine("{0}: GetCDRs is done in {1}", DateTime.Now, (DateTime.Now - start));
            return BatchCdrs;
        }

        #endregion


        protected override void DoWork(ImportCDRsInput inputArgument, AsyncActivityHandle handle)
        {
            inputArgument.OutputQueue.Enqueue(GetCDRsBySwitchId(inputArgument.SwitchID));
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
