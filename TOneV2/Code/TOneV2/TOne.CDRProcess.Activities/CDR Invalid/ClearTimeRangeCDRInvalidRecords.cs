using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.CDR.Business;
using TOne.CDRProcess.Arguments;
using Vanrise.BusinessProcess;
using Vanrise.Common;

namespace TOne.CDRProcess.Activities
{
    #region Arguments Classes

    public class ClearTimeRangeCDRInvalidRecordsInput
    {
        public TimeRange TimeRange { get; set; }
        public List<string> SupplierIds { get; set; }
        public List<string> CustomersIds { get; set; }
    }

    #endregion
    public sealed class ClearTimeRangeCDRInvalidRecords : BaseAsyncActivity<ClearTimeRangeCDRInvalidRecordsInput>
    {
        [RequiredArgument]
        public InArgument<TimeRange> TimeRange { get; set; }
        public InArgument<List<string>> CustomersIds { get; set; }
        public InArgument<List<string>> SupplierIds { get; set; }

        protected override void DoWork(ClearTimeRangeCDRInvalidRecordsInput inputArgument, AsyncActivityHandle handle)
        {
            DateTime startClearing = DateTime.Now;
            CDRInvalidManager cdrInvalidManager = new CDRInvalidManager();
            DateTime deletionStart = DateTime.Now;
            DateTime from = inputArgument.TimeRange.From;
            DateTime to = inputArgument.TimeRange.To;
            cdrInvalidManager.DeleteCDRInvalid(from, to, inputArgument.CustomersIds, inputArgument.SupplierIds);
            TimeSpan spent = DateTime.Now.Subtract(startClearing);
            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Clear TimeRange CDRInvalid({0:HH:mm}-{1:HH:mm}) done and takes:{2}", from, to, spent);
        }

        protected override ClearTimeRangeCDRInvalidRecordsInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new ClearTimeRangeCDRInvalidRecordsInput
            {
                TimeRange = this.TimeRange.Get(context),
                CustomersIds=this.CustomersIds.Get(context),
                SupplierIds = this.SupplierIds.Get(context)
            };
        }
    }
}
