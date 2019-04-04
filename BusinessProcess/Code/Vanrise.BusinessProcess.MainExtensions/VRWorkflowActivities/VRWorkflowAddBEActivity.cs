using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.BusinessProcess.MainExtensions.VRWorkflowActivities
{
    public class VRWorkflowAddBEActivity : VRWorkflowActivitySettings
    {
        public override Guid ConfigId => new Guid("14B247CE-109F-4493-8979-57CA3DD090C3");

        public override string Editor => throw new NotImplementedException();

        public override string Title => "Add Business Object";

        public Guid EntityDefinitionId { get; set; }

        public VRWorkflowAddBEActivitySettings Settings { get; set; }

        protected override string InternalGenerateWFActivityCode(IVRWorkflowActivityGenerateWFActivityCodeContext context)
        {
            this.Settings.ThrowIfNull("this.Settings");
            return (new VRWorkflowCustomLogicActivity
            {
                Code = new VRWorkflowCodeExpression
                {
                    CodeExpression = this.Settings.GenerateCode(new VRWorkflowAddBEActivitySettingsGenerateCodeContext(this.EntityDefinitionId))
                }
            }).GenerateWFActivityCode(context);
        }

        private class VRWorkflowAddBEActivitySettingsGenerateCodeContext : IVRWorkflowAddBEActivitySettingsGenerateCodeContext
        {
            public VRWorkflowAddBEActivitySettingsGenerateCodeContext(Guid businessEntityDefinitionId)
            {
                this.EntityDefinitionId = businessEntityDefinitionId;
            }

            public Guid EntityDefinitionId { get; private set; }
        }
    }
}
