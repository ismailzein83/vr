using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;

namespace TOne.WhS.Deal.BP.Activities
{
    public sealed class CalculateBeginDate : CodeActivity
    {
        public OutArgument<DateTime> BeginDate { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            this.BeginDate.Set(context, DateTime.Now);
        }
    }
}
