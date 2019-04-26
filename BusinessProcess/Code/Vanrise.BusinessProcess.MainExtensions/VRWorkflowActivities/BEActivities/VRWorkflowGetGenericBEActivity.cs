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
        public List<VRWorkflowGetGenericBEActivityOutputItem> OutputItems { get; set; }
        public override string GenerateCode(IVRWorkflowGetBEActivitySettingsGenerateCodeContext context)
        {
            StringBuilder codeBuilder = new StringBuilder();
            codeBuilder.AppendLine("var genericBEManager = new Vanrise.GenericData.Business.GenericBusinessEntityManager();");
            codeBuilder.AppendLine($@"var genericBusinessEntity = genericBEManager.GetGenericBusinessEntity({context.EntityIdCode}, new Guid(""{context.EntityDefinitionId}""));");
            if (OutputItems != null && OutputItems.Count > 0)
            {
                foreach (var outputItem in OutputItems)
                {
                    if (outputItem.Value != null)
                    {
                        codeBuilder.AppendLine($@"{outputItem.Value.GetCode(null)} = genericBusinessEntity.GetRecord({outputItem.FieldName});");

                    }
                }
            }
            return codeBuilder.ToString();
        }
        public class VRWorkflowGetGenericBEActivityOutputItem
        {
            public string FieldName { get; set; }

            public VRWorkflowExpression Value { get; set; }
        }
    }
}
