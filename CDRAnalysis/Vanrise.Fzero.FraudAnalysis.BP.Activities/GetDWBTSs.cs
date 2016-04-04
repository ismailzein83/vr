using System.Activities;
using System.Collections.Generic;
using System.Linq;
using Vanrise.BusinessProcess;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.Fzero.FraudAnalysis.Business;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.BP.Activities
{
    #region Arguments Classes

    public class GetDWBTSsInput
    {
    }


    public class GetDWBTSsOutput
    {
        public DWBTSDictionary BTSs { get; set; }
    }

    #endregion

    public sealed class GetDWBTSs : BaseAsyncActivity<GetDWBTSsInput, GetDWBTSsOutput>
    {
        #region Arguments

        [RequiredArgument]
        public InOutArgument<DWBTSDictionary> BTSs { get; set; }

        #endregion



        protected override GetDWBTSsInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new GetDWBTSsInput
            {
            };
        }

        protected override GetDWBTSsOutput DoWorkWithResult(GetDWBTSsInput inputArgument, AsyncActivityHandle handle)
        {
            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Started comparing BTSs");
            DWDimensionManager BTSManager = new DWDimensionManager();
            IEnumerable<DWDimension> listBTSs = BTSManager.GetDimensions("Dim_BTS");
            DWBTSDictionary BTSs = new DWBTSDictionary();
            if (listBTSs.Count() > 0)
                foreach (var i in listBTSs)
                    BTSs.Add(i.Description, i);
            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Finished comparing BTSs");
            return new GetDWBTSsOutput
            {
                BTSs = BTSs
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, GetDWBTSsOutput result)
        {
            this.BTSs.Set(context, result.BTSs);
        }
    }
}
