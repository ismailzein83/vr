using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.Jazz.Entities;

namespace TOne.WhS.Jazz.BP.Activities
{

    public sealed class SaveJazzReportDraft : CodeActivity
    {
        public InArgument<JazzTransactionsReport> TransactionsReport { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            //throw new NotImplementedException();
        }
    }


   
   
   
}
