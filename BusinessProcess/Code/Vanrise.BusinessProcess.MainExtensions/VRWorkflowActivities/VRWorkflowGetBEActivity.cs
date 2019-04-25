using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;
namespace Vanrise.BusinessProcess.MainExtensions.VRWorkflowActivities
{
    public class VRWorkflowGetBEActivity : VRWorkflowActivitySettings
    {
        public override Guid ConfigId => new Guid("D92AEEA3-2FCA-4CCC-8BEB-BB5C5181B1F0");

        public override string Editor => "businessprocess-vr-workflowactivity-getbusinessentity";

        public override string Title => "Get Business Object";

        public Guid EntityDefinitionId { get; set; }
        public VRWorkflowExpression EntityId { get; set; }
        public string DisplayName { get; set; }

        public VRWorkflowGetBEActivitySettings Settings { get; set; }

        protected override string InternalGenerateWFActivityCode(IVRWorkflowActivityGenerateWFActivityCodeContext context)
        {
            this.Settings.ThrowIfNull("this.Settings");
            return (new VRWorkflowCustomLogicActivity
            {
                Code = new VRWorkflowCodeExpression
                {
                    CodeExpression = this.Settings.GenerateCode(new VRWorkflowGetBEActivitySettingsGenerateCodeContext(this.EntityDefinitionId, this.EntityId.GetCode(null)))
                }
            }).GenerateWFActivityCode(context);
        }

        private class VRWorkflowGetBEActivitySettingsGenerateCodeContext : IVRWorkflowGetBEActivitySettingsGenerateCodeContext
        {
            public VRWorkflowGetBEActivitySettingsGenerateCodeContext(Guid businessEntityDefinitionId, string entityIdCode)
            {
                this.EntityDefinitionId = businessEntityDefinitionId;
                this.EntityIdCode = entityIdCode;
            }

            public Guid EntityDefinitionId { get; private set; }
            public string EntityIdCode { get; private set; }
        }
    }
}
