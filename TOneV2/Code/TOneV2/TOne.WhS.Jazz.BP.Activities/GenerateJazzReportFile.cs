using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Jazz.BP.Activities
{
    public sealed class GenerateJazzReportFile : CodeActivity
    {
        public InArgument<List<JazzReport>> JazzReports { get; set; }
        public OutArgument<int> FileId { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            throw new NotImplementedException();
        }
    }
}
