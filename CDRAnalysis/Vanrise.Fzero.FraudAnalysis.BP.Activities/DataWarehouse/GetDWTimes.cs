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

    public class GetDWTimesInput
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }
    }


    public class GetDWTimesOutput
    {
        public DWTimeDictionary Times { get; set; }
    }

    #endregion

    public sealed class GetDWTimes : BaseAsyncActivity<GetDWTimesInput, GetDWTimesOutput>
    {
        #region Arguments

        [RequiredArgument]
        public InOutArgument<DWTimeDictionary> Times { get; set; }


        [RequiredArgument]
        public InArgument<DateTime> FromDate { get; set; }


        [RequiredArgument]
        public InArgument<DateTime> ToDate { get; set; }


        #endregion



        protected override GetDWTimesInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new GetDWTimesInput
            {
                FromDate = this.FromDate.Get(context),
                ToDate = this.ToDate.Get(context),
            };
        }

        protected override GetDWTimesOutput DoWorkWithResult(GetDWTimesInput inputArgument, AsyncActivityHandle handle)
        {
            DWTimeManager timeManager = new DWTimeManager();
            IEnumerable<DWTime> times = timeManager.GetTimes(inputArgument.FromDate, inputArgument.ToDate);
            DWTimeDictionary Times = new DWTimeDictionary();
            if (times.Count() > 0)
                foreach (var i in times)
                    Times.Add(i.DateInstance, i);

            return new GetDWTimesOutput
            {
                Times = Times
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, GetDWTimesOutput result)
        {
            this.Times.Set(context, result.Times);
        }
    }
}
