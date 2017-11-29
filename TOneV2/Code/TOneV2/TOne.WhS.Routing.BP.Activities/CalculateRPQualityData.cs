using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.Routing.Business;

namespace TOne.WhS.Routing.BP.Activities
{
    public sealed class CalculateRPQualityData : CodeActivity
    {
        [RequiredArgument]
        public InArgument<DateTime> EffectiveDate { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            DateTime effectiveDate = this.EffectiveDate.Get(context);
        }
    }
}
