using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;

namespace Vanrise.BusinessProcess.MainExtensions.VRWorkflowActivities
{
    public class VRWorkflowAssignActivity : VRWorkflowActivitySettings
    {
        public override Guid ConfigId { get { return new Guid("82C21F82-3636-4174-BA83-11709140D959"); } }

        public override string Editor { get { return "businessprocess-vr-workflowactivity-assign"; } }

        public override string Title { get { return "Assign"; } }

        public List<VRWorkflowAssignActivityItem> Items { get; set; }

        protected override string InternalGenerateWFActivityCode(IVRWorkflowActivityGenerateWFActivityCodeContext context)
        {
            StringBuilder codeBuilder = new StringBuilder();
            if (this.Items != null)
            {
                foreach (var item in this.Items)
                {
                    item.To.ThrowIfNull("item.To");
                    item.Value.ThrowIfNull("item.Value", item.To);
                    codeBuilder.Append(item.To.GetCode(null));
                    codeBuilder.Append(" = ");
                    codeBuilder.Append(item.Value.GetCode(null));
                    codeBuilder.Append(";");
                    codeBuilder.AppendLine();
                }
            }
            return (new VRWorkflowCustomLogicActivity { Code = new VRWorkflowCodeExpression { CodeExpression = codeBuilder.ToString() } }).GenerateWFActivityCode(context);
        }
    }

    public class VRWorkflowAssignActivityItem
    {
        [Newtonsoft.Json.JsonConverter(typeof(VRWorkflowExpressionJsonConverter))]
        public VRWorkflowExpression To { get; set; }

        [Newtonsoft.Json.JsonConverter(typeof(VRWorkflowExpressionJsonConverter))]
        public VRWorkflowExpression Value { get; set; }
    }
}
