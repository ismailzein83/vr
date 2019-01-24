using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.Jazz.Entities;

namespace TOne.WhS.Jazz.BP.Activities
{

    public sealed class ProcessJazzReport : CodeActivity
    {
        [RequiredArgument]
        public InArgument<JazzReportDefinition> ReportDefinition { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> FromDate { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> ToDate { get; set; }

        [RequiredArgument]
        public OutArgument<JazzTransactionsReport> JazzTransactionsReport { get; set; }

        [RequiredArgument]
        public OutArgument<JazzReport> JazzReport { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            throw new NotImplementedException();
        }
    }
}
