using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.BusinessProcess.MainExtensions.VRWorkflowActivities
{
    public class VRWorkflowUpdateBEActivity : VRWorkflowActivitySettings
    {
        public override Guid ConfigId => new Guid("A3080724-CA0F-45FA-893E-513D9857EFB4");

        public override string Editor => "businessprocess-vr-workflowactivity-updatebusinessentity";

        public override string Title => "Update Business Object";

        public Guid EntityDefinitionId { get; set; }

        public VRWorkflowExpression EntityId { get; set; }

        public string DisplayName { get; set; }

        public VRWorkflowUpdateBEActivitySettings Settings { get; set; }

        protected override string InternalGenerateWFActivityCode(IVRWorkflowActivityGenerateWFActivityCodeContext context)
        {
            this.Settings.ThrowIfNull("this.Settings");
            return (new VRWorkflowCustomLogicActivity
            {
                Code = new VRWorkflowCodeExpression
                {
                    CodeExpression = this.Settings.GenerateCode(new VRWorkflowUpdateBEActivitySettingsGenerateCodeContext(this.EntityDefinitionId, this.EntityId.GetCode(null)))
                }
            }).GenerateWFActivityCode(context);
        }

        private class VRWorkflowUpdateBEActivitySettingsGenerateCodeContext : IVRWorkflowUpdateBEActivitySettingsGenerateCodeContext
        {
            public VRWorkflowUpdateBEActivitySettingsGenerateCodeContext(Guid businessEntityDefinitionId, string entityIdCode)
            {
                this.EntityDefinitionId = businessEntityDefinitionId;
                this.EntityIdCode = entityIdCode;
            }

            public Guid EntityDefinitionId { get; private set; }

            public string EntityIdCode { get; private set; }
        }
    }
}
