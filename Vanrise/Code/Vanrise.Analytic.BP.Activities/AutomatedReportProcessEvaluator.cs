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
using Vanrise.Entities;

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
                context.WriteBusinessTrackingMsg(LogEntryType.Information, "Started executing the Automated Report process.");
                handler.Settings.Execute(new VRAutomatedReportHandlerExecuteContext(queries, context.GetSharedInstanceData().InstanceInfo.TaskId, new AutomatedReportEvaluatorContext(context)));
            }
        }



    }

    public class AutomatedReportEvaluatorContext : IAutomatedReportEvaluatorContext
    {
        CodeActivityContext context;
        public AutomatedReportEvaluatorContext(CodeActivityContext context)
        {
            this.context = context;
        }
        public void WriteErrorBusinessTrackingMsg(string messageFormat, params object[] args)
        {
            context.WriteBusinessTrackingMsg(LogEntryType.Error, messageFormat, args);
        }

        public void WriteWarningBusinessTrackingMsg(string messageFormat, params object[] args)
        {
            context.WriteBusinessTrackingMsg(LogEntryType.Warning, messageFormat, args);
        }
        public void WriteInformationBusinessTrackingMsg(string messageFormat, params object[] args)
        {
            context.WriteBusinessTrackingMsg(LogEntryType.Information, messageFormat, args);
        }
    }
}
