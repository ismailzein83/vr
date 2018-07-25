using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BusinessProcess.Entities
{
    public class VRWorkflowInputArgument
    {
        public string ProcessName { get { return string.Format("VRWorkflowInputArgument_{0}", BPDefinitionId.ToString("N")); } }

        public Guid BPDefinitionId { get; set; }

        public Dictionary<string, object> Arguments { get; set; }

        //public string EntityId { get; set; }

        public int UserId { get; set; }

        //public abstract string GetTitle();

        public string GetDefinitionTitle()
        {
            return BusinessManagerFactory.GetManager<IBPDefinitionManager>().GetDefinitionTitle(this.ProcessName);
        }

        public void MapExpressionValues(Dictionary<string, object> evaluatedExpressions)
        {

        }

        public void PrepareArgumentForExecutionFromTask(IProcessInputArgumentPrepareArgumentForExecutionFromTaskContext context)
        {

        }
    }
}