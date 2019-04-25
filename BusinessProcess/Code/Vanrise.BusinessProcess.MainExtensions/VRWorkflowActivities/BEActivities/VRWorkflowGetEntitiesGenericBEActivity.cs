using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.BusinessProcess.MainExtensions.VRWorkflowActivities.BEActivities
{
    public class VRWorkflowGetEntitiesGenericBEActivity : VRWorkflowGetEntitiesBEActivitySettings
    {
        public VRWorkflowExpression GenericBusinessEntities { get; set; }
        public override string GenerateCode(IVRWorkflowGetEntitiesBEActivitySettingsGenerateCodeContext context)
        {
            StringBuilder codeBuilder = new StringBuilder();
            codeBuilder.AppendLine("var genericBEManager = new Vanrise.GenericData.Business.GenericBusinessEntityManager();");
            codeBuilder.AppendLine($@"{this.GenericBusinessEntities.GetCode(null)} = genericBEManager.GetAllGenericBusinessEntities(new Guid(""{context.EntityDefinitionId}""));");
            return codeBuilder.ToString();
        }
    }
}
