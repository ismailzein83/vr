using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.Analytic.BP.Arguments
{
    public class VRAutomatedReportProcessInput : BaseProcessInputArgument
    {
        public override string GetTitle()
        {
            return string.Format("Automated Report: {0}", this.Name);
        }
        public string Name { get; set; }

        public List<VRAutomatedReportQuery> Queries { get; set; }

        public VRAutomatedReportHandler Handler { get; set; }
        public override void OnAfterSaveAction(IProcessInputArgumentOnAfterSaveActionContext context)
        {
            if (Handler != null && Handler.Settings != null)
                Handler.Settings.OnAfterSaveAction(new VRAutomatedReportHandlerSettingsOnAfterSaveActionContext
                {
                    TaskId = context.TaskId
                });
        }
        //public override void PrepareArgumentForExecutionFromTask(Vanrise.BusinessProcess.Entities.IProcessInputArgumentPrepareArgumentForExecutionFromTaskContext context)
        //{
        //    this.Queries = null;
        //    this.Handler = null;
        //}
    }
}
