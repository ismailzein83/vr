using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;

namespace Vanrise.BusinessProcess.MainExtensions.VRWorkflowActivities
{
    public class VRWorkflowGetEntitiesBEActivity : VRWorkflowActivitySettings
    {
        public override Guid ConfigId => new Guid("CC48E833-AC65-4988-A8B5-D83250E77050");

        public override string Editor => "businessprocess-vr-workflowactivity-getentitiesbusinessentity";

        public override string Title => "GetEntities Business Object";

        public Guid EntityDefinitionId { get; set; }
        public string DisplayName { get; set; }
        public VRWorkflowGetEntitiesBEActivitySettings Settings { get; set; }
        protected override string InternalGenerateWFActivityCode(IVRWorkflowActivityGenerateWFActivityCodeContext context)
        {
            this.Settings.ThrowIfNull("this.Settings");
            return (new VRWorkflowCustomLogicActivity
            {
                Code = new VRWorkflowCodeExpression
                {
                    CodeExpression = this.Settings.GenerateCode(new VRWorkflowGetEntitiesBEActivitySettingsGenerateCodeContext(this.EntityDefinitionId))
                }
            }).GenerateWFActivityCode(context);
        }

        private class VRWorkflowGetEntitiesBEActivitySettingsGenerateCodeContext : IVRWorkflowGetEntitiesBEActivitySettingsGenerateCodeContext
        {
            public VRWorkflowGetEntitiesBEActivitySettingsGenerateCodeContext(Guid businessEntityDefinitionId)
            {
                this.EntityDefinitionId = businessEntityDefinitionId;
            }

            public Guid EntityDefinitionId { get; private set; }
        }
    }
}
