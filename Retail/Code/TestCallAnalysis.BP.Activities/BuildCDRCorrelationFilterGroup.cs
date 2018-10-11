using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestCallAnalysis.BP.Activities
{
    public sealed class BuildCDRCorrelationFilterGroup : CodeActivity
    {
        [RequiredArgument]
        public InArgument<TimeSpan> DateTimeMargin { get; set; }

        [RequiredArgument]
        public InArgument<TimeSpan> BatchIntervalTime { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            throw new NotImplementedException();
        }
    }
}
