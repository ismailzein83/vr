using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.BP.Arguments;
using Vanrise.Analytic.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Common;

namespace Vanrise.Analytic.BP.Activities
{
    public sealed class AutomatedReportProcessEvaluator : CodeActivity
    {
        #region Arguments

        [RequiredArgument]
        public InArgument<List<VRAutomatedReportQuery>> Queries { get; set; }

        [RequiredArgument]
        public InArgument<VRAutomatedReportHandler> Handler { get; set; }

        #endregion

        protected override void Execute(CodeActivityContext context)
        {
            if (this.Queries != null && this.Handler != null)
            {
                var handler = Handler.Get(context);
                var queries = Queries.Get(context);
                handler.ThrowIfNull("handler");
                handler.Settings.ThrowIfNull("handler.Settings");
                queries.ThrowIfNull("queries");
                handler.Settings.Execute(new VRAutomatedReportHandlerExecuteContext(queries));
            }
        }

    }
}
