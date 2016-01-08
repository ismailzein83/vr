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
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }
    }


    public class GetDWTimeOutput
    {
        public DWTimeDictionary Times { get; set; }
    }

    #endregion

    public sealed class GetDWTime : BaseAsyncActivity<GetDWTimeInput, GetDWTimeOutput>
    {
        #region Arguments

        [RequiredArgument]
        public InOutArgument<DWTimeDictionary> Times { get; set; }


        [RequiredArgument]
        public InArgument<DateTime> FromDate { get; set; }


        [RequiredArgument]
        public InArgument<DateTime> ToDate { get; set; }


        #endregion



        protected override GetDWTimeInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new GetDWTimeInput
            {
                FromDate = this.FromDate.Get(context),
                ToDate = this.ToDate.Get(context),
            };
        }

        protected override GetDWTimeOutput DoWorkWithResult(GetDWTimeInput inputArgument, AsyncActivityHandle handle)
        {
            DWTimeManager timeManager = new DWTimeManager();
            IEnumerable<DWTime> times = timeManager.GetTimes(inputArgument.FromDate, inputArgument.ToDate);
            DWTimeDictionary Times = new DWTimeDictionary();
            if (times.Count() > 0)
                foreach (var i in times)
                    Times.Add(i.DateInstance, i);

            return new GetDWTimeOutput
            {
                Times = Times
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, GetDWTimeOutput result)
        {
            this.Times.Set(context, result.Times);
        }
    }
}
