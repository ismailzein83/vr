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

    public class GetDWTimeInput
    {
        public Dictionary<DateTime, DWTime> Times { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

    }

    #endregion

    public sealed class GetDWTime : BaseAsyncActivity<GetDWTimeInput>
    {
        #region Arguments

        [RequiredArgument]
        public InOutArgument<Dictionary<DateTime, DWTime>> Times { get; set; }

       
        [RequiredArgument]
        public InArgument<DateTime> FromDate { get; set; }


        [RequiredArgument]
        public InArgument<DateTime> ToDate { get; set; }


        #endregion

        protected override void DoWork(GetDWTimeInput inputArgument, AsyncActivityHandle handle)
        {
            DWTimeManager timeManager = new DWTimeManager();
            IEnumerable<DWTime> times = timeManager.GetTimes(inputArgument.FromDate, inputArgument.ToDate);
            inputArgument.Times = times.ToDictionary(dim => dim.DateInstance, dim => dim);
        }


        protected override GetDWTimeInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new GetDWTimeInput
            {
                FromDate = this.FromDate.Get(context),
                ToDate = this.ToDate.Get(context),
                Times = this.Times.Get(context)
            };
        }
    }
}
