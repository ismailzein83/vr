using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;

namespace Vanrise.AccountBalance.BP.Activities
{

    public sealed class GetNextBalanceClosingDate : CodeActivity
    {
        [RequiredArgument]
        public OutArgument<DateTime> NextClosingDate { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            
        }
    }
}
