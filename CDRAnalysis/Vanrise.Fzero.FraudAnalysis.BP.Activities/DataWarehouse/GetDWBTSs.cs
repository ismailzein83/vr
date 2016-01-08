using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using Vanrise.BusinessProcess;
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
        public DWDimensionDictionary BTSs { get; set; }
    }

    #endregion

    public sealed class GetDWBTSs : BaseAsyncActivity<GetDWBTSsInput, GetDWBTSsOutput>
    {
        #region Arguments

        [RequiredArgument]
        public InOutArgument<DWDimensionDictionary> BTSs { get; set; }

        #endregion



        protected override GetDWBTSsInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new GetDWBTSsInput
            {
            };
        }

        protected override GetDWBTSsOutput DoWorkWithResult(GetDWBTSsInput inputArgument, AsyncActivityHandle handle)
        {
            DWDimensionManager BTSManager = new DWDimensionManager();
            IEnumerable<DWDimension> listBTSs = BTSManager.GetDimensions("Dim_BTS");
            DWDimensionDictionary BTSs = new DWDimensionDictionary();
            if (listBTSs.Count() > 0)
                foreach (var i in listBTSs)
                    BTSs.Add(i.Id, i);

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
