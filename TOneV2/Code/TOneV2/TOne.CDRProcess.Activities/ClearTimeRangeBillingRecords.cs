using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.Threading.Tasks;
using TOne.CDRProcess.Arguments;
using Vanrise.BusinessProcess;
using Vanrise.Common;

namespace TOne.CDRProcess.Activities
{

    #region Arguments Classes

    public class ClearTimeRangeBillingRecordsInput
    {
        public TimeRange TimeRange { get; set; }

    }

    #endregion

    public sealed class ClearTimeRangeBillingRecords : BaseAsyncActivity<ClearTimeRangeBillingRecordsInput>
    {

        [RequiredArgument]
        public InArgument<TimeRange> TimeRange { get; set; }


        protected override void DoWork(ClearTimeRangeBillingRecordsInput inputArgument, AsyncActivityHandle handle)
        {
            DateTime startClearing = DateTime.Now;
            CDRManager cdrManager = new CDRManager();
            DateTime deletionStart = DateTime.Now;
            DateTime from = inputArgument.TimeRange.From;
            DateTime to = inputArgument.TimeRange.To;
            Action[] deleteActions = new Action[]
                {
                    ()=> cdrManager.DeleteCDRMain(from,to),
                     ()=> cdrManager.DeleteCDRInvalid(from, to),
                     ()=> cdrManager.DeleteCDRCost(from, to), 
                     ()=> cdrManager.DeleteCDRSale(from, to), 
                     ()=> cdrManager.DeleteTrafficStats(from, to)
                };


            Parallel.ForEach(deleteActions, (action) =>
            {
                action();
            });
            TimeSpan spent = DateTime.Now.Subtract(startClearing);
            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Clear TimeRange billings({0:HH:mm}-{1:HH:mm}) done and takes:{2}", from, to, spent);
        }

        protected override ClearTimeRangeBillingRecordsInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new ClearTimeRangeBillingRecordsInput
            {
                TimeRange = this.TimeRange.Get(context)
            };
        }

    }
}
