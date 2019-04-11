using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;

namespace Vanrise.BusinessProcess.MainExtensions.VRWorkflowActivities.BEActivities
{
    public class VRWorkflowAddGenericBEActivity : VRWorkflowAddBEActivitySettings
    {
        public List<VRWorkflowAddGenericBEActivityInputItem> InputItems { get; set; }

        public VRWorkflowExpression EntityID { get; set; }

        public VRWorkflowExpression IsSucceeded { get; set; }

        public override string GenerateCode(IVRWorkflowAddBEActivitySettingsGenerateCodeContext context)
        {
            StringBuilder codeBuilder = new StringBuilder();

            codeBuilder.AppendLine("var genericBEManager = new Vanrise.GenericData.Business.GenericBusinessEntityManager();");
            codeBuilder.AppendLine("var itemToAdd = new Vanrise.GenericData.Entities.GenericBusinessEntityToAdd();");
            codeBuilder.AppendLine($@"itemToAdd.BusinessEntityDefinitionId = new Guid(""{context.EntityDefinitionId}"");");
            codeBuilder.AppendLine("itemToAdd.FieldValues = new Dictionary<string, Object>();");
            if (this.InputItems != null)
            {
                foreach (var inputItem in this.InputItems)
                {
                    codeBuilder.AppendLine($@"itemToAdd.FieldValues.Add(""{inputItem.FieldName}"", {inputItem.Value.GetCode(null)});");
                }
            }

            codeBuilder.AppendLine("var insertOutput = genericBEManager.AddGenericBusinessEntity(itemToAdd);");

            if (this.IsSucceeded != null)
            {
                codeBuilder.AppendLine($"{this.IsSucceeded.GetCode(null)} = (insertOutput.Result == Vanrise.Entities.InsertOperationResult.Succeeded);");
            }

            if (this.EntityID != null)
            {
                var idField = new Vanrise.GenericData.Business.GenericBusinessEntityDefinitionManager().GetIdFieldTypeForGenericBE(context.EntityDefinitionId);
                idField.ThrowIfNull("idField", context.EntityDefinitionId);
                codeBuilder.AppendLine(@"if(insertOutput.Result == Vanrise.Entities.InsertOperationResult.Succeeded)
                                         {");

                codeBuilder.AppendLine($@"{this.EntityID.GetCode(null)} = ({CSharpCompiler.TypeToString(idField.Type.GetRuntimeType())})insertOutput.InsertedObject.FieldValues[""{idField.Name}""].Value;");

                codeBuilder.AppendLine(@"}");
            }

            return codeBuilder.ToString();
        }
    }

    public class VRWorkflowAddGenericBEActivityInputItem
    {
        public string FieldName { get; set; }

        public VRWorkflowExpression Value { get; set; }
    }
}
