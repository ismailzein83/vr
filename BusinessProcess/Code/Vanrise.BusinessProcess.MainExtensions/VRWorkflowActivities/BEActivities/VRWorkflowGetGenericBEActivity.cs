using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.BusinessProcess.MainExtensions.VRWorkflowActivities.BEActivities
{
    public class VRWorkflowGetGenericBEActivity : VRWorkflowGetBEActivitySettings
    {
        public VRWorkflowExpression GenericBusinessEntity { get; set; }
        public override string GenerateCode(IVRWorkflowGetBEActivitySettingsGenerateCodeContext context)
        {
            StringBuilder codeBuilder = new StringBuilder();
            codeBuilder.AppendLine("var genericBEManager = new Vanrise.GenericData.Business.GenericBusinessEntityManager();");
            codeBuilder.AppendLine($@"{this.GenericBusinessEntity.GetCode(null)} = genericBEManager.GetGenericBusinessEntity({context.EntityIdCode}, new Guid(""{context.EntityDefinitionId}""));");
            return codeBuilder.ToString();
        }
    }
}
