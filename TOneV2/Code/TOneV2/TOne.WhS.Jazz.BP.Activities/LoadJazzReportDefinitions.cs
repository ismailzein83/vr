using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.Jazz.Entities;

namespace TOne.WhS.Jazz.BP.Activities
{

    public sealed class LoadJazzReportDefinitions : CodeActivity
    {
        [RequiredArgument]
        public OutArgument<List<JazzReportDefinition>> RepportDefinitions { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            //load only enabled reports
            throw new NotImplementedException();
        }
    }
}
