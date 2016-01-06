using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Fzero.FraudAnalysis.Business;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.BP.Activities
{

    public class GetTimeDimensions : CodeActivity
    {

        #region Arguments

        [RequiredArgument]
        public InArgument<DateTime> FromDate { get; set; }


        [RequiredArgument]
        public InArgument<DateTime> ToDate { get; set; }

        #endregion

        protected override void Execute(CodeActivityContext context)
        {
            DWTimeManager timeManager = new DWTimeManager();
            IEnumerable<Time> listDWTimes = timeManager.GetTimes(this.FromDate.Get(context), this.ToDate.Get(context));
            Dictionary<DateTime, Time> dictDWTimes = listDWTimes.ToDictionary(dim => dim.DateInstance, dim => dim);


        }
    }
}
