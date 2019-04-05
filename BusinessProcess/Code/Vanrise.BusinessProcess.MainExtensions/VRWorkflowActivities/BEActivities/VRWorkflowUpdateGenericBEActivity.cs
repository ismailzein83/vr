using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;

namespace Vanrise.BusinessProcess.MainExtensions.VRWorkflowActivities.BEActivities
{
    public class VRWorkflowUpdateGenericBEActivity : VRWorkflowUpdateBEActivitySettings
    {
        public List<VRWorkflowUpdateGenericBEActivityInputItem> InputItems { get; set; }

        public VRWorkflowExpression IsSucceeded { get; set; }
        public override string GenerateCode(IVRWorkflowUpdateBEActivitySettingsGenerateCodeContext context)
        {
            StringBuilder codeBuilder = new StringBuilder();

            codeBuilder.AppendLine("var genericBEManager = new Vanrise.GenericData.Business.GenericBusinessEntityManager();");
            codeBuilder.AppendLine("var itemToUpdate = new Vanrise.GenericData.Entities.GenericBusinessEntityToUpdate()");
            codeBuilder.AppendLine($@"itemToUpdate.BusinessEntityDefinitionId = new Guid(""{context.EntityDefinitionId}"");");
            codeBuilder.AppendLine($@"itemToUpdate.GenericBusinessEntityId = {context.EntityIdCode}");

            codeBuilder.AppendLine("itemToUpdate.FieldValues = new Dictionary<string, Object>();");
            if (this.InputItems != null)
            {
                foreach (var inputItem in this.InputItems)
                {
                    codeBuilder.AppendLine($@"itemToUpdate.FieldValues.Add(""{inputItem.FieldName}"", {inputItem.Value.GetCode(null)});");
                }
            }

            codeBuilder.AppendLine("var updateOutput = genericBEManager.UpdateGenericBusinessEntity(itemToUpdate);");

            if (this.IsSucceeded != null)
            {
                codeBuilder.AppendLine($"{this.IsSucceeded.GetCode(null)} = (updateOutput.Result == Vanrise.Entities.UpdateOperationResult.Succeeded);");
            }

            return codeBuilder.ToString();
        }
    }

    public class VRWorkflowUpdateGenericBEActivityInputItem
    {
        public string FieldName { get; set; }

        public VRWorkflowExpression Value { get; set; }
    }
}
